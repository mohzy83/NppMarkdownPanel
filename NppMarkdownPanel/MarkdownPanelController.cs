using CommonMark;
using CommonMark.Syntax;
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

        private IScintillaGateway scintillaGateway;
        private INotepadPPGateway notepadPPGateway;
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
                        ScrollToElementAtCaretPosition(scintillaGateway.GetCurrentPos());
                    }
                }
                else
                if (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
                {
                    RenderMarkdown();
                }
                else if (notification.Header.Code == (uint)SciMsg.SCN_MODIFIED)
                {
                    bool isInsert = (notification.ModificationType & (uint)SciMsg.SC_MOD_INSERTTEXT) != 0;
                    bool isDelete = (notification.ModificationType & (uint)SciMsg.SC_MOD_DELETETEXT) != 0;
                    // Any modifications made ?
                    if (isInsert || isDelete)
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
            catch (Exception ex)
            {
            }
        }

        private string GetCurrentEditorText()
        {
            return scintillaGateway.GetText(scintillaGateway.GetLength() + 1);
        }

        private Block lastParserResult;
        private void ScrollToElementAtCaretPosition(Position caretPosition)
        {
            var count = 0;
            // Stores the postion of an element within the syntax tree, which is as close as possbile to the caret position
            List<int> elementPosPerLevel = new List<int>();
            var mdText = GetCurrentEditorText();
            Block rootBlock;
            if (CheckEditorTextHasChanged(mdText))
            {
                rootBlock = CommonMarkConverter.Parse(mdText, new CommonMarkSettings { TrackSourcePosition = true });
                lastParserResult = rootBlock;
            }
            else
            {
                rootBlock = lastParserResult;
            }
            if (rootBlock != null)
            {
                var currentBlock = rootBlock.FirstChild;

                while (currentBlock != null)
                {
                    if (currentBlock.NextSibling != null && currentBlock.NextSibling.SourcePosition < caretPosition.Value)
                    {
                        if (!(currentBlock.Tag == CommonMark.Syntax.BlockTag.Paragraph && currentBlock.Parent != null && currentBlock.Parent.ListData != null && currentBlock.Parent.ListData.IsTight))
                            count++;
                        currentBlock = currentBlock.NextSibling;
                    }
                    else
                    {
                        if (currentBlock.FirstChild != null)
                        {
                            currentBlock = currentBlock.FirstChild;
                            elementPosPerLevel.Add(count);
                            count = 0;
                        }
                        else
                        {
                            elementPosPerLevel.Add(count);
                            break;
                        }
                    }
                }
            }
            markdownPreviewForm.ScrollToChildWithIndex(elementPosPerLevel);
        }

        private string lastEditorTextHash;
        private bool CheckEditorTextHasChanged(string text)
        {
            bool hasChanged = false;
            var newHash = Utils.CreateMD5(text);
            if (!string.Equals(newHash, lastEditorTextHash)) hasChanged = true;
            lastEditorTextHash = newHash;
            return hasChanged;
        }

        public void InitCommandMenu()
        {
            SetIniFilePath();
            syncViewWithCaretPosition = (Win32.GetPrivateProfileInt("Options", "SyncViewWithCaretPosition", 0, iniFilePath) != 0);
            PluginBase.SetCommand(0, "About", ShowAboutDialog, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "Toggle Markdown Panel", TogglePanelVisible);
            PluginBase.SetCommand(2, "Synchronize viewer with caret positionb", SyncViewWithCaret, syncViewWithCaretPosition);
            idMyDlg = 1;
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
            if (syncViewWithCaretPosition) ScrollToElementAtCaretPosition(scintillaGateway.GetCurrentPos());
        }

        public void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = Properties.Resources.markdown_16x16_solid.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        public void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("Options", "SyncViewWithCaretPosition", syncViewWithCaretPosition ? "1" : "0", iniFilePath);
        }

        private void ShowAboutDialog()
        {
            MessageBox.Show(
                "NppMarkdownPanel for Notepad++\n" +
                "Created by Mohzy 2019\n" +
                "Github: https://github.com/mohzy83/NppMarkdownPanel\n" +
                "\n" +
                "Using markdown style github-markdown-css by sindresorhus - https://github.com/sindresorhus/github-markdown-css\n" +
                "\n" +
                "Using CommonMark.NET by Knagis - https://github.com/Knagis/CommonMark.NET\n" +
                "\n" +
                "Using portions of nea's **MarkdownViewerPlusPlus** Plugin code - https://github.com/nea/MarkdownViewerPlusPlus"
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
    }
}
