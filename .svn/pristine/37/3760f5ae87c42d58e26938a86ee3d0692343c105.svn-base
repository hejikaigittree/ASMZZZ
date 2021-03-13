using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 
    /// </summary>
    public enum JFValueLimit
    {
        NonLimit = 0,
        MinLimit = 1,
        MaxLimit = 2,
        Options = 4,
        FilePath = 8,
        FolderPath = 16,
    }



    /// <summary>
    /// 参数描述信息,（比如：用于描述 IJFInitializable接口的初始化参数，IJFMethod的输入参数）
    /// 待改进功能 ：当选择文件时，range应该为文件格式描述（后缀名）
    /// </summary>
    public class JFParamDescribe
    {
        /// <summary>
        /// 创建一个对象
        /// </summary>
        /// <param name="type">参数类型,不能为空值</param>
        /// <param name="limit">参数限制</param>
        /// <param name="range">参数范围，如果参数限制值为JFValueLimit.MinLimit/MaxLimit/Options，此值不能为空</param>
        /// <param name="maxLength">如果参数为数组类型，maxLength为最大长度限制， <0时表示无长度限制</param>
        /// <param name="summary">参数简介文本，可为空值</param>
        /// <returns></returns>
        public static JFParamDescribe Create(string name,Type type, JFValueLimit limit, object[] range, string summary =null,int maxCollectionLength = -1)
        {
            if (null == type)
                throw new ArgumentNullException("JFParamDescribe.Create(Type type....) failed By:type = null");
            if((limit & JFValueLimit.MinLimit) != 0 && (limit & JFValueLimit.MaxLimit) != 0)//同时有最大/最小值限制
            {
                if (null == range || range.Length != 2)
                    throw new ArgumentException(string.Format("JFParamDescribe.Create(type = {0},limit = {1}, object[] range ...) failed By:{2}",
                                                        type.Name, limit.ToString(),
                                                        null == range ? "range == null" : ("range's count = " + range.Length + "!Must be 2 ")));
            }
            if((limit & JFValueLimit.MinLimit) != 0 || (limit & JFValueLimit.MaxLimit) != 0)// || (limit & JFValueLimit.Options) != 0 )
                if(null == range || range.Length == 0)
                    throw new ArgumentException(string.Format("JFParamDescribe.Create(type = {0},limit = {1}, object[] range ...) failed By:range is null or empty!",
                                                       type.Name, limit.ToString()));
            if ((limit & JFValueLimit.Options) != 0)
                if (null == range)
                    range = new object[] { };
            return new JFParamDescribe(name,type, limit, range, maxCollectionLength, summary);
        }
        internal JFParamDescribe(string name,Type type, JFValueLimit limit, object[] range, int maxLength,string summary)
        {
            DisplayName = name;
            ParamType = type;
            ParamLimit = limit;

            if (null != range && range.Length > 0)
            {
                ParamRange = new object[range.Length];
                Array.Copy(range, ParamRange, range.Length);
            }
            else
                ParamRange = null;

            CollectionMaxLength = maxLength;
            ParamSummary = summary;
        }
        public string DisplayName { get; private set; }
        public Type ParamType { get; private set; }
        public JFValueLimit ParamLimit { get; private set; }
        public object[] ParamRange { get; private set; }
        public int CollectionMaxLength { get; private set; }
        public string ParamSummary { get; private set; }
        
    }
}
