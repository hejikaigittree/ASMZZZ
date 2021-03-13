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
using JFUI;
using System.Threading;

namespace JFHub
{
    public partial class UcMethodFlow : UserControl
    {
        public UcMethodFlow()
        {
            InitializeComponent();
        }


        
        /// <summary>
        /// 工作状态发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currWorkStatus"></param>
        void OnMFWorkStatusChanged(object sender, JFWorkStatus currWorkStatus)
        {
            if (!Created)
                return;
            /*Begin*/Invoke(new dgMFWorkStatusChanged(_onMFWorkStatusChanged), new object[] { currWorkStatus });
        }

        delegate void dgMFWorkStatusChanged( JFWorkStatus currWorkStatus);
        void _onMFWorkStatusChanged(JFWorkStatus currWorkStatus)
        {
            UpdatePoolValues();
            switch(currWorkStatus)
            {
                case JFWorkStatus.Running:
                    lbWorkStatus.Text = "运行中";
                    btStop.Enabled = true;
                    btPause.Enabled = true;
                    btReset.Enabled = false;
                    btAction.Enabled = false;
                    if (_methodFlow.RunningMode != JFMethodFlow.RunMode.Step)
                        btStep.Enabled = false;
                    else
                    {
                        btAction.Enabled = false;
                        btAction.Text = "运行";
                        btStep.Enabled = true;
                    }
                    break;
                case JFWorkStatus.Pausing:
                    lbWorkStatus.Text = "暂停中";
                    ShowTips("已暂停...");
                    btAction.Enabled = true;
                    btPause.Enabled = false;
                    btAction.Text = "恢复";
                    break;
                case JFWorkStatus.Interactiving:
                    lbWorkStatus.Text = "交互中";
                    ShowTips("人机交互中...");
                    break;
                case JFWorkStatus.NormalEnd:
                    lbWorkStatus.Text = "正常结束";
                    btAction.Enabled = true;
                    btAction.Text = "运行";
                    btPause.Enabled = false;
                    btReset.Enabled = true;
                    btStop.Enabled = true;
                    btStep.Enabled = true;
                    ShowTips("已正常结束");
                    break;
                case JFWorkStatus.AbortExit:
                    lbWorkStatus.Text = "强制终止";
                    btAction.Enabled = true;
                    btAction.Text = "运行";
                    btPause.Enabled = false;
                    btReset.Enabled = true;
                    btStop.Enabled = true;
                    btStep.Enabled = true;
                    ShowTips("已强制终止");

                    break;
                case JFWorkStatus.CommandExit:
                    lbWorkStatus.Text = "指令停止";
                    btAction.Enabled = true;
                    btAction.Text = "运行";
                    btPause.Enabled = false;
                    btReset.Enabled = true;
                    btStop.Enabled = true;
                    btStep.Enabled = true;
                    ShowTips("已指令退出");
                    break;
                case JFWorkStatus.ErrorExit:
                    lbWorkStatus.Text = "错误停止";
                    btAction.Enabled = true;
                    btAction.Text = "运行";
                    btPause.Enabled = false;
                    btReset.Enabled = true;
                    btStop.Enabled = true;
                    btStep.Enabled = true;
                    ShowTips("已错误停止");
                    ShowTips("错误信息：" + _methodFlow.ActionErrorInfo());
                    break;
                case JFWorkStatus.ExceptionExit:
                    lbWorkStatus.Text = "异常停止";
                    btAction.Enabled = true;
                    btAction.Text = "运行";
                    btPause.Enabled = false;
                    btReset.Enabled = true;
                    btStop.Enabled = true;
                    btStep.Enabled = true;
                    ShowTips("错误信息：" + _methodFlow.ActionErrorInfo());
                    break;
                case JFWorkStatus.UnStart:
                    lbWorkStatus.Text = "未运行";
                    btAction.Enabled = true;
                    btAction.Text = "运行";
                    btPause.Enabled = false;
                    btReset.Enabled = true;
                    btStop.Enabled = true;
                    btStep.Enabled = true;
                    break;
                

            }
        }



