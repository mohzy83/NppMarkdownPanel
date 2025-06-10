namespace NppMarkdownPanel.Forms
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCssFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblZoomValue = new System.Windows.Forms.Label();
            this.btnChooseCss = new System.Windows.Forms.Button();
            this.btnDefaultCss = new System.Windows.Forms.Button();
            this.btnResetHtml = new System.Windows.Forms.Button();
            this.btnChooseHtml = new System.Windows.Forms.Button();
            this.tbHtmlFile = new System.Windows.Forms.TextBox();
            this.lblHtmlFile = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.sblInvalidHtmlPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnDefaultDarkmodeCss = new System.Windows.Forms.Button();
            this.btnChooseDarkmodeCss = new System.Windows.Forms.Button();
            this.tbDarkmodeCssFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbFileExt = new System.Windows.Forms.TextBox();
            this.btnDefaultFileExt = new System.Windows.Forms.Button();
            this.cbAutoShowPanel = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboRenderingEngine = new System.Windows.Forms.ComboBox();
            this.cbShowToolbar = new System.Windows.Forms.CheckBox();
            this.cbShowStatusbar = new System.Windows.Forms.CheckBox();
            this.cbAllowAllExtensions = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(2, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(693, 61);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Markdown Panel Settings";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::NppMarkdownPanel.Properties.Resources.markdown_16x16_solid;
            this.pictureBox1.Location = new System.Drawing.Point(10, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 20);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(468, 548);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 36);
            this.btnSave.TabIndex = 20;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(579, 548);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 36);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "CSS File:";
            // 
            // tbCssFile
            // 
            this.tbCssFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCssFile.Location = new System.Drawing.Point(170, 119);
            this.tbCssFile.Name = "tbCssFile";
            this.tbCssFile.Size = new System.Drawing.Size(386, 25);
            this.tbCssFile.TabIndex = 2;
            this.tbCssFile.TextChanged += new System.EventHandler(this.tbCssFile_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 217);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Zoom Level:";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(164, 217);
            this.trackBar1.Maximum = 800;
            this.trackBar1.Minimum = 80;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(474, 56);
            this.trackBar1.TabIndex = 8;
            this.trackBar1.TickFrequency = 5;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Value = 90;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // lblZoomValue
            // 
            this.lblZoomValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZoomValue.AutoSize = true;
            this.lblZoomValue.Location = new System.Drawing.Point(644, 232);
            this.lblZoomValue.Name = "lblZoomValue";
            this.lblZoomValue.Size = new System.Drawing.Size(33, 17);
            this.lblZoomValue.TabIndex = 7;
            this.lblZoomValue.Text = "90%";
            // 
            // btnChooseCss
            // 
            this.btnChooseCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseCss.Location = new System.Drawing.Point(562, 117);
            this.btnChooseCss.Name = "btnChooseCss";
            this.btnChooseCss.Size = new System.Drawing.Size(39, 25);
            this.btnChooseCss.TabIndex = 3;
            this.btnChooseCss.Text = "...";
            this.btnChooseCss.UseVisualStyleBackColor = true;
            this.btnChooseCss.Click += new System.EventHandler(this.btnChooseCss_Click);
            // 
            // btnDefaultCss
            // 
            this.btnDefaultCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultCss.Location = new System.Drawing.Point(607, 117);
            this.btnDefaultCss.Name = "btnDefaultCss";
            this.btnDefaultCss.Size = new System.Drawing.Size(73, 26);
            this.btnDefaultCss.TabIndex = 4;
            this.btnDefaultCss.Text = "Default";
            this.btnDefaultCss.UseVisualStyleBackColor = true;
            this.btnDefaultCss.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnResetHtml
            // 
            this.btnResetHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetHtml.Location = new System.Drawing.Point(607, 294);
            this.btnResetHtml.Name = "btnResetHtml";
            this.btnResetHtml.Size = new System.Drawing.Size(73, 28);
            this.btnResetHtml.TabIndex = 11;
            this.btnResetHtml.Text = "Default";
            this.btnResetHtml.UseVisualStyleBackColor = true;
            this.btnResetHtml.Click += new System.EventHandler(this.btnResetHtml_Click);
            // 
            // btnChooseHtml
            // 
            this.btnChooseHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseHtml.Location = new System.Drawing.Point(562, 294);
            this.btnChooseHtml.Name = "btnChooseHtml";
            this.btnChooseHtml.Size = new System.Drawing.Size(39, 27);
            this.btnChooseHtml.TabIndex = 10;
            this.btnChooseHtml.Text = "...";
            this.btnChooseHtml.UseVisualStyleBackColor = true;
            this.btnChooseHtml.Click += new System.EventHandler(this.btnChooseHtml_Click);
            // 
            // tbHtmlFile
            // 
            this.tbHtmlFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHtmlFile.Location = new System.Drawing.Point(170, 296);
            this.tbHtmlFile.Name = "tbHtmlFile";
            this.tbHtmlFile.Size = new System.Drawing.Size(386, 25);
            this.tbHtmlFile.TabIndex = 9;
            this.tbHtmlFile.TextChanged += new System.EventHandler(this.tbHtmlFile_TextChanged);
            this.tbHtmlFile.Leave += new System.EventHandler(this.tbHtmlFile_Leave);
            // 
            // lblHtmlFile
            // 
            this.lblHtmlFile.AutoSize = true;
            this.lblHtmlFile.Location = new System.Drawing.Point(12, 286);
            this.lblHtmlFile.Name = "lblHtmlFile";
            this.lblHtmlFile.Size = new System.Drawing.Size(125, 51);
            this.lblHtmlFile.TabIndex = 10;
            this.lblHtmlFile.Text = "Automatically Save \r\nHTML from Current \r\nPreview to this File:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sblInvalidHtmlPath});
            this.statusStrip1.Location = new System.Drawing.Point(0, 596);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(696, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // sblInvalidHtmlPath
            // 
            this.sblInvalidHtmlPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sblInvalidHtmlPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.sblInvalidHtmlPath.ForeColor = System.Drawing.Color.Red;
            this.sblInvalidHtmlPath.Name = "sblInvalidHtmlPath";
            this.sblInvalidHtmlPath.Size = new System.Drawing.Size(681, 16);
            this.sblInvalidHtmlPath.Spring = true;
            // 
            // btnDefaultDarkmodeCss
            // 
            this.btnDefaultDarkmodeCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultDarkmodeCss.Location = new System.Drawing.Point(607, 165);
            this.btnDefaultDarkmodeCss.Name = "btnDefaultDarkmodeCss";
            this.btnDefaultDarkmodeCss.Size = new System.Drawing.Size(73, 26);
            this.btnDefaultDarkmodeCss.TabIndex = 7;
            this.btnDefaultDarkmodeCss.Text = "Default";
            this.btnDefaultDarkmodeCss.UseVisualStyleBackColor = true;
            this.btnDefaultDarkmodeCss.Click += new System.EventHandler(this.btnDefaultDarkmodeCss_Click);
            // 
            // btnChooseDarkmodeCss
            // 
            this.btnChooseDarkmodeCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseDarkmodeCss.Location = new System.Drawing.Point(562, 165);
            this.btnChooseDarkmodeCss.Name = "btnChooseDarkmodeCss";
            this.btnChooseDarkmodeCss.Size = new System.Drawing.Size(39, 25);
            this.btnChooseDarkmodeCss.TabIndex = 6;
            this.btnChooseDarkmodeCss.Text = "...";
            this.btnChooseDarkmodeCss.UseVisualStyleBackColor = true;
            this.btnChooseDarkmodeCss.Click += new System.EventHandler(this.btnChooseCss_Click);
            // 
            // tbDarkmodeCssFile
            // 
            this.tbDarkmodeCssFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDarkmodeCssFile.Location = new System.Drawing.Point(170, 167);
            this.tbDarkmodeCssFile.Name = "tbDarkmodeCssFile";
            this.tbDarkmodeCssFile.Size = new System.Drawing.Size(386, 25);
            this.tbDarkmodeCssFile.TabIndex = 5;
            this.tbDarkmodeCssFile.TextChanged += new System.EventHandler(this.tbDarkmodeCssFile_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "Darkmode CSS File:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 397);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 39);
            this.label5.TabIndex = 21;
            this.label5.Text = "Supported File Extensions:";
            // 
            // tbFileExt
            // 
            this.tbFileExt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFileExt.Location = new System.Drawing.Point(170, 397);
            this.tbFileExt.Name = "tbFileExt";
            this.tbFileExt.Size = new System.Drawing.Size(386, 25);
            this.tbFileExt.TabIndex = 15;
            this.tbFileExt.TextChanged += new System.EventHandler(this.tbFileExt_TextChanged);
            // 
            // btnDefaultFileExt
            // 
            this.btnDefaultFileExt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultFileExt.Location = new System.Drawing.Point(607, 397);
            this.btnDefaultFileExt.Name = "btnDefaultFileExt";
            this.btnDefaultFileExt.Size = new System.Drawing.Size(73, 28);
            this.btnDefaultFileExt.TabIndex = 16;
            this.btnDefaultFileExt.Text = "Default";
            this.btnDefaultFileExt.UseVisualStyleBackColor = true;
            this.btnDefaultFileExt.Click += new System.EventHandler(this.btnDefaultFileExt_Click);
            // 
            // cbAutoShowPanel
            // 
            this.cbAutoShowPanel.AutoSize = true;
            this.cbAutoShowPanel.Location = new System.Drawing.Point(170, 439);
            this.cbAutoShowPanel.Name = "cbAutoShowPanel";
            this.cbAutoShowPanel.Size = new System.Drawing.Size(290, 21);
            this.cbAutoShowPanel.TabIndex = 17;
            this.cbAutoShowPanel.Text = "Automatically show panel for supported files";
            this.cbAutoShowPanel.UseVisualStyleBackColor = true;
            this.cbAutoShowPanel.CheckedChanged += new System.EventHandler(this.cbAutoShowPanel_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 17);
            this.label6.TabIndex = 26;
            this.label6.Text = "HTML Rendering Engine:";
            // 
            // comboRenderingEngine
            // 
            this.comboRenderingEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboRenderingEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRenderingEngine.FormattingEnabled = true;
            this.comboRenderingEngine.Items.AddRange(new object[] {
            "Internet Explorer 11 (WebView 1)",
            "Edge (WebView 2)"});
            this.comboRenderingEngine.Location = new System.Drawing.Point(170, 80);
            this.comboRenderingEngine.Name = "comboRenderingEngine";
            this.comboRenderingEngine.Size = new System.Drawing.Size(511, 25);
            this.comboRenderingEngine.TabIndex = 1;
            this.comboRenderingEngine.SelectedIndexChanged += new System.EventHandler(this.comboRenderingEngine_SelectedIndexChanged);
            // 
            // cbShowToolbar
            // 
            this.cbShowToolbar.AutoSize = true;
            this.cbShowToolbar.Location = new System.Drawing.Point(170, 466);
            this.cbShowToolbar.Name = "cbShowToolbar";
            this.cbShowToolbar.Size = new System.Drawing.Size(223, 21);
            this.cbShowToolbar.TabIndex = 18;
            this.cbShowToolbar.Text = "Show Toolbar in Preview Window";
            this.cbShowToolbar.UseVisualStyleBackColor = true;
            this.cbShowToolbar.CheckedChanged += new System.EventHandler(this.cbShowToolbar_Changed);
            // 
            // cbShowStatusbar
            // 
            this.cbShowStatusbar.AutoSize = true;
            this.cbShowStatusbar.Location = new System.Drawing.Point(170, 495);
            this.cbShowStatusbar.Name = "cbShowStatusbar";
            this.cbShowStatusbar.Size = new System.Drawing.Size(321, 21);
            this.cbShowStatusbar.TabIndex = 19;
            this.cbShowStatusbar.Text = "Show Statusbar in Preview Window (Preview Links)";
            this.cbShowStatusbar.UseVisualStyleBackColor = true;
            this.cbShowStatusbar.CheckedChanged += new System.EventHandler(this.cbShowStatusbar_CheckedChanged);
            // 
            // cbAllowAllExtensions
            // 
            this.cbAllowAllExtensions.AutoSize = true;
            this.cbAllowAllExtensions.Location = new System.Drawing.Point(170, 357);
            this.cbAllowAllExtensions.Name = "cbAllowAllExtensions";
            this.cbAllowAllExtensions.Size = new System.Drawing.Size(164, 21);
            this.cbAllowAllExtensions.TabIndex = 14;
            this.cbAllowAllExtensions.Text = "Allow all file extensions";
            this.cbAllowAllExtensions.UseVisualStyleBackColor = true;
            this.cbAllowAllExtensions.CheckedChanged += new System.EventHandler(this.cbAllowAllExtensions_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(696, 618);
            this.Controls.Add(this.cbAllowAllExtensions);
            this.Controls.Add(this.comboRenderingEngine);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbShowStatusbar);
            this.Controls.Add(this.cbAutoShowPanel);
            this.Controls.Add(this.btnDefaultFileExt);
            this.Controls.Add(this.tbFileExt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnDefaultDarkmodeCss);
            this.Controls.Add(this.btnChooseDarkmodeCss);
            this.Controls.Add(this.tbDarkmodeCssFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cbShowToolbar);
            this.Controls.Add(this.btnResetHtml);
            this.Controls.Add(this.btnChooseHtml);
            this.Controls.Add(this.tbHtmlFile);
            this.Controls.Add(this.lblHtmlFile);
            this.Controls.Add(this.btnDefaultCss);
            this.Controls.Add(this.btnChooseCss);
            this.Controls.Add(this.lblZoomValue);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbCssFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbCssFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lblZoomValue;
        private System.Windows.Forms.Button btnChooseCss;
        private System.Windows.Forms.Button btnDefaultCss;
        private System.Windows.Forms.Button btnResetHtml;
        private System.Windows.Forms.Button btnChooseHtml;
        private System.Windows.Forms.TextBox tbHtmlFile;
        private System.Windows.Forms.Label lblHtmlFile;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel sblInvalidHtmlPath;
        private System.Windows.Forms.Button btnDefaultDarkmodeCss;
        private System.Windows.Forms.Button btnChooseDarkmodeCss;
        private System.Windows.Forms.TextBox tbDarkmodeCssFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbFileExt;
        private System.Windows.Forms.Button btnDefaultFileExt;
        private System.Windows.Forms.CheckBox cbAutoShowPanel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboRenderingEngine;
        private System.Windows.Forms.CheckBox cbShowToolbar;
        private System.Windows.Forms.CheckBox cbShowStatusbar;
        private System.Windows.Forms.CheckBox cbAllowAllExtensions;
    }
}