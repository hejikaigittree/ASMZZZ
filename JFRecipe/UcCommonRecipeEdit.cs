using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFUI;
using JFInterfaceDef;

namespace JFRecipe
{

    /// <summary>
    /// 用于编辑JFCommonRecipe的控件
    /// </summary>
    public partial class UcCommonRecipeEdit : UserControl
    {
        public UcCommonRecipeEdit()
        {
            InitializeComponent();
        }



        private void UcCommonRecipeEdit_Load(object sender, EventArgs e)
        {
            tableLayoutPanel1.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutPanel1_CellPaint);
            AdjustRecipe2View();
        }

        JFCommonRecipe _recipe = null;
        List<UcJFParamEdit> _lstItems = new List<UcJFParamEdit>(); //数据项编辑控件
        List<Button> _lstDeleteItemButtons = new List<Button>(); //删除单个数据项的按钮


        public void SetRecipe(JFCommonRecipe recipe)
        {
            _recipe = recipe;
            if (Created)
                AdjustRecipe2View();
        }

        bool _isEditting = false;

        /// <summary>
        /// 
        /// </summary>
        public bool EditRecipe
        {
            get { return _isEditting; }
            set
            {
                if (null == _recipe)
                    return;
                _isEditting = value;
                btAddItem.Enabled = _isEditting;
                foreach (UcJFParamEdit paramEdit in _lstItems)
                    paramEdit.IsValueReadOnly = !_isEditting;
                foreach (Button bt in _lstDeleteItemButtons)
                    bt.Enabled = _isEditting;
            }
        }


        public void AdjustRecipe2View()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustRecipe2View));
                return;
            }

            

            _lstItems.Clear();
            _lstDeleteItemButtons.Clear();
            tableLayoutPanel1.Controls.Clear();
            EditRecipe = false;
            if (null == _recipe)
            {
                lbTips.Text = "Recipe未设置";
                return;
            }

            lbTips.Text = "Categoty:" + _recipe.Categoty + " ID:" + _recipe.ID;
            string[] itemNames = _recipe.AllItemNames();
            if (null == itemNames || 0 == itemNames.Length)
                return;
            foreach(string itemName in itemNames)
            {
                UcJFParamEdit pe = new UcJFParamEdit();
                //pe.SetParamType(_recipe.GetItemValue(itemName).GetType());
                pe.SetParamDesribe(JFParamDescribe.Create(itemName, _recipe.GetItemValue(itemName).GetType(), JFValueLimit.NonLimit, null));
                pe.SetParamValue(_recipe.GetItemValue(itemName));
                pe.IsValueReadOnly = true;
                pe.Height = 23;
                pe.Width = 500;
                pe.IsHelpVisible = false;
                tableLayoutPanel1.Controls.Add(pe);
                Button btDel = new Button();
                btDel.Text = "删除";
                btDel.Tag = itemName;
                btDel.Click += OnDelButtonClick;
                _lstItems.Add(pe);
                _lstDeleteItemButtons.Add(btDel);
                tableLayoutPanel1.Controls.Add(btDel);

            }
            EditRecipe = false;

        }


        /// <summary>
        /// 为Recipe添加一个配置项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAddItem_Click(object sender, EventArgs e)
        {
            FormXCfgItemEdit fmGenItem = new FormXCfgItemEdit();
            fmGenItem.SetItemAllowedTypes(new Type[]
            { typeof(int), typeof(double), typeof(string),typeof(bool),typeof(PointF), 
              typeof(List<int>),typeof(List<double>),typeof(List<string>),typeof(List<bool>),typeof(List<PointF>)});
            if (DialogResult.OK == fmGenItem.ShowDialog())
            {
                string[] allItemNames = _recipe.AllItemNames();
                if(null != allItemNames && allItemNames.Contains(fmGenItem.GetItemName()))
                {
                    MessageBox.Show("添加数据项失败，已包含数据项:" + fmGenItem.GetItemName());
                    return;
                }
                _recipe.AddItem(fmGenItem.GetItemName(), fmGenItem.GetItemValue());
                UcJFParamEdit pe = new UcJFParamEdit();
                //if (fmGenItem.GetItemType().IsPrimitive)
                    pe.Height = 23;
                pe.Width = 500;
                pe.IsHelpVisible = false;

                pe.SetParamDesribe(JFParamDescribe.Create(fmGenItem.GetItemName(), fmGenItem.GetItemType(), JFValueLimit.NonLimit, null));//pe.SetParamType(fmGenItem.GetItemType());
                pe.SetParamValue(fmGenItem.GetItemValue());
                pe.IsValueReadOnly = false; //添加后状态为可编辑
                tableLayoutPanel1.Controls.Add(pe);
                Button btDel = new Button();
                btDel.Text = "删除";
                btDel.Tag = fmGenItem.GetItemName();
                btDel.Click += OnDelButtonClick;
                tableLayoutPanel1.Controls.Add(btDel);
                _lstItems.Add(pe);
                _lstDeleteItemButtons.Add(btDel);
            }
        }

        void OnDelButtonClick(object sender, EventArgs e)
        {
            Button bt = sender as Button;
            string itemName = bt.Tag as string;
            if(DialogResult.OK == MessageBox.Show("确定要删除数据项:" + itemName,"删除警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning))
            {
                _recipe.RemoveItem(itemName);
                for(int i = 0; i < _lstDeleteItemButtons.Count;i++)
                {
                    if(_lstDeleteItemButtons[i] == bt)
                    {
                        _lstDeleteItemButtons.RemoveAt(i);
                        _lstItems.RemoveAt(i);
                        tableLayoutPanel1.Controls.RemoveAt(i * 2);
                        tableLayoutPanel1.Controls.RemoveAt(i * 2);
                    }
                }
            }
        }


        void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle r = e.CellBounds;
            if (e.Row %2 == 0)
                g.FillRectangle(new SolidBrush(SystemColors.Control), r);//g.FillRectangle(Brushes.Blue, r);

        }
    }
}
