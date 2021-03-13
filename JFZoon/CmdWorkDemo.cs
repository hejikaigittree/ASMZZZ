using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JFHub;

namespace JFZoon
{
    /// <summary>
    /// 用于测试 JFCmdWork 软件流程的示例类, 
    /// </summary>
    public class CmdWorkDemo : JFCmdWorkBase
    {
        public CmdWorkDemo():base()
        {
            CycleMilliseconds = 0;
        }
        public int CurrFrame{ get { return currFrame; } }//当前播放帧数
        public override int[] AllCmds { get { return new int[] { 1, 2 ,4}; } } //1倍速/2/4倍速

        public override int CurrCustomStatus { get { return currCustomStatus; } }

        public override int[] AllCustomStatus { get { return new int[] { 1,2,4}; } }

        public override string GetCmdName(int cmd)
        {
            if (1 == cmd)
                return "1X";
            else if (2 == cmd)
                return "2X";
            else if (4 == cmd)
                return "4X";
            return "无效指令:" + cmd.ToString();
        }

        public override string GetCustomStatusName(int status)
        {
            if (1 == status)
                return "状态:1倍速";
            else if (2 == status)
                return "状态:2倍速";
            else if (4 == status)
                return "状态:4倍速";
            return "未知的状态";
        }

        protected override void CleanupWhenWorkExit()
        {
            
        }

        protected override void OnPause()
        {
            return;
        }

        protected override void OnResume()
        {
            return;
        }

        protected override void OnStop()
        {
            return;
        }

        protected override void PrepareWhenWorkStart()
        {
            currFrame = 0;
        }

        

        protected override void RunLoopInWork()
        {
            currFrame++;
            int cmdRcved = 0;
            if (WaitCmd(out cmdRcved))
            {
                if (AllCustomStatus.Contains(cmdRcved))
                {
                    //currCustomStatus = cmdRcved;
                    RespCmd(JFInterfaceDef.JFWorkCmdResult.Success);
                    ChangeCustomStatus(cmdRcved, "Custom Status change to " + currCustomStatus.ToString(), null);
                    
                }
                else
                    RespCmd(JFInterfaceDef.JFWorkCmdResult.IllegalCmd);
            }
            int sleepMillsec = 1000;
            if (2 == currCustomStatus)
                sleepMillsec = 500;
            else if (4 == currCustomStatus)
                sleepMillsec = 250;

            if (sleepMillsec > CycleMilliseconds)
                CheckCmd(sleepMillsec - CycleMilliseconds);



        }

        new void ChangeCustomStatus(int customStatus, string info, object param)
        {
            if (CurrCustomStatus == customStatus)
                return;
            currCustomStatus = customStatus;
            base.ChangeCustomStatus( customStatus, info, param);
        }

        int currCustomStatus = 1;

        int currFrame = 0;
    }
}
