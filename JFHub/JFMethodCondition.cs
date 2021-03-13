using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFHub
{
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制" })]
    [JFDisplayName("IF_ELSE")]
    public class JFMethodCondition : JFMethodInitParamBase, IJFMethod_T, IJFStationBaseAcq, IJFMethodOutterDataPoolAcq, IJFConfigUIProvider
    {
        static string PN_TrueFlowText = "TrueMethodFlow"; //处理true的方法流
        static string PN_FalseFlowText = "FalseMethodFlow";
        static string IN_BoolVal = "Bool条件变量"; //外部输入的待判断条件

        public JFMethodCondition()
        {
            string trueFlowText = _trueFlow.ToTxt();
            string falseFlowText = _falseFlow.ToTxt();
            DeclearInitParam(PN_TrueFlowText, typeof(string), trueFlowText);
            DeclearInitParam(PN_FalseFlowText, typeof(string), falseFlowText);
            DeclearInput(IN_BoolVal, typeof(bool), true);

        }

        public JFMethodFlow TrueFlow{ get { return _trueFlow; } }
        public JFMethodFlow FalseFlow { get { return _falseFlow; } }

        protected override bool ActionGenuine(out string errorInfo)
        {
            bool val = Convert.ToBoolean(GetMethodInputValue(IN_BoolVal));
            bool ret = true;
            errorInfo = "Success";
            if (val)
            {
                ret = _trueFlow.Action();
                if (!ret)
                    errorInfo = _trueFlow.ActionErrorInfo();
            }
            else
            {
                ret = _falseFlow.Action();
                if (!ret)
                    errorInfo = _falseFlow.ActionErrorInfo();
            }
            return ret;
        }

        public override bool SetInitParamValue(string name,object val)
        {
            //return base.SetInitParamValue(name, val);
            string flowTxt = val as string;
            if (name == PN_TrueFlowText)
                return _trueFlow.FromTxt(flowTxt);
            else
                return _falseFlow.FromTxt(flowTxt);
        }

        public override object GetInitParamValue(string name)
        {
            if (name == PN_TrueFlowText)
                return _trueFlow.ToTxt();
            else if (name == PN_FalseFlowText)
                return _falseFlow.ToTxt();
            return null;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            //_trueFlow.Clear();
            //_trueFlow.Clear();

            //string trueFlowTxt = GetInitParamValue(PN_TrueFlowText) as string;
            //string falseFlowTxt = GetInitParamValue(PN_FalseFlowText) as string;
            try
            {
                StringBuilder sbError = new StringBuilder();

                bool isOK = true;
                //if(!_trueFlow.FromTxt(trueFlowTxt))
                //{
                //    sbError.AppendLine("TrueFlow初始化失败");
                //    isOK = false;
                //}

                //if(!_falseFlow.FromTxt(falseFlowTxt))
                //{
                //    sbError.AppendLine("FalseFlow初始化失败");
                //    isOK = false;
                //}
                if (isOK)
                    errorInfo = "Success";
                else
                    errorInfo = sbError.ToString();
                return isOK;
            }
            catch(Exception ex)
            {
                errorInfo = ex.ToString();
                return false;
            }
        }

        public void Pause()
        {
            bool isTrueSwitch = Convert.ToBoolean(GetMethodInputType(IN_BoolVal));
            if (isTrueSwitch)
                _trueFlow.Pause();
            else
                _falseFlow.Pause();
        }

        public void Resume()
        {
            bool isTrueSwitch = Convert.ToBoolean(GetMethodInputType(IN_BoolVal));
            if (isTrueSwitch)
                _trueFlow.Resume();
            else
                _falseFlow.Resume();
        }

        public void Exit()
        {
            bool isTrueSwitch = Convert.ToBoolean(GetMethodInputType(IN_BoolVal));
            if (isTrueSwitch)
                _trueFlow.Stop();
            else
                _falseFlow.Stop();
        }

        JFStationBase _stationBase = null;
        public void SetStation(JFStationBase station)
        {
            _stationBase = station;
            _trueFlow.SetStation(_stationBase);
            _falseFlow.SetStation(_stationBase);
        }

        public void SetOutterDataPool(Dictionary<string, object> outterDataPool, Dictionary<string, Type> outterTypePool, string[] outterAvailedIDs)
        {
            _trueFlow.SetOutterDataPool(outterDataPool, outterTypePool, outterAvailedIDs);
            _falseFlow.SetOutterDataPool(outterDataPool, outterTypePool, outterAvailedIDs);
        }

        public void ShowCfgDialog()
        {
            FormMethodConditionCfgUI fm = new FormMethodConditionCfgUI();
            fm.SetMethodFlow(this);
            fm.ShowDialog();
        }

        JFMethodFlow _trueFlow = new JFMethodFlow();
        JFMethodFlow _falseFlow = new JFMethodFlow();
    }
}
