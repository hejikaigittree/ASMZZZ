using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{
    public class WireLineGauseAlgoParaAll : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private string name = "line_guass_pro";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }
        private int imageIndex = 0;
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }
        public WireLineGauseAlgoParaAll(int index)
        {
            this.index = index;
        }
        // add by lht
        public WireLineGauseAlgoParaAll()
        {
            Index = 0;
        }
        private double wireWidth=3.0;
        public double WireWidth
        {
            get => wireWidth;
            set => OnPropertyChanged(ref wireWidth, value);
        }

        private double wireContrast = 15.0;
        public double WireContrast
        {
            get => wireContrast;
            set => OnPropertyChanged(ref wireContrast, value);
        }

        private string lightOrDark = "dark";
        /// <summary>
        /// 亮暗因数 dark light all 
        /// </summary>
        public string LightOrDark
        {
            get => lightOrDark;
            set => OnPropertyChanged(ref lightOrDark, value);
        }

        /// <summary>
        /// 筛选目标时的特征量
        /// </summary>
        private string[] selMetric = { "contour_length", "direction"};
        public string[] SelMetric
        {
            get => selMetric;
            set => OnPropertyChanged(ref selMetric, value);
        }

        /// <summary>
        /// [3,-rad(25)]  3, -25/180*Math.PI
        /// </summary>
        private double[] selMin = {3, -0.436 };
        public double[] SelMin
        {
            get => selMin;
            set => OnPropertyChanged(ref selMin, value);
        }

        // 25/180 * Math.PI
        private double[] selMax = { 999, 0.436 };
        public double[] SelMax
        {
            get => selMax;
            set => OnPropertyChanged(ref selMax, value);
        }

        //25 / 180 * Math.PI
        private double linePhiDiff =0.436;
        public double LinePhiDiff
        {
            get => linePhiDiff;
            set => OnPropertyChanged(ref linePhiDiff, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private double maxWireGap = 5;
        public double MaxWireGap
        {
            get => maxWireGap;
            set => OnPropertyChanged(ref maxWireGap, value);
        }

        /// <summary>
        /// 是否开启双线预处理 "是" "否"
        /// </summary>
        private bool isDoubleLines = false;
        public bool IsDoubleLines
        {
            get => isDoubleLines;
            set => OnPropertyChanged(ref isDoubleLines, value);
        }

        /// <summary>
        /// 亮暗因数 light_dark_light dark_light_dark
        /// </summary>
        private string doubleLinesType = "light_dark_light";
        public string DoubleLinesType
        {
            get => doubleLinesType;
            set => OnPropertyChanged(ref doubleLinesType, value);
        }

        /// <summary>
        /// 焊点至金线法线距离阈值
        /// </summary>
        private double middleLineWidth = 3;
        public double MiddleLineWidth
        {
            get => middleLineWidth;
            set => OnPropertyChanged(ref middleLineWidth, value);
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
        public static WireLineGauseAlgoParaAll DeepCopyByReflection<WireLineGauseAlgoParaAll>(WireLineGauseAlgoParaAll obj)
        {
            if (obj is string || obj.GetType().IsValueType)
                return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    //1208
                    if (field.FieldType.Name == "Double[]" || field.FieldType.Name == "Int32[]" || field.FieldType.Name == "String[]")
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
                    MessageBox.Show("区域-金线高性能参数-赋值出现错误");
                    return (WireLineGauseAlgoParaAll)retval;
                }
            }

            return (WireLineGauseAlgoParaAll)retval;
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
