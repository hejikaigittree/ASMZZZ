using JFInterfaceDef;
using LFAOIRecipe.Algorithm;
using System;

namespace LFAOIRecipe
{
    public partial class ucListFoldersImage : JFRealtimeUI
    {
        public ucListFoldersImage()
        {
            InitializeComponent();
        }

        private void ucListFoldersImage_Load(object sender, EventArgs e)
        {
            //MainForm fm = new MainForm();
            //fm.FormBorderStyle = FormBorderStyle.None;
            //fm.Dock = DockStyle.Fill;
            //Controls.Add(fm);
            //fm.Parent = this;
            //fm.Show();
            MainUI mui = new MainUI();
            elementHost1.Child = mui;

            //Controls.Add(mui);
        }

        //MainUI mui = null;

        public override void UpdateSrc2UI()
        {

        }

        VM_ListImageFile _methodObj = null;
        public void SetMethodObj(VM_ListImageFile method)
        {
            _methodObj = method;
        }
    }
}
