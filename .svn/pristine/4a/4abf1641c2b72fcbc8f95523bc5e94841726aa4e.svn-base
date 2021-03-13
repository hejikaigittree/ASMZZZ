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

namespace JFHub
{

    
    public partial class UcMethodNode : UserControl
    {
        internal JFDelegateShowInfo EventShowInfo;
        public UcMethodNode()
        {
            InitializeComponent();
        }

        private void UcMethodNode_Load(object sender, EventArgs e)
        {
            isFormLoaded = true;
            AdjustView();
        }

        bool isFormLoaded = false;
        JFMethodFlow.MethodItem methodItem = null;
        List<ComboBox> cbInputIDs = new List<ComboBox>();//输入参数编辑控件
        List<TextBox> tbOutputIDs = new List<TextBox>();//输出参数编辑控件
        string[] availableIDs = null;
        bool isBindEditting = false;
        
        bool IsBindEditting
        {
            get { return isBindEditting; }
            set
            {
                isBindEditting = value;
                btBindEdit.Text = isBindEditting ? "保存绑定" : "编辑绑定";
                btBindCancel.Enabled = isBindEditting;
                foreach (ComboBox cbInputID in cbInputIDs)
                    cbInputID.Enabled = isBindEditting;
                
                //foreach(TextBox tbOutputID in tbOutputIDs)//输出参数ID不可编辑
                //{
                //    tbOutputID.ReadOnly = !isBindEditting;
                //    tbOutputID.BackColor = isBindEditting ? Color.White : SystemColors.Control;
                //}
            }
        }
        public void SetMethodItem(JFMethodFlow.MethodItem item)
        {
            methodItem = item;
            if (isFormLoaded)
                AdjustView();
        }


        void AdjustView()
        {
            gbIn.Controls.Clear();
            gbOut.Controls.Clear();
            IsBindEditting = false;
            if (null == methodItem)
            {
                gbIn.Height = 20;   
                gbOut.Top = gbIn.Bottom + 1;
                gbOut.Height = 20;
                btBindEdit.Enabled = false;
                btUI.Enabled = false;
                lbMethodName.Text = "未设置";
                Height = gbOut.Bottom + 1;
                return;
            }
            lbMethodName.Text = methodItem.Name;
            btBindEdit.Enabled = true;
            btUI.Enabled = true;
            string[] inParamNames = methodItem.Value.MethodInputNames;
            if(null == inParamNames || 0 == inParamNames.Length)
                gbIn.Height = 20;
            else //添加输入参数
            {
                int locX = 3;
                int locY = 15;
                for(int i = 0;i < inParamNames.Length;i++)
                {
                    TextBox tbInputName = new TextBox();
                    tbInputName.Location = new Point(locX, locY);
                    tbInputName.Width = 90;
                    tbInputName.ReadOnly = true;
                    tbInputName.BackColor = SystemColors.Control;
                    tbInputName.Text = inParamNames[i];
                    gbIn.Controls.Add(tbInputName);
                    ComboBox cbInputID = new ComboBox();
                    cbInputID.Location = new Point(tbInputName.Right + 1, locY);
                    cbInputID.Width = gbIn.Width - cbInputID.Left - 1;
                    cbInputID.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;//cbInputID.Dock = DockStyle.Top | DockStyle.Left | DockStyle.Right;
                    
                    if (null != availableIDs && availableIDs.Length > 0)
                        cbInputID.Items.AddRange(availableIDs);
                    cbInputID.Text = methodItem.InputID(inParamNames[i]);
                    cbInputID.SelectedIndexChanged += OnComboSelectIndexChanged_InputID;
                    cbInputID.Enabled = false;
                    gbIn.Controls.Add(cbInputID);
                    cbInputIDs.Add(cbInputID);
                    locY = tbInputName.Bottom + 1;
                    
                }
                gbIn.Height = locY;
            }
            gbOut.Top = gbIn.Bottom + 1;
            string[] outParamNames = methodItem.Value.MethodOutputNames;
            if(null != outParamNames && outParamNames.Length > 0)
            {
                int locX = 3;
                int locY = 15;
                for (int i = 0; i < outParamNames.Length; i++)
                {
                    TextBox tbOutputName = new TextBox();
                    tbOutputName.Location = new Point(locX, locY);
                    tbOutputName.Width = 90;
                    tbOutputName.ReadOnly = true;
                    tbOutputName.BackColor = SystemColors.Control;
                    tbOutputName.Text = outParamNames[i];
                    gbOut.Controls.Add(tbOutputName);
                    TextBox tbOutputID = new TextBox();
                    tbOutputID.Location = new Point(tbOutputName.Right + 1, locY);
                    tbOutputID.Width = gbIn.Width - tbOutputID.Left - 1;
                    tbOutputID.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;//tbOutputID.Dock = DockStyle.Top | DockStyle.Left | DockStyle.Right;
                    tbOutputID.Text = methodItem.OutputID(outParamNames[i]);
                    tbOutputID.Enabled = false;
                    gbOut.Controls.Add(tbOutputID);
                    tbOutputIDs.Add(tbOutputID);
                    locY = tbOutputID.Bottom + 1;
                    
                }
                gbOut.Height = locY + 1;
            }

            Height = gbOut.Bottom + 1;
        }

