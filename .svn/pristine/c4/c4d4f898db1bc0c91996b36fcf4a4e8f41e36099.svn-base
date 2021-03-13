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
using JFToolKits;
using System.Threading;

namespace JFUI
{
    public partial class UcAxisTest : UserControl
    {
        public enum JFDisplayMode
        {
            full = 0, //控件全部显示
            middling,
            simple, //简单界面 ，显示位置/速度操作面板
            simplest_pos,//只显示位置操作面板
            simplest_vel,//只显示速度操作面板
        }
        public UcAxisTest()
        {
            InitializeComponent();
            IsBoxShowError = false;

        }

        private void UcAxisTest_Load(object sender, EventArgs e)
        {
            isLoaded = true;
            cbHomeParamMode.Items.Clear();
            cbHomeParamMode.Items.AddRange(new string[] { "原点", "限位", "EZ" });//HomeMode = 0,1,2
            cbHomeParamDir.Items.AddRange(new string[] { "正", "负" });
            cbHomeParamEzEnable.Items.AddRange(new string[] { "是", "否" });
            AdjustAxisView();
            UpdateAllAxisData2UI();
            //UpdateAxisUI(); 
        }

        [Category("JF属性"), Description("发生错误时显示消息框"), Browsable(true)]
        public bool IsBoxShowError { get; set; }


        JFDisplayMode _displayMode = JFDisplayMode.full;
        [Category("JF属性"), Description("显示模式"), Browsable(true)]
        public JFDisplayMode DisplayMode 
        {
            get { return _displayMode; } 
            set
            {
                if (value == _displayMode)
                    return;
                _displayMode = value;
                if (isLoaded)
                {
                    AdjustAxisView();
                    UpdateAllAxisData2UI();//UpdateAxisUI();
                }
            }
        }




        /// <summary>
        /// 是否正在做往复运动
        /// </summary>
        public bool IsRepeating { get; set; }


        int maxTips = 100;//最多显示100条提示信息
        delegate void dgShowTip(string txt);
        public void ShowTip(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return;
            if (InvokeRequired)
            {
                Invoke(new dgShowTip(ShowTip), new object[] { txt });
                return;
            }

            rtbTips.AppendText(txt + "\n");
            string[] lines = rtbTips.Text.Split('\n');
            if (lines.Length >= maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rtbTips.Text = rtbTips.Text.Substring(rmvChars);
            }
            rtbTips.Select(rtbTips.TextLength, 0); //滚到最后一行
            rtbTips.ScrollToCaret();//滚动到控件光标处 
        }

        void ShowError(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return;
            ShowTip(txt);
            if (IsBoxShowError && DisplayMode != JFDisplayMode.full)
                MessageBox.Show(txt);
        }

        public void SetAxis(IJFModule_Motion module,int axis)
        {
            motion = module;
            this.axis = axis;
            if (isLoaded)
            {
                AdjustAxisView();
                UpdateAllAxisData2UI();
            }
            
        }

        bool CheckMotionAxis(out string errInfo)
        {
            errInfo = "Success";
            if (null == motion || axis < 0 || axis >= motion.AxisCount)
            {
                if (null == motion)
                    errInfo = "运动控制模块未设置！";
                else
                {
                    if (axis < 0)
                        errInfo = "运动控制模块未设置！";
                    else
                        errInfo = string.Format("轴号Axis = {0}超出限制:0~{1}", axis, motion.AxisCount - 1);
                }
                
                return false;
            }
            return true;
        }

        int orgWidth = 480, orgHeight = 360;//界面原始尺寸
        int simpleWidth = 292, simpleHeight = 100; //简单模式尺寸
        int simplestWidth = 292, simplestHeight = 50;
        int middingWidth = 480, middingHeight = 197;
        int locX1 = 91, locY1 = 1;//位置移动面板的起始位置
        int locX2 = 91, locY2 = 51;//速度移动面板的起始位置

        int pnCfgMiddingHeight = 197, pnCfgFullHeight = 274;
        Point btWriteLocFull = new Point(117, 247);
        Point btWriteLocMid = new Point(3, 143);


        int orgPnHeight = 250;//全尺寸时，操作面板的高度

