using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;


namespace LFAOIReview
{

   

    public class DefectStatistics: DefectPriorityReport
    {
        /// <summary>
        /// 统计错误个数
        /// </summary>
        public Dictionary<int, int> CountDefectResult;

        public Dictionary<int, int> CountN2KDefectResult ;

        public List<string> DefectResultType;

        /// <summary>
        /// 存在未定义优先级的错误
        /// </summary>
        public bool Flag = true;

        /// <summary>
        /// 如果存在重复优先级，照样统计数据，但是报存重复错误码
        /// </summary>
        public List<HTuple> List_RepeatPriority;

        public DefectStatistics()
        {
            this.CountDefectResult = new Dictionary<int, int>();
            
            this.List_RepeatPriority = new List<HTuple>();

            this.DefectResultType = new List<string>();

            this.CountN2KDefectResult = new Dictionary<int, int>();
        }
    }
}
