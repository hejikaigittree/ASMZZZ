using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;
namespace JFZoon
{
    /// <summary>
    /// 一个工站示例，仅供测试
    /// </summary>
    [JFDisplayName("工站示例")]
    public class StationDemo:JFStationBase
    {
        public StationDemo()
        {
            DeclearCfgParam("测试数组参数", typeof(List<string>));
            List<string> cfgListArray = new List<string>() { "123", "456","789","0000" };
            SetCfgParamValue("测试数组参数", cfgListArray);

            DeclearCfgParam("测试数组BOOL", typeof(List<bool>));
            List<bool> cfgListB = new List<bool>();
            SetCfgParamValue("测试数组BOOL", cfgListB);


            DeclearCfgParam("测试数组SB", typeof(List<StringBuilder>));
            List<StringBuilder> cfgListSB = new List<StringBuilder>() ;
            SetCfgParamValue("测试数组SB", cfgListSB);


            DeclearCfgParam("测试double", typeof(double));
            DeclearCfgParam("测试int", typeof(int));
            DeclearCfgParam("测试bool", typeof(bool));
            DeclearCfgParam("测试string", typeof(string));





            DeclearCfgParam("测试double1", typeof(double),"AnotherTag");
            DeclearCfgParam("测试int1", typeof(int), "AnotherTag");
            DeclearCfgParam("测试bool1", typeof(bool), "AnotherTag");
            DeclearCfgParam("测试string1", typeof(string), "AnotherTag");

        }

        string _name = "";
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public override int[] AllCmds { get { return null; } }//不支持其他控制指令

        public override int CurrCustomStatus { get { return 0; } }

        public override int[] AllCustomStatus { get { return null; } }

        public override string GetCmdName(int cmd)
        {
            return null;
        }

        public override string GetCustomStatusName(int status)
        {
            return null;
        }



        protected override void CleanupWhenWorkExit()
        {
            return;
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
            return;
        }

        protected override void RunLoopInWork()
        {
            return;
        }


        /// <summary>
        /// 工站在收到结批指令后会执行此函数
        /// </summary>
        protected override void ExecuteEndBatch() //收到结批指令后的处理函数
        {
            return;
        }

        public override JFWorkCmdResult Reset()
        {
            return JFWorkCmdResult.Success;
        }
    }
}
