using JFInterfaceDef;
using JFToolKits;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace JFUI
{
    /// <summary>
    ///  用于显示
    /// </summary>
    public partial class FormXCfgEdit : Form
    {
        public FormXCfgEdit()
        {
            InitializeComponent();
            AllowAddTypes = new List<Type>() {
                typeof(int),typeof(double),typeof(bool),typeof(string),
                typeof(int[]),typeof(double[]),typeof(bool[]),typeof(string[]),
                typeof(List<int>),typeof(List<double>),typeof(List<bool>),typeof(List<string>)
            };
        }

        [Category("JF属性"), Description("允许添加配置项"), Browsable(true)]
        public bool AllowedAddItem
        {
            get { return btAddItem.Visible; }
            set { btAddItem.Visible = value; }
        }


        [Category("JF属性"), Description("允许删除配置项"), Browsable(true)]
        public bool AllowedDeleteItem
        {
            get { return btDeletItem.Visible; }
            set { btDeletItem.Visible = value; }
        }

        [Category("JF属性"), Description("允许另存为"), Browsable(true)]
        public bool AllowedSaveAs
        {
            get { return btSaveAs.Visible; }
            set { btSaveAs.Visible = value; }
        }


        bool isShowUnnameTag = true;
        [Category("JF属性"), Description("显示无名Tag"), Browsable(true)]
        public bool AllowedShowUnnameTag
        {
            get { return isShowUnnameTag; }
            set { isShowUnnameTag = value; }
        }






        /// <summary>
        /// 允许新添加数据项的类型
        /// </summary>
        public List<Type> AllowAddTypes { get; private set; }

        public void SetCfg(JFXCfg cfg)
        {
            _cfg = cfg;
            if(_isFormLoaded)
                AdjustStationView();
        }

        JFXCfg _cfg = null;//JFStationBase _station = null;
        bool _isFormLoaded = false;
        private void FormStationCfgParam_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustStationView();
        }

        bool isEditting = false;
        void AdjustStationView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustStationView));
                return;
            }
            tabControlCF1.TabPages.Clear();
            isEditting = false;
            btEditSave.Text = "编辑";
            btCancel.Enabled = false;
            if(null == _cfg)
            {
                lbInfo.Text = "无配置项！";
                btEditSave.Enabled = false;
                return;
            }
            lbInfo.Text = "文件路径:" + _cfg.FilePath;
            string[] namedCategorys = _cfg.AllTags;
            //if(null == categorys || categorys.Length < 2) //只有一个无名称Tag，由于保存私有配置
            //{
            //    lbInfo.Text += " 无定制化参数";
            //    btEditSave.Enabled = false;
            //    return;
            //}
            List<string> categorys = new List<string>();
            if (!AllowedShowUnnameTag)
            {
                if(null == categorys || categorys.Count < 2) //只有一个无名称Tag，由于保存私有配置
                {
                    lbInfo.Text += " 无定制化参数";
                    btEditSave.Enabled = false;
                    return;
                }
            }
            else
                categorys.Add("");
            if (namedCategorys != null)
                categorys.AddRange(namedCategorys);
            btEditSave.Enabled = true;
            foreach (string category in categorys)
            {
                if (string.IsNullOrEmpty(category))
                    continue;
                TabPage tp = new TabPage(category);
                tabControlCF1.TabPages.Add(tp);
                string[] itemNames = _cfg.ItemNamesInTag(category);
                if (null == itemNames)
                    continue;
                TableLayoutPanel panel = new TableLayoutPanel();
                //panel.RowStyles[0] = new RowStyle(SizeType.Absolute, 55);
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                panel.ColumnCount = 1;
                panel.AutoScroll = true;
                panel.Dock = DockStyle.Fill;
                tp.Controls.Add(panel);
                for(int i = 0; i < itemNames.Length;i++)//foreach (string itemName in itemNames)
                {
                    string itemName = itemNames[i];
                    UcJFParamEdit ucParam = new UcJFParamEdit();
                    ucParam.IsHelpVisible = false;
                    ucParam.Height = 50;
                    ucParam.Width = 600;//panel.Width*2/3;
                    //ucParam.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right ;
                    //ucParam.SetParamType(_cfg.GetItemValue(itemName).GetType());//ucParam.SetParamDesribe(_station.GetCfgParamDescribe(itemName));
                    ucParam.SetParamDesribe(JFParamDescribe.Create(itemName, _cfg.GetItemValue(itemName).GetType(), JFValueLimit.NonLimit, null));
                    ucParam.SetParamValue(_cfg.GetItemValue(itemName));
                    ucParam.IsValueReadOnly = false;
                    panel.Controls.Add(ucParam);
                    ucParam.IsValueReadOnly = true;
                    //panel.RowStyles[i].SizeType = SizeType.Absolute;
                    //panel.RowStyles[i].Height = 55;
                }
            }
            //if (tabControlCF1.TabCount > 0)
            //    tabControlCF1.SelectedIndex = 0;


        }

        private void FormStationCfgParam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 编辑/保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditSave_Click(object sender, EventArgs e)
        {
            if (tabControlCF1.SelectedIndex < 0)
                return;
            if(!isEditting)
            {
                TableLayoutPanel panel = tabControlCF1.SelectedTab.Controls[0] as TableLayoutPanel;
                if(0 == panel.Controls.Count)
                {
                    MessageBox.Show("没有可供编辑的参数项"); 
                    return;
                }
                foreach (UcJFParamEdit ucParam in panel.Controls)
                    ucParam.IsValueReadOnly = false;
                isEditting = true;
                btEditSave.Text = "保存";
                btCancel.Enabled = true;
                btSaveAs.Enabled = true;
                btAddItem.Enabled = true;
                btDeletItem.Enabled = true;
            }
            else
            {
                TableLayoutPanel panel = tabControlCF1.SelectedTab.Controls[0] as TableLayoutPanel;
                foreach (UcJFParamEdit ucParam in panel.Controls)
                {
                    string paramName = ucParam.GetParamDesribe().DisplayName;
                    object paramVal;
                    if(!ucParam.GetParamValue(out paramVal))
                    {
                        MessageBox.Show("未能获取参数 Name = " + paramName);
                        return;
                    }
                    //_station.SetCfgParamValue(paramName, paramVal);
                    //ucParam.IsValueReadOnly = false;
                }

                foreach (UcJFParamEdit ucParam in panel.Controls)
                {
                    string paramName = ucParam.GetParamDesribe().DisplayName;
                    object paramVal;
                    ucParam.GetParamValue(out paramVal) ;
                    _cfg.SetItemValue(paramName, paramVal);//_station.SetCfgParamValue(paramName, paramVal);
                    ucParam.IsValueReadOnly = true;
                }
                _cfg.Save();

                isEditting = false;
                btEditSave.Text = "编辑";
                btCancel.Enabled = false;
                btSaveAs.Enabled = false;
                btAddItem.Enabled = false;
                btDeletItem.Enabled = false;
            }
        }

        /// <summary>
        /// 将当前所选页的工站配置更新到界面上
        /// </summary>
        void UpdateCurrPage(bool isReload,bool enabled = false)
        {
            TabPage currTP = tabControlCF1.SelectedTab;
            if (null == currTP)
                return;
            TableLayoutPanel currPanel = currTP.Controls[0] as TableLayoutPanel;
            if(isReload)
            {
                string category = currTP.Text;
                currPanel.Controls.Clear();
                string[] itemNames = _cfg.ItemNamesInTag(category);
                if (null == itemNames)
                    return;
                foreach(string itemName in itemNames)
                {
                    UcJFParamEdit ucParam = new UcJFParamEdit();
                    ucParam.IsHelpVisible = false;
                    ucParam.Height = 50;
                    ucParam.Width = 600;
                    ucParam.SetParamDesribe(JFParamDescribe.Create(itemName, _cfg.GetItemValue(itemName).GetType(), JFValueLimit.NonLimit, null));
                    currPanel.Controls.Add(ucParam);
                }

            }
            foreach(UcJFParamEdit ucParam in currPanel.Controls)
            {
                //object paramVal = _station.GetCfgParamValue(ucParam.GetParamDesribe().DisplayName);
                object paramVal = _cfg.GetItemValue(ucParam.GetParamDesribe().DisplayName);
                ucParam.SetParamValue(paramVal);
                ucParam.IsValueReadOnly = !enabled;
            }
        }



        /// <summary>
        /// 取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            //_station.LoadCfg();
            //AdjustStationView();
            UpdateCurrPage(true);
            isEditting = false;
            btCancel.Enabled = false;
            btSaveAs.Enabled = false;
            btAddItem.Enabled = false;
            btDeletItem.Enabled = false;
            btEditSave.Text = "编辑";
        }

        public void FormStationCfgParam_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {

            }
            else
            {
                if(isEditting)
                {
                    if (DialogResult.OK == MessageBox.Show("退出窗口前是否保存当前变更？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        TableLayoutPanel panelEditted = tabControlCF1.SelectedTab.Controls[0] as TableLayoutPanel;
                        foreach (UcJFParamEdit ucParam in panelEditted.Controls)
                        {
                            string paramName = ucParam.GetParamDesribe().DisplayName;
                            object paramVal;
                            ucParam.GetParamValue(out paramVal);
                            _cfg.SetItemValue(paramName, paramVal);//_station.SetCfgParamValue(paramName, paramVal);
                            ucParam.IsValueReadOnly = true;
                        }
                        _cfg.Save();//_station.SaveCfg();
                        btEditSave.Text = "编辑";
                        btCancel.Enabled = false;
                        //isEditting = false;
                    }
                    else
                        UpdateCurrPage(false);

                    isEditting = false;
                }
            }
        }

        private void tabControlCF1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (isEditting)
            {
                MessageBox.Show("当前页正处于编辑状态，请先保存或取消");
                e.Cancel = true;
                return;
            }
        }


        private void btAddItem_Click(object sender, EventArgs e)
        {
            if (null == _cfg)
                return;
            if(tabControlCF1.SelectedIndex < 0)
            {
                MessageBox.Show("请在右侧Tab栏选择数据项类别！");
                return;
            }
            FormXCfgItemEdit fmAddItem = new FormXCfgItemEdit();
            fmAddItem.SetItemAllowedTypes(AllowAddTypes.ToArray());
            if(DialogResult.OK == fmAddItem.ShowDialog())
            {
                string itemName = fmAddItem.GetItemName();
                if(_cfg.ContainsItem(itemName))
                {
                    MessageBox.Show("已包含同名配置项，不能重复添加");
                    return;
                }
                string itemTag = tabControlCF1.SelectedTab.Text;
                object itemVal = fmAddItem.GetItemValue();

                _cfg.AddItem(itemName, itemVal, itemTag);
                _cfg.Save();
                UpdateCurrPage(true);

            }

        }

        /// <summary>
        /// 删除数据项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeletItem_Click(object sender, EventArgs e)
        {
            if (null == _cfg)
                return;
            TabPage currTP = tabControlCF1.SelectedTab;
            if (null == currTP)
                return;
            string category = currTP.Text;
            string[] itemNames = _cfg.ItemNamesInTag(category);
            if (null == itemNames || 0 == itemNames.Length)
            {
                MessageBox.Show("没有可供删除的数据项");
                return;
            }
            FormSelectStrings fm = new FormSelectStrings();
            fm.Text = "选择待删除项";
            fm.SetSelectStrings(itemNames);
            if(DialogResult.OK == fm.ShowDialog())
            {
                string[] delItems = fm.GetSelectedStrings();
                if(DialogResult.OK ==MessageBox.Show("确定要删除以下数据项?\n" + string.Join("\n", itemNames),"删除警告",MessageBoxButtons.OKCancel))
                {
                    foreach (string delItem in delItems)
                        _cfg.RemoveItem(delItem);
                }
                _cfg.Save();
                UpdateCurrPage(true);
            }

        }

        /// <summary>
        /// 当前配置另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveAs_Click(object sender, EventArgs e)
        {
            if (_cfg == null)
                return;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML文件(*.xml;*.XML)|*.xml;*.XML"; //设置文件类型
            sfd.FileName = "保存";//设置默认文件名
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _cfg.Save(sfd.FileName);
                MessageBox.Show("已保存为:" + sfd.FileName);
            }
        }
    }
}
