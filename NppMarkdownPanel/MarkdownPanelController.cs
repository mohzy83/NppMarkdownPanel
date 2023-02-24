using Kbg.NppPluginNET.PluginInfrastructure;
using NppMarkdownPanel.Entities;
using NppMarkdownPanel.Forms;
using NppMarkdownPanel.Generator;
using System;
using System.Collections.Generic;
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
        private IViewerInterface viewerInterface;
        private Timer renderTimer;

        private int idMyDlg = -1;

        private const int Unused = 0;

        private const int renderRefreshRateMilliSeconds = 500;
        private const int inputUpdateThresholdMiliseconds = 400;
        private int lastTickCount = 0;

        private bool isPanelVisible;

        private readonly Func<IScintillaGateway> scintillaGatewayFactory;
        private readonly INotepadPPGateway notepadPPGateway;

        private string iniFilePath;

        private int lastCaretPosition;
        private bool syncViewWithCaretPosition;

        private int currentFirstVisibleLine;
        private bool syncViewWithFirstVisibleLine;

        private bool nppReady;
        private Settings settings;

        public MarkdownPanelController()
        {
            scintillaGatewayFactory = PluginBase.GetGatewayFactory();
            notepadPPGateway = new NotepadPPGateway();
            SetIniFilePath();
            settings = LoadSettingsFromIni();
            viewerInterface = MarkdownPreviewForm.InitViewer(settings, HandleWndProc);
            renderTimer = new Timer();
            renderTimer.Interval = renderRefreshRateMilliSeconds;
            renderTimer.Tick += OnRenderTimerElapsed;
        }

        private Settings LoadSettingsFromIni()
        {
            Settings settings = new Settings();
            settings.PreProcessorCommandFilename = Win32.ReadIniValue("Options", "PreProcessorExe", iniFilePath, "");
            settings.PreProcessorArguments = Win32.ReadIniValue("Options", "PreProcessorArguments", iniFilePath, "");
            settings.PostProcessorCommandFilename = Win32.ReadIniValue("Options", "PostProcessorExe", iniFilePath, "");
            settings.PostProcessorArguments = Win32.ReadIniValue("Options", "PostProcessorArguments", iniFilePath, "");
            settings.CssFileName = Win32.ReadIniValue("Options", "CssFileName", iniFilePath, "style.css");
            settings.CssDarkModeFileName = Win32.ReadIniValue("Options", "CssDarkModeFileName", iniFilePath, "style-dark.css");
            settings.ZoomLevel = Win32.GetPrivateProfileInt("Options", "ZoomLevel", 130, iniFilePath);
            settings.HtmlFileName = Win32.ReadIniValue("Options", "HtmlFileName", iniFilePath);
            settings.ShowToolbar = PluginUtils.ReadIniBool("Options", "ShowToolbar", iniFilePath);
            settings.ShowStatusbar = PluginUtils.ReadIniBool("Options", "ShowStatusbar", iniFilePath);
            settings.SupportedFileExt = Win32.ReadIniValue("Options", "SupportedFileExt", iniFilePath, Settings.DEFAULT_SUPPORTED_FILE_EXT);
            settings.IsDarkModeEnabled = IsDarkModeEnabled();
            settings.AutoShowPanel = PluginUtils.ReadIniBool("Options", "AutoShowPanel", iniFilePath);
            return settings;
        }

        public void OnNotification(ScNotification notification)
        {
            if (isPanelVisible && notification.Header.Code == (uint)SciMsg.SCN_UPDATEUI)
            {
                var scintillaGateway = scintillaGatewayFactory();
                if (syncViewWithCaretPosition)
                {
                    if (lastCaretPosition != scintillaGateway.GetCurrentPos())
                    {
                        lastCaretPosition = scintillaGateway.GetCurrentPos();
                        ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber());
                    }
                }
                else if (syncViewWithFirstVisibleLine)
                {
                    if (currentFirstVisibleLine != scintillaGateway.GetFirstVisibleLine())
                    {
                        var firstVisibleLine = scintillaGateway.GetFirstVisibleLine();
                        currentFirstVisibleLine = firstVisibleLine;
                        ScrollToElementAtLineNo(firstVisibleLine);
                    }
                }
            }
            if (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
            {
                // Focus was switched to a new document
                var currentFilePath = notepadPPGateway.GetCurrentFilePath();
                viewerInterface.SetMarkdownFilePath(currentFilePath);
                if (isPanelVisible)
                {
                    RenderMarkdownDirect(false);
                }
                AutoShowOrHidePanel(currentFilePath);
            }
            // NPPN_DARKMODECHANGED (NPPN_FIRST + 27) // To notify plugins that Dark Mode was enabled/disabled
            if (notification.Header.Code == (uint)(NppMsg.NPPN_FIRST + 27))
            {
                settings.IsDarkModeEnabled = IsDarkModeEnabled();
                viewerInterface.UpdateSettings(settings);
                if (isPanelVisible) RenderMarkdownDirect();
            }
            if (isPanelVisible && notification.Header.Code == (uint)SciMsg.SCN_MODIFIED)
            {
                lastTickCount = Environment.TickCount;
                RenderMarkdownDeferred();
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
            // if we get a lot of key stroks within a short period, dont update preview
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
            viewerInterface.RenderMarkdown(GetCurrentEditorText(), notepadPPGateway.GetCurrentFilePath(), preserveVerticalScrollPosition);
        }

        private string GetCurrentEditorText()
        {
            var scintillaGateway = scintillaGatewayFactory();
            return scintillaGateway.GetText(scintillaGateway.GetLength() + 1);
        }

        private void ScrollToElementAtLineNo(int lineNo)
        {
            viewerInterface.ScrollToElementWithLineNo(lineNo);
        }

        public void InitCommandMenu()
        {
            syncViewWithCaretPosition = (Win32.GetPrivateProfileInt("Options", "SyncViewWithCaretPosition", 0, iniFilePath) != 0);
            syncViewWithFirstVisibleLine = (Win32.GetPrivateProfileInt("Options", "SyncWithFirstVisibleLine", 0, iniFilePath) != 0);
            PluginBase.SetCommand(0, "Toggle &Markdown Panel", TogglePanelVisible);
            PluginBase.SetCommand(1, "---", null);
            PluginBase.SetCommand(2, "Synchronize with &caret position", SyncViewWithCaret, syncViewWithCaretPosition);
            PluginBase.SetCommand(3, "Synchronize with &first visible line in editor", SyncViewWithFirstVisibleLine, syncViewWithFirstVisibleLine);
            PluginBase.SetCommand(4, "---", null);
            PluginBase.SetCommand(5, "&Settings", EditSettings);
            PluginBase.SetCommand(6, "&Help", ShowHelp);
            PluginBase.SetCommand(7, "&About", ShowAboutDialog);
            idMyDlg = 0;
        }

        private void EditSettings()
        {
            var settingsForm = new SettingsForm(settings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settings.CssFileName = settingsForm.CssFileName;
                settings.CssDarkModeFileName = settingsForm.CssDarkModeFileName;
                settings.ZoomLevel = settingsForm.ZoomLevel;
                settings.HtmlFileName = settingsForm.HtmlFileName;
                settings.ShowToolbar = settingsForm.ShowToolbar;
                settings.SupportedFileExt = settingsForm.SupportedFileExt;
                settings.ShowStatusbar = settingsForm.ShowStatusbar;
                settings.AutoShowPanel = settingsForm.AutoShowPanel;

                settings.IsDarkModeEnabled = IsDarkModeEnabled();
                viewerInterface.UpdateSettings(settings);
                SaveSettings();
                //Update Preview
                if (isPanelVisible) RenderMarkdownDirect();
            }
        }

        private void ShowHelp()
        {
            var currentPluginPath = PluginUtils.GetPluginDirectory();
            var helpFile = Path.Combine(currentPluginPath, "README.md");
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, 0, helpFile);
            if (!isPanelVisible)
                TogglePanelVisible();
            RenderMarkdownDirect();
        }

        private void SetIniFilePath()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, Main.ModuleName + ".ini");
        }

        private void SyncViewWithCaret()
        {
            syncViewWithCaretPosition = !syncViewWithCaretPosition;
            if (syncViewWithCaretPosition && syncViewWithFirstVisibleLine)
                // Disable syncWithFirstVisibleLine
                SyncViewWithFirstVisibleLine();
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[2]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithCaretPosition ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            var scintillaGateway = scintillaGatewayFactory();
            if (syncViewWithCaretPosition) ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber());
        }

        private void SyncViewWithFirstVisibleLine()
        {
            syncViewWithFirstVisibleLine = !syncViewWithFirstVisibleLine;
            if (syncViewWithFirstVisibleLine && syncViewWithCaretPosition)
                // Disable syncViewWithCaretPosition
                SyncViewWithCaret();
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[3]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithFirstVisibleLine ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            var scintillaGateway = scintillaGatewayFactory();
            if (syncViewWithFirstVisibleLine) ScrollToElementAtLineNo(scintillaGateway.GetFirstVisibleLine());
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
            Win32.WritePrivateProfileString("Options", "SyncWithFirstVisibleLine", syncViewWithFirstVisibleLine ? "1" : "0", iniFilePath);
            SaveSettings();
        }

        private void SaveSettings()
        {
            Win32.WriteIniValue("Options", "CssFileName", settings.CssFileName, iniFilePath);
            Win32.WriteIniValue("Options", "CssDarkModeFileName", settings.CssDarkModeFileName, iniFilePath);
            Win32.WriteIniValue("Options", "ZoomLevel", settings.ZoomLevel.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "HtmlFileName", settings.HtmlFileName, iniFilePath);
            Win32.WriteIniValue("Options", "ShowToolbar", settings.ShowToolbar.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "ShowStatusbar", settings.ShowStatusbar.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "SupportedFileExt", settings.SupportedFileExt, iniFilePath);
            Win32.WriteIniValue("Options", "AutoShowPanel", settings.AutoShowPanel.ToString(), iniFilePath);
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
                _nppTbData.hClient = viewerInterface.Handle;
                _nppTbData.pszName = Main.PluginTitle;
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)ConvertBitmapToIcon(Properties.Resources.markdown_16x16_solid_bmp).Handle;
                _nppTbData.pszModuleName = Main.ModuleName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                initDialog = true;
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, !isPanelVisible ? (uint)NppMsg.NPPM_DMMSHOW : (uint)NppMsg.NPPM_DMMHIDE, 0, viewerInterface.Handle);
            }
            isPanelVisible = !isPanelVisible;
            if (isPanelVisible)
            {
                var currentFilePath = notepadPPGateway.GetCurrentFilePath();
                viewerInterface.SetMarkdownFilePath(currentFilePath);
                viewerInterface.UpdateSettings(settings);
                RenderMarkdownDirect(false);
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

        private bool IsDarkModeEnabled()
        {
            // NPPM_ISDARKMODEENABLED (NPPMSG + 107)
            IntPtr ret = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)(Constants.NPPMSG + 107), Unused, Unused);
            return ret.ToInt32() == 1;
        }


        private void AutoShowOrHidePanel(string currentFilePath)
        {
            if (nppReady && settings.AutoShowPanel)
            {
                // automatically show panel for supported file types
                if ((!isPanelVisible && viewerInterface.IsValidFileExtension(currentFilePath)) ||
                    (isPanelVisible && !viewerInterface.IsValidFileExtension(currentFilePath)))
                {
                    TogglePanelVisible();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

        public enum WindowsMessage
        {
            WM_NOTIFY = 0x004E
        }

        protected void HandleWndProc(ref Message m)
        {
            //Listen for the closing of the dockable panel to toggle the toolbar icon
            switch (m.Msg)
            {
                case (int)WindowsMessage.WM_NOTIFY:
                    var notify = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                    if (notify.code == (int)DockMgrMsg.DMN_CLOSE)
                    {
                        ToolWindowCloseAction();
                    }
                    break;
            }
        }

    }
}
