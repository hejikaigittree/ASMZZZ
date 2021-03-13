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
    public partial class RoundButton : Button
    {
        public enum MouseState
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




        //private Color _borderColor = Color.FromArgb(161, 162, 160);
        //private Color _innerBorderColor = Color.FromArgb(200, 255, 255, 255);
        private Color _baseColor = Color.FromArgb(174, 218, 151);
        private Color _baseColorEnd = Color.FromArgb(174, 218, 151);
        private Color _arrowColor = Color.FromArgb(64, 64, 64);
        private int _imageWidth = 80;
        private int _imageHeight = 80;
        private RoundInRectStyle _roundStyle = RoundInRectStyle.All;
        private int _radius = 24;
        private int _imageTextSpace = 2;
        private bool _pressOffset = true;
        private bool _alwaysShowBorder = false;
        private bool _showSpliteButton = false;
        private int _spliteButtonWidth = 18;
        private MouseState _controlState;
        private ButtonMousePosition _mousePosition;

        private bool _contextHandle;
        private bool _contextOpened;
        private int _contextOffset = 5;

        /// <summary>         /// 普通按钮按下事件         /// </summary> 
        public event EventHandler OnButtonClick;
        /// <summary>         /// 分割按钮按下事件         /// </summary>     
        public event EventHandler OnSpliteButtonClick;

        /// <summary>         /// 鼠标的当前位置         /// </summary> 
        public enum ButtonMousePosition
        {
            /// <summary>
            /// 
            /// </summary>
            None,
            /// <summary>
            /// 
            /// </summary>
            Button,
            /// <summary>
            /// 
            /// </summary>
            Splitebutton,
        }

        /// <summary>
        /// 
        /// </summary>
        public RoundButton()
                    : base()
        {
            SetStyle(
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Opaque, false);

            this._controlState = MouseState.Normal;
            this.BackColor = Color.Transparent;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(5)]
        [Description("下拉菜单与按钮的距离")]
        public int ContextOffset
        {
            get { return _contextOffset; }
            set
            {
                _contextOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        [Description("是否启用分割按钮")]
        public bool ShowSpliteButton
        {
            get { return _showSpliteButton; }
            set
            {
                if (_showSpliteButton != value)
                {
                    _showSpliteButton = value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        [Description("分割按钮的宽度")]
        public int SpliteButtonWidth
        {
            get { return _spliteButtonWidth; }
            set
            {
                if (_spliteButtonWidth != value)
                {
                    _spliteButtonWidth = value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        [Description("当鼠标按下时图片和文字是否产生偏移")]
        public bool PressOffset
        {
            get { return _pressOffset; }
            set
            {
                _pressOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        [Description("是否一直显示按钮边框\n设置为false则只在鼠标经过和按下时显示边框")]
        public bool AlwaysShowBorder
        {
            get { return _alwaysShowBorder; }
            set
            {
                if (_alwaysShowBorder != value)
                {
                    _alwaysShowBorder = value;
                    base.Invalidate();
                }
            }
        }

        //[DefaultValue(typeof(Color), "64,64,64")]
        //[Description("当显示分割按钮时，分割按钮的箭头颜色")]
        //public Color ArrowColor
        //{
        //    get { return _arrowColor; }
        //    set
        //    {
        //        if (_arrowColor != value)
        //        {
        //            _arrowColor = value;
        //            base.Invalidate();
        //        }
        //    }
        //}

        //[DefaultValue(typeof(Color), "161, 162, 160")]
        //[Description("按钮的边框颜色")]
        //public Color BorderColor
        //{
        //    get { return _borderColor; }
        //    set
        //    {
        //        if (_borderColor != value)
        //        {
        //            _borderColor = value;
        //            base.Invalidate();
        //        }
        //    }
        //}

        //[DefaultValue(typeof(Color), "200, 255, 255, 255")]
        //[Description("按钮内边框颜色")]
        //public Color InnerBorderColor
        //{
        //    get { return _innerBorderColor; }
        //    set
        //    {
        //        if (_innerBorderColor != value)
        //        {
        //            _innerBorderColor = value;
        //            base.Invalidate();
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(typeof(Color), "10 ,66, 204, 160")]
        [Description("鼠标经过和按下时按钮的渐变背景颜色")]
        public Color BaseColor
        {
            get { return _baseColor; }
            set
            {
                if (_baseColor != value)
                {
                    _baseColor = value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(typeof(Color), "200 ,66, 204, 160")]
        [Description("鼠标经过和按下时按钮的渐变背景颜色")]
        public Color BaseColorEnd
        {
            get { return _baseColorEnd; }
            set
            {
                if (_baseColorEnd != value)
                {
                    _baseColorEnd = value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(24)]
        [Description("图片宽度")]
        public int ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                if (value != _imageWidth)
                {

                    _imageWidth = value < 12 ? 12 : value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(24)]
        [Description("图片高度")]
        public int ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                if (value != _imageHeight)
                {

                    _imageHeight = value < 12 ? 12 : value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(typeof(RoundInRectStyle), "1")]
        [Description("按钮圆角样式")]
        public RoundInRectStyle RStyle
        {
            get { return _roundStyle; }
            set
            {
                if (_roundStyle != value)
                {
                    _roundStyle = value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(2)]
        [Description("按钮圆角弧度")]
        public int Radius
        {
            get { return _radius; }
            set
            {
                if (_radius != value)
                {
                    _radius = value < 2 ? 2 : value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(2)]
        [Description("图片与文字之间的间距")]
        public int ImageTextSpace
        {
            get { return _imageTextSpace; }
            set
            {
                if (_imageTextSpace != value)
                {
                    _imageTextSpace = value < 0 ? 0 : value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>         /// 按钮当前状态         /// </summary>         
        internal MouseState ControlState
        {
            get { return _controlState; }
            set
            {
                if (_controlState != value)
                {
                    _controlState = value;
                    Invalidate();
                }
            }
        }

        /// <summary>         /// 鼠标当前所在位置         /// </summary>         
        internal ButtonMousePosition CurrentMousePosition
        {
            get { return _mousePosition; }
            set
            {
                if (_mousePosition != value)
                {
                    _mousePosition = value;
                    base.Invalidate();
                }
            }
        }

        /// <summary>         /// 普通按钮矩形位置         /// </summary>         
        internal Rectangle ButtonRect
        {
            get
            {
                if (ShowSpliteButton)
                {
                    return new Rectangle(0, 0, ClientRectangle.Width - SpliteButtonWidth, ClientRectangle.Height);
                }
                else
                {
                    return ClientRectangle;
                }
            }
        }

        /// <summary>         /// 分割按钮矩形位置         /// </summary>         
        internal Rectangle SpliteButtonRect
        {
            get
            {
                if (ShowSpliteButton)
                {
                    return new Rectangle(ClientRectangle.Width - SpliteButtonWidth, 0, SpliteButtonWidth, ClientRectangle.Height);
                }
                else
                {
                    return Rectangle.Empty;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!_contextOpened)
            {
                ControlState = MouseState.Normal;
                CurrentMousePosition = ButtonMousePosition.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mevent"></param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            if (ClientRectangle.Contains(mevent.Location))
            {
                ControlState = MouseState.Hover;
                if (ShowSpliteButton)
                {
                    CurrentMousePosition = ButtonRect.Contains(mevent.Location) ? ButtonMousePosition.Button : ButtonMousePosition.Splitebutton;
                }
                else
                {
                    CurrentMousePosition = ButtonMousePosition.Button;
                }
            }
            else
            {
                ControlState = MouseState.Normal;
                CurrentMousePosition = ButtonMousePosition.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ControlState = MouseState.Pressed;
                if (ShowSpliteButton)
                {
                    CurrentMousePosition = ButtonRect.Contains(e.Location) ? ButtonMousePosition.Button : ButtonMousePosition.Splitebutton;
                }
                else
                {
                    CurrentMousePosition = ButtonMousePosition.Button;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (ClientRectangle.Contains(e.Location))
                {
                    ControlState = MouseState.Hover;
                    if (ShowSpliteButton)
                    {
                        CurrentMousePosition = ButtonRect.Contains(e.Location) ? ButtonMousePosition.Button : ButtonMousePosition.Splitebutton;
                        if (CurrentMousePosition == ButtonMousePosition.Splitebutton)
                        {
                            if (OnSpliteButtonClick != null)
                            {
                                OnSpliteButtonClick(this, EventArgs.Empty);
                            }
                            if (this.ContextMenuStrip != null)
                            {
                                if (!_contextHandle)
                                {
                                    _contextHandle = true;
                                    this.ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
                                    this.ContextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(ContextMenuStrip_Closed);
                                }
                                this.ContextMenuStrip.Opacity = 1.0;
                                this.ContextMenuStrip.Show(this, 0, this.Height + ContextOffset);
                            }
                        }
                        else
                        {
                            if (OnButtonClick != null)
                            {
                                OnButtonClick(this, EventArgs.Empty);
                            }
                        }
                    }
                    else
                    {
                        CurrentMousePosition = ButtonMousePosition.Button;
                    }
                }
                else
                {
                    ControlState = MouseState.Normal;
                    CurrentMousePosition = ButtonMousePosition.None;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _contextOpened = false;
            ControlState = MouseState.Normal;
            CurrentMousePosition = ButtonMousePosition.None;
            base.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _contextOpened = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //      base.OnPaint(e);
            //      base.OnPaintBackground(e);

            Graphics g = e.Graphics;
            Rectangle imageRect;
            Rectangle textRect;

            CalculateRect(out imageRect, out textRect, g);
            g.SmoothingMode = SmoothingMode.AntiAlias;


            //  画边框与背景
            RenderBackGroundInternal(
            g,
                ClientRectangle,
                RStyle,
                Radius
                        );
            //   画图像
            if (Image != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(
                    Image,
                    imageRect,
                    0,
                    0,
                    Image.Width,
                    Image.Height,
                    GraphicsUnit.Pixel);
            }
            //画文字      
            if (Text != "")
            {
                TextRenderer.DrawText(
                    g,
                    Text,
                    Font,
                   textRect,
                    //        this.ClientRectangle,
                    ForeColor,
                    GetTextFormatFlags(TextAlign, RightToLeft == RightToLeft.Yes));

                //StringFormat sf = new StringFormat();
                //sf.Alignment = StringAlignment.Center;
                //sf.LineAlignment = StringAlignment.Center;
                //g.DrawString(Text, Font, fontBrush, textRect,  sf);、


            }
            //画分割按钮  
            if (ShowSpliteButton)
            {
                RenderSpliteButton(g, ClientRectangle);
            }
        }

        /// <summary>
        /// 获取图像以及文字的位置
        /// </summary>
        /// <param name="imageRect"></param>
        /// <param name="textRect"></param>
        /// <param name="g"></param>
        private void CalculateRect(
            out Rectangle imageRect, out Rectangle textRect, Graphics g)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            SizeF textSize = g.MeasureString(Text, Font);
            //    if (Image == null)
            //    {
            //         switch (TextAlign)
            //         {
            //             case ContentAlignment.BottomCenter:
            //                 textRect = new Rectangle((ButtonRect.Width - (int)textSize.Width) / 2, Height - (int)textSize.Height - 3, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.BottomLeft:
            //                 textRect = new Rectangle(2, Height - (int)textSize.Height - 3, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.BottomRight:
            //                 textRect = new Rectangle(ButtonRect.Width - (int)textSize.Width - 3, Height - (int)textSize.Height - 3, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.MiddleCenter:
            //                 textRect = new Rectangle((ButtonRect.Width - (int)textSize.Width) / 2, (Height - (int)textSize.Height) / 2, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.MiddleLeft:
            //                 textRect = new Rectangle(2, (Height - (int)textSize.Height) / 2, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.MiddleRight:
            //                 textRect = new Rectangle(ButtonRect.Width - (int)textSize.Width - 3, (Height - (int)textSize.Height) / 2, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.TopCenter:
            //                 textRect = new Rectangle((ButtonRect.Width - (int)textSize.Width) / 2, 2, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.TopLeft:
            //                 textRect = new Rectangle(2, 2, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //             case ContentAlignment.TopRight:
            //                 textRect = new Rectangle(ButtonRect.Width - (int)textSize.Width - 3, 2, (int)textSize.Width, (int)textSize.Height);
            //                 break;
            //         }
            //         if (PressOffset && ControlState == ControlState.Pressed && CurrentMousePosition == ButtonMousePosition.Button)
            //         {
            //             textRect.X += 1;
            //             textRect.Y += 1;
            ////             Debug.WriteLine(string.Format("x is {0}, y is {1},", textRect.X, textRect.Y));
            //         }
            //         if (RightToLeft == RightToLeft.Yes)
            //         {
            //             textRect.X = ButtonRect.Width - textRect.Right;
            //         }
            //         return;
            //     }
            if (this.Text == "")
            {
                switch (ImageAlign)
                {
                    case ContentAlignment.BottomCenter:
                        imageRect = new Rectangle((ButtonRect.Width - ImageWidth) / 2, Height - ImageHeight - 3, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.BottomLeft:
                        imageRect = new Rectangle(2, Height - ImageHeight - 3, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.BottomRight:
                        imageRect = new Rectangle(ButtonRect.Width - ImageWidth - 3, Height - ImageHeight - 3, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.MiddleCenter:
                        imageRect = new Rectangle((ButtonRect.Width - ImageWidth) / 2, (Height - ImageHeight) / 2, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.MiddleLeft:
                        imageRect = new Rectangle(2, (Height - ImageHeight) / 2, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.MiddleRight:
                        imageRect = new Rectangle(ButtonRect.Width - ImageWidth - 3, (Height - ImageHeight) / 2, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.TopCenter:
                        imageRect = new Rectangle((ButtonRect.Width - ImageWidth) / 2, 2, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.TopLeft:
                        imageRect = new Rectangle(2, 2, ImageWidth, ImageHeight);
                        break;
                    case ContentAlignment.TopRight:
                        imageRect = new Rectangle(ButtonRect.Width - ImageWidth - 3, 2, ImageWidth, ImageHeight);
                        break;
                }
                if (PressOffset && ControlState == MouseState.Pressed && CurrentMousePosition == ButtonMousePosition.Button)
                {
                    imageRect.X += 1;
                    imageRect.Y += 1;
                }
                if (RightToLeft == RightToLeft.Yes)
                {
                    imageRect.X = ButtonRect.Width - imageRect.Right;
                }
                return;
            }
            switch (TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    imageRect = new Rectangle(
                        ButtonRect.Left,
                         ButtonRect.Top,
                        ButtonRect.Width,
                        ButtonRect.Height);
                    textRect = new Rectangle(
                        ButtonRect.Left,
                         ButtonRect.Top,
                        ButtonRect.Width,
                        ButtonRect.Height);
                    break;
                case TextImageRelation.ImageAboveText:
                    imageRect = new Rectangle(
                        (ButtonRect.Width - ImageWidth) / 2,
                        (Height - ImageHeight - (int)textSize.Height - ImageTextSpace) / 2,
                        ImageWidth,
                        ImageHeight);
                    textRect = new Rectangle(
                        (ButtonRect.Width - (int)textSize.Width) / 2,
                        imageRect.Bottom + ImageTextSpace,
                        (int)textSize.Width,
                        (int)textSize.Height);
                    break;
                case TextImageRelation.ImageBeforeText:
                    imageRect = new Rectangle(
                        (ButtonRect.Width - ImageWidth - (int)textSize.Width - ImageTextSpace) / 2,
                        (Height - ImageHeight) / 2,
                        ImageWidth,
                        ImageHeight);
                    textRect = new Rectangle(
                        imageRect.Right + ImageTextSpace,
                        (Height - (int)textSize.Height) / 2,
                        (int)textSize.Width,
                        (int)textSize.Height);
                    break;
                case TextImageRelation.TextAboveImage:
                    textRect = new Rectangle(
                        (ButtonRect.Width - (int)textSize.Width) / 2,
                        (Height - (int)textSize.Height - ImageHeight - ImageTextSpace) / 2,
                        (int)textSize.Width,
                        (int)textSize.Height);
                    imageRect = new Rectangle(
                        (ButtonRect.Width - ImageWidth) / 2,
                        textRect.Bottom + ImageTextSpace,
                        ImageWidth,
                        ImageHeight);
                    break;
                case TextImageRelation.TextBeforeImage:
                    textRect = new Rectangle(
                        (ButtonRect.Width - ImageWidth - (int)textSize.Width - ImageTextSpace) / 2,
                        (Height - (int)textSize.Height) / 2,
                        (int)textSize.Width,
                        (int)textSize.Height);
                    imageRect = new Rectangle(
                        textRect.Right + ImageTextSpace,
                        (Height - ImageHeight) / 2,
                        ImageWidth,
                        ImageHeight);
                    break;
            }
            if (PressOffset && ControlState == MouseState.Pressed && CurrentMousePosition == ButtonMousePosition.Button)
            {
                imageRect.X += 1;
                imageRect.Y += 1;
                textRect.X += 1;
                textRect.Y += 1;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                imageRect.X = ButtonRect.Width - imageRect.Right;
                textRect.X = ButtonRect.Width - textRect.Right;
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
            rect.Width--;
            rect.Height--;
            if (style != RoundInRectStyle.None)
            {
                using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                {
                    if (ControlState == MouseState.Normal)
                    {
                        using (SolidBrush brush = new SolidBrush(_baseColor))
                        {
                            if (!ShowSpliteButton)
                            {
                                g.FillPath(brush, path);
                            }

                        }
                    }
                    //if (ControlState != ControlState.Normal)
                    else
                    {
                        using (LinearGradientBrush brush = (ControlState == MouseState.Pressed) ? new LinearGradientBrush(rect, _baseColorEnd, _baseColor, LinearGradientMode.ForwardDiagonal) : new LinearGradientBrush(rect, _baseColor, _baseColorEnd, LinearGradientMode.Vertical))
                        {
                            if (!ShowSpliteButton)
                            {
                                g.FillPath(brush, path);
                            }
                            else
                            {
                                if (CurrentMousePosition == ButtonMousePosition.Button)
                                {
                                    using (GraphicsPath buttonpath = GraphicsPathHelper.CreatePath(ButtonRect, roundWidth, RoundInRectStyle.Left, true))
                                    {
                                        g.FillPath(brush, buttonpath);
                                    }
                                }
                                else
                                {
                                    using (GraphicsPath splitepath = GraphicsPathHelper.CreatePath(SpliteButtonRect, roundWidth, RoundInRectStyle.Right, true))
                                    {
                                        g.FillPath(brush, splitepath);
                                    }
                                }
                            }
                        }
                    }
                    //using (Pen pen = new Pen(_borderColor))
                    //{
                    //    g.DrawPath(pen, path);
                    //}
                }
                //rect.Inflate(-1, -1);
                //using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                //{
                //    using (Pen pen = new Pen(InnerBorderColor))
                //    {
                //        g.DrawPath(pen, path);
                //    }
                //}
            }
            else
            {
                if (ControlState != MouseState.Normal)
                {
                    if (ControlState == MouseState.Normal)
                    {
                        using (SolidBrush brush = new SolidBrush(_baseColor))
                        {
                            if (!ShowSpliteButton)
                            {
                                g.FillRectangle(brush, rect);
                            }

                        }
                    }
                    //if (ControlState != ControlState.Normal)
                    else
                    {
                        using (LinearGradientBrush brush = (ControlState == MouseState.Pressed) ? new LinearGradientBrush(rect, _baseColorEnd, _baseColor, LinearGradientMode.ForwardDiagonal) : new LinearGradientBrush(rect, _baseColor, _baseColorEnd, LinearGradientMode.Vertical))

                            if (!ShowSpliteButton)
                            {
                                g.FillRectangle(brush, rect);
                            }
                            else
                            {
                                if (CurrentMousePosition == ButtonMousePosition.Button)
                                {
                                    g.FillRectangle(brush, ButtonRect);
                                }
                                else
                                {
                                    g.FillRectangle(brush, SpliteButtonRect);
                                }
                            }
                    }
                }
                //using (Pen pen = new Pen(_borderColor))
                //{
                //    g.DrawRectangle(pen, rect);
                //}
                //rect.Inflate(-1, -1);
                //using (Pen pen = new Pen(InnerBorderColor))
                //{
                //    g.DrawRectangle(pen, rect);
                //}
            }
            //   }
        }

        /// <summary>
        /// 画分割按钮
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>       
        internal void RenderSpliteButton(Graphics g, Rectangle rect)
        {
            Point[] points = new Point[3];
            points[0] = new Point(rect.Width - SpliteButtonWidth + 2, (rect.Height - 4) / 2);
            points[1] = new Point(rect.Width - SpliteButtonWidth + 2 + 8, (rect.Height - 4) / 2);
            points[2] = new Point(rect.Width - SpliteButtonWidth + 2 + 4, (rect.Height - 4) / 2 + 4);

            if (PressOffset && ControlState == MouseState.Pressed && CurrentMousePosition == ButtonMousePosition.Splitebutton)
            {
                points[0].X += 1;
                points[0].Y += 1;
                points[1].X += 1;
                points[1].Y += 1;
                points[2].X += 1;
                points[2].Y += 1;
            }
            using (SolidBrush brush = new SolidBrush(_arrowColor))
            {
                g.FillPolygon(brush, points);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="rightToleft"></param>
        /// <returns></returns>
        internal static TextFormatFlags GetTextFormatFlags(ContentAlignment alignment, bool rightToleft)
        {
            TextFormatFlags flags = TextFormatFlags.WordBreak;
            //  |TextFormatFlags.SingleLine;
            if (rightToleft)
            {
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }

            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
            }
            return flags;
        }


    }
}
