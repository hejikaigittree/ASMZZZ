using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace DLAF_DS
{
    /// <summary>
    /// DLAF检测结果接收者
    /// </summary>
    internal interface IDLAFInspectionReceiver
    {
        /// <summary>
        /// 已检测的RecipeID
        /// </summary>
        string InspectedRecipeID { get; set; }

        /// <summary>
        /// 已检测的IC行数
        /// </summary>
        int InspectedICRow { get; set; }

        /// <summary>
        /// 已检测的IC列数
        /// </summary>
        int InspectedICCol { get; set; }

        /// <summary>
        /// 已检测的FOV名称
        /// </summary>
        string InspectedFovName { get; set; }


        /// <summary>
        /// 视觉检测流程结果
        /// </summary>
        bool InspectedResult { get; set; }

        /// <summary>
        ///  视觉检测流程错误信息
        /// </summary>
        string InspectedErrorInfo { get; set; }

        HObject InspectedBondContours { get; set; }
        HObject InspectedWires { get; set; }
        HObject InspectedFailRegs { get; set; }



        //重新定义Fov流程后的Die错误信息

        /// <summary>
        /// 多颗Die的检测结果代码
        /// 一个FOV中可能存在多个die ， 每颗Die存在可能多个检测出的错误码
        /// 如错误码只有一个且为0表示OK
        /// </summary>
        List<int[]> InspectedDiesErrorCodes { get; set; }

        /// <summary>
        /// 每颗Die的检测结果描述
        /// </summary>
        string[] InspectedDieErrorInfos { get; set; }


        /// <summary>
        /// 其他可能存在的检测结果
        /// </summary>
        Dictionary<string, object> InspectedReverse { get; set; }






    }
}
