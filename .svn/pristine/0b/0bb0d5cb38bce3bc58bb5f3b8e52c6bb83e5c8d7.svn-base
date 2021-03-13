using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;

namespace JFHub
{

    /// <summary>
    /// 与视觉相关的对象（相机示教助手/标定住手等）管理类
    /// </summary>
    public class JFVisionManager
    {
        public JFVisionManager(string filePath)
        {
            _cfg.Load(filePath, true);
            if (!_cfg.ContainsItem("SVCfgs"))
            {
                _lstSVCfgs = new List<JFSingleVisionCfgParam>();
                _cfg.AddItem("SVCfgs", _lstSVCfgs, "单视野视觉配置参数");
            }
            else
                _lstSVCfgs = _cfg.GetItemValue("SVCfgs") as List<JFSingleVisionCfgParam>;
        }

        
        JFXCfg _cfg = new JFXCfg();
        List<JFSingleVisionCfgParam> _lstSVCfgs ; //Single Vision Configs

        /// <summary>
        /// 所有单相机视觉示教助手名称
        /// </summary>
        /// <returns></returns>
        public string [] AllSVAssistNames()
        {
            return JFHubCenter.Instance.InitorManager.GetIDs(typeof(JFSingleVisionAssist));
        }

        /// <summary>
        /// 获取一个单相机视觉示教助手
        /// </summary>
        /// <param name="svaName"></param>
        /// <returns></returns>
        public JFSingleVisionAssist GetSVAssistByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            string[] allAssistNames = AllSVAssistNames();
            if (null == allAssistNames)
                return null;
            if (!allAssistNames.ToList().Contains(name))
                return null;
            return JFHubCenter.Instance.InitorManager.GetInitor(name) as JFSingleVisionAssist;
        }


        /// <summary>
        /// 系统中是否包含名称为name的视觉助手
        /// </summary>
        /// <param name="svaName"></param>
        /// <returns></returns>
        public bool ContainSVAssistName(string name)
        {
            string[] allAssistNames = AllSVAssistNames();
            if (null == allAssistNames)
                return false;
            return allAssistNames.ToList().Contains(name);
        }

        /// <summary>
        /// 向系统配置中添加一个单相机视觉示教助手
        /// </summary>
        /// <param name="name">名称必须是全局唯一的InitorName，否则会报一个参数异常</param>
        /// <param name="sva"></param>
        public void AddSVAssist(string name, JFSingleVisionAssist sva)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("JFVisionManager.AddSVAssist(string name, ... ) failed by name is null or empty");
            if(null == sva)
                throw new ArgumentNullException("JFVisionManager.AddSVAssist(string name, JFSingleVisionAssist sva) failed  by sva == null");
            if (ContainSVAssistName(name))
                throw new Exception("JFVisionManager.AddSVAssist(name ...) failed by name = " + name + " 's initor_object is Existed!");
            JFHubCenter.Instance.InitorManager.Add(name, sva);

            //////保存到配置文件
            //JFXmlSortedDictionary<string, List<object>> dictInitorParam = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            //List<object> paramsInCfg = new List<object>();
            //paramsInCfg.Add(sva.GetType().AssemblyQualifiedName);
            //for (int i = 0; i < sva.InitParamNames.Length; i++)
            //    paramsInCfg.Add(sva.GetInitParamValue(sva.InitParamNames[i]));
            //dictInitorParam.Add(name, paramsInCfg);
            ////JFHubCenter.Instance.SystemCfg.NotifyItemChanged(JFHubCenter.CK_InitDevParams);
            //JFHubCenter.Instance.SystemCfg.Save();
        }

        /// <summary>
        /// 从系统中删除一个单相机视觉示教助手
        /// </summary>
        /// <param name="name"></param>
        public void DelSVAssist(string name)
        {
            if (!ContainSVAssistName(name))
                return;
            IJFInitializable initor = JFHubCenter.Instance.InitorManager.GetInitor(name);
            if (typeof(JFSingleVisionAssist) != initor.GetType())
                throw new Exception("DelSVAssist(string name = " + name + ") failed by the initor's type is not JFSingleVisionAssist");
            JFHubCenter.Instance.InitorManager.Remove(name); //从设备管理器中删除
            JFXmlSortedDictionary<string, List<object>> devCfg = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            devCfg.Remove(name);//从设备配置文件中删除
            JFHubCenter.Instance.SystemCfg.NotifyItemChanged(JFHubCenter.CK_InitDevParams);
            JFHubCenter.Instance.SystemCfg.Save();
        }

        /// <summary>
        /// 获取所有单视野视觉配置名称
        /// </summary>
        /// <returns></returns>
        public string[] AllSingleVisionCfgNames()
        {
            List<string> ret = new List<string>();
            foreach (JFSingleVisionCfgParam cfg in _lstSVCfgs)
                ret.Add(cfg.Name);
            return ret.ToArray();
        }

        public bool ContainSingleVisionCfgByName(string name)
        {
            foreach (JFSingleVisionCfgParam cfg in _lstSVCfgs)
                if (cfg.Name == name)
                    return true;
            return false;
        }

        /// <summary>
        /// 获取一个单视野视觉配置参数项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFSingleVisionCfgParam GetSingleVisionCfgByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("JFVisionManager.GetSingleVisionCfg(name) failed by name is null or empty");
            foreach (JFSingleVisionCfgParam cfg in _lstSVCfgs)
            {
                if (cfg.Name == name)
                    return cfg;
            }
            return null;
        }

        /// <summary>
        /// 添加一个单视野视觉配置参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cfg"></param>
        public void AddSingleVisionCfg(string name,JFSingleVisionCfgParam cfg )
        {
            if (null == cfg)
                throw new ArgumentNullException("JFVisionManager.AddSingleVisionCfgParam(cfg) failed by cfg == null");

            if (_lstSVCfgs.Contains(cfg))
                throw new ArgumentException("JFVisionManager.AddSingleVisionCfgParam(cfg) failed by cfg had Contained-in!");
            
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("JFVisionManager.AddSingleVisionCfgParam(cfg) failed by cfg.Name is null or empty");
                      
            if(ContainSingleVisionCfgByName(name))
                throw new ArgumentException("JFVisionManager.AddSingleVisionCfgParam(name ...) failed by name = " + name + " is Existed!");

            cfg.Name = name;
            _lstSVCfgs.Add(cfg);
            _cfg.Save();
        }

        public void Save()
        {
            _cfg.Save();
        }

        /// <summary>
        /// 删除一个单视野配置参数
        /// </summary>
        /// <param name="name"></param>
        public void DelSingleVisionCfg(string name)
        {
            if (!ContainSingleVisionCfgByName(name))
                return;
            foreach(JFSingleVisionCfgParam cfg in _lstSVCfgs)
            {
                if (cfg.Name == name)
                {
                    _lstSVCfgs.Remove(cfg);
                    _cfg.Save();
                    break;
                }
            }
            return;
        }

        /// <summary>
        /// 获取由指定的相机助手创建的所有配置项
        /// </summary>
        /// <param name="assistName"></param>
        /// <returns></returns>
        public string[] SingleVisionCfgNameByOwner(string assistName)
        {
            List<string> ret = new List<string>();
            foreach (JFSingleVisionCfgParam cfg in _lstSVCfgs)
                if (cfg.OwnerAssist == assistName)
                    ret.Add(cfg.Name);
            return ret.ToArray();
        }

        public string GetSVAName(JFSingleVisionAssist assist)
        {
            if (null == assist)
                return null;
            return JFHubCenter.Instance.InitorManager.GetIDByInitor(assist);
        }









    }
}
