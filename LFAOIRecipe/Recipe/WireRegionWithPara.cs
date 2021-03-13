using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;
//using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class WireRegionWithPara : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        //public ObservableCollection<IParameter> RegionWithParameters { get; set; } = new ObservableCollection<IParameter>();


        public WireThresAlgoPara WireThresAlgoPara { get; set; }

        public WireLineGauseAlgoPara WireLineGauseAlgoPara { get; set; }

        public WireLineGauseAlgoParaAll WireLineGauseAlgoParaAll { get; set; }

        public WireRegionWithPara()
        {
            WireThresAlgoPara = new WireThresAlgoPara(this.index);
            WireLineGauseAlgoPara = new WireLineGauseAlgoPara(this.index);
            WireLineGauseAlgoParaAll = new WireLineGauseAlgoParaAll(this.index);
        }

        //功能拓展
        public WireRegionWithPara(int index)
        {
            WireThresAlgoPara   = new WireThresAlgoPara(this.index);
            WireLineGauseAlgoPara  = new WireLineGauseAlgoPara(this.index);
            WireLineGauseAlgoParaAll = new WireLineGauseAlgoParaAll(this.index);
            this.index = index;

            //RegionWithParameters.Add(WireThresAlgoPara);
            //RegionWithParameters.Add(WireLineGauseAlgoPara);

        }

        public WireRegionWithPara DeepClone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as WireRegionWithPara;

        }


        public static WireRegionWithPara DeepCopyByReflection<WireRegionWithPara>(WireRegionWithPara obj)
        {
            if (obj is string || obj.GetType().IsValueType)
                return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    //1208 在此实现三个区域参数子类的深拷贝，不用在外部单独实现
                    if (field.FieldType.FullName == "System.Int32")
                    {
                        //field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj)));
                        field.SetValue(retval, field.GetValue(obj));
                    }

                    if (field.FieldType.FullName == "LFAOIRecipe.WireThresAlgoPara")
                    {
                        int a;
                        a = 1;
                        field.SetValue(retval, WireThresAlgoPara.DeepCopyByReflection(field.GetValue(obj)));
                        //WireThresAlgoPara tmp = WireThresAlgoPara.DeepCopyByReflection(field.GetValue(obj) as WireThresAlgoPara);
                        //field.SetValue(retval, tmp);

                    }
                    if (field.FieldType.FullName == "LFAOIRecipe.WireLineGauseAlgoPara")
                    {
                        int b;
                        b = 1;
                        field.SetValue(retval, WireLineGauseAlgoPara.DeepCopyByReflection(field.GetValue(obj)));
                    }
                    if (field.FieldType.FullName == "LFAOIRecipe.WireLineGauseAlgoParaAll")
                    {
                        int c;
                        c = 1;
                        field.SetValue(retval, WireLineGauseAlgoParaAll.DeepCopyByReflection(field.GetValue(obj)));
                    }

                    //field.SetValue(retval, field.GetValue(obj));  //1208 不分类(分别调用三个DeepCopyByReflection)，还是会产生引用

                }
                catch
                {
                    int a;
                    a = 1;
                }
            }

            return (WireRegionWithPara)retval;
        }
    }
}
