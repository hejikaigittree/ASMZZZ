using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>带触发功能的光源控制器的工作模式 </summary>
    public enum JFLightWithTrigWorkMode
    {
        TurnOnOff = 0, //数字控制的开关模式
        Trigger        //外部触发式控制
    }
    /// <summary>
    /// 带有触发功能的数字式光源控制器
    /// </summary>
    public interface IJFDevice_LightControllerWithTrig:IJFDevice_LightController,IJFDevice_TrigController
    {
        int SetWorkMode(JFLightWithTrigWorkMode mode);
        int GetWorkMode(out JFLightWithTrigWorkMode workMode);
    }
}
