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
using JFToolKits;
using JFUI;

namespace JFHub
{
    public partial class FormCreateInitor : Form
    {
        public FormCreateInitor()
        {
            InitializeComponent();
            initor = null;
            MatchType = typeof(IJFInitializable);
            ExistIDs = JFHubCenter.Instance.InitorManager.InitorIDs;
        }

        private void FormSelectInitor_Load(object sender, EventArgs e)
        {
            dgvTypes.Rows.Clear();
            dgvTypes.MultiSelect = false;
            dgvTypes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Type[] initorTypes = JFHubCenter.Instance.InitorHelp.InstantiatedClasses(MatchType);
            foreach (Type it in initorTypes)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cellType = new DataGridViewTextBoxCell();
                cellType.Value = it.AssemblyQualifiedName;
                row.Cells.Add(cellType);
                JFDisplayNameAttribute[] vn = it.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
                if (null != vn && vn.Length > 0)
                {
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = vn[0].Name;
                    row.Cells.Add(cellName);
                }
                dgvTypes.Rows.Add(row);
            }
            dgvTypes.ClearSelection();

            if(_isFixedID)
            {
                tbID.Text = _fixID;
                tbID.ReadOnly = true;
            }
        }


        /// <summary>确定创建Initor对象</summary>
        private void btOK_Click(object sender, EventArgs e)
        {
            if (dgvTypes.SelectedRows == null || dgvTypes.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择需要创建的设备类型");
                return;
            }
            if(string.IsNullOrWhiteSpace(tbID.Text))
            {
                MessageBox.Show("设备ID不能为空,请重新输入");
                tbID.Focus();
                return;
            }

            if(!_isFixedID && JFHubCenter.Instance.InitorManager.ContainID(tbID.Text))
            {
                IJFInitializable dev = JFHubCenter.Instance.InitorManager.GetInitor(tbID.Text);
                string disTypeName = JFinitializerHelper.DispalyTypeName(dev.GetType());
                MessageBox.Show(string.Format("Initor列表中已存在ID = \"{0}\" Type = \"{1}\"的设备\n请重新输入ID!", tbID.Text, disTypeName));
                tbID.Focus();
                return;
            }
            //IJFDevice newDev = JFHubCenter.Instance.InitorHelp.CreateInstance(dgvTypes.SelectedRows[0].Cells[0].Value.ToString()) as IJFDevice;

            //for (int i = 2; i < gbParams.Controls.Count;i++)
            for(int i = 0; i < panelParams.Controls.Count;i++)
            {
                //UcJFParamEdit pe = gbParams.Controls[i] as UcJFParamEdit;
                UcJFParamEdit pe = panelParams.Controls[i] as UcJFParamEdit;
                object paramVal = null;
                if(!pe.GetParamValue(out paramVal))
                {

                    MessageBox.Show("初始化参数:\"" + pe.Name + "\"获取失败!Error:" + pe.GetParamErrorInfo());
                    //newDev.Dispose();
                    pe.Focus();
                    return;
                }
                if(!initor.SetInitParamValue(initor.InitParamNames[i], paramVal))
                {
                    MessageBox.Show(string.Format("设置初始化参数失败:Name = {0},Value = {1}", initor.InitParamNames[i],paramVal.ToString()));
                    //newDev.Dispose();
                    pe.Focus();
                    return;
                }
            }
            
            DialogResult = DialogResult.OK;
        }



        private void btCancel_Click(object sender, EventArgs e)
        {
            if (null != initor)
            {
                initor.Dispose();
                initor = null;
            }
            DialogResult = DialogResult.Cancel;
        }

        int RowIndexSelected = -1;
        private void dgvTypes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (RowIndexSelected == e.RowIndex)
                return;
            RowIndexSelected = e.RowIndex;
            if (null != initor)
            {
                initor.Dispose();
                initor = null;
            }
            //while (gbParams.Controls.Count > 2)
            //    gbParams.Controls.RemoveAt(2);
            panelParams.Controls.Clear();
            panelParams.AutoScroll = true;
            initor = JFHubCenter.Instance.InitorHelp.CreateInstance(dgvTypes.Rows[e.RowIndex].Cells[0].Value.ToString());
            if(null == initor)
            {
                string err = string.Format("Invoke Initor's Ctor Failed！InitorHelp.CreateInstance(Type = {0}) return null", dgvTypes.Rows[e.RowIndex].Cells[0].Value.ToString());
                lbTips.Text = err;
                //MessageBox.Show(err);
                return;
            }
            //int locY = tbID.Location.Y + tbID.Size.Height + 5;
            string[] paramNames = initor.InitParamNames;
            if (null == paramNames || 0 == paramNames.Length)
                return;
            for(int i = 0; i < paramNames.Length;i++)
            {
                string pn = paramNames[i];
                JFParamDescribe pd = initor.GetInitParamDescribe(pn);
                UcJFParamEdit uc = new UcJFParamEdit();
                //uc.Margin = new Padding(3, 10, 3, 10);
                panelParams.Controls.Add(uc);
                uc.SetParamDesribe(pd);
                uc.IsValueReadOnly = false;
            }
        }
        //Type _matchType;
        public Type MatchType
        {
            private get;// { return _matchType; }
            set;//
            //{
            //    _matchType = value;
            //}
        }

        //系统中已经存在的名称
        public string[] ExistIDs
        {
            private get;
            set;
        }

        public IJFInitializable Initor { get { return initor; } }
        public string ID { get { return tbID.Text; } }


        bool _isFixedID = false;
        string _fixID = null;
        /// <summary>
        /// 设置一个固定的ID（用于指定唯一的系统变量）
        /// </summary>
        /// <param name="fixID"></param>
        public void SetFixedID(string fixID)
        {
            _isFixedID = true;
            _fixID = fixID;
        }

        IJFInitializable initor;
    }
}