        delegate void dgUpdateAvailableIDs(string[] ids);
        /// <summary>更新可用的参数ID列表</summary>
        public void UpdateAvailableIDs(string[] ids)
        {
            if(InvokeRequired)
            {
                Invoke(new dgUpdateAvailableIDs(UpdateAvailableIDs), new object[] { ids });
                return;
            }
            
            foreach(ComboBox cb in cbInputIDs)
            {
                string txt = cb.Text;
                cb.Items.Clear();
                //methodItem.InnerAvailedIDs = ids;
                //if (null != ids && ids.Length > 0)
                //{
                //    if (methodItem.OutputNameIDs.Values == null || methodItem.OutputNameIDs.Values.Count == 0)
                //        cb.Items.AddRange(ids);
                //    else //排除自己的输出参数
                //    {
                //        foreach (string id in ids)
                //            if (!methodItem.OutputNameIDs.Values.Contains(id))
                //                cb.Items.Add(id);
                //    }
                //    cb.Text = txt;
                //}
                List<string> availedIDs = new List<string>();
                availedIDs.AddRange(methodItem.AvailedInputIDs);
                if (null != ids)
                    foreach (string id in ids)
                        if (!availedIDs.Contains(id))
                            availedIDs.Add(id);
                cb.Items.AddRange(availedIDs.ToArray()/*methodItem.AvailedInputIDs*/);
                cb.Text = txt;

            }
            
        }

        void OnComboSelectIndexChanged_InputID(object sender, EventArgs e)
        {

        }

        private void btUI_Click(object sender, EventArgs e) ///显示Method配置界面
        {
            if(null == methodItem)
            {
                ShowTips("无效操作，方法对象未设置！");
                return;
            }
            FormMethodUI fm = new FormMethodUI();
            fm.SetMethod(methodItem.Value);
            fm.ShowDialog();
        }

        private void btBindEdit_Click(object sender, EventArgs e)//编辑/保存绑定
        {
            if(!IsBindEditting)
            {
                IsBindEditting = true;
                return;
            }
            ///当前处于编辑模式
            if (UpdateView2IDs())
                IsBindEditting = false;
        }

        bool UpdateView2IDs()//将界面绑定数据更新到对象中
        {
            if (null == methodItem)
                return true;
            bool ret = true;
            do
            {
                string[] inParamNames = methodItem.Value.MethodInputNames;
                if (null != inParamNames && inParamNames.Length > 0)
                    for (int i = 0; i < inParamNames.Length; i++)
                    {
                        if (string.IsNullOrEmpty(cbInputIDs[i].Text))
                        {
                            MessageBox.Show("输入参数\"" + inParamNames[i] + "\"未绑定数据项！\n");
                            return false;
                        }
                        else
                        {
                            //类型转换判断
                            Type dstType = methodItem.Value.GetMethodInputType(inParamNames[i]);//方法的输入参数类型
                            Type srcType = null;
                            if (methodItem.TypePool.Keys.Contains(cbInputIDs[i].Text))
                                srcType = methodItem.TypePool[cbInputIDs[i].Text];//数池中变量的类型;
                            else
                                srcType = methodItem.OutterTypePool[cbInputIDs[i].Text];
                            if (dstType == srcType || JFTypeExt.IsImplicitFrom(dstType, srcType))//if (dstType == srcType ||dstType.IsAssignableFrom(srcType))//类型完全匹配
                                methodItem.SetInputID(inParamNames[i], cbInputIDs[i].Text);
                            else if(JFTypeExt.IsExplicitFrom(dstType, srcType))//可以进行强制转换
                            {
                                string msg = string.Format("参数项 {0}: 数据值类型\"{1}\" 需要强制转化为输入类型\"{2}\"\n是否继续绑定操作？", inParamNames[i], srcType.ToString(), dstType.ToString());
                                if (DialogResult.OK != MessageBox.Show(msg, "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                                    return false;
                                methodItem.SetInputID(inParamNames[i], cbInputIDs[i].Text);
                            }
                            else if(!JFTypeExt.IsExplicitFrom(dstType, srcType)) //不可以进行强制转化的类型
                            {
                                string msg = string.Format("参数项 {0}: 数据值类型\"{1}\" 无法强制转化为输入类型\"{2}\"", inParamNames[i], srcType.ToString(), dstType.ToString());
                                MessageBox.Show(msg);
                                return false;
                            }
                        }

                    }
            } while (false);
            return ret;
        }

        

        void UpdateIDs2View() //将当前的绑定关系显示到界面上
        {
            if (null == methodItem)
                return;
            string[] inParamNames = methodItem.Value.MethodInputNames;
            if (null != inParamNames && 0 != inParamNames.Length)
                for (int i = 0; i < inParamNames.Length; i++)
                    cbInputIDs[i].Text = methodItem.InputID(inParamNames[i]);
                

            
            string[] outParamNames = methodItem.Value.MethodOutputNames;
            if(null != outParamNames && outParamNames.Length > 0)
                for(int i = 0; i < outParamNames.Length;i++)
                    tbOutputIDs[i].Text = methodItem.OutputID(outParamNames[i]);
        }

        private void btBindCancel_Click(object sender, EventArgs e)//取消编辑
        {
            if (null == methodItem)
            {
                ShowTips("无效操作，方法对象未设置！");
                return;
            }
            if (!IsBindEditting)
                return;
            IsBindEditting = false;
            UpdateIDs2View();
        }

        private void btAction_Click(object sender, EventArgs e)//单步执行一次
        {
            if(null == methodItem)
            {
                ShowTips("无效操作！方法对象未设置");
                return;
            }
            try
            {
                bool isOK = methodItem.Action();
                if(isOK)
                {
                    ShowTips("\""+Name + "\"执行成功，耗时:" + methodItem.Value.GetActionSeconds().ToString("F3"));
                    return;
                }
                else
                    ShowTips(methodItem.ActionErrorInfo());
            }
            catch(Exception ex)
            {
                ShowTips("\"" + Name + "\"执行时异常:" + ex.ToString());
            }
        }

        void ShowTips(string info)
        {
            if (null == info)
                return;
            if (null == EventShowInfo)
            {
                MessageBox.Show(info);
                return;
            }
            EventShowInfo(this,info);
        }
    }
}
