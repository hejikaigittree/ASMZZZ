using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using JFInterfaceDef;

namespace JFUI
{
    public partial class UcImageView : UserControl
    {
        private IJFImage jFImage = null;
        private Bitmap bitImage;
        private Point mouseDownPoint;
        private bool isSelected = false;
        private bool isShowCross = true;
        private bool isAutoFit = true;
        private object locker = new object();
        public UcImageView()
        {
            InitializeComponent();
            this.ptBox.MouseWheel += PtBox_MouseWheel;
        }

        delegate void dgLoadImage(IJFImage jImage);
        public void LoadImage(IJFImage jImage)
        {
            if (!Created)
                return;
            if(InvokeRequired)
            {
                Invoke(new dgLoadImage(LoadImage), new object[] { jImage });
                return;
            }
            //if (jImage != jFImage)
            //{
            //    if (null != jFImage)
            //    {
            //        jFImage.Dispose();
            //        jFImage = null;
            //    }
            //}
            jFImage = jImage;
            if(null != bitImage)
            {
                bitImage.Dispose();
                bitImage = null;
            }
            if (null != jFImage)
            {
                int iRet = jFImage.GenBmp(out bitImage);
                if (0 == iRet)
                {
                    DrawImage();
                }
                else
                {
                    string errorInfo = jFImage.GetErrorInfo(iRet);
                    this.labErrorInfo.Text = errorInfo;
                }
            }
            else
                labErrorInfo.Text = "Image = null";
        }

        public void UpdateView()
        {
            try
            {
                
                if (null != bitImage)
                {
                    DrawImage();
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                this.labErrorInfo.Text = ex.Message;
            }
        }

        private void DrawImage()
        {
            lock (locker)
            {
                if (bitImage != null)
                {
                    Bitmap tmpImage = (Bitmap)bitImage.Clone();
                    Rectangle targetRectangle = new Rectangle(0, 0, this.ptBox.Width, this.ptBox.Height);
                    BufferedGraphicsContext current = BufferedGraphicsManager.Current;
                    BufferedGraphics buf = current.Allocate(this.ptBox.CreateGraphics(), targetRectangle);
                    buf.Graphics.Clear(this.ptBox.BackColor);
                    buf.Graphics.DrawImage(tmpImage, targetRectangle);
                    if (isShowCross)
                    {
                        buf.Graphics.DrawLine(new Pen(Color.Blue, 1), 0, this.ptBox.Height / 2, this.ptBox.Width, this.ptBox.Height / 2);
                        buf.Graphics.DrawLine(new Pen(Color.Blue, 1), this.ptBox.Width / 2, 0, this.ptBox.Width / 2, this.ptBox.Height);
                    }
                    buf.Render();
                }
            }
        }

        private void AsyncDrawImage(int Delayms)
        {
            Thread theader = new Thread(new ThreadStart(new Action(() =>
            {
                Thread.Sleep(Delayms);
                DrawImage();
            })));
            theader.Start();
        }

        private void PtBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.ptBox.Width < 100 || this.ptBox.Height < 100)
            {
                if (e.Delta < 0)
                {
                    return;
                }
            }
            double scale = 1;
            if (this.ptBox.Height > 0)
            {
                scale = (double)this.ptBox.Width / (double)this.ptBox.Height;
            }
            this.ptBox.Width += (int)(e.Delta * scale);
            this.ptBox.Height += e.Delta;
            isAutoFit = false;
        }

        private void ptBox_MouseEnter(object sender, EventArgs e)
        {
            this.ptBox.Focus();
            this.ptBox.Cursor = Cursors.SizeAll;
        }

        private void ptBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isSelected = true;
            }
        }

        private bool IsMouseInPanel()
        {
            if (this.panPicture.Left < PointToClient(Cursor.Position).X
                    && PointToClient(Cursor.Position).X < this.panPicture.Left + this.panPicture.Width
                    && this.panPicture.Top < PointToClient(Cursor.Position).Y
                    && PointToClient(Cursor.Position).Y < this.panPicture.Top + this.panPicture.Height)
            { return true; }
            else
            { return false; }
        }

        private void ptBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected && IsMouseInPanel())
            {
                this.ptBox.Left = this.ptBox.Left + (Cursor.Position.X - mouseDownPoint.X);
                this.ptBox.Top = this.ptBox.Top + (Cursor.Position.Y - mouseDownPoint.Y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isAutoFit = false;
            }
        }

        private void ptBox_MouseUp(object sender, MouseEventArgs e)
        {
            isSelected = false;
        }

        private void ptBox_Paint(object sender, PaintEventArgs e)
        {
            //DrawImage();
            AsyncDrawImage(65);
        }

        private void btnAutoFit_Click(object sender, EventArgs e)
        {
            this.ptBox.Location = new Point(0, 0);
            this.ptBox.Size = new Size(this.panPicture.Width, this.panPicture.Height);
            isAutoFit = true;
            AsyncDrawImage(15);
        }

        private void ptBox_SizeChanged(object sender, EventArgs e)
        {
            AsyncDrawImage(15);
        }

        private void panPicture_Resize(object sender, EventArgs e)
        {
            if (isAutoFit)
            {
                this.ptBox.Location = new Point(0, 0);
                this.ptBox.Size = new Size(this.panPicture.Width, this.panPicture.Height);
            }
        }

        private void ckbShowCross_CheckedChanged(object sender, EventArgs e)
        {
            this.isShowCross = this.ckbShowCross.Checked;
            DrawImage();
        }

        private void UcImageView_Paint(object sender, PaintEventArgs e)
        {
            AsyncDrawImage(60);
        }

        private void UcImageView_VisibleChanged(object sender, EventArgs e)
        {
            AsyncDrawImage(60);
        }
    }
}
