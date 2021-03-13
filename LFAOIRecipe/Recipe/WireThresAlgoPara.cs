using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{
    public class WireThresAlgoPara : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private string name= "threshold";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }

        //检测焊点周围区域图层选择 add by wj
        private int imageIndex = 0;
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }

        public WireThresAlgoPara(int index)
        {
            this.index = index;
        }
        // add by lht
        public WireThresAlgoPara()
        {
            Index = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        private double[] threshGray= { 0, 120 };
        public double[] ThreshGray
        {
            get => threshGray;
            set => OnPropertyChanged(ref threshGray, value);
        }


        private string lightOrDark = "dark";
        /// <summary>
        /// 亮暗因数
        /// </summary>
        public string LightOrDark
        {
            get => lightOrDark;
            set => OnPropertyChanged(ref lightOrDark, value);
        }

        /// <summary>
        /// 闭操作核大小
        /// </summary>
        private double closingSize = 2.5;
        public double ClosingSize
        {
            get => closingSize;
            set => OnPropertyChanged(ref closingSize, value);
        }

        /// <summary>
        /// 半宽
        /// </summary>
        private double[] wireWidth= { 3.5, 999 };
        public double[] WireWidth
        {
            get => wireWidth;
            set => OnPropertyChanged(ref wireWidth, value);
        }

        /// <summary>
        /// 半长
        /// </summary>
        private double[] wireLength = { 13, 999 };
        public double[] WireLength
        {
            get => wireLength;
            set => OnPropertyChanged(ref wireLength, value);
        }

        /// <summary>
        /// 面积
        /// </summary>
        private double[] wireArea= { 45, 9999 };
        public double[] WireArea
        {
            get => wireArea;
            set => OnPropertyChanged(ref wireArea, value);
        }

        /// <summary>
        /// 焊点至金线法线距离阈值
        /// </summary>
        private double distTh = 0;
        public double DistTh
        {
            get => distTh;
            set => OnPropertyChanged(ref distTh, value);
        }

        // add by lht
        public static WireThresAlgoPara DeepCopyByReflection<WireThresAlgoPara>(WireThresAlgoPara obj)
        {
            if (obj is string || obj.GetType().IsValueType)
                return obj;

            //1208
            //if (obj.GetType().BaseType == typeof(Array))
            //{
            //    double[] para_double;
            //    //para_double = Array.ConvertAll(obj, new Converter<WireThresAlgoPara, double>(Double.Parse));
            //    return obj;
            //}

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    //1208
                    if (field.FieldType.Name == "Double[]" || field.FieldType.Name == "Int32[]" || field.FieldType.Name == "string[]")
                    {
                        field.SetValue(retval, DeepCopyValuesArray(field.GetValue(obj)));
                    }
                    else
                    {
                        field.SetValue(retval, field.GetValue(obj));
                    }
                    //field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj)));
                    //field.SetValue(retval, field.GetValue(obj));
                }
                catch
                {
                    MessageBox.Show("区域-阈值参数-赋值出现错误");
                    return (WireThresAlgoPara)retval;
                }
            }
            return (WireThresAlgoPara)retval;
        }

        //1208
        public static object DeepCopyValuesArray<Array>(Array obj)
        {
            double[] para_double;
            double[] para_double2;
            int[] para_int;
            int[] para_int2;
            string[] para_string;
            string[] para_string2;

            if (obj.GetType() == typeof(Double[]))
            {
                para_double = obj as double[];
                para_double2 = new Double[para_double.Length];
                for (int i = 0; i < para_double.Length; i++)
                {
                    para_double2[i] = para_double[i];
                }
                return para_double2;
            }
            if (obj.GetType() == typeof(Int32[]))
            {
                para_int = obj as int[];
                para_int2 = new Int32[para_int.Length];
                for (int i = 0; i < para_int.Length; i++)
                {
                    para_int2[i] = para_int[i];
                }
                return para_int2;
            }
            if (obj.GetType() == typeof(string[]))
            {
                para_string = obj as string[];
                para_string2 = new string[para_string.Length];
                for (int i = 0; i < para_string.Length; i++)
                {
                    para_string2[i] = para_string[i];
                }
                return para_string2;
            }
            //para_double2[0] = para_double[0];
            //para_double2[1] = para_double[1];
            return obj;
        }
    }
}
