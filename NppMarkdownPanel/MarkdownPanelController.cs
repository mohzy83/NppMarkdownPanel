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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel
{
    public class MarkdownPanelController : IDisposable
    {
        private IViewerInterface viewerInterface;
        private Timer renderTimer;

        private int idMyDlg = -1;

        private const int Unused = 0;

        private const int renderRefreshRateMilliSeconds = 500;
        private const int inputUpdateThresholdMiliseconds = 400;
        private int lastTickCount = 0;

        private enum PanelState { Hidden, Docked, FullScreen }
        private PanelState _panelState;
        private bool isPanelVisible => _panelState != PanelState.Hidden;

        private readonly Func<IScintillaGateway> scintillaGatewayFactory;
        private readonly INotepadPPGateway notepadPPGateway;

        private string iniFilePath;

        private int lastCaretPosition;
        private bool syncViewWithCaretPosition;

        private int currentFirstVisibleLine;
        private bool syncViewWithFirstVisibleLine;

        private bool showOutline;

        private bool nppReady;
        private Settings settings;
        private MarkdownPreviewForm previewForm;
        private bool _disposedValue;
        IntPtr _ptrNppTbData;
        private Icon _icon;
        private IntPtr _fullScreenSplitterHandle;
        private double _savedWidthRatio;

        public MarkdownPanelController()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            scintillaGatewayFactory = PluginBase.GetGatewayFactory();
            notepadPPGateway = new NotepadPPGateway();
            SetIniFilePath();
            settings = LoadSettingsFromIni();
            viewerInterface = MarkdownPreviewForm.InitViewer(settings, HandleWndProc);
            previewForm = (MarkdownPreviewForm)viewerInterface;
            previewForm.SetCheckboxToggleHandler(ToggleCheckboxAtLine);
            previewForm.SetRadioToggleHandler(ToggleRadioAtLine);
            renderTimer = new Timer();
            renderTimer.Interval = renderRefreshRateMilliSeconds;
            renderTimer.Tick += OnRenderTimerElapsed;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var di = new DirectoryInfo(Path.Combine(PluginUtils.GetPluginDirectory(), "lib"));

            var modulename = args.Name.Split(',')[0];

            var module = di.GetFiles().FirstOrDefault(i => i.Name == modulename + ".dll");
            if (module != null)
            {
                return Assembly.LoadFrom(module.FullName);
            }
            return null;
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
            settings.SupportFilesWithNoExt = PluginUtils.ReadIniBool("Options", "SupportFilesWithNoExt", iniFilePath);
            settings.AllowAllExtensions = PluginUtils.ReadIniBool("Options", "AllowAllExtensions", iniFilePath);
            settings.IsDarkModeEnabled = IsDarkModeEnabled();
            settings.AutoShowPanel = PluginUtils.ReadIniBool("Options", "AutoShowPanel", iniFilePath);
            settings.EnableThreeStateToggle = PluginUtils.ReadIniBool("Options", "EnableThreeStateToggle", iniFilePath);
            settings.RenderingEngine = Win32.ReadIniValue("Options", "RenderingEngine", iniFilePath, Settings.RENDERING_ENGINE_WEBVIEW2_EDGE);
            settings.ShowOutline = PluginUtils.ReadIniBool("Options", "ShowOutline", iniFilePath, false);
            settings.OfflineMode = PluginUtils.ReadIniBool("Options", "OfflineMode", iniFilePath, false);
            settings.OfflineMermaidScriptFileName = Win32.ReadIniValue("Options", "OfflineMermaidScriptFileName", iniFilePath, "");
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
                activeBufferId = notification.Header.IdFrom;
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
                viewerInterface.UpdateSettings(settings, OpenLocalFileInNpp);
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

            if (notification.Header.Code == (uint)NppMsg.NPPN_FILEBEFORESAVE)
            {
                if (notification.Header.IdFrom == activeBufferId)
                {
                    beforeSafeFilename = notepadPPGateway.GetFilePathFromBufferId(notification.Header.IdFrom);
                }
            }

            if (notification.Header.Code == (uint)NppMsg.NPPN_FILESAVED)
            {
                if (notification.Header.IdFrom == activeBufferId)
                {
                    var savedFilename = notepadPPGateway.GetFilePathFromBufferId(notification.Header.IdFrom);
                    if (!string.Equals(beforeSafeFilename, savedFilename))
                    {
                        HandleFilenameUpdate(notification.Header.IdFrom);
                    }
                    beforeSafeFilename = "";
                }
            }

            if (notification.Header.Code == (uint)NppMsg.NPPN_FILERENAMED)
            {
                HandleFilenameUpdate(notification.Header.IdFrom);
            }
        }

        string beforeSafeFilename = "";
        bool updateFilename = false;
        IntPtr bufferIdForFilenameUpdate;
        IntPtr activeBufferId;

        private void HandleFilenameUpdate(IntPtr bufferId)
        {
            bufferIdForFilenameUpdate = bufferId;
            updateFilename = true;
            lastTickCount = Environment.TickCount;
            RenderMarkdownDeferred();
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
                if (updateFilename)
                {
                    updateFilename = false;
                    var currentFilePath = notepadPPGateway.GetFilePathFromBufferId(bufferIdForFilenameUpdate);
                    viewerInterface.SetMarkdownFilePath(currentFilePath, true);
                    AutoShowOrHidePanel(currentFilePath);
                }

                RenderMarkdownDirect();
            }
            catch
            {
            }
        }

        private void RenderMarkdownDirect(bool preserveVerticalScrollPosition = true)
        {
            if (!_disposedValue)
            {
                viewerInterface.RenderMarkdown(GetCurrentEditorText(), notepadPPGateway.GetCurrentFilePath(), preserveVerticalScrollPosition);
            }
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

        private void ToggleCheckboxAtLine(int lineNo)
        {
            var gw = scintillaGatewayFactory();
            string lineText = gw.GetLine(lineNo);

            var idxEmpty = lineText.IndexOf("[ ]");
            if (idxEmpty >= 0)
            {
                int pos = gw.PositionFromLine(lineNo) + idxEmpty + 1;
                gw.SetSelection(pos, pos + 1);
                gw.ReplaceSel("x");
                RenderAfterToggle();
                return;
            }

            var idxChecked = lineText.IndexOf("[x]", StringComparison.OrdinalIgnoreCase);
            if (idxChecked >= 0)
            {
                int pos = gw.PositionFromLine(lineNo) + idxChecked + 1;
                gw.SetSelection(pos, pos + 1);
                gw.ReplaceSel(" ");
                RenderAfterToggle();
            }
        }

        private void RenderAfterToggle()
        {
            renderTimer.Stop();
            RenderMarkdownDirect();
        }

        private void ToggleRadioAtLine(int lineNo)
        {
            var gw = scintillaGatewayFactory();
            string lineText = gw.GetLine(lineNo);

            var hasEmpty = lineText.IndexOf("( )") >= 0;
            var idxChecked = lineText.IndexOf("(x)", StringComparison.OrdinalIgnoreCase);

            if (hasEmpty)
            {
                int pos = gw.PositionFromLine(lineNo) + lineText.IndexOf("( )") + 1;
                gw.SetSelection(pos, pos + 1);
                gw.ReplaceSel("x");
                UncheckRadioGroup(gw, lineNo);
            }
            else if (idxChecked >= 0)
            {
                int pos = gw.PositionFromLine(lineNo) + idxChecked + 1;
                gw.SetSelection(pos, pos + 1);
                gw.ReplaceSel(" ");
            }

            RenderAfterToggle();
        }

        private void UncheckRadioGroup(IScintillaGateway gw, int excludeLine)
        {
            int totalLines = gw.GetLineCount();
            int startLine = excludeLine;
            int endLine = excludeLine;

            for (int i = excludeLine - 1; i >= 0; i--)
            {
                string text = gw.GetLine(i).TrimStart();
                if (IsRadioLine(text))
                    startLine = i;
                else
                    break;
            }

            for (int i = excludeLine + 1; i < totalLines; i++)
            {
                string text = gw.GetLine(i).TrimStart();
                if (IsRadioLine(text))
                    endLine = i;
                else
                    break;
            }

            for (int i = startLine; i <= endLine; i++)
            {
                if (i == excludeLine) continue;
                string text = gw.GetLine(i);
                int idx = text.IndexOf("(x)", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    int pos = gw.PositionFromLine(i) + idx + 1;
                    gw.SetSelection(pos, pos + 1);
                    gw.ReplaceSel(" ");
                }
            }
        }

        private static bool IsRadioLine(string text)
        {
            if (!(text.StartsWith("- ") || text.StartsWith("* ") || text.StartsWith("+ ")))
                return false;
            int p = text.IndexOf('(');
            return p >= 0 && p <= 3 && (text.Substring(p).StartsWith("( )") || text.Substring(p).StartsWith("(x)", StringComparison.OrdinalIgnoreCase));
        }

        public void InitCommandMenu()
        {
            syncViewWithCaretPosition = (Win32.GetPrivateProfileInt("Options", "SyncViewWithCaretPosition", 0, iniFilePath) != 0);
            syncViewWithFirstVisibleLine = (Win32.GetPrivateProfileInt("Options", "SyncWithFirstVisibleLine", 0, iniFilePath) != 0);
            showOutline = PluginUtils.ReadIniBool("Options", "ShowOutline", iniFilePath, false);
            PluginBase.SetCommand(0, "Toggle &Markdown Panel", TogglePanelVisible);
            PluginBase.SetCommand(1, "---", null);
            PluginBase.SetCommand(2, "Synchronize with &caret position", SyncViewWithCaret, syncViewWithCaretPosition);
            PluginBase.SetCommand(3, "Synchronize with &first visible line in editor", SyncViewWithFirstVisibleLine, syncViewWithFirstVisibleLine);
            PluginBase.SetCommand(4, "Show &outline", ToggleShowOutline, showOutline);
            PluginBase.SetCommand(5, "---", null);
            PluginBase.SetCommand(6, "&Settings", EditSettings);
            PluginBase.SetCommand(7, "&Help", ShowHelp);
            PluginBase.SetCommand(8, "&About", ShowAboutDialog);
            PluginBase.SetCommand(9, "---", null);
            PluginBase.SetCommand(10, "Export to &PDF", ExportToPdf);
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
                settings.SupportFilesWithNoExt = settingsForm.SupportFilesWithNoExt;
                settings.AllowAllExtensions = settingsForm.AllowAllExtensions;
                settings.ShowStatusbar = settingsForm.ShowStatusbar;
                settings.AutoShowPanel = settingsForm.AutoShowPanel;
                settings.EnableThreeStateToggle = settingsForm.EnableThreeStateToggle;
                settings.RenderingEngine = settingsForm.RenderingEngine;
                settings.OfflineMode = settingsForm.OfflineMode;
                settings.OfflineMermaidScriptFileName = settingsForm.OfflineMermaidScriptFileName;

                settings.IsDarkModeEnabled = IsDarkModeEnabled();
                viewerInterface.UpdateSettings(settings, OpenLocalFileInNpp);
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

        private void ToggleShowOutline()
        {
            showOutline = !showOutline;
            settings.ShowOutline = showOutline;
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[4]._cmdID, Win32.MF_BYCOMMAND | (showOutline ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            if (isPanelVisible) RenderMarkdownDirect();
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
            Win32.WriteIniValue("Options", "SupportFilesWithNoExt", settings.SupportFilesWithNoExt.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "AutoShowPanel", settings.AutoShowPanel.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "EnableThreeStateToggle", settings.EnableThreeStateToggle.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "AllowAllExtensions", settings.AllowAllExtensions.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "RenderingEngine", settings.RenderingEngine, iniFilePath);
            Win32.WriteIniValue("Options", "ShowOutline", settings.ShowOutline.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "OfflineMode", settings.OfflineMode.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "OfflineMermaidScriptFileName", settings.OfflineMermaidScriptFileName, iniFilePath);
        }
        private void ShowAboutDialog()
        {
            var aboutDialog = new AboutForm();
            aboutDialog.ShowDialog();
        }

        private void ExportToPdf()
        {
            viewerInterface.ExportToPdf();
        }

        private bool initDialog;

        private void TogglePanelVisible()
        {
            if (settings.EnableThreeStateToggle)
            {
                switch (_panelState)
                {
                    case PanelState.Hidden:
                        ShowDocked();
                        _panelState = PanelState.Docked;
                        break;
                    case PanelState.Docked:
                        GoFullscreen();
                        _panelState = PanelState.FullScreen;
                        break;
                    case PanelState.FullScreen:
                        RestoreFromFullscreen();
                        ClosePanel();
                        _panelState = PanelState.Hidden;
                        break;
                }
            }
            else
            {
                if (_panelState == PanelState.FullScreen)
                {
                    RestoreFromFullscreen();
                    ClosePanel();
                    _panelState = PanelState.Hidden;
                }
                else if (_panelState == PanelState.Hidden)
                {
                    ShowDocked();
                    _panelState = PanelState.Docked;
                }
                else
                {
                    ClosePanel();
                }
            }

            if (isPanelVisible)
            {
                var currentFilePath = notepadPPGateway.GetCurrentFilePath();
                viewerInterface.SetMarkdownFilePath(currentFilePath);
                viewerInterface.UpdateSettings(settings, OpenLocalFileInNpp);
                RenderMarkdownDirect(false);
            }
        }

        private void ClosePanel()
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, viewerInterface.Handle);
            _panelState = PanelState.Hidden;
        }

        private void ShowDocked()
        {
            if (!initDialog)
            {
                viewerInterface.InitRenderingEngine(settings, OpenLocalFileInNpp);

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = viewerInterface.Handle;
                _nppTbData.pszName = Main.PluginTitle;
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _icon = ConvertBitmapToIcon(Properties.Resources.markdown_16x16_solid_bmp);
                _nppTbData.hIconTab = (uint)_icon.Handle;
                _nppTbData.pszModuleName = $"{Main.ModuleName}.dll";
                _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                initDialog = true;
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, viewerInterface.Handle);
            }
        }

        private void GoFullscreen()
        {
            var containerHandle = Win32.GetParent(viewerInterface.Handle);
            var nppHandle = PluginBase.nppData._nppHandle;

            _fullScreenSplitterHandle = FindRightSplitter(nppHandle, containerHandle);

            var dockMgrHandle = Win32.FindWindowEx(nppHandle, IntPtr.Zero, Win32.DOCKING_MANAGER_CLASS, null);

            if (dockMgrHandle != IntPtr.Zero && _fullScreenSplitterHandle != IntPtr.Zero)
            {
                Win32.GetClientRect(nppHandle, out RECT nppClient);
                Win32.GetWindowRect(containerHandle, out RECT containerScreenRect);

                var clientOrigin = new Point(0, 0);
                Win32.ClientToScreen(nppHandle, ref clientOrigin);

                int currentWidth = containerScreenRect.Right - containerScreenRect.Left;
                _savedWidthRatio = (double)currentWidth / nppClient.Right;
                int targetOffset = nppClient.Right - currentWidth - 4;

                Win32.SendMessage(dockMgrHandle, (uint)DockMgrMsg.DMM_MOVE_SPLITTER, targetOffset, _fullScreenSplitterHandle);
            }
        }

        private IntPtr FindRightSplitter(IntPtr nppHandle, IntPtr containerHandle)
        {
            Win32.GetWindowRect(containerHandle, out RECT containerScreenRect);

            IntPtr hwnd = IntPtr.Zero;
            while ((hwnd = Win32.FindWindowEx(nppHandle, hwnd, Win32.VERT_SPLITTER_CLASS, null)) != IntPtr.Zero)
            {
                Win32.GetWindowRect(hwnd, out RECT splitterRect);
                if (splitterRect.Right >= containerScreenRect.Left - 10 && splitterRect.Right <= containerScreenRect.Left + 5)
                    return hwnd;
            }
            return IntPtr.Zero;
        }

        private void RestoreFromFullscreen()
        {
            var nppHandle = PluginBase.nppData._nppHandle;
            var dockMgrHandle = Win32.FindWindowEx(nppHandle, IntPtr.Zero, Win32.DOCKING_MANAGER_CLASS, null);

            if (dockMgrHandle != IntPtr.Zero && _fullScreenSplitterHandle != IntPtr.Zero && _savedWidthRatio > 0)
            {
                Win32.GetClientRect(nppHandle, out RECT nppClient);
                var containerHandle = Win32.GetParent(viewerInterface.Handle);
                Win32.GetWindowRect(containerHandle, out RECT containerScreenRect);

                var clientOrigin = new Point(0, 0);
                Win32.ClientToScreen(nppHandle, ref clientOrigin);

                int currentWidth = containerScreenRect.Right - containerScreenRect.Left;
                int targetWidth = (int)(nppClient.Right * _savedWidthRatio);
                int restoreOffset = targetWidth - currentWidth;

                Win32.SendMessage(dockMgrHandle, (uint)DockMgrMsg.DMM_MOVE_SPLITTER, restoreOffset, _fullScreenSplitterHandle);
            }
            _savedWidthRatio = 0;
            _fullScreenSplitterHandle = IntPtr.Zero;

        }

        private void ToolWindowCloseAction()
        {
            if (_panelState == PanelState.FullScreen)
            {
                RestoreFromFullscreen();
            }
            _panelState = PanelState.Hidden;
        }

        private void AutoShowOrHidePanel(string currentFilePath)
        {
            if (nppReady && settings.AutoShowPanel)
            {
                if (_panelState == PanelState.FullScreen)
                {
                    if (!viewerInterface.IsValidFileExtension(currentFilePath))
                    {
                        RestoreFromFullscreen();
                        ClosePanel();
                        _panelState = PanelState.Hidden;
                    }
                    return;
                }

                if (isPanelVisible && !viewerInterface.IsValidFileExtension(currentFilePath))
                {
                    ClosePanel();
                    return;
                }

                if (!isPanelVisible && viewerInterface.IsValidFileExtension(currentFilePath))
                {
                    TogglePanelVisible();
                }
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

        private bool IsDarkModeEnabled()
        {
            // NPPM_ISDARKMODEENABLED (NPPMSG + 107)
            IntPtr ret = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)(Constants.NPPMSG + 107), Unused, Unused);
            return ret.ToInt32() == 1;
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

        /// <summary>
        /// Sets the <see cref="Win32.WS_EX_CONTROLPARENT"/> extended attribute on <paramref name="parent"/> and any child controls,
        /// following @mahee96's advice on the archived Plugin.Net issue tracker.
        /// <para>
        /// <seealso href="https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net/issues/17#issuecomment-683455467"/>
        /// </para>
        /// </summary>
        /// <param name="parent">
        /// A WinForm that's been registered with Npp's Docking Manager by sending <see cref="NppMsg.NPPM_DMMREGASDCKDLG"/>.
        /// </param>
        private void SetControlParent(Control parent, Func<IntPtr, int, IntPtr> wndLongGetter, Func<IntPtr, int, IntPtr, IntPtr> wndLongSetter)
        {
            if (parent.HasChildren)
            {
                long extAttrs = (long)wndLongGetter(parent.Handle, Win32.GWL_EXSTYLE);
                if (Win32.WS_EX_CONTROLPARENT != (extAttrs & Win32.WS_EX_CONTROLPARENT))
                {
                    wndLongSetter(parent.Handle, Win32.GWL_EXSTYLE, new IntPtr(extAttrs | Win32.WS_EX_CONTROLPARENT));
                }
                foreach (Control c in parent.Controls)
                {
                    SetControlParent(c, wndLongGetter, wndLongSetter);
                }
            }
        }

        // Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
                if (disposing)
                {
                    renderTimer.Enabled = false; //deactive timer
                    _icon?.Dispose();
                    _icon = null;
                    if (_ptrNppTbData != IntPtr.Zero)
                    {
                        Marshal.DestroyStructure(_ptrNppTbData, typeof(NppTbData));
                        Marshal.FreeHGlobal(_ptrNppTbData);
                        _ptrNppTbData = IntPtr.Zero;
                    }
                    previewForm?.Cleanup();
                }

            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void OpenLocalFileInNpp(string filename)
        {
          // Opens any local file in new NPP Tab. Even a binary file (e.g. executable file) is interpreted as chars and shouldnt be a problem
            notepadPPGateway.OpenFile(filename);
        }

    }
}
