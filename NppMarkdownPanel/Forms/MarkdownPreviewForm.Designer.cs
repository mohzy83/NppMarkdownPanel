﻿using System;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkdownPreviewForm));
            this.panelPreview = new System.Windows.Forms.Panel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pictureBoxScreenshot = new System.Windows.Forms.PictureBox();
            this.webBrowserPreview = new System.Windows.Forms.WebBrowser();
            this.tbPreview = new System.Windows.Forms.ToolStrip();
            this.btnSaveHtml = new System.Windows.Forms.ToolStripButton();
            this.btnPrintPreview = new System.Windows.Forms.ToolStripButton();
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
            // this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pictureBoxScreenshot);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.webBrowserPreview);
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
            this.pictureBoxScreenshot.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScreenshot.Name = "pictureBoxScreenshot";
            this.pictureBoxScreenshot.Size = new System.Drawing.Size(811, 573);
            this.pictureBoxScreenshot.TabIndex = 1;
            this.pictureBoxScreenshot.TabStop = false;
            this.pictureBoxScreenshot.Visible = false;
            // 
            // webBrowserPreview
            // 
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
            this.webBrowserPreview.StatusTextChanged += new EventHandler(this.webBrowserPreview_StatusTextChanged);
            // 
            // tbPreview
            // 
            this.tbPreview.Dock = System.Windows.Forms.DockStyle.None;
            this.tbPreview.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tbPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveHtml,
            this.btnPrintPreview});
            this.tbPreview.Location = new System.Drawing.Point(0, 0);
            this.tbPreview.Name = "tbPreview";
            this.tbPreview.Size = new System.Drawing.Size(811, 27);
            this.tbPreview.Stretch = true;
            this.tbPreview.TabIndex = 0;
            // 
            // btnSaveHtml
            // 
            this.btnSaveHtml.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveHtml.Image")));
            this.btnSaveHtml.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveHtml.Name = "btnSaveHtml";
            this.btnSaveHtml.Size = new System.Drawing.Size(93, 24);
            this.btnSaveHtml.Text = "Save As...";
            this.btnSaveHtml.Click += new System.EventHandler(this.btnSaveHtml_Click);
            // 
            // btnPrintPreview
            // 
            this.btnPrintPreview.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintPreview.Image")));
            this.btnPrintPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrintPreview.Name = "btnPrintPreview";
            this.btnPrintPreview.Size = new System.Drawing.Size(118, 24);
            this.btnPrintPreview.Text = "Print Preview";
            this.btnPrintPreview.Visible = false;
            //
            // statusStrip2
            //
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip2.Location = new System.Drawing.Point(0, 553);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(811, 22);
            this.statusStrip2.TabIndex = 2;
            this.statusStrip2.Text = "statusStrip2";
            //
            // toolStripStatusLabel1
            //
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "";
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
            this.toolStripContainer1.ContentPanel.PerformLayout();
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
        private System.Windows.Forms.WebBrowser webBrowserPreview;
        private System.Windows.Forms.PictureBox pictureBoxScreenshot;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip tbPreview;
        private System.Windows.Forms.ToolStripButton btnSaveHtml;
        private System.Windows.Forms.ToolStripButton btnPrintPreview;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}