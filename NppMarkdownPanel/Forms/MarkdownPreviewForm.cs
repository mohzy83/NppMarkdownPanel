using CommonMark;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public MarkdownPreviewForm()
        {
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

                }, context);
                renderTask.Start();
            }
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            webBrowserPreview.ShowPrintPreviewDialog();
        }

        private void webBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void ZoomBrowser()
        {
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ZoomBrowser();
        }
    }
}
