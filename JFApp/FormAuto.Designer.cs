﻿namespace JFApp
{
    partial class FormAuto
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
            this.pnStations = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnStations
            // 
            this.pnStations.AutoScroll = true;
            this.pnStations.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnStations.ColumnCount = 1;
            this.pnStations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnStations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnStations.Location = new System.Drawing.Point(0, 0);
            this.pnStations.MaximumSize = new System.Drawing.Size(374, 0);
            this.pnStations.Name = "pnStations";
            this.pnStations.RowCount = 1;
            this.pnStations.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnStations.Size = new System.Drawing.Size(0, 446);
            this.pnStations.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnStations);
            this.splitContainer1.Panel1MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 4;
            this.splitContainer1.TabIndex = 0;
            // 
            // FormAuto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAuto";
            this.Text = "FormAuto";
            this.Load += new System.EventHandler(this.FormAuto_Load);
            this.VisibleChanged += new System.EventHandler(this.FormAuto_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnStations;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}