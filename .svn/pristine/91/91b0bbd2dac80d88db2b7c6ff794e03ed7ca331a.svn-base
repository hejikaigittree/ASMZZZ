using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;


namespace JFUI
{
    public partial class FormCmprTrigChnCfg : Form
    {
        public FormCmprTrigChnCfg()
        {
            InitializeComponent();
        }

        private void FormCmprTrigChnCfg_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            cbMode.Items.AddRange(new string[] { "禁用", "线性模式", "点表模式" });
            AdjustUI();
            if (IsChnValid)
                timer1.Enabled = true;
            else
                timer1.Enabled = false;
        }

        bool _isFormLoaded = false;
        IJFModule_CmprTrigger _module = null;
        int _encChn = 0;
        string[] _trigChnNames = null;
        List<CheckBox> chkBindTrigs = new List<CheckBox>();  //绑定输出通道 使能/禁用
        List<TextBox> tbTrigCounts = new List<TextBox>();//已触发次数
        //List<Button> btResetTrigs = new List<Button>();//触发次数置0

        bool _isParamEditting = false; //是否正在编辑触发参数
        public void SetChnInfo(IJFModule_CmprTrigger module,int encChn,string encChnName,string[] trigChnNames)
        {
            _module = module;
            _encChn = encChn;
            _trigChnNames = trigChnNames;
            Text = "触发器通道 " + encChnName == null ? "" : encChnName + " 参数设置" + "EncChn = " + _encChn;
            if (_isFormLoaded)
            {
                AdjustUI();
                if(IsChnValid)
                    timer1.Enabled = true;
            }
        }

