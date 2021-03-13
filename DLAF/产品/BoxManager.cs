using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Formatters.Binary;
using JFToolKits;
namespace DLAF
{
    /// <summary>
    /// 料盒类
    /// </summary>
    [Serializable]
    public class Box
    {
        public string _name;
        public int _idx;
        public double _heightLastSlot;
        public double _heightLastSlot_y;
        public double _heightFirstSlot;
        public double _heightFirstSlot_y;
        public double _heightLastSlot_Unload;
        public double _heightLastSlot_Unload_y;
        public double _heightFirstSlot_Unload;
        public double _heightFirstSlot_Unload_y;
        public int _slotNumber;
        public int _blankNumber_Unload;
        public double _frameWidth;
        public double _y_LoadFramePos;
        public double _z_LoadFramePos;
        public double _y_UnloadFramePos;
        public double _z_UnloadFramePos;
        public double _x_LoadPushRodWaitPos;
        public double _x_LoadPushRodOverPos;
        public double _x_ChuckLoadFramePos;

        [CategoryAttribute("料盒属性"), DisplayNameAttribute("①料盒名")]
        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        [CategoryAttribute("料盒属性"),ReadOnly(true), DisplayNameAttribute("②料盒索引")]
        public Int32 Idx
        {
            get { return _idx; }
            set
            {
                _idx = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("③上料最后槽上料位Z坐标（mm）")]
        public Double HeightLastSlot
        {
            get{return _heightLastSlot;}
            set {
                _heightLastSlot = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("③上料最后槽上料位Y坐标（mm）")]
        public Double HeightLastSlot_y
        {
            get
            {
                return _heightLastSlot_y;
            }
            set
            {
                _heightLastSlot_y = value;
            }
        }

        [CategoryAttribute("料盒属性"), DisplayNameAttribute("④上料第一槽上料位Z坐标（mm）")]
        public Double HeightFirstSlot
        {
            get { return _heightFirstSlot; }
            set
            {
                _heightFirstSlot = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("④上料第一槽上料位Y坐标（mm）")]
        public Double HeightFirstSlot_y
        {
            get { return _heightFirstSlot_y; }
            set
            {
                _heightFirstSlot_y = value;
                _y_LoadFramePos = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑤下料最后槽上料位Z坐标（mm）")]
        public Double HeightLastSlot_Unload
        {
            get { return _heightLastSlot_Unload; }
            set
            {
                _heightLastSlot_Unload = value;
            }
        }

        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑤下料最后槽上料位Y坐标（mm）")]
        public Double HeightLastSlot_Unload_y
        {
            get { return _heightLastSlot_Unload_y; }
            set
            {
                _heightLastSlot_Unload_y = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑥下料第一槽上料位Z坐标（mm）")]
        public Double HeightFirstSlot_Unload
        {
            get { return _heightFirstSlot_Unload; }
            set
            {
                _heightFirstSlot_Unload = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑥下料第一槽上料位Y坐标（mm）")]
        public Double HeightFirstSlot_Unload_y
        {
            get { return _heightFirstSlot_Unload_y; }
            set
            {
                _heightFirstSlot_Unload_y = value;
                y_UnloadFramePos = value;
            }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑦料盒内槽数目")]
        public int SlotNumber
        {
            get { return _slotNumber; }
            set { _slotNumber = value; }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑧顺序下料间隔槽数目"), DescriptionAttribute("顺序下料间隔槽数目，同层时无效")]
        public int BlankNumber_Unload
        {
            get { return _blankNumber_Unload; }
            set { _blankNumber_Unload = value; }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑨导轨Y轴位")]
        public double FrameWidth
        {
            get { return _frameWidth; }
            set { _frameWidth = value; }
        }
        [CategoryAttribute("料盒属性"), ReadOnly(true), DisplayNameAttribute("⑩上料Y轴上片位(mm)"), DescriptionAttribute("下料料盒搬运机构Y轴,Y方向")]
        public double y_LoadFramePos
        {
            get { return _y_LoadFramePos; }
            set { _y_LoadFramePos = value; }
        }
        [CategoryAttribute("料盒属性"), ReadOnly(true), DisplayNameAttribute("⑫下料Y轴下片位(mm)"), DescriptionAttribute("下料料盒搬运机构Y轴,Y方向")]
        public double y_UnloadFramePos
        {
            get { return _y_UnloadFramePos; }
            set { _y_UnloadFramePos = value; }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑭上料推杆等待位(mm)"), DescriptionAttribute("上料推杆位置,X方向")]
        public double x_LoadPushRodWaitPos
        {
            get { return _x_LoadPushRodWaitPos; }
            set { _x_LoadPushRodWaitPos = value; }
        }
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("⑮上料推杆结束位(mm)"), DescriptionAttribute("上料推杆位置,X方向")]
        public double x_LoadPushRodOverPos
        {
            get { return _x_LoadPushRodOverPos; }
            set { _x_LoadPushRodOverPos = value; }
        }

        [CategoryAttribute("流程属性"), DisplayNameAttribute("①产品检测位(mm)"), DescriptionAttribute("载台左侧上料夹爪的位置, X方向")]
        public Double x_ChuckLoadFramePos 
        { 
            get { return _x_ChuckLoadFramePos; } 
            set { _x_ChuckLoadFramePos = value; }
        }

        public Box()
        {
            Name = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">料盒名</param>
        /// <param name="idx">料盒索引</param>
        public Box(string name,int idx)
        {
            Name = name;
            Idx = idx;
        }
        public override string ToString()
        {
            return Name;
        }
        public Box Clone()

        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as Box;
        }

    }
    /// <summary>
    /// 多料盒管理类
    /// </summary>
    public class BoxManager: StringConverter
    {
        public static int Num;
        public static ConcurrentDictionary<int,Box> Dir_Boxes = new ConcurrentDictionary<int, Box>();
        
        public BoxManager()
        {
            //Dir_Boxes = new ConcurrentDictionary<int, Box>();
        }

        private static readonly Lazy<BoxManager> lazy = new Lazy<BoxManager>(() => new BoxManager());
        public static BoxManager Instance { get { return lazy.Value; } }

        public bool Save()
        {
            string path = @"BoxData.ini";
          
            JFXCfg cfg = new JFXCfg();                     
            int i = 0;
            foreach (var pair in Dir_Boxes)
            {
                Box item = pair.Value;               
                cfg.SetItemValue("Box_" + i,item);
                i++;
            }
            Num = Dir_Boxes.Count;
            cfg.SetItemValue("Count",Num);

            cfg.Save(path);
            return true;
        }
        public bool Load()
        {
            JFXCfg cfg = new JFXCfg();
            string path = @"BoxData.ini";
         
            if (File.Exists(@"BoxData.ini"))
            {
                cfg.Load(path,true);             
                Num = (int)cfg.GetItemValue("Count");
                //Dir_Boxes = new ConcurrentDictionary<int, Box>();
                Box BoxItem = null;
                for (int i = 0; i < Num; i++)
                {
                    BoxItem = new Box();                   
                    BoxItem = cfg.GetItemValue("Box_" + i) as Box;
                    if (BoxItem.Name == null) BoxItem.Name = "Box_" + i;
                    if (BoxItem.Name == "") BoxItem.Name = "Box_" + i;
                    Dir_Boxes.TryAdd(BoxItem.Idx, BoxItem);
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 重写类的默认值改写支持属性
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// 重写默认的值获取属性
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] ArrayStr = new string[Num];
            int i = 0;
            foreach(var pair in Dir_Boxes)
            {
                ArrayStr[i] = pair.Value.Name;
                i++;
            }
            return new StandardValuesCollection(ArrayStr);
        }
        /// <summary>
        /// 重写类的默认值改写描述属性
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
