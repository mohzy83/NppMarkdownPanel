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
using Webview2Viewer;

namespace NppMarkdownPanel.Forms
{
    public partial class MarkdownPreviewForm : Form, IViewerInterface
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
                <body class=""markdown-body"" style=""{2}"">
                {3}
                </body>
            </html>
            ";

        const string MSG_NO_SUPPORTED_FILE_EXT = "<h3>The current file <u>{0}</u> has no valid Markdown file extension.</h3><div>Valid file extensions:{1}</div>";

        private Task<RenderResult> renderTask;

        private string htmlContentForExport;
        private Settings settings;
        private string currentFilePath;
        private IWebbrowserControl webbrowserControl;
        private IWebbrowserControl webview1Instance;
        private IWebbrowserControl webview2Instance;

        public void UpdateSettings(Settings settings)
        {
            this.settings = settings;

            var isDarkModeEnabled = settings.IsDarkModeEnabled;
            if (isDarkModeEnabled)
            {
                tbPreview.BackColor = Color.Black;
                btnSaveHtml.ForeColor = Color.White;
                statusStrip2.BackColor = Color.Black;
                toolStripStatusLabel1.ForeColor = Color.White;
            }
            else
            {
                tbPreview.BackColor = SystemColors.Control;
                btnSaveHtml.ForeColor = SystemColors.ControlText;
                statusStrip2.BackColor = SystemColors.Control;
                toolStripStatusLabel1.ForeColor = SystemColors.ControlText;
            }

            tbPreview.Visible = settings.ShowToolbar;
            statusStrip2.Visible = settings.ShowStatusbar;

            if (webbrowserControl != null)
            {
                if (webbrowserControl.GetRenderingEngineName() != settings.RenderingEngine)
                {
                    InitRenderingEngine(settings);
                }
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

        private void InitRenderingEngine(Settings settings)
        {
            panel1.Controls.Clear();
          

            if (settings.IsRenderingEngineIE11())
            {
                if (webview1Instance == null)
                {
                    webbrowserControl = new IE11WebbrowserControl();
                    webbrowserControl.Initialize(settings.ZoomLevel);
                    webview1Instance = webbrowserControl;
                }
                else
                {
                    webbrowserControl = webview1Instance;
                }
            }
            else if (settings.IsRenderingEngineEdge())
            {
                if (webview2Instance == null)
                {
                    webbrowserControl = new Webview2WebbrowserControl();
                    webbrowserControl.Initialize(settings.ZoomLevel);
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
            var markdownStyleContent = GetCssContent(filepath);

            if (!IsValidFileExtension(currentFilePath))
            {
                var invalidExtensionMessageBody = string.Format(MSG_NO_SUPPORTED_FILE_EXT, Path.GetFileName(filepath), settings.SupportedFileExt);
                var invalidExtensionMessage = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), markdownStyleContent, defaultBodyStyle, invalidExtensionMessageBody);

                return new RenderResult(invalidExtensionMessage, invalidExtensionMessage, invalidExtensionMessageBody, markdownStyleContent);
            }

            var resultForBrowser = markdownService.ConvertToHtml(currentText, filepath, true);
            var resultForExport = markdownService.ConvertToHtml(currentText, null, false);

            var markdownHtmlBrowser = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), markdownStyleContent, defaultBodyStyle, resultForBrowser);
            var markdownHtmlFileExport = string.Format(DEFAULT_HTML_BASE, Path.GetFileName(filepath), markdownStyleContent, defaultBodyStyle, resultForExport);
            return new RenderResult(markdownHtmlBrowser, markdownHtmlFileExport, resultForBrowser, markdownStyleContent);
        }

        private string GetCssContent(string filepath)
        {
            // Path of plugin directory
            var cssContent = "";

            var assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);

            var defaultCss = settings.IsDarkModeEnabled ? Settings.DefaultDarkModeCssFile : Settings.DefaultCssFile;
            var customCssFile = settings.IsDarkModeEnabled ? settings.CssDarkModeFileName : settings.CssFileName;
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
            if (renderTask == null || renderTask.IsCompleted)
            {
                MakeAndDisplayScreenShot();
                webbrowserControl.PrepareContentUpdate(preserveVerticalScrollPosition);

                var context = TaskScheduler.FromCurrentSynchronizationContext();
                renderTask = new Task<RenderResult>(() => RenderHtmlInternal(currentText, filepath));
                renderTask.ContinueWith((renderedText) =>
                {
                    webbrowserControl.SetContent(renderedText.Result.ResultForBrowser, renderedText.Result.ResultBody, renderedText.Result.ResultStyle, currentFilePath);
                    htmlContentForExport = renderedText.Result.ResultForExport;
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
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "html files (*.html, *.htm)|*.html;*.htm|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(currentFilePath);
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(currentFilePath);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    writeHtmlContentToFile(saveFileDialog.FileName);
                }
            }
        }

        private void writeHtmlContentToFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                File.WriteAllText(filename, htmlContentForExport);
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
            }
            catch (Exception)
            {
            }

            return matchExtensionList;
        }

        public void SetMarkdownFilePath(string filepath)
        {
            currentFilePath = filepath;
        }
    }
}