        /// <summary>
        /// 调整界面布局
        /// </summary>
        void AdjustUI()
        {
            foreach(CheckBox cb in chkBindTrigs)
                gbEncParam.Controls.Remove(cb);
            chkBindTrigs.Clear();
            foreach(TextBox tb in tbTrigCounts)
                gbEncParam.Controls.Remove(tb);
            tbTrigCounts.Clear();
            //foreach(Button bt in btResetTrigs)
            //    gbEncParam.Controls.Remove(bt);
            //btResetTrigs.Clear();
            if (null == _module)
            {
                ShowTips("触发模块未设置（空值）");
                gbEncParam.Enabled = false;
                gbTrigParam.Enabled = false;
                return;
            }
            else
            {
                if(_encChn < 0 || _encChn >= _module.EncoderChannels)
                {
                    ShowTips(string.Format(" 编码器通道号 = {0} 超出范围 0~{1}",_encChn,_module.EncoderChannels-1));
                    gbEncParam.Enabled = false;
                    gbTrigParam.Enabled = false;
                    return;
                }
            }
            gbEncParam.Enabled = true;
            gbTrigParam.Enabled = true;
            ///获取脉冲当量
            double dVal = 0;
            int err = _module.GetEncoderFactor(_encChn, out dVal);
            if (err != 0)
            {
                ShowTips("未能获取脉冲当量参数，错误信息:" + _module.GetErrorInfo(err));
                tbFactor.Text = "获取失败";
            }
            else
                tbFactor.Text = dVal.ToString();
            
            ////获取触发输出通道信息
            int[] trigBinded = null;
            err = _module.GetEncoderTrigBind(_encChn, out trigBinded);
            if (err != 0)
                ShowTips("未能获取绑定的脉冲输出，错误信息:" + _module.GetErrorInfo(err));
            int locY = btTrig.Location.Y + btTrig.Height+3;
            for (int i = 0; i < _module.TrigChannels;i++)
            {
                CheckBox cb = new CheckBox();
                if (null == _trigChnNames)
                    cb.Text = "绑定触发通道_" + i;
                else
                {
                    if (i < _trigChnNames.Count())
                        cb.Text = "绑定触发通道_" + _trigChnNames[i];
                    else
                        cb.Text = "绑定触发通道_" + i;
                }
                cb.Location = new Point(3, locY);
                if (trigBinded != null && trigBinded.Contains(i))
                    cb.Checked = true;
                else
                    cb.Checked = false;
                chkBindTrigs.Add(cb);
                gbEncParam.Controls.Add(cb);
                cb.CheckedChanged += OnChkBindChange;

               //触发次数显示
                TextBox tb = new TextBox();
                tb.Location = new Point(103, locY);
                tbTrigCounts.Add(tb);
                gbEncParam.Controls.Add(tb);
                locY += 25;
            }

            _isParamEditting = false;
            btEditSave.Enabled = true;
            btEditSave.Text = "编辑";
            btCancel.Enabled = false;
            lbPosCount.Visible = false;
            tbPosCount.Visible = false;
            cbMode.Enabled = true;
            dgvParams.Rows.Clear();
            //获取触发模式
            JFCompareTrigMode mode;
            err = _module.GetTrigMode(_encChn, out mode);
            if (0 != err)
            {
                ShowTips("未能获取触发模式，错误信息:" + _module.GetErrorInfo(err));
                //cbMode.SelectedIndex = 0;
                cbMode.Text = "禁用";
            }
            else
            {
                if (mode == JFCompareTrigMode.disable)
                    cbMode.Text = "禁用";
                else if (mode == JFCompareTrigMode.liner)
                    cbMode.Text = "线性模式";
                else
                    cbMode.Text = "点表模式";

                //cbMode.SelectedIndex = (int)mode;
                //if(mode == JFCompareTrigMode.disable)
                //{
                //    btEditSave.Enabled = false;
                //}
                //if (mode == JFCompareTrigMode.liner)
                //{
                //    btEditSave.Enabled = true;
                //    JFCompareTrigLinerParam lp;
                //    err = _module.GetTrigLiner(_encChn, out lp);
                //    DataGridViewRow row = new DataGridViewRow();
                //    dgvParams.Rows.Add(row);
                //    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                //    cell.Value = "起始位置";
                //    row.Cells.Add(cell);
                //    cell = new DataGridViewTextBoxCell();
                //    cell.Value = (err == 0 ? lp.start.ToString() : "Error");
                //    row.Cells.Add(cell);

                //    row = new DataGridViewRow();
                //    dgvParams.Rows.Add(row);
                //    cell = new DataGridViewTextBoxCell();
                //    cell.Value = "触发间隔";
                //    row.Cells.Add(cell);
                //    cell = new DataGridViewTextBoxCell();
                //    cell.Value = (err == 0 ? lp.interval.ToString() : "Error");
                //    row.Cells.Add(cell);


                //    row = new DataGridViewRow();
                //    dgvParams.Rows.Add(row);
                //    cell = new DataGridViewTextBoxCell();
                //    cell.Value = "重复次数";
                //    row.Cells.Add(cell);
                //    cell = new DataGridViewTextBoxCell();
                //    cell.Value = (err == 0 ? lp.repeats.ToString() : "Error");
                //    row.Cells.Add(cell);

                //}
                //else// if()
                //{
                //    btEditSave.Enabled = true;
                //    lbPosCount.Visible = true;
                //    tbPosCount.Visible = true;
                //    tbPosCount.Enabled = false;
                //    double[] tables; //触发点表
                //    err = _module.GetTrigTable(_encChn, out tables);
                //    if (0 != err)
                //    {
                //        tbPosCount.Text = "Err";
                //    }
                //    else
                //    {
                //        if (null == tables)
                //            tbPosCount.Text = "0";
                //        else
                //        {
                //            tbPosCount.Text = tables.Length.ToString();
                //            for(int i = 0;i < tables.Length;i++)
                //            {
                //                DataGridViewRow row = new DataGridViewRow();
                //                dgvParams.Rows.Add(row);
                //                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                //                cell.Value = (i+1).ToString();
                //                row.Cells.Add(cell);
                //                cell = new DataGridViewTextBoxCell();
                //                cell.Value = tables[i].ToString();
                //                row.Cells.Add(cell);
                //                cell.ReadOnly = true;
                //            }
                //        }

                //    }
                //}
            }
            UpdateChnStatus();
        }

