using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using JFInterfaceDef;
using JFToolKits;

namespace JFHub
{
    /// <summary>
    /// IJFMethod动作流
    /// </summary>
    public class JFMethodFlow:JFCmdWorkBase
    {
        internal enum RunMode
        {
            Synch = 0, //同步运行模式（在启动Action的线程中运行）
            Async,//异步运行模式
            Step, //单步调试模式
        }


        public JFMethodFlow()
        {
            RunningMode = RunMode.Synch;
        }


        public override string Name { get; set; }

        

        Dictionary<string, object> dataPool = new Dictionary<string, object>();
        Dictionary<string, Type> typeMap = new Dictionary<string, Type>(); //保存各MethodItem的输出参数的类型

        //动作链表
        protected List<MethodItem> methodList = new List<MethodItem>();
        int currStep = -1; //当前执行步骤，-1表示未执行

        public Dictionary<string, object> DataPool { get { return dataPool; }}
        public Dictionary<string, Type> TypePool { get { return typeMap; } }
        //public Dictionary<string, Type> DataTyps { get { return typeMap; } }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
            foreach (MethodItem mi in methodList)
            {
                IJFMethod method = mi.Value;
                if (typeof(IJFStationBaseAcq).IsAssignableFrom(method.GetType()))
                    (method as IJFStationBaseAcq).SetStation(_station);
            }
        }

        /// <summary>
        /// 向数据池中添加一个数据项
        /// 如数据项已存在，则什么都不做
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void DeclearDPItem(string name,Type type,object value)
        {
            if (DataPool.ContainsKey(name))
                return;
            if(type.IsValueType && value == null)
                 value =  Activator.CreateInstance(type);
            if (value != null)
            {
                Type t = value.GetType();
                
                if (value.GetType() != type && !type.IsAssignableFrom(value.GetType()))
                {
                    value = JFConvertExt.ChangeType(value, type);
                    if(value == null)
                        throw new Exception("DeclearDPItem failed by: Value'Type = " + value.GetType().Name + " not match to " + type.Name);

                }
            }
            DataPool.Add(name, value);
            TypePool.Add(name, type);

        }

        public bool SetDP(string name,object value)
        {
            if (!DataPool.ContainsKey(name))
                return false;
            DataPool[name] = value;
            return true;
        }


        public void SetOutterDataPool(Dictionary<string,object> outterDataPool,Dictionary<string ,Type> outterTypePool,string[] outterAvailedIDs)
        {
            OutterDataPool = outterDataPool;
            OutterTypePool = outterTypePool;
            OutterAvailedIDs = outterAvailedIDs;
        }

        public Dictionary<string, object> OutterDataPool { get; set; }
        public Dictionary<string, Type> OutterTypePool { get; set; }
        public string[] OutterAvailedIDs { get; set; }


        public int CurrStep{get{ return currStep; } }

        /// <summary>从Xml节点中加载</summary>
        public void Load(XmlElement xe)
        {
            Clear();
            if (null == xe)
                return;
            //Name = xe.GetAttribute("Name");
            XmlElement xeName = xe.SelectSingleNode("MethodFlowName") as XmlElement;
            if (null != xeName)
                Name = xeName.InnerText;
            XmlElement xeMethodItems = xe.SelectSingleNode("MethodItems") as XmlElement;

            if (null != xeMethodItems && null != xeMethodItems.ChildNodes)
                foreach (XmlNode xnMethodItem in xeMethodItems.ChildNodes)
                {
                    //读进每一个"Operations"，给oi
                    MethodItem oi = CreateItemFromXE(xnMethodItem as XmlElement);
                    methodList.Add(oi);
                    if(typeof(IJFStationBaseAcq).IsAssignableFrom( oi.Value.GetType()))
                    {
                        if (null != _station)
                            (oi.Value as IJFStationBaseAcq).SetStation(_station);
                    }

                    if (typeof(IJFMethodFlowAcq).IsAssignableFrom(oi.Value.GetType()))
                    {
                        (oi.Value as IJFMethodFlowAcq).SetFlow(this);
                    }
                }
        }



        public void Load(string filePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);

