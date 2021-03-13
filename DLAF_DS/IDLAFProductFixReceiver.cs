using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAF_DS
{
    /// <summary>
    /// 能接受DLAF产品标定结果的接口类
    /// </summary>
    internal interface IDLAFProductFixReceiver
    {
        //矫正产品的ID（主要用于验证）
        string PFRecipeID { get; set; }

        /// <summary>
        /// 产品标定结果 ， 0表示标定成功 PF = product fix
        /// </summary>
        int PFErrorCode { get; set; } 

        /// <summary>
        /// 产品标定错误信息
        /// </summary>
        string PFErrorInfo { get; set; }

        //矫正后的IC中心X坐标
        double[] PFICCenterX { get; set; }

        /// <summary>
        /// 矫正后的IC中心Y
        /// </summary>
        double[] PFICCenterY { get; set; }

        /// <summary>
        /// 校正后的Fov offset X
        /// </summary>
        double[] PFFovOffsetX { get; set; }


        /// <summary>
        /// 校正后的Fov offset Y
        /// </summary>
        double[] PFFovOffsetY { get; set; }

    }
}
