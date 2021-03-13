using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFHub
{

    [Serializable]
    public class JFAxisPos
    {
        JFAxisPos()
        {
            AxisName = "";
            Position = 0;
        }

        public static JFAxisPos Create(string axisName,double pos)
        {
            JFAxisPos ret = new JFAxisPos();
            ret.AxisName = axisName;
            ret.Position = pos;
            return ret;
        }

        public string AxisName { get; set; }
        public double Position { get; set; }
    }
    /// <summary>
    /// 多轴点位，用于标识多个轴位置所表示的一个点
    /// </summary>
    public class JFMultiAxisPosition
    {

        public JFMultiAxisPosition()
        {
            Name = "";
            Positions = new List<JFAxisPos>();
        }
        /// <summary>
        /// 点位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 点位中各轴位置
        /// </summary>
        public List<JFAxisPos> Positions;

        public string[] AxisNames
        { 
            get 
            {
                List<string> ret = new List<string>();
                foreach (JFAxisPos ap in Positions)
                    ret.Add(ap.AxisName);
                return ret.ToArray();
            }
        }

        /// <summary>
        /// 是否包含轴
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public bool ContainAxis(string axisName)
        {
            foreach (JFAxisPos pos in Positions)
                if (pos.AxisName == axisName)
                    return true;
            return false;
        }

        public void RemoveAxis(string axisName)
        {
            for(int i = 0; i < Positions.Count; i++)
            {
                if(Positions[i].AxisName == axisName)
                {
                    Positions.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 设置轴坐标，如果坐标轴不存在，则会添加
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="pos"></param>
        public void SetAxisPos(string axisName,double pos)
        {
            for (int i = 0; i < Positions.Count; i++)
                if (Positions[i].AxisName == axisName)
                {
                    Positions[i].Position = pos;
                    return;
                }
            Positions.Add(JFAxisPos.Create(axisName, pos));
        }

        public double GetAxisPos(string axisName)
        {
            foreach(JFAxisPos ap in Positions)
                if(ap.AxisName == axisName)
                    return ap.Position;
            throw new ArgumentException(string.Format("JFMultiAxisPosition.GetAxisPos(axisName = {0}) failed by: axisName is not included by AxisNames = {1},PositionName = {2}", axisName, string.Join("|", AxisNames), Name));
        }

        /// <summary>
        /// 点位当前轴号在参数axisNames中不存在的部分（非法）
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public string[] NotAllowedBy(string[] axisNames)
        {
            if (null == axisNames || 0 == axisNames.Length)
                return AxisNames;
            return  axisNames.Except(AxisNames).ToArray();//axisNames存在，而当前轴名称中没有的  
        }

    }
}