            XmlElement xeRoot = xd.SelectSingleNode("root") as XmlElement;
            Load(xeRoot);

        }

        public void Save(XmlElement xe)
        {
            if (null == xe)
                return;
            xe.RemoveAll();

            //xe.SetAttribute("Name", Name);
            XmlElement xeName = xe.OwnerDocument.CreateElement("MethodFlowName");
            xe.AppendChild(xeName);
            xeName.InnerText = Name;
            XmlElement xeOperations = xe.OwnerDocument.CreateElement("MethodItems");
            xe.AppendChild(xeOperations);
            foreach (MethodItem methodItem in methodList)
            {
                XmlElement xeOpt = xe.OwnerDocument.CreateElement("MethodItem");
                xeOperations.AppendChild(xeOpt);
                SaveItemToXE(methodItem, xeOpt);
            }
            

        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            _MakesureFile(filePath);
            XmlDocument xd = new XmlDocument();

            XmlElement xeRoot = xd.CreateElement("root");
            xd.AppendChild(xeRoot);
            Save(xeRoot);
            xd.Save(filePath);
        }

        /// <summary>
        /// 将动作节点保存到XML元素中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="xe"></param>
        void SaveItemToXE(MethodItem item, XmlElement xe)
        {
            xe.SetAttribute("Name", item.Name);
            xe.SetAttribute("Type", item.Value.GetType().Name);
            //保存operation的初始化参数

            //保存IJFMethod序列化参数
            IJFMethod mv = item.Value;
            if(typeof(IJFInitializable).IsAssignableFrom(mv.GetType()))
            {
                IJFInitializable initor = mv as IJFInitializable;
                string[] initParamNames = initor.InitParamNames;
                if (null != initParamNames && initParamNames.Length > 0)
                {
                    XmlElement xeInitParams = xe.OwnerDocument.CreateElement("InitParams");
                    xe.AppendChild(xeInitParams);
                    foreach (string initParamName in initParamNames)
                    {
                        XmlElement xeInitParam = xe.OwnerDocument.CreateElement("Param");
                        xeInitParams.AppendChild(xeInitParam);
                        if (null == initor.GetInitParamValue(initParamName))
                        {
                            xeInitParam.SetAttribute("Name", initParamName);
                            xeInitParam.SetAttribute("Type", initor.GetInitParamDescribe(initParamName).GetType().AssemblyQualifiedName);
                            xeInitParam.RemoveAttribute("Value");
                        }
                        else
                        {
                            string xsValue, xsType;
                            ToFTString(initor.GetInitParamValue(initParamName), out xsValue, out xsType);
                            xeInitParam.SetAttribute("Name", initParamName);
                            xeInitParam.SetAttribute("Type", xsType);
                            xeInitParam.SetAttribute("Value", xsValue);
                        }
                    }
                }
            }
            //保存MethodItem的输入项绑定信息
            if (item.InputNameIDs.Count > 0) //保存输入项绑定信息
            {
                XmlElement xeInputNameIDs = xe.OwnerDocument.CreateElement("InputNameIDs");
                xe.AppendChild(xeInputNameIDs);
                foreach (KeyValuePair<string, string> kvNameID in item.InputNameIDs)
                {
                    XmlElement xeInputNameID = xe.OwnerDocument.CreateElement("Name-ID");
                    xeInputNameIDs.AppendChild(xeInputNameID);
                    xeInputNameID.SetAttribute("Name", kvNameID.Key);
                    xeInputNameID.SetAttribute("IDinPool", kvNameID.Value);
                }
            }

            if (item.OutputNameIDs.Count > 0) //保存输出项绑定信息
            {
                XmlElement xeOutputNameIDs = xe.OwnerDocument.CreateElement("OutputNameIDs");
                xe.AppendChild(xeOutputNameIDs);
                foreach (KeyValuePair<string, string> kvNameID in item.OutputNameIDs)
                {
                    XmlElement xeOutputNameID = xe.OwnerDocument.CreateElement("Name-ID");
                    xeOutputNameIDs.AppendChild(xeOutputNameID);
                    xeOutputNameID.SetAttribute("Name", kvNameID.Key);
                    xeOutputNameID.SetAttribute("IDinPool", kvNameID.Value);
                }
            }
        }

        protected virtual MethodItem CreateItemFromXE(XmlElement xe)
        {
            string name = xe.GetAttribute("Name");
            string optTypeName = xe.GetAttribute("Type");

            IJFMethod opt = CreateMethod(optTypeName);
            if (typeof(IJFStationBaseAcq).IsAssignableFrom(opt.GetType()))
                (opt as IJFStationBaseAcq).SetStation(_station);

            if (typeof(IJFInitializable).IsAssignableFrom(opt.GetType()))
            {
                IJFInitializable initor = opt as IJFInitializable;
                string[] initParamNames = initor.InitParamNames;
                if (null != initParamNames && initParamNames.Length > 0)
                {
                    XmlElement xeInitParams = xe.SelectSingleNode("InitParams") as XmlElement;
                    if (xeInitParams != null && null != xeInitParams.ChildNodes)
                        foreach (XmlNode xnInitParam in xeInitParams.ChildNodes)
                        {
                            XmlElement xeInitParam = xnInitParam as XmlElement;
                            string paramName = xeInitParam.GetAttribute("Name");
                            string paramType = xeInitParam.GetAttribute("Type");
                            string paramValue = null;
                            if (xeInitParam.HasAttribute("Value"))
                                paramValue = xeInitParam.GetAttribute("Value");

                            if (null == paramValue)
                                initor.SetInitParamValue(paramName, null);
                            else
                                initor.SetInitParamValue(paramName, FromFTString(paramValue, paramType));
                        }
                }
                initor.Initialize();
            }

            MethodItem item = new MethodItem(name, opt, DataPool, TypePool);
            item.SetOutterDataPool(DataPool, TypePool, DataPool.Keys.ToArray()); //此时DataPool中的Key

            //加载Item的绑定信息   //Input绑定信息
            XmlElement xeInputNameIDs = xe.SelectSingleNode("InputNameIDs") as XmlElement;

            if (null != xeInputNameIDs && null != xeInputNameIDs.ChildNodes)
                foreach (XmlNode xnInputNameID in xeInputNameIDs.ChildNodes)
                {
                    XmlElement xeInputNameID = xnInputNameID as XmlElement;
                    string inputName = xeInputNameID.GetAttribute("Name");
                    string idInPool = xeInputNameID.GetAttribute("IDinPool");
                    item.SetInputID(inputName, idInPool);
                }

            ////Output绑定信息
            XmlElement xeOutputNameIDs = xe.SelectSingleNode("OutputNameIDs") as XmlElement;
            if (null != xeOutputNameIDs && null != xeOutputNameIDs.ChildNodes)
                foreach (XmlNode xnOutputNameID in xeOutputNameIDs.ChildNodes)
                {
                    XmlElement xeOutputNameID = xnOutputNameID as XmlElement;
                    string OutputName = xeOutputNameID.GetAttribute("Name");
                    string idInPool = xeOutputNameID.GetAttribute("IDinPool");
                    item.SetOutputID(OutputName, idInPool);
                }
            item.UnexportIDs = UnexportIDs;
            return item;
        }

        static string ToFString(object o)
        {
            StringBuilder buffer = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(o.GetType());
            using (TextWriter writer = new StringWriter(buffer))
            {
                serializer.Serialize(writer, o);
            }
            return buffer.ToString();
        }

        public static void ToFTString(object o, out string xml, out string typeString)
        {
            xml = ToFString(o);
            typeString = o.GetType().AssemblyQualifiedName;
        }

        public static object FromFTString(string xml, string typeString)
        {
            Object obj = null;
            StringBuilder buffer = new StringBuilder();
            buffer.Append(xml);

            XmlSerializer serializer = new XmlSerializer(Type.GetType(typeString));

            using (TextReader reader = new StringReader(buffer.ToString()))
            {
                obj = serializer.Deserialize(reader);
            }

            return obj;
        }

        void _MakesureFile(string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            XmlTextWriter xml = new XmlTextWriter(filePath, Encoding.UTF8);
            xml.WriteStartDocument();
            xml.WriteStartElement("root");
            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Flush();
            xml.Close();

        }

        /// <summary>
        /// 数据池中不是方法输出的变量ID
        /// </summary>
        public string[] UnexportIDs
        {
            get
            {
                List<string> allIDs = DataPool.Keys.ToList();
                List<string> allOutputIDs = new List<string>();
                for (int i = 0; i < methodList.Count; i++)
                    allOutputIDs.AddRange(methodList[i].OutputNameIDs.Values);
                List<string> ret = new List<string>();
                foreach (string id in allIDs)
                    if (!allOutputIDs.Contains(id))
                        ret.Add(id);
                return ret.ToArray();
            }
        }

        /// <summary>
        /// 在动作队列末尾添加一个动作
        /// </summary>
        /// <param name="opt"></param>
        public bool Add(IJFMethod method,string name = null)
        {
            return Insert(method, -1,name);
        }

        /// <summary>
        /// 按序号插入一个动作对象
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="index"></param>
        public virtual bool Insert(IJFMethod method, int index,string name = null)  //Insert(opt, -1);
        {
            if (typeof(IJFStationBaseAcq).IsAssignableFrom(method.GetType()))
                (method as IJFStationBaseAcq).SetStation(_station);
            if (typeof(IJFMethodFlowAcq).IsAssignableFrom(method.GetType()))
                (method as IJFMethodFlowAcq).SetFlow(this);

            int nameSuffix = 0;
            string optName = method.GetType().Name + "_" + nameSuffix.ToString();
            string[] existNames = AllMethodNames; //已存在的方法名称
            List<string> existOutputIDs = new List<string>();
            int countBefore = (index < 0 || index >= methodList.Count) ? methodList.Count : index;
            for (int i = 0; i < countBefore;i++)
            {
                MethodItem itemBefore = methodList[i];
                if (null != itemBefore.OutputNameIDs && itemBefore.OutputNameIDs.Count > 0)
                    existOutputIDs.AddRange(itemBefore.OutputNameIDs.Values);
            }
            MethodItem oi = null;

            if (string.IsNullOrEmpty(name))
            {
                while (existNames.Contains(optName))
                    optName = method.GetType().Name + "_" + (++nameSuffix).ToString();
                oi = new MethodItem(optName, method, dataPool, typeMap, existOutputIDs.ToArray());
                oi.SetOutterDataPool(OutterDataPool, OutterTypePool, OutterAvailedIDs);
            }
            else
            {
                if (existNames.Contains(name))
                    return false;
                oi = new MethodItem(name, method, dataPool, typeMap, existOutputIDs.ToArray());
                oi.SetOutterDataPool(OutterDataPool, OutterTypePool, OutterAvailedIDs);
            }
            if (index < 0 || index >= methodList.Count)
                methodList.Add(oi);
            else
                methodList.Insert(index, oi);
            for(int i = countBefore + 1;i < methodList.Count;i++)
            {
                existOutputIDs.AddRange(methodList[i - 1].OutputNameIDs.Values); //本方法前的所有输出
                methodList[i].InnerAvailedIDs = existOutputIDs.ToArray();
                methodList[i].UnexportIDs = UnexportIDs;
                if (typeof(IJFMethodOutterDataPoolAcq).IsAssignableFrom(methodList[i].Value.GetType()))
                    (methodList[i].Value as IJFMethodOutterDataPoolAcq).SetOutterDataPool(DataPool, TypePool, existOutputIDs.ToArray());
            }

            return true;
        }

        /// <summary>
        /// 获取添加方法对象的默认名称
        /// </summary>
        /// <param name="methodType"></param>
        /// <returns></returns>
        public string GetDefaultNameWillAddMethod(Type methodType)
        {
            if (!typeof(IJFMethod).IsAssignableFrom(methodType))
                return null;
            int nameSuffix = 0;
            JFDisplayNameAttribute[] disName = methodType.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
            string namePerfix = methodType.Name;
            if (null != disName && disName.Length > 0)
                namePerfix = disName[0].Name;
            string ret = namePerfix + "_" + nameSuffix.ToString();
            string[] existNames = AllMethodNames;
            while (existNames.Contains(ret))
                ret = namePerfix + "_" + (++nameSuffix).ToString();
            return ret;

        }

        /// <summary>
        /// 获取所有已存在的Operator名称
        /// </summary>
        /// <returns></returns>
        public string[] AllMethodNames
        {
            get
            {
                if (methodList.Count == 0)
                    return new string[] { };
                List<string> ret = new List<string>();
                foreach (MethodItem oi in methodList)
                    ret.Add(oi.Name);
                return ret.ToArray();
            }
        }

        /// <summary>
        /// 删除一个动作
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _RemoveOutputIDs(index);
            methodList.RemoveAt(index);
            //重新整理一下所有可用的输入变量
            List<string> existOutputIDs = new List<string>();
            for (int i = 0; i < methodList.Count - 1; i++)
            {
                existOutputIDs.AddRange(methodList[i].OutputNameIDs.Values);
                methodList[i + 1].InnerAvailedIDs = existOutputIDs.ToArray();
                //methodList[i].UnexportIDs = UnexportIDs;
                if (typeof(IJFMethodOutterDataPoolAcq).IsAssignableFrom(methodList[i + 1].Value.GetType()))
                    (methodList[i + 1].Value as IJFMethodOutterDataPoolAcq).SetOutterDataPool(DataPool, TypePool, existOutputIDs.ToArray());
            }
        }

        /// <summary>
        ///  将序号为index的 OptItem的所有输出项从数据池中清除
        /// </summary>
        /// <param name="index"></param>
        void _RemoveOutputIDs(int index)
        {
            if (null == dataPool)
                return;
            string[] outputIDs = methodList[index].OutputNameIDs.Values.ToArray<string>();
            if (null == outputIDs || 0 == outputIDs.Length)
                return;
            foreach (string id in outputIDs)
            {
                if (dataPool.ContainsKey(id))
                {
                    object ov = dataPool[id];
                    if (ov is IDisposable)
                        (ov as IDisposable).Dispose();
                    dataPool.Remove(id);
                }
                if (typeMap.ContainsKey(id))
                    typeMap.Remove(id);
            }
            
        }

        /// <summary>
        /// 删除所有动作
        /// </summary>
        public void Clear()
        {
            currStep = -1;
            while (methodList.Count > 0)
                RemoveAt(0);
            
        }



        /// <summary>
        /// 执行整个动作流（在调用此函数的线程中同步运行） ,如果执行失败
        /// </summary>

        public string ActionErrorInfo() { return actionErrorInfo; }

        string actionErrorInfo = "Non-Ops";

        /// <summary>
        /// 复位动作流(主要用于调试，只是将执行标志置为未开始)
        /// </summary>
        public void Reset()
        {
            currStep = -1;
            actionErrorInfo = "Non-Ops";
            foreach(MethodItem mi in methodList)
            {
                string[] outputIDs = mi.OutputNameIDs.Values.ToArray();
                if (outputIDs != null)
                    foreach (string outputID in outputIDs)
                        if (dataPool.ContainsKey(outputID))
                            dataPool[outputID] = null;
            }
        }


        ///// <summary>
        ///// 单步执行
        ///// </summary>
        //public bool Step()
        //{
        //    if (methodList.Count == 0)
        //        return true;
        //    if (currStep >= methodList.Count - 1)//已执行到最后一步
        //        return false;
        //    currStep++;
        //    try
        //    {
        //        bool isOK = methodList[currStep].Action();
        //        if (!isOK)
        //            actionErrorInfo = methodList[currStep].ActionErrorInfo();
        //        return isOK;
        //    }
        //    catch(JFBreakMethodFlowException)
        //    {
        //        currStep = methodList.Count;
        //        return true;
        //    }
        //}

        //int _runningStatus = 0;//方法流当前状态 0表示未运行，1表示正在运行 2表示暂停 3表示退出
        //ManualResetEvent _eventCmd = new ManualResetEvent(false);


        ///// <summary>
        ///// 暂停运行 供异步调用
        ///// </summary>
        //internal void Pause()
        //{
        //    if (_runningStatus != 1)
        //        return;
        //    _eventCmd.Reset();
        //    _runningStatus = 2;
        //    if (typeof(IJFMethod_T).IsAssignableFrom(methodList[CurrStep].GetType()))
        //        (methodList[CurrStep] as IJFMethod_T).Pause();
        //}

        /// <summary>
        /// 恢复运行 供异步调用
        /// </summary>
        //internal void Resume()
        //{
        //    if (_runningStatus != 2)
        //        return;
        //    if (typeof(IJFMethod_T).IsAssignableFrom(methodList[CurrStep].GetType()))
        //        (methodList[CurrStep] as IJFMethod_T).Resume();
        //    _runningStatus = 1;
        //    _eventCmd.Set();
        //}

        /// <summary>
        /// 退出运行
        /// </summary>
        //internal void Exit()
        //{
        //    if (_runningStatus == 0 || 3 == _runningStatus)
        //        return;

        //    if(_runningStatus == 2)//当前正处于暂停状态
        //    {
        //        if (typeof(IJFMethod_T).IsAssignableFrom(methodList[CurrStep].GetType()))
        //            (methodList[CurrStep] as IJFMethod_T).Exit();
        //        _runningStatus = 3;
        //        _eventCmd.Set();
        //        return;
        //    }

        //    if (typeof(IJFMethod_T).IsAssignableFrom(methodList[CurrStep].GetType()))
        //        (methodList[CurrStep] as IJFMethod_T).Exit();
        //    _runningStatus = 3;
        //}

        /// <summary>
        /// 获取程序集中指定类型的所有子类
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="tParent"></param>
        /// <returns></returns>
        public static Type[] AllChildClass(/*string assemblyName,*/ Type tParent)   //Type[] tps = AllChirdClass("OperatorDemo", typeof(IOperation));
        {
            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies(); //获取程序加载的所有的程序集
            List<Type> typeList = new List<Type>();
            foreach (Assembly ass in asses)
            {
                try
                {
                    //Assembly ass = Assembly.Load(assemblyName);//程序集
                    Module md = ass.ManifestModule;
                    if (null == md)
                        continue;
                    Type[] types = md.GetTypes(); //模块中的所有类型
                    if (null == types)
                        continue;
                    foreach (Type type in types)
                        if (tParent.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                            //typeList.Add(tParent);
                            typeList.Add(type);
                }
                catch
                {

                }
            }
            return typeList.ToArray();   //System.Type[]
        }

        public static string[] JFMethodNames(Type tParent)//OperatorTypes()
        {
            Type[] tps = AllChildClass(/*"OperatorDemo",*/ tParent);
            List<string> ret = new List<string>();
            foreach (Type t in tps)
                ret.Add(t.Name);
            return ret.ToArray();
        }

        //创建一个动作，IOperation接口
        public static IJFMethod CreateMethod(string typeName)
        {
            Type[] tps = AllChildClass(/*"OperatorDemo", */typeof(IJFMethod));

            foreach (Type t in tps)
                if (typeName == t.Name)
                {
                    //BindingFlags.Static
                    ConstructorInfo[] ctors = t.GetConstructors(System.Reflection.BindingFlags.Instance
                                                              | System.Reflection.BindingFlags.NonPublic
                                                              | System.Reflection.BindingFlags.Public);
                    foreach (ConstructorInfo ctor in ctors)
                    {
                        ParameterInfo[] pis = ctor.GetParameters();

                        if (null == pis || 0 == pis.Length)//无参数构造
                            return ctor.Invoke(null) as IJFMethod;
                    }
                }
            return null;
        }

        public int Count { get { return methodList.Count; } }

        public override int[] AllCmds { get { return new int[] { 1 }; } } //下一步指令

        public override int[] AllCustomStatus
        {
            get
            {
                if (methodList.Count == 0)
                    return new int[] { };
                int[] ret = new int[methodList.Count];
                for (int i = 0; i < methodList.Count; i++)
                    ret[i] = i;
                return ret;
            }
        }

        public MethodItem GetItem(int index) { return methodList[index]; }

        public string ToTxt()
        {
            XmlDocument xd = new XmlDocument();
            XmlElement xe = xd.CreateElement("temp");
            Save(xe);
            return xe.InnerXml;
            
        }

        public bool FromTxt(string txt)
        {
            if(string.IsNullOrEmpty(txt))
            {
                Clear();
                return true;
            }
            XmlDocument xd = new XmlDocument();
            XmlElement xe = xd.CreateElement("temp");
            xe.InnerXml = txt;
            Load(xe);
            return true;
        }

        /// <summary>
        /// 检查是否有暂停指令到来并响应
        /// </summary>
        void _CheckPausing()
        {
            while (true)
            {
                if (CurrWorkStatus == JFWorkStatus.Pausing)
                    CheckCmd(CycleMilliseconds);
                else
                    break;
            }
        }



        public bool Action()
        {
            lock (WorkStatusLocker)
            {
                if (IsWorking())
                {
                    actionErrorInfo = "运行失败，工作流当前状态:" + CurrWorkStatus;
                    return false;
                }
                RunningMode = RunMode.Synch;
                ChangeWorkStatus(JFWorkStatus.Running);
            }

            try
            {
                
                for (currStep  = 0; currStep < methodList.Count; currStep++)//foreach (MethodItem oi in methodList)
                {
                    
                    MethodItem oi = methodList[currStep];
                    bool isOK = oi.Action();
                    if (!isOK)
                    {
                        actionErrorInfo = "Step:" + currStep + " MethodName:" + oi.Name + " ActionError:" + oi.ActionErrorInfo();
                        ChangeWorkStatus(JFWorkStatus.ErrorExit);
                        return false;
                    }
                    _CheckPausing();

                }

                
                ChangeWorkStatus(JFWorkStatus.NormalEnd);
                actionErrorInfo = "Success";
                return true;
            }
            catch (JFBreakMethodFlowException)
            {
                ChangeWorkStatus(JFWorkStatus.NormalEnd);
                actionErrorInfo = "Success";
                return true;
            }
            catch (Exception ex)
            {
                ChangeWorkStatus(JFWorkStatus.ExceptionExit);
                actionErrorInfo = "Error!Exception:" + ex.Message;
                return false;
            }


        }


        internal RunMode RunningMode { get; private set; }//_runMode = RunMode.Normal;

        public override JFWorkCmdResult Start()
        {
            lock (WorkStatusLocker)
            {
                JFWorkStatus ws = CurrWorkStatus;
                if (IsWorkingStatus(ws))
                {

                    actionErrorInfo = "当前运行状态" + ws;
                    return JFWorkCmdResult.StatusError;

                }
                RunningMode = RunMode.Async;
                return base.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  JFWorkCmdResult Step()
        {
            lock (WorkStatusLocker)
            {
                if (IsWorking())
                {
                    if (RunningMode != RunMode.Step)
                    {
                        actionErrorInfo = "当前处于运行状态,RunningMode = "+ RunningMode;
                        return JFWorkCmdResult.StatusError;
                    }
                    if (CurrStep > Count - 1)
                    {
                        actionErrorInfo = "已运行到最后一步";
                        return JFWorkCmdResult.StatusError;
                    }
                    return base.SendCmd(1,CycleMilliseconds*5);//1为单步运行指令
                }
                RunningMode = RunMode.Step;
                return base.Start();
            }
        }



        public override JFWorkCmdResult Pause(int timeoutMilliSeconds = -1)
        {
            lock(WorkStatusLocker)
            {
                if (!IsWorking())
                    return JFWorkCmdResult.StatusError;
                if (CurrWorkStatus == JFWorkStatus.Pausing)
                    return JFWorkCmdResult.Success;

                //CurrWorkStatus = JFWorkStatus.Pausing; // 先让方法流停止
                ChangeWorkStatus(JFWorkStatus.Pausing);
                if(CurrStep >-1 && CurrStep < Count) //再（尝试）暂停正在运行的方法
                {
                    IJFMethod mt = methodList[CurrStep].Value;
                    if (typeof(IJFMethod_T).IsAssignableFrom(mt.GetType()))
                        (mt as IJFMethod_T).Pause();
                }
                
                return JFWorkCmdResult.Success;

                //return base.Pause(timeoutMilliSeconds);
            }
        }


        public override JFWorkCmdResult Resume(int timeoutMilliseconds = -1)
        {
            lock (WorkStatusLocker)
            {
                if (!IsWorking())
                    return JFWorkCmdResult.StatusError;
                if (CurrWorkStatus != JFWorkStatus.Pausing)
                    return JFWorkCmdResult.Success;

                //CurrWorkStatus = JFWorkStatus.Running;
                if (CurrStep > -1 && CurrStep < Count) //再（尝试）暂停正在运行的方法
                {
                    IJFMethod mt = methodList[CurrStep].Value;
                    if (typeof(IJFMethod_T).IsAssignableFrom(mt.GetType()))
                        (mt as IJFMethod_T).Resume();
                }
                ChangeWorkStatus(JFWorkStatus.Running);

                return JFWorkCmdResult.Success;
            }
        }



        protected override void PrepareWhenWorkStart()
        {
            Reset();
            if(RunningMode == RunMode.Step)
            {
                int cmd = 0;
                while(true)
                {
                    if (WaitCmd(out cmd, CycleMilliseconds) && cmd == 1) //WaitCmd中已包含CheckCmd
                    {
                        RespCmd(JFWorkCmdResult.Success);
                        break;
                    }
                }
            }
        }

        protected override void RunLoopInWork()
        {
            for (int i = 0; i < methodList.Count; i++)//foreach (MethodItem oi in methodList)
            {
                currStep = i;
                ChangeCustomStatus(i);
                MethodItem oi = methodList[i];
                bool isOK = oi.Action();
                if (!isOK)
                {
                    actionErrorInfo = "Step:" + i + " MethodName:" + oi.Name + " ActionError:" + oi.ActionErrorInfo();
                    ChangeWorkStatus(JFWorkStatus.ErrorExit);
                    ExitWork(WorkExitCode.Error, actionErrorInfo);
                }
                if (CurrStep == Count - 1) //动作全部执行完成
                    break;
                if (RunningMode == RunMode.Step)
                {
                    int customCmd = 0;
                    while (true)
                    {
                        if (WaitCmd(out customCmd) && customCmd == 1) //等待下一步指令
                        {
                            RespCmd(JFWorkCmdResult.Success);
                            break;
                        }
                    }
                }

                if (CurrWorkStatus == JFWorkStatus.Pausing)
                    _CheckPausing();
            }

            currStep++;

            ChangeWorkStatus(JFWorkStatus.NormalEnd);
            actionErrorInfo = "Success";
            ExitWork(WorkExitCode.Normal, "");
        }

        protected override void CleanupWhenWorkExit()
        {
            
        }

        protected override void OnPause()
        {
           
        }

        protected override void OnResume()
        {
           
        }

        protected override void OnStop()
        {
            
        }

        public override string GetCmdName(int cmd)
        {
            if (cmd == 1)
                return "下一步";
            return "未知命令";
        }

        public override string GetCustomStatusName(int status)
        {
            if (status < 0 || status > methodList.Count - 1)
                return "未知状态";
            return status + ":" + methodList[status].Name;
        }



        //class ActionFailedException : Exception
        //{
        //    public ActionFailedException(string errorInfo):base(errorInfo)
        //    {

        //    }
        //}
        public class MethodItem:IDisposable
        {
            
            /// <summary>
            /// OperationItem初始化
            /// </summary>
            /// <param name="name"></param>
            /// <param name="method"></param>
            /// <param name="dataPool"></param>
            public MethodItem(string name, IJFMethod method, Dictionary<string, object> dataPool,Dictionary<string,Type> typePool,string[] aVailedIDs = null)  //OperationItem oi = new OperationItem(optName, opt, DataPool);
            {
                Name = name;
                Value = method;
                DataPool = dataPool;
                TypePool = typePool;
                InputNameIDs = new JFXmlSortedDictionary<string, string>();
                OutputNameIDs = new JFXmlSortedDictionary<string, string>();
                if(typeof(IJFMethodOutterDataPoolAcq).IsAssignableFrom(Value.GetType()))//方法内部的MethodFlow需要访问本对象的数据池
                    (Value as IJFMethodOutterDataPoolAcq).SetOutterDataPool(DataPool, TypePool, DataPool.Keys.ToArray());
                if (null == aVailedIDs)
                    InnerAvailedIDs = DataPool.Keys.ToArray(); //将数据池中当前已有的变量作为可输入项
                else
                    InnerAvailedIDs = aVailedIDs;
                //生成默认的数据池绑定键              
                //public IOperation Opt { get; private set; }
                string[] inputNames = Value.MethodInputNames;

                if (null != inputNames && inputNames.Length > 0)
                    foreach (string inputName in inputNames)
                        InputNameIDs.Add(inputName,null /*Name + "." + inputName*/);


                ///将方法的输出项注册到数据池和类型池，以供后继方法是用
                string[] outputNames = Value.MethodOutputNames;
                if (null != outputNames && outputNames.Length > 0)
                    foreach (string outputName in outputNames)
                    {
                        string outputID = Name + "." + outputName;
                        OutputNameIDs.Add(outputName, outputID);
                        if (null != DataPool)
                        {
                            if (!DataPool.ContainsKey(outputID))
                                DataPool.Add(outputID, null/*Value.GetMethodOutputValue(outputName)*/);
                            else
                                DataPool[outputID] = null;//Value.GetMethodOutputValue(outputName);
                        }

                        if(typePool != null)
                        {
                            if (!typePool.ContainsKey(outputID))
                                typePool.Add(outputID, Value.GetMethodOutputType(outputName));
                            else
                                typePool[outputID] = Value.GetMethodOutputType(outputName);
                        }
                    }

                
            }
            ~MethodItem()
            {
                Dispose(false);
            }

            /// <summary>
            /// 方法项在队列中的名称（ID）
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// 方法对象
            /// </summary>
            public IJFMethod Value { get; private set; }

            /// <summary>
            /// 方法所用到的数据池
            /// </summary>
            public Dictionary<string, object> DataPool { get; set; }

            /// <summary>
            /// 方法所用到的数据的类型池
            /// </summary>
            public Dictionary<string, Type> TypePool { get; set; }

            /// <summary>
            /// 方法所须的输入参数在数据池中的名称， Key = 方法输入参数名，Value = 参数在数据池中的名称
            /// </summary>
            internal JFXmlSortedDictionary<string, string> InputNameIDs;

            /// <summary>
            /// 方法输出参数在数据池中的名称
            /// </summary>
            internal JFXmlSortedDictionary<string, string> OutputNameIDs;

            /// <summary>
            /// 方法可用的输入项ID（本Item所属的MethodFlow内部的可用数据ID）
            /// </summary>
            public string[] InnerAvailedIDs { get; set; }
            

            public void SetOutterDataPool(Dictionary<string,object> outterDataPool,Dictionary<string,Type> outterTypePool,string[] outterAvailedIDs)
            {
                _outterDataPool = outterDataPool;
                _outterTypePool = outterTypePool;
                _outterAvailedIDs = outterAvailedIDs;
            }

            public Dictionary<string, Type> OutterTypePool { get { return _outterTypePool; } }


            Dictionary<string, object> _outterDataPool = null; //外部数据池
            Dictionary<string, Type> _outterTypePool = null;//外部类型池
            string[] _outterAvailedIDs = null;

            public string[] UnexportIDs { get; set; } //数据池中未导出的变量

            /// <summary>
            /// 方法算子所有可用的数据ID（包含内部/外部数据池中的可用对象）
            /// </summary>
            public string[] AvailedInputIDs 
            { 
                get 
                {
                    List<string> ret = new List<string>();
                    if (null != InnerAvailedIDs)
                        ret.AddRange(InnerAvailedIDs);
                    if (null != _outterAvailedIDs)
                        ret.AddRange(_outterAvailedIDs);
                    if(null != UnexportIDs)
                        ret.AddRange(UnexportIDs);
                    return ret.ToArray();
                }
            }

            public string InputID(string methodInputName)
            {
                return InputNameIDs[methodInputName];
            }

            public bool SetInputID(string methodInputName, string idInPool)  //optItem.SetInputID(inputNames[i], cbInputIDs[i].Text);
            {
                if (!Value.MethodInputNames.Contains(methodInputName))
                    return false;
                //if (!AvailedInputIDs.Contains(idInPool))
                //    return false;
                if (InputNameIDs.ContainsKey(methodInputName))
                    InputNameIDs[methodInputName] = idInPool;
                else
                    InputNameIDs.Add(methodInputName, idInPool);
                return true;
            }

            public string OutputID(string methodOutputName)
            {
                return OutputNameIDs[methodOutputName];
            }

            public void SetOutputID(string methodOutputName, string idInPool)
            {
                if (OutputNameIDs.ContainsKey(methodOutputName))
                    OutputNameIDs[methodOutputName] = idInPool;
                else
                    OutputNameIDs.Add(methodOutputName, idInPool);
            }

            string _errorInfo = "No-Ops";
            public string ActionErrorInfo()
            {
                return _errorInfo;
            }

            public bool Action()
            {
                string[] inputNames = Value.MethodInputNames;
                if (null != inputNames && inputNames.Count() > 0) //需要输入参数
                {
                    if (DataPool == null)
                        throw new Exception(Name +"方法执行失败！数据池对象未设置");
                    foreach (string inputName in inputNames)
                    {
                        if (!InputNameIDs.ContainsKey(inputName) || InputNameIDs[inputName] == null) //输入未绑定数据池中的对象
                        {
                            //throw new Exception(inputName + "is not bind to DataPool");
                            _errorInfo = Name + "方法执行失败！未绑定输入参数:" + inputName;
                            return false;
                        }

                        if(!DataPool.Keys.Contains(InputID(inputName)))//if (!AvailedInputIDs.Contains(InputID(inputName))) //数据池中不包含所需变量
                        {
                            //throw new Exception("DataPool unContains:" + InputID(inputName));
                            _errorInfo = Name + "方法执行失败！数据池中不存在数据项:" + InputID(inputName);
                            return false;
                        }

                        ///类型转换
                        Type dstType = Value.GetMethodInputType(inputName);
                        Type srcType = null;
                        if (TypePool != null && TypePool.ContainsKey(InputID(inputName))) //使用内部数据池对象
                            srcType = TypePool[InputID(inputName)];
                        else
                            srcType = _outterTypePool[InputID(inputName)];
                        if (!JFTypeExt.IsExplicitFrom(dstType, srcType))//类型不能强制转换
                        {
                            _errorInfo = Name + "方法执行失败！参数名:" + inputName + " 的输入数据类型:" + srcType.Name + "不能转化为需要的类型:" + dstType.Name;
                            return false;
                        }
                        object val = null;
                        if (TypePool != null && TypePool.ContainsKey(InputID(inputName))) //使用内部数据池对象
                            val = JFConvertExt.ChangeType(DataPool[InputID(inputName)], dstType);
                        else
                            val = JFConvertExt.ChangeType(_outterDataPool[InputID(inputName)], dstType);
                        Value.SetMethodInputValue(inputName, val);//Value.SetMethodInputValue(inputName, DataPool[InputID(inputName)]);
                    }
                }

                bool isOK = Value.Action();
                if (!isOK) //没有数据池，不需要输出
                {
                    _errorInfo = Name + "方法执行失败！错误信息:" + Value.GetActionErrorInfo();
                    return false;
                }
                string[] outputNames = Value.MethodOutputNames;
                if (null != outputNames)//
                    foreach (string outputName in outputNames)
                    {
                        if (OutputNameIDs.ContainsKey(outputName)) //需要绑定数据池中的输出项
                        {
                            if (!DataPool.ContainsKey(OutputID(outputName)))
                                DataPool.Add(OutputID(outputName), Value.GetMethodOutputValue(outputName));
                            else
                                DataPool[OutputID(outputName)] = Value.GetMethodOutputValue(outputName);
                        }
                    }
                _errorInfo = "Success";
                return true;
            }



            #region IDisposable Support
            private bool disposedValue = false; // 要检测冗余调用

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 释放托管状态(托管对象)。
                        
                    }
                    if (null != Value && Value is IDisposable)
                    {
                        (Value as IDisposable).Dispose();
                        Value = null;
                    }
                        // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                        // TODO: 将大型字段设置为 null。

                        disposedValue = true;
                }
            }

            

            // 添加此代码以正确实现可处置模式。
            public void Dispose()
            {
                // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
                Dispose(true);
                // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
                // GC.SuppressFinalize(this);
            }
            #endregion

        }

        public class MethodFlowException:Exception
        {

            public MethodFlowException():base("动作流执行出错")
            {
                FlowName = "未知";
                StepIndex = -1;
                StepName = "未知";
                ActionErrorInfo = "未知";
            }

            public MethodFlowException(string flowName,int stepIndex,string stepName,string actionErrorInfo) : base("动作流:" + flowName + " 步骤:" + stepIndex + " 方法名:"+ stepName + " 错误信息:" + actionErrorInfo)
            {
                FlowName = flowName;
                StepIndex = stepIndex;
                StepName = stepName;
                ActionErrorInfo = actionErrorInfo;
            }

            public string FlowName { get ; internal set; }
            public int StepIndex { get; internal set; }
            public string StepName { get; internal set; }

            public string ActionErrorInfo { get; internal set; }


        }

    }
}
