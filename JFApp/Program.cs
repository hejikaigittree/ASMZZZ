using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFHub;
using DLAF;
using DLAF_DS;

namespace JFApp
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
            ///注册主工站, 每个App必须注册一个主工站
            JFHubCenter.Instance.StationMgr.DeclearMainStation(new MainStation()); //这里的JFMainStationBase 只是一个示例
            
            ///注册检测工站
            //DLAFDetectStation detectStation = new DLAFDetectStation();
            //detectStation.Name = "检测工站";
            //JFHubCenter.Instance.StationMgr.DeclearStation(detectStation);




            Application.ApplicationExit += CleanUpPreExit;
            Application.EnableVisualStyles();
            
            FormMain mainForm = new FormMain(); 

            Application.Run(mainForm);
        }

        static void CleanUpPreExit(object sender, EventArgs e)
        {
            //日后添加程序退出前的处理
            JFHubCenter.Instance.StationMgr.Stop();
        }
    }
}
