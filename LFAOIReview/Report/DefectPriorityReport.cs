using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LFAOIReview
{
    public class DefectPriorityReport 
    {
        //定义一个字典 缺陷优先级 key为错误代码 Value为一个二维数组 0：优先级 1为最高优先级 1：错误代码 可以通过优先级直接获取错误代码
        public Dictionary<int, int[]> DefectPriority;

      

        public DefectPriorityReport()
        {
            this.DefectPriority = new Dictionary<int, int[]>();

            this.DefectPriority.Add(1, new int[2] { 15, 1 });//1 芯片脱落
            this.DefectPriority.Add(2, new int[2] { 16, 2 });//2 芯片偏移
            this.DefectPriority.Add(3, new int[2] { 17, 3 });//3 芯片转角
            this.DefectPriority.Add(4, new int[2] { 18, 4 });//4 芯片反向
            this.DefectPriority.Add(5, new int[2] { 19, 5 });//5 错误芯片
            this.DefectPriority.Add(6, new int[2] { 20, 6 });//6 墨点芯片
            this.DefectPriority.Add(7, new int[2] { 10, 7 });//7 芯片外来物
            this.DefectPriority.Add(8, new int[2] { 11, 8 });//8 芯片崩角
            this.DefectPriority.Add(9, new int[2] { 12, 9 });//9 银胶异常
            this.DefectPriority.Add(10, new int[2] { 3, 10 });//10 金球大小异常
            this.DefectPriority.Add(11, new int[2] { 4, 11 });//11 金（铜）球偏移
            this.DefectPriority.Add(12, new int[2] { 1, 12 });//12 断线
            this.DefectPriority.Add(13, new int[2] { 2, 13 });//13 弯曲
            this.DefectPriority.Add(14, new int[2] { 6, 14 });//14 第二焊点脱落
            this.DefectPriority.Add(15, new int[2] { 7, 15 });//15 第二焊点偏移
            this.DefectPriority.Add(16, new int[2] { 30, 16 });//16 ##16##
            this.DefectPriority.Add(17, new int[2] { 30, 17 });//17 钉架异物
            this.DefectPriority.Add(18, new int[2] { 9, 18 });//18 框架错误
            this.DefectPriority.Add(19, new int[2] { 30, 19 });// 19 ##19##
            this.DefectPriority.Add(20, new int[2] { 30, 20 });//20 ##20##
            this.DefectPriority.Add(21, new int[2] { 30, 21 });//21 ##21##
            this.DefectPriority.Add(22, new int[2] { 5, 22 });//22 第一焊点脱落
            this.DefectPriority.Add(23, new int[2] { 30, 23 });//23 ##23##
            this.DefectPriority.Add(24, new int[2] { 8, 24 });//24 尾丝长
            this.DefectPriority.Add(25, new int[2] { 9, 25 });//25 双丝
            this.DefectPriority.Add(26, new int[2] { 30, 26 });//26 ##26##
            this.DefectPriority.Add(27, new int[2] { 13, 27 });//27 框架变形
            this.DefectPriority.Add(28, new int[2] { 30, 28 });//28 ##28##
            this.DefectPriority.Add(29, new int[2] { 14, 29 });// 29 桥接缺陷
            this.DefectPriority.Add(30, new int[2] { 30, 30 });//30 框架异物

        }

    }
}
