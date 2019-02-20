using CommonMark;
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
                    ScrollToElementAtCaretPosition(scintillaGateway.GetCurrentPos());
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

        private void ScrollToElementAtCaretPosition(Position caretPosition)
        {
            var count = 0;
            var mdText = GetCurrentEditorText();
            var rootBlock = CommonMarkConverter.Parse(mdText, new CommonMarkSettings { TrackSourcePosition = true });
            if (rootBlock != null)
            {
                var currentBlock = rootBlock.FirstChild;

                while (currentBlock != null)
                {
                    if (currentBlock.SourcePosition > caretPosition.Value) break;
                    count++;
                    currentBlock = currentBlock.NextSibling;
                }
            }
            markdownPreviewForm.ScrollToChildWithIndex(count);
        }

        public void InitCommandMenu()
        {
            PluginBase.SetCommand(0, "About", ShowAboutDialog, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "Toggle Markdown Panel", TogglePanelVisible);
            idMyDlg = 1;
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
            //Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
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
