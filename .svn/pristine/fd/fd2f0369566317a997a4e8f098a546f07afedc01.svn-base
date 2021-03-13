using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFUI
{
    public partial class LampButton : Button
    {
        ControlState _state = ControlState.Normal;
        /// <summary>
        /// 
        /// </summary>
        public LampButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ControlState
        {
            /// <summary>
            /// 
            /// </summary>
            Normal,
            /// <summary>
            /// 
            /// </summary>
            Hover,
            /// <summary>
            /// 
            /// </summary>
            Pressed,
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _state = ControlState.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mevent"></param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            _state = ControlState.Hover;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _state = ControlState.Pressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _state = ControlState.Hover;
        }
        private void CalculateRect(
        out Rectangle imageRect, out Rectangle textRect, Graphics g)
        {
            if (Image != null)
            {

                //imageRect = new Rectangle(0, (ClientRectangle.Height - Image.Size.Height) / 2,
                //    Image.Size.Width, Image.Size.Height);
                //textRect = new Rectangle(Image.Size.Width, 0,
                //    ClientRectangle.Width - Image.Width, ClientRectangle.Height);
                imageRect = new Rectangle(0, ClientRectangle.Height/6,
                    ClientRectangle.Height *2/3, ClientRectangle.Height * 2 / 3);
                textRect = new Rectangle(ClientRectangle.Height *5/ 6, 0,
                    ClientRectangle.Width - ClientRectangle.Height * 5 / 6, ClientRectangle.Height);

            }
            else
            {
                imageRect = new Rectangle(0, 0, 0, 0);
                textRect = ClientRectangle;
            }

        }

        /// <summary>
        /// 画边框与背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="style"></param>
        /// <param name="roundWidth"></param>
        internal void RenderBackGroundInternal(Graphics g, Rectangle rect, RoundInRectStyle style, int roundWidth)
        {
            //       if (ControlState != ControlState.Normal || AlwaysShowBorder)
            //       {
            //      rect.Width--;
            //       rect.Height--;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            
            //         base.OnPaint(e);
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            Rectangle imageRect;
            Rectangle textRect;

            CalculateRect(out imageRect, out textRect, g);
            g.SmoothingMode = SmoothingMode.HighQuality;


            //  画边框与背景
            //RenderBackGroundInternal(
            //g,
            //    ClientRectangle,
            //    RoundStyle,
            //    Radius
            //            );
            //   画图像
            if (_state == ControlState.Normal)
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }
            else if (_state == ControlState.Hover)
            {
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.LightGray))
                {
                    g.FillRectangle(brush, ClientRectangle);
                }
            }
            else if (_state == ControlState.Pressed)
            {
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Gainsboro))
                {
                    g.FillRectangle(brush, ClientRectangle);
                }
            }

            if (Image != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(Image, imageRect.Left, imageRect.Top, imageRect.Width,imageRect.Height);
                //g.DrawImage(
                //    Image,
                //    imageRect,
                //    0,
                //    0,
                //    Image.Width,
                //    Image.Height,
                //    GraphicsUnit.Pixel);
            }
            //画文字      
            if (Text != "")
            {
                //    TextFormatFlags flags = base.TextAlign;
                TextRenderer.DrawText(
                    g,
                    Text,
                    Font,
                   textRect,
                    //        this.ClientRectangle,
                    ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                //StringFormat sf = new StringFormat();
                //sf.Alignment = StringAlignment.Center;
                //sf.LineAlignment = StringAlignment.Center;
                //g.DrawString(Text, Font, fontBrush, textRect,  sf);、
            }

        }


        [Category("General"), Description("Control's Value"), Browsable(true)]
        public Size IconSize
        {
            get { return imageList1.ImageSize; }
            set { imageList1.ImageSize = value; }
        }

        public enum LColor
        {
            Gray = 0,
            Green = 1,
            Red ,
            Yellow
        }

        public LColor LampColor
        {
            get
            {
                return (LColor)ImageIndex;
            }
            set
            {
                ImageIndex = (int)value;
            }
        }
    }
}
