using Microsoft.Web.WebView2.Core;
using PanelCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Webview2Viewer
{
    public class Webview2WebbrowserControl : IWebbrowserControl, IDisposable
    {
        const string virtualHostProtocol = "http://";
        const string virtualHostName = "markdownpanel-virtualhost";
        const string CONFIG_FOLDER_NAME = "MarkdownPanel";
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private bool webViewInitialized = false;

        public Action<string> StatusTextChangedAction { get; set; }
        public Action RenderingDoneAction { get; set; }
        public Action AfterInitCompletedAction { get; set; }
        public Action<int> CheckboxToggleAction { get; set; }
        public Action<int> RadioToggleAction { get; set; }

        private string currentBody;
        private string currentStyle;

        private string currentDocumentPath;

        private bool currentPageHasOutline;
        private bool forceFullReload;

        private Action<string> openLocalFileInNppAction;

        private CoreWebView2Environment environment = null;

        public Webview2WebbrowserControl()
        {
            webView = null;
        }

        public void Dispose()
        {
            webView?.Dispose();
            webView = null;
        }

        public void Initialize(int zoomLevel, Action<string> openLocalFileInNppAction)
        {
            this.openLocalFileInNppAction = openLocalFileInNppAction;
            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FOLDER_NAME, "webview2");
            //var props = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
            //props.UserDataFolder = cacheDir;
            //props.AdditionalBrowserArguments = "--disable-web-security --allow-file-access-from-files --allow-file-access";
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            var opt = new CoreWebView2EnvironmentOptions();

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            CoreWebView2Environment.CreateAsync(null, cacheDir, opt)
                    .ContinueWith(envTask =>
                    {
                        if (envTask.IsFaulted)
                        {
                            return;
                        }

                        environment = envTask.Result;
                        webView.EnsureCoreWebView2Async(environment)
                            .ContinueWith(ensureTask =>
                            {
                                if (ensureTask.IsFaulted)
                                {
                                    return;
                                }

                                webView.AccessibleName = "webView";
                                webView.Name = "webView";
                                webView.ZoomFactor = ConvertToZoomFactor(zoomLevel);
                                webView.Source = new Uri("about:blank", UriKind.Absolute);
                                webView.Location = new Point(1, 27);
                                webView.Size = new Size(800, 424);
                                webView.Dock = DockStyle.Fill;
                                webView.TabIndex = 0;
                                webView.NavigationStarting += OnWebBrowser_NavigationStarting;
                                webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
                                webView.NavigationCompleted += WebView_NavigationCompleted;
                                webView.ZoomFactor = ConvertToZoomFactor(zoomLevel);
                            }, scheduler);
                    }, scheduler);
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webViewInitialized = true;
                if (AfterInitCompletedAction != null) AfterInitCompletedAction();
            }
            else
            {
                MessageBox.Show("WebView2 Initialization Error: " + e?.InitializationException?.Message, "WebView2 Initialization Error");
            }

        }

        public void AddToHost(Control host)
        {
            host.Controls.Add(webView);
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!IsInitialized()) return;

            if (!String.IsNullOrEmpty(currentDocumentPath) && scrollYForFilename.ContainsKey(currentDocumentPath) && scrollYForFilename[currentDocumentPath] > 0)
            {
                ExecuteWebviewAction(new Action(async () =>
                {
                    await webView.ExecuteScriptAsync("window.scrollBy(0, " + scrollYForFilename[currentDocumentPath] + " )");
                    if (RenderingDoneAction != null) RenderingDoneAction();
                }));
            }

            if (e.IsSuccess)
            {
                ExecuteWebviewAction(new Action(async () =>
                {
                    // inject JS to listen to the "Scrollend" event
                    string jsScript = @"

                                window.addEventListener('scrollend', function() {
                                    window.chrome.webview.postMessage('scrollEndUpdate;' + window.scrollY);
                                });
                            ";
                    await webView.ExecuteScriptAsync(jsScript);
                }));

                ExecuteWebviewAction(new Action(async () =>
                {
                    await webView.ExecuteScriptAsync(checkboxToggleScript);
                }));

                ExecuteWebviewAction(new Action(async () =>
                {
                    await webView.ExecuteScriptAsync(radioToggleScript);
                }));

                blockScrollUpdates = false;
            }

        }

        public Bitmap MakeScreenshot()
        {
            if (!IsInitialized()) return null;
            return null;
        }

        public void PrepareContentUpdate(bool preserveVerticalScrollPosition)
        {
            if (!IsInitialized()) return;
        }

        const string scrollScript =
            "var element = document.querySelector('[data-line=\"{0}\"]');\n" +
            "var headerOffset = 10;\n" +
            "var elementPosition = element.getBoundingClientRect().top;\n" +
            "var offsetPosition = elementPosition + window.pageYOffset - headerOffset;\n" +
            "window.scrollTo({{top: offsetPosition}});";

        const string checkboxToggleScript = @"
            var checkboxes = document.querySelectorAll('input[type=""checkbox""]');
            for (var i = 0; i < checkboxes.length; i++) {
                checkboxes[i].disabled = false;
            }
            if (!window.__checkboxToggleHandlerInstalled) {
                window.__checkboxToggleHandlerInstalled = true;
                document.addEventListener('click', function(e) {
                    if (e.target.tagName === 'INPUT' && e.target.type === 'checkbox') {
                        e.preventDefault();
                        var line = e.target.closest('[data-line]');
                        if (line) {
                            window.chrome.webview.postMessage('checkboxToggle;' + line.getAttribute('data-line'));
                        }
                    }
                }, true);
            }";

        const string radioToggleScript = @"
            var lis = document.querySelectorAll('li[data-line]');
            for (var i = 0; i < lis.length; i++) {
                var li = lis[i];
                var text = li.innerHTML;
                if (/^\([ xX]\)\s/.test(text)) {
                    li.innerHTML = text.replace(/^\([ xX]\)/, '<span class=""md-radio"">$&</span>');
                }
            }
            if (!window.__radioToggleHandlerInstalled) {
                window.__radioToggleHandlerInstalled = true;
                document.addEventListener('click', function(e) {
                    if (e.target.classList && e.target.classList.contains('md-radio')) {
                        e.preventDefault();
                        var line = e.target.closest('[data-line]');
                        if (line) {
                            var parentList = e.target.closest('ul, ol');
                            if (parentList) {
                                var radios = parentList.querySelectorAll('.md-radio');
                                for (var j = 0; j < radios.length; j++) {
                                    radios[j].textContent = '( )';
                                }
                            }
                            e.target.textContent = '(x)';
                            window.chrome.webview.postMessage('radioToggle;' + line.getAttribute('data-line'));
                        }
                    }
                }, true);
            }";


        public void ScrollToElementWithLineNo(int lineNo)
        {
            if (!IsInitialized()) return;
            if (lineNo <= 0) lineNo = 0;
            ExecuteWebviewAction(new Action(async () =>
            {
                await webView.ExecuteScriptAsync(string.Format(scrollScript, lineNo));
            }));
        }

        public void SetContent(string content, string body, string style, string currentDocumentPath)
        {
            if (!IsInitialized()) return;

            var currentPath = Path.GetDirectoryName(currentDocumentPath);
            var replaceFileMapping = "file:///" + currentPath.Replace('\\', '/');

            content = content.Replace(replaceFileMapping, virtualHostProtocol + virtualHostName);
            body = body.Replace(replaceFileMapping, virtualHostProtocol + virtualHostName);

            var fullReload = false;
            if (forceFullReload)
            {
                forceFullReload = false;
                fullReload = true;
            }
            if (this.currentDocumentPath != currentDocumentPath)
            {
                ExecuteWebviewAction(new Action(() =>
                {
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(virtualHostName, currentPath, CoreWebView2HostResourceAccessKind.Allow);
                }));
                this.currentDocumentPath = currentDocumentPath;
                fullReload = true;
            }

            // Detect the actual outline DOM element, not the string "outline-sidebar":
            // the outline CSS rules (.outline-sidebar {...}) are embedded in every page via
            // the style placeholder, so a bare substring check is always true and the toggle
            // would never force a full reload.
            var pageHasOutline = content.Contains("<nav id=\"outline-sidebar\"");
            if (!fullReload && this.currentPageHasOutline != pageHasOutline)
            {
                fullReload = true;
            }
            this.currentPageHasOutline = pageHasOutline;

            if (!fullReload && currentBody != null && currentStyle != null)
            {
                if (currentBody != body)
                {
                    currentBody = body;
                    ExecuteWebviewAction(new Action(async () =>
                    {
                        await webView.ExecuteScriptAsync(
                            "(function(){var om=document.getElementById('outline-main');if(om){om.innerHTML='" + HttpUtility.JavaScriptStringEncode(currentBody) + "';if(window.buildOutline)window.buildOutline();}else{document.body.innerHTML='" + HttpUtility.JavaScriptStringEncode(currentBody) + "';}})();" +
                            "if(typeof mermaid!=='undefined'){mermaid.run();}"
                        );
                        await webView.ExecuteScriptAsync(checkboxToggleScript);
                        await webView.ExecuteScriptAsync(radioToggleScript);
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
            if (!IsInitialized()) return;
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
            // Allow same-document fragment navigations (#anchor links)
            if (e.Uri.ToString().Contains("#"))
                return;

            if (e.Uri.ToString().StartsWith("about:blank"))
            {
                e.Cancel = true;
            }
            else if (!e.Uri.ToString().StartsWith("data:"))
            {
                var navUri = e.Uri.ToString();
                if (navUri.StartsWith(virtualHostProtocol + virtualHostName))
                {
                    e.Cancel = true;
                    var currentPath = Path.GetDirectoryName(currentDocumentPath);
                    navUri = navUri.Replace(virtualHostProtocol + virtualHostName, currentPath);
                    navUri = Uri.UnescapeDataString(navUri);
                    openLocalFileInNppAction(navUri); 
                } else
                {
                    forceFullReload = true;
                }
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
                if (webView != null)
                    webView.Invoke(action);
            }
            catch (Exception ex)
            {
            }
        }

        public bool IsInitialized()
        {
            return webViewInitialized && webView != null;
        }

        Dictionary<string, int> scrollYForFilename = new Dictionary<string, int>();
        bool blockScrollUpdates = true;

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();

            var splittedParams = message.Split(';');
            if (splittedParams.Length > 1)
            {
                string action = splittedParams[0];
                if (action == "scrollEndUpdate" && !blockScrollUpdates)
                {
                    try
                    {
                        var scrolly = int.Parse(splittedParams[1].Split('.')[0]);
                        if (scrollYForFilename.ContainsKey(currentDocumentPath))
                        {
                            scrollYForFilename[currentDocumentPath] = scrolly;
                        }
                        else
                        {
                            scrollYForFilename.Add(currentDocumentPath, scrolly);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (action == "checkboxToggle")
                {
                    try
                    {
                        int lineNo = int.Parse(splittedParams[1]);
                        if (CheckboxToggleAction != null)
                        {
                            CheckboxToggleAction(lineNo);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (action == "radioToggle")
                {
                    try
                    {
                        int lineNo = int.Parse(splittedParams[1]);
                        if (RadioToggleAction != null)
                        {
                            RadioToggleAction(lineNo);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void CurrentDocumentRenamed(string newDocumentPath)
        {
            if (scrollYForFilename.ContainsKey(currentDocumentPath))
            {
                scrollYForFilename.Add(newDocumentPath, scrollYForFilename[currentDocumentPath]);
                scrollYForFilename.Remove(currentDocumentPath);
            }

            currentDocumentPath = newDocumentPath;
        }

        public void StopScrollPositionTracking()
        {
            blockScrollUpdates = true;
        }

        public void ExportToPdf(string filePath)
        {
            if (!IsInitialized()) return;
            ExecuteWebviewAction(new Action(async () =>
            {
                await webView.CoreWebView2.PrintToPdfAsync(filePath);
            }));
        }

    }
}
