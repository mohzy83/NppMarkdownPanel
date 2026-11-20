using NppMarkdownPanel.Entities;
using PanelCommon;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class SettingsForm : Form
    {
        public int ZoomLevel { get; set; }
        public string CssFileName { get; set; }
        public string CssDarkModeFileName { get; set; }
        public string HtmlFileName { get; set; }
        public bool ShowToolbar { get; set; }
        public string SupportedFileExt { get; set; }
        public bool AllowAllExtensions { get; set; }
        public bool SupportFilesWithNoExt { get; set; }
        public bool AutoShowPanel { get; set; }
        public bool ShowStatusbar { get; set; }
        public bool EnableThreeStateToggle { get; set; }
        public string RenderingEngine { get; set; }
        public bool OfflineMode { get; set; }
        public string OfflineMermaidScriptFileName { get; set; }

        private GroupBox gbOfflineMode;
        private CheckBox cbOfflineMode;
        private Label lblOfflineMermaidScript;
        private TextBox tbOfflineMermaidScript;
        private Button btnChooseOfflineMermaidScript;


        public SettingsForm(Settings settings)
        {
            ZoomLevel = settings.ZoomLevel;
            CssFileName = settings.CssFileName;
            CssDarkModeFileName = settings.CssDarkModeFileName;
            HtmlFileName = settings.HtmlFileName;
            ShowToolbar = settings.ShowToolbar;
            SupportedFileExt = settings.SupportedFileExt;
            AutoShowPanel = settings.AutoShowPanel;
            ShowStatusbar = settings.ShowStatusbar;
            RenderingEngine = settings.RenderingEngine;
            AllowAllExtensions = settings.AllowAllExtensions;
            SupportFilesWithNoExt = settings.SupportFilesWithNoExt;
            EnableThreeStateToggle = settings.EnableThreeStateToggle;
            OfflineMode = settings.OfflineMode;
            OfflineMermaidScriptFileName =
                settings.OfflineMermaidScriptFileName ?? String.Empty;

            InitializeComponent();
            InitializeOfflineModeControls();

            trackBar1.Value = ZoomLevel;
            lblZoomValue.Text = $"{ZoomLevel}%";
            tbCssFile.Text = CssFileName;
            tbDarkmodeCssFile.Text = CssDarkModeFileName;
            tbHtmlFile.Text = HtmlFileName;
            cbShowToolbar.Checked = ShowToolbar;
            tbFileExt.Text = SupportedFileExt;
            cbAutoShowPanel.Checked = AutoShowPanel;
            cbShowStatusbar.Checked = ShowStatusbar;
            cbAllowAllExtensions.Checked = AllowAllExtensions;
            cbFilesWithNoExt.Checked = SupportFilesWithNoExt;
            cbEnableThreeStateToggle.Checked = EnableThreeStateToggle;

            if (settings.IsRenderingEngineIE11())
            {
                comboRenderingEngine.SelectedIndex = 1;
            }
            else if (settings.IsRenderingEngineEdge())
            {
                comboRenderingEngine.SelectedIndex = 0;
            }
        }

        private void InitializeOfflineModeControls()
        {
            const int addedHeight = 110;

            SuspendLayout();
            ClientSize = new Size(ClientSize.Width, ClientSize.Height + addedHeight);

            gbOfflineMode = new GroupBox();
            gbOfflineMode.Anchor =
                AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            gbOfflineMode.Location = new Point(12, 583);
            gbOfflineMode.Name = "gbOfflineMode";
            gbOfflineMode.Size = new Size(672, 98);
            gbOfflineMode.TabIndex = 29;
            gbOfflineMode.TabStop = false;
            gbOfflineMode.Text = "WebView2 Offline-mode";

            cbOfflineMode = new CheckBox();
            cbOfflineMode.AutoSize = true;
            cbOfflineMode.Location = new Point(12, 23);
            cbOfflineMode.Name = "cbOfflineMode";
            cbOfflineMode.Size = new Size(399, 21);
            cbOfflineMode.TabIndex = 0;
            cbOfflineMode.Text =
                "Block non-local WebView2 requests and background networking";
            cbOfflineMode.UseVisualStyleBackColor = true;
            cbOfflineMode.CheckedChanged += cbOfflineMode_CheckedChanged;

            lblOfflineMermaidScript = new Label();
            lblOfflineMermaidScript.AutoSize = true;
            lblOfflineMermaidScript.Location = new Point(12, 62);
            lblOfflineMermaidScript.Name = "lblOfflineMermaidScript";
            lblOfflineMermaidScript.Size = new Size(151, 17);
            lblOfflineMermaidScript.TabIndex = 1;
            lblOfflineMermaidScript.Text = "Local Mermaid JS file:";

            tbOfflineMermaidScript = new TextBox();
            tbOfflineMermaidScript.Anchor =
                AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            tbOfflineMermaidScript.Location = new Point(170, 59);
            tbOfflineMermaidScript.Name = "tbOfflineMermaidScript";
            tbOfflineMermaidScript.Size = new Size(444, 25);
            tbOfflineMermaidScript.TabIndex = 2;
            tbOfflineMermaidScript.TextChanged +=
                tbOfflineMermaidScript_TextChanged;

            btnChooseOfflineMermaidScript = new Button();
            btnChooseOfflineMermaidScript.Anchor =
                AnchorStyles.Right | AnchorStyles.Top;
            btnChooseOfflineMermaidScript.Location = new Point(620, 57);
            btnChooseOfflineMermaidScript.Name =
                "btnChooseOfflineMermaidScript";
            btnChooseOfflineMermaidScript.Size = new Size(39, 27);
            btnChooseOfflineMermaidScript.TabIndex = 3;
            btnChooseOfflineMermaidScript.Text = "...";
            btnChooseOfflineMermaidScript.UseVisualStyleBackColor = true;
            btnChooseOfflineMermaidScript.Click +=
                btnChooseOfflineMermaidScript_Click;

            gbOfflineMode.Controls.Add(cbOfflineMode);
            gbOfflineMode.Controls.Add(lblOfflineMermaidScript);
            gbOfflineMode.Controls.Add(tbOfflineMermaidScript);
            gbOfflineMode.Controls.Add(btnChooseOfflineMermaidScript);
            Controls.Add(gbOfflineMode);

            cbOfflineMode.Checked = OfflineMode;
            tbOfflineMermaidScript.Text = OfflineMermaidScriptFileName;
            UpdateOfflineModeControls();

            ResumeLayout(false);
            PerformLayout();
        }

        private void cbOfflineMode_CheckedChanged(object sender, EventArgs e)
        {
            OfflineMode = cbOfflineMode.Checked;
            UpdateOfflineModeControls();
        }

        private void tbOfflineMermaidScript_TextChanged(
            object sender,
            EventArgs e)
        {
            OfflineMermaidScriptFileName = tbOfflineMermaidScript.Text;
        }

        private void btnChooseOfflineMermaidScript_Click(
            object sender,
            EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter =
                    "JavaScript files (*.js)|*.js|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbOfflineMermaidScript.Text = openFileDialog.FileName;
                }
            }
        }

        private void UpdateOfflineModeControls()
        {
            bool localScriptEnabled = OfflineMode;

            lblOfflineMermaidScript.Enabled = localScriptEnabled;
            tbOfflineMermaidScript.Enabled = localScriptEnabled;
            btnChooseOfflineMermaidScript.Enabled = localScriptEnabled;
        }

        private bool ValidateOfflineModeSettings()
        {
            if (!OfflineMode ||
                String.IsNullOrWhiteSpace(tbOfflineMermaidScript.Text))
            {
                OfflineMermaidScriptFileName =
                    tbOfflineMermaidScript.Text.Trim();
                return true;
            }

            string fullPath;
            if (!WebviewResourceConstants.TryGetExistingLocalFile(
                tbOfflineMermaidScript.Text,
                out fullPath))
            {
                MessageBox.Show(
                    "The local Mermaid JavaScript file must be an existing " +
                    "file on a local drive. UNC paths and mapped network " +
                    "drives are not allowed in Offline-mode.",
                    "Invalid Offline-mode Mermaid File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (!String.Equals(
                Path.GetExtension(fullPath),
                ".js",
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "The local Mermaid file must have a .js extension.",
                    "Invalid Offline-mode Mermaid File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            OfflineMermaidScriptFileName = fullPath;
            tbOfflineMermaidScript.Text = fullPath;
            return true;
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
        private void tbDarkmodeCssFile_TextChanged(object sender, EventArgs e)
        {
            CssDarkModeFileName = tbDarkmodeCssFile.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateOfflineModeSettings())
                return;

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
                    if ((sender as Button).Name == "btnChooseCss")
                    {
                        CssFileName = openFileDialog.FileName;
                        tbCssFile.Text = CssFileName;
                    }
                    else if ((sender as Button).Name == "btnChooseDarkmodeCss")
                    {
                        CssDarkModeFileName = openFileDialog.FileName;
                        tbDarkmodeCssFile.Text = CssDarkModeFileName;
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbCssFile.Text = "";
        }
        private void btnDefaultDarkmodeCss_Click(object sender, EventArgs e)
        {
            tbDarkmodeCssFile.Text = "";
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

        private void btnDefaultFileExt_Click(object sender, EventArgs e)
        {
            tbFileExt.Text = Settings.DEFAULT_SUPPORTED_FILE_EXT;
        }

        private void tbFileExt_TextChanged(object sender, EventArgs e)
        {
            SupportedFileExt = tbFileExt.Text;
        }

        private void cbAutoShowPanel_CheckedChanged(object sender, EventArgs e)
        {
            AutoShowPanel = cbAutoShowPanel.Checked;
        }

        private void cbShowStatusbar_CheckedChanged(object sender, EventArgs e)
        {
            ShowStatusbar = cbShowStatusbar.Checked;
        }

        private void comboRenderingEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboRenderingEngine.SelectedIndex == 0)
            {
                RenderingEngine = Settings.RENDERING_ENGINE_WEBVIEW2_EDGE;
            }
            else if (comboRenderingEngine.SelectedIndex == 1)
            {
                RenderingEngine = Settings.RENDERING_ENGINE_WEBVIEW1_IE11;
            }
            else
            {
                throw new NotSupportedException("Rendering Engine with id " + comboRenderingEngine.SelectedIndex + " not supported!");
            }

            if (cbOfflineMode != null)
                UpdateOfflineModeControls();
        }

        private void cbAllowAllExtensions_CheckedChanged(object sender, EventArgs e)
        {
            AllowAllExtensions = cbAllowAllExtensions.Checked;
            if (AllowAllExtensions)
            {
                tbFileExt.Enabled = false;
                cbFilesWithNoExt.Enabled = false;   
            }
            else
            {
                tbFileExt.Enabled = true;
                cbFilesWithNoExt.Enabled = true;
            }
        }

        private void cbFilesWithNoExt_CheckedChanged(object sender, EventArgs e)
        {
            SupportFilesWithNoExt = cbFilesWithNoExt.Checked;
        }

        private void cbEnableThreeStateToggle_CheckedChanged(object sender, EventArgs e)
        {
            EnableThreeStateToggle = cbEnableThreeStateToggle.Checked;
        }
    }
}
