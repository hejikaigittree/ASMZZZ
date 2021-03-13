using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;

namespace JFMethodCommonLib.工站方法
{
    [JFCategoryLevels(new string[] { "JF 工站方法" })]
    [JFDisplayName("设置工站业务状态")]
    [JFVersion("1.0.0.0")]
    public class JFSM_ChangeCustomStatus_S:JFMethodInitParamBase,IJFStationBaseAcq
    {
        static string PN_CustomStatusName = "业务状态名称";
        public JFSM_ChangeCustomStatus_S()
        {
            DeclearInitParam(PN_CustomStatusName, typeof(string), "");
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }




        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_CustomStatusName && null != _station)
            {
                List<string> optionItems = new List<string>();
                int[] allCustomStatus = _station.AllCustomStatus;
                if (null != allCustomStatus)
                    foreach (int cs in allCustomStatus)
                        optionItems.Add(_station.GetCustomStatusName(cs));
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, optionItems.ToArray());
            }
            return base.GetInitParamDescribe(name);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            errorInfo = "";
            return false;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string csName = GetInitParamValue(PN_CustomStatusName) as string;
            if(string.IsNullOrEmpty(csName))
            {
                errorInfo = PN_CustomStatusName + " 未设置";
                return false;
            }
            errorInfo = "Success";
            return true;

        }
    }
}
