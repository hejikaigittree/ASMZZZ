using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    public partial class FormMethodCollectionConfigUI : Form
    {
        public FormMethodCollectionConfigUI()
        {
            InitializeComponent();
            ucMethodFlow = new UcMethodFlow();
        }
        //bool isLoaded = false;
        UcMethodFlow ucMethodFlow = null;

        private void FormMethodCollectionConfigUI_Load(object sender, EventArgs e)
        {
            ucMethodFlow.Parent = this;
            ucMethodFlow.Dock = DockStyle.Fill;
        }

        public void SetMethodFlow(JFMethodFlow methodFlow)
        {
            ucMethodFlow.SetMethodFlow(methodFlow);
        }
    }
}
