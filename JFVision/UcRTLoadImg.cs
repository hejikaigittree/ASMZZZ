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
using HalconDotNet;

namespace JFVision
{
    public partial class UcRTLoadImg :JFRealtimeUI
    {
        public UcRTLoadImg()
        {
            InitializeComponent();
        }

        private void UcRTLoadImg_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustView();

        }

        bool _isFormLoaded = false;
        //bool _isFilePathEditting = false;
        JFVM_LoadImage _method = null;
        HWindow _hcWnd = new HWindow();
        HObject _hoImge = null;

        public void SetMethodObj(JFVM_LoadImage method)
        {
            _method = method;
            if (_isFormLoaded)
                AdjustView();
        }

        public void AdjustView()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            _hcWnd.OpenWindow(picBox.Location.X, picBox.Location.Y, picBox.Width, picBox.Height, picBox.Handle, "visible", "");
            UpdateSrc2UI();
                
            
        }



        public override void UpdateSrc2UI()
        {
            //base.UpdateSrc2UI();
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSrc2UI));
                return;
            }

            if (null == _method)
            {
                btAction.Enabled = false;
                btSelectFile.Enabled = false;
                tbFilePath.Enabled = false;
                tbFilePath.Text = "";
                tbFilePath.BackColor = SystemColors.Control;
                tssImgWidth.Text = "宽:未知";
                tssImgHeight.Text = "高:未知";
                tssSpanSeconds.Text = "耗时:未执行";
                tssInfo.Text = "信息:方法对象未设置！";
                return;
            }
            btAction.Enabled = true;
            btSelectFile.Enabled = true;
            tbFilePath.Enabled = true;
            tbFilePath.BackColor = SystemColors.Control;
            tbFilePath.Text = _method.GetInitParamValue("FilePath") as string;//_method.GetMethodInputValue("FilePath") as string;
            IJFImage img = _method.GetMethodOutputValue("Image") as IJFImage;
            if (null == img)
            {
                tssImgWidth.Text = "宽:未知";
                tssImgHeight.Text = "高:未知";
                tssSpanSeconds.Text = "耗时:未执行";
                tssInfo.Text = "信息:无图像";
            }
            else
            {
                tssImgWidth.Text = "宽:" + img.PicWidth;
                tssImgHeight.Text = "高:" + img.PicHeight;
                tssSpanSeconds.Text = "耗时:" + _method.GetActionSeconds().ToString("F3");
                tssInfo.Text = "信息:图像正常";
                ShowImage(img);
            }


        }

        delegate void dgShowTips(string txt);
        public void ShowTips(string txt)
        {
            if(InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { txt });
                return;
            }
            tssInfo.Text = "信息:" + txt;
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSelectFile_Click(object sender, EventArgs e)
        {
            if (null == _method)
            {
                ShowTips("无效操作，方法对象未设置");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图片文件(*.*)|*.*";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _method.SetInitParamValue("FilePath", ofd.FileName);
                tbFilePath.BackColor = SystemColors.Control;
                tbFilePath.Text = ofd.FileName;
                //_isFilePathEditting = false;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAction_Click(object sender, EventArgs e)
        {
            if (null == _method)
            {
                ShowTips("无效操作，方法对象未设置");
                return;
            }
            bool isOK = _method.Action();
            if(!isOK)
            {
                ShowTips("执行失败！错误信息:" + _method.GetActionErrorInfo());
                UpdateSrc2UI();
                return;
            }
        }

        private void tbFilePath_TextChanged(object sender, EventArgs e)
        {
            if (null == _method)
                return;
            string currPath = _method.GetInitParamValue("FilePath") as string;
            if(currPath != tbFilePath.Text)
                tbFilePath.BackColor = Color.Orange;
            else
                tbFilePath.BackColor = Color.White;
        }

        private void tbFilePath_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape) //取消编辑
            {
                //_isFilePathEditting = false;
                UpdateSrc2UI();
            }
            else if(e.KeyCode == Keys.Enter)
            {
                if(null != _method)
                    _method.SetInitParamValue("FilePath", tbFilePath.Text);
                //_isFilePathEditting = false;
                UpdateSrc2UI();
            }
        }

        private void tbFilePath_Enter(object sender, EventArgs e)
        {
            //_isFilePathEditting = true;
            tbFilePath.BackColor = Color.White;
        }

        private void tbFilePath_Leave(object sender, EventArgs e)
        {
            //_isFilePathEditting = false;
            UpdateSrc2UI();
        }

        
        public void ShowImage(IJFImage img)
        {
            if (null == img)
            {
                _hcWnd.ClearWindow();
                return;
            //img.DisplayTo(pPicWnd);
            }
            else
            {
                if(null != _hoImge)
                {
                    _hoImge.Dispose();
                    _hoImge = null;
                }
                object oImg = null;
                int err = img.GenHalcon(out oImg);
                if (0 != err)
                {
                    _hcWnd.ClearWindow();
                    ShowTips("图像显示失败!错误信息:" + img.GetErrorInfo(err));
                }
                _hoImge = oImg as HObject;
                HOperatorSet.SetPart(_hcWnd, 0, 0, img.PicHeight - 1, img.PicWidth - 1);// ch: 使图像显示适应窗口大小 || en: Make the image adapt the window size

                HOperatorSet.DispObj(_hoImge, _hcWnd);
                ShowTips("图像已显示");
            }
        }

        private void UcRTLoadImg_SizeChanged(object sender, EventArgs e)
        {
            _hcWnd.CloseWindow();
            if (picBox.Width > 0 && picBox.Height > 0)
            {
                _hcWnd.OpenWindow(picBox.Location.X, picBox.Location.Y, picBox.Width, picBox.Height, picBox.Handle, "visible", "");
                if (null != _hoImge)
                {
                    HTuple hWidth, hHeight;
                    HOperatorSet.GetImageSize(_hoImge, out hWidth, out hHeight);
                    HOperatorSet.SetPart(_hcWnd, 0, 0, hHeight - 1, hWidth - 1);
                    HOperatorSet.DispObj(_hoImge, _hcWnd);
                }
                else
                    _hcWnd.ClearWindow();
            }
            //ShowImage(_hoImge);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if(_hoImge != null)
            {
                _hoImge.Dispose();
                _hoImge = null;
            }
        }
    }
}
