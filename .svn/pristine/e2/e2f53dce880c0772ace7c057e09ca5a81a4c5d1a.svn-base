using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 线性标定参数结构
    /// </summary>
    public struct JFLinearCalibData
    {
        //public JFLinearCalibData()
        //{

        //}

        /// <summary>
        /// （图像）坐标原点在物理坐标系中的坐标X
        /// </summary>
        public double ZeroXInWCS 
        { 
            get { return zeroXInWCS; }
            set { zeroXInWCS = value; }
         }


        /// <summary>
        /// （图像）坐标原点在物理坐标系中的坐标Y
        /// </summary>
        public double ZeroYInWCS
        {
            get { return zeroYInWCS; }
            set { zeroYInWCS = value; }
        }


        /// <summary>
        /// （图像）像素当量X ， FactorX = 0.01 表示一个像素宽度的实际物理长度为0.01(mm)
        /// </summary>
        public double FactorX
        {
            get { return factorX; }
            set { factorX = value; }
        }

        /// <summary>
        /// （图像）像素当量Y ， FactorY = 0.01 表示一个像素宽度的实际物理长度为0.01(mm)
        /// </summary>
        public double FactorY
        {
            get { return factorY; }
            set { factorY = value; }
        }

        /// <summary>
        /// 图像坐标轴X 在物理坐标系中的角度，单位为度，逆时针为正方向
        /// </summary>
        public double LineXAngleInWCS
        {
            get { return lineXAngleInWCS; }
            set { lineXAngleInWCS = value; }
        }


        /// <summary>
        /// 图像坐标轴Y 在图像坐标系中的角度，单位为度，逆时针为正方向
        /// </summary>
        public double LineYAngleInLoc
        {
            get { return lineYAngleInLoc; }
            set { lineYAngleInLoc = value; }
        }


        //（图像）坐标原点在物理坐标系中的坐标
        double zeroXInWCS ;
        double zeroYInWCS ;


        ///（ 像素）当量 ， （一个图像）宽度表示的实际物理距离
        double factorX ;
        double factorY ;

        double lineXAngleInWCS ; //X轴在物理坐标系中的角度,单位为度 ( 和物理坐标系X轴的夹角)
        double lineYAngleInLoc ; //本地（图像）坐标系中X轴到Y轴的夹角,逆时针为正
    }
}
