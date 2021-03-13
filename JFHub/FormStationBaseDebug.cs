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
    /// 用于显示StationBase状态，调试StationBase参数的窗口界面
    /// </summary>
    public partial class FormStationBaseDebug : Form
    {
        public FormStationBaseDebug()
        {
            InitializeComponent();
        }

        bool _isFormLoaded = false;
        private void FormStationBase_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            IsMdiContainer = true;
            if(null != Parent)
                Parent.VisibleChanged += new EventHandler(this.FormStationBase_VisibleChanged);
            
            _panelDio.MdiParent = this;
            VisibleChanged += new EventHandler(_panelDio.FormStationBaseDioPanel_VisibleChanged);
            _panelDio.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelAio.MdiParent = this;
            VisibleChanged += new EventHandler(_panelAio.FormStationBaseAioPanel_VisibleChanged);
            _panelAio.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelAxis.MdiParent = this;
            VisibleChanged += new EventHandler(_panelAxis.FormStationBaseAxisPanel_VisibleChanged);
            _panelAxis.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelCmpTrig.MdiParent = this;
            VisibleChanged += new EventHandler(_panelCmpTrig.FormStationBaseCmpTrigPanel_VisibleChanged);
            _panelCmpTrig.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelLight.MdiParent = this;
            VisibleChanged += new EventHandler(_panelLight.FormStationBaseLightPanel_VisibleChanged);
            _panelLight.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelCmr.MdiParent = this;
            VisibleChanged += new EventHandler(_panelCmr.FormStationBaseCmrPanel_VisibleChanged);
            _panelCmr.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelWorkFlow.MdiParent = this;
            VisibleChanged += new EventHandler(_panelWorkFlow.FormStationBaseWorkFlowPanel_VisibleChanged);
            _panelWorkFlow.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            _panelCfgParam.MdiParent = this;
            VisibleChanged += new EventHandler(_panelCfgParam.FormStationCfgParam_VisibleChanged);
            _panelCfgParam.FormClosing += new FormClosingEventHandler(MdiChildClosing);
            //_panelCfgParam.Dock = DockStyle.Fill;

            LayoutStationUI();
        }


        FormStationBaseDioPanel _panelDio = new FormStationBaseDioPanel(); //DIO调试面板
        FormStationBaseAioPanel _panelAio = new FormStationBaseAioPanel(); //AIO调试面板
        FormStationBaseAxisPanel _panelAxis = new FormStationBaseAxisPanel();//轴电机调试面板
        FormStationBaseCmpTrigPanel _panelCmpTrig = new FormStationBaseCmpTrigPanel(); //比较触发器面板

        FormStationBaseLightPanel _panelLight = new FormStationBaseLightPanel();//光源控制器面板
        FormStationBaseCmrPanel _panelCmr = new FormStationBaseCmrPanel();//相机测试面板
        FormStationBaseWorkFlowPanel _panelWorkFlow = new FormStationBaseWorkFlowPanel();//工作流测试面板

        FormStationBaseCfgParam _panelCfgParam = new FormStationBaseCfgParam();   

        public void SetStation(JFStationBase station)
        {
            _station = station;
            if (null != _station)
                Text = station.Name;
            _panelDio.SetStation(station);
            _panelAio.SetStation(station);
            _panelAxis.SetStation(station);
            _panelCmpTrig.SetStation(station);
            _panelLight.SetStation(station);
            _panelCmr.SetStation(station);
            _panelWorkFlow.SetStation(station);
            _panelCfgParam.SetStation(station);
            if (_isFormLoaded)
                LayoutStationUI();
        }

        /// <summary>
        /// 根据工站的元素布置界面， 在SetStation/FormLoaded /重新显示时需要调用一次（防止Station的配置）
        /// </summary>
        void LayoutStationUI()
        {

        }

        /// <summary>
        /// 将工站状态更新到界面上（实时）
        /// </summary>
        void UpdateStation2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateStation2UI));
                return;
            }
        }

        //子窗口关闭事件
        private void MdiChildClosing(object sender, FormClosingEventArgs e)
        {
            if (sender == _panelAxis)
                menuItemAxis.Checked = false;
            else if (sender == _panelAio)
                menuItemAio.Checked = false;
            else if (sender == _panelDio)
                menuItemDio.Checked = false;
            else if (sender == _panelCmpTrig)
                menuItemCmpTrig.Checked = false;
            else if (sender == _panelLight)
                menuItemLight.Checked = false;
            else if (sender == _panelCmr)
                menuItemCmr.Checked = false;
            else if (sender == _panelWorkFlow)
                menuItemWorkFlow.Checked = false;
            else if (sender == _panelCfgParam)
                CfgParamToolStripMenuItem.Checked = false;

        }

        JFStationBase _station = null;

        /// <summary>
        /// 显示/关闭 轴测试界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemAxis_Click(object sender, EventArgs e)
        {
            if (!menuItemAxis.Checked)
            {
                _panelAxis.Show();
                menuItemAxis.Checked = true;
            }
            else
            {
                _panelAxis.Hide();
                menuItemAxis.Checked = false;
            }
        }

        /// <summary>
        /// 显示/关闭 DIO界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemDIO_Click(object sender, EventArgs e)
        {
            if (!menuItemDio.Checked)
            {
                _panelDio.Show();
                menuItemDio.Checked = true;
            }
            else
            {
                _panelDio.Hide();
                menuItemDio.Checked = false;
            }
        }

        /// <summary>
        /// 显示/关闭相机界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemCmr_Click(object sender, EventArgs e)
        {
            if (!menuItemCmr.Checked)
            {
                _panelCmr.Show();
                menuItemCmr.Checked = true;
            }
            else
            {
                _panelCmr.Hide();
                menuItemCmr.Checked = false;
            }
        }

        /// <summary>
        /// 显示/关闭 光源控制器界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemLightCtrl_Click(object sender, EventArgs e)
        {
            if (!menuItemLight.Checked)
            {
                _panelLight.Show();
                menuItemLight.Checked = true;
            }
            else
            {
                _panelLight.Hide();
                menuItemLight.Checked = false;
            }
        }

        /// <summary>
        /// 显示关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemCmpTrig_Click(object sender, EventArgs e)
        {
            if (!menuItemCmpTrig.Checked)
            {
                _panelCmpTrig.Show();
                menuItemCmpTrig.Checked = true;
            }
            else
            {
                _panelCmpTrig.Hide();
                menuItemCmpTrig.Checked = false;
            }
        }

        /// <summary>
        /// 显示/关闭 动作流测试界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemWorkFlow_Click(object sender, EventArgs e)
        {
            if (!menuItemWorkFlow.Checked)
            {
                _panelWorkFlow.Show();
                menuItemWorkFlow.Checked = true;
            }
            else
            {
                _panelWorkFlow.Hide();
                menuItemWorkFlow.Checked = false;
            }
        }

        /// <summary>
        /// 显示/关闭 AIO面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemAIO_Click(object sender, EventArgs e)
        {
            if (!menuItemAio.Checked)
            {
                _panelAio.Show();
                menuItemAio.Checked = true;
            }
            else
            {
                _panelAio.Hide();
                menuItemAio.Checked = false;
            }
        }

        /// <summary>
        /// 显示模式MDI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemViewModeMDI_Click(object sender, EventArgs e)
        {
            ///暂时只支持MDI一种显示模式，日后添加其他非MDI显示模式
        }

        private void FormStationBase_VisibleChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 显示工站参数配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CfgParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CfgParamToolStripMenuItem.Checked)
            {
                _panelCfgParam.Show();
                CfgParamToolStripMenuItem.Checked = true;
            }
            else
            {
                _panelCfgParam.Hide();
                CfgParamToolStripMenuItem.Checked = false;
            }
        }
    }
}
