using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFToolKits;
using JFInterfaceDef;
using JFUI;
using System.Diagnostics;
using System.Xml.Serialization;
using JFHub;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace JFZoon
{


    public partial class Form1 : Form
    {
        public delegate void ClearupBeforeClose();
        public event ClearupBeforeClose Clearup;
        public Form1()
        {
            InitializeComponent();
        }
        
        //MyStruct ms;

        void OnSysCfgChange(string itemName,object newValue)
        {
            MessageBox.Show(itemName + " 's Value Changed!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JFHubCenter.Instance.SystemCfg.ItemChangedEvent += OnSysCfgChange;
            ucJFParamEdit.SetParamType(typeof(JFLinearCalibData), "线性标定数据结构测试");
        }

        private void btUnknowTest_Click(object sender, EventArgs e)
        {
            //JFinitializerHelper ih = JFHubCenter.Instance.InitorHelp;
            FormCmprTrigChnCfg fm = new FormCmprTrigChnCfg();
            fm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定要退出程序吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                if (null != Clearup)
                    Clearup();
            }
            else
                e.Cancel = true;
        }

        private void btSysCfg_Click(object sender, EventArgs e)
        {
            FormSysCfg fm = new FormSysCfg();
            fm.ShowDialog();
        }

        private void btDllMgr_Click(object sender, EventArgs e)
        {
            FormDllMgr fm = new FormDllMgr();
            fm.ShowDialog();
        }

        /// <summary>
        /// 动作流程调试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMethodFlow_Click(object sender, EventArgs e)
        {
            FormMethodFlowTest fm = new FormMethodFlowTest();
            fm.ShowDialog();
        }

        private void btCmdWorkTest_Click(object sender, EventArgs e)
        {
            Form4CmdWorkTest fm = new Form4CmdWorkTest();
            fm.ShowDialog();
        }

        /// <summary>
        /// 控制卡命名管理功能测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCellNameMgrTest_Click(object sender, EventArgs e)
        {
            //JFMotionDaqCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            //string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_MotionDaq));
            //string devID = devIDs?[0];
            //if (null == devID)
            //    return;
            //if (!mgr.AllDevices.Contains(devID))
            //    mgr.AddDevice(devID);
            //int DioModuleCount = mgr.GetDioModuleCount(devID);
            //mgr.SetDioModuleCount(devID,2);
            //mgr.Save(); //初步测试OK
            FormDeviceCellNameManager fm = new FormDeviceCellNameManager();
            fm.ShowDialog();

        }

        /// <summary>
        /// 工站点位存取测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLoadSaveAxisPos_Click(object sender, EventArgs e)
        {
            JFXCfg cfg = new JFXCfg();
            cfg.Load("StationDemoPosTable.xml", true);
            List<JFMultiAxisPosition> maPoses = new List<JFMultiAxisPosition>();
            JFMultiAxisPosition maPos1 = new JFMultiAxisPosition();
            maPos1.Name = "Pos1";
            maPos1.Positions.Add(JFAxisPos.Create("X", 1.23));
            maPos1.Positions.Add(JFAxisPos.Create("Y", 4.56));
            maPoses.Add(maPos1);
            JFMultiAxisPosition maPos2 = new JFMultiAxisPosition();
            maPos2.Name = "Pos2";
            maPos2.Positions.Add(JFAxisPos.Create("Y", 9.87));
            maPos2.Positions.Add(JFAxisPos.Create("Z", 6.54));
            maPoses.Add(maPos2);
            if (!cfg.ContainsItem("Positions"))
                cfg.AddItem("Positions", maPoses);
            else
                cfg.SetItemValue("Positions", maPoses);
            cfg.Save();

            JFXCfg cfgAnother = new JFXCfg();
            cfgAnother.Load("StationDemoPosTable.xml", false);
            List<JFMultiAxisPosition> maPoses1 = cfgAnother.GetItemValue("Positions") as List<JFMultiAxisPosition>;//测试OK，可获取

        }

        private void button1_Click(object sender, EventArgs e)
        {
            JFMethodFlow fl = new JFMethodFlow();
            fl.Load("D:\\Project\\JoinFrame\\bin\\x64\\Debug\\123.jff");
            if (string.IsNullOrEmpty(fl.Name))
            {
                fl.Name = "Hehe";
                fl.Save("D:\\Project\\JoinFrame\\bin\\x64\\Debug\\123.jff");
            }
            string txt = fl.ToTxt();
            JFMethodFlow anotherJF = new JFMethodFlow();
            anotherJF.FromTxt(txt);
            anotherJF.Save("D:\\Project\\JoinFrame\\bin\\x64\\Debug\\456.jff");
        }


        /// <summary>
        /// 工站测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string[] stationNames = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFStation));
            if(null == stationNames || 0 == stationNames.Length)
            {
                MessageBox.Show("系统中没有工站");
                return;
            }
            IJFStation station = JFHubCenter.Instance.InitorManager.GetInitor(stationNames[0]) as IJFStation;
            Form stationForm = station.GenForm();
            if (null != stationForm)
                stationForm.Show();
        }

        private void btSortedListTest_Click(object sender, EventArgs e)
        {
            SortedList<string, string> sl = new SortedList<string, string>();//经测试，和Dictionary几乎一样，不能实现按索引访问
            sl.Add("b", "bbb");
            sl.Add("a", "aaa");
            sl.Add("c", "ccc");
            foreach (string key in sl.Keys)
            {
                string hehe = key;
            }
            //for (int i = 0; i < sl.Count; i++)
            //{
            //    string haha = sl.get;
            //}
        }

        private void btUnknownDebug_Click(object sender, EventArgs e)
        {
            FormStationBaseCmrPanel fm = new FormStationBaseCmrPanel();
            fm.ShowDialog();
        }

        private void btEdit_Click(object sender, EventArgs e)
        {
            ucJFParamEdit.IsValueReadOnly = false;
        }

        private void btEndEdit_Click(object sender, EventArgs e)
        {
            ucJFParamEdit.IsValueReadOnly = true;
        }

        private void btSetDataPool_Click(object sender, EventArgs e)
        {
            IJFDataPool dp = JFHubCenter.Instance.DataPool;
            dp.RegistItem("F-ListInt", typeof(List<int>));
            dp.RegistItem("List<string>", typeof(List<string>));
            dp.RegistItem("ArrayString", typeof(string[]));

            dp.RegistList("Collect-string", typeof(string));
            dp.EnqueList("C-Collect-bool", true);
            dp.EnqueList("C-Collect-bool", false);
            dp.RegistList("D-Collect-string", typeof(string));


            ucDataPoolEdit1.SetDataPool(JFHubCenter.Instance.DataPool);
        }


        /// <summary>
        /// 编辑系统配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditSysCfg_Click(object sender, EventArgs e)
        {
            JFXCfg sysCfg = JFHubCenter.Instance.SystemCfg;
            FormXCfgEdit fm = new FormXCfgEdit();
            fm.AllowAddTypes.Add(typeof(Point));
            fm.AllowAddTypes.Add(typeof(JFMotionParam));
            fm.AllowAddTypes.Add(typeof(JFHomeParam));
            fm.AllowAddTypes.Add(typeof(MyStruct));
            fm.AllowAddTypes.Add(typeof(JFLinearCalibData));
            fm.SetCfg(sysCfg);
            fm.ShowDialog();

            //JFXCfg _cfg = new JFXCfg();
            //double[][] demo = new double[2][] { new double[]{ 0,0.1}, new double[]{1.2,3.4 } };
            //_cfg.AddItem("二维数组测试", demo,"定制参数");
            //_cfg.Save("111.xml");
            //fm = new FormXCfgEdit();
            //fm.SetCfg(_cfg);
            //fm.ShowDialog();

        }

        private void btSingleVisionCfgEdit_Click(object sender, EventArgs e)
        {
            JFXCfg cfg = new JFXCfg(); //序列化存储已测试
            List<JFSingleVisionCfgParam> _lst = new List<JFSingleVisionCfgParam>();
            JFSingleVisionCfgParam cp1 = new JFSingleVisionCfgParam();
            //cp1.Name = "hehe";
            cp1.LightChnNames = new string[] { "Light1", "Light2" };
            cp1.LightIntensities = new int[] { 11, 22 };
            //cp1.TestMethodFlowTxt = "测试方法流Demo";
            JFSingleVisionCfgParam cp2 = new JFSingleVisionCfgParam();
            //cp2.Name = "haha";
            cp2.LightChnNames = new string[] { "Light3", "Light4" };
            cp2.LightIntensities = new int[] { 33, 44 };

            _lst.Add(cp1);
            _lst.Add(cp2);
            cfg.AddItem("SVCfgs", _lst, "单视野参数配置");
            cfg.Save("视觉配置Demo");


            JFXCfg cfg1 = new JFXCfg();
            cfg1.Load("视觉配置Demo", false);
            List<JFSingleVisionCfgParam> _lst1 = cfg1.GetItemValue("SVCfgs") as List<JFSingleVisionCfgParam>;





        }

        private void button3_Click(object sender, EventArgs e)
        {

            bool isArray = typeof(List<int>).IsArray; //不是
            isArray = typeof(object[]).IsArray; //是

            Type bt = typeof(List<int>).GetElementType();//返回null
            bool isList = typeof(IList).IsAssignableFrom(typeof(List<int>)); //是
            bt = typeof(object[]).GetElementType(); //可以获取

            Type t = typeof(string[]);

            SerializableAttribute[] sas = t.GetCustomAttributes(typeof(SerializableAttribute),false) as SerializableAttribute[];


            JFXCfg cfg = new JFXCfg();
            List<object> lst = new List<object>();
            lst.Add(0);
            lst.Add(new string[] { "hehe", "haha" });
            cfg.AddItem("测试复合类型", lst);
            cfg.Save("测试CfgDemo.xml");
        }
    }
}
