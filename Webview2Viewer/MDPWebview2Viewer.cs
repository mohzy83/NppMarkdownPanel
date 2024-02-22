using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Webview2Viewer
{
    public class MDPWebview2Viewer
    {
        const string CONFIG_FOLDER_NAME = "MarkdownPanel";
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public void AddViewerToHost(Control host)
        {

            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FOLDER_NAME, "webview2");
            var props = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
            props.UserDataFolder = cacheDir;
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.webView.AccessibleName = "webView";
            this.webView.Location = new System.Drawing.Point(1, 27);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(800, 424);
            this.webView.CreationProperties = props;
            this.webView.Dock = DockStyle.Fill;



            this.webView.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this.webView.TabIndex = 0;




            //this.webView.Text = "webView21";
            this.webView.ZoomFactor = 1D;

            host.Controls.Add(webView);
        }

        public void SetHtml(string html)
        {
            this.webView.Invoke(new MethodInvoker(delegate
            {
                this.webView.NavigateToString(html);
            }));
        }

    }
}
