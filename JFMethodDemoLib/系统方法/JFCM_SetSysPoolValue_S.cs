using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;

namespace JFMethodCommonLib
{
    /// <summary>
    ///  向系统数据池中写入一个单项值 
    ///  如果数据项不存在，则返回一个错误
    ///  主要用途:将工作流数据池中的对象导出到系统数据池中
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("将工作流数据项写入系统_S")]
    [JFVersion("1.0.0.0")]
    public class JFCM_SetSysPoolValue_S : JFMethodInitParamBase,IJFMethodFlowAcq
    {
        static string PN_SysPoolItemName = "系统变量名称";
        static string PN_LocPoolItemName = "本地变量名称";
        //static string PN_WriteMode = "写入模式"; //当系统变量不存在时  创建/出错 覆盖
        //static string PN_TypeMatchMode = "类型匹配模式";
        /// Output 's name
        //static string IN_WriteVal = "待写入值"; //需要向系统数据池中写入的值
        public JFCM_SetSysPoolValue_S()
        {
            DeclearInitParam(PN_SysPoolItemName, typeof(string), "");
            DeclearInitParam(PN_LocPoolItemName, typeof(string), "");
            //DeclearInitParam(PN_WriteMode)
            //DeclearInput(IN_WriteVal, typeof(object), null);
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            //if (name == PN_SysPoolItemName)
            //    return JFParamDescribe.Create(PN_SysPoolItemName, typeof(string), JFValueLimit.Options, JFHubCenter.Instance.DataPool.AllItemKeys);
            /*else */if(name == PN_LocPoolItemName)
            {
                if(null != _owner)
                    return JFParamDescribe.Create(PN_LocPoolItemName, typeof(string), JFValueLimit.Options, _owner.DataPool.Keys.ToArray());

            }
            return base.GetInitParamDescribe(name);

        }

        /// <summary>
        /// 方法对象所在的工作流
        /// </summary>
        JFMethodFlow _owner = null;
        public void SetFlow(JFMethodFlow mf)
        {
            _owner = mf;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
           if(null == _owner)
            {
                errorInfo = "Method's owner Flow is null"; //异常
                return false;
            }
            string locItemName = GetInitParamValue(PN_LocPoolItemName) as string;
            if(!_owner.DataPool.ContainsKey(locItemName))
            {
                errorInfo = "动作流数据池中未包含数据项项名:\"" + locItemName + "\"";
                return false;
            }

            string sysItemName = GetInitParamValue(PN_SysPoolItemName) as string;

            object locVal = _owner.DataPool[locItemName];
            IJFDataPool sysDataPool = JFHubCenter.Instance.DataPool;
            if(!sysDataPool.ContainItem(sysItemName))//注册并写入新值
            {
                if(!sysDataPool.RegistItem(sysItemName,_owner.TypePool[locItemName], locVal))
                {
                    errorInfo = string.Format("向系统数据池写入/注册 失败，SysItemName = {0},Type = {1},Value = {2}", sysItemName, _owner.TypePool[locItemName].Name, locVal.ToString());
                    return false;
                }
            }
            else //覆盖写入
            {
                if(!sysDataPool.SetItemValue(sysItemName, locVal))
                {
                    errorInfo = string.Format("向系统数据池写入 失败，SysItemName = {0},本地类型 = {1}, 系统类型:{2},Value = {3}", sysItemName, _owner.TypePool[locItemName].Name,sysDataPool.GetItemType(sysItemName).Name ,locVal.ToString());
                    return false;
                }
            }


            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string locItemName = GetInitParamValue(PN_LocPoolItemName) as string;
            if(string.IsNullOrEmpty(locItemName))
            {
                errorInfo = PN_LocPoolItemName + "未设置";
                return false;
            }

            string sysItemData = GetInitParamValue(PN_LocPoolItemName) as string;
            if(string.IsNullOrEmpty(sysItemData))
            {
                errorInfo = PN_LocPoolItemName + "未设置";
                return false;
            }
            errorInfo = "Success";
            return true;

        }
    }
}
