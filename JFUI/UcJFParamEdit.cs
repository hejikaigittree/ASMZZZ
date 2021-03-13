using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using JFToolKits;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Http.Headers;
using System.CodeDom;
using System.Threading;
using System.Reflection;

namespace JFUI
{
    /// <summary>
    /// 目前不能同时支持数组元素 + 可选项值，日后必须修改
    /// </summary>
    public partial class UcJFParamEdit : UserControl
    {
        private JFPropertyCollection proCollect = new JFPropertyCollection();
        private JFProperty proty = new JFProperty();
        private Type paramType;
        private JFParamDescribe paramDescribe;

        public UcJFParamEdit()
        {
            InitializeComponent();
            //SetParamType(typeof(string));
        }

        /// <summary>所有可设置的参数项Cell是否为只读</summary>
        [Category("JF属性"), Description("ValueReadOnly"), Browsable(true)]
        public bool IsValueReadOnly
        {
            get { return !pGrid.Enabled; }
            set { pGrid.Enabled = !value; }
        }

        public bool IsHelpVisible
        {
            get { return pGrid.HelpVisible; }
            set { pGrid.HelpVisible = value; }
        }

        private void UcJFInitorParam_Load(object sender, EventArgs e)
        {
            proCollect.Add(proty);
        }

        public JFParamDescribe GetParamDesribe()
        {
            return paramDescribe;
        }

        /// <summary>
        /// 设置参数描述信息
        /// </summary>
        /// <param name="pd"></param>
        public void SetParamDesribe(JFParamDescribe pd)
        {
            paramDescribe = pd;
            proty.Name = paramDescribe.DisplayName;
            proty.ParamType = paramDescribe.ParamType;
            paramType = paramDescribe.ParamType;

            if (paramDescribe.ParamLimit == JFValueLimit.FilePath)
            {
                proty.Editor = new PropertyGridFileItem();
            }
            else if (paramDescribe.ParamLimit == JFValueLimit.FolderPath)
            {
                proty.Editor = new PropertyGridFolderItem();
            }
            else if (paramDescribe.ParamLimit == JFValueLimit.Options)
            {
                proty.TConverter = new JFComboItemConvert(paramDescribe.ParamRange);
            }
            else
            {

            }



            proty.Value = GetDefaultValue(paramDescribe.ParamType);

            if (proty.Value is List<string> )
            {
                proty.Editor = new PropertyGridListStringItem();
            }
            else if(proty.Value is string[])
            {
                proty.Editor = new PropertyGridArrayStringItem();
            }
            
            if(paramDescribe.ParamType.IsValueType && !paramDescribe.ParamType.IsPrimitive && !paramDescribe.ParamType.IsEnum) //如果是结构体，使用拓展类型转换
                proty.TConverter = new ExpandableObjectConverter();

            StringBuilder sb = new StringBuilder();
            int ct = 0;
            //有最小值限制
            if ((paramDescribe.ParamLimit & JFValueLimit.MinLimit) > 0 && paramDescribe.ParamRange != null && paramDescribe.ParamRange.Length > 0)
            {
                sb.Append("Min:" + paramDescribe.ParamRange[0].ToString());
                ct++;
            }
            //有最大值限制
            if ((paramDescribe.ParamLimit & JFValueLimit.MaxLimit) > 0 && paramDescribe.ParamRange != null && paramDescribe.ParamRange.Length > ct)
            {
                if (ct > 0)
                    sb.Append("|");
                sb.Append("Max:" + paramDescribe.ParamRange[ct].ToString());
                ct++;
            }
            proty.Description = sb.ToString();
            this.pGrid.SelectedObject = proCollect;
            this.pGrid.Refresh();
        }

