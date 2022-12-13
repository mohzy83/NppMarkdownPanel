using Kbg.NppPluginNET.PluginInfrastructure;
using NppMarkdownPanel.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics; // for Process()
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel
{
    public class MarkdownPanelController
    {
        private MarkdownPreviewForm markdownPreviewForm;
        private Timer renderTimer;

        private int idMyDlg = -1;

        private const int Unused = 0;

        private const int renderRefreshRateMilliSeconds = 500;
        private const int inputUpdateThresholdMiliseconds = 400;
        private int lastTickCount = 0;

        private bool isPanelVisible;

        private readonly Func<IScintillaGateway> scintillaGatewayFactory;
        private readonly INotepadPPGateway notepadPPGateway;
        private int lastCaretPosition;
        private string iniFilePath;
        private bool syncViewWithCaretPosition;
        private bool syncViewWithScrollPosition;

        private bool autoShowPanel = false;
        private bool nppReady;

        public const string DEFAULT_SUPPORTED_MKDN_EXT = "md,mkd,mdwn,mdown,mdtxt,markdown";
        public const string DEFAULT_SUPPORTED_HTML_EXT = "html,htm";

        private const int FILTERS = 10;
        private string[] filterExts = new string[FILTERS];
        private string[] filterProgs = new string[FILTERS];
        private string[] filterArgs = new string[FILTERS];
        private int filtersFound = 0;

        public MarkdownPanelController()
        {
            scintillaGatewayFactory = PluginBase.GetGatewayFactory();
            notepadPPGateway = new NotepadPPGateway();
            markdownPreviewForm = new MarkdownPreviewForm(ToolWindowCloseAction);
            renderTimer = new Timer();
            renderTimer.Interval = renderRefreshRateMilliSeconds;
            renderTimer.Tick += OnRenderTimerElapsed;
        }

        public void OnNotification(ScNotification notification)
        {
            if (isPanelVisible)
            {
                if (notification.Header.Code == (uint)SciMsg.SCN_UPDATEUI)
                {
                    var scintillaGateway = scintillaGatewayFactory();
                    if ( ! (markdownPreviewForm.isValidMkdnExtension() || markdownPreviewForm.isValidHtmlExtension()) )
                        return;

                    var firstVisible = scintillaGateway.GetFirstVisibleLine();
                    var buffer = scintillaGateway.LinesOnScreen()/2;
                    var lastLine = scintillaGateway.GetLineCount();

                    if (syncViewWithCaretPosition && lastCaretPosition != scintillaGateway.GetCurrentPos())
                    {
                        lastCaretPosition = scintillaGateway.GetCurrentPos();
                        if ((scintillaGateway.GetCurrentLineNumber() - buffer) < 0)
                        {
                            ScrollToElementAtLineNo(0);
                        }
                        else
                        {
                            ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber() - buffer);
                        }
                    }
                    else if (syncViewWithScrollPosition && lastCaretPosition != scintillaGateway.GetFirstVisibleLine())
                    {
                        lastCaretPosition = scintillaGateway.GetFirstVisibleLine();
                        var middleLine = lastCaretPosition + buffer;
                        if (scintillaGateway.GetFirstVisibleLine() == 0)
                        {
                            ScrollToElementAtLineNo(0);
                        }
                        else if ((lastCaretPosition + scintillaGateway.LinesOnScreen()) >= lastLine)
                        {
                            ScrollToElementAtLineNo(lastLine);
                        }
                        else
                        {
                            // if ((notification.Updated & (uint)SciMsg.SC_UPDATE_V_SCROLL) == (uint)SciMsg.SC_UPDATE_V_SCROLL)
                            // {
                                ScrollToElementAtLineNo(middleLine - buffer);
                            // }
                            // else
                            // {
                                // ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber() - buffer);
                            // }
                        }
                    }
                }
                else if (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
                {
                    // Focus was switched to a new document
                    var currentFilePath = notepadPPGateway.GetCurrentFilePath();
                    markdownPreviewForm.CurrentFilePath = currentFilePath;

                    // if we get a lot tab switches within a short period, dont update preview
                    RenderMarkdownDeferred();
                }
                else if (notification.Header.Code == (uint)SciMsg.SCN_MODIFIED)
                {
                    if ( markdownPreviewForm.isValidMkdnExtension() || markdownPreviewForm.isValidHtmlExtension() )
                    {
                        lastTickCount = Environment.TickCount;
                        RenderMarkdownDeferred();
                    }
                }
                // NPPN_DARKMODECHANGED (NPPN_FIRST + 27) // To notify plugins that Dark Mode was enabled/disabled                else             
                else if (notification.Header.Code == (uint)(NppMsg.NPPN_FIRST + 27))
                {
                    markdownPreviewForm.IsDarkModeEnabled = IsDarkModeEnabled();
                    RenderMarkdownDirect();
                }
                else if (notification.Header.Code == (uint)NppMsg.NPPN_FILESAVED)
                {
                    RenderMarkdownDirect();
                }
            }

            if (notification.Header.Code == (uint)NppMsg.NPPN_READY)
            {
                nppReady = true;
                var currentFilePath = notepadPPGateway.GetCurrentFilePath();
                AutoShowOrHidePanel(currentFilePath);
            }
        }

        private void RenderMarkdownDeferred()
        {
            // if we get a lot of key strokes within a short period, dont update preview
            var currentDeltaMilliseconds = Environment.TickCount - lastTickCount;
            if (currentDeltaMilliseconds < inputUpdateThresholdMiliseconds)
            {
                // Reset Timer
                renderTimer.Stop();
            }
            renderTimer.Start();
            lastTickCount = Environment.TickCount;
        }

        private void OnRenderTimerElapsed(object source, EventArgs e)
        {
            renderTimer.Stop();
            try
            {
                RenderMarkdownDirect();
            }
            catch
            {
            }
        }

        private void RenderMarkdownDirect(bool preserveVerticalScrollPosition = true)
        {
            if (markdownPreviewForm.isValidMkdnExtension())
                markdownPreviewForm.RenderMarkdown(GetCurrentEditorText(), notepadPPGateway.GetCurrentFilePath(), preserveVerticalScrollPosition);
            else if (markdownPreviewForm.isValidHtmlExtension())
                markdownPreviewForm.RenderHtml(GetCurrentEditorText(), notepadPPGateway.GetCurrentFilePath(), preserveVerticalScrollPosition);
            else
            {
                int filter = ValidateFilterExtension();
                if ( filter < 0 )
                {
                    markdownPreviewForm.RenderMarkdown($"Not a valid Markdown file extension: {markdownPreviewForm.MkdnExtensions}\n\nNot a valid HTML file extension: {markdownPreviewForm.HtmlExtensions}", notepadPPGateway.GetCurrentFilePath(), false);
                    return;
                }

                var filterProgram = filterProgs[filter];
                var filterArguments = filterArgs[filter];
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filterProgram,
                        Arguments = $"{filterArguments} \"{notepadPPGateway.GetCurrentFilePath()}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string data = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                markdownPreviewForm.RenderHtml(data, notepadPPGateway.GetCurrentFilePath(), false);
            }
        }

        private string GetCurrentEditorText()
        {
            var scintillaGateway = scintillaGatewayFactory();
            return scintillaGateway.GetText(scintillaGateway.GetLength() + 1);
        }

        private void ScrollToElementAtLineNo(int lineNo)
        {
            var scintillaGateway = scintillaGatewayFactory();
            if ( markdownPreviewForm.isValidMkdnExtension() )
                markdownPreviewForm.ScrollToElementWithLineNo(lineNo);
            else
            {
                var lastLine = scintillaGateway.GetLineCount();
                double percent = (double)lineNo / lastLine;
                markdownPreviewForm.ScrollToHtmlLineNo(percent);
            }
        }

        public void InitCommandMenu()
        {
            SetIniFilePath();
            syncViewWithCaretPosition = (Win32.GetPrivateProfileInt("Options", "SyncViewWithCaretPosition", 0, iniFilePath) != 0);
            syncViewWithScrollPosition = (Win32.GetPrivateProfileInt("Options", "SyncViewWithScrollPosition", 0, iniFilePath) != 0);
            markdownPreviewForm.CssFileName = Win32.ReadIniValue("Options", "CssFileName", iniFilePath, "style.css");
            markdownPreviewForm.CssDarkModeFileName = Win32.ReadIniValue("Options", "CssDarkModeFileName", iniFilePath, "style-dark.css");
            markdownPreviewForm.ZoomLevel = Win32.GetPrivateProfileInt("Options", "ZoomLevel", 130, iniFilePath);
            markdownPreviewForm.HtmlFileName = Win32.ReadIniValue("Options", "HtmlFileName", iniFilePath);
            markdownPreviewForm.ShowToolbar = Utils.ReadIniBool("Options", "ShowToolbar", iniFilePath);
            markdownPreviewForm.MkdnExtensions = Win32.ReadIniValue("Options", "MkdnExtensions", iniFilePath, DEFAULT_SUPPORTED_MKDN_EXT);
            markdownPreviewForm.HtmlExtensions = Win32.ReadIniValue("Options", "HtmlExtensions", iniFilePath, DEFAULT_SUPPORTED_HTML_EXT);
            autoShowPanel = Utils.ReadIniBool("Options", "AutoShowPanel", iniFilePath);

            for ( int i = 0; i < FILTERS; i++ )
            {
                var section = $"Filter{i}";
                filterExts[i]  = Win32.ReadIniValue(section, "Extensions", iniFilePath, "!!!");
                filterProgs[i] = Win32.ReadIniValue(section, "Program", iniFilePath, "!!!");
                filterArgs[i]  = Win32.ReadIniValue(section, "Arguments", iniFilePath, "!!!");
                if ( filterExts[i].Contains("!!!") )
                    break;
                filtersFound++;
            }
            markdownPreviewForm.IsDarkModeEnabled = IsDarkModeEnabled();
            PluginBase.SetCommand(0, "Toggle &Markdown Panel", TogglePanelVisible);
            PluginBase.SetCommand(1, "---", null);
            PluginBase.SetCommand(2, "Synchronize with &caret position", SyncViewWithCaret, syncViewWithCaretPosition);
            PluginBase.SetCommand(3, "Synchronize on &vertical scroll", SyncViewWithScroll, syncViewWithScrollPosition);
            PluginBase.SetCommand(4, "---", null);
            PluginBase.SetCommand(5, "&Settings", EditSettings);
            PluginBase.SetCommand(6, "&About", ShowAboutDialog, new ShortcutKey(false, false, false, Keys.None));
            idMyDlg = 0;
        }


        private void EditSettings()
        {
            var settingsForm = new SettingsForm(markdownPreviewForm.ZoomLevel, markdownPreviewForm.CssFileName, markdownPreviewForm.HtmlFileName, markdownPreviewForm.ShowToolbar, markdownPreviewForm.CssDarkModeFileName, markdownPreviewForm.MkdnExtensions, markdownPreviewForm.HtmlExtensions, autoShowPanel);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                markdownPreviewForm.CssFileName = settingsForm.CssFileName;
                markdownPreviewForm.CssDarkModeFileName = settingsForm.CssDarkModeFileName;
                markdownPreviewForm.ZoomLevel = settingsForm.ZoomLevel;
                markdownPreviewForm.HtmlFileName = settingsForm.HtmlFileName;
                markdownPreviewForm.ShowToolbar = settingsForm.ShowToolbar;
                markdownPreviewForm.MkdnExtensions = settingsForm.MkdnExtensions;
                markdownPreviewForm.HtmlExtensions = settingsForm.HtmlExtensions;
                autoShowPanel = settingsForm.AutoShowPanel;
                markdownPreviewForm.IsDarkModeEnabled = IsDarkModeEnabled();
                SaveSettings();
                //Update Preview
                RenderMarkdownDirect();
            }
        }

        private void SetIniFilePath()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, Main.PluginFilename + ".ini");
        }

        private void SyncViewWithCaret()
        {
            var scintillaGateway = scintillaGatewayFactory();
            syncViewWithCaretPosition = !syncViewWithCaretPosition;
            syncViewWithScrollPosition = false;
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[3]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithScrollPosition ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[2]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithCaretPosition ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            if (syncViewWithCaretPosition) ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber());
        }

        private void SyncViewWithScroll()
        {
            var scintillaGateway = scintillaGatewayFactory();
            syncViewWithScrollPosition = !syncViewWithScrollPosition;
            syncViewWithCaretPosition = false;
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[2]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithCaretPosition ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[3]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithScrollPosition ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            if (syncViewWithScrollPosition) ScrollToElementAtLineNo(scintillaGateway.GetFirstVisibleLine());
        }

        public void SetToolBarIcon()
        {
            toolbarIcons tbIconsOld = new toolbarIcons();
            tbIconsOld.hToolbarBmp = Properties.Resources.markdown_16x16_solid.GetHbitmap();
            tbIconsOld.hToolbarIcon = Properties.Resources.markdown_16x16_solid_dark.GetHicon();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIconsOld));
            Marshal.StructureToPtr(tbIconsOld, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        public void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("Options", "SyncViewWithCaretPosition", syncViewWithCaretPosition ? "1" : "0", iniFilePath);
            Win32.WritePrivateProfileString("Options", "SyncViewWithScrollPosition", syncViewWithScrollPosition ? "1" : "0", iniFilePath);
            SaveSettings();
        }

        private void SaveSettings()
        {
            Win32.WriteIniValue("Options", "CssFileName", markdownPreviewForm.CssFileName, iniFilePath);
            Win32.WriteIniValue("Options", "CssDarkModeFileName", markdownPreviewForm.CssDarkModeFileName, iniFilePath);
            Win32.WriteIniValue("Options", "ZoomLevel", markdownPreviewForm.ZoomLevel.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "HtmlFileName", markdownPreviewForm.HtmlFileName, iniFilePath);
            Win32.WriteIniValue("Options", "ShowToolbar", markdownPreviewForm.ShowToolbar.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "MkdnExtensions", markdownPreviewForm.MkdnExtensions, iniFilePath);
            Win32.WriteIniValue("Options", "HtmlExtensions", markdownPreviewForm.HtmlExtensions, iniFilePath);
            Win32.WriteIniValue("Options", "AutoShowPanel", autoShowPanel.ToString(), iniFilePath);
        }
        private void ShowAboutDialog()
        {
            var aboutDialog = new AboutForm();
            aboutDialog.ShowDialog();
        }

        private bool initDialog;

        private void TogglePanelVisible()
        {
            if (!initDialog)
            {
                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = markdownPreviewForm.Handle;
                _nppTbData.pszName = Main.PluginName;
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)ConvertBitmapToIcon(Properties.Resources.markdown_16x16_solid_bmp).Handle;
                _nppTbData.pszModuleName = Main.PluginFilename;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                initDialog = true;
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, !isPanelVisible ? (uint)NppMsg.NPPM_DMMSHOW : (uint)NppMsg.NPPM_DMMHIDE, 0, markdownPreviewForm.Handle);
            }
            isPanelVisible = !isPanelVisible;
            if (isPanelVisible)
            {
                var currentFilePath = notepadPPGateway.GetCurrentFilePath();
                markdownPreviewForm.CurrentFilePath = currentFilePath;
                RenderMarkdownDirect();
            }
        }

        private Icon ConvertBitmapToIcon(Bitmap bitmapImage)
        {
            using (Bitmap newBmp = new Bitmap(16, 16))
            {
                Graphics g = Graphics.FromImage(newBmp);
                ColorMap[] colorMap = new ColorMap[1];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.Fuchsia;
                colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                g.DrawImage(bitmapImage, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                return Icon.FromHandle(newBmp.GetHicon());
            }
        }

        /// <summary>
        /// Actions to do after the tool window was closed
        /// </summary>
        private void ToolWindowCloseAction()
        {
            TogglePanelVisible();
        }

        public static IMarkdownGenerator GetMarkdownGeneratorImpl()
        {
            return new MarkdigWrapperMarkdownGenerator();
        }

        private bool IsDarkModeEnabled()
        {
            // NPPM_ISDARKMODEENABLED (NPPMSG + 107)
            IntPtr ret = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)(Constants.NPPMSG + 107), Unused, Unused);
            return ret.ToInt32() == 1;
        }


        private void AutoShowOrHidePanel(string currentFilePath)
        {
            if (nppReady && autoShowPanel)
            {
                // automatically show panel for supported file types
                // if ((!isPanelVisible && markdownPreviewForm.isValidFileExtension(currentFilePath)) ||
                    // (isPanelVisible && !markdownPreviewForm.isValidFileExtension(currentFilePath)))
                // {
                    // TogglePanelVisible();
                // }
            }
        }

        private int ValidateFilterExtension()
        {
            StringBuilder sbFileExtension = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEXTPART, Win32.MAX_PATH, sbFileExtension);
            var currentExtension = sbFileExtension.ToString().ToLower();
            if ( String.IsNullOrEmpty(currentExtension) )
                return -1;

            for ( int i = 0; i < filtersFound; i++ )
            {
                // if (filterExts[i].Contains(currentExtension.ToLower()))
                    // return i;
                var matchExtensionList = false;
                try
                {
                    matchExtensionList = filterExts[i].Split(',').Any(ext => ext != null && currentExtension.Equals("." + ext.Trim().ToLower()));
                }
                catch (Exception)
                {
                }
                if ( matchExtensionList )
                    return i;
            }
            return -1;
        }
    }
}