        void UpdateChnStatus()
        {
            int err = 0;
            //获取编码器当前位置
            if (!tbPos.Focused)
            {
                if (_module == null || _encChn < 0 || _encChn >= _module.EncoderChannels)
                    tbPos.Text = "无效设置";
                else
                {
                    double dVal = 0;
                    err = _module.GetEncoderCurrPos(_encChn, out dVal);
                    if (err != 0)
                        tbPos.Text = "获取失败";
                    else
                        tbPos.Text = dVal.ToString();
                }
            }

            //获取各通道脉冲计数
            if (!(_module == null || _encChn < 0 || _encChn >= _module.EncoderChannels))
            {
                int trigTimes = 0;
                for (int i = 0; i < _module.TrigChannels; i++)
                {
                    if (0 == _module.GetTriggedCount(i, out trigTimes))
                        tbTrigCounts[i].Text = trigTimes.ToString();
                    else
                        tbTrigCounts[i].Text = "获取失败";
                }

            }
        }

        int maxTips = 100;
        void ShowTips(string txt)
        {
            rbTips.AppendText(txt + "\n");
            string[] lines = rbTips.Text.Split('\n');
            if (lines.Length >= maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rbTips.Text = rbTips.Text.Substring(rmvChars);
            }
            rbTips.Select(rbTips.TextLength, 0); //滚到最后一行
            rbTips.ScrollToCaret();//滚动到控件光标处 
        }

        void OnChkBindChange(object sender, EventArgs e)
        {
            List<int> trigBinds = new List<int>();
            for (int i = 0; i < chkBindTrigs.Count; i++)
                if (chkBindTrigs[i].Checked)
                    trigBinds.Add(i);
            int err = _module.SetEncoderTrigBind(_encChn, trigBinds.ToArray());
            if (err != 0)
                ShowTips("绑定编码器的触发输出通道失败，错误信息:" + _module.GetErrorInfo(err));
            else
                ShowTips("绑定编码器的触发输出通道完成");
        }

        bool IsChnValid { get { return _module != null && _encChn >= 0 && _encChn <= _module.EncoderChannels; } }

        private void tbFactor_Enter(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            else
                tbFactor.BackColor = Color.White;

        }

        private void tbFactor_Leave(object sender, EventArgs e)
        {
            tbFactor.BackColor = SystemColors.Control;
            if (!IsChnValid)
            {
                tbFactor.Text = "无效设置";
                return;
            }
            double dVal = 0;
            int err = _module.GetEncoderFactor(_encChn, out dVal);
            if (0 != err)
                tbFactor.Text = "获取失败";
            else
                tbFactor.Text = dVal.ToString();
        }