        delegate void dgMFCustomStatusChanged(object sender, int currCustomStatus);
        /// <summary>
        /// 自定义状态发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currCustomStatus"></param>
        void OnMFCustomStatusChanged(object sender, int currCustomStatus)
        {
            if(InvokeRequired)
            {
                /*Begin*/Invoke(new dgMFCustomStatusChanged(OnMFCustomStatusChanged), new object[] { sender, currCustomStatus });
                return;
            }
            UpdatePoolValues();
            lbCurrStep.Text = _methodFlow.GetCustomStatusName(currCustomStatus);
        }

        void OnMFMsgInfo(object sender, string msg)
        {
            ShowTips(msg);
        }

        private void UcMethodFlow_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustView();
            UpdateTypeTreeView();
        }
        bool _isFormLoaded = false;
        Type[] _methodTypes = new Type[] { typeof(IJFMethod) };  // 可选用的方法接口类型
        List<Button> _lstBtDel = new List<Button>();//用于删除MethodItem的按钮集合
        List<Button> _lstBtAct = new List<Button>();//用于调试MethodItem的Action按钮集合
        List<JFRealtimeUI> _lstRtUi = new List<JFRealtimeUI>();
        JFMethodFlow _methodFlow = null;
        /// <summary>
        /// 设置需要调试的动作流对象
        /// </summary>
        /// <param name="mf"></param>
        public void SetMethodFlow(JFMethodFlow mf)
        {
            _methodFlow = mf;
            _methodFlow.WorkStatusChanged += OnMFWorkStatusChanged;
            _methodFlow.CustomStatusChanged += OnMFCustomStatusChanged;
            _methodFlow.WorkMsg2Outter += OnMFMsgInfo;
            if (_isFormLoaded)
                AdjustView();
        }


        /// <summary>
        /// 根据_methodFlow 更新界面内容
        /// </summary>
        void AdjustView()
        {
            lbName.Text = "名称:null";
            if (null == _methodFlow)
            {
                btClearFlow.Enabled = false;
                btStep.Enabled = false;
                btAction.Enabled = false;
                btReset.Enabled = false;
                ShowTips("动作流对象未设置");

            }
            else
            {
                btClearFlow.Enabled = true;
                btStep.Enabled = true;
                btAction.Enabled = true;
                btReset.Enabled = true;
                lbName.Text = "名称:" + _methodFlow.Name;
                ShowTips("动作流对象已设置:" + _methodFlow.Name);
            }
            UpdateFlow2UI();
        }

