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

namespace JFUI
{
    public partial class UcMotionDaq : JFRealtimeUI
    {
        public UcMotionDaq()
        {
            InitializeComponent();
            fmDios = new FormDios();
            fmMotions = new FormMotions();
            fmCmprTrigs = new FormCmprTrigs();
            fmAios = new FormAios();

        }

        private void UcMotionDaq_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustUI();
        }

        public void SetDevice(IJFDevice_MotionDaq dev,string id)
        {
            _dev = dev;
            _id = id;
            if(_isFormLoaded)
                AdjustUI();
        }

        bool _isFormLoaded = false;
        IJFDevice_MotionDaq _dev = null;
        FormDios fmDios;
        FormMotions fmMotions;
        FormCmprTrigs fmCmprTrigs;
        FormAios fmAios;
        string _id = null;

        void AdjustUI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustUI));
                return;
            }

            while (tabControlCF1.TabCount > 1)
                tabControlCF1.TabPages.RemoveAt(tabControlCF1.TabCount - 1);

            gbInitParams.Controls.Clear();

            if (null == _dev)
            {
                lbModel.Text = "空";
                lbID.Text = "空";
                lbOpend.Text = "空";
                btOpenClose.Enabled = false;
                btOpenClose.Text = "空设备";
                return;
            }
            else //更新设备信息
            {
                lbModel.Text = _dev.DeviceModel;
                lbID.Text = _id;
                //UpdateDevice();

                int locY = 20;
                int locX = 5;
                Label lbDevInit = new Label();
                lbDevInit.Text = _dev.IsInitOK?"初始化完成":"初始化失败";
                lbDevInit.Location = new Point(locX, locY);
                gbInitParams.Controls.Add(lbDevInit);
                locY += 28;
                for(int i = 0; i < _dev.InitParamNames.Length;i++)
                {
                    UcJFParamEdit ucParam = new UcJFParamEdit();
                    ucParam.SetParamDesribe(_dev.GetInitParamDescribe(_dev.InitParamNames[i]));
                    ucParam.SetParamValue(_dev.GetInitParamValue(_dev.InitParamNames[i]));
                    ucParam.Enabled = false;
                    ucParam.Location = new Point(locX , locY);
                    gbInitParams.Controls.Add(ucParam);
                    ucParam.Width = gbInitParams.Width-20;
                    locY = ucParam.Bottom + 2;
                }

                if (_dev.DioCount > 0) 
                {
                    ///fmDios = new FormDios();
                    for (int i = 0; i < _dev.DioCount; i++)
                        fmDios.AddModule(_dev.GetDio(i), "模块_" + i.ToString());

                    TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
                    tabPage.Text = "DIO模块";
                    tabPage.Name = "DioModules";
                    tabPage.BackColor = fmDios.BackColor;

                    tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

                    fmDios.TopLevel = false;
                    fmDios.FormBorderStyle = FormBorderStyle.None;
                    fmDios.Dock = DockStyle.Fill;
                    fmDios.Parent = tabPage;
                    fmDios.Visible = true;
                    tabPage.Controls.Add(fmDios);  //tabPage选项卡添加一个窗体对象 
                }

                if(_dev.McCount > 0)
                {
                    //fmMotions = new FormMotions();
                    for(int i = 0; i < _dev.McCount; i++)
                        fmMotions.AddModule(_dev.GetMc(i), "模块_" + i.ToString());

                    TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
                    tabPage.Text = "Motion模块";
                    tabPage.Name = "MotionModules";
                    tabPage.BackColor = fmMotions.BackColor;

                    tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

                    fmMotions.TopLevel = false;
                    fmMotions.FormBorderStyle = FormBorderStyle.None;
                    fmMotions.Dock = DockStyle.Fill;
                    fmMotions.Parent = tabPage;
                    fmMotions.Visible = true;
                    tabPage.Controls.Add(fmMotions);  //tabPage选项卡添加一个窗体对象 
                }

                if(_dev.CompareTriggerCount > 0)
                {
                    for (int i = 0; i < _dev.CompareTriggerCount; i++)
                        fmCmprTrigs.AddModule(_dev.GetCompareTrigger(i), "模块_" + i.ToString());

                    TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
                    tabPage.Text = "CmprTrig模块";
                    tabPage.Name = "CmprTrigModule";
                    tabPage.BackColor = fmCmprTrigs.BackColor;

                    tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

                    fmCmprTrigs.TopLevel = false;
                    fmCmprTrigs.FormBorderStyle = FormBorderStyle.None;
                    fmCmprTrigs.Dock = DockStyle.Fill;
                    fmCmprTrigs.Parent = tabPage;
                    fmCmprTrigs.Visible = true;
                    tabPage.Controls.Add(fmCmprTrigs);  //tabPage选项卡添加一个窗体对象 
                }

                if(_dev.AioCount > 0)
                {
                    for (int i = 0; i < _dev.AioCount; i++)
                        fmAios.AddModule(_dev.GetAio(i), "模块_" + i.ToString());

                    TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
                    tabPage.Text = "AIO模块";
                    tabPage.Name = "AioModules";
                    tabPage.BackColor = fmAios.BackColor;

                    tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

                    fmAios.TopLevel = false;
                    fmAios.FormBorderStyle = FormBorderStyle.None;
                    fmAios.Dock = DockStyle.Fill;
                    fmAios.Parent = tabPage;
                    fmAios.Visible = true;
                    tabPage.Controls.Add(fmAios);  //tabPage选项卡添加一个窗体对象 
                }

            }

            tabControlCF1.SelectedIndex = 0;
            UpdateModuleStatus();
        }

        public void UpdateModuleStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            string tagName = tabControlCF1.TabPages[tabControlCF1.SelectedIndex].Name;
            if (tagName == "DevStatus")
                UpdateDevice();
            else if (tagName == "DioModules")
                UpdateDio();
            else if (tagName == "MotionModules")
                UpdateAxis();
            else if (tagName == "CmprTrigModule")
                UpdateCmprTrig();
            else if (tagName == "AioModules")
                UpdateAio();
        }

        public override void UpdateSrc2UI()
        {
            UpdateModuleStatus();
        }

        void UpdateDevice()
        {
            lbOpend.Text = _dev.IsDeviceOpen ? "已打开" : "已关闭";
            btOpenClose.Enabled = _dev.IsInitOK;
            btOpenClose.Text = _dev.IsDeviceOpen ? "关闭设备" : "打开设备";

            lbDioCount.Text = _dev.DioCount.ToString();
            lbMotionCount.Text =  _dev.McCount.ToString();
            lbAioCount.Text = _dev.AioCount.ToString();
            lbCmprTrigCount.Text = _dev.CompareTriggerCount.ToString();

         
            return;
       

        }

        void UpdateDio()
        {
            fmDios.UpdateModleStatus();
        }

        void UpdateAxis()
        {
            fmMotions.UpdateModleStatus();
        }

        void UpdateCmprTrig()
        {
            fmCmprTrigs.UpdateModleStatus();
        }

        void UpdateAio()
        {
            fmAios.UpdateModuleStatus();
        }

        private void btOpenClose_Click(object sender, EventArgs e)
        {
            if(null == _dev)
            {
                MessageBox.Show("打开设备失败！设备对象为空");
                return;
            }
            if(_dev.IsDeviceOpen)
            {
                int err = _dev.CloseDevice();
                if(err != 0)
                {
                    MessageBox.Show("关闭设备失败，错误信息：" + _dev.GetErrorInfo(err));
                    return;
                }

            }
            else
            {
                int err = _dev.OpenDevice();
                if (err != 0)
                {
                    MessageBox.Show("打开设备失败，错误信息：" + _dev.GetErrorInfo(err));
                    return;
                }
            }
            AdjustUI();
        }
    }
}
