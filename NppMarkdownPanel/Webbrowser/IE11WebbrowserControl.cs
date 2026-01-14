using PanelCommon;
using NppMarkdownPanel.Forms;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NppMarkdownPanel.Entities;

namespace NppMarkdownPanel.Webbrowser
{
    public class IE11WebbrowserControl : IWebbrowserControl
    {
        private System.Windows.Forms.WebBrowser webBrowserPreview;

        private int lastVerticalScroll = 0;
        public Action<string> StatusTextChangedAction { get; set; }
        public Action RenderingDoneAction { get; set; }
        public Action AfterInitCompletedAction { get; set; }

        bool webViewInitialized = false;

        public void Initialize(int zoomLevel)
        {
            this.webBrowserPreview = new System.Windows.Forms.WebBrowser();
            this.webBrowserPreview.AllowWebBrowserDrop = false;
            this.webBrowserPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserPreview.Location = new System.Drawing.Point(0, 0);
            this.webBrowserPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.webBrowserPreview.MinimumSize = new System.Drawing.Size(18, 21);
            this.webBrowserPreview.Name = "webBrowserPreview";
            this.webBrowserPreview.Size = new System.Drawing.Size(811, 573);
            this.webBrowserPreview.TabIndex = 0;
            this.webBrowserPreview.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserPreview_DocumentCompleted);
            this.webBrowserPreview.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserPreview_Navigating);
            this.webBrowserPreview.StatusTextChanged += new System.EventHandler(this.webBrowserPreview_StatusTextChanged);
            webViewInitialized = true;
            if (AfterInitCompletedAction != null) AfterInitCompletedAction();
        }

        public void Dispose()
        {
        }

        public void AddToHost(Control host)
        {
            host.Controls.Add(this.webBrowserPreview);
        }

        private int CalculateAbsoluteYOffset(HtmlElement currentElement)
        {
            int baseY = currentElement.OffsetRectangle.Top;
            var offsetParent = currentElement.OffsetParent;
            while (offsetParent != null)
            {
                baseY += offsetParent.OffsetRectangle.Top;
                offsetParent = offsetParent.OffsetParent;
            }

            return baseY;
        }

        public void PrepareContentUpdate(bool preserveVerticalScrollPosition)
        {
            if (preserveVerticalScrollPosition)
            {
                SaveLastVerticalScrollPosition();
            }
            else
            {
                lastVerticalScroll = 0;
            }
        }

        public void ScrollToElementWithLineNo(int lineNo)
        {
            Application.DoEvents();
            if (webBrowserPreview.Document != null)
            {
                HtmlElement child = null;

                for (int i = lineNo; i >= 0; i--)
                {
                    var htmlElement = webBrowserPreview.Document.GetElementById(i.ToString());
                    if (htmlElement != null)
                    {
                        child = htmlElement;
                        break;
                    }
                }

                if (child != null)
                    webBrowserPreview.Document.Window.ScrollTo(0, CalculateAbsoluteYOffset(child) - 20);
            }
        }

        public void SetContent(string content, string body, string style, string currentDocumentPath)
        {
            webBrowserPreview.DocumentText = content;
        }

        /// <summary>
        /// Increase Zoomlevel in case of higher DPI settings
        /// </summary>
        public void SetZoomLevel(int zoomLevelVal)
        {
            Application.DoEvents();

            var browserInst = ((SHDocVw.IWebBrowser2)(webBrowserPreview.ActiveXInstance));

            int zoomLevel = zoomLevelVal;

            browserInst.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, zoomLevel, IntPtr.Zero);
        }

        /// <summary>
        /// Saves the last vertical scrollpositions, after reloading the position will be 0
        /// </summary>
        private void SaveLastVerticalScrollPosition()
        {
            if (webBrowserPreview.Document != null)
            {
                try
                {
                    lastVerticalScroll = webBrowserPreview.Document.GetElementsByTagName("HTML")[0].ScrollTop;
                }
                catch { }
            }
        }

        private void GoToLastVerticalScrollPosition()
        {
            webBrowserPreview.Document.Window.ScrollTo(0, lastVerticalScroll);
            Application.DoEvents();
        }


        private void webBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor.Current = Cursors.IBeam;
            GoToLastVerticalScrollPosition();
            if (RenderingDoneAction != null) RenderingDoneAction();

        }

        private void webBrowserPreview_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (!e.Url.ToString().StartsWith("about:blank"))
            {
                e.Cancel = true;
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(e.Url.ToString());
                p.Start();
            }
            else
            {
                // Jump to correct anchor on the page
                if (e.Url.ToString().Contains("#"))
                {
                    var urlParts = e.Url.ToString().Split('#');
                    e.Cancel = true;
                    var element = webBrowserPreview.Document.GetElementById(urlParts[1]);
                    if (element != null)
                    {
                        element.Focus();
                        element.ScrollIntoView(true);
                    }
                }
            }
        }

        private void webBrowserPreview_StatusTextChanged(object sender, EventArgs e)
        {
            if (StatusTextChangedAction != null)
                StatusTextChangedAction(webBrowserPreview.StatusText);
        }

        public Bitmap MakeScreenshot()
        {
            var screenshot = new Bitmap(webBrowserPreview.Width, webBrowserPreview.Height);
            ActiveXScreenShotMaker.GetImage(webBrowserPreview.ActiveXInstance, screenshot, Color.White);
            return screenshot;
        }

        public string GetRenderingEngineName()
        {
            return Settings.RENDERING_ENGINE_WEBVIEW1_IE11;
        }

        public bool IsInitialized()
        {
            return webViewInitialized && webBrowserPreview != null;
        }

    }
}
