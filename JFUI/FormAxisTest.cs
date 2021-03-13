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

namespace JFUI
{
    /// <summary>
    /// 单轴全功能测试窗口
    /// </summary>
    public partial class FormAxisTest : Form
    {
        public FormAxisTest()
        {
            InitializeComponent();
        }

        private void FormModuleTest_Axis_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            ucAxisStatus1.SetAxis(_module, _axisIndex);
            ucAxisTest1.SetAxis(_module, _axisIndex);
            Text = _axisID;
            if (null != _module)
                timerFlushStatus.Enabled = true;
            
        }
        bool _isFormLoaded = false;
        IJFModule_Motion _module = null;
        int _axisIndex = 0;
        string _axisID = "轴测试";
        /// <summary>设置轴模块</summary>
        public void SetAxisInfo(IJFModule_Motion module,int axis,string axisID)
        {
            _module = module;
            _axisIndex = axis;
            _axisID = axisID == null?"轴_" + axis.ToString(): axisID;
            if (_isFormLoaded)
            {
                ucAxisStatus1.SetAxis(_module, _axisIndex);
                ucAxisTest1.SetAxis(_module, _axisIndex);
                timerFlushStatus.Enabled = true;
            }
        }

        void UpdateAxisStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateAxisStatus));
                return;
            }
            ucAxisStatus1.UpdateAxisStatus();
            ucAxisTest1.UpdateAxisUI();
        }

        private void timerFlushStatus_Tick(object sender, EventArgs e)
        {
            UpdateAxisStatus();
        }
    }
}