        private object GetDefaultValue(Type t/*Type paraType*/)
        {
            /*
            object proValue = "";
            if (paraType == typeof(string))
            {
                proValue = "";
            }
            else if (paraType == typeof(int)
                || paraType == typeof(float)
                || paraType == typeof(double))
            {
                proValue = 0;
            }
            else if (paraType == typeof(bool))
            {
                proValue = false;
            }
            else if (paraType == typeof(int[]))
            {
                proValue = new int[0] { };
            }
            else if (paraType == typeof(float[]))
            {
                proValue = new float[0] { };
            }
            else if (paraType == typeof(double[]))
            {
                proValue = new double[0] { };
            }
            else if (paraType == typeof(ArrayList[]))
            {
                proValue = new ArrayList();
            }
            else if (paraType == typeof(List<int>))
            {
                proValue = new List<int>();
            }
            else if (paraType == typeof(List<float>))
            {
                proValue = new List<float>();
            }
            else if (paraType == typeof(List<double>))
            {
                proValue = new List<double>();
            }
            else if (paraType == typeof(Bitmap))
            {
                proValue = new Bitmap(1, 1);
            }
            return proValue;
            */
            if (t == typeof(string))
                return "";
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            ConstructorInfo[] ctors = t.GetConstructors(System.Reflection.BindingFlags.Instance
                                                          | System.Reflection.BindingFlags.NonPublic
                                                          | System.Reflection.BindingFlags.Public);
            if (null == ctors)
                throw new Exception("CreateInstance(Type t) failed By: Not found t-Instance's Constructor");
            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] ps = ctor.GetParameters();
                if (ps == null || ps.Length == 0)
                    return ctor.Invoke(null);
                else if (ps.Length == 1 && ps[0].ParameterType == typeof(int)) //用于初始化数组类/
                    return ctor.Invoke(new object[] { 0 });

            }

