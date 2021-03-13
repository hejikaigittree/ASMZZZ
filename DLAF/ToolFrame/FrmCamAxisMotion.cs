using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using System.IO;
using JFInterfaceDef;

namespace DLAF
{
    public partial class FrmCamAxisMotion : Form
    {
        #region 参数
        public static FrmCamAxisMotion Instance = null;
        double distance = 0;
        bool MoveEnable = true;
        #endregion

        IJFStation _station;
        public HObject Image = new HObject();
        public HObject showRegion = new HObject();

        public FrmCamAxisMotion()
        {
            InitializeComponent();
            this.htWindow.SetTablePanelVisible(false);
            distance = (double)numDistance.Value;

            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Instance = this;
        }

        public void SetStation(IJFStation station)
        {
            _station = station;
            InitParam();
        }

        private void InitParam()
        {
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                for (int i = 0; i < ((CalibStation)_station).Num_Camera; i++)
                {
                    cbb_Cam.Items.Add("Camera_" + i);
                }
                timer1.Enabled = true;
                if (((CalibStation)_station).SelectedIndex > -1 && ((CalibStation)_station).SelectedIndex < ((CalibStation)_station).Num_Camera)
                    cbb_Cam.SelectedIndex = ((CalibStation)_station).SelectedIndex;
            }
            else
            {
                for (int i = 0; i < ((AutoMappingStation)_station).Num_Camera; i++)
                {
                    cbb_Cam.Items.Add("Camera_" + i);
                }
                timer1.Enabled = true;
                if (((AutoMappingStation)_station).SelectedIndex > -1 && ((AutoMappingStation)_station).SelectedIndex < ((AutoMappingStation)_station).Num_Camera)
                    cbb_Cam.SelectedIndex = ((AutoMappingStation)_station).SelectedIndex;
            }
        }

