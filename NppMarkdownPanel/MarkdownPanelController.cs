using Kbg.NppPluginNET.PluginInfrastructure;
using NppMarkdownPanel.Forms;
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
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using static Kbg.NppPluginNET.PluginInfrastructure.Win32;

namespace NppMarkdownPanel
{
    public class MarkdownPanelController
    {
        private MarkdownPreviewForm markdownPreviewForm;
        private Timer renderTimer;

        private int idMyDlg = -1;

        private const int renderRefreshRateMilliSeconds = 500;
        private const int inputUpdateThresholdMiliseconds = 400;
        private int lastTickCount = 0;

        private bool isPanelVisible;

        private readonly IScintillaGateway scintillaGateway;
        private readonly INotepadPPGateway notepadPPGateway;
        private int lastCaretPosition;
        private string iniFilePath;
        private bool syncViewWithCaretPosition;

        public MarkdownPanelController()
        {
            scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());
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
                    if (syncViewWithCaretPosition && lastCaretPosition != scintillaGateway.GetCurrentPos().Value)
                    {
                        lastCaretPosition = scintillaGateway.GetCurrentPos().Value;
                        ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber());
                    }
                }
                else
                if (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
                {
                    RenderMarkdown();
                }
                else if (notification.Header.Code == (uint)SciMsg.SCN_MODIFIED)
                {
                    /*bool isInsert = (notification.ModificationType & (uint)SciMsg.SC_MOD_INSERTTEXT) != 0;
                    bool isDelete = (notification.ModificationType & (uint)SciMsg.SC_MOD_DELETETEXT) != 0;*/
                    // Any modifications made ?
                    if (true)
                    {
                        lastTickCount = Environment.TickCount;
                        RenderMarkdown();
                    }
                }
            }
        }

        private void RenderMarkdown()
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
                markdownPreviewForm.RenderMarkdown(GetCurrentEditorText(), notepadPPGateway.GetCurrentFilePath());
            }
            catch 
            {
            }
        }

        private string GetCurrentEditorText()
        {
            return scintillaGateway.GetText(scintillaGateway.GetLength() + 1);
        }

        private void ScrollToElementAtLineNo(int lineNo)
        {
            markdownPreviewForm.ScrollToElementWithLineNo(lineNo);
        }

        public void InitCommandMenu()
        {
            SetIniFilePath();
            syncViewWithCaretPosition = (Win32.GetPrivateProfileInt("Options", "SyncViewWithCaretPosition", 0, iniFilePath) != 0);
            markdownPreviewForm.CssFileName = Win32.ReadIniValue("Options", "CssFileName", iniFilePath, "style.css");
            markdownPreviewForm.ZoomLevel = Win32.GetPrivateProfileInt("Options", "ZoomLevel", 130, iniFilePath);
            markdownPreviewForm.HtmlFileName = Win32.ReadIniValue("Options", "HtmlFileName", iniFilePath);
            markdownPreviewForm.ShowToolbar = Utils.ReadIniBool("Options", "ShowToolbar", iniFilePath);
            PluginBase.SetCommand(0, "About", ShowAboutDialog, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "Toggle Markdown Panel", TogglePanelVisible);
            PluginBase.SetCommand(2, "Synchronize viewer with caret position", SyncViewWithCaret, syncViewWithCaretPosition);
            PluginBase.SetCommand(3, "Edit Settings", EditSettings);

            idMyDlg = 1;
        }


        private void EditSettings()
        {
            var settingsForm = new SettingsForms(markdownPreviewForm.ZoomLevel, markdownPreviewForm.CssFileName, markdownPreviewForm.HtmlFileName, markdownPreviewForm.ShowToolbar);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                markdownPreviewForm.CssFileName = settingsForm.CssFileName;
                markdownPreviewForm.ZoomLevel = settingsForm.ZoomLevel;
                markdownPreviewForm.HtmlFileName = settingsForm.HtmlFileName;
                markdownPreviewForm.ShowToolbar = settingsForm.ShowToolbar;
                SaveSettings();
                //Update Preview
                RenderMarkdown();
            }
        }

        private void SetIniFilePath()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, Main.PluginName + ".ini");
        }

        private void SyncViewWithCaret()
        {
            syncViewWithCaretPosition = !syncViewWithCaretPosition;
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[2]._cmdID, Win32.MF_BYCOMMAND | (syncViewWithCaretPosition ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            if (syncViewWithCaretPosition) ScrollToElementAtLineNo(scintillaGateway.GetCurrentLineNumber());
        }

        public void SetToolBarIcon()
        {
            toolbarIconsOld tbIconsOld = new toolbarIconsOld();
            tbIconsOld.hToolbarBmp = Properties.Resources.markdown_16x16_solid.GetHbitmap();
            tbIconsOld.hToolbarIcon = Properties.Resources.markdown_16x16_solid_dark.GetHicon();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIconsOld));
            Marshal.StructureToPtr(tbIconsOld, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_DEPRECATED, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);

            // if npp version >= 8 then use new NPPM_ADDTOOLBARICON_FORDARKMODE
            /*    toolbarIconsNew tbIconsNew = new toolbarIconsNew();
                tbIconsNew.hToolbarBmp = Properties.Resources.markdown_16x16_solid.GetHbitmap();
                tbIconsNew.hToolbarIcon = Properties.Resources.markdown_16x16_solid.GetHicon();
                tbIconsNew.hToolbarIconDarkMode = Properties.Resources.markdown_16x16_solid.GetHicon();
                //    tbIconsOld.hToolbarIconDarkMode = Properties.Resources.markdown_16x16_solid_dark.GetHbitmap();
                IntPtr pTbIconsNew = Marshal.AllocHGlobal(Marshal.SizeOf(tbIconsNew));
                Marshal.StructureToPtr(tbIconsNew, pTbIconsNew, false);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIconsNew);
                Marshal.FreeHGlobal(pTbIconsNew);*/
        }

        public void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("Options", "SyncViewWithCaretPosition", syncViewWithCaretPosition ? "1" : "0", iniFilePath);
            SaveSettings();
        }

        private void SaveSettings()
        {
            Win32.WriteIniValue("Options", "CssFileName", markdownPreviewForm.CssFileName, iniFilePath);
            Win32.WriteIniValue("Options", "ZoomLevel", markdownPreviewForm.ZoomLevel.ToString(), iniFilePath);
            Win32.WriteIniValue("Options", "HtmlFileName", markdownPreviewForm.HtmlFileName, iniFilePath);
            Win32.WriteIniValue("Options", "ShowToolbar", markdownPreviewForm.ShowToolbar.ToString(), iniFilePath);
        }

        private void ShowAboutDialog()
        {
            MessageBox.Show(
                MainResources.AboutDialogText
                , "About");
        }

        private bool initDialog;

        private void TogglePanelVisible()
        {
            if (!initDialog)
            {
                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = markdownPreviewForm.Handle;
                _nppTbData.pszName = Main.PluginTitle;
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)ConvertBitmapToIcon(Properties.Resources.markdown_16x16_solid_bmp).Handle;
                _nppTbData.pszModuleName = Main.PluginName;
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
                RenderMarkdown();
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
            return new MarkdigMarkdownGenerator();
        }
    }
}
