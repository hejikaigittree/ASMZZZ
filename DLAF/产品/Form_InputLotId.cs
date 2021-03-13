using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLAF
{
    public partial class Form_InputLotId : Form
    {
        public static Form_InputLotId Instance = null;
        public Form_InputLotId()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Instance = this;
        }
        /// <summary>
        /// 显示本窗体的一个模态窗体实例
        /// </summary>
        public static void ShowDialogForm()
        {
            Form_InputLotId frm = Form_InputLotId.Instance;
            if (frm != null)
            {
                if (!frm.IsDisposed)
                {
                    Form_InputLotId.Instance.Dispose();
                }
            }
            Form_InputLotId.Instance = new Form_InputLotId();
            frm = Form_InputLotId.Instance;
            int SH = Screen.PrimaryScreen.Bounds.Height;
            int SW = Screen.PrimaryScreen.Bounds.Width;
            //frm.Location = new Point((SW - frm.Size.Width) / 2, (SH - frm.Size.Height) / 2);
            frm.StartPosition = FormStartPosition.CenterParent;
            //App.mainWin.Enabled = false;
            Form_InputLotId.Instance.textBox1.Focus();
            frm.ShowDialog();
            //App.mainWin.Enabled = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!HT_Lib.HTUi.PopYesNo("是否确认当前批次名为“" + textBox1.Text + "”？"))
                {
                    return;
                }
                //App.obj_Process.InitialNewLot(textBox1.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Time_Click(object sender, EventArgs e)
        {

            try
            {
                textBox1.Text=DateTime.Now.ToString("yyyyMMdd-HH-mm-ss");
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    btn_OK_Click(null, null);
                    break;
            }
        }
    }
}
