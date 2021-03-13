using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLAF
{
    /// <summary>
    /// 用于显示测试结果（矩阵式数据）的控件
    /// </summary>
    public partial class JFUcResultMatrix : UserControl
    {
        public JFUcResultMatrix()
        {
            InitializeComponent();
        }

        private void UcDetectResultMap_Load(object sender, EventArgs e)
        {

        }

        public int RowCount { get; set; }
    }
}
