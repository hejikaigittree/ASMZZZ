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
    public partial class UcNamesEditTest_Motion : UserControl
    {
        public UcNamesEditTest_Motion()
        {
            InitializeComponent();
        }

        private void UcNamesEditTest_Motion_Load(object sender, EventArgs e)
        {

        }
        List<TextBox> lstTbAxisIDs = new List<TextBox>();

        public void UpdateChannelsInfo(string devID, int moduleIndex)
        {
            lstTbAxisIDs.Clear();
            pnAxes.Controls.Clear();
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            IJFModule_Motion md = null;
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(devID) as IJFDevice_MotionDaq;
            if (dev != null && dev.DioCount > moduleIndex)
                md = dev.GetMc(moduleIndex);
            int axisCount = mgr.GetAxisCount(devID, moduleIndex);
            for (int i = 0; i < axisCount; i++)
            {
                Label lbIndex = new Label();
                lbIndex.Text = "轴序号:" +i.ToString("D2") + " 轴ID:";
                lbIndex.Location = new Point(2, 5 + i * 70 + 2);
                pnAxes.Controls.Add(lbIndex);
                TextBox tbAxisID = new TextBox();
                tbAxisID.Location = new Point(lbIndex.Right, i * 70 + 2);
                string axisID = mgr.GetAxisName(devID, moduleIndex, i);
                tbAxisID.Text = axisID;
                
                tbAxisID.Enabled = false;
                tbAxisID.BackColor = SystemColors.Control;
                pnAxes.Controls.Add(tbAxisID);
                lstTbAxisIDs.Add(tbAxisID);
                UcAxisStatus ucas = new UcAxisStatus();

                tbAxisID.Width = ucas.Width - 5 - lbIndex.Width;
                pnAxes.Controls.Add(ucas);
                ucas.Location = new Point(2, lbIndex.Bottom-3);
                //ucas.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ucas.DisplayMode = UcAxisStatus.JFDisplayMode.simple;
                //ucDi.SetDioInfo(md, moduleIndex, false, mgr.GetDiName(devID, moduleIndex, i));
                ucas.SetAxis(md, i);
                UcAxisTest ucat = new UcAxisTest();
                ucat.DisplayMode = UcAxisTest.JFDisplayMode.simplest_vel;
                ucat.Location = new Point(ucas.Right, tbAxisID.Top);
                pnAxes.Controls.Add(ucat);
                ucat.SetAxis(md, i);
            }
            
            
        }

        public void BeginEdit()
        {
            foreach(TextBox tb in lstTbAxisIDs)
            {
                tb.BackColor = Color.White;
                tb.Enabled = true;
            }
        }


        public void EndEdit()
        {
            foreach (TextBox tb in lstTbAxisIDs)
            {
                tb.BackColor = SystemColors.Control;
                tb.Enabled = false;
            }
        }

        public string[] AxisNames
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (TextBox tb in lstTbAxisIDs)
                    ret.Add(tb.Text);
                return ret.ToArray();
            }
        }

        public void UpdateAxis2UI()
        {
            foreach(Control ctrl in pnAxes.Controls)
            {
                if (ctrl is UcAxisStatus)
                    (ctrl as UcAxisStatus).UpdateAxisStatus();
                else if (ctrl is UcAxisTest)
                    (ctrl as UcAxisTest).UpdateAxisUI();
            }
        }
    }
}
