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

namespace NppMarkdownPanel
{
    public class MarkdownPanelController
    {
        public IScintillaGateway ScintillaGateway { get; protected set; }
        public INotepadPPGateway NotepadPPGateway { get; protected set; }
        private MarkdownPreviewForm markdownPreviewForm;

        //private string iniFilePath = null;
        //private bool someSetting = false;

        private int idMyDlg = -1;

        private bool isPanelVisible;
        private bool isUpdating = false;

        public MarkdownPanelController()
        {
            ScintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            NotepadPPGateway = new NotepadPPGateway();
            markdownPreviewForm = new MarkdownPreviewForm();
        }

        public void OnNotification(ScNotification notification)
        {
            if (isPanelVisible)
            {
                if (notification.Header.Code == (uint)SciMsg.SCN_UPDATEUI)
                {
                    //Update the view
                    //Update((notification.Updated & (uint)SciMsg.SC_UPDATE_V_SCROLL) != 0);
                    //bool scrollbarUpdated = (notification.Updated & (uint)SciMsg.SC_UPDATE_V_SCROLL) != 0;
                    //if (!scrollbarUpdated)
                    //    RenderMarkdown();
                }
                else if (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
                {
                    //Update the scintilla handle in all cases to keep track of which instance is active
                    //UpdateEditorInformation();
                    //((ScintillaGateway)ScintillaGateway).CurrentBufferID = notification.Header.IdFrom;
                    //Update(true, true);
                    //mdfilename = 
                    RenderMarkdown();
                }
                else if (notification.Header.Code == (uint)SciMsg.SCN_MODIFIED)
                {
                    bool isInsert = (notification.ModificationType & (uint)SciMsg.SC_MOD_INSERTTEXT) != 0;
                    bool isDelete = (notification.ModificationType & (uint)SciMsg.SC_MOD_DELETETEXT) != 0;

                    //Track if any text modifications have been made
                    if (isInsert || isDelete) RenderMarkdown();
                }
            }
        }

        private void RenderMarkdown()
        {
            try
            {
                if (!isUpdating)
                {
                    isUpdating = true;
                    markdownPreviewForm.RenderMarkdown(ScintillaGateway.GetText(ScintillaGateway.GetLength()), NotepadPPGateway.GetCurrentFilePath());
                    isUpdating = false;
                }
            }
            catch (Exception e)
            {

            }

        }

        public void InitCommandMenu()
        {

            //StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            //Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            //iniFilePath = sbIniFilePath.ToString();
            //if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            //iniFilePath = Path.Combine(iniFilePath, Main.PluginName + ".ini");
            //someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

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
            MessageBox.Show("NppMarkdownPanel for Notepad++\nCreated by Mohzy 2019\nGithub: https://github.com/mohzy83/NppMarkdownPanel", "About");
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
            RenderMarkdown();
            isPanelVisible = !isPanelVisible;
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

    }
}
