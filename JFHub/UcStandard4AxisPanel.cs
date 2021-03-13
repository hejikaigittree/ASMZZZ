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

    
    /// <summary>
    /// 标准4轴操作面板 XYZR
    /// </summary>
    public partial class UcStandard4AxisPanel : UserControl
    {
        public delegate void DGShowInfo(string info);

        public DGShowInfo EventShowInfo;
        public UcStandard4AxisPanel()
        {
            InitializeComponent();
            btHomes[0] = btHomeX;
            btHomes[1] = btHomeY;
            btHomes[2] = btHomeZ;
            btHomes[3] = btHomeR;
            btAbsMoves[0] = btAbsMoveX; //四个轴的绝对位置移动
            btAbsMoves[1] = btAbsMoveY;
            btAbsMoves[2] = btAbsMoveZ;
            btAbsMoves[3] = btAbsMoveR;
            numAbsPos[0] = numAbsCmdX; //  绝对位置
            numAbsPos[1] = numAbsCmdY;
            numAbsPos[2] = numAbsCmdZ;
            numAbsPos[3] = numAbsCmdR;
            btCfgs[0] = btCfgX; //四轴的设置
            btCfgs[1] = btCfgY;
            btCfgs[2] = btCfgZ;
            btCfgs[3] = btCfgR;
            btPMoves[0] = btPX;//四轴正向移动
            btPMoves[1] = btPY;
            btPMoves[2] = btPZ;
            btPMoves[3] = btPR;
            btNMoves[0]=  btNX; //四轴负向移动
            btNMoves[1] = btNY;
            btNMoves[2] = btNZ;
            btNMoves[3] = btNR;
            tbCurrPoses[0] = tbPosX;
            tbCurrPoses[1] = tbPosY;
            tbCurrPoses[2] = tbPosZ;
            tbCurrPoses[3] = tbPosR;
            btServons[0] = lmpServoX;
            btServons[1] = lmpServoY;
            btServons[2] = lmpServoZ;
            btServons[3] = lmpServoR;
            btServons[0].Click += OnServonButtonClicked;
            btServons[1].Click += OnServonButtonClicked;
            btServons[2].Click += OnServonButtonClicked;
            btServons[3].Click += OnServonButtonClicked;
        }

        bool _isFormLoaded = false;
        private void UcStationStandardAxisPanel_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            cbMoveMode.SelectedIndex = 0;//寸动模式
            AdjustStationView();
        }

        string[] _axisNames = null;

        /// <summary>
        /// 标准XYZR轴数量由数组长度决定
        /// </summary>
        /// <param name="axisNames"></param>
        public void SetAxisNames(string[] axisNames)
        {
            _axisNames = axisNames;
            if (_isFormLoaded)
                AdjustStationView();
        }

        /// <summary>
        /// 更新一下当前4轴的可用性
        /// </summary>
        public void UpdateAxisEnabled()
        {
            if (_isFormLoaded)
                AdjustStationView();
        }


        Button[] btHomes = new Button[4]; //四个轴的归零按钮
        Button[] btAbsMoves = new Button[4]; //四个轴的绝对位置移动
        NumericUpDown[] numAbsPos = new NumericUpDown[4];//  目标位置（绝对）
        Button[] btCfgs = new Button[4];//四轴的设置
        Button[] btPMoves = new Button[4];//四轴正向移动
        Button[] btNMoves = new Button[4];//四轴负向移动
        TextBox[] tbCurrPoses = new TextBox[4];
        LampButton[] btServons = new LampButton[4];

        void AdjustStationView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustStationView));
                return;
            }
            int i = 0;
            if(null != _axisNames)
            {
                for (i = 0; i < _axisNames.Length; i++)
                    CheckAxis(i);
            }
            for(;i<4;i++)
            {
                ShowTips(_AxisAlias[i] + "轴未设置");
                AxisEnabled(i, false);
            }

        }
        string[] _AxisAlias = new string[] { "X", "Y", "Z", "R" };
        void CheckAxis(int index)
        {
            if(string.IsNullOrEmpty(_axisNames[index]))
            {
                ShowTips(_AxisAlias[index] + " 未设置/已禁用");
                AxisEnabled(index, false);
                return;
            }
            JFDevCellInfo dcInfo = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(_axisNames[index]);
            if (null == dcInfo)
            {
                ShowTips(_AxisAlias[index] + "对应轴名称：\"" + _axisNames[index] + "\"在设备命名表中不存在!");
                AxisEnabled(index, false);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(dcInfo.DeviceID) as IJFDevice_MotionDaq;
            if(null == dev)
            {
                ShowTips(string.Format("{0}对应轴：\"{1}\"所属设备\"{2}\"不存在!", _AxisAlias[index], _axisNames[index],dcInfo.DeviceID));
                AxisEnabled(index, false);
                return;
            }
            if(!dev.IsDeviceOpen)
            {
                ShowTips(string.Format("{0}对应轴：\"{1}\"所属设备\"{2}\"未打开!", _AxisAlias[index], _axisNames[index], dcInfo.DeviceID));
                AxisEnabled(index, false);
                return;
            }
            if(dcInfo.ModuleIndex >= dev.McCount)
            {
                ShowTips(string.Format( "{0} 对应轴名称：\"{1}\"所属模块序号{2}超限 (0~{3}) !", _AxisAlias[index], _axisNames[index], dcInfo.ModuleIndex, dev.McCount - 1)) ;
                AxisEnabled(index, false);
                return;
            }
            IJFModule_Motion md = dev.GetMc(dcInfo.ModuleIndex);
            if(dcInfo.ChannelIndex >= md.AxisCount)
            {
                ShowTips(string.Format("{0} 对应轴名称：\"{1}\"所属轴序号{2}超限 (0~{3}) !", _AxisAlias[index], _axisNames[index], dcInfo.ChannelIndex, md.AxisCount - 1));
                AxisEnabled(index, false);
                return;
            }
            AxisEnabled(index, true);

        }

        void AxisEnabled(int index,bool enabled)
        {
           btHomes[index].Enabled = enabled; //四个轴的归零按钮
           btAbsMoves[index].Enabled = enabled; //四个轴的绝对位置移动
            numAbsPos[index].Enabled = enabled; //  绝对位置
            btCfgs[index].Enabled = enabled; //四轴的设置
            btPMoves[index].Enabled = enabled;//四轴正向移动
            btNMoves[index].Enabled = enabled; //四轴负向移动
            btServons[index].Enabled = enabled;
        }

        

        /// <summary>
        /// 将轴状态更新到界面
        /// </summary>
        public void UpdateStationUI()
        {
            int i = 0;
            if(null != _axisNames)
            for(i = 0; i < _axisNames.Length;i++)
                {
                    JFDevCellInfo dcInfo = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(_axisNames[i]);
                    if (dcInfo == null)
                    {
                        tbCurrPoses[i].Text = "无效值";
                        continue;
                    }
                    IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(dcInfo.DeviceID) as IJFDevice_MotionDaq;
                    if (null == dev)
                    {
                        tbCurrPoses[i].Text = "无效值";
                        continue;
                    }
                    if (dcInfo.ModuleIndex >= dev.McCount)
                    {
                        tbCurrPoses[i].Text = "无效值";
                        continue;
                    }
                    IJFModule_Motion md = dev.GetMc(dcInfo.ModuleIndex);
                    if (dcInfo.ChannelIndex >= md.AxisCount)
                    {
                        tbCurrPoses[i].Text = "无效值";
                        continue;
                    }
                    double pos = 0;
                    int errCode = md.GetFbkPos(dcInfo.ChannelIndex, out pos);
                    if (0 != errCode)
                        tbCurrPoses[i].Text = "获取错误！代码:" + errCode;
                    else
                        tbCurrPoses[i].Text = pos.ToString();

                    bool isServon = md.IsSVO(i);
                    btServons[i].LampColor = isServon ? LampButton.LColor.Green : LampButton.LColor.Gray;


                }
            for (; i < 4; i++)
            {
                tbCurrPoses[i].Text = "无效值";
                btServons[i].LampColor = LampButton.LColor.Gray;
            }



        }

        int _maxTips = 50;
        delegate void dgShowTips(string info);
        public void ShowTips(string info)
        {
            EventShowInfo?.Invoke("标准四轴面板->" + info);
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

        JFDevCellInfo CheckAxisDevInfo(string axisName, out string errorInfo)
        {
            JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName); //在命名表中的通道信息
            if (null == ci)
            {
                errorInfo = "未找到轴:\"" + axisName + "\"设备信息";
                return null;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            if (null == dev)
            {
                errorInfo = "未找到轴:\"" + axisName + "\"所属设备:\"" + ci.DeviceID + "\"";
                return null;
            }
            if (!dev.IsDeviceOpen)
            {
                errorInfo = "轴:\"" + axisName + "\"所属设备:\"" + ci.DeviceID + "\"未打开";
                return null;
            }
            if (ci.ModuleIndex >= dev.McCount)
            {
                errorInfo = "轴:\"" + axisName + "\"模块序号:\"" + ci.ModuleIndex + "\"超出限制!";
                return null;
            }
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            if (ci.ChannelIndex >= md.AxisCount)
            {
                errorInfo = "轴:\"" + axisName + "\"通道序号:\"" + ci.ChannelIndex + "\"超出限制!";
                return null;
            }

            errorInfo = "";
            return ci;
        }

        void OnServonButtonClicked(object sender,EventArgs e)
        {
            
            int i = 0;
            for (i = 0; i < btServons.Length; i++)
                if (sender == btServons[i])
                    break;
            string axisName = _axisNames[i];
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errInfo);
            if (null == ci)
            {
                MessageBox.Show("轴" + _axisNames[i] + "伺服操作失败，ErrorInfo:"+ errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            bool isCurrSerOn = md.IsSVO(ci.ChannelIndex);
            int errCode = 0;
            if (isCurrSerOn)
                errCode = md.ServoOff(ci.ChannelIndex);
            else
                errCode = md.ServoOn(ci.ChannelIndex);
            if(errCode != 0)
            {
                MessageBox.Show("轴" + _axisNames[i] + "伺服操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("轴" + _axisNames[i] + "伺服" + (isCurrSerOn?"去使能":"使能")+ "成功" );

        }

        private void btHomeX_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("X轴:\"" + _axisNames[0] + "\" Home操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            int errCode = md.Home(ci.ChannelIndex);
            if(0 != errCode)
            {

                MessageBox.Show("X轴:\"" + _axisNames[0] + "\" Home操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("X轴:\"" + _axisNames[0] + "\" Home运动开始");
        }

        private void btHomeY_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴:\"" + _axisNames[1] + "\" Home操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            int errCode = md.Home(ci.ChannelIndex);
            if (0 != errCode)
            {

                MessageBox.Show("Y轴:\"" + _axisNames[1] + "\" Home操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Y轴:\"" + _axisNames[1] + "\" Home运动开始");
        }

        private void btHomeZ_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴:\"" + _axisNames[2] + "\" Home操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            int errCode = md.Home(ci.ChannelIndex);
            if (0 != errCode)
            {

                MessageBox.Show("Z轴:\"" + _axisNames[2] + "\" Home操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Z轴:\"" + _axisNames[2] + "\" Home运动开始");
        }

        private void btHomeR_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴:\"" + _axisNames[3] + "\" Home操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            int errCode = md.Home(ci.ChannelIndex);
            if (0 != errCode)
            {

                MessageBox.Show("R轴:\"" + _axisNames[3] + "\" Home操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("R轴:\"" + _axisNames[3] + "\" Home运动开始");
        }

        /// <summary>
        /// X轴绝对移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAbsMoveX_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("X轴:\"" + _axisNames[0] + "\" 定位操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            double tgtPos = Convert.ToDouble(numAbsCmdX.Value);
            int errCode = md.AbsMove(ci.ChannelIndex, tgtPos);
            if(errCode != 0)
            {
                MessageBox.Show("X轴:\"" + _axisNames[0] + "\" 定位操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("X轴:\"" + _axisNames[0] + "\" 开始移动到 ：" + tgtPos);
        }

        private void btAbsMoveY_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴:\"" + _axisNames[1] + "\" 定位操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            double tgtPos = Convert.ToDouble(numAbsCmdY.Value);
            int errCode = md.AbsMove(ci.ChannelIndex, tgtPos);
            if (errCode != 0)
            {
                MessageBox.Show("Y轴:\"" + _axisNames[1] + "\" 定位操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Y轴:\"" + _axisNames[1] + "\" 开始移动到 ：" + tgtPos);
        }

        private void btAbsMoveZ_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴:\"" + _axisNames[2] + "\" 定位操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            double tgtPos = Convert.ToDouble(numAbsCmdZ.Value);
            int errCode = md.AbsMove(ci.ChannelIndex, tgtPos);
            if (errCode != 0)
            {
                MessageBox.Show("Z轴:\"" + _axisNames[2] + "\" 定位操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Z轴:\"" + _axisNames[2] + "\" 开始移动到 ：" + tgtPos);
        }

        private void btAbsMoveR_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴:\"" + _axisNames[3] + "\" 定位操作失败，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            double tgtPos = Convert.ToDouble(numAbsCmdR.Value);
            int errCode = md.AbsMove(ci.ChannelIndex, tgtPos);
            if (errCode != 0)
            {
                MessageBox.Show("R轴:\"" + _axisNames[3] + "\" 定位操作失败，ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("R轴:\"" + _axisNames[3] + "\" 开始移动到 ：" + tgtPos);
        }

        /// <summary>
        /// 显示X轴全功能界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCfgX_Click(object sender, EventArgs e)
        {

            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("X轴:\"" + _axisNames[0] + "\" 未能显示参数配置窗口，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            FormAxisTest fm = new FormAxisTest();
            fm.SetAxisInfo(md, ci.ChannelIndex, _axisNames[0]);
            fm.ShowDialog();

        }

        private void btCfgY_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴:\"" + _axisNames[1] + "\" 未能显示参数配置窗口，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            FormAxisTest fm = new FormAxisTest();
            fm.SetAxisInfo(md, ci.ChannelIndex, _axisNames[1]);
            fm.ShowDialog();
        }

        private void btCfgZ_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴:\"" + _axisNames[2] + "\" 未能显示参数配置窗口，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            FormAxisTest fm = new FormAxisTest();
            fm.SetAxisInfo(md, ci.ChannelIndex, _axisNames[2]);
            fm.ShowDialog();
        }

        private void btCfgR_Click(object sender, EventArgs e)
        {
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴:\"" + _axisNames[3] + "\" 未能显示参数配置窗口，ErrorInfo:" + errInfo);
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            FormAxisTest fm = new FormAxisTest();
            fm.SetAxisInfo(md, ci.ChannelIndex, _axisNames[3]);
            fm.ShowDialog();
        }

        /// <summary>
        /// 停止XYZR轴的移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStop_Click(object sender, EventArgs e)
        {
            bool isOK = true;
            foreach(string axisName in _axisNames)
            {
                string errInfo;
                JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errInfo);
                if (null != ci)
                {
                    IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
                    int errCode = md.StopAxis(ci.ChannelIndex);
                    if(errCode != 0)
                    {
                        isOK = false;
                        ShowTips("停止轴:\"" + axisName + "\" 失败,Errornfo: " + md.GetErrorInfo(errCode));
                    }
                }
                else
                {
                    isOK = false;
                    ShowTips("停止轴:\"" + axisName + "\" 失败,未找到通道信息 ");
                }
                if (isOK)
                    ShowTips("所有轴已停止");
            }
        }

        double _dVelParam = 10; //连续运动模式下的速度参数
        double _dStepParam = 1;//寸动参数
        private void cbMoveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex < 0)
                return;
            if(0 == cbMoveMode.SelectedIndex) //寸动模式
            {
                lbMoveParam.Text = "步长";
                numMoveParam.Value = Convert.ToDecimal(_dStepParam);
            }
            else if(1 == cbMoveMode.SelectedIndex) //连续模式
            {
                lbMoveParam.Text = "速度";
                numMoveParam.Value = Convert.ToDecimal(_dVelParam);
            }
        }

        private void MoveParam_ValueChanged(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex < 0)
                return;
            if(0 == numMoveParam.Value)
            {
                MessageBox.Show(cbMoveMode.SelectedIndex == 0 ? "步长" : "速度" + "参数不能为0！");
                numMoveParam.Value = Convert.ToDecimal(0.001);
                return;
            }
            if(cbMoveMode.SelectedIndex == 0) //寸动模式
            {
                _dStepParam = Convert.ToDouble(numMoveParam.Value);
            }
            else if (cbMoveMode.SelectedIndex == 1)//连续模式
            {
                _dVelParam = Convert.ToDouble(numMoveParam.Value);
            }
        }

        /// <summary>
        /// 相对运动(寸动)X+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPX_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0],out errInfo);
            if(null == ci)
            {
                MessageBox.Show("X轴正向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, _dStepParam);
            if(errCode != 0)
            {
                MessageBox.Show("X轴正向寸动失败！，AxisName = \"" + _axisNames[0] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("X轴正向寸动OK");


        }

        private void btNX_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("X轴负向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, -_dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("X轴负向寸动失败！，AxisName = \"" + _axisNames[0] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("X轴负向寸动OK");
        }

        private void btPY_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴正向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, _dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("Y轴正向寸动失败！，AxisName = \"" + _axisNames[1] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Y轴正向寸动OK");
        }

        private void btNY_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴负向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, -_dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("Y轴负向寸动失败！，AxisName = \"" + _axisNames[1] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Y轴负向寸动OK");
        }

        private void btPZ_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴正向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, _dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("Z轴正向寸动失败！，AxisName = \"" + _axisNames[2] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Z轴正向寸动OK");
        }

        private void btNZ_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴负向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, -_dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("Z轴负向寸动失败！，AxisName = \"" + _axisNames[2] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("Z轴负向寸动OK");
        }

        private void btPR_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴正向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, _dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("R轴正向寸动失败！，AxisName = \"" + _axisNames[3] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("R轴正向寸动OK");
        }

        private void btNR_Click(object sender, EventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 0) //未选中寸动模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴负向寸动失败！配置中没有通道信息，Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }

            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, -_dStepParam);
            if (errCode != 0)
            {
                MessageBox.Show("R轴负向寸动失败！，AxisName = \"" + _axisNames[3] + "\" ErrorInfo:" + md.GetErrorInfo(errCode));
                return;
            }
            ShowTips("R轴负向寸动OK");

        }

        /// <summary>
        /// 开始连续运动X正向
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPX_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("X轴正向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, true);
            if(errCode != 0)
            {
                MessageBox.Show("X轴正向连续运动失败！Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }

        }

        /// <summary>
        /// 停止连续运动X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPX_MouseUp(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
                return;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.StopAxis(ci.ChannelIndex);
            if (errCode != 0)
            {
                ShowTips("X轴停止运动失败！Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }
            ShowTips("X轴已停止");
        }

        private void btNX_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[0], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("X轴负向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, false);
            if (errCode != 0)
            {
                MessageBox.Show("X轴负向连续运动失败！Name = \"" + _axisNames[0] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btNX_MouseUp(object sender, MouseEventArgs e)
        {
            btPX_MouseUp(sender, e);
        }

        private void btPY_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴正向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, true);
            if (errCode != 0)
            {
                MessageBox.Show("Y轴正向连续运动失败！Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btPY_MouseUp(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
                return;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.StopAxis(ci.ChannelIndex);
            if (errCode != 0)
            {
                ShowTips("Y轴停止运动失败！Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }
            ShowTips("Y轴已停止");
        }

        private void btNY_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[1], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Y轴负向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, false);
            if (errCode != 0)
            {
                MessageBox.Show("Y轴负向连续运动失败！Name = \"" + _axisNames[1] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btNY_MouseUp(object sender, MouseEventArgs e)
        {
            btPY_MouseUp(sender, e);
        }

        private void btPZ_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴正向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, true);
            if (errCode != 0)
            {
                MessageBox.Show("Z轴正向连续运动失败！Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btPZ_MouseUp(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
                return;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.StopAxis(ci.ChannelIndex);
            if (errCode != 0)
            {
                ShowTips("Z轴停止运动失败！Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }
            ShowTips("Z轴已停止");
        }

        private void btNZ_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[2], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("Z轴负向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, false);
            if (errCode != 0)
            {
                MessageBox.Show("Z轴负向连续运动失败！Name = \"" + _axisNames[2] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btNZ_MouseUp(object sender, MouseEventArgs e)
        {
            btPZ_MouseUp(sender, e);
        }

        private void btPR_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴正向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, false);
            if (errCode != 0)
            {
                MessageBox.Show("R轴正向连续运动失败！Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btPR_MouseUp(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
                return;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.StopAxis(ci.ChannelIndex);
            if (errCode != 0)
            {
                ShowTips("R轴停止运动失败！Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }
            ShowTips("R轴已停止");
        }

        private void btNR_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbMoveMode.SelectedIndex != 1) //未选中连续模式
                return;
            string errInfo;
            JFDevCellInfo ci = CheckAxisDevInfo(_axisNames[3], out errInfo);
            if (null == ci)
            {
                MessageBox.Show("R轴负向连续运动失败！配置中没有通道信息，Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.VelMove(ci.ChannelIndex, _dVelParam, false);
            if (errCode != 0)
            {
                MessageBox.Show("R轴负向连续运动失败！Name = \"" + _axisNames[3] + "\" ErrorInfo:" + errInfo);
                return;
            }
        }

        private void btNR_MouseUp(object sender, MouseEventArgs e)
        {
            btPR_MouseUp(sender, e);
        }


    }
}
