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

namespace JFUI
{
    public partial class UcAIOPanel : UserControl
    {
        public UcAIOPanel()
        {
            InitializeComponent();
        }

        bool _isLoaded = false;
        private void UcAIOPanel_Load(object sender, EventArgs e)
        {
            _isLoaded = true;
        }
        bool _isAnologOut = false;
        [Category("JF属性"), Description("模拟量输出面板"), Browsable(true)]
        public bool IsAnalogOut
        {
            get{ return _isAnologOut; }
            set
            {
                _isAnologOut = value;
                lbTital.Text = _isAnologOut ? "Analog-OUT Panel" : "Analog-IN Panel";
            }
        }

        public void AddIO(IJFModule_AIO aio,int index,string name = null)
        {
            if (name == null)
                name = (IsAnalogOut ? "Out_" : "In_") + index.ToString("D2");
            foreach (UcAIOChn uc in lstUcAio)
                if (uc.AIOModule == aio && index == uc.IOIndex)
                    return;
            UcAIOChn uc2Add = new UcAIOChn();
            uc2Add.Height = 23;
            uc2Add.SetIOInfo(aio, index, IsAnalogOut, name);
            Controls.Add(uc2Add);
            lstUcAio.Add(uc2Add);
            if (_isLoaded)
                AdjustIOView();
        }

        public void RemoveIO(IJFModule_AIO aio, int index)
        {
            foreach (UcAIOChn uc in lstUcAio)
                if (uc.AIOModule == aio && index == uc.IOIndex)
                {
                    lstUcAio.Remove(uc);
                    Controls.Remove(uc);
                    AdjustIOView();
                    return;
                }
        }

        public void RemoveAllAIO()
        {
            foreach (UcAIOChn uc in lstUcAio)
                Controls.Remove(uc);
            lstUcAio.Clear();
            AdjustIOView();
        }

        int _ucMargin = 2; //IO控件之间的距离
        public void AdjustIOView()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AdjustIOView));
                return;
            }
            int locY = lbTital.Location.Y + lbTital.Height + 5;
            foreach(UcAIOChn uc in lstUcAio)
            {
                uc.Location = new Point(_ucMargin, locY);

                locY += uc.Height + _ucMargin;
                uc.Width = Width > uc.MaximumSize.Width + _ucMargin * 2 ? uc.MaximumSize.Width : (Width < uc.MinimumSize.Width + _ucMargin * 2 ? uc.MinimumSize.Width : Width - _ucMargin * 2);
            }
        }

        public string[] AIONames 
        { 
            get
            {
                List<string> ret = new List<string>();
                foreach (UcAIOChn uc in lstUcAio)
                    ret.Add(uc.IOName);
                return ret.ToArray();
            }
        }

        bool _isNameEditting = false;
        public bool IsNamesEditting
        {
            get { return _isNameEditting; }
            set
            {
                _isNameEditting = value;
                foreach (UcAIOChn uc in lstUcAio)
                    uc.IsEditting = _isNameEditting;
            }
        }


        public void UpdateSrc2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateSrc2UI));
                return;
            }
            foreach (UcAIOChn uc in lstUcAio)
                uc.UpdateIO();
        }

        List<UcAIOChn> lstUcAio = new List<UcAIOChn>(); //UcAoi控件


    }
}
