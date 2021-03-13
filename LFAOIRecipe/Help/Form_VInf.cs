using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace LFAOIRecipe
{
    public partial class Form_Lv_Inf : Form
    {
        public Form_Lv_Inf()
        {
            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            textBox_Lv_program.Text = "Recipe 版本：" + " " + version.Major + "." + version.Minor + " (build " + version.Build + ")" + "." + version.Revision; //change form title
            //textBox_Lv_program.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