        /// <summary>
        /// 更新动作流面板和数据池面板
        /// </summary>
        public void UpdateFlow2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateFlow2UI));
                return;
            }
            pnFlow.Controls.Clear();
            dgvDataPool.Rows.Clear();
            dgvOutterDataPool.Rows.Clear();
            _lstBtDel.Clear();
            _lstBtAct.Clear();
            _lstRtUi.Clear();
            if (null == _methodFlow)
                return;
            int mtdCount = _methodFlow.Count;
            List<string> ExistOutputIDs = new List<string>();
            //for (int i = 0; i < mtdCount; i++)
            //    ExistOutputIDs.AddRange(_methodFlow.GetItem(i).OutputNameIDs.Values);

            int locX = 0, locY = 3;
            for (int i = 0; i < mtdCount; i++)
            {
                JFMethodFlow.MethodItem mi = _methodFlow.GetItem(i);
                ///获取各Method对象的实时界面
                IJFMethod mtd = mi.Value;
                if (mtd is IJFRealtimeUIProvider)
                    _lstRtUi.Add((mtd as IJFRealtimeUIProvider).GetRealtimeUI());
                else
                    _lstRtUi.Add(null);


                Label lbMiName = new Label(); //动作单元名称
                lbMiName.Text = i + ":" + mi.Name;
                lbMiName.Location = new Point(locX , locY+3);
                pnFlow.Controls.Add(lbMiName);
                Button btDel = new Button();

                Button btAct = new Button();
                btAct.Text = "执行";
                btAct.Width = 37;
                btAct.Location = new Point(270 - btAct.Width, locY);//270为UcMethodNode对象的宽度
                btAct.Click += OnButtonClick_MethodItemAction;
                _lstBtAct.Add(btAct);
                pnFlow.Controls.Add(btAct);

                btDel.Text = "删除";
                btDel.Width = 37;
                btDel.Location = new Point(btAct.Left - btDel.Width-10, locY);//270为UcMethodNode对象的宽度
                btDel.Click += OnButtonClick_DelMethodItem;
                _lstBtDel.Add(btDel);
                pnFlow.Controls.Add(btDel);


                locY = btDel.Bottom + 2;
                UcMethodNode mn = new UcMethodNode();
                mn.SetMethodItem(mi);
                mn.EventShowInfo += ShowMethodItemInfo;
                mn.Location = new Point(locX, locY);
                pnFlow.Controls.Add(mn);
                mn.UpdateAvailableIDs(_methodFlow.UnexportIDs);//(mi.AvailedInputIDs/*allOutputID.ToArray()*/);
                locY = mn.Bottom + 2;

                ///更新数据池列表(内部)
                string[] outputNames = mi.Value.MethodOutputNames;
                if (null != outputNames)
                    foreach(string outname in outputNames)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                        string outputID = mi.OutputID(outname);
                        cellName.Value = outputID;
                        ExistOutputIDs.Add(outputID);
                        row.Cells.Add(cellName);
                        DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
                        row.Cells.Add(cellValue);
                        dgvDataPool.Rows.Add(row);
                    }
            }
            foreach(string key in _methodFlow.DataPool.Keys) //更新数据池中的其他变量（不是算子的输出）
                if(!ExistOutputIDs.Contains(key))
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    
                    cellName.Value = key;
                    ExistOutputIDs.Add(key);
                    row.Cells.Add(cellName);
                    DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
                    row.Cells.Add(cellValue);
                    dgvDataPool.Rows.Add(row);
                }
            ///跟新外部数据池
            string[] OutterAvailedDataIDs = _methodFlow.OutterAvailedIDs;
            if(null != OutterAvailedDataIDs)
                foreach (string dataID in OutterAvailedDataIDs)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = dataID;
                    ExistOutputIDs.Add(dataID);
                    row.Cells.Add(cellName);
                    DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
                    row.Cells.Add(cellValue);
                    dgvOutterDataPool.Rows.Add(row);
                }


            UpdatePoolValues();
        }

        /// <summary>
        /// 更新数据池中的值
        /// </summary>
        public void UpdatePoolValues()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdatePoolValues));
                return;
            }
            if (null == _methodFlow)
                return;
            ///更新内部数据池
            Dictionary<string, object> dataPool = _methodFlow.DataPool;
            Dictionary<string, Type> dataTypes = _methodFlow.TypePool;
            foreach(DataGridViewRow row in dgvDataPool.Rows)
            {
                string paramID = row.Cells[0].Value as string;
                if(dataPool.ContainsKey(paramID))
                {
                    object paramValue = dataPool[paramID];
                    if (null == paramValue)
                        row.Cells[1].Value = "null";
                    else
                    {
                        Type paramType = dataTypes[paramID];
                        if (paramType.IsValueType && paramType.IsPrimitive || paramType.IsEnum || paramType == typeof(string))
                            row.Cells[1].Value = paramValue.ToString();
                        else
                        {
                            Type valuesType = paramValue.GetType();
                            if(valuesType.IsValueType && valuesType.IsPrimitive || valuesType.IsEnum || valuesType == typeof(string))
                                row.Cells[1].Value = paramValue.ToString();
                            else
                            row.Cells[1].Value = "类型:" + paramType.Name + " 无法显示";
                        }
                    }
                }
                else
                {
                    row.Cells[1].Value = "无效ID";
                }
            }
            ///更新外部数据池
            Dictionary<string, object> outterDataPool = _methodFlow.OutterDataPool;
            Dictionary<string, Type> outterDataTypes = _methodFlow.OutterTypePool;
            foreach (DataGridViewRow row in dgvOutterDataPool.Rows)
            {
                string paramID = row.Cells[0].Value as string;
                if (outterDataPool.ContainsKey(paramID))
                {
                    object paramValue = outterDataPool[paramID];
                    if (null == paramValue)
                        row.Cells[1].Value = "null";
                    else
                    {
                        Type paramType = outterDataTypes[paramID];
                        if (paramType.IsValueType && paramType.IsPrimitive || paramType.IsEnum || paramType == typeof(string))
                            row.Cells[1].Value = paramValue.ToString();
                        else
                        {
                            Type valueType = paramValue.GetType();
                            if (valueType.IsValueType && valueType.IsPrimitive || valueType.IsEnum || valueType == typeof(string))
                                row.Cells[1].Value = paramValue.ToString();
                            else
                                row.Cells[1].Value = "类型:" + valueType.Name + " 无法显示";
                        }
                    }
                }
                else
                {
                    row.Cells[1].Value = "无效ID";
                }
            }
        }

        void ShowMethodItemInfo(object sender,string info)
        {
            ShowTips(info);
        }

        void OnButtonClick_DelMethodItem(object sender,EventArgs e)
        {
            for(int i = 0; i < _lstBtDel.Count;i++)
                if(sender == _lstBtDel[i])
                {
                    if(DialogResult.OK == MessageBox.Show("确定删除方法节点:" + _methodFlow.GetItem(i).Name + "?","警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning))
                    {
                        _methodFlow.RemoveAt(i);
                        UpdateFlow2UI();
                    }
                }
        }

        void OnButtonClick_MethodItemAction(object sender, EventArgs e)
        {
            for (int i = 0; i < _lstBtAct.Count; i++)
                if (sender == _lstBtAct[i])
                {
                    JFMethodFlow.MethodItem mi = _methodFlow.GetItem(i);
                    bool isOK = false;
                    try
                    {
                        isOK = mi.Action();
                    }
                    catch(JFBreakMethodFlowException)
                    {
                        isOK = true;
                        ShowTips("动作节点:" + mi.Name + " Break 退出流程" );
                        UpdateMethodUi(i);
                        UpdatePoolValues();
                        return;
                    }
                    catch(Exception ex)
                    {
                        ShowTips("动作节点:" + mi.Name + " 执行异常，信息:" + ex.Message);
                        UpdateMethodUi(i);
                        UpdatePoolValues();
                        return;
                    }
                    if (!isOK)
                        ShowTips("动作节点:" + mi.Name + " 执行失败，错误信息:" + mi.ActionErrorInfo());
                    else
                    {
                        ShowTips("动作节点:" + mi.Name + " 执行完成，耗时：" + mi.Value.GetActionSeconds().ToString("F3") + "秒");
                        UpdateMethodUi(i);
                        UpdatePoolValues();
                    }
                    return;
                }
        }


        /// <summary>
        /// 设置方法列表中可选的方法接口类型
        /// </summary>
        /// <param name="types">如果是null，表示程序集中的所有方法都可以选用</param>
        public void SetMethodTypeFilter(Type[] methodTypes)
        {
            if (methodTypes == null)
                _methodTypes = new Type[] { typeof(IJFMethod) };
            else
            {
                List<Type> existMethodType = new List<Type>();
                foreach(Type mt in methodTypes)
                    if(typeof(IJFMethod).IsAssignableFrom(mt) )
                        if(!existMethodType.Contains(mt))
                            existMethodType.Add(mt);
                        
                    
                _methodTypes = existMethodType.ToArray();
            }
            if(_isFormLoaded)
            UpdateTypeTreeView();
        }

        /// <summary>
        /// 判断 方法类是否未被分类
        /// </summary>
        /// <param name="categoryLevels"></param>
        /// <returns></returns>
        bool _IsUncategorized(JFCategoryLevelsAttribute[] categoryLevels)
        {
            if (null == categoryLevels || 0 == categoryLevels.Length || categoryLevels[0].Levels == null || categoryLevels[0].Levels.Length == 0 || string.IsNullOrEmpty(categoryLevels[0].Levels[0]))
                return true;
            return false;
        }

        /// <summary>
        /// 将方法类添加到未分类
        /// </summary>
        /// <param name="t"></param>
        void Add2Uncategorized(Type t)
        {
            string tital = t.Name;
            JFDisplayNameAttribute[] disName = t.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
            if (null != disName && disName.Length > 0)
                tital = disName[0].Name;
            TreeNode tnMIType = new TreeNode(tital);
            tnMIType.Tag = t;
            if (!trvMethodRoot.Nodes.ContainsKey("Uncategorized") )
                trvMethodRoot.Nodes.Add("Uncategorized", "未分类方法");
            
            trvMethodRoot.Nodes["Uncategorized"].Nodes.Add(tnMIType);



            //trvMethodRoot.Nodes.Add(tnMIType);
            //Type[] classTypes = JFMethodFlow.AllChildClass(t);//JFFunctions.GetClassTypesInherited(t);
            //foreach (Type ct in classTypes)
            //{
            //    tital = ct.Name;
            //    disName = ct.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
            //    if (null != disName && disName.Length > 0)
            //        tital = disName[0].Name;
            //    TreeNode tnMethod = new TreeNode(tital);
            //    tnMethod.Tag = ct;
            //    tnMIType.Nodes.Add(tnMethod);

            //}
        }

        /// <summary>
        /// 根据类型的类别描述，添加到Tree中
        /// </summary>
        /// <param name="t"></param>
        /// <param name="categotyLevels"></param>
        void AddCategory(Type t,string[] categotyLevels)
        {            
            string tital = t.Name;
            JFDisplayNameAttribute[] disName = t.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
            if (null != disName && disName.Length > 0)
                tital = disName[0].Name;
            TreeNode tnMIType = new TreeNode(tital);
            tnMIType.Tag = t;

            if (!trvMethodRoot.Nodes.ContainsKey(categotyLevels[0]))
                trvMethodRoot.Nodes.Add(categotyLevels[0], categotyLevels[0]);
            TreeNode parentNode = trvMethodRoot.Nodes[categotyLevels[0]];
            for(int i = 1; i < categotyLevels.Length;i++)
            {
                if (!parentNode.Nodes.ContainsKey(categotyLevels[i]))
                    parentNode.Nodes.Add(categotyLevels[i], categotyLevels[i]);
                parentNode = parentNode.Nodes[categotyLevels[i]];
            }
            parentNode.Nodes.Add(tnMIType);
        }

        void UpdateTypeTreeView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateTypeTreeView));
                return;
            }
            trvMethodRoot.Nodes.Clear();
            foreach (Type t in _methodTypes)
            {
                Type[] methodClassTypes = JFMethodFlow.AllChildClass(t);
                if (null != methodClassTypes)
                    foreach (Type methodCT in methodClassTypes)
                    {
                        JFCategoryLevelsAttribute[] categoryLevels = methodCT.GetCustomAttributes(typeof(JFCategoryLevelsAttribute), false) as JFCategoryLevelsAttribute[];
                        if (_IsUncategorized(categoryLevels))
                            Add2Uncategorized(methodCT);
                        else
                            AddCategory(methodCT, categoryLevels[0].Levels);
                    }
                

            }



        }

        int maxTips = 200;
        delegate void dgShowTips(string info);
        public void ShowTips(string info)
        {
            if(InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            rchTips.AppendText(info + "\n");
            string[] lines = rchTips.Text.Split('\n');
            if (lines.Length >= maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rchTips.Text = rchTips.Text.Substring(rmvChars);
            }
            rchTips.Select(rchTips.TextLength, 0); //滚到最后一行
            rchTips.ScrollToCaret();//滚动到控件光标处 
        }

        /// <summary>
        /// 清空动作流中所有方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClearFlow_Click(object sender, EventArgs e)
        {
            if (null == _methodFlow)
                return;
            if (DialogResult.OK == MessageBox.Show("确定清空流程中的所有动作么？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                _methodFlow.Clear();
                UpdateFlow2UI();
            }


        }

        /// <summary>
        /// 清空信息栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        /// <summary>
        /// 单步调试动作流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStep_Click(object sender, EventArgs e)
        {
            if(null == _methodFlow)
            {
                MessageBox.Show("无效操作！动作流对象未设置");
                return;
            }
            if(_methodFlow.Count == 0)
            {
                MessageBox.Show("无效操作！流程中没有动作节点");
                return;
            }

            if (_methodFlow.CurrStep == _methodFlow.Count - 1)
            {
                MessageBox.Show("无效操作！当前动作流已执行到结尾");
                return;
            }

            if (_methodFlow.IsWorking() )
            {
                if(_methodFlow.RunningMode != JFMethodFlow.RunMode.Step)
                {
                    MessageBox.Show("正在连续运行，不能单步调试");
                    return;
                }
                JFWorkCmdResult ret = _methodFlow.Step();
                if (ret != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("发送单步指令失败，错误代码 = " + ret);
                    return;
                }

            }
            else //当前未运行
            {
                JFWorkCmdResult ret = _methodFlow.Step();
                if(ret != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("启动单步调试失败,错误代码:" + ret);
                    return;
                }
            }

          

        }

        /// <summary>
        /// 重置动作流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReset_Click(object sender, EventArgs e)
        {
            if (null == _methodFlow)
            {
                MessageBox.Show("无效操作！动作流未设置/空值");
                return;
            }

            if(_methodFlow.IsWorking())
            {
                MessageBox.Show("复位操作失败，动作流正在运行中！");
                return ;
            }

            _methodFlow.Reset();
            UpdatePoolValues();
            lbCurrStep.Text = "未执行";
            ShowTips("动作流已复位");
        }

        /// <summary>
        /// 执行方法流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAction_Click(object sender, EventArgs e)
        {
            if (null == _methodFlow)
            {
                MessageBox.Show("无效操作！动作流对象未设置");
                return;
            }
            if(0 == _methodFlow.Count)
            {
                MessageBox.Show("无效操作！动作流中没有方法节点");
                return;
            }
            JFWorkCmdResult ret = JFWorkCmdResult.UnknownError;
            if (_methodFlow.CurrWorkStatus == JFWorkStatus.Pausing)
            {
                ret = _methodFlow.Resume(2000);
                if(ret != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("恢复运行失败，错误代码 = " + ret);
                    return;
                }
                ShowTips("已恢复运行");
                return;
            }

            if (_methodFlow.CurrWorkStatus == JFWorkStatus.Running ||
                _methodFlow.CurrWorkStatus == JFWorkStatus.Interactiving)//当前正在运行
            {
                ShowTips("无效操作，工作流正在运行");
                return;
            }
            UpdatePoolValues();
            ret = _methodFlow.Start();
            if (ret != JFWorkCmdResult.Success)
            {
                MessageBox.Show(" 启动工作流失败,错误代码:" + ret);
                return;
            }
            
          
        }

        TreeNode currNode = null;
        private void trvMethodTypes_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                Point ClickPoint = new Point(e.X, e.Y);
                currNode = trvMethodRoot.GetNodeAt(ClickPoint);
                if (currNode != null)//判断你点的是不是一个节点
                {
                    //switch (CurrentNode.Name)//根据不同节点显示不同的右键菜单，当然你可以让它显示一样的菜单
                    //{
                    //    case "errorUrl":
                    //        CurrentNode.ContextMenuStrip = contextMenuStripErrorUrl;
                    //        break;
                    //}
                    //CurrentNode.SelectedNode = CurrentNode;//选中这个节点

                    if (null == currNode.Tag)
                        return;
                    if (!(currNode.Tag is Type))
                        return;
                    if (_methodFlow == null)
                    {
                        ShowTips("请先创建工作流对象！");
                        return;
                    }
                    if (_methodFlow.Count == 0)
                        ToolStripMenuItem_Insert.Enabled = false;
                    else
                        ToolStripMenuItem_Insert.Enabled = true;
                    toolStripComboBox_Index.Items.Clear();
                    for (int i = 0; i < _methodFlow.Count; i++)
                        toolStripComboBox_Index.Items.Add(i);
                    currNode.ContextMenuStrip = contextMenuStrip1;
                }
            }
        }

        /// <summary>
        /// 添加一个方法对象到动作流中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_Add_Click(object sender, EventArgs e)
        {
            Type methodType = currNode.Tag as Type;
            string dafaultNewName = _methodFlow.GetDefaultNameWillAddMethod(methodType);
            BenameDialog bnd = new BenameDialog();
            bnd.Text = "新方法节点命名";
            bnd.SetName(dafaultNewName);
            if (DialogResult.OK != bnd.ShowDialog())
                return;
            IJFMethod newMethod = JFMethodFlow.CreateMethod(methodType.Name);
            bool isOK = _methodFlow.Add(newMethod, bnd.GetName());
            if (isOK)
            {
                ToolStripMenuItem_Insert.Enabled = true;
                ShowTips("添加动作节点成功！");
                UpdateFlow2UI();
            }
            else
            {
                MessageBox.Show("添加动作节点失败，请检查动作名称是否已存在于流程中!");
                return;
            }
            

        }

        private void ToolStripMenuItem_Insert_Click(object sender, EventArgs e)
        {
            int idx = toolStripComboBox_Index.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("请选择动作节点序号");
                return;
            }

            Type methodType = currNode.Tag as Type;
            string dafaultNewName = _methodFlow.GetDefaultNameWillAddMethod(methodType);
            BenameDialog bnd = new BenameDialog();
            bnd.Text = "新方法节点命名";
            bnd.SetName(dafaultNewName);
            if (DialogResult.OK != bnd.ShowDialog())
                return;
            IJFMethod newMethod = JFMethodFlow.CreateMethod(methodType.Name);
            bool isOK = _methodFlow.Insert(newMethod, idx, bnd.GetName());
            if (isOK)
            {
                ShowTips("插入动作节点成功！");
                UpdateFlow2UI();
            }
            else
            {
                MessageBox.Show("插入动作节点失败，请检查\n节点序号是否超限/动作名称是否已存在于流程中!");
                return;
            }


        }
        /// <summary>
        /// 将方法实时界面显示到Panel中
        /// </summary>
        /// <param name="ui"></param>
        void UpdateMethodUi(int stepIndex)
        {
            pnRealtimeUI.Controls.Clear();
            if (_lstRtUi[stepIndex] == null)
                return;
            pnRealtimeUI.Controls.Add(_lstRtUi[stepIndex]);
            _lstRtUi[stepIndex].Dock = DockStyle.Fill;
            //SetTag(pnRealtimeUI);
            _lstRtUi[stepIndex].UpdateSrc2UI();
        }




        private void pnRealtimeUI_Resize(object sender, EventArgs e)
        {
            //SetTag(pnRealtimeUI);
            //float newx = (pnRealtimeUI.Width) / oldX;
            //float newy = (pnRealtimeUI.Height) / oldY;
            //SetControl(newx, newy, pnRealtimeUI);
            //if(null != pnRealtimeUI.Controls && pnRealtimeUI.Controls.Count > 0)
            //{
            //    if (pnRealtimeUI.Controls[0] is JFRealtimeUI)
            //        pnRealtimeUI.Controls[0].Size = pnRealtimeUI.Size;
            //}
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            if (JFWorkCmdResult.Success != _methodFlow.Stop(1000))
            {
                ShowTips("停止工作流失败，即将强行停止工作流线程！");
                _methodFlow.Abort();
            }     
        }

        private void btPause_Click(object sender, EventArgs e)
        {
            //if(_methodFlow.CurrWorkStatus == JFWorkStatus.AbortExit ||
            //    _methodFlow.CurrWorkStatus == JFWorkStatus.CommandExit ||
            //    _methodFlow.CurrWorkStatus == JFWorkStatus.ErrorExit ||
            //    _methodFlow.CurrWorkStatus == JFWorkStatus.ExceptionExit ||
            //    _methodFlow.CurrWorkStatus == JFWorkStatus.NormalEnd ||
            //    _methodFlow.CurrWorkStatus == JFWorkStatus.UnStart)
            //{
            //    MessageBox.Show("暂停操作失败，工作流当前未运行");
            //    return;
            //}

            JFWorkCmdResult ret = _methodFlow.Pause(2000);
            if(ret != JFWorkCmdResult.Success)
            {
                MessageBox.Show("暂停失败：ErrorCode = " + ret);
            }

        }
    }
}
