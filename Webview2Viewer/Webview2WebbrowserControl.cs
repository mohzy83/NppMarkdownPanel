using Microsoft.Web.WebView2.Core;
using PanelCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Webview2Viewer
{
    public class Webview2WebbrowserControl : IWebbrowserControl, IDisposable
    {
        private const string VirtualHostProtocol = "http://";
        private const string VirtualHostName =
            WebviewResourceConstants.DocumentVirtualHostName;
        private const string ConfigFolderName = "MarkdownPanel";

        private const string OfflineBrowserArguments =
            "--disable-background-networking " +
            "--disable-component-update " +
            "--disable-domain-reliability " +
            "--disable-sync " +
            "--disable-default-apps " +
            "--disable-client-side-phishing-detection " +
            "--disable-dns-prefetch " +
            "--disable-breakpad " +
            "--no-first-run " +
            "--disable-features=OptimizationHints,MediaRouter,Translate," +
            "AutofillServerCommunication," +
            "CertificateTransparencyComponentUpdater";

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private bool webViewInitialized = false;
        private readonly bool offlineMode;
        private readonly string offlineMermaidScriptFileName;

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
        private bool documentFolderMappingConfigured;

        public Webview2WebbrowserControl()
            : this(false, null)
        {
        }

        public Webview2WebbrowserControl(
            bool offlineMode,
            string offlineMermaidScriptFileName)
        {
            this.offlineMode = offlineMode;

            string localScriptFileName;
            if (WebviewResourceConstants.TryGetExistingLocalFile(
                offlineMermaidScriptFileName,
                out localScriptFileName))
            {
                this.offlineMermaidScriptFileName =
                    localScriptFileName;
            }

            webView = null;
        }

        public void Dispose()
        {
            if (webView != null)
            {
                webView.Dispose();
                webView = null;
            }

            environment = null;
            webViewInitialized = false;
        }

        public async void Initialize(
            int zoomLevel,
            Action<string> openLocalFileInNppAction)
        {
            this.openLocalFileInNppAction = openLocalFileInNppAction;

            string profileFolderName =
                offlineMode ? "webview2-offline" : "webview2";
            string cacheDir = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                ConfigFolderName,
                profileFolderName);

            webView =
                new Microsoft.Web.WebView2.WinForms.WebView2();

            CoreWebView2EnvironmentOptions options =
                new CoreWebView2EnvironmentOptions();

            if (offlineMode)
            {
                options.AdditionalBrowserArguments =
                    OfflineBrowserArguments;

                // This property exists in recent WebView2 SDKs. Reflection
                // keeps the plugin compatible with older SDKs as well.
                TrySetBooleanProperty(
                    options,
                    "IsCustomCrashReportingEnabled",
                    true);
            }

            try
            {
                environment =
                    await CoreWebView2Environment.CreateAsync(
                        null,
                        cacheDir,
                        options);
                await webView.EnsureCoreWebView2Async(environment);

                ConfigureWebView(zoomLevel);
                webViewInitialized = true;

                Action afterInitialization =
                    AfterInitCompletedAction;
                if (afterInitialization != null)
                    afterInitialization();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "WebView2 Initialization Error: " + ex.Message,
                    "WebView2 Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ConfigureWebView(int zoomLevel)
        {
            webView.AccessibleName = "webView";
            webView.Name = "webView";
            webView.ZoomFactor = ConvertToZoomFactor(zoomLevel);
            webView.Location = new Point(1, 27);
            webView.Size = new Size(800, 424);
            webView.Dock = DockStyle.Fill;
            webView.TabIndex = 0;

            webView.NavigationStarting +=
                OnWebBrowser_NavigationStarting;
            webView.CoreWebView2.WebMessageReceived +=
                WebView_WebMessageReceived;
            webView.NavigationCompleted +=
                WebView_NavigationCompleted;

            if (offlineMode)
                ConfigureOfflineMode();

            webView.Source =
                new Uri("about:blank", UriKind.Absolute);
        }

        private void ConfigureOfflineMode()
        {
            CoreWebView2 coreWebView = webView.CoreWebView2;

            // Disable SmartScreen URL reputation checks when the installed
            // WebView2 SDK/runtime exposes the setting.
            TrySetBooleanProperty(
                coreWebView.Settings,
                "IsReputationCheckingRequired",
                false);

            if (!String.IsNullOrWhiteSpace(
                offlineMermaidScriptFileName))
            {
                string scriptDirectory = Path.GetDirectoryName(
                    offlineMermaidScriptFileName);

                coreWebView.SetVirtualHostNameToFolderMapping(
                    WebviewResourceConstants
                        .OfflineAssetsVirtualHostName,
                    scriptDirectory,
                    CoreWebView2HostResourceAccessKind.DenyCors);
            }

            coreWebView.AddWebResourceRequestedFilter(
                "*",
                CoreWebView2WebResourceContext.All);
            coreWebView.WebResourceRequested +=
                CoreWebView2_WebResourceRequested;
            coreWebView.NewWindowRequested +=
                CoreWebView2_NewWindowRequested;
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

            string currentPath =
                Path.GetDirectoryName(currentDocumentPath);
            bool canMapDocumentFolder =
                !String.IsNullOrWhiteSpace(currentPath) &&
                (!offlineMode ||
                 !WebviewResourceConstants.IsNetworkPath(currentPath));

            if (canMapDocumentFolder)
            {
                string replaceFileMapping =
                    "file:///" + currentPath.Replace('\\', '/');
                string virtualBaseUrl =
                    VirtualHostProtocol + VirtualHostName;

                content = content.Replace(
                    replaceFileMapping,
                    virtualBaseUrl);
                body = body.Replace(
                    replaceFileMapping,
                    virtualBaseUrl);
            }

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
                    if (documentFolderMappingConfigured)
                    {
                        webView.CoreWebView2
                            .ClearVirtualHostNameToFolderMapping(
                                VirtualHostName);
                        documentFolderMappingConfigured = false;
                    }

                    if (canMapDocumentFolder)
                    {
                        CoreWebView2HostResourceAccessKind accessKind =
                            offlineMode
                                ? CoreWebView2HostResourceAccessKind
                                    .DenyCors
                                : CoreWebView2HostResourceAccessKind
                                    .Allow;

                        webView.CoreWebView2
                            .SetVirtualHostNameToFolderMapping(
                                VirtualHostName,
                                currentPath,
                                accessKind);
                        documentFolderMappingConfigured = true;
                    }
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

        void OnWebBrowser_NavigationStarting(
            object sender,
            CoreWebView2NavigationStartingEventArgs e)
        {
            string navUri = e.Uri ?? String.Empty;
            string documentVirtualBaseUrl =
                VirtualHostProtocol + VirtualHostName;

            if (navUri.StartsWith(
                documentVirtualBaseUrl,
                StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;

                string currentPath =
                    Path.GetDirectoryName(currentDocumentPath);
                if (String.IsNullOrWhiteSpace(currentPath) ||
                    (offlineMode &&
                     WebviewResourceConstants.IsNetworkPath(
                         currentPath)))
                {
                    return;
                }

                string localPath = navUri.Replace(
                    documentVirtualBaseUrl,
                    currentPath);
                localPath = Uri.UnescapeDataString(localPath);

                if (openLocalFileInNppAction != null)
                    openLocalFileInNppAction(localPath);

                return;
            }

            Uri parsedUri;
            if (!Uri.TryCreate(
                navUri,
                UriKind.Absolute,
                out parsedUri))
            {
                if (offlineMode)
                    e.Cancel = true;
                return;
            }

            if (String.Equals(
                parsedUri.Scheme,
                "about",
                StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                return;
            }

            if (offlineMode && !IsAllowedOfflineUri(parsedUri))
            {
                e.Cancel = true;
                forceFullReload = true;
                return;
            }

            if (!String.Equals(
                parsedUri.Scheme,
                "data",
                StringComparison.OrdinalIgnoreCase) &&
                parsedUri.Fragment.Length == 0)
            {
                forceFullReload = true;
            }
        }

        private void CoreWebView2_WebResourceRequested(
            object sender,
            CoreWebView2WebResourceRequestedEventArgs e)
        {
            Uri requestUri;
            if (Uri.TryCreate(
                e.Request.Uri,
                UriKind.Absolute,
                out requestUri) &&
                IsAllowedOfflineUri(requestUri))
            {
                return;
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(
                "Blocked by NppMarkdownPanel Offline-mode.");
            MemoryStream responseStream =
                new MemoryStream(responseBytes, false);

            e.Response = environment.CreateWebResourceResponse(
                responseStream,
                403,
                "Blocked by Offline-mode",
                "Content-Type: text/plain; charset=utf-8\r\n" +
                "Cache-Control: no-store\r\n");
        }

        private void CoreWebView2_NewWindowRequested(
            object sender,
            CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
        }

        private bool IsAllowedOfflineUri(Uri uri)
        {
            if (uri == null)
                return false;

            string scheme = uri.Scheme;

            if (String.Equals(
                scheme,
                Uri.UriSchemeHttp,
                StringComparison.OrdinalIgnoreCase) ||
                String.Equals(
                    scheme,
                    Uri.UriSchemeHttps,
                    StringComparison.OrdinalIgnoreCase))
            {
                if (String.Equals(
                    uri.Host,
                    VirtualHostName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return documentFolderMappingConfigured;
                }

                return String.Equals(
                    uri.Host,
                    WebviewResourceConstants
                        .OfflineAssetsVirtualHostName,
                    StringComparison.OrdinalIgnoreCase) &&
                    !String.IsNullOrWhiteSpace(
                        offlineMermaidScriptFileName);
            }

            if (String.Equals(
                scheme,
                Uri.UriSchemeFile,
                StringComparison.OrdinalIgnoreCase))
            {
                return !uri.IsUnc &&
                    !WebviewResourceConstants.IsNetworkPath(
                        uri.LocalPath);
            }

            if (String.Equals(
                scheme,
                "about",
                StringComparison.OrdinalIgnoreCase) ||
                String.Equals(
                    scheme,
                    "data",
                    StringComparison.OrdinalIgnoreCase) ||
                String.Equals(
                    scheme,
                    "blob",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Deny WebSocket, FTP, custom protocols, and every other
            // unrecognized scheme in Offline-mode.
            return false;
        }

        private static void TrySetBooleanProperty(
            object target,
            string propertyName,
            bool value)
        {
            if (target == null)
                return;

            try
            {
                var property = target.GetType().GetProperty(
                    propertyName);

                if (property != null &&
                    property.CanWrite &&
                    property.PropertyType == typeof(bool))
                {
                    property.SetValue(target, value, null);
                }
            }
            catch (Exception)
            {
                // Optional WebView2 properties may not be exposed by an
                // older installed runtime. Request interception and CSP
                // remain active regardless.
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
