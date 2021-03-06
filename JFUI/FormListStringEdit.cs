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
    public partial class FormListStringEdit : Form
    {
        public List<string> Value { get; set; }

        public FormListStringEdit()
        {
            InitializeComponent();
        }


        //public FormListStringEdit(string[] value)
        //{
        //    InitializeComponent();
        //    if (null == value)
        //        Value = new List<string>();
        //    else
        //        Value = value.ToList();
        //}

        public FormListStringEdit(List<string> value)
        {
            InitializeComponent();
            if (value == null)
            {
                value = new List<string>();
            }
            Value = value;
        }

        private void FormListStringEdit_Load(object sender, EventArgs e)
        {
            this.listViewString.Items.Clear();
            if (this.Value != null && this.Value.Count > 0)
            {
                for (int i = 0; i < this.Value.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = i.ToString();
                    lvi.SubItems.Add(this.Value[i]);
                    this.listViewString.Items.Add(lvi);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(listViewString.SelectedItems.Count == 0) //Add it By Bob
            {
                MessageBox.Show("请先选择编辑项");
                return;
            }
            this.listViewString.SelectedItems[0].SubItems[1].Text = this.txtString.Text;
        }

        private void listViewString_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewString.SelectedItems != null && this.listViewString.SelectedItems.Count > 0)
            {
                this.txtString.Text = this.listViewString.SelectedItems[0].SubItems[1].Text;
            }
            else
            {
                this.txtString.Text = "";
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (this.listViewString.SelectedItems.Count == 0)
            {
                return;
            }
            this.listViewString.BeginUpdate();
            foreach (ListViewItem lvi in this.listViewString.SelectedItems)
            {
                ListViewItem lviSelectedItem = lvi;
                int indexSelectedItem = lvi.Index;
                if (indexSelectedItem == 0)
                {
                    break;
                }
                this.listViewString.Items.RemoveAt(indexSelectedItem);
                this.listViewString.Items.Insert(indexSelectedItem - 1, lviSelectedItem);
            }
            UpdateIndex();
            this.listViewString.EndUpdate();

            if (this.listViewString.Items.Count > 0 && this.listViewString.SelectedItems.Count > 0)
            {
                this.listViewString.Focus();
                this.listViewString.SelectedItems[0].Focused = true;
                this.listViewString.SelectedItems[0].EnsureVisible();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.listViewString.SelectedItems.Count == 0)
            {
                return;
            }
            this.listViewString.BeginUpdate();
            int indexMaxSelectedItem = this.listViewString.SelectedItems[this.listViewString.SelectedItems.Count - 1].Index;
            if (indexMaxSelectedItem < this.listViewString.Items.Count - 1)
            {
                for (int i = this.listViewString.SelectedItems.Count - 1; i >= 0; i--)
                {
                    ListViewItem lviSelectedItem = this.listViewString.SelectedItems[i];
                    int indexSelectedItem = lviSelectedItem.Index;
                    this.listViewString.Items.RemoveAt(indexSelectedItem);
                    this.listViewString.Items.Insert(indexSelectedItem + 1, lviSelectedItem);
                }
            }
            UpdateIndex();
            this.listViewString.EndUpdate();
            if (this.listViewString.Items.Count > 0 && this.listViewString.SelectedItems.Count > 0)
            {
                this.listViewString.Focus();
                this.listViewString.SelectedItems[this.listViewString.SelectedItems.Count - 1].Focused = true;
                this.listViewString.SelectedItems[this.listViewString.SelectedItems.Count - 1].EnsureVisible();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.listViewString.SelectedItems.Count > 0)
            {
                ListViewItem lviSelectedItem = this.listViewString.SelectedItems[0];
                int indexSelectedItem = lviSelectedItem.Index;
                this.listViewString.Items.RemoveAt(indexSelectedItem);
                UpdateIndex();
            }
        }

        private void UpdateIndex()
        {
            for (int i = 0; i < this.listViewString.Items.Count; i++)
            {
                this.listViewString.Items[i].Text = i.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string strTxt = this.txtString.Text;
            int index = this.listViewString.Items.Count;
            ListViewItem lvi = new ListViewItem();
            lvi.Text = index.ToString();
            lvi.SubItems.Add(strTxt);
            this.listViewString.Items.Add(lvi);

            this.listViewString.Focus();
            lvi.Focused = true;
            lvi.EnsureVisible();

            this.txtString.Text = "";
        }

        private void btnConfrim_Click(object sender, EventArgs e)
        {
            this.Value.Clear();
            for (int i = 0; i < this.listViewString.Items.Count; i++)
            {
                this.Value.Add(this.listViewString.Items[i].SubItems[1].Text);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
