using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFHub;
using JFToolKits;
using JFUI;
using JFInterfaceDef;

namespace JFApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            IsAutoRunning = true;
        }

        //当前是否处于自动运行模式
        public bool IsAutoRunning { get; private set; } 
        FormAuto _autoForm = null; //
        FormManual _manualForm = null; //
        FormCfg _cfgForm = null; //设置窗口
        FormLog _logForm = null; //日志窗口
        FormBrief _briefForm = null; //机器简介窗口
        FormUser _userForm = null; //用户权限管理窗口
        FormVision _visionForm = null; //视觉调试窗口


        FormStationsRunInfo _sriForm = new FormStationsRunInfo();

        private void FormMain_Load(object sender, EventArgs e)
        {
            ///检查APP运行的条件
            //1.检查主工站是否注册
            //if(JFHubCenter.Instance.StationMgr.MainStation == null)
            //{
            //    MessageBox.Show("Main-Station is not regist ,App will Exit!");
            //    Application.Exit();
            //}

            ////功能窗口初始化
            _autoForm = new FormAuto();
            _autoForm.TopLevel = false;
            _autoForm.Parent = subFormPanel;
            _autoForm.Dock = DockStyle.Fill;

            _manualForm = new FormManual();
            _manualForm.Dock = DockStyle.Fill;
            //_manualForm.Visible = false;
            _manualForm.TopLevel = false;
            _manualForm.Parent = subFormPanel;
            _cfgForm = new FormCfg(); //设置窗口
            _cfgForm.Dock = DockStyle.Fill;
            //_cfgForm.Visible = false;
            _cfgForm.TopLevel = false;
            _cfgForm.Parent = subFormPanel;
            _logForm = new FormLog(); //日志窗口
            _logForm.Dock = DockStyle.Fill;
            _logForm.TopLevel = false;
            //_logForm.Visible = false;
            _logForm.Parent = subFormPanel;
            _briefForm = new FormBrief(); //机器简介窗口
            _briefForm.Dock = DockStyle.Fill;
            _briefForm.TopLevel = false;
            // _briefForm.Visible = false;
            _briefForm.Parent = subFormPanel;
            _userForm = new FormUser(); //用户权限管理窗口
            _userForm.Dock = DockStyle.Fill;
            // _userForm.Visible = false;
            _userForm.TopLevel = false;
            _userForm.Parent = subFormPanel;
            _visionForm = new FormVision(); //视觉调试窗口
            _visionForm.Dock = DockStyle.Fill;
            //_visionForm.Visible = false;
            _visionForm.TopLevel = false;
            _visionForm.Parent = subFormPanel;


            base.OnResize(e);

            //控件根据窗体的大小变化而变化，等比例放大缩小
            this.Resize += new EventHandler(FormResize);
            oldX = this.Width;
            oldY = this.Height;
            SetTag(this);
            FormResize(new object(), new EventArgs());
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;//窗体全屏打开

          
            _autoForm.Show();


            //StationManager 消息挂靠
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            mgr.EventStationWorkStatusChanged += OnStationWorkStatusChanged;
            mgr.EventStationCustomStatusChanged += OnStationCustomStatusChanged;
            mgr.EventStationProductFinished += OnStationProductFinished;

            _sriForm.TopMost = true;
        }



        delegate void dgWorkStatusChanged(IJFWork station, JFWorkStatus currWorkStatus);
        void OnStationWorkStatusChanged(object station, JFWorkStatus currWorkStatus)
        {
            if (InvokeRequired)
            {
                Invoke(new dgWorkStatusChanged(OnStationWorkStatusChanged), new object[] { station, currWorkStatus });
                return;
            }

        }


        delegate void dgCustomStatusChanged(object station, int currCustomStatus);
        void OnStationCustomStatusChanged(object station, int currCustomStatus)
        {
            if (InvokeRequired)
            {
                Invoke(new dgCustomStatusChanged(OnStationCustomStatusChanged), new object[] { station, currCustomStatus });
                return;
            }

        }

        delegate void dgTxtMsg(object station, string txt);
        void OnStationTxtMsg(object station, string txt)
        {
        }


        delegate void dgProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo);
        void OnStationProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new dgProductFinished(OnStationProductFinished), new object[] { station, passCount, passIDs, ngCount, ngIDs, ngInfo });
                return;
            }
            string info = "加工完成:PassCount = " + passCount + ", NGCount = " + ngCount;
            if (null != passIDs && passIDs.Length > 0)
                info += "\n     PassIDs:" + String.Join(",", passIDs);
            if (null != ngIDs && ngIDs.Length > 0)
                info += "\n     NGIDs:" + String.Join(",", ngIDs);
            //ShowTips(info);
        }


        delegate void dgCustomizeMsg(IJFStation station, string msgCategory, object[] msgParams);
        void OnCustomizeMsg(IJFStation station, string msgCategory, object[] msgParams)
        {
            if (InvokeRequired)
            {
                Invoke(new dgCustomizeMsg(OnCustomizeMsg), new object[] { station, msgCategory, msgParams });
                return;
            }
            string info = "专属消息:Category = " + msgCategory + " ParamCount = " + (msgParams == null ? 0 : msgParams.Length);
            //ShowTips(info);
        }




        #region 窗体子控件自动缩放功能
        float oldX = 1;
        float oldY = 1;

        private void SetTag(Control ctrl)
        {
            foreach (Control con in ctrl.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    SetTag(con);
                }
            }
        }

        private void SetControl(float newx, float newy, Control ctrl)
        {
            foreach (Control con in ctrl.Controls)
            {
                if (con.Tag != null)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                    float a = Convert.ToSingle(mytag[0]) * newx;
                    con.Width = (int)a;
                    a = Convert.ToSingle(mytag[1]) * newy;
                    con.Height = (int)a;
                    a = Convert.ToSingle(mytag[2]) * newx;
                    con.Left = (int)a;
                    a = Convert.ToSingle(mytag[3]) * newy;
                    con.Top = (int)a;
                    Single currentsize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                    con.Font = new Font(con.Font.Name, currentsize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                        SetControl(newx, newy, con);
                }
            }
        }

        void FormResize(object sender, EventArgs e)
        {
            float newx = (this.Width) / oldX;
            float newy = (this.Height) / oldY;
            //SetControl(newx, newy, this);
        }


        #endregion

        #region  操作按钮/子窗口

        


        #endregion

        /// <summary>
        /// 清除报警信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAlarm_Click(object sender, EventArgs e) 
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            string errInfo = "";
            bool isOK = ms.ClearAlarming(out errInfo);
            if(!isOK)
            {
                JFTipsDelayClose.Show("清除报警失败:" + errInfo, 20);
                return;
            }
            JFTipsDelayClose.Show("报警已清除:" ,2);

        }

        /// <summary>
        /// 设备运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStart_Click(object sender, EventArgs e)
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            if(ms.WorkStatus == JFWorkStatus.Running)
            {
                JFTipsDelayClose.Show("无效操作:正在运行中", 2);
                return;
            }
            string errorInfo;
            if (!_isStationWorking(ms.WorkStatus))
            {
                ///先将所有使能工站切换为自动模式
                JFStationManager mgr = JFHubCenter.Instance.StationMgr;
                string[] allEnableStationNames = mgr.AllEnabledStationNames();
                if(null != allEnableStationNames)
                    foreach(string sn in allEnableStationNames)
                    {
                        IJFStation station = mgr.GetStation(sn);
                        if(!station.SetRunMode(JFStationRunMode.Auto))
                        {
                            MessageBox.Show("启动运行失败，未能将工站：" + sn + "切换为自动运行模式");
                            return;
                        }
                    }


                //// 添加消息回调
                //string[] allEnableStationNames = JFHubCenter.Instance.StationMgr.AllEnabledStationNames();
                bool isOK = ms.Start(out errorInfo);
                if (!isOK)
                {
                    MessageBox.Show("启动失败:" + errorInfo);
                    return;
                }
                else
                    JFTipsDelayClose.Show("设备开始运行", 1);
            }

            if(ms.WorkStatus == JFWorkStatus.Pausing) //当前处于暂停状态
            {
                bool isOK = ms.Resume(out errorInfo);
                if(!isOK)
                {
                    MessageBox.Show("恢复运行失败:" + errorInfo);
                    return;
                }
                else
                    JFTipsDelayClose.Show("设备开始运行", 1);
            }



        }

        /// <summary>
        /// 设备暂停/恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPause_Click(object sender, EventArgs e)
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            if (!_isStationWorking(ms.WorkStatus))
            {
                JFTipsDelayClose.Show("无效操作:设备未运行", 2);
                return;
            }
            if (ms.WorkStatus == JFWorkStatus.Pausing)
            {
                JFTipsDelayClose.Show("无效操作:设备已暂停运行", 2);
                return;
            }
            string errorInfo = null;
            bool isOK = ms.Pause(out errorInfo);
            if (!isOK)
            {
                JFTipsDelayClose.Show("暂停操作失败:" + errorInfo,2);
                return;
            }
            else
                JFTipsDelayClose.Show("设备已暂停", 2);
            
        }

        /// <summary>
        /// 设备停止 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStop_Click(object sender, EventArgs e)
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            string errInfo = "";
            if(!ms.Stop(out errInfo))
            {
                JFTipsDelayClose.Show("停止操作失败:" + errInfo, 5);
                return;
            }
            //else
            //    JFTipsDelayClose.Show("停止操作OK" + errInfo, 2);
            return;

        }

        /// <summary>
        /// 设备结批
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBatch_Click(object sender, EventArgs e)
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            string errInfo = "";
            if (!ms.EndBatch(out errInfo))
                JFTipsDelayClose.Show("结批操作失败:" + errInfo, 5);
            else
                JFTipsDelayClose.Show("结批指令OK:",2);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUser_Click(object sender, EventArgs e)
        {
            ///显示用户管理界面,待添加权限管理代码 ... 
            JustShowSubForm(_userForm);
        }

        void JustShowSubForm(Form fm)
        {
            Form[] sbfms = new Form[] { _autoForm, _manualForm, _cfgForm, _logForm, _briefForm, _userForm, _visionForm };
            foreach(Form subfm in sbfms)
            {
                if (subfm == fm)
                    subfm.Show();
                else
                    subfm.Hide();
            }
        }

        private void btAuto_Click(object sender, EventArgs e)
        {
            if(IsAutoRunning) //当前处于自动化运行模式
                JustShowSubForm(_autoForm);
            else //非自动运行状态，可能有工站正在运行测试
            {
                JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;
                string[] allEnableStationNames = stationMgr.AllEnabledStationNames();
                if(null == allEnableStationNames || allEnableStationNames.Length == 0 )
                {
                    JustShowSubForm(_autoForm);
                    statusLabelRunMode.Text = "自动运行";
                    return;
                }
                
   
                List<IJFStation> runningStations = new List<IJFStation>();
                foreach(string sn in allEnableStationNames)
                {
                    IJFStation station = stationMgr.GetStation(sn);
                    if (_isStationWorking(station.CurrWorkStatus))
                    runningStations.Add(station);
                        
                }
                
                if(runningStations.Count > 0)
                {
                    StringBuilder sbInfo = new StringBuilder("切换到自动运行模式失败，以下工站未停止：\n");
                    foreach (IJFStation station in runningStations)
                        sbInfo.AppendLine("工站:" + station.Name + " 当前状态:" + station.CurrWorkStatus.ToString());
                    JFTipsDelayClose.Show(sbInfo.ToString(), 5);
                    return;
                }
                IsAutoRunning = true;
                JustShowSubForm(_autoForm);
                statusLabelRunMode.Text = "自动运行";
            }
           
        }

        private void btManual_Click(object sender, EventArgs e)
        {
            if (!IsAutoRunning)
            {
                _manualForm.Show();
                return;
            }
            if(_isStationWorking(JFHubCenter.Instance.StationMgr.MainStation.WorkStatus))
            {
                JFTipsDelayClose.Show("自动运行时不能切换到手动调试界面", 3);
                return;
            }
            IsAutoRunning = false;
            statusLabelRunMode.Text = "手动测试";
            JustShowSubForm(_manualForm);
        }

        private void btBrief_Click(object sender, EventArgs e)
        {
            JustShowSubForm(_briefForm);
        }

        private void btLog_Click(object sender, EventArgs e)
        {
            JustShowSubForm(_logForm);
        }

        bool _isStationWorking(JFWorkStatus status)
        {
            return status == JFWorkStatus.Running || status == JFWorkStatus.Pausing || status == JFWorkStatus.Interactiving;
        }

        private void btCfg_Click(object sender, EventArgs e)
        {
            //if(_isStationWorking(JFHubCenter.Instance.StationMgr.MainStation.WorkStatus)) //当正在自动运行时，可以查看配置，不能修改
            //{
            //    //JFTipsDelayClose.Show("正在运行，不能修改配置")
            //}
            JustShowSubForm(_cfgForm); //
        }

        private void btVision_Click(object sender, EventArgs e)
        {
            if(IsAutoRunning && _isStationWorking(JFHubCenter.Instance.StationMgr.MainStation.WorkStatus))
            {
                JFTipsDelayClose.Show("设备正在自动运行，不能打开视觉调试界面",5);
                return;
            }
            JustShowSubForm(_visionForm);
        }

        JFWorkStatus _lastStatus = JFWorkStatus.UnStart;//上一次更新时的状态
        bool _isLastAlarming = false;

        private void timerFlush_Tick(object sender, EventArgs e) //主界面刷新
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            if(ms.IsAlarming != _isLastAlarming)
            {
                _isLastAlarming = !_isLastAlarming;
                if (_isLastAlarming)
                {
                    btAlarm.Enabled = true;
                    btAlarm.ImageKey = "Alarm_On.png";
                    statusLabelDevStatus.Text = "已报警:" + ms.GetAlarmInfo();
                    BackColor = Color.Red;
                }
                else //报警状态转为正常
                {
                    btAlarm.ImageKey = "Alarm_Off.png";
                    statusLabelDevStatus.Text = "未报警";
                    btAlarm.Enabled = false;
                    _lastStatus = ms.WorkStatus;
                    //添加代码 ... 
                    switch (_lastStatus)
                    {
                        case JFWorkStatus.UnStart:// = 0,    //线程未开始运行
                            BackColor = Color.White;
                            statusLabelDevStatus.Text = "未运行/已停止";
                            break;
                        case JFWorkStatus.Running://,        //线程正在运行，未退出
                            BackColor = Color.Green;
                            statusLabelDevStatus.Text = "运行中...";
                            break;
                        case JFWorkStatus.Pausing://,        //线程暂停中
                            BackColor = Color.Yellow;
                            statusLabelDevStatus.Text = "暂停中...";
                            break;
                        case JFWorkStatus.Interactiving://,  //人机交互 ， 等待人工干预指令
                            BackColor = Color.OrangeRed;
                            statusLabelDevStatus.Text = "等待人工确认中...";
                            break;
                        case JFWorkStatus.NormalEnd://,     //线程正常完成后退出
                            BackColor = Color.White;
                            statusLabelDevStatus.Text = "已停止/正常结束";
                            break;
                        case JFWorkStatus.CommandExit://,    //收到退出指令
                            BackColor = Color.White;
                            statusLabelDevStatus.Text = "已停止/指令结束";
                            break;
                        case JFWorkStatus.ErrorExit://,      //发生错误退出，（重启或人工消除错误后可恢复）
                            BackColor = Color.DarkRed;
                            statusLabelDevStatus.Text = "已停止/发生错误";
                            break;
                        case JFWorkStatus.ExceptionExit://,  //发生异常退出 ,  (不可恢复的错误)
                            BackColor = Color.DarkRed;
                            statusLabelDevStatus.Text = "已停止/发生异常";
                            break;
                        case JFWorkStatus.AbortExit://,      //由调用者强制退出
                            BackColor = Color.DarkRed;
                            statusLabelDevStatus.Text = "已停止/指令强制";
                            break;
                        default:
                            //statusLabelDevStatus.Text = "状态未知:" + ws.ToString();
                            break;
                    }
                }

            }

            if (_isLastAlarming)
                return;
            JFWorkStatus currWs = ms.WorkStatus;
            if (currWs == _lastStatus)
                return;
            _lastStatus = currWs;
            switch (_lastStatus)
            {
                case JFWorkStatus.UnStart:// = 0,    //线程未开始运行
                    BackColor = Color.White;
                    statusLabelDevStatus.Text = "未运行/已停止";
                    break;
                case JFWorkStatus.Running://,        //线程正在运行，未退出
                    BackColor = Color.Green;
                    statusLabelDevStatus.Text = "运行中...";
                    break;
                case JFWorkStatus.Pausing://,        //线程暂停中
                    BackColor = Color.Yellow;
                    statusLabelDevStatus.Text = "暂停中...";
                    break;
                case JFWorkStatus.Interactiving://,  //人机交互 ， 等待人工干预指令
                    BackColor = Color.OrangeRed;
                    statusLabelDevStatus.Text = "等待人工确认中...";
                    break;
                case JFWorkStatus.NormalEnd://,     //线程正常完成后退出
                    BackColor = Color.White;
                    statusLabelDevStatus.Text = "已停止/正常结束";
                    break;
                case JFWorkStatus.CommandExit://,    //收到退出指令
                    BackColor = Color.White;
                    statusLabelDevStatus.Text = "已停止/指令结束";
                    break;
                case JFWorkStatus.ErrorExit://,      //发生错误退出，（重启或人工消除错误后可恢复）
                    BackColor = Color.DarkRed;
                    statusLabelDevStatus.Text = "已停止/发生错误";
                    break;
                case JFWorkStatus.ExceptionExit://,  //发生异常退出 ,  (不可恢复的错误)
                    BackColor = Color.DarkRed;
                    statusLabelDevStatus.Text = "已停止/发生异常";
                    break;
                case JFWorkStatus.AbortExit://,      //由调用者强制退出
                    BackColor = Color.DarkRed;
                    statusLabelDevStatus.Text = "已停止/指令强制";
                    break;
                default:
                    //statusLabelDevStatus.Text = "状态未知:" + ws.ToString();
                    break;
            }


            

            

          

        }

        /// <summary>
        /// 复位按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReset_Click(object sender, EventArgs e)
        {
            IJFMainStation ms = JFHubCenter.Instance.StationMgr.MainStation;
            string errInfo = "";
            if (!ms.Reset(out errInfo))
                JFTipsDelayClose.Show("归零操作失败:" + errInfo, 5);
            else
                JFTipsDelayClose.Show("归零指令OK:", 2);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            JFWorkStatus ws = JFHubCenter.Instance.StationMgr.MainStation.WorkStatus;
            if(ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving)
            {
                if (DialogResult.OK == MessageBox.Show("设备正在运行，是否停止并退出？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    string errorInfo;
                    if (!JFHubCenter.Instance.StationMgr.MainStation.Stop(out errorInfo))
                        MessageBox.Show("停止设备发生错误:" + errorInfo); //日后在此处添加日志
                    JFHubCenter.Instance.StationMgr.Stop();//停止所有工站（比如正在单站测试中）
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (DialogResult.OK == MessageBox.Show("是否退出程序？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                JFHubCenter.Instance.StationMgr.Stop();
                e.Cancel = false;
            }
            else
                e.Cancel = true;

        }


        /// <summary>
        /// 显示工站运行（消息）信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStationRunInfo_Click(object sender, EventArgs e)
        {
            if (_sriForm.Visible)
                _sriForm.Hide();
            else
                _sriForm.Show();
        }
    }
}
