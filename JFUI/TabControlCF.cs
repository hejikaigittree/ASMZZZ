using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFUI
{
    [ToolboxBitmap(typeof(TabControl))]
    public partial class TabControlCF : TabControl
    {



        private Color mTabColor = SystemColors.Control;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TabControlCF()
        {
            InitializeComponent();
            // double buffering
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SizeMode = TabSizeMode.Fixed;
            TabColor = SystemColors.ControlDark;
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public Color TabColor
        {
            get { return mTabColor; }
            set { mTabColor = value; this.Invalidate(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawControl(e.Graphics);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        internal void DrawControl(Graphics g)
        {
            if (!Visible)
                return;

            Rectangle TabControlArea = this.ClientRectangle;
            Rectangle TabArea = this.DisplayRectangle;

            // fill client area
            Brush br = new SolidBrush(this.Parent.BackColor); //(SystemColors.Control); UPDATED
            g.FillRectangle(br, TabControlArea);
            br.Dispose();

            // draw border
            //    int nDelta = SystemInformation.Border3DSize.Width;
            Pen border = new Pen(SystemColors.ControlDark);
            border.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            //TabArea.Inflate(nDelta, nDelta);
            g.DrawRectangle(border, TabArea);
            border.Dispose();


            // draw tabs
            for (int i = 0; i < this.TabCount; i++)
                DrawTab(g, this.TabPages[i], i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tabPage"></param>
        /// <param name="nIndex"></param>
        internal void DrawTab(Graphics g, TabPage tabPage, int nIndex)
        {
            Rectangle recBounds = this.GetTabRect(nIndex);
            RectangleF tabTextArea = (RectangleF)this.GetTabRect(nIndex);

            bool bSelected = (this.SelectedIndex == nIndex);

            Point[] pt = new Point[7];
            if (this.Alignment == TabAlignment.Top)
            {
                pt[0] = new Point(recBounds.Left, recBounds.Bottom);
                pt[1] = new Point(recBounds.Left, recBounds.Top + 5);
                pt[2] = new Point(recBounds.Left + 5, recBounds.Top);
                pt[3] = new Point(recBounds.Right - 5, recBounds.Top);
                pt[4] = new Point(recBounds.Right, recBounds.Top + 5);
                pt[5] = new Point(recBounds.Right, recBounds.Bottom);
                pt[6] = new Point(recBounds.Left, recBounds.Bottom);

            }
            else if (this.Alignment == TabAlignment.Bottom)
            {
                pt[0] = new Point(recBounds.Left, recBounds.Top);
                pt[1] = new Point(recBounds.Right, recBounds.Top);
                pt[2] = new Point(recBounds.Right, recBounds.Bottom - 5);
                pt[3] = new Point(recBounds.Right - 5, recBounds.Bottom);
                pt[4] = new Point(recBounds.Left + 5, recBounds.Bottom);
                pt[5] = new Point(recBounds.Left, recBounds.Bottom - 5);
                pt[6] = new Point(recBounds.Left, recBounds.Top);

            }
            else if (this.Alignment == TabAlignment.Left)
            {
                pt[0] = new Point(recBounds.Left, recBounds.Top + 5);
                pt[1] = new Point(recBounds.Left + 5, recBounds.Top);
                pt[2] = new Point(recBounds.Right, recBounds.Top);
                pt[3] = new Point(recBounds.Right, recBounds.Bottom);
                pt[4] = new Point(recBounds.Left + 5, recBounds.Bottom);
                pt[5] = new Point(recBounds.Left, recBounds.Bottom - 5);
                pt[6] = new Point(recBounds.Left, recBounds.Top + 5);

            }
            else  //right
            {
                pt[0] = new Point(recBounds.Left, recBounds.Top);
                pt[1] = new Point(recBounds.Right - 5, recBounds.Top);
                pt[2] = new Point(recBounds.Right, recBounds.Top + 5);
                pt[3] = new Point(recBounds.Right, recBounds.Bottom - 5);
                pt[4] = new Point(recBounds.Right - 5, recBounds.Bottom);
                pt[5] = new Point(recBounds.Left, recBounds.Bottom);
                pt[6] = new Point(recBounds.Left, recBounds.Top);

            }

            if (bSelected)
            {
                //----------------------------
                // fill this tab with background color
                Brush brush = new SolidBrush(tabPage.BackColor);
                g.FillPolygon(brush, pt);
                brush.Dispose();
                // draw border
                //g.DrawRectangle(SystemPens.ControlDark, recBounds);
                g.DrawPolygon(SystemPens.ControlDark, pt);

            }
            else
            {
                Brush brush = new SolidBrush(TabColor);
                g.FillPolygon(brush, pt);
                brush.Dispose();
                // draw border
                //g.DrawRectangle(SystemPens.ControlDark, recBounds);
                g.DrawPolygon(SystemPens.ControlDark, pt);
            }
            // draw tab's icon
            if ((tabPage.ImageIndex >= 0) && (ImageList != null) && (ImageList.Images[tabPage.ImageIndex] != null))
            {
                int nLeftMargin = 8;
                int nRightMargin = 2;

                Image img = ImageList.Images[tabPage.ImageIndex];

                Rectangle rimage = new Rectangle(recBounds.X + nLeftMargin, recBounds.Y + 1, img.Width, img.Height);

                // adjust rectangles
                float nAdj = (float)(nLeftMargin + img.Width + nRightMargin);

                rimage.Y += (recBounds.Height - img.Height) / 2;
                tabTextArea.X += nAdj;
                tabTextArea.Width -= nAdj;

                // draw icon
                g.DrawImage(img, rimage);
            }
            // draw string
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Brush br = new SolidBrush(tabPage.ForeColor);

            g.DrawString(tabPage.Text, Font, br, tabTextArea, stringFormat);
        }


    }
}
