using NppMarkdownPanel.Entities;
using NppMarkdownPanel.Generator;
using NppMarkdownPanel.Webbrowser;
using PanelCommon;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;
using Webview2Viewer;

namespace NppMarkdownPanel.Forms
{
    public partial class MarkdownPreviewForm : DockingFormBase, IViewerInterface
    {
        const string DEFAULT_HTML_BASE =
         @"<!DOCTYPE html>
            <html>
                <head>                    
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""></meta>
                    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8""></meta>
                    <title>{0}</title>
                    <style type=""text/css"">
                    {1}
                    </style>
                </head>
                <body style=""{2}"">
                    <nav id=""outline-sidebar"" class=""outline-sidebar"">
                        <div class=""outline-header"">Outline</div>
                        <div id=""outline-content"" class=""outline-content""></div>
                    </nav>
                    <div id=""outline-main"" class=""outline-main markdown-body"">{3}</div>
                    <button id=""outline-toggle"" class=""outline-toggle"" title=""Toggle Outline"" onclick=""document.getElementById('outline-sidebar').classList.toggle('collapsed');this.classList.toggle('collapsed');"">&#9776;</button>
OUTLINE_SCRIPT_PLACEHOLDER
                </body>
            </html>
            ";

        const string OUTLINE_SCRIPT = @"<script>
(function(){
    var content = document.getElementById('outline-content');
    var main = document.getElementById('outline-main');
    var sidebar = document.getElementById('outline-sidebar');
    var toggle = document.getElementById('outline-toggle');

    window.buildOutline = function() {
        content.innerHTML = '';
        var hs = main.querySelectorAll('h1, h2, h3, h4, h5, h6');
        if (hs.length === 0) {
            sidebar.style.display = 'none';
            toggle.style.display = 'none';
            return;
        }
        sidebar.style.display = '';
        toggle.style.display = '';
        var min = 7;
        for (var i = 0; i < hs.length; i++) {
            var lvl = parseInt(hs[i].tagName.substring(1));
            if (lvl < min) min = lvl;
        }
        for (var i = 0; i < hs.length; i++) {
            var h = hs[i];
            var lvl = parseInt(h.tagName.substring(1)) - min + 1;
            var txt = h.textContent || h.innerText || '';
            var ln = h.getAttribute('data-line') || '';
            var a = document.createElement('a');
            a.className = 'outline-item outline-l' + lvl;
            a.setAttribute('data-line', ln);
            a.textContent = txt;
            a.addEventListener('click', function(e) {
                e.preventDefault();
                var t = main.querySelector('[data-line=""' + this.getAttribute('data-line') + '""]');
                if (t) t.scrollIntoView({behavior:'smooth',block:'start'});
            });
            content.appendChild(a);
        }
    };

    window.addEventListener('scroll', function() {
        var items = content.querySelectorAll('.outline-item');
        if (items.length === 0) return;
        var hs = main.querySelectorAll('h1, h2, h3, h4, h5, h6');
        var sp = window.scrollY + 140;
        var fl = null;
        for (var i = 0; i < hs.length; i++) {
            if (hs[i].getBoundingClientRect().top + window.scrollY <= sp) fl = hs[i].getAttribute('data-line');
        }
        for (var i = 0; i < items.length; i++) items[i].classList.remove('active');
        if (fl) {
            var m = content.querySelector('[data-line=""' + fl + '""]');
            if (m) m.classList.add('active');
        }
    });

    buildOutline();
})();
</script>";

        const string MSG_NO_SUPPORTED_FILE_EXT = "<h3>The current file <u>{0}</u> has no valid Markdown file extension.</h3><div>Valid file extensions:{1}</div>";

        private Task<RenderResult> renderTask;

        private string htmlContentForExport;
        private string htmlContentForExportWithLightTheme;
        private string currentMarkdownText;
        private Settings settings;
        private string currentFilePath;
        private IWebbrowserControl webbrowserControl;
        private IWebbrowserControl webview1Instance;
        private IWebbrowserControl webview2Instance;
        private bool cleanupStarted;

        public void UpdateSettings(Settings newSettings)
        {
            this.settings = newSettings;

            var isDarkModeEnabled = newSettings.IsDarkModeEnabled;
            if (isDarkModeEnabled)
            {
                tbPreview.BackColor = Color.Black;
                btnSaveHtml.ForeColor = Color.White;
                btnSaveWithLightTheme.ForeColor = Color.White;
                statusStrip2.BackColor = Color.Black;
                toolStripStatusLabel1.ForeColor = Color.White;
            }
            else
            {
                tbPreview.BackColor = SystemColors.Control;
                btnSaveHtml.ForeColor = SystemColors.ControlText;
                btnSaveWithLightTheme.ForeColor = SystemColors.ControlText;
                statusStrip2.BackColor = SystemColors.Control;
                toolStripStatusLabel1.ForeColor = SystemColors.ControlText;
            }

            tbPreview.Visible = newSettings.ShowToolbar;
            statusStrip2.Visible = newSettings.ShowStatusbar;

            if (webbrowserControl.GetRenderingEngineName() != settings.RenderingEngine)
            {
                InitRenderingEngine(settings);
            }

        }

