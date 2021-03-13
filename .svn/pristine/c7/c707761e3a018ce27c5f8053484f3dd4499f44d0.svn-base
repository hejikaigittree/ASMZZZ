using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    /// <summary>
    /// IF_ELSE 方法的内部动作流程配置界面
    /// </summary>
    public partial class FormMethodConditionCfgUI : Form
    {
        public FormMethodConditionCfgUI()
        {
            InitializeComponent();
        }

        private void FormMethodConditionCfgUI_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages[0].Controls.Add(_ucTrueFlow);
            tabControl1.TabPages[1].Controls.Add(_ucFalseFlow);
        }
        UcMethodFlow _ucTrueFlow = new UcMethodFlow();
        UcMethodFlow _ucFalseFlow = new UcMethodFlow();


        public void SetMethodFlow(JFMethodCondition mf)
        {
            _ucTrueFlow.SetMethodFlow(mf.TrueFlow);
            _ucFalseFlow.SetMethodFlow(mf.FalseFlow);
        }
    }
}
