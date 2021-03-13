using System.Drawing;

namespace JFUI
{
    partial class TabControlCF
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TabControlCF
            // 
            this.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.ItemSize = new System.Drawing.Size(35, 100);
            this.Multiline = true;
            this.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.ResumeLayout(false);

        }

        #endregion
    }
}
