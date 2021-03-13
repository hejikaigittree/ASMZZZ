using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace JFUI
{
    public partial class UcAIO : UserControl
    {
        public UcAIO()
        {
            InitializeComponent();
        }



        private void UcAIO_Load(object sender, EventArgs e)
        {

        }

        private void UcAIO_SizeChanged(object sender, EventArgs e)
        {
            ucAiPanel.Location = new Point(2, 2);
            ucAiPanel.Size = new Size((Width - 6) / 2, tbInfo.Top-2);
            ucAoPanel.Location = new Point(ucAiPanel.Right + 2, 2);
            ucAoPanel.Size = new Size((Width - 6) / 2, tbInfo.Top - 2);
        }

         delegate void dgShowTips(string txt);
        public void ShowTips(string txt)
        {
            if(InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { txt });
                return;
            }
            tbInfo.Text = txt;
        }

        delegate void dgSetModule(IJFModule_AIO module, string[] aiNames, string[] aoNames);
        public void SetModuleInfo(IJFModule_AIO module,string[] aiNames,string[] aoNames)
        {
            if (InvokeRequired)
            {
                Invoke(new dgSetModule(SetModuleInfo), new object[] { module, aiNames, aoNames });
                return;
            }
            ucAiPanel.RemoveAllAIO();
            ucAoPanel.RemoveAllAIO();
            if (null == module)
                return;
            for (int i = 0; i < module.AICount; i++)
                ucAiPanel.AddIO(module, i, aiNames == null ? null : (aiNames.Length > i ? aiNames[i] : null));
            for (int i = 0; i < module.AOCount; i++)
                ucAoPanel.AddIO(module, i, aoNames == null ? null : (aoNames.Length > i ? aoNames[i] : null));

        }

        public void UpdateSrc2UI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSrc2UI));
                return;
            }
            ucAiPanel.UpdateSrc2UI();
            ucAoPanel.UpdateSrc2UI();
            tbInfo.Text = "IO Auto Flashing " + DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
