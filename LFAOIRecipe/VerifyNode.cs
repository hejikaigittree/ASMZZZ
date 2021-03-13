using System;
using System.Windows.Forms;
using JFInterfaceDef;
using LFAOIRecipe.Algorithm;

namespace LFAOIRecipe
{
    public partial class VerifyNode : JFRealtimeUI
    {
        public VerifyNode()
        {
            InitializeComponent();
        }

        private void VerifyNode_Load(object sender, EventArgs e)
        {
            Page_InspectNode mui = new Page_InspectNode();
            elementHost1.Child = mui;
        }

        public override void UpdateSrc2UI()
        {

        }

        JFVerifyNode _methodObj = null;
        public void SetMethodObj(JFVerifyNode method)
        {
            _methodObj = method;
        }
    }
}
