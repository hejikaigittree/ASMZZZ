using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;
using HalconDotNet;
namespace DLAF_DS
{
    [JFDisplayName("DLAF检测结果输出")]
    [JFVersion("1.0.0.0")]
    public class DLAFMethod_InspectResultHandle:JFMethodBase
    {
        public DLAFMethod_InspectResultHandle()
        {
            DeclearInput("ResultHandle", typeof(IDLAFInspectionReceiver), null); //检测结果接受者
            DeclearInput("RecipeID", typeof(string), ""); //当前检测的RecipeID
            DeclearInput("ICRow", typeof(int), -1); //当前检测的行数
            DeclearInput("ICCol", typeof(int), -1);//当前检测的列数
            DeclearInput("FovName", typeof(string), "");

            DeclearInput("DectectResult", typeof(bool), false);
            DeclearInput("DectectErrorInfo", typeof(string), "");
            DeclearInput("DiesErrorCodes", typeof(List<int[]>), null);
            DeclearInput("DieErrorInfos", typeof(string[]), null);
            DeclearInput("BondRegions", typeof(HObject), null);
            DeclearInput("WiresRegions", typeof(HObject), null);
            DeclearInput("FailRegions", typeof(HObject), null);
        }



        protected override bool ActionGenuine(out string errorInfo)
        {
            IDLAFInspectionReceiver rcv = GetMethodInputValue("ResultHandle") as IDLAFInspectionReceiver;
            if (null == rcv)
            {
                errorInfo = "ResultHandle 未设置/空值";
                return false;
            }
            string recipeID = GetMethodInputValue("RecipeID") as string;
            if(string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "RecipeID 未设置/空字串";
                return false;
            }
            rcv.InspectedRecipeID = recipeID;
            rcv.InspectedICRow = Convert.ToInt32(GetMethodInputValue("ICRow"));
            rcv.InspectedICCol = Convert.ToInt32(GetMethodInputValue("ICCol"));
            rcv.InspectedFovName = GetMethodInputValue("FovName") as string;
            rcv.InspectedErrorInfo = GetMethodInputValue("DectectErrorInfo") as string;
            rcv.InspectedResult = Convert.ToBoolean(GetMethodInputValue("DectectResult"));
            rcv.InspectedBondContours =  GetMethodInputValue("BondRegions") as HObject;
            rcv.InspectedWires = GetMethodInputValue("WiresRegions") as HObject;
            rcv.InspectedFailRegs = GetMethodInputValue("FailRegions") as HObject;
            rcv.InspectedReverse = null; //其他测试数据，待拓展
            rcv.InspectedDiesErrorCodes = GetMethodInputValue("DiesErrorCodes") as List<int[]>;
            rcv.InspectedDieErrorInfos = GetMethodInputValue("DieErrorInfos") as string[];
            errorInfo = "Success";
            return true;
        }
    }
}
