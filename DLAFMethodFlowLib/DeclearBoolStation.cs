using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;

namespace DLAFMethodLib
{
    [JFDisplayName("JF声明系统变量工站")]
    [JFVersion("1.0.0.0")]
    class DeclearBoolStation:JFRuleStation
    {
        public DeclearBoolStation()
        {
            DeclearSPItemAlias("下料准备收板完成", typeof(bool), false);
        }
    }
}
