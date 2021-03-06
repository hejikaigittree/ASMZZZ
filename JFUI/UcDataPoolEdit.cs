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

namespace JFUI
{
    /// <summary>
    /// 用于显示/编辑数据池变量
    /// </summary>
    public partial class UcDataPoolEdit : UserControl
    {
        public UcDataPoolEdit()
        {
            InitializeComponent();
        }

        IJFDataPool _dataPool = null;
        bool _isFormLoaded = false;
        List<UcJFParamEdit> _lstUcSingleItems = new List<UcJFParamEdit>(); //用于显示单值的控件
        List<CheckBox> _lstChkSingleDisableUpdates = new List<CheckBox>();//单值控件是否停更
        List<Button> _lstBtSingleWrites = new List<Button>(); //单值控件写入

        List<UcJFParamEdit> _lstUcCollectItems = new List<UcJFParamEdit>(); //用于显示集合值的控件
        List<CheckBox> _lstChkCollectDisableUpdates = new List<CheckBox>();// 集合值控件是否停更
        List<Button> _lstBtCollectWrites = new List<Button>(); //集合值控件写入

        private void UcDataPoolEdit_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustPoolItem();
        }

        /// <summary>
        /// 设置数据池对象
        /// </summary>
        /// <param name="dataPool"></param>
        public void SetDataPool(IJFDataPool dataPool)
        {
            _dataPool = dataPool;
            if (_isFormLoaded)
            {
               timerFlush.Enabled = false;
                AdjustPoolItem();
            }
        }