        private MarkdownService markdownService;
        private ActionRef<Message> wndProcCallback;

        public static IViewerInterface InitViewer(Settings settings, ActionRef<Message> wndProcCallback)
        {
            return new MarkdownPreviewForm(settings, wndProcCallback);
        }

        private MarkdownPreviewForm(Settings settings, ActionRef<Message> wndProcCallback)
        {
            InitializeComponent();

            this.wndProcCallback = wndProcCallback;
            markdownService = new MarkdownService(new MarkdigWrapper.MarkdigWrapper());
            markdownService.PreProcessorCommandFilename = settings.PreProcessorCommandFilename;
            markdownService.PreProcessorArguments = settings.PreProcessorArguments;
            markdownService.PostProcessorCommandFilename = settings.PostProcessorCommandFilename;
            markdownService.PostProcessorArguments = settings.PostProcessorArguments;
            this.settings = settings;
            panel1.Visible = true;

            InitRenderingEngine(settings);
        }

        private void InitRenderingEngine(Settings newSettings)
        {
            panel1.Controls.Clear();

            if (newSettings.IsRenderingEngineIE11())
            {
                if (webview1Instance == null)
                {
                    webbrowserControl = new IE11WebbrowserControl();
                    webbrowserControl.Initialize(newSettings.ZoomLevel);
                    webview1Instance = webbrowserControl;
                }
                else
                {
                    webbrowserControl = webview1Instance;
                }
            }
            else if (newSettings.IsRenderingEngineEdge())
            {
                if (webview2Instance == null)
                {
                    webbrowserControl = new Webview2WebbrowserControl();
                    webbrowserControl.Initialize(newSettings.ZoomLevel);
                    webview2Instance = webbrowserControl;
                }
                else
                {
                    webbrowserControl = webview2Instance;
                }
            }

            webbrowserControl.AddToHost(panel1);
            webbrowserControl.RenderingDoneAction = () => { HideScreenshotAndShowBrowser(); };
            webbrowserControl.StatusTextChangedAction = (status) => { toolStripStatusLabel1.Text = status; };
        }

        private RenderResult RenderHtmlInternal(string currentText, string filepath)
        {
            var defaultBodyStyle = "";
            var markdownStyleContent = GetCssContent();

            if (!IsValidFileExtension(currentFilePath))
            {
                var invalidExtensionMessageBody = string.Format(MSG_NO_SUPPORTED_FILE_EXT, Path.GetFileName(filepath), settings.SupportedFileExt);
                var invalidExtensionMessage = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), markdownStyleContent, defaultBodyStyle, invalidExtensionMessageBody);
                invalidExtensionMessage = InjectOutlineScript(invalidExtensionMessage);

                return new RenderResult(invalidExtensionMessage, invalidExtensionMessage, invalidExtensionMessageBody, markdownStyleContent, invalidExtensionMessage);
            }

            var resultForBrowser = markdownService.ConvertToHtml(currentText, filepath, true);
            var resultForExport = markdownService.ConvertToHtml(currentText, null, false);

