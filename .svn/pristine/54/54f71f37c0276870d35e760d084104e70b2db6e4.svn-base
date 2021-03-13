using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFUI;
using JFInterfaceDef;

namespace JFHub
{
    public partial class FormStationBaseDioPanel : Form
    {
        public FormStationBaseDioPanel()
        {
            InitializeComponent();
        }


        //int ioButtonWidth = 80;
        //[Category("JFUI属性"), Description("IO按钮高度"), Browsable(true)]
        //public int IOButtonWidth
        //{
        //    get { return ioButtonWidth}
        //}

        bool _isFormLoaded = false;
        private void FormStationBaseDioPanel_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustStationUI();
        }

        int _maxTips = 500;
        delegate void dgShowTips(string info);
        void ShowTips(string info)
        {
            if (InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            rchTips.AppendText(info + "\n");
            string[] lines = rchTips.Text.Split('\n');
            if (lines.Length >= _maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - _maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rchTips.Text = rchTips.Text.Substring(rmvChars);
            }
            rchTips.Select(rchTips.TextLength, 0); //滚到最后一行
            rchTips.ScrollToCaret();//滚动到控件光标处 
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            bool isFlushEnable = timerFlush.Enabled;
            if (timerFlush.Enabled)
                timerFlush.Enabled = false;

            _station = station;
            if (_isFormLoaded)
                AdjustStationUI();

            if (isFlushEnable && _station != null)
                timerFlush.Enabled = true;
            else
                UpdateStationUI();
        }

        List<string> diNamesInView = new List<string>(); //本窗口保存的Di名称
        List<string> doNamesInView = new List<string>();

        int _ioButtonWidth = 180;
        int _ioButtonHeight = 20;
        /// <summary>
        /// 根据工站的DIO确定控件数量
        /// </summary>
        public void AdjustStationUI()
        {
            //rchTips.Text = "";
            if (_station == null)
            {
                diNamesInView.Clear();
                doNamesInView.Clear();
                panelDIs.Controls.Clear();
                panelDOs.Controls.Clear();
                ShowTips("工站未设置");
                btOpenAllDev.Enabled = false;
                return;
            }
            btOpenAllDev.Enabled = true;
            string[] doNamesInStation = _station.DONames; //当前
            string[] diNamesInStation = _station.DINames;
            bool isNeedReAdjustDo = false; // 需要重新规划控件（IO数量/名称/排列发生变化）
            do
            {
                if (doNamesInStation == null || 0 == doNamesInStation.Length)
                {
                    if (doNamesInView.Count != 0)
                    {
                        isNeedReAdjustDo = true;
                        break;
                    }
                }
                else
                {
                    if (doNamesInView.Count != doNamesInStation.Length)
                    {
                        isNeedReAdjustDo = true;
                        break;
                    }
                    for (int i = 0; i < doNamesInView.Count; i++)
                        if (doNamesInView[i] != doNamesInStation[i])
                        {
                            isNeedReAdjustDo = true;
                            break;
                        }
                }
            } while (false);
            if(isNeedReAdjustDo)
            {
                doNamesInView.Clear();
                panelDOs.Controls.Clear();
                if (null != doNamesInStation)
                    doNamesInView.AddRange(doNamesInStation);
                for(int i = 0; i < doNamesInView.Count;i++)
                {
                    LampButton lampDo = new LampButton();
                    lampDo.Size = new Size(_ioButtonWidth, _ioButtonHeight);
                    lampDo.Text = doNamesInView[i];
                    lampDo.Click += new EventHandler(DOButtonClicked);
                    panelDOs.Controls.Add(lampDo);
                }
            }

            bool isNeedReAdjustDi = false;
            do { 
                if (diNamesInStation == null || 0 == diNamesInStation.Length)
                {
                    if (diNamesInView.Count != 0)
                    {
                        isNeedReAdjustDi = true;
                        break;
                    }
                }
                else
                {
                    if (diNamesInView.Count != diNamesInStation.Length)
                    {
                        isNeedReAdjustDi = true;
                        break;
                    }
                    for (int i = 0; i < diNamesInView.Count; i++)
                        if (diNamesInView[i] != diNamesInStation[i])
                        {
                            isNeedReAdjustDi = true;
                            break;
                        }
                }

            } while (false);
            if (isNeedReAdjustDi) 
            {
                diNamesInView.Clear();
                panelDIs.Controls.Clear();
                if (null != diNamesInStation)
                    diNamesInView.AddRange(diNamesInStation);
                for (int i = 0; i < diNamesInView.Count; i++)
                {
                    LampButton lampDi = new LampButton();
                    lampDi.Enabled = false;
                    lampDi.Size = new Size(_ioButtonWidth, _ioButtonHeight);
                    lampDi.Text = diNamesInView[i];
                    panelDIs.Controls.Add(lampDi);   
                }

            }

            ////更新按钮提示信息
            toolTips.RemoveAll();
            
            for (int i = 0; i < doNamesInView.Count; i++)
            {
                LampButton lampDo = panelDOs.Controls[i] as LampButton;
                JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDoCellInfo(doNamesInView[i]);
                do
                {
                    if (ci == null) //名称在配置中不存在
                    {
                        lampDo.LampColor = LampButton.LColor.Gray;
                        lampDo.ForeColor = Color.Red;
                        lampDo.Enabled = false;
                        toolTips.SetToolTip(lampDo, "名称在命名配置表中不存在");
                        ShowTips(string.Format("DO: \"{0}\" 在命名配置表中不存在", doNamesInView[i]));
                        break;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                    if (null == dev)
                    {
                        lampDo.LampColor = LampButton.LColor.Gray;
                        lampDo.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        lampDo.Enabled = false;
                        toolTips.SetToolTip(lampDo, "未发现所属设备:" + ci.DeviceID);
                        ShowTips(string.Format("DO: \"{0}\" 所属设备 \"{1}\" 在系统中不存在", doNamesInView[i],ci.DeviceID));
                        break;
                    }
                    if (!dev.IsDeviceOpen)
                    {
                        lampDo.LampColor = LampButton.LColor.Gray;
                        lampDo.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        lampDo.Enabled = false;
                        ShowTips(string.Format("DO: \"{0}\" 所属设备 \"{1}\" 未打开（关闭状态）", doNamesInView[i], ci.DeviceID));
                        toolTips.SetToolTip(lampDo, "设备未打开");
                        break;
                    }
                    if (dev.DioCount <= ci.ModuleIndex)
                    {
                        lampDo.LampColor = LampButton.LColor.Gray;
                        lampDo.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        lampDo.Enabled = false;
                        toolTips.SetToolTip(lampDo, "所属模块序号超限");
                        ShowTips(string.Format("DO: \"{0}\" 所属设备 \"{1}\" 所属模块Index = {2} 超出范围0~{3}", doNamesInView[i], ci.DeviceID,ci.ModuleIndex, dev.DioCount == 0?0: dev.DioCount-1));
                        break;
                    }
                    IJFModule_DIO md = dev.GetDio(ci.ModuleIndex);
                    if (md.DOCount <= ci.ChannelIndex)
                    {
                        lampDo.LampColor = LampButton.LColor.Gray;
                        lampDo.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        lampDo.Enabled = false;
                        toolTips.SetToolTip(lampDo, "通道序号超限");
                        ShowTips(string.Format("DO: \"{0}\" 通道序号:{1} 超出范围0~{2}", doNamesInView[i], ci.ChannelIndex, md.DOCount == 0 ? 0 : md.DOCount - 1));
                        break;
                    }
                    lampDo.Enabled = true;
                    lampDo.ForeColor = Color.Black;

                } while (false);
            }

            for (int i = 0; i < diNamesInView.Count; i++)
            {
                LampButton lampDi = panelDIs.Controls[i] as LampButton;
                JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDiCellInfo(diNamesInView[i]);
                do
                {
                    if (ci == null) //名称在配置中不存在
                    {
                        lampDi.LampColor = LampButton.LColor.Gray;
                        lampDi.ForeColor = Color.Red;
                        toolTips.SetToolTip(lampDi, "名称在命名配置表中不存在");
                        ShowTips(string.Format("DI: \"{0}\" 在命名配置表中不存在", diNamesInView[i]));
                        break;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                    if (null == dev)
                    {
                        lampDi.LampColor = LampButton.LColor.Gray;
                        lampDi.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        toolTips.SetToolTip(lampDi, "未发现所属设备:" + ci.DeviceID);
                        ShowTips(string.Format("DI: \"{0}\" 所属设备 \"{1}\" 在系统中不存在", diNamesInView[i], ci.DeviceID));
                        break;
                    }
                    if (!dev.IsDeviceOpen)
                    {
                        lampDi.LampColor = LampButton.LColor.Gray;
                        lampDi.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        ShowTips(string.Format("DOI: \"{0}\" 所属设备 \"{1}\" 未打开（关闭状态）", diNamesInView[i], ci.DeviceID));
                        toolTips.SetToolTip(lampDi, "设备未打开");
                        break;
                    }
                    if (dev.DioCount <= ci.ModuleIndex)
                    {
                        lampDi.LampColor = LampButton.LColor.Gray;
                        lampDi.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        toolTips.SetToolTip(lampDi, "所属模块序号超限");
                        ShowTips(string.Format("DI: \"{0}\"  所属设备 \"{1}\" 所属模块Index = {2} 超出范围0~{3}", diNamesInView[i], ci.DeviceID, ci.ModuleIndex, dev.DioCount == 0 ? 0 : dev.DioCount - 1));
                        break;
                    }
                    IJFModule_DIO md = dev.GetDio(ci.ModuleIndex);
                    if (md.DICount <= ci.ChannelIndex)
                    {
                        lampDi.LampColor = LampButton.LColor.Gray;
                        lampDi.ForeColor = Color.Red;//ucdo.IONameTextColor = Color.Red;
                        toolTips.SetToolTip(lampDi, "通道序号超限");
                        ShowTips(string.Format("DI: \"{0}\" 通道序号:{1} 超出范围0~{2}", diNamesInView[i], ci.ChannelIndex, md.DICount == 0 ? 0 : md.DOCount - 1));
                        break;
                    }
                    //lampDo.Enabled = true;
                    lampDi.ForeColor = Color.Black;

                } while (false);
            }

        }

        /// <summary>
        /// 将工站IO状态更新到界面上
        /// </summary>
        void UpdateStationUI()
        {
            if (_station == null)
                return;
            for (int i = 0; i < doNamesInView.Count; i++)
            {
                string doName = doNamesInView[i];
                LampButton lmpdo = panelDOs.Controls[i] as LampButton;
                JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDoCellInfo(doName);
                do
                {
                    if (ci == null) //名称在配置中不存在
                    {
                        lmpdo.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                    if (null == dev)
                    {
                        lmpdo.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                   if (!dev.IsDeviceOpen)
                   {
                        lmpdo.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                    if (dev.DioCount <= ci.ModuleIndex)
                    {
                        lmpdo.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                    
                    IJFModule_DIO md = dev.GetDio(ci.ModuleIndex);
                    if (md.DOCount <= ci.ChannelIndex)
                    {
                        lmpdo.LampColor = LampButton.LColor.Gray;
                        break;
                    }

                   bool isOn = false;
                   int errCode = md.GetDO(ci.ChannelIndex, out isOn);
                    if (0 == errCode)
                    {
                        lmpdo.ForeColor = Color.Black;
                        lmpdo.LampColor = isOn ? LampButton.LColor.Green : LampButton.LColor.Gray;
                    }
                    else
                    {
                        lmpdo.LampColor = LampButton.LColor.Gray;
                        lmpdo.ForeColor = Color.Orange;//ucdo.IONameTextColor = Color.Red;
                    } 
                } while (false);
            }

            for (int i = 0; i < diNamesInView.Count; i++)
            {
                string diName = diNamesInView[i];
                LampButton lmpdi = panelDIs.Controls[i] as LampButton;
                JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDiCellInfo(diName);
                do
                {
                    if (ci == null) //名称在配置中不存在
                    {
                        lmpdi.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                    if (null == dev)
                    {
                        lmpdi.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                    if (!dev.IsDeviceOpen)
                    {
                        lmpdi.LampColor = LampButton.LColor.Gray;
                        break;
                    }
                    if (dev.DioCount <= ci.ModuleIndex)
                    {
                        lmpdi.LampColor = LampButton.LColor.Gray;
                        break;
                    }

                    IJFModule_DIO md = dev.GetDio(ci.ModuleIndex);
                    if (md.DICount <= ci.ChannelIndex)
                    {
                        lmpdi.LampColor = LampButton.LColor.Gray;
                        break;
                    }

                    bool isOn = false;
                    int errCode = md.GetDI(ci.ChannelIndex, out isOn);
                    if (0 == errCode)
                    {
                        lmpdi.ForeColor = Color.Black;
                        lmpdi.LampColor = isOn ? LampButton.LColor.Green : LampButton.LColor.Gray;
                    }
                    else
                    {
                        lmpdi.LampColor = LampButton.LColor.Gray;
                        lmpdi.ForeColor = Color.Orange;//ucdo.IONameTextColor = Color.Red;
                    }
                } while (false);
            }
        }

        void DOButtonClicked(object sender,EventArgs eventArgs)
        {
            LampButton doBt= sender as LampButton;
            string doName = doBt.Text;

            JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDoCellInfo(doName);

            if (ci == null) //名称在配置中不存在
            {
                ShowTips("DO操作失败:名称 \"" + doName + "\" 不存在");
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            if (null == dev)
            {
                ShowTips(string.Format("DO: \"{0}\" 操作失败:所属设备 \"{1}\" 在系统中不存在", doName, ci.DeviceID));
                return;
            }
            if (!dev.IsDeviceOpen)
            {
                ShowTips(string.Format("DO \"{0}\" 操作失败:所属设备 \"{1}\" 未打开（关闭状态）", doName, ci.DeviceID));
                return;
            }
            if (dev.DioCount <= ci.ModuleIndex)
            {
                
                ShowTips(string.Format("DO \"{0}\" 操作失败:所属设备 \"{1}\" 所属模块Index = {2} 超出范围0~{3}", doName, ci.DeviceID, ci.ModuleIndex, dev.DioCount == 0 ? 0 : dev.DioCount - 1));
                return;
            }
            IJFModule_DIO md = dev.GetDio(ci.ModuleIndex);
            if (md.DOCount <= ci.ChannelIndex)
            {
                ShowTips(string.Format("DO \"{0}\" 操作失败:通道序号:{1} 超出范围0~{2}", doName, ci.ChannelIndex, md.DOCount == 0 ? 0 : md.DOCount - 1));
                return;
            }
            bool isON = false;
            int errCode = md.GetDO(ci.ChannelIndex, out isON);
            if(0!= errCode)
            {
                ShowTips(string.Format("DO \"{0}\" 操作失败:未能获取当前状态，信息:{1}", doName, md.GetErrorInfo(errCode)));
                return;
            }

            errCode = md.SetDO(ci.ChannelIndex, !isON);
            if (0 != errCode)
            {
                ShowTips(string.Format("DO \"{0}\" {1}操作失败:未能，信息:{2}", doName,isON?"关闭":"打开", md.GetErrorInfo(errCode)));
                return;
            }



        }

        public void FormStationBaseDioPanel_VisibleChanged(object sender, EventArgs e)
        {
            timerFlush.Enabled = Visible;
            if (Visible)
            {
                
            }
            else
            {

            }
        }

        private void FormStationBaseDioPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 刷新IO状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerFlush_Tick(object sender, EventArgs e)
        {
            if(null == _station)
            {
                timerFlush.Enabled = false;
                ShowTips("工站为空，刷新定时器关闭！");
                return;
            }
            UpdateStationUI();
        }




        //清除提示信息
        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        private void btReflush_Click(object sender, EventArgs e)
        {
            AdjustStationUI();
        }

        private void btOpenAllDev_Click(object sender, EventArgs e)
        {
            if(null == _station)
            {
                ShowTips("打开所有DIO设备失败:工站未设置");
                return;
            }
            string[] diNames = _station.DINames;
            string[] doNames = _station.DONames;
            if((null == diNames || 0 == diNames.Length)&& (null == doNames || 0 == doNames.Length))
            {
                ShowTips("打开所有DIO设备失败:工站未设置DI/DO");
                return;
            }
            bool isOK = true;
            if(null != diNames)
                foreach(string diName in diNames)
                {
                    JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDiCellInfo(diName);
                    if(null == ci)
                    {
                        isOK = false;
                        ShowTips("未发现DI = \"" + diName + "\"所属 通道信息！");
                        continue;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                    if(null == dev)
                    {
                        isOK = false;
                        ShowTips("未发现DI = \"" + diName + "\"所属 设备ID = \""+ ci.DeviceID +"\"!");
                        continue;
                    }
                    if(!dev.IsDeviceOpen)
                    {
                        int errCode = dev.OpenDevice();
                        if(0 != errCode)
                        {
                            isOK = false;
                            ShowTips(string.Format("打开DI = \"{0}\"所属设备=\"{1}\"失败，错误信息:{2}",diName,ci.DeviceID,dev.GetErrorInfo(errCode)));
                            continue;
                        }
                        ShowTips(string.Format("DI = \"{0}\"所属设备=\"{1}\"已打开 ", diName, ci.DeviceID));
                    }
                }


            if (null != doNames)
                foreach (string doName in doNames)
                {
                    JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetDoCellInfo(doName);
                    if (null == ci)
                    {
                        isOK = false;
                        ShowTips("未发现DO = \"" + doName + "\"所属 通道信息！");
                        continue;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                    if (null == dev)
                    {
                        isOK = false;
                        ShowTips("未发现DO = \"" + doName + "\"所属 设备ID = \"" + ci.DeviceID + "\"!");
                        continue;
                    }
                    if (!dev.IsDeviceOpen)
                    {
                        int errCode = dev.OpenDevice();
                        if (0 != errCode)
                        {
                            isOK = false;
                            ShowTips(string.Format("打开DO = \"{0}\"所属设备=\"{1}\"失败，错误信息:{2}", doName, ci.DeviceID, dev.GetErrorInfo(errCode)));
                            continue;
                        }
                        ShowTips(string.Format("DO = \"{0}\"所属设备=\"{1}\"已打开 ", doName, ci.DeviceID));
                    }
                }
            if (isOK)
                ShowTips("工站所有DIO设备已打开");
            AdjustStationUI();


        }
    }
}
