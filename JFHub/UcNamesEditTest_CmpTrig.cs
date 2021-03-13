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
using JFUI;

namespace JFHub
{
    public partial class UcNamesEditTest_CmpTrig : UserControl
    {
        public UcNamesEditTest_CmpTrig()
        {
            InitializeComponent();
        }

        private void UcNamesEditTest_Trig_Load(object sender, EventArgs e)
        {

        }

        List<TextBox> lstTbTrigIDs = new List<TextBox>();

        public void UpdateChannelsInfo(string devID, int moduleIndex)
        {
            lstTbTrigIDs.Clear();
            pnTrigs.Controls.Clear();
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            IJFModule_CmprTrigger md = null;
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(devID) as IJFDevice_MotionDaq;
            if (dev != null && dev.CompareTriggerCount > moduleIndex)
                md = dev.GetCompareTrigger(moduleIndex);
            int trigChannCount = mgr.GetCmpTrigCount(devID,moduleIndex);
            for(int i = 0; i < trigChannCount;i++)
            {
                Label lb = new Label();
                lb.Location = new Point(2, 2+5+33 * i);
                lb.Text = "序号:" + i.ToString("D2") + "名称:";
                pnTrigs.Controls.Add(lb);
                TextBox tbID = new TextBox();
                tbID.Location = new Point(lb.Right, 2+33 * i);
                tbID.Text = mgr.GetCmpTrigName(devID, moduleIndex, i);
                tbID.BackColor = SystemColors.Control;
                tbID.ReadOnly = true;
                tbID.Width = 200;
                pnTrigs.Controls.Add(tbID);
                lstTbTrigIDs.Add(tbID);
                UcCmprTrgChn ucTrig = new UcCmprTrgChn();
                ucTrig.Location = new Point(tbID.Right, 2 + 33 * i);
                ucTrig.SetModuleChn(md, i, "", null);
                pnTrigs.Controls.Add(ucTrig);
            }
        }

        public void BeginEdit()
        {
            foreach(TextBox tb in lstTbTrigIDs)
            {
                tb.ReadOnly = false;
                tb.BackColor = Color.White;
            }
        }

        public void EndEdit()
        {
            foreach (TextBox tb in lstTbTrigIDs)
            {
                tb.ReadOnly = true;
                tb.BackColor = SystemColors.Control;
            }
        }

        public string[] TrigNames
        {
            get 
            {
                List<string> ret = new List<string>();
                foreach (TextBox tb in lstTbTrigIDs)
                    ret.Add(tb.Text);
                return ret.ToArray();
            }
        }

    }
}