            return null;
        }

        /// <summary>
        /// 设置参数类型
        /// </summary>
        /// <param name="t"></param>
        public void SetParamType(Type paramType, string paramName = "")
        {
            if (null == paramType) //默认以string为参数类型
                paramType = typeof(string);
            this.paramType = paramType;
            if (string.IsNullOrEmpty(paramName))
                paramName = paramType.Name;
            paramDescribe = JFParamDescribe.Create(paramName, paramType, JFValueLimit.NonLimit, null);
            proty.Name = paramDescribe.DisplayName;
            proty.ParamType = paramType;
            proty.Value = GetDefaultValue(paramDescribe.ParamType);
            this.pGrid.SelectedObject = proCollect;
            this.pGrid.Refresh();
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="pv"></param>
        public void SetParamValue(object pv)
        {
            if (pv != null)
            {
                if (proty.TConverter is JFComboItemConvert)
                {
                    JFComboItemConvert cmb = (JFComboItemConvert)proty.TConverter;
                    foreach (DictionaryEntry myDE in cmb.myRealhash)
                    {
                        if (myDE.Value.Equals(pv))
                        {
                            proty.Value = myDE.Key;
                            break;
                        }
                    }
                }
                else
                {
                    proty.Value = pv;
                }
                this.pGrid.SelectedObject = proCollect;
                this.pGrid.Refresh();
            }
            else //Added by bob
            {
                proty.Value = pv;
                this.pGrid.SelectedObject = proCollect;
                this.pGrid.Refresh();
            }
        }

        string _paramErrorInfo = "";
        /// <summary>在调用GetParamValue函数失败后，可通过此函数获取失败信息</summary>
        public string GetParamErrorInfo()
        {
            return _paramErrorInfo;
        }

        public bool GetParamValue(out object val)
        {
            val = null;
            _paramErrorInfo = "";

            try
            {
                if (proty.Value == null)
                {
                    _paramErrorInfo = "输入参数为空";
                    return false;
                }

                if (proty.TConverter is JFComboItemConvert)
                {
                    JFComboItemConvert cmb = (JFComboItemConvert)proty.TConverter;
                    if (cmb.myRealhash.ContainsKey(proty.Value))
                    { val = cmb.myRealhash[proty.Value]; }
                }
                else
                {
                    val = proty.Value;
                }
                return true;
            }
            catch (Exception ex)
            {
                _paramErrorInfo = ex.Message;
                val = null;
                return false;
            }
        }

        private void AsyncRefresh(int Delayms)
        {
            Thread theader = new Thread(new ThreadStart(new Action(() =>
            {
                Thread.Sleep(Delayms);
                if (InvokeRequired)
                {
                    Invoke(new System.Windows.Forms.MethodInvoker(this.pGrid.Refresh), null);
                }
                else
                {
                    this.pGrid.Refresh();
                }
            })));
            theader.Start();
        }
        private void UcJFParamEdit_Paint(object sender, PaintEventArgs e)
        {
            //AsyncRefresh(60); //在其他线程中被调用时，会出异常
            this.pGrid.Refresh();
            
        }

        private void pGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //有最小值限制
            if (paramType.IsValueType && (paramDescribe.ParamLimit & JFValueLimit.MinLimit) > 0 && paramDescribe.ParamRange != null && paramDescribe.ParamRange.Length > 0)
            {
                double dlValue = Convert.ToDouble(e.ChangedItem.Value);
                if (dlValue < Convert.ToDouble(paramDescribe.ParamRange[0]))
                {
                    e.ChangedItem.PropertyDescriptor.SetValue(this.pGrid.SelectedObject, e.OldValue);
                    MessageBox.Show(string.Format("取值超出范围",
                        paramDescribe.ParamRange[0].ToString(), paramDescribe.ParamRange[0].ToString()),
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            //有最大值限制
            if (paramType.IsValueType && (paramDescribe.ParamLimit & JFValueLimit.MaxLimit) > 0 && paramDescribe.ParamRange != null && paramDescribe.ParamRange.Length > 1)
            {
                double dlValue = Convert.ToDouble(e.ChangedItem.Value);
                if (dlValue > Convert.ToDouble(paramDescribe.ParamRange[1]))
                {
                    e.ChangedItem.PropertyDescriptor.SetValue(this.pGrid.SelectedObject, e.OldValue);
                    MessageBox.Show(string.Format("取值超出范围",
                        paramDescribe.ParamRange[0].ToString(), paramDescribe.ParamRange[1].ToString()),
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }

    /// <summary>
    /// 自定义属性对象
    /// </summary>
    public class JFProperty
    {
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private object value = null;
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        //Add it by Bob
        Type _paramType = typeof(object);
        public Type ParamType
        {
            get { return _paramType; }
            set { _paramType = value; }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private TypeConverter tConverter = null;
        public TypeConverter TConverter
        {
            get { return tConverter; }
            set { tConverter = value; }
        }

        private object _editor = null;
        public virtual object Editor   //属性编辑器   
        {
            get { return _editor; }
            set { _editor = value; }
        }

        public override string ToString()
        {
            return string.Format("Name:{0},Value:{1}", name.ToString(), value.ToString());
        }
    }

    /// <summary>
    /// 自定义性质描述类
    /// </summary>
    public class JFPropertyDescription : PropertyDescriptor
    {
        private JFProperty proty = null;
        public JFPropertyDescription(JFProperty proty, Attribute[] attrs) : base(proty.Name, attrs)
        {
            this.proty = proty;
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return this.GetType(); }
        }

        public override object GetValue(object component)
        {
            return proty.Value;
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get
            {
                return proty.ParamType;//return proty.Value.GetType();
            }
        }

        public override void ResetValue(object component)
        {
            //不重置，无动作 
        }

        public override void SetValue(object component, object value)
        {
            proty.Value = value;
        }
        /// <summary>
        /// 是否应该持久化保存
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        /// <summary>
        /// 属性说明
        /// </summary>
        public override string Description
        {
            get { return proty.Description; }
        }
        public override TypeConverter Converter
        {
            get { return proty.TConverter; }
        }

        public override object GetEditor(Type editorBaseType)
        {
            return proty.Editor == null ? base.GetEditor(editorBaseType) : proty.Editor;
        }
    }

    /// <summary>
    /// 实现自定义的特殊属性对象必须继承ICustomTypeDescriptor,并实现Dictionary
    /// </summary>
    public class JFPropertyCollection : Dictionary<String, JFProperty>, ICustomTypeDescriptor
    {
        /// <summary>
        /// 重写Add方法
        /// </summary>
        /// <param name="proty"></param>
        public void Add(JFProperty proty)
        {
            if (!this.ContainsKey(proty.Name))
            {
                base.Add(proty.Name, proty);
            }
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            int count = this.Values.Count;
            PropertyDescriptor[] pds = new PropertyDescriptor[count];
            int index = 0;
            foreach (JFProperty item in this.Values)
            {
                pds[index] = new JFPropertyDescription(item, attributes);
                index++;
            }
            return new PropertyDescriptorCollection(pds);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }

    //重写下拉菜单中的项，使之与属性页的项关联
    public abstract class ComboBoxItemTypeConvert : TypeConverter
    {
        public Hashtable myhash = null;

        public Hashtable myRealhash = null;
        public ComboBoxItemTypeConvert()
        {
            myhash = new Hashtable();
            myRealhash = new Hashtable();
            GetConvertHash();
        }
        public abstract void GetConvertHash();

        //是否支持选择列表的编辑
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        //重写combobox的选择列表
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            int[] ids = new int[myhash.Values.Count];
            int i = 0;
            foreach (DictionaryEntry myDE in myhash)
            {
                ids[i++] = (int)(myDE.Key);
            }
            return new StandardValuesCollection(ids);
        }

        //判断转换器是否可以工作
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);

        }
        //重写转换器，将选项列表（即下拉菜单）中的值转换到该类型的值
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object obj)
        {
            if (obj is string)
            {
                foreach (DictionaryEntry myDE in myhash)
                {
                    if (myDE.Value.Equals(obj))
                        return myDE.Key;
                    //if (myDE.Key.ToString() == obj.ToString())
                    //    return myDE.Value;
                }
            }
            // return base.ConvertFrom(context, culture, obj);
            return "";
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);

        }

        //重写转换器将该类型的值转换到选择列表中
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object obj, Type destinationType)
        {

            if (destinationType == typeof(string))
            {
                foreach (DictionaryEntry myDE in myhash)
                {
                    if (myDE.Key.Equals(obj))
                        return myDE.Value.ToString();
                }
                return "";
            }
            return base.ConvertTo(context, culture, obj, destinationType);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }
    //重写下拉菜单，在这里实现定义下拉菜单内的项
    public class JFComboItemConvert : ComboBoxItemTypeConvert
    {
        private Hashtable hash;
        private Hashtable Realhash;
        public override void GetConvertHash()
        {
            try
            {
                myhash = hash;
                myRealhash = Realhash;
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
        public JFComboItemConvert(string str)
        {
            hash = new Hashtable();
            Realhash = new Hashtable();
            string[] stest = str.Split(',');
            for (int i = 0; i < stest.Length; i++)
            {
                hash.Add(i, stest[i]);
                Realhash.Add(i, stest[i]);
            }
            GetConvertHash();
            value = 0;
        }
        public JFComboItemConvert(object[] Items)
        {
            hash = new Hashtable();
            Realhash = new Hashtable();
            if(null != Items )
                for (int i = 0; i < Items.Length; i++)
                {
                    hash.Add(i, Items[i].ToString());
                    Realhash.Add(i, Items[i]);
                }
            GetConvertHash();
            value = 0;
        }

        public int value { get; set; }

        public JFComboItemConvert(string str, int s)
        {
            hash = new Hashtable();
            Realhash = new Hashtable();
            string[] stest = str.Split(',');
            for (int i = 0; i < stest.Length; i++)
            {
                hash.Add(i, stest[i]);
                Realhash.Add(i, stest[i]);
            }
            GetConvertHash();
            value = s;
        }
    }

    public class PropertyGridFileItem : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // 可以打开任何特定的对话框
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.CheckFileExists = false;
                dialog.AddExtension = false;
                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {
                    return dialog.FileName;
                }
            }
            return value;
        }
    }

    public class PropertyGridFolderItem : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // 可以打开任何特定的对话框
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {
                    return dialog.SelectedPath;
                }
            }
            return value;
        }
    }

    public class PropertyGridListStringItem : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                
                // 可以打开任何特定的对话框
                FormListStringEdit dialog = new FormListStringEdit((List<string>)value);
                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {
                    return dialog.Value;
                }

            }
            return value;
        }
    }


    public class PropertyGridArrayStringItem : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {

                // 可以打开任何特定的对话框
                FormListStringEdit dialog = new FormListStringEdit(value == null ? null:(value as string[]).ToList()) ;
                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {
                    return dialog.Value.ToArray();
                }

            }
            return value;
        }
    }

}