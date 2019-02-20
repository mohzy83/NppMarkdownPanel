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
        private int lastVerticalScroll = 0;

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
                SaveLastVerticalScrollPosition();
                MakeAndDisplayScreenShot();

                var context = TaskScheduler.FromCurrentSynchronizationContext();
                renderTask = new Task<string>(() =>
                {
                    this.currentText = currentText;
                    this.filepath = filepath;
                    var result = CommonMarkConverter.Convert(currentText);
                    var defaultBodyStyle = "";
                    var rr = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), MainResources.DefaultCss, defaultBodyStyle, result);
                    return rr;

                });
                renderTask.ContinueWith((renderedText) =>
                {
                    webBrowserPreview.DocumentText = renderedText.Result;
                    AdjustZoomForCurrentDpi();
                }, context);
                renderTask.Start();
            }
        }
        /// <summary>
        /// Makes and displays a screenshot of the current browser content to prevent it from flickering 
        /// while loading updated content
        /// </summary>
        private void MakeAndDisplayScreenShot()
        {
            Bitmap screenshot = new Bitmap(webBrowserPreview.Width, webBrowserPreview.Height);
            ActiveXScreenShotMaker.GetImage(webBrowserPreview.ActiveXInstance, screenshot, Color.White);
            pictureBoxScreenshot.Image = screenshot;
            pictureBoxScreenshot.Visible = true;
        }

        /// <summary>
        /// Saves the last vertical scrollpositions, after reloading the position will be 0
        /// </summary>
        private void SaveLastVerticalScrollPosition()
        {
            if (webBrowserPreview.Document != null)
            {
                try
                {
                    lastVerticalScroll = webBrowserPreview.Document.GetElementsByTagName("HTML")[0].ScrollTop;
                }
                catch { }
            }
        }

        private void webBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor.Current = Cursors.IBeam;
            GoToLastVerticalScrollPosition();
            HideScreenshotAndShowBrowser();
        }

        private void GoToLastVerticalScrollPosition()
        {
            webBrowserPreview.Document.Window.ScrollTo(0, lastVerticalScroll);
            Application.DoEvents();
        }

        private void HideScreenshotAndShowBrowser()
        {
            pictureBoxScreenshot.Visible = false;
            pictureBoxScreenshot.Image = null;
        }

        public void ScrollToChildWithIndex(int elementIndex)
        {
            Application.DoEvents();
            if (webBrowserPreview.Document != null && webBrowserPreview.Document.Body != null && webBrowserPreview.Document.Body.Children.Count > elementIndex - 1)
            {
                var currentTop = webBrowserPreview.Document.GetElementsByTagName("HTML")[0].ScrollTop;
                var child = webBrowserPreview.Document.Body.Children[elementIndex - 1];

                if (child != null && (child.OffsetRectangle.Top < currentTop || child.OffsetRectangle.Bottom > currentTop + webBrowserPreview.Height)) child.ScrollIntoView(true);
            }
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

        //public void ScrollByRatioVertically(double scrollRatio)
        //{
        //    if (webBrowserPreview.Document != null)
        //    {
        //        var rect = webBrowserPreview.Document.Body.ScrollRectangle;
        //        int verticalScroll = (int)((rect.Height - webBrowserPreview.Height) * scrollRatio);
        //        webBrowserPreview.Document.Window.ScrollTo(0, verticalScroll);
        //    }
        //}

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