        /// <summary>
        /// 将数据池项更新到界面上
        /// </summary>
        void AdjustPoolItem()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustPoolItem));
                return;
            }
            bool isTimerFlushEnabled = timerFlush.Enabled;
            timerFlush.Enabled = false;
            chkAutoUpdate.Checked = false;
            panelSingleItems.Controls.Clear();
            panelCollectionItems.Controls.Clear();
            _lstUcSingleItems.Clear();
            _lstChkSingleDisableUpdates.Clear();
            _lstBtSingleWrites.Clear();

            _lstUcCollectItems.Clear();
            _lstChkCollectDisableUpdates.Clear();
            _lstBtCollectWrites.Clear();

            if(null == _dataPool)
            {
                lbInfo.Text = "数据池未设置";
                btAdjust.Enabled = false;
                btRead.Enabled = false;
                btAdjust.Enabled = false;
                chkAutoUpdate.Enabled = false;
                return;
            }
            lbInfo.Text = "";
            btAdjust.Enabled = true;
            btRead.Enabled = true;
            btAdjust.Enabled = true;
            chkAutoUpdate.Enabled = true;

            string[] singleItemNames = _dataPool.AllItemKeys;
            if(null != singleItemNames)
            {
                foreach(string itemName in singleItemNames)
                {
                    Type t = _dataPool.GetItemType(itemName);
                    object val;
                    bool isOK = _dataPool.GetItemValue(itemName, out val);
                    if (!isOK) //测试中才会用到
                        MessageBox.Show("获取SingleItem值失败，ItemName = " + itemName);
                    UcJFParamEdit uc = new UcJFParamEdit();
                    uc.Height = 23;
                    uc.IsHelpVisible = false;
                    uc.SetParamDesribe(JFParamDescribe.Create(itemName, t, JFValueLimit.NonLimit, null));
                    uc.SetParamValue(val);
                    CheckBox chkDisableUpdate = new CheckBox();
                    chkDisableUpdate.Text = "停更";
                    Button btSingleWrite = new Button();
                    btSingleWrite.Text = "写入";
                    btSingleWrite.Click += OnSingleItemWriteButtonClick;
                    _lstUcSingleItems.Add(uc);
                    _lstChkSingleDisableUpdates.Add(chkDisableUpdate);
                    _lstBtSingleWrites.Add(btSingleWrite);
                    panelSingleItems.Controls.Add(uc);
                    panelSingleItems.Controls.Add(chkDisableUpdate);
                    panelSingleItems.Controls.Add(btSingleWrite);
                }
                TableLayoutRowStyleCollection styles = panelSingleItems.RowStyles;
                foreach (RowStyle style in styles)
                {
                    // Set the row height to 20 pixels.
                    style.SizeType = SizeType.Absolute;
                    style.Height = 25;
                }
            }


            string[] collectItemNames = _dataPool.AllListKeys;
            if (null != collectItemNames)
            {
                foreach (string itemName in collectItemNames)
                {
                    Type elementType = _dataPool.GetListElementType(itemName);
                    Type lstType = typeof(List<>).MakeGenericType(elementType); //动态获取数组类型
                    /*List<object>*/object itemVal = _dataPool.LockList(itemName);
                    _dataPool.UnlockList(itemName);
                    //var itemValRealy = JFConvertExt.ChangeType(itemVal, lstType);
                    

                    UcJFParamEdit uc = new UcJFParamEdit();
                    uc.Height = 50;
                    uc.IsHelpVisible = false;
                    uc.SetParamDesribe(JFParamDescribe.Create(itemName, lstType, JFValueLimit.NonLimit, null));
                    uc.SetParamValue(itemVal);
                    uc.SetParamValue(null);
                    CheckBox chkDisableUpdate = new CheckBox();
                    chkDisableUpdate.Text = "停更";
                    Button btCollectWrite = new Button();
                    btCollectWrite.Text = "写入";
                    btCollectWrite.Click += OnCollectItemWriteButtonClick;
                    _lstUcCollectItems.Add(uc);
                    _lstChkCollectDisableUpdates.Add(chkDisableUpdate);
                    _lstBtCollectWrites.Add(btCollectWrite);
                    panelCollectionItems.Controls.Add(uc);
                    panelCollectionItems.Controls.Add(chkDisableUpdate);
                    panelCollectionItems.Controls.Add(btCollectWrite);
                }
                TableLayoutRowStyleCollection styles = panelCollectionItems.RowStyles;
                foreach (RowStyle style in styles)
                {
                    // Set the row height to 20 pixels.
                    style.SizeType = SizeType.Absolute;
                    style.Height = 55;
                }

            }

            chkAutoUpdate.Checked = false;


            if (isTimerFlushEnabled)
            {
                chkAutoUpdate.Checked = true;
                timerFlush.Enabled = true;
            }
            else
                chkAutoUpdate.Checked = false;

        }

        /// <summary>
        /// 更新数据池中的值到界面上
        /// </summary>
        void UpdatePoolValue()
        {
            if (_dataPool == null)
                return;
            for (int i = 0; i < _lstUcSingleItems.Count; i++)
                if (!_lstChkSingleDisableUpdates[i].Checked)
                {
                    string itemName = _lstUcSingleItems[i].GetParamDesribe().DisplayName;
                    object val;
                    _dataPool.GetItemValue(itemName, out val);
                    _lstUcSingleItems[i].SetParamValue(val);
                }

            for(int i = 0; i <_lstUcCollectItems.Count;i++)
                if(!_lstChkCollectDisableUpdates[i].Checked)
                {
                    string itemName =_lstUcCollectItems[i].GetParamDesribe().DisplayName;
                    object val = _dataPool.LockList(itemName);
                    _dataPool.UnlockList(itemName);
                    _lstUcCollectItems[i].SetParamValue(val);
                }



        }

        /// <summary>
        /// 更新数据项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAdjust_Click(object sender, EventArgs e)
        {
            AdjustPoolItem();
        }

        /// <summary>
        /// 是否自动更新数据值到界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAutoUpdate.Checked) //开始自动更新数据值到界面
            {
                timerFlush.Enabled = true;
                btRead.Enabled = false;
            }
            else
            {
                timerFlush.Enabled = false;
                btRead.Enabled = true;
            }
        }

        public bool AutoUpdateDataPool
        {
            get { return chkAutoUpdate.Checked; }
            set { chkAutoUpdate.Checked = value; }
        }

        /// <summary>
        /// 读取数据值按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRead_Click(object sender, EventArgs e)
        {
            UpdatePoolValue();
        }

        /// <summary>
        /// 写入按钮,将所有变量值写入数据池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btWrite_Click(object sender, EventArgs e)
        {
            StringBuilder sbErrorInfo = new StringBuilder();
            bool isOK = true;

            ///先写入所有单值 项
            for(int i = 0;i < _lstUcSingleItems.Count;i++)
            {
                object val = null;
                UcJFParamEdit pe = _lstUcSingleItems[i];
                if(!pe.GetParamValue(out val))
                {
                    isOK = false;
                    sbErrorInfo.AppendLine(pe.GetParamDesribe().DisplayName + " 参数格式错误");
                }
                else
                {
                    if (!_dataPool.SetItemValue(pe.GetParamDesribe().DisplayName, val))
                    {
                        isOK = false;
                        sbErrorInfo.AppendLine(pe.GetParamDesribe().DisplayName + " 写入失败");
                    }
                }

            }

            ///再写入所有集合项
            //待添加代码


            if(!isOK)
            {
                JFTipsDelayClose.Show(sbErrorInfo.ToString(), -1);
                return;
            }
            JFTipsDelayClose.Show("写入成功",1);
        }

        /// <summary>
        /// 刷新控件值定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerFlush_Tick(object sender, EventArgs e)
        {
            UpdatePoolValue();
        }

        /// <summary>
        /// 单个单值数据项写入按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSingleItemWriteButtonClick(object sender,EventArgs e)
        {
            int idx = _lstBtSingleWrites.IndexOf(sender as Button);
            object val = null;
            if(!_lstUcSingleItems[idx].GetParamValue(out val))
            {
                JFTipsDelayClose.Show("未能写入数据池\n数据格式错误！", -1);
                return;
            }
            bool isOK = _dataPool.SetItemValue(_lstUcSingleItems[idx].GetParamDesribe().DisplayName, val);
            if(!isOK)
            {
                JFTipsDelayClose.Show("向数据池写入时发生错误!", -1);
                return;
            }
            JFTipsDelayClose.Show("写入成功", 1);
        }


        /// <summary>
        /// 单个集合数据项写入按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectItemWriteButtonClick(object sender, EventArgs e)
        {

        }


    }
}
