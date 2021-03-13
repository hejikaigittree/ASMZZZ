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
    public partial class UcMotion : JFRealtimeUI
    {
        public UcMotion()
        {
            InitializeComponent();
        }

        private void UcMotion_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            ucLineParam.SetParamDesribe(JFParamDescribe.Create("运动参数", typeof(JFMotionParam), JFValueLimit.NonLimit, null));
            //JFMotionParam mp = new JFMotionParam() { vs = 0, vm = 0,ve = 0, acc =0, dec =0, curve  =0, jerk  = 0};
            //ucLineParam.SetParamValue(mp);
            ucArcParam.SetParamDesribe(JFParamDescribe.Create("运动参数", typeof(JFMotionParam), JFValueLimit.NonLimit, null));
            //ucArcParam.SetParamValue(mp);
            AdjustUI();
        }

        bool _isFormLoaded = false;
        IJFModule_Motion _module = null;
        string[] _axisIDs = null;

        List<UcAxisStatus> _lstStatus = new List<UcAxisStatus>(); //轴状态面板
        List<UcAxisTest> _lstTest = new List<UcAxisTest>(); //轴测试面板
        List<ComboBox> _lstMode = new List<ComboBox>(); //每个轴的模式（速度运动/PTP运动）
        List<Button> _lstCfg = new List<Button>(); //显示每个轴的全功能测试界面
        public void SetModuleInfo(IJFModule_Motion module,string[] axisIDs)
        {
            _module = module;
            _axisIDs = axisIDs;
            if (_isFormLoaded)
                AdjustUI();
        }

        /// <summary> JFRealtimeUI 's API</summary>
        public override void UpdateSrc2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            UpdateModuleStatus();
        }

        void AdjustUI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustUI));
                return;
            }
            //Controls.Clear();
            _lstStatus.Clear();
            _lstTest.Clear();
            _lstMode.Clear();
            _lstCfg.Clear();
            if (null == _module)
            {
                gbLiner.Enabled = false;
                gbArc.Enabled = false;
                return;
            }
            gbLiner.Enabled = true;
            gbArc.Enabled = true;
            int locX = 2;
            int locY = 0;
            for(int i = 0; i < _module.AxisCount;i++)
            {
                GroupBox gb = new GroupBox();//835,68
                gb.Location = new Point(locX, locY);
                gb.Size = new Size(860, 68);
                //Controls.Add(gb);
                pnAxes.Controls.Add(gb);
                UcAxisStatus axisStatus = new UcAxisStatus();
                axisStatus.DisplayMode = UcAxisStatus.JFDisplayMode.full;
                axisStatus.Location = new Point(1, 15);
                axisStatus.SetAxis(_module, i);
                gb.Text = (_axisIDs != null && _axisIDs.Length > i) ? _axisIDs[i] : ("轴序号_" + i.ToString("D2"));
                UcAxisTest axisTest = new UcAxisTest();
                axisTest.SetAxis(_module, i);
                axisTest.Location = new Point(axisStatus.Right, 15);
                axisTest.DisplayMode = UcAxisTest.JFDisplayMode.simplest_pos;
                ComboBox cbMode = new ComboBox();
                cbMode.Items.AddRange(new string[] { "位置模式", "速度模式" });
                cbMode.SelectedIndex = 0;
                cbMode.Location = new Point(774, 17);
                cbMode.Width = 73;
                cbMode.SelectedIndexChanged += OnCbSelectChanged;
                Button btCfg = new Button();
                btCfg.Location = new Point(774, 38);
                btCfg.Text = "配置";
                btCfg.Width = 73;
                btCfg.Click += OnBtClick;
                _lstStatus.Add(axisStatus);
                _lstTest.Add(axisTest);
                _lstMode.Add(cbMode);
                _lstCfg.Add(btCfg);
                gb.Controls.AddRange(new Control[] { axisStatus, axisTest, cbMode, btCfg });
                locY = gb.Bottom;
            }

            UpdateModuleStatus();
        }

        void UpdateModuleStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            if (null == _module)
                return;
            if (tabCtrl.SelectedIndex == 0) //更新单轴控制界面
                for (int i = 0; i < _module.AxisCount; i++)
                {
                    _lstStatus[i].UpdateAxisStatus();
                    _lstTest[i].UpdateAxisUI();
                }
            else //更新插补测试界面
            {
                if (dgvLinePos.Rows.Count > 0) //更新直线查补控件
                    for (int i = 0; i < dgvLinePos.Rows.Count; i++)
                    {
                        DataGridViewRow row = dgvLinePos.Rows[i];
                        string axisIDTxt = row.Cells[0].Value as string;
                        if (!string.IsNullOrEmpty(axisIDTxt))
                        {
                            int axisID = Convert.ToInt32(axisIDTxt);
                            if(axisID <= _module.AxisCount)
                            {
                                double currPos = 0;
                                int err = _module.GetFbkPos(axisID, out currPos);
                                if (err != 0)
                                    row.Cells[2].Value = "获取失败";
                                else
                                    row.Cells[2].Value = currPos.ToString("F3");
                                if (_module.IsMDN(axisID))
                                    lampLineDone.LampColor = LampButton.LColor.Green;
                                else
                                    lampLineDone.LampColor = LampButton.LColor.Gray;
                            }
                            else
                                row.Cells[2].Value = "轴序号错误";
                            
                        }
                        else
                            row.Cells[2].Value = "";
                    }
            }
        }

        /// <summary>
        /// 轴运动模式改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCbSelectChanged(object sender,EventArgs e)
        {
            for(int i=  0;i<_lstMode.Count;i++)
                if(sender == _lstMode[i])
                {
                    if (_lstMode[i].SelectedIndex == 0)//位置运动模式
                        _lstTest[i].DisplayMode = UcAxisTest.JFDisplayMode.simplest_pos;
                    else//速度运动模式
                        _lstTest[i].DisplayMode = UcAxisTest.JFDisplayMode.simplest_vel;
                }
        }

        /// <summary>
        /// 显示轴参数调试界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBtClick(object sender, EventArgs e)
        {
            for(int i = 0; i < _lstCfg.Count;i++)
                if(sender == _lstCfg[i])
                {
                    FormAxisTest fm = new FormAxisTest();
                    string axisID = ((_axisIDs != null && _axisIDs.Count() > i) ? _axisIDs[i] : "轴_" + i.ToString()) + "测试";
                    fm.SetAxisInfo(_module, i, axisID);
                    fm.ShowDialog();
                }
        }



        int _maxTips = 500;
        void ShowTips(string info)
        {

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


        bool _isSettingNumLineAxisCount = false;
        /// <summary>
        /// 更改直线插补轴数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numLineAxisCount_ValueChanged(object sender, EventArgs e)
        {
            if (_isSettingNumLineAxisCount)
                return;
            if (_module == null)
            {
                _isSettingNumLineAxisCount = true;
                MessageBox.Show("无效操作：轴模块未设置");
                numLineAxisCount.Value = 0;
                _isSettingNumLineAxisCount = false;
            }

            if(_module.AxisCount == 0)
            {
                _isSettingNumLineAxisCount = true;
                MessageBox.Show("无效操作：模块中轴数量为0");
                numLineAxisCount.Value = 0;
                _isSettingNumLineAxisCount = false;
            }


            int nVal = Convert.ToInt32(numLineAxisCount.Value);

            if(nVal > _module.AxisCount)
            {
                _isSettingNumLineAxisCount = true;
                MessageBox.Show("无效操作：插补轴数量不能超过模块轴数:" + _module.AxisCount);
                numLineAxisCount.Value = _module.AxisCount;
                _isSettingNumLineAxisCount = false;
            }
            bool enabled = nVal != 0;
            ucLineParam.IsValueReadOnly = !enabled;
            chkAbsLine.Enabled = enabled;
            dgvLinePos.Enabled = enabled;
            btStartLiner.Enabled = enabled;
            btStopLiner.Enabled = enabled;
            btLineServOn.Enabled = enabled;
            if (nVal == 0)
            {
                dgvLinePos.Rows.Clear();
                return;
            }
            else
            {
                if(nVal == 1 && dgvLinePos.Rows.Count == 0) //第一次添加轴，将第0轴运动参数作为默认参数
                {
                    JFMotionParam mp;
                    int err = _module.GetMotionParam(0, out mp);
                    if (0 == err )
                    {
                        //object objVal;
                        //if(ucLineParam.GetParamValue(out objVal))
                        {
                            //if (null == objVal)
                                ucLineParam.SetParamValue(mp);
                        }
                    }

                }
            }
            if(nVal < dgvLinePos.Rows.Count) //删除一个点位
            {
                dgvLinePos.Rows.RemoveAt(dgvLinePos.Rows.Count - 1);
                return;
            }
            else //添加一个点位
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewComboBoxCell cellIndex = new DataGridViewComboBoxCell();
                for (int i = 0; i < _module.AxisCount; i++)
                    cellIndex.Items.Add(i.ToString());
                row.Cells.Add(cellIndex);
                DataGridViewTextBoxCell cellTrgtPos = new DataGridViewTextBoxCell();
                row.Cells.Add(cellTrgtPos);
                DataGridViewTextBoxCell cellCurrPos = new DataGridViewTextBoxCell();
                cellCurrPos.ReadOnly = true;
                row.Cells.Add(cellCurrPos);
                dgvLinePos.Rows.Add(row);
            }

        }

        /// <summary>
        /// 检查轴是否满足运动条件
        /// </summary>
        /// <param name="axisID"></param>
        /// <returns></returns>
        bool _IsAxisCanMove(int axisID)
        {
            if(null == _module)
            {
                MessageBox.Show("操作失败,运动模块未设置/空值");
                return false;
            }
            if(!_module.IsOpen)
            {
                MessageBox.Show("操作失败,运动模块未开启");
                return false;
            }
            if(axisID >= _module.AxisCount)
            {
                MessageBox.Show("操作失败,轴序号 = " + axisID + " 超出限制:0~" + (_module.AxisCount-1));
                return false;
            }
            bool[] motionStatus;
            int err = _module.GetMotionStatus(axisID, out motionStatus);
            if(err != 0)
            {
                MessageBox.Show("操作失败,轴序号:" + axisID + " 状态未知!");
                return false;
            }

            if(!motionStatus[_module.MSID_SVO])
            {
                MessageBox.Show("操作失败,轴序号:" + axisID + " 伺服未使能!");
                return false;
            }

            if (motionStatus[_module.MSID_ALM])
            {
                MessageBox.Show("操作失败,轴序号:" + axisID + " 伺服已报警!");
                return false;
            }

            if(_module.MSID_MDN >=0)
            if (!motionStatus[_module.MSID_MDN])
            {
                MessageBox.Show("操作失败,轴序号:" + axisID + " 运动未完成!");
                return false;
            }

            return true;
        }

        bool _isMotionParamValid(JFMotionParam mp)
        {
            if(mp.acc <= 0)
            {
                MessageBox.Show("无效操作:加速度参数acc <=0");
                return false;
            }

            if (mp.dec <= 0)
            {
                MessageBox.Show("无效操作:减速度参数dec <=0");
                return false;
            }

            if(mp.vs < 0)
            {
                MessageBox.Show("无效操作:初始速度参数vs <0");
                return false;
            }

            if (mp.vm <= 0)
            {
                MessageBox.Show("无效操作:工作速度参数vm <=0");
                return false;
            }

            if (mp.ve < 0)
            {
                MessageBox.Show("无效操作:终点速度参数ve <0");
                return false;
            }

            if(mp.curve < 0 || mp.curve > 1)
            {
                MessageBox.Show("无效操作:加速取消参数 curve 不在允许范围0~1");
                return false;
            }

            return true;
        }

       

        /// <summary>
        /// 开始直线插补运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStartLiner_Click(object sender, EventArgs e)
        {
            if(dgvLinePos.Rows.Count ==0)
            {
                MessageBox.Show("请设置差补运动的轴数量");
                return;
            }
            List<int> axisIDs = new List<int>();
            List<double> trgtPoses = new List<double>();
            for(int i = 0; i < dgvLinePos.Rows.Count;i++)
            {
                string axisIDText = dgvLinePos.Rows[i].Cells[0].Value as string;
                if(string.IsNullOrEmpty(axisIDText))
                {
                    MessageBox.Show("请设置差补轴" + (i + 1) + " 的轴序号");
                    return;
                }
                int axisID = Convert.ToInt32(axisIDText);
                axisIDs.Add(axisID);
                string trgtPos = dgvLinePos.Rows[i].Cells[1].Value as string;
                if(string.IsNullOrEmpty(trgtPos))
                {
                    MessageBox.Show("请设置差补轴" + (i + 1) + " 的目标位置");
                    return;
                }
                double dVal = 0;
                if(!double.TryParse(trgtPos,out dVal))
                {
                    MessageBox.Show("差补轴" + (i + 1) + " 的目标位置不是一个数字，请重新输入");
                    return;
                }
                if (!_IsAxisCanMove(axisID))
                    return;
                trgtPoses.Add(dVal);
            }

            if(dgvLinePos.Rows.Count ==0)
            {
                MessageBox.Show("请设置差补运动的轴数量");
                return;
            }
            object omp = null;
            if(!ucLineParam.GetParamValue(out omp))
            {
                MessageBox.Show("未能获取运动参数，请检查输入");
                return;
            }
            JFMotionParam mp = (JFMotionParam)omp;
            if (!_isMotionParamValid(mp))
                return;
            int err = 0;
            if (chkAbsLine.Checked)
                err = _module.AbsLine_P(axisIDs.ToArray(), trgtPoses.ToArray(), mp);
            else
                err = _module.RelLine_P(axisIDs.ToArray(), trgtPoses.ToArray(), mp);

            if (err != 0)
                MessageBox.Show("操作失败:" + _module.GetErrorInfo(err));
            else
                ShowTips("开始插补运动");

        }

        /// <summary>
        /// 停止直线插补运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStopLiner_Click(object sender, EventArgs e)
        {
            if (_module == null)
            {
                ShowTips("无效操作，模块未设置");
                return;
            }
            if (!_module.IsOpen)
            {
                ShowTips("无效操作，模块未打开");
                return;
            }
            if (dgvLinePos.Rows.Count == 0)
            {
                MessageBox.Show("无效操作:没有设置插补轴");
                return;
            }
            List<int> axisIDs = new List<int>();
            foreach (DataGridViewRow row in dgvLinePos.Rows)
            {
                string axisIDTxt = row.Cells[0].Value as string;

                if (string.IsNullOrEmpty(axisIDTxt))
                {
                    MessageBox.Show("操作失败:存在未设置的轴序号");
                    return;
                }
                axisIDs.Add(Convert.ToInt32(axisIDTxt));

            }
            bool isSuccess = true;
            StringBuilder sbErr = new StringBuilder();
            foreach (int axisID in axisIDs)
            {
                int err = _module.StopAxis(axisID);
                if (err != 0)
                {
                    sbErr.Append("轴" + axisID + " 停止失败，ErrorInfo:" + _module.GetErrorInfo(err) + "\n");
                    isSuccess = false;
                    break;
                }
                else
                {
                    //ShowTips("已停止直线插补");
                    //return;
                    break;
                }
            }

            if (isSuccess)
                ShowTips("直线插补已停止");
            else
                ShowTips("停止直线插补失败:\n" + sbErr.ToString());
        }

        /// <summary>
        /// 将直线插补运动的所有轴伺服上电
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLineServOn_Click(object sender, EventArgs e)
        {
            if (_module == null)
            {
                ShowTips("无效操作，模块未设置");
                return;
            }
            if (!_module.IsOpen)
            {
                ShowTips("无效操作，模块未打开");
                return;
            }
            if(dgvLinePos.Rows.Count == 0)
            {
                MessageBox.Show("无效操作:没有设置插补轴");
                return;
            }
            List<int> axisIDs = new List<int>();
            foreach (DataGridViewRow row in dgvLinePos.Rows)
            {
                string axisIDTxt = row.Cells[0].Value as string;

                if (string.IsNullOrEmpty(axisIDTxt))
                {
                    MessageBox.Show("操作失败:存在未设置的轴序号");
                    return;
                }
                axisIDs.Add(Convert.ToInt32(axisIDTxt));
                   
            }
            bool isSuccess = true;
            StringBuilder sbErr = new StringBuilder();
            foreach(int axisID in axisIDs)
            {
                int err = _module.ServoOn(axisID);
                if(err != 0)
                {
                    sbErr.Append("轴" + axisID + " 伺服上电失败，ErrorInfo:" + _module.GetErrorInfo(err) + "\n");
                    isSuccess = false;
                }
            }

            if (isSuccess)
                ShowTips("所有直线插补伺服上电完成");
            else
            {
                MessageBox.Show("操作失败:\n" + sbErr.ToString());
            }
        }

        /// <summary>
        /// 圆弧运动模式改变:中心/角度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdCA_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 圆弧运动模式改变:中心/终点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdCE_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 开始圆弧运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStartArc_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 清空信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        /// <summary>
        /// 停止圆弧运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStopAcr_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 所有圆弧插补轴伺服上电
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btServoArc_Click(object sender, EventArgs e)
        {

        }
    }
}
