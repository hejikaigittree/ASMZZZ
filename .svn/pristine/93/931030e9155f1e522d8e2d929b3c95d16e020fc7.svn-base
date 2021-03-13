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
    /// <summary>
    /// 用于显示轴状态的面板类
    /// </summary>
    public partial class UcAxisStatus : UserControl
    {
        public enum JFDisplayMode
        {
            full = 0, //控件全部显示
            simple, //简单的界面,一排横列
            simplest //简单模式

        }
        public UcAxisStatus()
        {
            InitializeComponent();
            lampAlm.OnColor = LampButton.LColor.Red;
            lampEmg.OnColor = LampButton.LColor.Red;
            lampPl.OnColor = LampButton.LColor.Red;
            lampNl.OnColor = LampButton.LColor.Red;
            lampSpl.OnColor = LampButton.LColor.Red;
            lampSnl.OnColor = LampButton.LColor.Red;
            ptInp = lampInp.Location; //Inp按钮原始坐标
            ptemg = lampEmg.Location; //emg按钮原始坐标
            ptPos = tbPos.Location; //反馈位置的原始坐标
            ptPosTital = lbTitalPos.Location;
            ptCmd = tbEnc.Location;//命令位置的原始坐标
            ptCmdTital = lbTitalEnc.Location;//命令标题的原始坐标
        }

        private void UcAxisStatus_Load(object sender, EventArgs e)
        {
            
        }

        [Category("JF属性"), Description("显示模式"), Browsable(true)]
        public JFDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if (value == _displayMode)
                    return;
                _displayMode = value;
                AdjustView();
            }
        }

        int orgWidth = 480, orgHeight = 51;//原始尺寸
        int halfHeight = 26; //半高
        int simplestWidth = 180;//最简模式的宽度
        void AdjustView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            switch(_displayMode)
            {
                case JFDisplayMode.full:
                    
                    lampInp.Location = ptInp;
                    lampOrg.Visible = true;
                    lampEmg.Visible = true;
                    lampPl.Visible = true;
                    lampNl.Visible = true;
                    lampMd.Visible = true;
                    lampSpl.Visible = true;
                    lampSnl.Visible = true;
                    lampHomeDone.Visible = true;

                    lbTitalPos.Location = ptPosTital;
                    tbPos.Location = ptPos;
                    lbTitalEnc.Visible = true;
                    tbEnc.Visible = true;
                    Size = new Size(orgWidth,orgHeight);
                    break;
                case JFDisplayMode.simple: //只一行显示 伺服/报警/正负限 到位 反馈位置
                    lampEmg.Visible = false;
                    lampInp.Location = ptemg;
                    lampOrg.Visible = false;
                    lampMd.Visible = false;
                    lampSpl.Visible = false;
                    lampSnl.Visible = false;
                    lampHomeDone.Visible = false;
                    lbTitalEnc.Visible = false;
                    tbEnc.Visible = false;
                    lbTitalPos.Location = new Point(lampOrg.Location.X, lampOrg.Location.Y+4);
                    tbPos.Location = lampMd.Location;
                    lbTitalPos.Visible = true;
                    tbPos.Visible = true;
                    Size = new Size(orgWidth, halfHeight);

                    break;
                case JFDisplayMode.simplest://简单模式，显示两行 伺服/报警/到位   ， 反馈位置
                    lampEmg.Visible = false;
                    lampInp.Location = ptemg;
                    lampOrg.Visible = false;
                    lampMd.Visible = false;
                    lampSpl.Visible = false;
                    lampSnl.Visible = false;
                    lampHomeDone.Visible = false;
                    lbTitalEnc.Visible = false;
                    tbEnc.Visible = false;
                    lbTitalPos.Location = lbTitalEnc.Location;
                    tbPos.Location = tbEnc.Location;
                    Size = new Size(simplestWidth, orgHeight);
                    break;
            }
        }

        public void SetAxis(IJFModule_Motion module,int axis)
        {
            motionModule = module;
            axisIndex = axis;
        }

        public void UpdateAxisStatus()
        {
            //if (null == motionModule)
            //    throw new ArgumentNullException("UpdateAxisStatus() failed By:motionModule = null!");
            //if(axisIndex < 0 || axisIndex >= motionModule.AxisCount)
            //    throw new ArgumentOutOfRangeException()
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateAxisStatus));
                return;
            }
            
            if(null == motionModule)
            {
                tbEnc.Text = "Err:module=null";
                tbPos.Text = "Err:module=null";
                return;
            }
            if(axisIndex < 0 || axisIndex >= motionModule.AxisCount)
            {
                string errInfo = string.Format("Err:axis={0}|Cnt={1}", axisIndex, motionModule.AxisCount);
                tbEnc.Text = errInfo;
                tbPos.Text = errInfo;
                return;
            }
            double cmd = 0;
            int err = motionModule.GetCmdPos(axisIndex, out cmd);
            if (err != 0)
                tbEnc.Text = motionModule.GetErrorInfo(err);
            else
                tbEnc.Text = cmd.ToString("F3");

            err = motionModule.GetFbkPos(axisIndex, out cmd);
            if (err != 0)
                tbPos.Text = motionModule.GetErrorInfo(err);
            else
                tbPos.Text = cmd.ToString("F3");

            bool[] status;
            err = motionModule.GetMotionStatus(axisIndex,out status);
            if(err != 0)
            {
                lampSvo.IOName = "伺服X";
                lampAlm.IOName = "报警X";
                lampEmg.IOName = "急停X";
                lampPl.IOName = "正限X";
                lampNl.IOName = "负限X";
                lampOrg.IOName = "原点X";
                lampMd.IOName = "完成X";
                lampInp.IOName = "到位X";
                lampSpl.IOName = "正软X";
                lampSnl.IOName = "负软X";

                lampSvo.IsTurnOn = false;
                lampAlm.IsTurnOn = false;
                lampEmg.IsTurnOn = false;
                lampPl.IsTurnOn = false;
                lampNl.IsTurnOn = false;
                lampOrg.IsTurnOn = false;
                lampMd.IsTurnOn = false;
                lampInp.IsTurnOn = false;
                lampSnl.IsTurnOn = false;
                lampSpl.IsTurnOn = false;
            }
            else
            {
                lampSvo.IOName = "伺服";
                lampAlm.IOName = "报警";
                lampEmg.IOName = "急停";
                lampPl.IOName = "正限";
                lampNl.IOName = "负限";
                lampOrg.IOName = "原点";
                lampMd.IOName = "完成";
                lampInp.IOName = "到位";
                lampSnl.IOName = "正软";
                lampSpl.IOName = "负软";

                lampSvo.IsTurnOn = motionModule.MSID_SVO<0?false: status[motionModule.MSID_SVO];
                lampAlm.IsTurnOn = motionModule.MSID_ALM < 0 ? false : status[motionModule.MSID_ALM];
                lampEmg.IsTurnOn = motionModule.MSID_EMG < 0 ? false : status[motionModule.MSID_EMG];
                lampPl.IsTurnOn = motionModule.MSID_PL < 0 ? false : status[motionModule.MSID_PL];
                lampNl.IsTurnOn = motionModule.MSID_NL < 0 ? false : status[motionModule.MSID_NL]; ;
                lampOrg.IsTurnOn = motionModule.MSID_ORG < 0 ? false : status[motionModule.MSID_ORG]; ;
                lampMd.IsTurnOn = motionModule.MSID_MDN < 0 ? false : status[motionModule.MSID_MDN]; ;
                lampInp.IsTurnOn = motionModule.MSID_INP < 0 ? false : status[motionModule.MSID_INP]; ;
                lampSpl.IsTurnOn = motionModule.MSID_SPL < 0 ? false : status[motionModule.MSID_SPL];
                lampSnl.IsTurnOn = motionModule.MSID_SNL < 0 ? false : status[motionModule.MSID_SNL];

            }

            bool isHomeDone = false;
            err = motionModule.IsHomeDone(axisIndex, out isHomeDone);
            if (0 == err)
                lampHomeDone.IsTurnOn = isHomeDone;
            else
                lampHomeDone.IsTurnOn = false;





        }

        Point ptInp;    //Inp按钮原始坐标
        Point ptemg;    //emg按钮原始坐标
        Point ptPos;    //反馈位置的原始坐标
        Point ptPosTital;  //反馈标题的原始坐标

        Point ptCmd;//命令位置的原始坐标
        Point ptCmdTital;//命令标题的原始坐标



        JFDisplayMode _displayMode = JFDisplayMode.full;

        IJFModule_Motion motionModule = null;
        int axisIndex = 0;
    }
}
