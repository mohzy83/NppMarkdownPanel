
using PanelCommon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Webview2Viewer
{
    public class Webview2WebbrowserControl : IWebbrowserControl
    {
        const string CONFIG_FOLDER_NAME = "MarkdownPanel";
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;


        public Action<string> StatusTextChangedAction { get; set; }
        public Action RenderingDoneAction { get; set; }


        public Webview2WebbrowserControl()
        {
            webView = null;
        }

        public void AddToHost(Control host)
        {
            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FOLDER_NAME, "webview2");
            var props = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
            props.UserDataFolder = cacheDir;
            props.AdditionalBrowserArguments = "--allow-file-access-from-files";
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            webView.AccessibleName = "webView";
            webView.Location = new Point(1, 27);
            webView.Name = "webView";
            webView.Size = new Size(800, 424);
            webView.CreationProperties = props;
            webView.Dock = DockStyle.Fill;



            webView.Source = new Uri("about:blank", UriKind.Absolute);
            webView.TabIndex = 0;


            //this.webView.Text = "webView21";
            webView.ZoomFactor = 1D;

            host.Controls.Add(webView);
        }

        public Bitmap MakeScreenshot()
        {
            var screenshot = new Bitmap(800, 600);
            return screenshot;
        }

        public void PrepareContentUpdate(bool preserveVerticalScrollPosition)
        {

        }

        public void ScrollToElementWithLineNo(int lineNo)
        {

        }

        public void SetContent(string content)
        {
            try
            {
                webView.Invoke(new MethodInvoker(delegate
                {
                    webView.NavigateToString(content);
                }));

            }
            catch (Exception ex)
            {

            }
            if (RenderingDoneAction != null) RenderingDoneAction();
        }

        public void SetZoomLevel(int zoomLevel)
        {

        }
    }
}
