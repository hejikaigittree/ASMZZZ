﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFHub
{
    /// <summary>
    /// IJFMethodFlowAcq : 用于和MethodFlow进行交互的类标识
    /// 使用场景:
    /// </summary>
    public interface IJFMethodFlowAcq
    {
        void SetFlow(JFMethodFlow mf);
    }
}