            var markdownHtmlBrowser = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), markdownStyleContent, defaultBodyStyle, resultForBrowser);
            var markdownHtmlFileExport = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), markdownStyleContent, defaultBodyStyle, resultForExport);
            var markdownHtmlFileExportWithLightTheme = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), GetCssContent(true), defaultBodyStyle, resultForExport);

            markdownHtmlBrowser = InjectOutlineScript(markdownHtmlBrowser);
            markdownHtmlFileExport = InjectOutlineScript(markdownHtmlFileExport);
            markdownHtmlFileExportWithLightTheme = InjectOutlineScript(markdownHtmlFileExportWithLightTheme);

            return new RenderResult(markdownHtmlBrowser, markdownHtmlFileExport, resultForBrowser, markdownStyleContent, markdownHtmlFileExportWithLightTheme);
        }

        private string GetCssContent(bool forceLightTheme = false)
        {
            // Path of plugin directory
            var cssContent = "";

            var assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);

            var defaultCss = settings.IsDarkModeEnabled && !forceLightTheme ? Settings.DefaultDarkModeCssFile : Settings.DefaultCssFile;
            var customCssFile = settings.IsDarkModeEnabled && !forceLightTheme ? settings.CssDarkModeFileName : settings.CssFileName;
            if (File.Exists(customCssFile))
            {
                cssContent = File.ReadAllText(customCssFile);
            }
            else
            {
                cssContent = File.ReadAllText(assemblyPath + "\\" + defaultCss);
            }

            return cssContent;
        }

        public void RenderMarkdown(string currentText, string filepath, bool preserveVerticalScrollPosition = true)
        {
            Action renderAction = () =>
            {
                if (renderTask == null || renderTask.IsCompleted)
                {
                    MakeAndDisplayScreenShot();
                    webbrowserControl.PrepareContentUpdate(preserveVerticalScrollPosition);

                    var context = TaskScheduler.FromCurrentSynchronizationContext();
                    renderTask = new Task<RenderResult>(() => RenderHtmlInternal(currentText, filepath));
                    renderTask.ContinueWith((renderedText) =>
                    {
                        if (!cleanupStarted)
                        {
                            webbrowserControl.SetContent(renderedText.Result.ResultForBrowser, renderedText.Result.ResultBody, renderedText.Result.ResultStyle, currentFilePath);
                            htmlContentForExport = renderedText.Result.ResultForExport;
                            htmlContentForExportWithLightTheme = renderedText.Result.ResultForExportWithLightTheme;
                            currentMarkdownText = currentText;
                            if (!String.IsNullOrWhiteSpace(settings.HtmlFileName))
                            {
                                bool valid = Utils.ValidateFileSelection(settings.HtmlFileName, out string fullPath, out string error, "HTML Output");
                                if (valid)
                                {
                                    settings.HtmlFileName = fullPath; // the validation was run against this path, so we want to make sure the state of the preview matches that
                                    writeHtmlContentToFile(settings.HtmlFileName);
                                }
                            }
                            webbrowserControl.SetZoomLevel(settings.ZoomLevel);
                        }

                    }, context);
                    renderTask.Start();
                }
            };
            webbrowserControl.AfterInitCompletedAction = renderAction;
            if (webbrowserControl.IsInitialized())
            {
                webbrowserControl.AfterInitCompletedAction = null;
                renderAction();
            }
        }

        /// <summary>
        /// Makes and displays a screenshot of the current browser content to prevent it from flickering 
        /// while loading updated content
        /// </summary>
        private void MakeAndDisplayScreenShot()
        {
            Bitmap bm = webbrowserControl.MakeScreenshot();
            if (bm != null)
            {
                pictureBoxScreenshot.Image = bm;
                pictureBoxScreenshot.Visible = true;
            }

        }

        private void HideScreenshotAndShowBrowser()
        {
            if (pictureBoxScreenshot.Image != null)
            {
                pictureBoxScreenshot.Visible = false;
                pictureBoxScreenshot.Image = null;
            }
        }

        public void ScrollToElementWithLineNo(int lineNo)
        {
            webbrowserControl.ScrollToElementWithLineNo((int)lineNo);
        }

        protected override void WndProc(ref Message m)
        {
            wndProcCallback(ref m);

            //Continue the processing, as we only toggle
            base.WndProc(ref m);
        }

        private void btnSaveHtml_Click(object sender, EventArgs e)
        {
            ShowSaveAs(false);
        }


        private void btnSaveLightTheme_Click(object sender, EventArgs e)
        {
            ShowSaveAs(true);
        }

        private void ShowSaveAs(bool overrideLightTheme)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "html files (*.html, *.htm)|*.html;*.htm|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(currentFilePath);
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(currentFilePath);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    writeHtmlContentToFile(saveFileDialog.FileName, overrideLightTheme);
                }
            }
        }

        private void writeHtmlContentToFile(string filename, bool overrideLightTheme = false)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                File.WriteAllText(filename, overrideLightTheme ? htmlContentForExportWithLightTheme : htmlContentForExport);
            }
        }

        public bool IsValidFileExtension(string filename)
        {
            if (settings.AllowAllExtensions) return true;
            var currentExtension = Path.GetExtension(filename).ToLower();
            var matchExtensionList = false;
            try
            {
                matchExtensionList = settings.SupportedFileExt.Split(',').Any(ext => ext != null && currentExtension.Equals("." + ext.Trim().ToLower()));
                if (currentExtension == "" && settings.SupportFilesWithNoExt) matchExtensionList = true;
            }
            catch (Exception)
            {
            }

            return matchExtensionList;
        }

        public void SetMarkdownFilePath(string filepath, bool isRename = false)
        {

            if (isRename)
            {
                webbrowserControl.CurrentDocumentRenamed(filepath);
            }
            else
            {
                if (currentFilePath!= filepath)
                {
                    // We're about to switch to a new file. Stop tracking the current scolly value, as we can get unexpected results now...
                    webbrowserControl.StopScrollPositionTracking();
                }
            }

            currentFilePath = filepath;

        }

        public void Cleanup()
        {
            cleanupStarted = true;
            if (renderTask != null)
            {
                renderTask.Wait();
                renderTask = null;
            }
            if (webview2Instance != null)
            {
                webview2Instance.Dispose();
                webview2Instance = null;
            }
        }

        private static string InjectOutlineScript(string html)
        {
            return html.Replace("OUTLINE_SCRIPT_PLACEHOLDER", OUTLINE_SCRIPT);
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            ClipboardHelper.CopyToClipboard(htmlContentForExport, htmlContentForExport);
        }
    }
}
