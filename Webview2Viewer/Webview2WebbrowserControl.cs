using Microsoft.Web.WebView2.Core;
using PanelCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int lastVerticalScroll = 0;
        private bool webViewInitialized = false;

        public Action<string> StatusTextChangedAction { get; set; }
        public Action RenderingDoneAction { get; set; }


        public Webview2WebbrowserControl()
        {
            webView = null;
        }

        public void Initialize(int zoomLevel)
        {
            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FOLDER_NAME, "webview2");
            var props = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
            props.UserDataFolder = cacheDir;
            //props.AdditionalBrowserArguments = "--disable-web-security --allow-file-access-from-files --allow-file-access";
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            webView.CreationProperties = props;
            webView.AccessibleName = "webView";
            webView.Name = "webView";
            webView.ZoomFactor = ConvertToZoomFactor(zoomLevel);
            webView.Source = new Uri("about:blank", UriKind.Absolute);
            webView.Location = new Point(1, 27);
            webView.Size = new Size(800, 424);
            webView.Dock = DockStyle.Fill;
            webView.TabIndex = 0;
            webView.NavigationStarting += OnWebBrowser_NavigationStarting;
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.ZoomFactor = ConvertToZoomFactor(zoomLevel);
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        public void AddToHost(Control host)
        {
            host.Controls.Add(webView);
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webViewInitialized = true;
        }

        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView.ExecuteScriptAsync("window.scrollBy(0, " + lastVerticalScroll + " )");
            if (RenderingDoneAction != null) RenderingDoneAction();
        }

        /*public async Task SetScreenshot(PictureBox pictureBox)
        {
            pictureBox.Image = null;
            if (!webViewInitialized) return;
            var ms = new MemoryStream();
            await webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, ms);
            var screenshot = new Bitmap(ms);
            pictureBox.Image = screenshot;
            pictureBox.Visible = true;
        }*/

        public Bitmap MakeScreenshot()
        {
            return null;
        }

        public async void PrepareContentUpdate(bool preserveVerticalScrollPosition)
        {
            if (!webViewInitialized) return;
            if (preserveVerticalScrollPosition)
            {
                var scrollPosition = await webView.ExecuteScriptAsync("window.pageYOffset");
                lastVerticalScroll = int.Parse(scrollPosition.Split('.')[0]);
            }
            else
            {
                lastVerticalScroll = 0;
            }
        }


        public void ScrollToElementWithLineNo(int lineNo)
        {

        }

        public void SetContent(string content)
        {
            if (!webViewInitialized) return;
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

        }

        public void SetZoomLevel(int zoomLevel)
        {
            try
            {
                double zoomFactor = ConvertToZoomFactor(zoomLevel);
                webView.Invoke(new MethodInvoker(delegate
                {
                    if (webView.ZoomFactor != zoomFactor)
                        webView.ZoomFactor = zoomFactor;
                }));

            }
            catch (Exception ex)
            {

            }
        }

        private double ConvertToZoomFactor(int zoomLevel)
        {
            double zoomFactor = Convert.ToDouble(zoomLevel) / 100;
            return zoomFactor;
        }

        void OnWebBrowser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith("about:blank"))
            {
                e.Cancel = true;
            }
            else if (!e.Uri.ToString().StartsWith("data:"))
            {
                e.Cancel = true;
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(e.Uri.ToString());
                p.Start();
            }
        }

        public string GetRenderingEngineName()
        {
            return "EDGE";
        }


    }
}
