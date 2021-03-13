using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 系统", "工站配置操作" })]
    [JFDisplayName("获取工站配置操作")]
    [JFVersion("1.0.0.0")]
    class JFCM_GetStationCateCfgToPool : JFMethodInitParamBase, IJFStationBaseAcq,IJFMethodFlowAcq
    {
        JFStationBase _station = null;
        JFMethodFlow _mf = null;
        static string CateName = "配置类别名称";
        public JFCM_GetStationCateCfgToPool()
        {
            DeclearInitParam(CateName, typeof(string), null);
           
        }
        public void SetFlow(JFMethodFlow mf)
        {
            _mf = mf;
        }

        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = _station.Name+"所属工站不存在！";
                return false;
            }
            if (_mf == null)
            {
                errorInfo = _station.Name + "动作流不存在！";
                return false;
            }
            string category =  GetInitParamValue(CateName) as string;
            List<string> cfgName = _station.Config.ItemNamesInTag(category).ToList();

            if (cfgName == null)
            {
                errorInfo = _station.Name + "配置项不存在！";
                return false;
            }
            try
            {
                foreach (string name in cfgName)
                {
                    if (_mf.DataPool.ContainsKey(name))
                        _mf.DataPool.Remove(name);
                    if (_mf.TypePool.ContainsKey(name))
                        _mf.TypePool.Remove(name);
                    object ob = _station.GetCfgParamValue(name);
                    if (ob != null)
                    {
                        _mf.DataPool.Add(name, ob);
                        _mf.TypePool.Add(name, ob.GetType());
                       // SetOutputParamValue(name,ob);
                    }
                }
            }
            catch (Exception ex)
            {
                errorInfo = _station.Name + ex.Message;
                return false;
            }
            errorInfo = "Success";
            return true;

        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
           string str = GetInitParamValue(CateName) as string;
            if(str == null)
            {
                errorInfo = "为绑定配置项目录名称";
                return false;
            }

            errorInfo = "Success";
            return true;
        }
    }
}
