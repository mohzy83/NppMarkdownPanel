using CommonMark;
using Kbg.NppPluginNET.PluginInfrastructure;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class MarkdownPreviewForm : Form
    {
        const string DEFAULT_HTML_BASE =
         @"<!DOCTYPE html>
            <html>
                <head>
                    <title>{0}</title>
	                <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""></meta>
	                <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"">
                    <style type=""text/css"">
                    {1}
                    </style>
                </head>
                <body style=""{2}"">
                {3}
                </body>
            <html>
            ";

        private string currentText;
        private string filepath;
        private Task<string> renderTask;
        private bool dpiAdjusted;
        private Action toolWindowCloseAction;


        public int WM_NOTIFY { get; private set; }

        public MarkdownPreviewForm(Action toolWindowCloseAction)
        {
            this.toolWindowCloseAction = toolWindowCloseAction;
            InitializeComponent();
            CommonMarkSettings.Default.UriResolver =
                (fname) =>
                {
                    // ignore uri with http
                    var absolutePath = fname != null && !fname.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? Path.Combine(Path.GetDirectoryName(filepath), fname) : fname;

                    try
                    {
                        var uri = new Uri(absolutePath);
                        return uri.AbsoluteUri;
                    }
                    catch (Exception e)
                    {
                        return fname;
                    }
                };
        }

        public void RenderMarkdown(string currentText, string filepath)
        {
            if (renderTask == null || renderTask.IsCompleted)
            {
                var context = TaskScheduler.FromCurrentSynchronizationContext();
                renderTask = new Task<string>(() =>
                {
                    this.currentText = currentText;
                    this.filepath = filepath;
                    var result = CommonMarkConverter.Convert(currentText);
                    var defaultBodyStyle = "";
                    return string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), MainResources.DefaultCss, defaultBodyStyle, result);
                });
                renderTask.ContinueWith((renderedText) =>
                {
                    webBrowserPreview.DocumentText = renderedText.Result;
                    AdjustZoomForCurrentDpi();
                }, context);
                renderTask.Start();
            }
        }

        private void webBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor.Current = Cursors.IBeam;
            Application.DoEvents();
        }

        /// <summary>
        /// Increase Zoomlevel in case of higher DPI settings
        /// </summary>
        private void AdjustZoomForCurrentDpi()
        {
            Application.DoEvents();
            if (!dpiAdjusted)
            {
                dpiAdjusted = true;
                float dpiX, dpiY;
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpiX = graphics.DpiX;
                    dpiY = graphics.DpiY;
                }

                float zoomfactor = 120 * (dpiX / 96);
                int browserZoom = Convert.ToInt32(zoomfactor);
                var browserInst = ((SHDocVw.IWebBrowser2)(webBrowserPreview.ActiveXInstance));
                browserInst.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, browserZoom, IntPtr.Zero);
                webBrowserPreview.Document.Window.ScrollTo(0, 0);
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

        protected override void WndProc(ref Message m)
        {
            //Listen for the closing of the dockable panel to toggle the toolbar icon
            switch (m.Msg)
            {
                case (int)WindowsMessage.WM_NOTIFY:
                    var notify = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                    if (notify.code == (int)DockMgrMsg.DMN_CLOSE)
                    {
                        toolWindowCloseAction();
                    }
                    break;
            }
            //Continue the processing, as we only toggle
            base.WndProc(ref m);
        }

    }
}