        private void numDistance_ValueChanged(object sender, EventArgs e)
        {
            distance = (double)numDistance.Value;
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (!MoveEnable) return;
            MoveEnable = false;
            string errMsg = "";
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[1], distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[1], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[1] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[1], distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[1], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[1] + "运动完成超时");
                    return;
                }
            }

            if (checkBox_MoveByCapture.Checked) btnCapture_Click(null, null);
            MoveEnable = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (!MoveEnable) return;
            MoveEnable = false;
            string errMsg = "";

            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[1], -1 * distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[1], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[1] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[1], -1 * distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[1], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[1] + "运动完成超时");
                    return;
                }
            }

            if (checkBox_MoveByCapture.Checked) btnCapture_Click(null, null);
            MoveEnable = true;
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (!MoveEnable) return;
            MoveEnable = false;
            string errMsg = "";
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[0], -1 * distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[0], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[0] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[0], -1 * distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[0], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[0] + "运动完成超时");
                    return;
                }
            }

            if (checkBox_MoveByCapture.Checked) btnCapture_Click(null, null);
            MoveEnable = true;
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (!MoveEnable) return;
            MoveEnable = false;
            string errMsg = "";
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[0], distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[0], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[0] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[0], distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[0], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[0] + "运动完成超时");
                    return;
                }
            }
            if (checkBox_MoveByCapture.Checked) btnCapture_Click(null, null);
            MoveEnable = true;
        }

        private void btnZfocus_Click(object sender, EventArgs e)
        {
            string errMsg = "";
            if (!MoveEnable) return;
            MoveEnable = false;
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[2], ((CalibStation)_station).ZFocus, true, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[2], ((AutoMappingStation)_station).ZFocus, true, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }
            MoveEnable = true;
        }

        private void btnZsafe_Click(object sender, EventArgs e)
        {
            string errMsg = "";
            if (!MoveEnable) return;
            MoveEnable = false;
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[2], ((CalibStation)_station).Z_safe, true, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[2], ((AutoMappingStation)_station).Z_safe, true, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    MoveEnable = true;
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }
            MoveEnable = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double xFbkPos = 0, yFbkPos = 0, zFbkPos = 0;
            string errMsg = "";

            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                ((CalibStation)_station).GetAxisPosition(((CalibStation)_station).AxisXYZ[0], out xFbkPos, out errMsg);
                ((CalibStation)_station).GetAxisPosition(((CalibStation)_station).AxisXYZ[1], out yFbkPos, out errMsg);
                ((CalibStation)_station).GetAxisPosition(((CalibStation)_station).AxisXYZ[2], out zFbkPos, out errMsg);
            }
            else
            {
                ((AutoMappingStation)_station).GetAxisPosition(((AutoMappingStation)_station).AxisXYZ[0], out xFbkPos, out errMsg);
                ((AutoMappingStation)_station).GetAxisPosition(((AutoMappingStation)_station).AxisXYZ[1], out yFbkPos, out errMsg);
                ((AutoMappingStation)_station).GetAxisPosition(((AutoMappingStation)_station).AxisXYZ[2], out zFbkPos, out errMsg);
            }

            label2.Text = "X(mm):" + xFbkPos.ToString("F3");
            label3.Text = "Y(mm):" + yFbkPos.ToString("F3");
            label4.Text = "Z(mm):" + zFbkPos.ToString("F3");
        }

        private void FrmCamAxisMotion_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Space:
                    btnCapture_Click(null, null);
                    break;
                case Keys.A:
                    btnLeft_Click(null, null);
                    break;
                case Keys.W:
                    btnUp_Click(null, null);
                    break;
                case Keys.D:
                    btnRight_Click(null, null);
                    break;
                case Keys.S:
                    btnDown_Click(null, null);
                    break;
                case Keys.Q:
                    btnZUp_Click(null, null);
                    break;
                case Keys.E:
                    btnZDown_Click(null, null);
                    break;
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                string errStr = "";
                //if (_station.ToString().IndexOf("CalibStation") >= 0)
                //{
                //    if (((CalibStation)_station).operation.SWPosTrig(((CalibStation)_station).AxisX, out errStr) != 0)
                //    {
                //        MessageBox.Show(errStr);
                //        return;
                //    }
                //}
                //else
                //{
                //    if (((AutoMappingStation)_station).operation.SWPosTrig(((AutoMappingStation)_station).AxisX, out errStr) != 0)
                //    {
                //        MessageBox.Show(errStr);
                //        return;
                //    }

                //}
                //4. 取图
                HOperatorSet.GenEmptyObj(out Image);

                HTuple width, height;

                if (_station.ToString().IndexOf("CalibStation") >= 0)
                {
                    if (((CalibStation)_station).operation.CaputreOneImage(((CalibStation)_station).CamereDev[((CalibStation)_station).SelectedIndex], "Halcon", out Image, out errStr) != 0)
                    {
                        MessageBox.Show(errStr);
                        return;
                    }
                }
                else
                {
                    if (((AutoMappingStation)_station).operation.CaputreOneImage(((AutoMappingStation)_station).CamereDev[((AutoMappingStation)_station).SelectedIndex], "Halcon", out Image, out errStr) != 0)
                    {
                        MessageBox.Show(errStr);
                        return;
                    }
                }

                HOperatorSet.GetImageSize(Image, out width, out height);
                var a = width / 2;
                var b = height / 2;
                HOperatorSet.GenCrossContourXld(out showRegion, b, a, width / 20, 0);

                if (_station.ToString().IndexOf("CalibStation") >= 0)
                    ((CalibStation)_station).operation.ShowImage(htWindow, Image, showRegion);
                else
                    ((AutoMappingStation)_station).operation.ShowImage(htWindow, Image, showRegion);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnZUp_Click(object sender, EventArgs e)
        {
            if (!MoveEnable) return;
            MoveEnable = false;
            string errMsg = "";

            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[2], distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }
            else
            {

                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[2], distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }

            if (checkBox_MoveByCapture.Checked) btnCapture_Click(null, null);
            MoveEnable = true;
        }

        private void btnZDown_Click(object sender, EventArgs e)
        {
            if (!MoveEnable) return;
            MoveEnable = false;

            string errMsg = "";
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (!((CalibStation)_station).MoveAxis(((CalibStation)_station).AxisXYZ[2], -1 * distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    return;
                }
                if (((CalibStation)_station).WaitMotionDone(((CalibStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((CalibStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }
            else
            {
                if (!((AutoMappingStation)_station).MoveAxis(((AutoMappingStation)_station).AxisXYZ[2], -1 * distance, false, out errMsg))
                {
                    MessageBox.Show(errMsg);
                    return;
                }
                if (((AutoMappingStation)_station).WaitMotionDone(((AutoMappingStation)_station).AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    MessageBox.Show("等待轴" + ((AutoMappingStation)_station).AxisXYZ[2] + "运动完成超时");
                    return;
                }
            }

            if (checkBox_MoveByCapture.Checked) btnCapture_Click(null, null);
            MoveEnable = true;
        }

        private void btnConfig_Cam_Click(object sender, EventArgs e)
        {
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                if (cbb_Cam.SelectedIndex > -1 && cbb_Cam.SelectedIndex < ((CalibStation)_station).Num_Camera)
                {
                    ((CalibStation)_station).SelectedIndex = cbb_Cam.SelectedIndex;
                    ((CalibStation)_station).operation.SaveCameraData();
                }
            }
            else
            {
                if (cbb_Cam.SelectedIndex > -1 && cbb_Cam.SelectedIndex < ((AutoMappingStation)_station).Num_Camera)
                {
                    ((AutoMappingStation)_station).SelectedIndex = cbb_Cam.SelectedIndex;
                    ((AutoMappingStation)_station).operation.SaveCameraData();
                }
            }
        }

        private void btnSaveSnap_Click(object sender, EventArgs e)
        {
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                Directory.CreateDirectory(((CalibStation)_station).imageFolder + "\\Snap");
                HOperatorSet.WriteImage(Image, "tiff", 0, ((CalibStation)_station).imageFolder + "\\Snap\\" + fileName + ".tiff");
            }
            else
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                Directory.CreateDirectory(((AutoMappingStation)_station).imageFolder + "\\Snap");
                HOperatorSet.WriteImage(Image, "tiff", 0, ((AutoMappingStation)_station).imageFolder + "\\Snap\\" + fileName + ".tiff");
            }
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            if (_station.ToString().IndexOf("CalibStation") >= 0)
            {
                Directory.CreateDirectory(((CalibStation)_station).imageFolder + "\\Snap");
                System.Diagnostics.Process.Start("explorer.exe", ((CalibStation)_station).imageFolder + "\\Snap");
            }
            else
            {
                Directory.CreateDirectory(((AutoMappingStation)_station).imageFolder + "\\Snap");
                System.Diagnostics.Process.Start("explorer.exe", ((AutoMappingStation)_station).imageFolder + "\\Snap");
            }
        }
    }
}
