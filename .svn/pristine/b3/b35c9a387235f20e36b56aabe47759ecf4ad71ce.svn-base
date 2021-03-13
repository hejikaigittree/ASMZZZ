using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT_Lib;

namespace JFHTMDevice
{
    public static class HtmDllManager
    {
        static HtmDllManager()
        {
            HTM.GetInitPara(out _initParam);
        }
        public static int OpendDevCount = 0;
        //public static int MotionDaqCount = 0; //已经创建（并初始化成功）的控制卡对象数量
        //public static bool IsMotionDaqOpened = false;
        public static List<int> LightTrigInited = new List<int>();//已打开的光源触发板DevIndex
        //public static int LightTrigOpend = 0; //已经打开的光源触发板数量
        public static object AsynLocker = new object();//线程同步锁


        static HTM.INIT_PARA _initParam;

        public static bool IsHtmParamInited() //初始化参数是否已经被赋值
        {
            return _IsHtmParamInited;
        }
        public static bool _IsHtmParamInited = false;

        public static void SetInitParam(HTM.INIT_PARA initParam)
        {
            _initParam = initParam;
            _IsHtmParamInited = true;
        }




        public static bool InitParamEqualExisted(HTM.INIT_PARA initParam) //initParam参数和当前存储的参数是否一致
        {
            if (!IsHtmParamInited())
                return false;
            //INIT_PARA currParamInCard;
            //if (0 != HTM.GetInitPara(out currParamInCard))//HTM控制器未打开
            //    return false;
            if (initParam.para_file != _initParam.para_file)
                return false;
            if (initParam.use_aps_card != _initParam.use_aps_card )
                return false;
            if (initParam.use_htnet_card != _initParam.use_htnet_card )
                return false;
            if (initParam.offline_mode != _initParam.offline_mode)
                return false;
            if (initParam.max_axis_num != _initParam.max_axis_num)
                return false;
            if (initParam.max_io_num != _initParam.max_io_num)
                return false;
            if (initParam.max_dev_num != _initParam.max_dev_num)
                return false;
            if (initParam.max_module_num != _initParam.max_module_num)
                return false;
            return true;
        }
    }
}
