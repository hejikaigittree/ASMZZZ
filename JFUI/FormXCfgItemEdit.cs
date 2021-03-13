using JFInterfaceDef;
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
    /// 用于向JFXCfg中添加数据项的窗口
    /// </summary>
    public partial class FormXCfgItemEdit : Form
    {
        public FormXCfgItemEdit()
        {
            InitializeComponent();
        }

        private void FormAddItem2XCfg_Load(object sender, EventArgs e)
        {

        }

        public string GetItemName()
        {
            return tbItemName.Text;
        }

        public Type GetItemType()
        {
            if (cbItemType.SelectedIndex < 0)
                return null;
            return cbItemType.SelectedItem as Type;
        }

        public object GetItemValue()
        {
            object val = null;
            if (ucItemValue.GetParamValue(out val))
                return val;
            return null;
        }



        delegate void dgSetItemName(string name);
        public void SetItemName(string name)
        {
            if(InvokeRequired)
            {
                Invoke(new dgSetItemName(SetItemName), new object[] { name });
                return;
            }
            tbItemName.Text = name;
        }





        delegate void dgSetItemAllowedTypes(Type[] types);
        /// <summary>
        /// 设置可供选择的数据类型
        /// </summary>
        /// <param name="types"></param>
        public void SetItemAllowedTypes(Type[] types)
        {
            if(InvokeRequired)
            {
                Invoke(new dgSetItemAllowedTypes(SetItemAllowedTypes), new object[] { types });
                return;
            }
            cbItemType.Items.Clear();
            cbItemType.Items.AddRange(types);

        }

        delegate void dgSetItemType(Type type);
        public void SetItemType(Type type)
        {
            if(InvokeRequired)
            {
                Invoke(new dgSetItemType(SetItemType), new object[] { type });
                return;
            }
            if (null == type)
                return;
            if (!cbItemType.Items.Contains(type))
                cbItemType.Items.Add(type);
                //cbItemType.SelectedItem = type;
                //return;
            

            cbItemType.SelectedItem = type;

        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbItemName.Text))
            {
                MessageBox.Show("名称不能为空");
                return;
            }
            if(cbItemType.SelectedIndex < 0)
            {
                MessageBox.Show("请选择数据类型");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void tbItemName_TextChanged(object sender, EventArgs e)
        {
            if (cbItemType.SelectedIndex < 0)
                return;
            if (string.IsNullOrEmpty(tbItemName.Text))
                return;
            ucItemValue.SetParamDesribe(JFParamDescribe.Create(tbItemName.Text, cbItemType.SelectedItem as Type, JFValueLimit.NonLimit, null));
        }

        private void cbItemType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbItemType.SelectedIndex < 0)
                return;
            if (string.IsNullOrEmpty(tbItemName.Text))
                return;
            ucItemValue.SetParamDesribe(JFParamDescribe.Create(tbItemName.Text, cbItemType.SelectedItem as Type, JFValueLimit.NonLimit, null));

        }
    }
}
