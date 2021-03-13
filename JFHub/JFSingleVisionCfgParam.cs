using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFHub
{
    /// <summary>
    ///  JFVisionCfgParam类：用于保存一种单视野相机视觉配置参数
    ///  包含：可配置的相机参数  可配置的轴数量/位置参数 ， 可配置的光源数量/亮度参数 ， 可配置的触发控制器数量/强度参数
    /// </summary>
    [Serializable]
    public class JFSingleVisionCfgParam
    {
        public JFSingleVisionCfgParam()
        {

        }

        public string Name { get;  set; }


        /// <summary>
        /// 生成本对象的助手名称（）
        /// </summary>
        public string OwnerAssist { get; set; }

        /// <summary>
        /// 相机参数，是否反转X轴
        /// </summary>
        public bool CmrReverseX { get; set; }

        /// <summary>
        /// 相机参数，是否反转Y轴
        /// </summary>
        public bool CmrReverseY { get; set; }

        /// <summary>
        /// 相机参数，曝光时长，单位：毫秒
        /// </summary>
        public double CmrExposure { get; set; }

        /// <summary>
        /// 相机参数,增益
        /// </summary>
        public double CmrGain { get; set; }

        /// <summary>
        /// 光源通道名称
        /// </summary>
        public string[] LightChnNames { get; set; }

        /// <summary>
        /// 各通道光源亮度，0~255
        /// </summary>
        public int[] LightIntensities { get; set; }

        /// <summary>
        /// 各配置轴名称
        /// </summary>
        public string[] AxisNames { get; set; }

        /// <summary>
        /// 各配置轴位置信息
        /// </summary>
        public double[]  AxisPositions{ get; set; }

        /// <summary>
        /// 各触发通道名称 ，定拍模式用不到
        /// </summary>
        public string[] TrigChnNames { get; set; }

        /// <summary>
        /// 各触发通道强度参数 0~255
        /// </summary>
        public int[] TrigIntensities { get; set; }


        /// <summary>
        /// 各触发通道时长参数
        /// </summary>
        public double[] TrigDurations { get; set; }


        /// <summary>
        /// 供测试的方法流（已转化为文本） ， 供示教助手调试使用
        /// </summary>
        public string TestMethodFlowTxt { get; set; }

        /// <summary>
        /// 提供一个可配置的工作流程，用于多次不同配置的拍照组合等...
        /// </summary>
        public string CfgMethodFlowTxt { get; set; }



    }
}
