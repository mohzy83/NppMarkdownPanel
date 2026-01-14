namespace NppMarkdownPanel.Forms
{
    partial class MarkdownPreviewForm
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
            this.panelPreview = new System.Windows.Forms.Panel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pictureBoxScreenshot = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbPreview = new System.Windows.Forms.ToolStrip();
            this.btnSaveHtml = new System.Windows.Forms.ToolStripSplitButton();
            this.btnSaveWithLightTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCopyToClipboard = new System.Windows.Forms.ToolStripButton();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelPreview.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).BeginInit();
            this.tbPreview.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPreview
            // 
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPreview.Controls.Add(this.toolStripContainer1);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Location = new System.Drawing.Point(0, 0);
            this.panelPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(813, 602);
            this.panelPreview.TabIndex = 0;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pictureBoxScreenshot);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(811, 573);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(811, 600);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tbPreview);
            // 
            // pictureBoxScreenshot
            // 
            this.pictureBoxScreenshot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxScreenshot.Image = global::NppMarkdownPanel.Properties.Resources.markdown_16x16_solid_bmp;
            this.pictureBoxScreenshot.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScreenshot.Name = "pictureBoxScreenshot";
            this.pictureBoxScreenshot.Size = new System.Drawing.Size(811, 573);
            this.pictureBoxScreenshot.TabIndex = 1;
            this.pictureBoxScreenshot.TabStop = false;
            this.pictureBoxScreenshot.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 573);
            this.panel1.TabIndex = 2;
            // 
            // tbPreview
            // 
            this.tbPreview.Dock = System.Windows.Forms.DockStyle.None;
            this.tbPreview.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tbPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveHtml,
            this.btnCopyToClipboard});
            this.tbPreview.Location = new System.Drawing.Point(0, 0);
            this.tbPreview.Name = "tbPreview";
            this.tbPreview.Size = new System.Drawing.Size(811, 27);
            this.tbPreview.Stretch = true;
            this.tbPreview.TabIndex = 0;
            // 
            // btnSaveHtml
            // 
            this.btnSaveHtml.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveWithLightTheme});
            this.btnSaveHtml.Image = global::NppMarkdownPanel.Properties.Resources.Save_16x_32;
            this.btnSaveHtml.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnSaveHtml.Name = "btnSaveHtml";
            this.btnSaveHtml.Size = new System.Drawing.Size(108, 24);
            this.btnSaveHtml.Text = "Save As...";
            this.btnSaveHtml.ButtonClick += new System.EventHandler(this.btnSaveHtml_Click);
            // 
            // btnSaveWithLightTheme
            // 
            this.btnSaveWithLightTheme.Image = global::NppMarkdownPanel.Properties.Resources.Save_16x_32;
            this.btnSaveWithLightTheme.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnSaveWithLightTheme.Name = "btnSaveWithLightTheme";
            this.btnSaveWithLightTheme.Size = new System.Drawing.Size(271, 26);
            this.btnSaveWithLightTheme.Text = "Save As (with Light Theme)";
            this.btnSaveWithLightTheme.Click += new System.EventHandler(this.btnSaveLightTheme_Click);
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Image = global::NppMarkdownPanel.Properties.Resources.Copy_16x_32;
            this.btnCopyToClipboard.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(157, 24);
            this.btnCopyToClipboard.Text = "Copy To Clipboard";
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // statusStrip2
            // 
            this.statusStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip2.Location = new System.Drawing.Point(0, 578);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(813, 24);
            this.statusStrip2.TabIndex = 2;
            this.statusStrip2.Text = "statusStrip2";
            this.statusStrip2.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 18);
            // 
            // MarkdownPreviewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(813, 602);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.statusStrip2);
            this.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MarkdownPreviewForm";
            this.Text = "MarkdownPreviewForm";
            this.panelPreview.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).EndInit();
            this.tbPreview.ResumeLayout(false);
            this.tbPreview.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.PictureBox pictureBoxScreenshot;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip tbPreview;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSplitButton btnSaveHtml;
        private System.Windows.Forms.ToolStripMenuItem btnSaveWithLightTheme;
        private System.Windows.Forms.ToolStripButton btnCopyToClipboard;
    }
}