using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFHub
{
    /// <summary>
    /// 运行多个工作流（模板）的线程池类
    /// </summary>
    public class JFWorkFlowThreadPool
    {
        class JFWorkFlowThread : JFCmdWorkBase
        {
            internal JFWorkFlowThread(JFMethodFlow methodFlow,JFStationBase stationOwner)
            {
                _methodFlow = new JFMethodFlow();
                _methodFlow.FromTxt(methodFlow.ToTxt());
                _methodFlow.SetStation(stationOwner);
            }
            JFMethodFlow _methodFlow = null;


            public override int[] AllCmds => throw new NotImplementedException();

            public override int[] AllCustomStatus => throw new NotImplementedException();

            public override string GetCmdName(int cmd)
            {
                throw new NotImplementedException();
            }

            public override string GetCustomStatusName(int status)
            {
                throw new NotImplementedException();
            }

            protected override void CleanupWhenWorkExit()
            {
                throw new NotImplementedException();
            }

            protected override void OnPause()
            {
                throw new NotImplementedException();
            }

            protected override void OnResume()
            {
                throw new NotImplementedException();
            }

            protected override void OnStop()
            {
                
            }

            protected override void PrepareWhenWorkStart()
            {
                throw new NotImplementedException();
            }

            protected override void RunLoopInWork()
            {
                throw new NotImplementedException();
            }
        }



        public JFWorkFlowThreadPool(JFMethodFlow mf,int threadCount,JFStationBase stationOwner)
        {

        }
    }
}
