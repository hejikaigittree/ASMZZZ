using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFHub;
using DLAF;
using JFInterfaceDef;


namespace JFZoon
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.ApplicationExit += CleanUpPreExit;//当子模块调用Application.Exit时不会触发 ApplicationExit 事件
            //StationDemo stationDemo = new StationDemo();
            //stationDemo.Name = "hehe";
            //JFHubCenter.Instance.StationMgr.DeclearStation(stationDemo);

            //StationDemo stationDemo1 = new StationDemo();
            //stationDemo1.Name = "haha";
            //IJFStation st = stationDemo1;
            //JFHubCenter.Instance.StationMgr.DeclearStation(st);

            //CalibStation cs = new CalibStation();
            //cs.Name = "CalibStation";
            //JFHubCenter.Instance.StationMgr.DeclearStation(cs);


            Application.EnableVisualStyles();
           
            Form1 mainForm = new Form1();
            mainForm.Clearup += CleanupPreExit;
            Application.Run(mainForm);
        }


        static void CleanupPreExit() 
        {
            //MessageBox.Show("Cleanup before Exit"); //已测试
        }
    }
}
