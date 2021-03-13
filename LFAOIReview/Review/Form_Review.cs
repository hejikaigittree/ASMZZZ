using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LFAOIReview
{
    public partial class Form_Review : Form
    {
        public Form_Review()
        {
            //if (!File.Exists(Application.StartupPath + "\\" + "NPOI.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "NPOI.dll", Properties.Resources.NPOI);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "MaterialDesignColors.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "MaterialDesignColors.dll", Properties.Resources.MaterialDesignColors);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "MaterialDesignThemes.Wpf.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "MaterialDesignThemes.Wpf.dll", Properties.Resources.MaterialDesignThemes_Wpf);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "ICSharpCode.SharpZipLib.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "ICSharpCode.SharpZipLib.dll", Properties.Resources.ICSharpCode_SharpZipLib);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "NPOI.OOXML.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "NPOI.OOXML.dll", Properties.Resources.NPOI_OOXML);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "NPOI.OpenXml4Net.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "NPOI.OpenXml4Net.dll", Properties.Resources.NPOI_OpenXml4Net);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "NPOI.OpenXmlFormats.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "NPOI.OpenXmlFormats.dll", Properties.Resources.NPOI_OpenXmlFormats);
            //}
            //if (!File.Exists(Application.StartupPath + "\\" + "System.Data.SQLite.dll"))
            //{
            //    File.WriteAllBytes(Application.StartupPath + "\\" + "System.Data.SQLite.dll", Properties.Resources.System_Data_SQLite);
            //}


            InitializeComponent();
            this.review1 = new LFAOIReview.Review();
            this.elementHost1.Child = this.review1;
        }
        public Form_Review(DataShow dataShow)
        {
            InitializeComponent();
            this.review1 = new LFAOIReview.Review(dataShow);
            this.elementHost1.Child = this.review1;
        }


        private void Form_Review_SizeChanged(object sender, EventArgs e)
        {
            review1.Height = elementHost1.Height;
            review1.Width = elementHost1.Width;
        }
    }
}
