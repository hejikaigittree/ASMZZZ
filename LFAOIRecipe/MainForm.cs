using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFAOIRecipe
{
    public partial class MainForm : Form
    {
        MainUI mainUI = new MainUI();

        public MainForm()
        {
            InitializeComponent();
            this.elementHost1.Child = this.mainUI;
            this.SizeChanged += MainForm_SizeChanged;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            mainUI.Height = elementHost1.Height;
            mainUI.Width = elementHost1.Width;
        }
    }
}