        // 布置控件布局，SetAxis /DisplayMode发生改变后调用
        public void AdjustAxisView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustAxisView));
                return;
            }
            ////界面显示等级
            switch (DisplayMode)
            {
                case JFDisplayMode.full:
                    pnAction.Size = new Size(pnAction.Size.Width, orgPnHeight);
                    btClearTips.Visible = true;
                    rtbTips.Visible = true;
                    panelCfg.Size = new Size(panelCfg.Size.Width, pnCfgFullHeight);
                    panelCfg.Visible = true;
                    pnPtMove.Location = new Point(locX1, locY1);
                    pnPtMove.Visible = true;
                    pnSpdMove.Location = new Point(locX2, locY2);
                    pnSpdMove.Visible = true;
                    pnMotionParam.Visible = true;
                    pnRepeat.Visible = true;
                    btWriteAxisParam.Parent = panelCfg;
                    btWriteAxisParam.Location = btWriteLocFull;
                    Size = new Size(orgWidth, orgHeight);
                    break;
                case JFDisplayMode.middling:
                    pnAction.Size = new Size(pnAction.Size.Width, middingHeight);
                    btClearTips.Visible = false;
                    rtbTips.Visible = false;
                    panelCfg.Size = new Size(panelCfg.Size.Width, pnCfgMiddingHeight);
                    panelCfg.Visible = true;
                    pnPtMove.Location = new Point(locX1, locY1);
                    pnPtMove.Visible = true;
                    pnSpdMove.Location = new Point(locX2, locY2);
                    pnSpdMove.Visible = true;
                    pnMotionParam.Visible = true;
                    pnRepeat.Visible = false;
                    btWriteAxisParam.Parent = groupBox2;
                    btWriteAxisParam.Location = btWriteLocMid;
                    Size = new Size(middingWidth, middingHeight);
                    break;
                case JFDisplayMode.simple:
                    pnAction.Size = new Size(pnAction.Size.Width, simpleHeight);
                    btClearTips.Visible = false;
                    rtbTips.Visible = false;
                    panelCfg.Visible = false;
                    pnPtMove.Location = new Point(locX1, locY1);
                    pnPtMove.Visible = true;
                    pnSpdMove.Location = new Point(locX2, locY2);
                    pnSpdMove.Visible = true;
                    pnMotionParam.Visible = false;
                    pnRepeat.Visible = false;
                    Size = new Size(simpleWidth, simpleHeight);
                    break;
                case JFDisplayMode.simplest_pos:
                    pnAction.Size = new Size(pnAction.Size.Width, simplestHeight);
                    btClearTips.Visible = false;
                    rtbTips.Visible = false;
                    panelCfg.Visible = false;
                    pnPtMove.Location = new Point(locX1, locY1);
                    pnPtMove.Visible = true;
                    pnSpdMove.Location = new Point(locX2, locY2);
                    pnSpdMove.Visible = false;
                    pnMotionParam.Visible = false;
                    pnRepeat.Visible = false;
                    Size = new Size(simplestWidth, simplestHeight);
                    break;
                case JFDisplayMode.simplest_vel:
                    pnAction.Size = new Size(pnAction.Size.Width, simplestHeight);
                    btClearTips.Visible = false;
                    rtbTips.Visible = false;
                    panelCfg.Visible = false;
                    pnPtMove.Location = new Point(locX1, locY1);
                    pnPtMove.Visible = false;
                    pnSpdMove.Location = new Point(locX1, locY1);
                    pnSpdMove.Visible = true;
                    pnMotionParam.Visible = false;
                    pnRepeat.Visible = false;
                    Size = new Size(simplestWidth, simplestHeight);
                    break;
                default:
                    break;
            }

           


        }

        //将所有数据更新到界面上
        public void UpdateAllAxisData2UI() //更新轴状态到界面
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateAxisUI));
                return;
            }


            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowTip(errInfo);
                Enabled = false;
                return;
            }
            if (!motion.IsOpen)
            {
                ShowTip("模块未打开！");
                Enabled = false;
            }
            Enabled = true;

            if (!motion.IsSVO(axis))
                btSvo.Text = "伺服on";
            else
                btSvo.Text = "伺服of";

            string err;
            if (!ReadMotionParam(out err))
                ShowTip(err);
            if (!ReadAxisParam(out err))
                ShowTip(err);


        }

        public void UpdateAxisUI() //更新轴状态到界面
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateAxisUI));
                return;
            }
            

            string errInfo;
            if(!CheckMotionAxis(out errInfo))
            {
                ShowTip(errInfo);
                Enabled = false;
                return;
            }
            if (!motion.IsOpen)
            {
                ShowTip("模块未打开！");
                Enabled = false;
            }
            Enabled = true;

            if (!motion.IsSVO(axis))
                btSvo.Text = "伺服on";
            else
                btSvo.Text = "伺服of";

            //string err;
            //if (!ReadMotionParam(out err))
            //    ShowTip(err);
            //if (!ReadAxisParam(out err))
            //    ShowTip(err);
            if (pnLtc.Visible)
                ReadLctStatus();


        }

        bool ReadMotionParam(out string errInfo)
        {
            errInfo = "Success";
            JFMotionParam mp;
            int ret = motion.GetMotionParam(axis, out mp);
            if(ret != 0)
            {
                numMotionParamAcc.ResetText();
                numMotionParamDec.ResetText();
                numMotionParamVm.ResetText();
                numMotionParamVs.ResetText();
                numMotionParamVe.ResetText();
                numMotionParamCv.ResetText();
                numMotionParamJk.ResetText();


                errInfo = "获取轴运动参数失败：" + motion.GetErrorInfo(ret);
                return false;
            }
            
                numMotionParamAcc.Value = Convert.ToDecimal(mp.acc);
                numMotionParamDec.Value = Convert.ToDecimal(mp.dec);
                numMotionParamVm.Value = Convert.ToDecimal(mp.vm);
                numMotionParamVs.Value = Convert.ToDecimal(mp.vs);
                numMotionParamVe.Value = Convert.ToDecimal(mp.ve);
                numMotionParamCv.Value = Convert.ToDecimal(mp.curve);
                numMotionParamJk.Value = Convert.ToDecimal(mp.jerk);
           return true;
        }

        bool ReadAxisParam(out string errInfo)
        {
            errInfo = "";
            bool ret = true;
            double pulseFact;
            int err = motion.GetPulseFactor(axis, out pulseFact);
            if (0 != err)
            {
                ret = false;
                numPulseFactor.ResetText();
                errInfo = "获取电机脉冲当量参数失败，Error:" + motion.GetErrorInfo(err) + "\n";
                numPulseFactor.BackColor = Color.Red;
            }
            else
            {
                numPulseFactor.Value = Convert.ToDecimal(pulseFact);
                numPulseFactor.BackColor = Color.White;
            }

            JFHomeParam hp;
            err = motion.GetHomeParam(axis, out hp);
            if (0 != err)
            {
                ret = false;
                cbHomeParamMode.Text = "";
                cbHomeParamDir.Text = "";
                cbHomeParamEzEnable.Text = "";
                numHomeParamAcc.ResetText();
                numHomeParamVm.ResetText();
                numHomeParamVo.ResetText();
                numHomeParamVa.ResetText();
                numHomeParamShift.ResetText();
                numHomeParamOffset.ResetText();

                cbHomeParamMode.BackColor = Color.Red;
                cbHomeParamDir.BackColor = Color.Red;
                cbHomeParamEzEnable.BackColor = Color.Red;
                numHomeParamAcc.BackColor = Color.Red;
                numHomeParamVm.BackColor = Color.Red;
                numHomeParamVo.BackColor = Color.Red;
                numHomeParamVa.BackColor = Color.Red;
                numHomeParamShift.BackColor = Color.Red;
                numHomeParamOffset.BackColor = Color.Red;


                errInfo +="获取电机归零参数失败，Error:" + motion.GetErrorInfo(err)+"\n";
            }
            else
            {
                if (hp.mode > -1 && hp.mode < 3)//0,1,2
                    cbHomeParamMode.SelectedIndex = hp.mode;
                else
                    cbHomeParamMode.Text = hp.mode.ToString();
                cbHomeParamDir.SelectedIndex = hp.dir?0:1;
                cbHomeParamEzEnable.SelectedIndex = hp.eza?0:1;
                numHomeParamAcc.Value = Convert.ToDecimal(hp.acc);
                numHomeParamVm.Value = Convert.ToDecimal(hp.vm);
                numHomeParamVo.Value = Convert.ToDecimal(hp.vo); 
                numHomeParamVa.Value = Convert.ToDecimal(hp.va);
                numHomeParamShift.Value = Convert.ToDecimal(hp.shift);
                numHomeParamOffset.Value = Convert.ToDecimal(hp.offset);

                cbHomeParamMode.BackColor = Color.White;
                cbHomeParamDir.BackColor = Color.White;
                cbHomeParamEzEnable.BackColor = Color.White;
                numHomeParamAcc.BackColor = Color.White;
                numHomeParamVm.BackColor = Color.White;
                numHomeParamVo.BackColor = Color.White;
                numHomeParamVa.BackColor = Color.White;
                numHomeParamShift.BackColor = Color.White;
                numHomeParamOffset.BackColor = Color.White;
            }
            bool enable = false;
            double pos = 0;
            err = motion.GetSNLimit(axis, out enable, out pos);
            if(0!= err)
            {
                errInfo += "获取电机负限位参数失败，Error:" + motion.GetErrorInfo(err)+ "\n";
                chkEnableSnl.Checked = false;
                numSnl.Enabled = false;
            }
            else
            {
                chkEnableSnl.Checked = enable;
                numSnl.Enabled = enable;
                if (enable)
                    numSnl.Value = Convert.ToDecimal(pos);
                else
                    numSnl.ResetText();

            }

            err = motion.GetSPLimit(axis, out enable, out pos);
            if (0 != err)
            {
                errInfo += "获取电机正限位参数失败，Error:" + motion.GetErrorInfo(err)+"\n";
                chkEnableSpl.Checked = false;
                numSpl.Enabled = false;
            }
            else
            {
                chkEnableSpl.Checked = enable;
                numSpl.Enabled = enable;
                if (enable)
                    numSpl.Value = Convert.ToDecimal(pos);
                else
                    numSpl.ResetText();
            }
            if (!ret)
                errInfo = errInfo.Substring(0, errInfo.Length - 1);
            return ret;
        }

        FormShowLctBuff _fmLctData = new FormShowLctBuff();

        void ReadLctStatus()
        {
            int err = 0;
            bool bVal = false;
            if (!chkEnableLtc.Focused)
            {
                _isLtcEnableSetting = true;
                err = motion.GetLtcEnabled(axis, out bVal);
                if (0 == err && bVal)
                    chkEnableLtc.Checked = true;
                else
                    chkEnableLtc.Checked = false;
                _isLtcEnableSetting = false;
            }
            if(!chkLtcLogic.Focused)
            {
                _isLtcLogicSetting = true;
                err = motion.GetLtcLogic(axis, out bVal);
                if (err == 0 && bVal)
                    chkLtcLogic.Checked = true;
                else
                    chkLtcLogic.Checked = false;
                _isLtcLogicSetting = false;
            }

            double[] da;
            err = motion.GetLtcBuff(axis, out da);
            if (da == null || da.Length == 0)
                lbLtcCount.Text = "0";
            else
                lbLtcCount.Text = da.Length.ToString();
            _fmLctData.SetData(da);

        }


        bool isLoaded = false;
        IJFModule_Motion motion = null;
        int axis = -1;

        /// <summary>
        /// 紧急停止按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEmg_Click(object sender, EventArgs e)
        {
            string errInfo;
            if(!CheckMotionAxis(out errInfo))
            {
                rtbTips.ForeColor = Color.Red;
                ShowError("急停失败:" +errInfo);
                rtbTips.ForeColor = Color.Black;
                return;
            }
            try
            {
                StopRepeat(); //停止往返运动
                int err = motion.StopAxisEmg(axis);
                if (err != 0)
                {
                    rtbTips.ForeColor = Color.Red;
                    ShowError("急停失败:" + motion.GetErrorInfo(err));
                    rtbTips.ForeColor = Color.Black;
                }
                else
                    ShowTip("已急停");
            }
            catch (Exception ex)
            {
                rtbTips.ForeColor = Color.Red;
                ShowError("急停操作异常：" + ex);
                rtbTips.ForeColor = Color.Black;
            }

        }

        /// <summary>
        /// 停止按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStop_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                rtbTips.ForeColor = Color.Red;
                ShowTip("停止失败:"+errInfo);
                rtbTips.ForeColor = Color.Black;
                return;
            }
            try
            {
                StopRepeat();//停止往返运动
                int err = motion.StopAxis(axis);
                if (err != 0)
                {
                    rtbTips.ForeColor = Color.Red;
                    ShowTip("停止操作失败:" + motion.GetErrorInfo(err));
                    rtbTips.ForeColor = Color.Black;
                }
                else
                    ShowTip("已停止");
            }
            catch(Exception ex)
            {
                rtbTips.ForeColor = Color.Red;
                ShowError("停止操作异常：" + ex);
                rtbTips.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// 伺服按钮被按下，根据当前伺服状态做上电/断电的动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSvo_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError(errInfo);
                return;
            }
            bool isSvon = false;
            try
            {
                isSvon = motion.IsSVO(axis);
                int err;
                if(isSvon)
                    err = motion.ServoOff(axis);
                else
                    err = motion.ServoOn(axis);
                if (err != 0)
                {
                    if(isSvon)
                        ShowError("伺服断电失败:" + motion.GetErrorInfo(err));
                    else
                        ShowError("伺服使能失败:" + motion.GetErrorInfo(err));
                }
                else
                {
                    if (isSvon)
                        ShowTip("伺服已断电" );
                    else
                        ShowTip("伺服已使能");
                    btSvo.Text = isSvon ? "伺服of" : "伺服on";
                }
            }
            catch(Exception ex)
            {
                ShowError((isSvon?"伺服断电操作异常:":"伺服使能操作异常:") + ex);
            }
        }

        /// <summary>
        /// 清除报警
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClearAlm_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("清除报警失败:" + errInfo);
                return;
            }
            try
            {
                int err = motion.ClearAlarm(axis);
                if (err != 0)
                    ShowError("清除报警失败:" + motion.GetErrorInfo(err));
                else
                    ShowTip("报警已清除");
            }
            catch (Exception ex)
            {
                ShowError("清除报警操作异常：" + ex);
            }
        }

        /// <summary>
        /// 电机回归原点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btHome_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("电机归零失败:" + errInfo);
                return;
            }
            try
            {
                int err = motion.Home(axis);
                if (err != 0)
                    ShowError("电机归零失败:" + motion.GetErrorInfo(err));
                else
                    ShowTip("电机开始归零");
            }
            catch (Exception ex)
            {
                ShowError("电机归零操作异常：" + ex);
            }
        }

        private void tbCmd_TextChanged(object sender, EventArgs e)
        {
            double cmdPos = 0;
            if (!double.TryParse(tbCmd.Text, out cmdPos))
                tbCmd.BackColor = Color.Red;
            else
                tbCmd.BackColor = Color.White;
        }

        /// <summary>
        /// 设置电机指令位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetCmd_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("设置指令位置失败:" + errInfo);
                return;
            }
            double cmdPos = 0;
            if(!double.TryParse(tbCmd.Text,out cmdPos))
            {
                tbCmd.Focus();
                ShowError("设置指令位置失败:输入参数不是一个数字" );
            }

            try
            {
                int err = motion.SetCmdPos(axis, cmdPos);
                if (err != 0)
                    ShowError("设置指令位置失败:" + motion.GetErrorInfo(err));
                else
                    ShowTip("设置指令位置:" + tbCmd.Text + " 成功");
            }
            catch (Exception ex)
            {
                ShowError("设置指令位置操作异常：" + ex);
            }
        }

        private void tbFbk_TextChanged(object sender, EventArgs e)
        {
            double fbkPos = 0;
            if (!double.TryParse(tbFbk.Text, out fbkPos))
                tbFbk.BackColor = Color.Red;
            else
                tbFbk.BackColor = Color.White;
        }

        /// <summary>
        /// 设置反馈位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetFbk_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("设置反馈位置失败:" + errInfo);
                return;
            }
            double fbkPos = 0;
            if (!double.TryParse(tbFbk.Text, out fbkPos))
            {
                tbFbk.Focus();
                ShowError("设置反馈位置失败:输入参数不是一个数字");
            }

            try
            {
                int err = motion.SetFbkPos(axis, fbkPos);
                if (err != 0)
                    ShowError("设置反馈位置失败:" + motion.GetErrorInfo(err));
                else
                    ShowTip("设置反馈位置:" + tbFbk.Text + " 成功");
            }
            catch (Exception ex)
            {
                ShowError("设置反馈位置操作异常：" + ex);
            }
        }

        /// <summary>
        /// 位置运动模式发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPtMode_CheckedChanged(object sender, EventArgs e)
        {
            if(chkPtMode.Checked)//相对运动模式
            {
                btPtMove1.Text = "负向";
                btPtMove2.Text = "正向";
                btPtMove2.Visible = true;
            }
            else//绝对运动模式
            {
                btPtMove1.Text = "移动";
                btPtMove2.Text = "停止";
                btPtMove2.Visible = false;
            }
        }


        /// <summary>
        /// 位置运动 按钮1被按下 ， 如果是绝对模式，开始运动 ，如果是相对模式，往负方向运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPtMove1_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError(chkPtMode.Checked ? "相对" : "绝对" + "移动失败:" + errInfo);
                return;
            }
            try
            {

                double dis = Convert.ToDouble(numPtMoveParam.Value);
                int err = 0;
                if(chkPtMode.Checked)
                    err = motion.RelMove(axis, -dis); 
                else
                    err = motion.AbsMove(axis, dis);
                if(err !=0)
                    ShowError(chkPtMode.Checked ? "相对" : "绝对" + "位置移动失败:" + motion.GetErrorInfo(err));
                else
                    ShowTip("开始" + (chkPtMode.Checked ? "相对" : "绝对" + "位置移动到") + dis.ToString() );
            }
            catch (Exception ex)
            {
                ShowError(chkPtMode.Checked ? "相对" : "绝对" + "位置移动异常:" + ex);
            }

        }


        /// <summary>
        /// 位置运动 按钮2被按下 ， 如果是绝对模式，停止运动 ，如果是相对模式，往正方向运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPtMove2_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError(chkPtMode.Checked ? "相对移动失败" : "停止绝对移动失败:" + errInfo);
                return;
            }
            try
            {
                int err = 0;
                if (chkPtMode.Checked)
                {
                    double dis = Convert.ToDouble(numPtMoveParam.Value);
                    err = motion.RelMove(axis, dis);
                }
                else
                    err = motion.StopAxis(axis);
                if (err != 0)
                    ShowError(chkPtMode.Checked ? "相对位置移动失败:" : "停止轴移动" + motion.GetErrorInfo(err));
                else
                    ShowTip(chkPtMode.Checked ? "开始相对位置移动到" + numPtMoveParam.Value.ToString() :"轴已停止" );
            }
            catch (Exception ex)
            {
                ShowError(chkPtMode.Checked ? "相对位置移动异常" : "停止绝对移动异常:" + ex);
            }
        }

        private void chkSpdMode_CheckedChanged(object sender, EventArgs e)
        {
            if(chkSpdMode.Checked) //Jog模式
            {
                numSpdMoveParam.Enabled = false;
            }
            else //Speed运动模式
            {
                numSpdMoveParam.Enabled = true;
            }
        }

        /// <summary>
        /// 速度模式，负方向运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSpdMoveN_MouseDown(object sender, MouseEventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError(chkSpdMode.Checked?"Jog":"速度"+"模式负向移动失败:" + errInfo);
                return;
            }
            try
            {
                int err = 0;
                if (chkSpdMode.Checked)
                    err = motion.Jog(axis,false);
                else
                {
                    err = motion.VelMove(axis, Convert.ToDouble(numSpdMoveParam.Value),false);
                }
                if (err != 0)
                    ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "模式负向移动失败:" + motion.GetErrorInfo(err));
                else
                    ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "模式负向移动开始");
            }
            catch(Exception ex)
            {
                ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "模式负向移动异常:" + ex);
            }



        }
        /// <summary>
        /// 速度模式，停止负方向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSpdMoveN_MouseUp(object sender, MouseEventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                rtbTips.ForeColor = Color.Red;
                ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "负向 KeyUp Err:" + errInfo);
                rtbTips.ForeColor = Color.Black;
                return;
            }
            try
            {
                int err = motion.StopAxis(axis);
                if(err !=0)
                    ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "负向 KeyUp Err:" + motion.GetErrorInfo(err));
                else
                    ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "负向 KeyUp ->轴已停止" );

            }
            catch(Exception ex)
            {
                ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "负向 KeyUp 异常:" + ex);

            }
        }

        /// <summary>
        /// 速度模式，开始正方向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSpdMoveP_MouseDown(object sender, MouseEventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "模式正向移动失败:" + errInfo);
                return;
            }
            try
            {
                int err = 0;
                if (chkSpdMode.Checked)
                    err = motion.Jog(axis, true);
                else
                {
                    err = motion.VelMove(axis, Convert.ToDouble(numSpdMoveParam.Value), true);
                }
                if (err != 0)
                    ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "模式正向移动失败:" + motion.GetErrorInfo(err));
                else
                    ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "模式正向移动开始");
            }
            catch (Exception ex)
            {
                ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "模式负向移动异常:" + ex);
            }
        }

        /// <summary>
        /// 速度模式，停止正方向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSpdMoveP_MouseUp(object sender, MouseEventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                rtbTips.ForeColor = Color.Red;
                ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "正向 KeyUp Err:" + errInfo);
                rtbTips.ForeColor = Color.Black;
                return;
            }
            try
            {
                int err = motion.StopAxis(axis);
                if (err != 0)
                    ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "正向 KeyUp Err:" + motion.GetErrorInfo(err));
                else
                    ShowTip(chkSpdMode.Checked ? "Jog" : "速度" + "正向 KeyUp ->轴已停止");

            }
            catch (Exception ex)
            {
                ShowError(chkSpdMode.Checked ? "Jog" : "速度" + "正向 KeyUp 异常:" + ex);

            }
        }

        /// <summary>
        /// 从控制器读出轴的运动参数并显示在界面上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReadAxisMoveParam_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("从控制器读取运动参数失败：" + errInfo);
                return;
            }
            try
            {
                numMotionParamAcc.BackColor = Color.White;
                numMotionParamDec.BackColor = Color.White;
                numMotionParamVs.BackColor = Color.White;
                numMotionParamVm.BackColor = Color.White;
                numMotionParamVe.BackColor = Color.White;
                numMotionParamCv.BackColor = Color.White;
                numMotionParamJk.BackColor = Color.White;
                JFMotionParam mp;
                int err = motion.GetMotionParam(axis, out mp);
                if (err != 0)
                {
                    ShowError("读取运动参数失败：" + motion.GetErrorInfo(err));
                    return;
                }
                numMotionParamAcc.Value = Convert.ToDecimal(mp.acc);
                numMotionParamDec.Value = Convert.ToDecimal(mp.dec);
                numMotionParamVs.Value = Convert.ToDecimal(mp.vs);
                numMotionParamVm.Value = Convert.ToDecimal(mp.vm);
                numMotionParamVe.Value = Convert.ToDecimal(mp.ve);
                numMotionParamCv.Value = Convert.ToDecimal(mp.curve);
                numMotionParamJk.Value = Convert.ToDecimal(mp.jerk);
                ShowTip("读取运动参数成功");

            }
            catch(Exception ex)
            {
                ShowError("读取运动参数异常：" + ex);
            }
        }

        /// <summary>
        /// 将界面上的轴运动参数写入控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btWriteAxisMoveParam_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("写入运动参数到控制器失败：" + errInfo);
                return;
            }
            try
            {
                

                JFMotionParam mp = new JFMotionParam();
                mp.acc  = Convert.ToDouble(numMotionParamAcc.Value);
                mp.dec  = Convert.ToDouble(numMotionParamDec.Value);
                mp.vs  = Convert.ToDouble(numMotionParamVs.Value);
                mp.vm = Convert.ToDouble(numMotionParamVm.Value);
                mp.ve = Convert.ToDouble(numMotionParamVe.Value);
                mp.curve = Convert.ToDouble(numMotionParamCv.Value);
                mp.jerk = Convert.ToDouble(numMotionParamJk.Value);
                int err = motion.SetMotionParam(axis, mp);
                if(err != 0)
                {
                    numMotionParamAcc.BackColor = Color.Red;
                    numMotionParamDec.BackColor = Color.Red;
                    numMotionParamVs.BackColor = Color.Red;
                    numMotionParamVm.BackColor = Color.Red;
                    numMotionParamVe.BackColor = Color.Red;
                    numMotionParamCv.BackColor = Color.Red;
                    numMotionParamJk.BackColor = Color.Red;
                    ShowError("写入运动参数到控制器失败：" + motion.GetErrorInfo(err));
                }
                else
                {
                    numMotionParamAcc.BackColor = Color.White;
                    numMotionParamDec.BackColor = Color.White;
                    numMotionParamVs.BackColor = Color.White;
                    numMotionParamVm.BackColor = Color.White;
                    numMotionParamVe.BackColor = Color.White;
                    numMotionParamCv.BackColor = Color.White;
                    numMotionParamJk.BackColor = Color.White;
                    ShowTip("写入运动参数到控制器成功");
                }
            }
            catch(Exception ex)
            {
                numMotionParamAcc.BackColor = Color.Red;
                numMotionParamDec.BackColor = Color.Red;
                numMotionParamVs.BackColor = Color.Red;
                numMotionParamVm.BackColor = Color.Red;
                numMotionParamVe.BackColor = Color.Red;
                numMotionParamCv.BackColor = Color.Red;
                numMotionParamJk.BackColor = Color.Red;
                ShowError("写入运动参数到控制器异常：" + ex);
            }

        }


        void StopRepeat()
        {
            

            if (!IsRepeating)
                return;

            IsRepeating = false;
            if(!threadRepeat.Join(2000))
            {
                threadRepeat.Abort();
                try
                {
                    motion.StopAxisEmg(axis);
                }
                catch(Exception ex)
                {
                    ShowError("停止往复运动->StopAxisEmg 发生异常:" + ex);
                }
            }
            threadRepeat = null;

            lampRepeating.IsTurnOn = false;
            lampRepeating.IOName = "已停止";
            numRepeatCount.Enabled = true;
            numRepeatIntervalSec.Enabled = true;
            numRepeatPos1.Enabled = true;
            numRepeatPos2.Enabled = true;
            btStartRepeat.Enabled = true;
            btStopRepeat.Enabled = false;

        }
        /// <summary>
        /// 开始往复模式运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStopRepeat_Click(object sender, EventArgs e)
        {
            if (!IsRepeating)
                return;
            StopRepeat();
            ShowTip("往复运动已停止！");
        }

        void RepeatExit()
        {
            lampRepeating.IsTurnOn = false;
            lampRepeating.IOName = "已停止";
            numRepeatCount.Enabled = true;
            numRepeatIntervalSec.Enabled = true;
            numRepeatPos1.Enabled = true;
            numRepeatPos2.Enabled = true;
            btStartRepeat.Enabled = true;
            btStopRepeat.Enabled = false;

        }

        Thread threadRepeat = null;
        bool isGotoPos1 = true;
        int repeatedCount = 0; //已经完成的往复次数

        double repeatPos1 = 0, repeatPos2 = 0;
        double repeatIntervalSec = 0;
        int repeatTotals = 0; //需要完成的往复次数
        void RepeatLoop()
        {
            isGotoPos1 = true;
            repeatedCount = 0;
            int err = 0;
            double fbkPos = 0;
            try
            {
                err = motion.GetFbkPos(axis, out fbkPos);
                if (0 != err)
                {
                    ShowTip(string.Format("往复运动失败: 当前次数 ={0},获取轴位置失败：{1}", repeatedCount + 1,motion.GetErrorInfo(err)));
                    goto exit_loop;
                }
                if (fbkPos == repeatPos1)
                    err = motion.AbsMove(axis, repeatPos2);
                else
                {
                    isGotoPos1 = false;
                    err = motion.AbsMove(axis, repeatPos1);
                }
                if (0 != err)
                {
                    IsRepeating = false;
                    ShowTip(string.Format("往复运动失败:当前次数={0},目标：位置{1}", repeatedCount + 1,isGotoPos1?"1":"2"));
                    goto exit_loop;
                }
                ShowTip(string.Format("运动: 当前次数 ={0},位置 ={1} 开始...", repeatedCount + 1, isGotoPos1 ? "1" : "2"));

                while (IsRepeating)
                {
                    if(!motion.IsSVO(axis))
                    {
                        ShowTip("往复运动中断：伺服已断电");
                        goto exit_loop;
                    }

                    if (motion.IsALM(axis))
                    {
                        ShowTip("往复运动中断：轴已报警");
                        goto exit_loop;
                    }

                    if(motion.IsEMG(axis))
                    {
                        ShowTip("往复运动中断：急停按钮被按下");
                        goto exit_loop;
                    }

                    bool isInp = motion.IsINP(axis);
                    if(isInp)//本次运动完成
                    {
                        if(repeatTotals !=0)
                        {
                            if(repeatedCount >= repeatTotals)
                            {
                                ShowTip("往复运动完成！次数 = " + repeatedCount);
                                goto exit_loop;
                            }
                        }
                        repeatedCount++;
                        ShowTip("往复运动！第" + repeatedCount + "次完成");
                        if (repeatIntervalSec > 0)
                        {
                            ShowTip("暂停间隔时间:" + repeatIntervalSec + "毫秒"); 
                            int sleepMilsec = (int)repeatIntervalSec * 1000;
                            int sleepedMs = 0;
                            while(true)
                            {
                                if(!IsRepeating)
                                    goto exit_loop;
                                Thread.Sleep(200);
                                sleepedMs += 200;
                                if (sleepedMs >= sleepMilsec)
                                    break;
                            }
                        }
                        isGotoPos1 = !isGotoPos1;
                        if (isGotoPos1)
                            err = motion.AbsMove(axis, repeatPos1);
                        else
                            err = motion.AbsMove(axis, repeatPos2);

                        if(0!=err)
                        {
                            ShowTip(string.Format("往复运动失败:当前次数={0},目标：位置{1} error info = {2}", repeatedCount + 1, isGotoPos1 ? "1" : "2",motion.GetErrorInfo(err)));
                            goto exit_loop;
                        }
                        ShowTip(string.Format("运动: 当前次数 ={0},位置 ={1} 开始...", repeatedCount + 1, isGotoPos1 ? "1" : "2"));
                        Thread.Sleep(20);
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                    
                }
            }
            catch(Exception ex)
            {
                ShowError("往复运动发生异常:" + ex);
            }
        exit_loop:
            IsRepeating = false;
            Invoke(new Action(RepeatExit)) ;
            ShowTip("往复线程已停止");
        }

        /// <summary>
        /// 停止往复模式运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStartRepeat_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
                ShowError("启动往复运动失败：" + errInfo);
            if (IsRepeating)
                return;
            try
            {
                if (!motion.IsSVO(axis))
                {
                    ShowError("启动往复运动失败：伺服未上电");
                    return;
                }

                if(motion.IsALM(axis))
                {
                    ShowError("启动往复运动失败：电机已报警");
                    return;
                }

                if(!motion.IsMDN(axis))
                {
                    ShowError("启动往复运动失败：电机当前运动未完成");
                    return;
                }

                if(numRepeatPos1.Value == numRepeatPos2.Value)
                {
                    ShowError("启动往复运动失败：位置1和位置2参数值相同！");
                    return;
                }
                repeatPos1 = Convert.ToDouble(numRepeatPos1.Value);
                repeatPos2 = Convert.ToDouble(numRepeatPos2.Value);
                repeatTotals = Convert.ToInt32(numRepeatCount.Value);
                repeatIntervalSec = Convert.ToDouble(numRepeatIntervalSec.Value);
                isGotoPos1 = true;
                repeatedCount = 0;
                IsRepeating = true;
                lampRepeating.IsTurnOn = true;
                lampRepeating.IOName = "正在运行";

                numRepeatCount.Enabled = false;
                numRepeatIntervalSec.Enabled = false;
                numRepeatPos1.Enabled = false;
                numRepeatPos2.Enabled = false;
                btStartRepeat.Enabled = false;
                btStopRepeat.Enabled = true;


                threadRepeat = new Thread(new ThreadStart(RepeatLoop));
                threadRepeat.Start();
                ShowTip("启动往复运动成功！");
            }
            catch (Exception ex)
            {
                ShowError("启动往复运动异常:" + ex);
            }



        }

        private void UcAxisTest_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                UpdateAllAxisData2UI();
        }


        //bool _isLctPanelShow = false;
        bool _isLtcEnableSetting = false;
        bool _isLtcLogicSetting = false;
        /// <summary>
        /// 显示位置锁存功能界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLtc_Click(object sender, EventArgs e)
        {
            //if(_isLctPanelShow)
            //{

            //}
            if(pnLtc.Visible)
                pnLtc.Visible = false;
            else
                pnLtc.Visible = true;

        }

        private void chkLtcLogic_CheckedChanged(object sender, EventArgs e)
        {
            if (_isLtcLogicSetting)
                return;
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                MessageBox.Show("锁存操作失败:" + errInfo);
                _isLtcLogicSetting = true;
                chkLtcLogic.Checked = !chkLtcLogic.Checked;
                _isLtcLogicSetting = false;
                return;
            }

            int err = motion.SetLtcLogic(axis, chkLtcLogic.Checked);
            if (0 != err)
            {
                MessageBox.Show("锁存操作失败:" + motion.GetErrorInfo(err));
                _isLtcLogicSetting = true;
                chkLtcLogic.Checked = !chkLtcLogic.Checked;
                _isLtcLogicSetting = false;
                return;
            }
            ShowTip((chkLtcLogic.Checked ? "高电平" : "低电平") + "触发锁存设置OK");
        }

        private void btClearLtc_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                MessageBox.Show("清除锁存数据失败:" + errInfo);
            }
            int err = motion.ClearLtcBuff(axis);
            if(0!= err)
            {
                MessageBox.Show("清除锁存数据失败:" + motion.GetErrorInfo(err));
            }
            ShowTip("清除锁存数据成功！");
        }

        private void btShowLtc_Click(object sender, EventArgs e)
        {
            _fmLctData.Show();
        }

        private void chkEnableLtc_CheckedChanged(object sender, EventArgs e)
        {
            if (_isLtcEnableSetting)
                return;
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                MessageBox.Show("锁存操作失败:" + errInfo);
                _isLtcEnableSetting = true;
                chkEnableLtc.Checked = !chkEnableLtc.Checked;
                _isLtcEnableSetting = false;
                return;
            }

            int err = motion.SetLtcEnabled(axis, chkEnableLtc.Checked);
            if(0!= err)
            {
                MessageBox.Show("锁存操作失败:" + motion.GetErrorInfo(err));
                _isLtcEnableSetting = true;
                chkEnableLtc.Checked = !chkEnableLtc.Checked;
                _isLtcEnableSetting = false;
                return;
            }
            ShowTip((chkEnableLtc.Checked ? "使能" : "禁用") + "锁存OK");
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numPulseFactor_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.KeyCode == Keys.Enter)
            //{
            //    double fact = Convert.ToDouble(numPulseFactor.Value);
            //    int errCode = 
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numPulseFactor_ValueChanged(object sender, EventArgs e)
        {
            double fact = Convert.ToDouble(numPulseFactor.Value);
            if(fact == 0 )
            {
                numPulseFactor.BackColor = Color.Red;
                MessageBox.Show("脉冲当量参数错误： 不能为0");
                return;
            }

            int err = motion.SetPulseFactor(axis, fact);
            if(err !=0)
            {
                numPulseFactor.BackColor = Color.Red;
                MessageBox.Show("脉冲当量参数失败：" + motion.GetErrorInfo(err));
                return;
            }
            numPulseFactor.BackColor = Color.White;
            ShowTip("脉冲当量参数设置成功");
        }

        /// <summary>
        /// 从控制器读出轴参数（不包括运动控制参数）并显示到界面上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReadAxisParam_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("从控制器读取轴参数失败：" + errInfo);
                return;
            }
            try
            {


                if (!ReadAxisParam(out errInfo))
                {
                    ShowError("从控制器读取轴参数失败：" + errInfo);
                    return;
                }
                else
                {
                    ShowTip("从控制器读取轴参数成功");
                    return;
                }
            }
            catch(Exception ex)
            {
                ShowError("从控制器读取轴参数失败：" + ex);
            }
        }

        /// <summary>
        /// 将界面上的轴参数（不包括运动参数）写入到控制器中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btWriteAxisParam_Click(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
            {
                ShowError("向控制器写入轴参数失败:" + errInfo);
                return;
            }
            try
            {
                bool isOK = true;
                errInfo = "";
                int errCode = 0;
                double pulseFact = Convert.ToDouble(numPulseFactor.Value);
                if(pulseFact == 0 )
                {
                    isOK = false;
                    errInfo += "Error:脉冲当量参数为0！\n";
                    numPulseFactor.BackColor = Color.Red;
                }
                else
                {
                    double val;
                    errCode = motion.GetPulseFactor(axis, out val);
                    if(errCode != 0)
                    {
                        isOK = false;
                        errInfo += "写入脉冲当量失败！" + motion.GetErrorInfo(errCode) +"\n";
                        numPulseFactor.BackColor = Color.Red;
                    }
                    else
                        numPulseFactor.BackColor = Color.White;
                }

                bool isModeRight = true;
                Color bc = Color.White;
                int mode = 0;// cbHomeParamMode.SelectedIndex;
                if (!int.TryParse(cbHomeParamMode.Text, out mode))
                {
                    if(cbHomeParamMode.SelectedIndex < 0)
                    {
                        isOK = false;
                        errInfo += "写入归零模式失败，请选择预设模式 或 整数 \n"; //"写入电机归零参数失败，模式参数不是整数\n";
                        cbHomeParamMode.BackColor = Color.Red;
                        isModeRight = false;
                    }
                    mode = cbHomeParamMode.SelectedIndex;

                }
                
                if(isModeRight)
                {
                    cbHomeParamMode.BackColor = Color.White;
                    JFHomeParam hp = new JFHomeParam();
                    hp.mode = mode;
                    hp.dir = cbHomeParamDir.SelectedIndex == 0 ? true : false;
                    hp.eza = cbHomeParamEzEnable.SelectedIndex == 0 ? true : false;
                    hp.acc = Convert.ToDouble(numHomeParamAcc.Value);
                    hp.vm = Convert.ToDouble(numHomeParamVm.Value);
                    hp.vo = Convert.ToDouble(numHomeParamVo.Value);
                    hp.va = Convert.ToDouble(numHomeParamVa.Value);
                    hp.shift = Convert.ToDouble(numHomeParamShift.Value);
                    hp.offset = Convert.ToDouble(numHomeParamOffset.Value);

                    
                    errCode = motion.SetHomeParam(axis, hp);
                    if (errCode != 0)
                    {
                        errInfo += "写入电机归零参数失败,Error:" + motion.GetErrorInfo(errCode);
                        bc = Color.Red;
                        isOK = false;
                    }
                    cbHomeParamMode.BackColor = bc;
                    cbHomeParamDir.BackColor = bc;
                    cbHomeParamEzEnable.BackColor = bc;
                    numHomeParamAcc.BackColor = bc;
                    numHomeParamVm.BackColor = bc;
                    numHomeParamVo.BackColor = bc;
                    numHomeParamVa.BackColor = bc;
                    numHomeParamShift.BackColor = bc;
                    numHomeParamOffset.BackColor = bc;
                }
                bc = Color.White;
                errCode = motion.SetSPLimit(axis, chkEnableSpl.Checked, Convert.ToDouble(numSpl.Value));
                if(errCode != 0)
                {
                    isOK = false;
                    errInfo += "写入正软限位参数失败,Error:" + motion.GetErrorInfo(errCode);
                    bc = Color.Red;
                }
                chkEnableSpl.BackColor = bc;
                numSpl.BackColor = bc;


                bc = Color.White;
                errCode = motion.SetSNLimit(axis, chkEnableSnl.Checked, Convert.ToDouble(numSnl.Value));
                if (errCode != 0)
                {
                    isOK = false;
                    errInfo += "写入负软限位参数失败,Error:" + motion.GetErrorInfo(errCode);
                    bc = Color.Red;
                }
                chkEnableSnl.BackColor = bc;
                numSnl.BackColor = bc;

                if (!isOK)
                {
                    errInfo = errInfo.Substring(0, errInfo.Length - 1);
                    ShowError("向控制器写入轴参数失败: " + errInfo);
                }
                else
                    ShowTip("向控制器写入轴参数成功！");

            }
            catch(Exception ex)
            {
                ShowError("向控制器写入轴参数异常:" + ex);
            }

            
        }

        private void btClearTips_Click(object sender, EventArgs e)
        {
            rtbTips.Text = string.Empty;
        }


        /// <summary>
        /// 当鼠标离开轴运动控制参数控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeaveMotionParamCell(object sender, EventArgs e)
        {
            try
            {
                string errInfo;
                if (!CheckMotionAxis(out errInfo))
                    return;
                JFMotionParam mp;
                int err = motion.GetMotionParam(axis, out mp);
                if (err != 0)
                    return;
                if (Convert.ToDouble(numMotionParamAcc.Value) != mp.acc)
                    numMotionParamAcc.BackColor = Color.OrangeRed;
                else
                    numMotionParamAcc.BackColor = Color.White;
                if (Convert.ToDouble(numMotionParamDec.Value) != mp.dec)
                    numMotionParamDec.BackColor = Color.OrangeRed;
                else
                    numMotionParamDec.BackColor = Color.White;

                if (Convert.ToDouble(numMotionParamVs.Value) != mp.vs)
                    numMotionParamVs.BackColor = Color.OrangeRed;
                else
                    numMotionParamVs.BackColor = Color.White;

                if (Convert.ToDouble(numMotionParamVm.Value) != mp.vm)
                    numMotionParamVm.BackColor = Color.OrangeRed;
                else
                    numMotionParamVm.BackColor = Color.White;

                if (Convert.ToDouble(numMotionParamVe.Value) != mp.ve)
                    numMotionParamVe.BackColor = Color.OrangeRed;
                else
                    numMotionParamVe.BackColor = Color.White;

                if (Convert.ToDouble(numMotionParamCv.Value) != mp.curve)
                    numMotionParamCv.BackColor = Color.OrangeRed;
                else
                    numMotionParamCv.BackColor = Color.White;

                if (Convert.ToDouble(numMotionParamJk.Value) != mp.jerk)
                    numMotionParamJk.BackColor = Color.OrangeRed;
                else
                    numMotionParamJk.BackColor = Color.White;


            }
            catch
            {
                return;
            }
        }

        private void chkEnableSpl_CheckedChanged(object sender, EventArgs e)
        {
            numSpl.Enabled = chkEnableSpl.Checked;
        }

        private void chkEnableSnl_CheckedChanged(object sender, EventArgs e)
        {
            numSnl.Enabled = chkEnableSnl.Checked;
        }

        private void numPulseFactor_Leave(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
                return;
            try
            {
                double fct = 0;
                if (0 != motion.GetPulseFactor(axis, out fct))
                    return;
                if (fct != Convert.ToDouble(numPulseFactor.Value))
                    numPulseFactor.BackColor = Color.OrangeRed;
                else
                    numPulseFactor.BackColor = Color.White;
            }
            catch
            {
                numPulseFactor.BackColor = Color.OrangeRed;
                return;
            }
        }

        private void MouseLeaveHomeParamCell(object sender, EventArgs e)
        {
            string errInfo;
            if (!CheckMotionAxis(out errInfo))
                return;
            try
            {
                JFHomeParam hp;
                if(0!= motion.GetHomeParam(axis,out hp))
                {
                    cbHomeParamMode.BackColor = Color.OrangeRed;
                    cbHomeParamDir.BackColor = Color.OrangeRed;
                    cbHomeParamEzEnable.BackColor = Color.OrangeRed;
                    numHomeParamAcc.BackColor = Color.OrangeRed;
                    numHomeParamVm.BackColor = Color.OrangeRed;
                    numHomeParamVo.BackColor = Color.OrangeRed;
                    numHomeParamVa.BackColor = Color.OrangeRed;
                    numHomeParamShift.BackColor = Color.OrangeRed;
                    numHomeParamOffset.BackColor = Color.OrangeRed;
                    return;
                }
                else
                {
                    if (hp.mode.ToString() == cbHomeParamMode.Text)
                        cbHomeParamMode.BackColor = Color.White;
                    else
                        cbHomeParamMode.BackColor = Color.OrangeRed;
                    
                    if(cbHomeParamDir.SelectedIndex < 0)
                        cbHomeParamDir.BackColor = Color.OrangeRed;
                    else
                    {
                        if ((hp.dir && cbHomeParamDir.SelectedIndex == 0)
                            || (!hp.dir && cbHomeParamDir.SelectedIndex == 1))
                            cbHomeParamDir.BackColor = Color.White;
                        else
                            cbHomeParamDir.BackColor = Color.OrangeRed;
                    }


                    if (cbHomeParamEzEnable.SelectedIndex < 0)
                        cbHomeParamEzEnable.BackColor = Color.OrangeRed;
                    else
                    {
                        if ((hp.eza && cbHomeParamEzEnable.SelectedIndex == 0)
                            || (!hp.eza && cbHomeParamEzEnable.SelectedIndex == 1))
                            cbHomeParamEzEnable.BackColor = Color.White;
                        else
                            cbHomeParamEzEnable.BackColor = Color.OrangeRed;
                    }

                    if (hp.acc == Convert.ToDouble(numHomeParamAcc.Value))
                        numHomeParamAcc.BackColor = Color.White;
                    else
                        numHomeParamAcc.BackColor = Color.OrangeRed;

                    if (hp.vm == Convert.ToDouble(numHomeParamVm.Value))
                        numHomeParamVm.BackColor = Color.White;
                    else
                        numHomeParamVm.BackColor = Color.OrangeRed;

                    if (hp.vo == Convert.ToDouble(numHomeParamVo.Value))
                        numHomeParamVo.BackColor = Color.White;
                    else
                        numHomeParamVo.BackColor = Color.OrangeRed;

                    if (hp.va == Convert.ToDouble(numHomeParamVa.Value))
                        numHomeParamVa.BackColor = Color.White;
                    else
                        numHomeParamVa.BackColor = Color.OrangeRed;

                    if (hp.shift == Convert.ToDouble(numHomeParamShift.Value))
                        numHomeParamShift.BackColor = Color.White;
                    else
                        numHomeParamShift.BackColor = Color.OrangeRed;

                    if (hp.offset == Convert.ToDouble(numHomeParamOffset.Value))
                        numHomeParamOffset.BackColor = Color.White;
                    else
                        numHomeParamOffset.BackColor = Color.OrangeRed;
                }
            }
            catch
            {
                cbHomeParamMode.BackColor = Color.OrangeRed;
                cbHomeParamDir.BackColor = Color.OrangeRed;
                cbHomeParamEzEnable.BackColor = Color.OrangeRed;
                numHomeParamAcc.BackColor = Color.OrangeRed;
                numHomeParamVm.BackColor = Color.OrangeRed;
                numHomeParamVo.BackColor = Color.OrangeRed;
                numHomeParamVa.BackColor = Color.OrangeRed;
                numHomeParamShift.BackColor = Color.OrangeRed;
                numHomeParamOffset.BackColor = Color.OrangeRed;
                return;
            }
        }
    }
}
