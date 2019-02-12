using CommonMark;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

        public MarkdownPreviewForm()
        {
            InitializeComponent();
            CommonMarkSettings.Default.UriResolver =
                (fname) =>
                {
                    // ignore uri with http
                    var absolutePath = fname != null && !fname.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? Path.Combine(Path.GetDirectoryName(filepath), fname) : fname;
                    var uri = new Uri(absolutePath);
                    return uri.AbsoluteUri;
                };
        }

        public void RenderMarkdown(string currentText, string filepath)
        {
            this.currentText = currentText;
            this.filepath = filepath;
            var result = CommonMarkConverter.Convert(currentText);
            var defaultBodyStyle = "";
            webBrowserPreview.DocumentText = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), MainResources.DefaultCss, defaultBodyStyle, result);
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            webBrowserPreview.ShowPrintPreviewDialog();
        }
    }
}
