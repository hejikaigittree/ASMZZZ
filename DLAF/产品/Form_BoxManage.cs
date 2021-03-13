using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HT_Lib;

namespace DLAF
{
    public partial class Form_BoxManage : Form
    {
       // public static BoxManager BoxMger = new BoxManager();
        public static Form_BoxManage Instance;
        public Form_BoxManage()
        {
            InitializeComponent();
            BoxManager.Instance.Load();
            RefreshList();
            Instance = this;          
        }
        
        /// <summary>
        /// 刷新料盒列表
        /// </summary>
        public void RefreshList()
        {
            listBox_Box.Items.Clear();
            foreach(var pair in BoxManager.Dir_Boxes)
            {
                listBox_Box.Items.Add(pair.Value.Name);
            }
            if (BoxManager.Dir_Boxes.Count > 0) listBox_Box.SelectedIndex = 0;
        }
        private void listBox_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            foreach(var item in BoxManager.Dir_Boxes)
            {
                if (i== listBox_Box.SelectedIndex)
                {
                    propertyGrid_Box.SelectedObject = item.Value;
                    return;
                }
                i++;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Box ItemBox = new Box("Box_" + (BoxManager.Dir_Boxes.Count),
                    BoxManager.Dir_Boxes.Count == 0 ? 0 : BoxManager.Dir_Boxes.Last().Value.Idx + 1);
                BoxManager.Dir_Boxes.TryAdd(ItemBox.Idx, ItemBox);
                listBox_Box.Items.Add(BoxManager.Dir_Boxes.Last().Value.Name);
                if (BoxManager.Dir_Boxes.Count > 0) listBox_Box.SelectedIndex = BoxManager.Dir_Boxes.Count - 1;
            }
            catch
            {
                // HTUi.PopError("添加失败！\n");
                MessageBox.Show("添加失败！\n");
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {

            try
            {
                if (BoxManager.Dir_Boxes.Count > 0)
                {
                    Box selectBox = (Box)propertyGrid_Box.SelectedObject;
                    int selectBoxIdx = selectBox.Idx;
                    int selectIdx = listBox_Box.SelectedIndex;
                    listBox_Box.SelectedIndex = 0;
                    Box Item;
                    if (!BoxManager.Dir_Boxes.TryRemove(BoxManager.Dir_Boxes[selectBoxIdx].Idx, out Item))
                    {
                        MessageBox.Show("删除失败！");
                        return;
                    }
                    listBox_Box.Items.RemoveAt(selectIdx);
                }
                else
                {
                    MessageBox.Show("没有元素可供删除！\n");
                }
                if (BoxManager.Dir_Boxes.Count > 0) listBox_Box.SelectedIndex = BoxManager.Dir_Boxes.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除失败！\n" + ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BoxManager.Instance.Save();
            // frmProduct.Instance.prgProductMagzine.Refresh();
            MessageBox.Show("保存成功！");
        }

        private void btn_AddCopy_Click(object sender, EventArgs e)
        {
            try
            {
                propertyGrid_Box.Refresh();
                if (BoxManager.Dir_Boxes.Count == 0)
                {

                    MessageBox.Show("增加失败！\n");
                    return;
                }
                Box selectBox = (Box)propertyGrid_Box.SelectedObject;
                int selectBoxIdx = selectBox.Idx;
                int selectIdx = listBox_Box.SelectedIndex;
                Box ItemBox = BoxManager.Dir_Boxes[selectBoxIdx].Clone();
                ItemBox.Idx = BoxManager.Dir_Boxes.Last().Value.Idx + 1;
                ItemBox.Name = ItemBox.Name + "'s Copy";
                BoxManager.Dir_Boxes.TryAdd(ItemBox.Idx, ItemBox);
                listBox_Box.Items.Add(ItemBox.Name);
                if (BoxManager.Dir_Boxes.Count > 0) listBox_Box.SelectedIndex = BoxManager.Dir_Boxes.Count - 1;
            }
            catch
            {
                MessageBox.Show("添加失败！\n");
            }
        }
    }
}
