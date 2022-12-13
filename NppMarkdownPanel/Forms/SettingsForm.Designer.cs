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
            this.cbShowToolbar = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.sblInvalidHtmlPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnDefaultDarkmodeCss = new System.Windows.Forms.Button();
            this.btnChooseDarkmodeCss = new System.Windows.Forms.Button();
            this.tbDarkmodeCssFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbAutoShowPanel = new System.Windows.Forms.CheckBox();
            this.MkdnExt = new System.Windows.Forms.Label();
            this.HtmlExt = new System.Windows.Forms.Label();
            this.btnDefaultMkdnExts = new System.Windows.Forms.Button();
            this.btnDefaultHtmlExts = new System.Windows.Forms.Button();
            this.tbMkdnExts = new System.Windows.Forms.TextBox();
            this.tbHtmlExts = new System.Windows.Forms.TextBox();
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
            this.panel1.Size = new System.Drawing.Size(697, 61);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Markdown Panel Settings";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::NppMarkdownPanel.Properties.Resources.markdown_16x16_solid;
            this.pictureBox1.Location = new System.Drawing.Point(10, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 20);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(472, 462);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 36);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(583, 462);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 36);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "CSS File:";
            // 
            // tbCssFile
            // 
            this.tbCssFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCssFile.Location = new System.Drawing.Point(159, 97);
            this.tbCssFile.Name = "tbCssFile";
            this.tbCssFile.Size = new System.Drawing.Size(401, 21);
            this.tbCssFile.TabIndex = 6;
            this.tbCssFile.TextChanged += new System.EventHandler(this.tbCssFile_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 195);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Zoom Level:";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(153, 195);
            this.trackBar1.Maximum = 800;
            this.trackBar1.Minimum = 80;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(489, 45);
            this.trackBar1.TabIndex = 14;
            this.trackBar1.TickFrequency = 25;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Value = 90;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // lblZoomValue
            // 
            this.lblZoomValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZoomValue.AutoSize = true;
            this.lblZoomValue.Location = new System.Drawing.Point(648, 210);
            this.lblZoomValue.Name = "lblZoomValue";
            this.lblZoomValue.Size = new System.Drawing.Size(28, 13);
            this.lblZoomValue.TabIndex = 15;
            this.lblZoomValue.Text = "90%";
            // 
            // btnChooseCss
            // 
            this.btnChooseCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseCss.Location = new System.Drawing.Point(566, 95);
            this.btnChooseCss.Name = "btnChooseCss";
            this.btnChooseCss.Size = new System.Drawing.Size(39, 25);
            this.btnChooseCss.TabIndex = 7;
            this.btnChooseCss.Text = "...";
            this.btnChooseCss.UseVisualStyleBackColor = true;
            this.btnChooseCss.Click += new System.EventHandler(this.btnChooseCss_Click);
            // 
            // btnDefaultCss
            // 
            this.btnDefaultCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultCss.Location = new System.Drawing.Point(611, 95);
            this.btnDefaultCss.Name = "btnDefaultCss";
            this.btnDefaultCss.Size = new System.Drawing.Size(73, 26);
            this.btnDefaultCss.TabIndex = 8;
            this.btnDefaultCss.Text = "Default";
            this.btnDefaultCss.UseVisualStyleBackColor = true;
            this.btnDefaultCss.Click += new System.EventHandler(this.btnDefaultCss_Click);
            // 
            // btnResetHtml
            // 
            this.btnResetHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetHtml.Location = new System.Drawing.Point(611, 272);
            this.btnResetHtml.Name = "btnResetHtml";
            this.btnResetHtml.Size = new System.Drawing.Size(73, 28);
            this.btnResetHtml.TabIndex = 19;
            this.btnResetHtml.Text = "Default";
            this.btnResetHtml.UseVisualStyleBackColor = true;
            this.btnResetHtml.Click += new System.EventHandler(this.btnResetHtml_Click);
            // 
            // btnChooseHtml
            // 
            this.btnChooseHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseHtml.Location = new System.Drawing.Point(566, 272);
            this.btnChooseHtml.Name = "btnChooseHtml";
            this.btnChooseHtml.Size = new System.Drawing.Size(39, 27);
            this.btnChooseHtml.TabIndex = 18;
            this.btnChooseHtml.Text = "...";
            this.btnChooseHtml.UseVisualStyleBackColor = true;
            this.btnChooseHtml.Click += new System.EventHandler(this.btnChooseHtml_Click);
            // 
            // tbHtmlFile
            // 
            this.tbHtmlFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHtmlFile.Location = new System.Drawing.Point(159, 274);
            this.tbHtmlFile.Name = "tbHtmlFile";
            this.tbHtmlFile.Size = new System.Drawing.Size(401, 21);
            this.tbHtmlFile.TabIndex = 17;
            this.tbHtmlFile.TextChanged += new System.EventHandler(this.tbHtmlFile_TextChanged);
            this.tbHtmlFile.Leave += new System.EventHandler(this.tbHtmlFile_Leave);
            // 
            // lblHtmlFile
            // 
            this.lblHtmlFile.AutoSize = true;
            this.lblHtmlFile.Location = new System.Drawing.Point(12, 264);
            this.lblHtmlFile.Name = "lblHtmlFile";
            this.lblHtmlFile.Size = new System.Drawing.Size(108, 39);
            this.lblHtmlFile.TabIndex = 16;
            this.lblHtmlFile.Text = "Automatically Save \r\nHTML from Current \r\nPreview to this File:";
            // 
            // cbShowToolbar
            // 
            this.cbShowToolbar.AutoSize = true;
            this.cbShowToolbar.Location = new System.Drawing.Point(159, 413);
            this.cbShowToolbar.Name = "cbShowToolbar";
            this.cbShowToolbar.Size = new System.Drawing.Size(199, 17);
            this.cbShowToolbar.TabIndex = 27;
            this.cbShowToolbar.Text = "Show Toolbar in Preview Window";
            this.cbShowToolbar.UseVisualStyleBackColor = true;
            this.cbShowToolbar.CheckedChanged += new System.EventHandler(this.cbShowToolbar_Changed);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sblInvalidHtmlPath});
            this.statusStrip1.Location = new System.Drawing.Point(0, 510);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(700, 22);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // sblInvalidHtmlPath
            // 
            this.sblInvalidHtmlPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sblInvalidHtmlPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.sblInvalidHtmlPath.ForeColor = System.Drawing.Color.Red;
            this.sblInvalidHtmlPath.Name = "sblInvalidHtmlPath";
            this.sblInvalidHtmlPath.Size = new System.Drawing.Size(685, 17);
            this.sblInvalidHtmlPath.Spring = true;
            // 
            // btnDefaultDarkmodeCss
            // 
            this.btnDefaultDarkmodeCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultDarkmodeCss.Location = new System.Drawing.Point(611, 143);
            this.btnDefaultDarkmodeCss.Name = "btnDefaultDarkmodeCss";
            this.btnDefaultDarkmodeCss.Size = new System.Drawing.Size(73, 26);
            this.btnDefaultDarkmodeCss.TabIndex = 12;
            this.btnDefaultDarkmodeCss.Text = "Default";
            this.btnDefaultDarkmodeCss.UseVisualStyleBackColor = true;
            this.btnDefaultDarkmodeCss.Click += new System.EventHandler(this.btnDefaultDarkmodeCss_Click);
            // 
            // btnChooseDarkmodeCss
            // 
            this.btnChooseDarkmodeCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseDarkmodeCss.Location = new System.Drawing.Point(566, 143);
            this.btnChooseDarkmodeCss.Name = "btnChooseDarkmodeCss";
            this.btnChooseDarkmodeCss.Size = new System.Drawing.Size(39, 25);
            this.btnChooseDarkmodeCss.TabIndex = 11;
            this.btnChooseDarkmodeCss.Text = "...";
            this.btnChooseDarkmodeCss.UseVisualStyleBackColor = true;
            this.btnChooseDarkmodeCss.Click += new System.EventHandler(this.btnChooseCss_Click);
            // 
            // tbDarkmodeCssFile
            // 
            this.tbDarkmodeCssFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDarkmodeCssFile.Location = new System.Drawing.Point(159, 145);
            this.tbDarkmodeCssFile.Name = "tbDarkmodeCssFile";
            this.tbDarkmodeCssFile.Size = new System.Drawing.Size(401, 21);
            this.tbDarkmodeCssFile.TabIndex = 10;
            this.tbDarkmodeCssFile.TextChanged += new System.EventHandler(this.tbDarkmodeCssFile_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Darkmode CSS File:";
            // 
            // cbAutoShowPanel
            // 
            this.cbAutoShowPanel.AutoSize = true;
            this.cbAutoShowPanel.Location = new System.Drawing.Point(159, 384);
            this.cbAutoShowPanel.Name = "cbAutoShowPanel";
            this.cbAutoShowPanel.Size = new System.Drawing.Size(257, 17);
            this.cbAutoShowPanel.TabIndex = 26;
            this.cbAutoShowPanel.Text = "Automatically show panel for supported files";
            this.cbAutoShowPanel.UseVisualStyleBackColor = true;
            this.cbAutoShowPanel.CheckedChanged += new System.EventHandler(this.cbAutoShowPanel_CheckedChanged);
            // 
            // MkdnExt
            // 
            this.MkdnExt.AutoSize = true;
            this.MkdnExt.Location = new System.Drawing.Point(12, 316);
            this.MkdnExt.Name = "MkdnExt";
            this.MkdnExt.Size = new System.Drawing.Size(124, 13);
            this.MkdnExt.TabIndex = 20;
            this.MkdnExt.Text = "Markdown Extensions:";
            // 
            // HtmlExt
            // 
            this.HtmlExt.AutoSize = true;
            this.HtmlExt.Location = new System.Drawing.Point(12, 346);
            this.HtmlExt.Name = "HtmlExt";
            this.HtmlExt.Size = new System.Drawing.Size(97, 13);
            this.HtmlExt.TabIndex = 23;
            this.HtmlExt.Text = "HTML Extensions:";
            // 
            // btnDefaultMkdnExts
            // 
            this.btnDefaultMkdnExts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultMkdnExts.Location = new System.Drawing.Point(611, 310);
            this.btnDefaultMkdnExts.Name = "btnDefaultMkdnExts";
            this.btnDefaultMkdnExts.Size = new System.Drawing.Size(73, 26);
            this.btnDefaultMkdnExts.TabIndex = 22;
            this.btnDefaultMkdnExts.Text = "Default";
            this.btnDefaultMkdnExts.UseVisualStyleBackColor = true;
            this.btnDefaultMkdnExts.Click += new System.EventHandler(this.btnDefaultMkdnExts_Click);
            // 
            // btnDefaultHtmlExts
            // 
            this.btnDefaultHtmlExts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefaultHtmlExts.Location = new System.Drawing.Point(611, 339);
            this.btnDefaultHtmlExts.Name = "btnDefaultHtmlExts";
            this.btnDefaultHtmlExts.Size = new System.Drawing.Size(73, 26);
            this.btnDefaultHtmlExts.TabIndex = 25;
            this.btnDefaultHtmlExts.Text = "Default";
            this.btnDefaultHtmlExts.UseVisualStyleBackColor = true;
            this.btnDefaultHtmlExts.Click += new System.EventHandler(this.btnDefaultHtmlExts_Click);
            // 
            // tbMkdnExts
            // 
            this.tbMkdnExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMkdnExts.Location = new System.Drawing.Point(159, 312);
            this.tbMkdnExts.Name = "tbMkdnExts";
            this.tbMkdnExts.Size = new System.Drawing.Size(401, 21);
            this.tbMkdnExts.TabIndex = 21;
            this.tbMkdnExts.TextChanged += new System.EventHandler(this.tbMkdnExts_TextChanged);
            // 
            // tbHtmlExts
            // 
            this.tbHtmlExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHtmlExts.Location = new System.Drawing.Point(159, 341);
            this.tbHtmlExts.Name = "tbHtmlExts";
            this.tbHtmlExts.Size = new System.Drawing.Size(401, 21);
            this.tbHtmlExts.TabIndex = 24;
            this.tbHtmlExts.TextChanged += new System.EventHandler(this.tbHtmlExts_TextChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 532);
            this.Controls.Add(this.cbAutoShowPanel);
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
            this.Controls.Add(this.MkdnExt);
            this.Controls.Add(this.tbMkdnExts);
            this.Controls.Add(this.btnDefaultMkdnExts);
            this.Controls.Add(this.HtmlExt);
            this.Controls.Add(this.tbHtmlExts);
            this.Controls.Add(this.btnDefaultHtmlExts);
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
        private System.Windows.Forms.CheckBox cbShowToolbar;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel sblInvalidHtmlPath;
        private System.Windows.Forms.Button btnDefaultDarkmodeCss;
        private System.Windows.Forms.Button btnChooseDarkmodeCss;
        private System.Windows.Forms.TextBox tbDarkmodeCssFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label MkdnExt;
        private System.Windows.Forms.TextBox tbMkdnExts;
        private System.Windows.Forms.Button btnDefaultMkdnExts;
        private System.Windows.Forms.Label HtmlExt;
        private System.Windows.Forms.TextBox tbHtmlExts;
        private System.Windows.Forms.Button btnDefaultHtmlExts;
        private System.Windows.Forms.CheckBox cbAutoShowPanel;
    }
}