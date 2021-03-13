using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
using HalconDotNet;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace DLAF
{
    [JFDisplayName("相机标定工站")]
    public class CalibStation : JFStationBase
    {
        public FormUV2XY uv2xy;
        public CalibOperation operation;

        const bool use_current_dir = false;
        static string currentDir = Directory.GetCurrentDirectory();
        static string programDir = (use_current_dir ? Environment.CurrentDirectory : "D:\\ht'tech") + "\\AOI_LF";
        public string SystemUV2XYDir { get { return (programDir + "\\System\\UV2XY"); } }

        public List<HTuple> List_UV2XYResult = null;
        public string imageFolder = Application.StartupPath + "\\ImageFolder";
        public int _RunMode = 0;
        public int Num_Camera = 1;
        public int SelectedIndex = 0;
        public double ZFocus = 0;
        public double Z_safe = 0;

        #region System Params描述
        private string categoryName = "相机标定工站";
        private string mRunMode = "系统运行模式";
        private string mNum_Camera = "相机总数";
        private string mSelectIndex = "当前选定的相机";
        private string mZFocus = "Z轴聚焦高度";
        private string mZ_safe = "相机Z轴安全位";
        #endregion

        public string _calibrationUV2XYParameter = string.Empty;
        [BrowsableAttribute(false)]
        public string CalibrationUV2XYParameter
        {
            get
            {
                return _calibrationUV2XYParameter;
            }
            set
            {
                _calibrationUV2XYParameter = value;
            }
        }

        /// <summary>
        /// UV2XY模版存放路径
        /// </summary>
        public string _calibrUV2XYModelPath = string.Empty;
        [BrowsableAttribute(false)]
        public string CalibrUV2XYModelPath
        {
            get { return _calibrUV2XYModelPath; }
            set
            {
                _calibrUV2XYModelPath = value;
            }
        }


        #region 设备名称/通道描述
        public string[] AxisX = new string[] { "2D相机X轴" };
        public string[] AxisY = new string[] { "2D相机Y轴" };
        public string[] AxisZ = new string[] { "2D相机Z轴" };
        public string[] AxisXYZ = new string[] { "2D相机X轴", "2D相机Y轴", "2D相机Z轴" };
        public string[] CamereDev = new string[] { "海康相机", "海康相机" };
        #endregion 

        public CalibStation()
        {
            operation = new CalibOperation();
            operation.SetStation(this);
            DeclearStationCfgParam();

            _rtUi.SetStation(this);
        }

        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            return true;
        }

        private void DeclearStationCfgParam()
        {
            DeclearCfgParam(mRunMode, typeof(int), categoryName);
            DeclearCfgParam(mNum_Camera, typeof(int), categoryName);
            DeclearCfgParam(mSelectIndex, typeof(int), categoryName);
            DeclearCfgParam(mZFocus, typeof(double), categoryName);
            DeclearCfgParam(mZ_safe, typeof(double), categoryName);
        }

        public void InitStationParams()
        {
            _RunMode = (int)GetCfgParamValue(mRunMode);
            Num_Camera = (int)GetCfgParamValue(mNum_Camera);
            SelectedIndex = (int)GetCfgParamValue(mSelectIndex);
            ZFocus = (double)GetCfgParamValue(mZFocus);
            Z_safe = (double)GetCfgParamValue(mZ_safe);
            imageFolder = (string)JFHubCenter.Instance.RecipeManager.GetInitParamValue((string)JFHubCenter.Instance.RecipeManager.InitParamNames[2]);
        }

        private void InitSystemParams()
        {
            if (JFHubCenter.Instance.SystemCfg.ContainsItem(mRunMode))
                _RunMode = (int)JFHubCenter.Instance.SystemCfg.GetItemValue(mRunMode);
            else
                JFHubCenter.Instance.SystemCfg.AddItem(mRunMode, RunMode, categoryName);

            if (JFHubCenter.Instance.SystemCfg.ContainsItem(mNum_Camera))
                Num_Camera = (int)JFHubCenter.Instance.SystemCfg.GetItemValue(mNum_Camera);
            else
                JFHubCenter.Instance.SystemCfg.AddItem(mNum_Camera, Num_Camera, categoryName);

            if (JFHubCenter.Instance.SystemCfg.ContainsItem(mSelectIndex))
                SelectedIndex = (int)JFHubCenter.Instance.SystemCfg.GetItemValue(mSelectIndex);
            else
                JFHubCenter.Instance.SystemCfg.AddItem(mSelectIndex, SelectedIndex, categoryName);

            if (JFHubCenter.Instance.SystemCfg.ContainsItem(mZFocus))
                ZFocus = (double)JFHubCenter.Instance.SystemCfg.GetItemValue(mZFocus);
            else
                JFHubCenter.Instance.SystemCfg.AddItem(mZFocus, ZFocus, categoryName);

            if (JFHubCenter.Instance.SystemCfg.ContainsItem(mZ_safe))
                Z_safe = (double)JFHubCenter.Instance.SystemCfg.GetItemValue(mZ_safe);
            else
                JFHubCenter.Instance.SystemCfg.AddItem(mZ_safe, Z_safe, categoryName);
        }

        public void SaveStationParams()
        {
            SetCfgParamValue(mRunMode, _RunMode);
            SetCfgParamValue(mNum_Camera, Num_Camera);
            SetCfgParamValue(mSelectIndex, SelectedIndex);
            SetCfgParamValue(mZFocus, ZFocus);
            SetCfgParamValue(mZ_safe, Z_safe);
        }

        public override Form GenForm()
        {
            uv2xy = new FormUV2XY();
            uv2xy.SetStation(this);

            uv2xy.SetupUI();
            return uv2xy;
        }

        string _name = "";
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public override int[] AllCmds { get { return null; } }


        public override int[] AllCustomStatus { get { return null; } }

        public override JFStationRunMode RunMode { get { return JFStationRunMode.Auto; } }

        public override string GetCmdName(int cmd)
        {
            return "";
        }

        public override string GetCustomStatusName(int status)
        {
            return "";
        }

        protected override void CleanupWhenWorkExit()
        {
            return;
        }

        protected override void ExecuteEndBatch()
        {
            return;
        }

        protected override void OnPause()
        {
            return;
        }

        protected override void OnResume()
        {
            return;
        }

        protected override void OnStop()
        {
            return;
        }

        protected override void PrepareWhenWorkStart()
        {
            return;
        }

        protected override void RunLoopInWork()
        {
            return;
        }

        protected override void ExecuteReset()
        {
            return;
        }

        public override bool SetRunMode(JFStationRunMode runMode)
        {
            return false;
        }

        public UcRtCalib _rtUi = new UcRtCalib();

        public override JFRealtimeUI GetRealtimeUI()
        {
            return _rtUi;
            //return base.GetRealtimeUI();
        }
    }
}
