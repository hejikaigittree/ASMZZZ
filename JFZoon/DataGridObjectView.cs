using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFZoon
{
    public class DataGridObjectView : DataGridView
    {
        public DataGridObjectView() : base()
        {
            this.CellFormatting += OnCellFormatting;
        }

        void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((Rows[e.RowIndex].DataBoundItem != null) && (Columns[e.ColumnIndex].DataPropertyName.Contains(".")))
            {
                //对具有点语法的字段进行分割
                //比如JobModel.SkillModel.SkillName
                //分割成JobModel,SkillModel和SkillName
                string[] nameAndProp = Columns[e.ColumnIndex].DataPropertyName.Split(new char[] { '.' });

                object pObj = Rows[e.RowIndex].DataBoundItem;
                //i<nameAndProp.Length-1是因为，只需要循环属性名长度-1次就可以了。
                //比如，对于JobModel.JobName，在上一步中已经获取了JobModel实体
                //那么在for循环中的代码只需要执行一遍，即i<nameAndProp.Length-1次，即可获取JobName的属性
                for (int i = 0; i < nameAndProp.Length - 1; i++)
                {
                    pObj = GetObject(pObj, nameAndProp[i]);
                    //以JobModel.JobName为例，它只在i=0的时候进来执行一次并获取属性值
                    //那么这里就只能为nameAndProp.Length - 2才能顺利获取到属性值
                    if (i == nameAndProp.Length - 2)
                    {
                        //下面代码中的i+1可以保证它获取的是最后的属性值
                        //即：JobModel.JobName的时候取的是JobName的值
                        //或者JobModel.SkillModel.SkillName的时候取得是SkillName的值
                        PropertyInfo objectProperty = pObj.GetType().GetProperty(nameAndProp[i + 1]);
                        e.Value = objectProperty.GetValue(pObj, null).ToString();//取出字段值
                    }
                }
            }
        }

        private object GetObject(object pObj, string nameAndProp)
        {
            if (pObj == null)
            {
                return null;
            }
            PropertyInfo objProp = pObj.GetType().GetProperty(nameAndProp);
            return objProp.GetValue(pObj, null);
        }
    }
}
