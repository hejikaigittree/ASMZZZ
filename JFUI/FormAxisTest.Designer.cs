namespace JFUI
{
    partial class FormAxisTest
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
            this.components = new System.ComponentModel.Container();
            this.ucAxisStatus1 = new JFUI.UcAxisStatus();
            this.ucAxisTest1 = new JFUI.UcAxisTest();
            this.timerFlushStatus = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ucAxisStatus1
            // 
            this.ucAxisStatus1.DisplayMode = JFUI.UcAxisStatus.JFDisplayMode.full;
            this.ucAxisStatus1.Location = new System.Drawing.Point(0, 0);
            this.ucAxisStatus1.Name = "ucAxisStatus1";
            this.ucAxisStatus1.Size = new System.Drawing.Size(480, 51);
            this.ucAxisStatus1.TabIndex = 0;
            // 
            // ucAxisTest1
            // 
            this.ucAxisTest1.DisplayMode = JFUI.UcAxisTest.JFDisplayMode.full;
            this.ucAxisTest1.IsBoxShowError = false;
            this.ucAxisTest1.IsRepeating = false;
            this.ucAxisTest1.Location = new System.Drawing.Point(0, 51);
            this.ucAxisTest1.Name = "ucAxisTest1";
            this.ucAxisTest1.Size = new System.Drawing.Size(480, 360);
            this.ucAxisTest1.TabIndex = 1;
            // 
            // timerFlushStatus
            // 
            this.timerFlushStatus.Tick += new System.EventHandler(this.timerFlushStatus_Tick);
            // 
            // FormAxisTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 391);
            this.Controls.Add(this.ucAxisTest1);
            this.Controls.Add(this.ucAxisStatus1);
            this.Name = "FormAxisTest";
            this.Text = "轴测试";
            this.Load += new System.EventHandler(this.FormModuleTest_Axis_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UcAxisStatus ucAxisStatus1;
        private UcAxisTest ucAxisTest1;
        private System.Windows.Forms.Timer timerFlushStatus;
    }
}