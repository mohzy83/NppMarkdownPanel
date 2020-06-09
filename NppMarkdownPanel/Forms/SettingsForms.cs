using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class SettingsForms : Form
    {
        public int ZoomLevel { get; set; }
        public string CssFileName { get; set; }
        public string HtmlFileName { get; set; }
        public string MkdnExtensions { get; set; }
        public string HtmlExtensions { get; set; }
        public bool ShowToolbar { get; set; }

        public SettingsForms(int zoomLevel, string cssFileName, string htmlFileName, bool showToolbar, string mkdnExtensions, string htmlExtensions)
        {
            ZoomLevel = zoomLevel;
            CssFileName = cssFileName;
            HtmlFileName = htmlFileName;
            ShowToolbar = showToolbar;
            MkdnExtensions = mkdnExtensions;
            HtmlExtensions = htmlExtensions;
            InitializeComponent();

            trackBar1.Value = zoomLevel;
            tbCssFile.Text = cssFileName;
            tbHtmlFile.Text = htmlFileName;
            tbMkdnExts.Text = mkdnExtensions;
            tbHtmlExts.Text = htmlExtensions;
            cbShowToolbar.Checked = showToolbar;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            ZoomLevel = trackBar1.Value;
            lblZoomValue.Text = $"{ZoomLevel}%";
        }

        private void tbCssFile_TextChanged(object sender, EventArgs e)
        {
            CssFileName = tbCssFile.Text;
        }

        private void tbMkdnExts_TextChanged(object sender, EventArgs e)
        {
            MkdnExtensions = tbMkdnExts.Text;
        }

        private void btnResetMkdnExts_Click(object sender, EventArgs e)
        {
            tbMkdnExts.Text = ".md,.mkdn,.mkd";
        }

        private void tbHtmlExts_TextChanged(object sender, EventArgs e)
        {
            HtmlExtensions = tbHtmlExts.Text;
        }

        private void btnResetHtmlExts_Click(object sender, EventArgs e)
        {
            tbHtmlExts.Text = ".html,.htm";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbHtmlFile.Text) && String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
            {
                bool validHtmlPath = Utils.ValidateFileSelection(tbHtmlFile.Text, out string validPath, out string error, "HTML Output");
                if (!validHtmlPath)
                    sblInvalidHtmlPath.Text = error;
                else
                    tbHtmlFile.Text = validPath;
            }
            
            if (String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnChooseCss_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "css files (*.css)|*.css|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CssFileName = openFileDialog.FileName;
                    tbCssFile.Text = CssFileName;
                }
            }
        }

        private void btnResetCss_Click(object sender, EventArgs e)
        {
            tbCssFile.Text = "style.css";
        }

        #region Output HTML File
        private void tbHtmlFile_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbHtmlFile.Text))
            {
                bool valid = Utils.ValidateFileSelection(tbHtmlFile.Text, out string validPath, out string error, "HTML Output");
                if (valid)
                {
                    HtmlFileName = validPath;
                    if (!String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
                        sblInvalidHtmlPath.Text = String.Empty;
                }
                else
                {
                    sblInvalidHtmlPath.Text = error;
                }
            }
            else
            {
                HtmlFileName = String.Empty;
                if (!String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
                    sblInvalidHtmlPath.Text = String.Empty;
            }
        }

        private void tbHtmlFile_Leave(object sender, EventArgs e)
        {
            tbHtmlFile.Text = HtmlFileName;
        }

        private void btnChooseHtml_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "html files (*.html, *.htm)|*.html;*.htm|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    HtmlFileName = saveFileDialog.FileName;
                    tbHtmlFile.Text = HtmlFileName;
                }
            }
        }

        private void btnResetHtml_Click(object sender, EventArgs e)
        {
            tbHtmlFile.Text = "";
        }
        #endregion

        #region Show Toolbar
        private void cbShowToolbar_Changed(object sender, EventArgs e)
        {
            ShowToolbar = cbShowToolbar.Checked;
        }
        #endregion
    }
}
