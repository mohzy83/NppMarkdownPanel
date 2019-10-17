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
        public bool ShowToolbar { get; set; }
        public bool UseRegExp { get; set; }
        public string RegExpFileName { get; set; }

        public SettingsForms(int zoomLevel, string cssFileName, string htmlFileName, bool showToolbar, bool useRegExp, string regExpFileName)
        {
            ZoomLevel = zoomLevel;
            CssFileName = cssFileName;
            HtmlFileName = htmlFileName;
            ShowToolbar = showToolbar;
            UseRegExp = useRegExp;
            RegExpFileName = regExpFileName;

            InitializeComponent();

            trackBar1.Value = zoomLevel;
            tbCssFile.Text = cssFileName;
            tbHtmlFile.Text = htmlFileName;
            cbShowToolbar.Checked = showToolbar;
            cbUseRegExp.Checked = useRegExp;
            tbRegExpFile.Text = RegExpFileName;
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

        private void button1_Click(object sender, EventArgs e)
        {
            tbCssFile.Text = "";
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

        #region Use RegExp
        private void cbUseRegExp_CheckedChanged(object sender, EventArgs e)
        {
            UseRegExp = cbUseRegExp.Checked;
            tbRegExpFile.Enabled = UseRegExp;
            btnChooseRegExp.Enabled = UseRegExp;
            btnResetRegExp.Enabled = UseRegExp;
        }

        private void tbRegExpFile_TextChanged(object sender, EventArgs e)
        {
            RegExpFileName = tbRegExpFile.Text;
        }
        #endregion

        private void btnResetRegExp_Click(object sender, EventArgs e)
        {
            tbRegExpFile.Text = "RegExp3.txt";
        }

        private void btnChooseRegExp_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegExpFileName = openFileDialog.FileName;
                    tbRegExpFile.Text = RegExpFileName;
                }
            }
        }
    }
}