        private void tbFactor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsChnValid)
                return;
            double dVal = 0;
            if (e.KeyCode == Keys.Escape) //取消修改
            {
                tbFactor.BackColor = SystemColors.Control;
                int err = _module.GetEncoderFactor(_encChn, out dVal);
                if(0!= err)
                {
                    tbFactor.Text = "未知";
                    return;
                }

            }
            else if(e.KeyCode == Keys.Enter)
            {
                double dCurr;
                if (!double.TryParse(tbFactor.Text, out dCurr))
                {
                    MessageBox.Show("参数格式错误");
                    return;
                }

                int err = _module.SetEncoderFactor(_encChn, dCurr);
                if(err !=0)
                {
                    MessageBox.Show("设置脉冲当量失败，错误信息:" + _module.GetErrorInfo(err));
                    return;
                }
                else
                {
                    tbFactor.BackColor = SystemColors.Control;
                    ShowTips("设置脉冲当量完成");
                    return;
                }
            }

        }

        private void tbFactor_TextChanged(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            double dCurr = 0;
            if(!double.TryParse(tbFactor.Text,out dCurr))
            {
                tbFactor.BackColor = Color.Red;
                return;
            }

            double dVal = 0;
            int err = _module.GetEncoderFactor(_encChn, out dVal);
            if (err != 0 || dVal != dCurr)
                tbFactor.BackColor = Color.OrangeRed;
            else
                tbFactor.BackColor = Color.White;
        }

        private void tbPos_Enter(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            else
                tbPos.BackColor = Color.White;
        }

        private void tbPos_Leave(object sender, EventArgs e)
        {
            tbPos.BackColor = SystemColors.Control;
            if (!IsChnValid)
            {
                tbPos.Text = "无效设置";
                return;
            }
            double dVal = 0;
            int err = _module.GetEncoderCurrPos(_encChn, out dVal);
            if (0 != err)
                tbPos.Text = "获取失败";
            else
                tbPos.Text = dVal.ToString();
        }

        private void tbPos_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsChnValid)
                return;
            double dVal = 0;
            if (e.KeyCode == Keys.Escape) //取消修改
            {
                tbPos.BackColor = SystemColors.Control;
                int err = _module.GetEncoderCurrPos(_encChn, out dVal);
                if (0 != err)
                {
                    tbPos.Text = "未知";
                    return;
                }

            }
            else if (e.KeyCode == Keys.Enter)
            {
                double dCurr;
                if (!double.TryParse(tbPos.Text, out dCurr))
                {
                    MessageBox.Show("参数格式错误");
                    return;
                }

                int err = _module.SetEncoderCurrPos(_encChn, dCurr);
                if (err != 0)
                {
                    MessageBox.Show("设置编码器位置失败，错误信息:" + _module.GetErrorInfo(err));
                    return;
                }
                else
                {
                    tbPos.BackColor = SystemColors.Control;
                    ShowTips("设置编码器位置完成");
                    return;
                }
            }
        }

        private void tbPos_TextChanged(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            double dCurr = 0;
            if (!double.TryParse(tbPos.Text, out dCurr))
            {
                tbPos.BackColor = Color.Red;
                return;
            }

            double dVal = 0;
            int err = _module.GetEncoderCurrPos(_encChn, out dVal);
            if (err != 0 || dVal != dCurr)
                tbPos.BackColor = Color.OrangeRed;
            else
                tbPos.BackColor = Color.White;
        }

        private void btSync_Click(object sender, EventArgs e)
        {
            if(!IsChnValid)
            {
                MessageBox.Show("同步编码器失败:无效的编码器/通道号设置！");
                return;
            }
            int err = _module.SyncEncoderCurrPos(_encChn);
            if(0!=err)
            {
                MessageBox.Show("同步编码器失败，错误信息:" + _module.GetErrorInfo(err));
                return;
            }
            ShowTips("同步编码器完成！");
            UpdateChnStatus();
        }
        
        /// <summary>软触发编码器绑定的所有触发输出通道</summary>
        private void btTrig_Click(object sender, EventArgs e)
        {
            if(!IsChnValid)
            {
                MessageBox.Show("软触发失败:无效的编码器/通道号设置！");
                return;
            }

            int[] trigs;
            int err = _module.GetEncoderTrigBind(_encChn, out trigs);
            if(0!= err)
            {
                MessageBox.Show("软触发失败，未能获取编码器绑定的输出通道\n错误信息：" + _module.GetErrorInfo(err));
                return;
            }

            err = _module.SoftTrigge(trigs);
            if (0 != err)
            {
                MessageBox.Show("软触发失败，错误信息：" + _module.GetErrorInfo(err));
                return;
            }

            if(null == trigs || trigs.Length == 0)
            {
                ShowTips("软触发完成！未绑定输出通道");
                return;
            }

            StringBuilder sb = new StringBuilder("软触发完成！输出通道：");
            for(int i = 0; i < trigs.Length; i++)
            {
                sb.Append(trigs[i].ToString());
                if (i < trigs.Length - 1)
                    sb.Append("|");
            }
            ShowTips(sb.ToString());
            UpdateChnStatus();
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsChnValid)
            {
                ShowTips("更改触发模式失败：模块对象为空值/通道号超出范围");
                return;
            }
            if (cbMode.SelectedIndex < 0)
                return;
            int err = 0;
            lbPosCount.Visible = false;
            tbPosCount.Visible = false;
            dgvParams.Rows.Clear();
            if (cbMode.SelectedIndex ==0)//if (mode == JFCompareTrigMode.disable)
            {
                err = _module.SetTrigMode(_encChn, JFCompareTrigMode.disable);
                if (err != 0)
                    ShowTips("设置触发模式\"禁用\"失败，错误信息：" + _module.GetErrorInfo(err));
                else
                    ShowTips("设置触发模式\"禁用\"完成");
                btEditSave.Enabled = false;
            }
            else if (cbMode.SelectedIndex == 1)//(mode == JFCompareTrigMode.liner)
            {
                err = _module.SetTrigMode(_encChn, JFCompareTrigMode.liner);
                if (err != 0)
                {
                    ShowTips("设置触发模式\"线性模式\"失败，错误信息：" + _module.GetErrorInfo(err));
                    return;
                }
                else
                    ShowTips("设置触发模式\"线性模式\"完成");
                dgvParams.Rows.Clear();
                dgvParams.ReadOnly = true;
                btEditSave.Enabled = true;
                JFCompareTrigLinerParam lp;
                err = _module.GetTrigLiner(_encChn, out lp);
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = "起始位置";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = (err == 0 ? lp.start.ToString() : "Error");
                row.Cells.Add(cell);
                dgvParams.Rows.Add(row);

                row = new DataGridViewRow();
                cell = new DataGridViewTextBoxCell();
                cell.Value = "触发间隔";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = (err == 0 ? lp.interval.ToString() : "Error");
                row.Cells.Add(cell);
                dgvParams.Rows.Add(row);

                row = new DataGridViewRow();
                cell = new DataGridViewTextBoxCell();
                cell.Value = "重复次数";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = (err == 0 ? lp.repeats.ToString() : "Error");
                row.Cells.Add(cell);
                dgvParams.Rows.Add(row);
            }
            else// if()
            {
                err = _module.SetTrigMode(_encChn, JFCompareTrigMode.liner);
                if (err != 0)
                {
                    ShowTips("设置触发模式\" 点表模式\"失败，错误信息：" + _module.GetErrorInfo(err));
                    return;
                }
                else
                    ShowTips("设置触发模式\"点表模式\"完成");

                btEditSave.Enabled = true;
                lbPosCount.Visible = true;
                tbPosCount.Visible = true;
                tbPosCount.Enabled = false;
                dgvParams.ReadOnly = true;
                double[] tables; //触发点表
                err = _module.GetTrigTable(_encChn, out tables);
                if (0 != err)
                {
                    tbPosCount.Text = "Err";
                }
                else
                {
                    if (null == tables)
                        tbPosCount.Text = "0";
                    else
                    {
                        tbPosCount.Text = tables.Length.ToString();
                        for (int i = 0; i < tables.Length; i++)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                            cell.Value = (i + 1).ToString();
                            row.Cells.Add(cell);
                            cell = new DataGridViewTextBoxCell();
                            cell.Value = tables[i].ToString();
                            row.Cells.Add(cell);
                            dgvParams.Rows.Add(row);
                            //cell.ReadOnly = true;
                        }
                    }

                }
            }
        }

        private void btEditSave_Click(object sender, EventArgs e)
        {
            if(!IsChnValid)
            {
                MessageBox.Show("无效操作：控制器未设置/编码器通道超出范围");
                return;
            }
            if(!_isParamEditting)//开始编辑参数
            {
                if (cbMode.SelectedIndex <= 0)
                    return;
                if(cbMode.SelectedIndex == 2)
                    tbPosCount.Enabled = true;
                cbMode.Enabled = false;
                btEditSave.Text = "保存";
                dgvParams.ReadOnly = false;
                dgvParams.Columns[0].ReadOnly = true;
                dgvParams.Columns[1].ReadOnly = false;
                btCancel.Enabled = true;
                _isParamEditting = true;
            }
            else //保存参数
            {
                int err = 0;
                do
                {
                    if (cbMode.SelectedIndex <= 0)
                        break;
                    else if (cbMode.SelectedIndex == 1) //线性模式
                    {
                        double _start = 0, _interval = 0;
                        int _repeat = 0;
                        if(!double.TryParse(dgvParams.Rows[0].Cells[1].Value.ToString(),out _start))
                        {
                            dgvParams.Rows[0].Cells[1].Selected = true;
                            MessageBox.Show("起始位置参数格式错误");
                            return;
                        }

                        if (!double.TryParse(dgvParams.Rows[1].Cells[1].Value.ToString(), out _interval))
                        {
                            dgvParams.Rows[1].Cells[1].Selected = true;
                            MessageBox.Show("触发间隔参数格式错误");
                            return;
                        }

                        if(!int.TryParse(dgvParams.Rows[2].Cells[1].Value.ToString(), out _repeat))
                        {
                            dgvParams.Rows[2].Cells[1].Selected = true;
                            MessageBox.Show("重复次数参数格式错误");
                            return;
                        }

                        if(_repeat < 0)
                        {
                            dgvParams.Rows[2].Cells[1].Selected = true;
                            MessageBox.Show("重复次数参数必须为非负的整数");
                            return;
                        }
                        JFCompareTrigLinerParam param = new JFCompareTrigLinerParam() { start = _start,interval = _interval,repeats = _repeat};
                        err = _module.SetTrigLiner(_encChn, param);
                        if(err !=0)
                        {
                            MessageBox.Show("保存线性触发参数到控制器失败，错误信息:" + _module.GetErrorInfo(err));
                            return;
                        }
                        else
                        {
                            ShowTips("保存线性触发参数完成");
                            break;
                        }
                    }
                    else //点表模式
                    {
                        if(0 == dgvParams.Rows.Count)
                        {
                            MessageBox.Show("保存点表参数失败，点表为空");
                            return;
                        }
                        List<double> tables = new List<double>();
                        foreach(DataGridViewRow row in dgvParams.Rows)
                        {
                            double dTmp = 0 ;
                            if(!double.TryParse(row.Cells[1].Value.ToString(),out dTmp))
                            {
                                row.Cells[1].Selected = true;
                                MessageBox.Show("参数格式错误");
                                return;
                            }
                            tables.Add(dTmp);
                        }

                        err = _module.SetTrigTable(_encChn, tables.ToArray());
                        if(err !=0 )
                        {
                            MessageBox.Show("设置点表参数失败，错误信息:" + _module.GetErrorInfo(err));
                            return;
                        }
                        else
                        {
                            ShowTips("设置点表参数成功");
                            dgvParams.ClearSelection();
                        }
                    }
                } while (false);

                btEditSave.Text = "编辑";
                btCancel.Enabled = false;
                cbMode.Enabled = true;
                //dgvParams.Rows.Clear();
                dgvParams.ReadOnly = true;
                dgvParams.Columns[1].ReadOnly = true;
                btCancel.Enabled = false;
                _isParamEditting = false;
            }
        }

        private void btCancel_Click(object sender, EventArgs e) //取消编辑
        {
            int err = 0;
            btEditSave.Text = "编辑";
            btCancel.Enabled = false;
            cbMode.Enabled = true;
            dgvParams.Rows.Clear();
            dgvParams.Columns[1].ReadOnly = true;
            _isParamEditting = false;
            if (cbMode.SelectedIndex == 0)//if (mode == JFCompareTrigMode.disable)
                return;
            
            else if (cbMode.SelectedIndex == 1)//(mode == JFCompareTrigMode.liner)
            {
                btEditSave.Enabled = true;
                JFCompareTrigLinerParam lp;
                err = _module.GetTrigLiner(_encChn, out lp);
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = "起始位置";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = (err == 0 ? lp.start.ToString() : "Error");
                row.Cells.Add(cell);
                dgvParams.Rows.Add(row);

                row = new DataGridViewRow();
                cell = new DataGridViewTextBoxCell();
                cell.Value = "触发间隔";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = (err == 0 ? lp.interval.ToString() : "Error");
                row.Cells.Add(cell);
                dgvParams.Rows.Add(row);


                row = new DataGridViewRow();
                cell = new DataGridViewTextBoxCell();
                cell.Value = "重复次数";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = (err == 0 ? lp.repeats.ToString() : "Error");
                row.Cells.Add(cell);
                dgvParams.Rows.Add(row);

            }
            else// if()
            {
                tbPosCount.Enabled = false;
                double[] tables; //触发点表
                err = _module.GetTrigTable(_encChn, out tables);
                if (0 != err)
                {
                    tbPosCount.Text = "Err";
                }
                else
                {
                    if (null == tables)
                        tbPosCount.Text = "0";
                    else
                    {
                        tbPosCount.Text = tables.Length.ToString();
                        for (int i = 0; i < tables.Length; i++)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                            cell.Value = (i + 1).ToString();
                            row.Cells.Add(cell);
                            cell = new DataGridViewTextBoxCell();
                            cell.Value = tables[i].ToString();
                            row.Cells.Add(cell);
                            cell.ReadOnly = true;
                            dgvParams.Rows.Add(row);
                        }
                    }

                }
            }
            dgvParams.ClearSelection();
        }

        private void tbPosCount_Enter(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            tbPosCount.BackColor = Color.White;
        }

        //点表个数控件失去焦点
        private void tbPosCount_Leave(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;

            double[] tables; //触发点表
            int err = _module.GetTrigTable(_encChn, out tables);
            if (0 != err)
                tbPosCount.Text = "Err";
            else
            {
                if (null == tables)
                    tbPosCount.Text = "0";
                else
                    tbPosCount.Text = tables.Length.ToString();
            }
            tbPosCount.BackColor = SystemColors.Control;
        }

        private void tbPosCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsChnValid)
                return;

            double[] tables; //触发点表
            int err = _module.GetTrigTable(_encChn, out tables);



            if (e.KeyCode == Keys.Escape)//取消设置
            {
                tbPosCount.BackColor = SystemColors.Control;
                if (0 != err)
                {
                    tbPosCount.Text = "Err";
                }
                else
                {
                    if (null == tables)
                        tbPosCount.Text = "0";
                    else
                        tbPosCount.Text = tables.Length.ToString();
                }
            }
            else if(e.KeyCode == Keys.Enter) 
            {
                int count = 0;
                if (!int.TryParse(tbPosCount.Text, out count))
                {
                    MessageBox.Show("输入参数格式错误");
                    return;
                }
                if(count < 0)
                {
                    MessageBox.Show("参数错误,必须为非负整数");
                    return;
                }
                tbPosCount.BackColor = SystemColors.Control;
                if (count == dgvParams.Rows.Count)
                {
                    
                    return;
                }
                else if(count < dgvParams.Rows.Count) //需要减点
                {
                    while (dgvParams.Rows.Count > count)
                        dgvParams.Rows.RemoveAt(dgvParams.Rows.Count - 1);
                    return;
                }
                else //需要加点
                {
                    while (dgvParams.Rows.Count < count)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                        cell.Value = dgvParams.Rows.Count + 1;
                        row.Cells.Add(cell);
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = 0;
                        row.Cells.Add(cell);
                        dgvParams.Rows.Add(row);
                    }
                }

            }
        }

        private void tbPosCount_TextChanged(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            int count = 0;
            if(!int.TryParse(tbPosCount.Text,out count))
            {
                tbPosCount.BackColor = Color.Red;
                return;
            }

            double[] tables;
            int err = _module.GetTrigTable(_encChn, out tables);
            if (0 != err)
            {
                tbPosCount.BackColor = Color.OrangeRed;
                return;
            }
            else
            {
                if (null == tables)
                {
                    if (0 == count)
                    {
                        tbPosCount.BackColor = Color.White;
                        return;
                    }
                    tbPosCount.BackColor = Color.OrangeRed;
                    return;
                }
                else
                {
                    if(count == tables.Length)
                    {
                        tbPosCount.BackColor = Color.White;
                        return;
                    }
                    tbPosCount.BackColor = Color.OrangeRed;
                    return;
                }
            }


        }

        /// <summary>刷新状态</summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            UpdateChnStatus();
        }
    }
}
