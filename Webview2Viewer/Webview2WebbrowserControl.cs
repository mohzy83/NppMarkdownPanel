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
using System.Web;
using System.Windows.Forms;

namespace Webview2Viewer
{
    public class Webview2WebbrowserControl : IWebbrowserControl
    {
        const string virtualHostProtocol = "http://";
        const string virtualHostName = "markdownpanel-virtualhost";
        const string CONFIG_FOLDER_NAME = "MarkdownPanel";
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private int lastVerticalScroll = 0;
        private bool webViewInitialized = false;

        public Action<string> StatusTextChangedAction { get; set; }
        public Action RenderingDoneAction { get; set; }

        private string currentBody;
        private string currentStyle;

        private string currentDocumentPath;

        private CoreWebView2Environment environment = null;

        public Webview2WebbrowserControl()
        {
            webView = null;
        }

        public async void Initialize(int zoomLevel)
        {
            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FOLDER_NAME, "webview2");
            //var props = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
            //props.UserDataFolder = cacheDir;
            //props.AdditionalBrowserArguments = "--disable-web-security --allow-file-access-from-files --allow-file-access";
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            var opt = new CoreWebView2EnvironmentOptions();
            //opt.
            environment = await CoreWebView2Environment.CreateAsync(null, cacheDir, opt);
            await webView.EnsureCoreWebView2Async(environment);

            //webView.CreationProperties = props;
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

            webViewInitialized = true;

            //webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        public void AddToHost(Control host)
        {
            host.Controls.Add(webView);
        }

        /*private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webViewInitialized = true;
        }*/

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            ExecuteWebviewAction(new Action(async () =>
            {
                await webView.ExecuteScriptAsync("window.scrollBy(0, " + lastVerticalScroll + " )");
                if (RenderingDoneAction != null) RenderingDoneAction();
            }));
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

        public void PrepareContentUpdate(bool preserveVerticalScrollPosition)
        {
            if (!webViewInitialized) return;
            if (preserveVerticalScrollPosition)
            {
                ExecuteWebviewAction(new Action(async () =>
                {
                    var scrollPosition = await webView.ExecuteScriptAsync("window.pageYOffset");
                    lastVerticalScroll = int.Parse(scrollPosition.Split('.')[0]);
                }));
            }
            else
            {
                lastVerticalScroll = 0;
            }
        }

        const string scrollScript =
            "var element = document.getElementById('{0}');\n" +
            "var headerOffset = 10;\n" +
            "var elementPosition = element.getBoundingClientRect().top;\n" +
            "var offsetPosition = elementPosition + window.pageYOffset - headerOffset;\n" +
            "window.scrollTo({{top: offsetPosition}});";


        public void ScrollToElementWithLineNo(int lineNo)
        {
            if (lineNo <= 0) lineNo = 0;
            ExecuteWebviewAction(new Action(async () =>
            {
                await webView.ExecuteScriptAsync(string.Format(scrollScript, lineNo));
            }));
        }

        public void SetContent(string content, string body, string style, string currentDocumentPath)
        {
            if (!webViewInitialized) return;

            var currentPath = Path.GetDirectoryName(currentDocumentPath);
            var replaceFileMapping = "file:///" + currentPath.Replace('\\', '/');

            content = content.Replace(replaceFileMapping, virtualHostProtocol + virtualHostName);
            body = body.Replace(replaceFileMapping, virtualHostProtocol + virtualHostName);

            var fullReload = false;
            if (this.currentDocumentPath != currentDocumentPath)
            {
                ExecuteWebviewAction(new Action(() =>
                {
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(virtualHostName, currentPath, CoreWebView2HostResourceAccessKind.Allow);
                }));
                this.currentDocumentPath = currentDocumentPath;
                fullReload = true;
            }

            if (!fullReload && currentBody != null && currentStyle != null)
            {
                if (currentBody != body)
                {
                    currentBody = body;
                    ExecuteWebviewAction(new Action(async () =>
                    {
                        await webView.ExecuteScriptAsync("document.body.innerHTML = '" + HttpUtility.JavaScriptStringEncode(currentBody) + "'");
                    }));
                }
                if (currentStyle != style)
                {
                    currentStyle = style;
                    ExecuteWebviewAction(new Action(async () =>
                    {
                        await webView.ExecuteScriptAsync(
                            "document.head.removeChild(document.head.lastElementChild);\n" +
                            "var style = document.createElement('style');\n" +
                            "style.type = 'text/css'; \n" +
                            "style.textContent = '" + HttpUtility.JavaScriptStringEncode(currentStyle) + "'; \n" +
                            "document.head.appendChild(style); \n"
                            );
                    }));
                }
            }
            else
            {
                currentBody = body;
                currentStyle = style;
                ExecuteWebviewAction(new Action(() =>
                {
                    webView.NavigateToString(content);
                }));
            }
        }

        public void SetZoomLevel(int zoomLevel)
        {
            double zoomFactor = ConvertToZoomFactor(zoomLevel);
            ExecuteWebviewAction(new Action(() =>
            {
                if (webView.ZoomFactor != zoomFactor)
                    webView.ZoomFactor = zoomFactor;

            }));
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
                var navUri = e.Uri.ToString();
                if (navUri.StartsWith(virtualHostProtocol + virtualHostName))
                {
                    var currentPath = Path.GetDirectoryName(currentDocumentPath);
                    navUri = navUri.Replace(virtualHostProtocol + virtualHostName, currentPath);
                    navUri = Uri.UnescapeDataString(navUri);
                }
                p.StartInfo = new ProcessStartInfo(navUri);
                p.Start();
            }
        }

        public string GetRenderingEngineName()
        {
            return "EDGE";
        }


        private void ExecuteWebviewAction(Action action)
        {
            try
            {
                webView.Invoke(action);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
