using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFToolKits;

namespace JFHub
{

    public class JFDevCellInfo
    {
        public JFDevCellInfo(string devID,int moduleIndex,int channelIndex)
        {
            DeviceID = devID;
            ModuleIndex = moduleIndex;
            ChannelIndex = channelIndex;
        }
        public string DeviceID { get; private set; }
        public int ModuleIndex { get; private set; }
        public int ChannelIndex { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is JFDevCellInfo)
            {
                var b = (JFDevCellInfo)obj;

                return this.DeviceID == b.DeviceID && this.ModuleIndex == b.ModuleIndex && this.ChannelIndex == b.ChannelIndex;
            }

            return base.Equals(obj);
        }
        public static bool operator ==(JFDevCellInfo left, JFDevCellInfo right)
        {
            if ((left as object) == null)
            {
                if ((right as object) == null)
                    return true;
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(JFDevCellInfo left, JFDevCellInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + DeviceID.GetHashCode();
                hash = hash * 23 + ModuleIndex.GetHashCode();
                hash = hash * 23 + ChannelIndex.GetHashCode();
                return hash;
            }
        }


    }

    /// <summary>
    /// JFDevCellNameManeger 设备名称管理类
    /// </summary>
    public class JFDevCellNameManeger
    {
        [Serializable]
        public class MDCellNameInfo //MotionDaq's Info
        {
            public MDCellNameInfo()
            {
                MotionModules = new List<List<string>>();
                CmpTrigModles = new List<List<string>>();
                DioModules = new List<List<List<string>>>();
                AioModules = new List<List<List<string>>>();
            }
            public List<List<string>> MotionModules;
            public List<List<string>> CmpTrigModles;
            public List<List<List<string>>> DioModules;
            public List<List<List<string>>> AioModules;
        }


        public JFDevCellNameManeger()
        {
            cfgFilePath = null;
            dictAi = new JFXmlDictionary<string, JFDevCellInfo>(); //存储AIO（通道单元）名称
            dictAo = new JFXmlDictionary<string, JFDevCellInfo>();
            dictDi = new JFXmlDictionary<string, JFDevCellInfo>();
            dictDo = new JFXmlDictionary<string, JFDevCellInfo>();
            dictAxis = new JFXmlDictionary<string, JFDevCellInfo>();
            dictCmpTrig = new JFXmlDictionary<string, JFDevCellInfo>();
            dictDevices = new JFXmlSortedDictionary<string, MDCellNameInfo>();
            dictMultiChannelDevices = new JFXmlSortedDictionary<string, JFXmlSortedDictionary<string, List<string>>>();
        }


        public JFDevCellNameManeger(string cfgFilePath):this()
        {
            Load(cfgFilePath, false);
        }


        JFXmlDictionary<string, JFDevCellInfo> dictAi; //存储AI(数字输入通道) 名称-〉实际通道的映射关系
        JFXmlDictionary<string, JFDevCellInfo> dictAo;
        JFXmlDictionary<string, JFDevCellInfo> dictDi;
        JFXmlDictionary<string, JFDevCellInfo> dictDo;
        JFXmlDictionary<string, JFDevCellInfo> dictAxis;
        JFXmlDictionary<string, JFDevCellInfo> dictCmpTrig; //所有比较触发器通道

        /// <summary>
        /// Key = 设备名称 ， Value = DIO/AIO/Axis/Trig集合
        /// </summary>
        JFXmlSortedDictionary<string, MDCellNameInfo> dictDevices; //保存运动控制设备命名表

        /// <summary>
        ///  用于保存（多通道）设备的命名表 ,比如光源控制器 ， 光源触发器等等
        /// Key = 设备类型 ，自定义字符串 ，"LightTrig"/"LightCtrl" ...
        /// Value: Key=设备名称（ID） ， Value = 通道名称
        /// </summary>
        JFXmlSortedDictionary<string, JFXmlSortedDictionary<string, List<string>>> dictMultiChannelDevices;


        string cfgFilePath;





        public bool Load(string filePath,bool isOpenCreate)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            if(!isOpenCreate) //打开已存在的配置文件
            {
                if (!File.Exists(filePath))
                    return false;
            }
            JFXCfg cfg = new JFXCfg();
            try
            {
                cfg.Load(filePath, true);
                cfgFilePath = filePath;

                dictAi.Clear();
                dictAo.Clear();
                dictDi.Clear();
                dictDo.Clear();
                dictAxis.Clear();
                dictCmpTrig.Clear();
                if (!cfg.ContainsItem("MotionDaqCellNames"))
                {
                    dictDevices.Clear();
                    //return true;
                }
                else
                {
                    object objDevs = cfg.GetItemValue("MotionDaqCellNames");
                    if (null == objDevs)
                    {
                        dictDevices.Clear();
                        //return true;
                    }
                    else
                    {
                        dictDevices = objDevs as JFXmlSortedDictionary<string, MDCellNameInfo>;
                        foreach (KeyValuePair<string, MDCellNameInfo> kv in dictDevices)
                        {
                            string devID = kv.Key;
                            MDCellNameInfo cellNames = kv.Value;
                            if (cellNames.DioModules != null) //Dio名称处理
                                for (int moduleIndex = 0; moduleIndex < cellNames.DioModules.Count; moduleIndex++)
                                {
                                    List<List<string>> moduleCellNames = cellNames.DioModules[moduleIndex];
                                    List<string> diCellNames = moduleCellNames[0];
                                    for (int diIndex = 0; diIndex < diCellNames.Count; diIndex++)
                                        dictDi.Add(diCellNames[diIndex], new JFDevCellInfo(devID, moduleIndex, diIndex));
                                    List<string> doCellNames = moduleCellNames[1];
                                    for (int doIndex = 0; doIndex < doCellNames.Count; doIndex++)
                                        dictDo.Add(doCellNames[doIndex], new JFDevCellInfo(devID, moduleIndex, doIndex));
                                }
                            //AIO名称  处理代码
                            if (cellNames.AioModules != null)
                                for (int moduleIndex = 0; moduleIndex < cellNames.AioModules.Count; moduleIndex++)
                                {
                                    List<List<string>> moduleCellNames = cellNames.AioModules[moduleIndex];
                                    List<string> aiCellNames = moduleCellNames[0];
                                    for (int aiIndex = 0; aiIndex < aiCellNames.Count; aiIndex++)
                                        dictAi.Add(aiCellNames[aiIndex], new JFDevCellInfo(devID, moduleIndex, aiIndex));
                                    List<string> aoCellNames = moduleCellNames[1];
                                    for (int aoIndex = 0; aoIndex < aoCellNames.Count; aoIndex++)
                                        dictAo.Add(aoCellNames[aoIndex], new JFDevCellInfo(devID, moduleIndex, aoIndex));
                                }
                            //Axis名称 处理代码
                            if (null != cellNames.MotionModules)
                                for (int moduleIndex = 0; moduleIndex < cellNames.MotionModules.Count; moduleIndex++)
                                {
                                    List<string> moduleCellNames = cellNames.MotionModules[moduleIndex];
                                    for (int axisIndex = 0; axisIndex < moduleCellNames.Count; axisIndex++)
                                        dictAxis.Add(moduleCellNames[axisIndex], new JFDevCellInfo(devID, moduleIndex, axisIndex));
                                }
                            //Trig名称 处理代码
                            if (null != cellNames.CmpTrigModles)
                                for (int moduleIndex = 0; moduleIndex < cellNames.CmpTrigModles.Count; moduleIndex++)
                                {
                                    List<string> moduleCellNames = cellNames.CmpTrigModles[moduleIndex];
                                    for (int trigIndex = 0; trigIndex < moduleCellNames.Count; trigIndex++)
                                        dictCmpTrig.Add(moduleCellNames[trigIndex], new JFDevCellInfo(devID, moduleIndex, trigIndex));
                                }
                        }
                    }
                }

                if (!cfg.ContainsItem("MultiChannelDevCellNames"))
                    dictMultiChannelDevices.Clear();
                else
                {
                    object objMCDevs = cfg.GetItemValue("MultiChannelDevCellNames");
                    dictMultiChannelDevices = objMCDevs as JFXmlSortedDictionary<string, JFXmlSortedDictionary<string, List<string>>>;
                }
            }
            catch
            {
                return false;
            }
            cfgFilePath = filePath;
            return true;
        }
        public bool Save(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;
                JFXCfg cfg = new JFXCfg();
                cfg.Load(filePath,true);
                if (!cfg.ContainsItem("MotionDaqCellNames"))
                    cfg.AddItem("MotionDaqCellNames", dictDevices);
                else
                    cfg.SetItemValue("MotionDaqCellNames", dictDevices);

                if (!cfg.ContainsItem("MultiChannelDevCellNames"))
                    cfg.AddItem("MultiChannelDevCellNames", dictMultiChannelDevices);
                else
                    cfg.SetItemValue("MultiChannelDevCellNames", dictMultiChannelDevices);
                cfg.Save(filePath);
            }
            catch
            {
                return false;
            }
            return true;
            
        }

        public bool Load()
        {
            return Load(cfgFilePath, false);
        }
        public bool Save()
        {
            return Save(cfgFilePath);
        }

        /// <summary>
        /// 所有已保存的设备名称
        /// </summary>
        /// <returns></returns>
        public string[] AllMotionDaqDevices()
        { 
            return dictDevices.Keys.ToArray(); 
        }


        /// <summary>
        /// 添加一个设备（名称）
        /// </summary>
        /// <param name="devID"></param>
        public void AddMotionDaqDevice(string devID)
        {
            if (dictDevices.ContainsKey(devID))
                return;
            MDCellNameInfo mdi = new MDCellNameInfo();
            dictDevices.Add(devID, mdi);
        }

        /// <summary>
        /// 移除一个设备（名称）
        /// </summary>
        /// <param name="devID"></param>
        public void RemoveMotionDaqDevice(string devID)
        {
            if (!dictDevices.ContainsKey(devID))
                return;
            MDCellNameInfo mdcInfo = dictDevices[devID];
            //删除所有字典中的Name-Info值对
            if (mdcInfo.DioModules != null) //设备上有Dio模块
                foreach(List<List<string>> dioModule in mdcInfo.DioModules)
                {
                    List<string> diNames = dioModule[0];
                    foreach (string diName in diNames)
                        dictDi.Remove(diName);
                    List<string> doNames = dioModule[1];
                    foreach (string doName in doNames)
                        dictDo.Remove(doName);

                }
            if(mdcInfo.AioModules != null)//有Dio模块
                foreach (List<List<string>> AioModule in mdcInfo.AioModules)
                {
                    List<string> aiNames = AioModule[0];
                    foreach (string aiName in aiNames)
                        dictAi.Remove(aiName);
                    List<string> aoNames = AioModule[1];
                    foreach (string aoName in aoNames)
                        dictAo.Remove(aoName);

                }
            if (mdcInfo.MotionModules != null)
                foreach (List<string> motionModule in mdcInfo.MotionModules)
                    foreach (string axisName in motionModule)
                        dictAxis.Remove(axisName);
            if (mdcInfo.CmpTrigModles != null)
                foreach (List<string> trigModule in mdcInfo.CmpTrigModles)
                    foreach (string trigName in trigModule)
                        dictCmpTrig.Remove(trigName);

            dictDevices.Remove(devID);
        }

        public int GetDioModuleCount(string devID)
        {
            if (null == dictDevices[devID].DioModules)
                return 0;
            return dictDevices[devID].DioModules.Count;
        }
        public void SetDioModuleCount(string devID, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetDioModuleCount(string devID, int count) failed By count = " + count);
            if(null == dictDevices[devID].DioModules)
            {
                if (0 == count)
                    return;
                dictDevices[devID].DioModules = new List<List<List<string>>>();
            }
            if (count == dictDevices[devID].DioModules.Count)
                return;
            else if(count < dictDevices[devID].DioModules.Count) //需要删除已存在的Dio
            {
                while(dictDevices[devID].DioModules.Count > count)
                {
                    List<List<string>> dioModule = dictDevices[devID].DioModules[count];
                    List<string> diChnNames = dioModule[0];
                    foreach (string diChnName in diChnNames)
                        dictDi.Remove(diChnName);
                    List<string> doChnNames = dioModule[1];
                    foreach (string doChnName in doChnNames)
                        dictDo.Remove(doChnName);
                    dictDevices[devID].DioModules.RemoveAt(count);
                }
            }
            else //需要添加新的dio模块
            {
                while (dictDevices[devID].DioModules.Count < count)
                {
                    List<List<string>> dioModule = new List<List<string>>();
                    dioModule.Add(new List<string>());//DI
                    dioModule.Add(new List<string>());//DO
                    dictDevices[devID].DioModules.Add(dioModule);
                }
            }
        }

        public int GetDiChannelCount(string devID,int moduleIndex)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            if (dictDevices[devID].DioModules.Count < moduleIndex)
                return 0;
            return dictDevices[devID].DioModules[moduleIndex][0].Count;

        }

        public void SetDiChannelCount(string devID, int moduleIndex,int count)
        {
            if (count < 0)
                throw new ArgumentException("SetDiChannelCount(string devID, int moduleIndex,int count) failed by:count = " + count);
            if (dictDevices[devID].DioModules[moduleIndex][0].Count == count)
                return;
            else if(dictDevices[devID].DioModules[moduleIndex][0].Count > count) //需要删除现有DI
            {
                while(dictDevices[devID].DioModules[moduleIndex][0].Count > count)
                {
                    string diName = dictDevices[devID].DioModules[moduleIndex][0][count];
                    dictDevices[devID].DioModules[moduleIndex][0].RemoveAt(count);
                    dictDi.Remove(diName);
                }
            }
            else//需要添加新的
            {
                while(dictDevices[devID].DioModules[moduleIndex][0].Count < count)
                {
                    int nameSuffix = dictDevices[devID].DioModules[moduleIndex][0].Count;
                    string diName = devID + "_M" + moduleIndex.ToString("D2") + "_Di" + nameSuffix.ToString("D2");
                    while(dictDi.ContainsKey(diName))
                        diName = devID + "_M" + moduleIndex.ToString("D2") + "_Di" + (++nameSuffix).ToString("D2");
                    dictDi.Add(diName, new JFDevCellInfo(devID, moduleIndex, dictDevices[devID].DioModules[moduleIndex][0].Count));
                    dictDevices[devID].DioModules[moduleIndex][0].Add(diName);
                }
            }
        }

        public string GetDiName(string devID, int moduleIndex, int index)
        {
            return dictDevices[devID].DioModules[moduleIndex][0][index];
        }

        /// <summary>
        /// 设置DiChannel的名称
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="moduleIndex"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        public void SetDiName(string devID, int moduleIndex,int index,string name) //本函数存在缺陷，当存在同名的Channel时，后一个同名的Channel会覆盖前面一个Channel在dictDi中的Map
        {
            if (name == dictDevices[devID].DioModules[moduleIndex][0][index])
                return;
            ///修改map
            string orgName = dictDevices[devID].DioModules[moduleIndex][0][index];
            if (dictDi.ContainsKey(orgName))
                dictDi.Remove(orgName);
            dictDevices[devID].DioModules[moduleIndex][0][index] = name;
            if (dictDi.ContainsKey(name))
                dictDi[name] = new JFDevCellInfo(devID, moduleIndex, index);
            else
                dictDi.Add(name, new JFDevCellInfo(devID, moduleIndex, index));
        }

        /// <summary>
        /// 设置一个DO模块中的所有cell的名称
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="moduleIndex"></param>
        /// <param name="names"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetDiNames(string devID, int moduleIndex, string[] names,out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (names.Length != GetDiChannelCount(devID, moduleIndex))
            {
                errorInfo = "Di名称列表数量:" + names.Length + "  与Di通道数:" + GetDiChannelCount(devID, moduleIndex) + "不相等";
                return false;
            }
            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorInfo = "Di名称列表中含有空字符串";
                    return false;
                }
                if(hs.Contains(name))
                {
                    errorInfo = "Di名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            //检查是否和其他模块中的Di名称冲突
            List<string> existDiNames = new List<string>();//已存在的Di名称，不包含正在设置的当前模块
            string[] allMotionDaqDevices = AllMotionDaqDevices();
            foreach (string dvID in allMotionDaqDevices)
            {
                int dioModuleCount = GetDioModuleCount(dvID);
                if (dioModuleCount > 0)
                    for(int mdIndex = 0; mdIndex < dioModuleCount; mdIndex++)
                    {
                        if (dvID == devID && mdIndex == moduleIndex) //排除当前正在设置的DI名称列表
                            continue;
                        int diCount = GetDiChannelCount(dvID, mdIndex);
                        if (diCount == 0)
                            continue;
                        existDiNames.AddRange(dictDevices[dvID].DioModules[mdIndex][0]);
                    }
            }
            var q = from a in existDiNames join b in names on a equals b select a;
            if(q.Count() > 0)
            {
                StringBuilder sbError = new StringBuilder();
                foreach(string diName in q)
                {
                    JFDevCellInfo diInfo = GetDiCellInfo(diName);
                    sbError.AppendFormat("Di名称\"{0}\"已存在于DevID:{1},ModuleIndex:{2},DiChannel:{3}\n",diName,diInfo.DeviceID,diInfo.ModuleIndex,diInfo.ChannelIndex);
                    errorInfo = sbError.ToString();
                }
                return false;
            }
            for (int i = 0; i < names.Length; i++)
                SetDiName(devID, moduleIndex, i, names[i]);
            errorInfo = "Success";
            return true;

        }

        public JFDevCellInfo GetDiCellInfo(string name)
        {
            if (!dictDi.ContainsKey(name))
                return null ;
            return dictDi[name];
        }

        

        public int GetDoChannelCount(string devID, int moduleIndex)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            if (dictDevices[devID].DioModules.Count < moduleIndex)
                return 0;
            return dictDevices[devID].DioModules[moduleIndex][1].Count;
        }

        public void SetDoChannelCount(string devID, int moduleIndex, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetDoChannelCount(string devID, int moduleIndex,int count) failed by:count = " + count);
            if (dictDevices[devID].DioModules[moduleIndex][1].Count == count)
                return;
            else if (dictDevices[devID].DioModules[moduleIndex][1].Count > count) //需要删除现有channel
            {
                while (dictDevices[devID].DioModules[moduleIndex][1].Count > count)
                {
                    string doName = dictDevices[devID].DioModules[moduleIndex][1][count];
                    dictDevices[devID].DioModules[moduleIndex][1].RemoveAt(count);
                    dictDo.Remove(doName);
                }
            }
            else//需要添加新的
            {
                while (dictDevices[devID].DioModules[moduleIndex][1].Count < count)
                {
                    int nameSuffix = dictDevices[devID].DioModules[moduleIndex][1].Count;
                    string doName = devID + "_M" + moduleIndex.ToString("D2") + "_Do" + nameSuffix.ToString("D2");
                    while (dictDo.ContainsKey(doName))
                        doName = devID + "_M" + moduleIndex.ToString("D2") + "_Do" + (++nameSuffix).ToString("D2");
                    dictDo.Add(doName, new JFDevCellInfo(devID, moduleIndex, dictDevices[devID].DioModules[moduleIndex][1].Count));
                    dictDevices[devID].DioModules[moduleIndex][1].Add(doName);
                }
            }
        }

        public string GetDoName(string devID, int moduleIndex, int index)
        {
            return dictDevices[devID].DioModules[moduleIndex][1][index];
        }

        public void SetDoName(string devID, int moduleIndex, int index, string name)
        {
            if (dictDevices[devID].DioModules[moduleIndex][1][index] == name)
                return;
            string orgName = dictDevices[devID].DioModules[moduleIndex][1][index];
            if (dictDo.ContainsKey(orgName))
                dictDo[orgName] = new JFDevCellInfo(devID, moduleIndex, index);
            else
                dictDo.Add(name, new JFDevCellInfo(devID, moduleIndex, index));
            dictDevices[devID].DioModules[moduleIndex][1][index] = name;
        }

        public bool SetDoNames(string devID, int moduleIndex, string[] names, out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (names.Length != GetDoChannelCount(devID, moduleIndex))
            {
                errorInfo = "Do名称列表数量:" + names.Length + "  与Do通道数:" + GetDoChannelCount(devID, moduleIndex) + "不相等";
                return false;
            }
            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorInfo = "Do名称列表中含有空字符串";
                    return false;
                }
                if (hs.Contains(name))
                {
                    errorInfo = "Do名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            //检查是否和其他模块中的Di名称冲突
            List<string> existDoNames = new List<string>();//已存在的Do名称，不包含正在设置的当前模块
            string[] allMotionDaqDevices = AllMotionDaqDevices();
            foreach (string dvID in allMotionDaqDevices)
            {
                int dioModuleCount = GetDioModuleCount(dvID);
                if (dioModuleCount > 0)
                    for (int mdIndex = 0; mdIndex < dioModuleCount; mdIndex++)
                    {
                        if (dvID == devID && mdIndex == moduleIndex) //排除当前正在设置的Do名称列表
                            continue;
                        int diCount = GetDoChannelCount(dvID, mdIndex);
                        if (diCount == 0)
                            continue;
                        existDoNames.AddRange(dictDevices[dvID].DioModules[mdIndex][1]);
                    }
            }
            var q = from a in existDoNames join b in names on a equals b select a;
            if (q.Count() > 0)
            {
                StringBuilder sbError = new StringBuilder();
                foreach (string doName in q)
                {
                    JFDevCellInfo doInfo = GetDoCellInfo(doName);
                    sbError.AppendFormat("Do名称\"{0}\"已存在于DevID:{1},ModuleIndex:{2},DoChannel:{3}\n", doName, doInfo.DeviceID, doInfo.ModuleIndex, doInfo.ChannelIndex);
                    errorInfo = sbError.ToString();
                }
                return false;
            }
            for (int i = 0; i < names.Length; i++)
                SetDoName(devID, moduleIndex, i, names[i]);
            errorInfo = "Success";
            return true;

        }



        public JFDevCellInfo GetDoCellInfo(string name)
        {
            if (!dictDo.ContainsKey(name))
                return null;
            return dictDo[name];
        }

        public int GetAioModuleCount(string devID)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            return dictDevices[devID].AioModules.Count ;
        }
        public void SetAioModuleCount(string devID, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetAioModuleCount(string devID, int count) failed By count = " + count);
            if (null == dictDevices[devID].AioModules)
            {
                if (0 == count)
                    return;
                dictDevices[devID].AioModules = new List<List<List<string>>>();
            }
            if (count == dictDevices[devID].AioModules.Count)
                return;
            else if (count < dictDevices[devID].AioModules.Count) //需要删除已存在的Module
            {
                while (dictDevices[devID].AioModules.Count > count)
                {
                    List<List<string>> AioModule = dictDevices[devID].AioModules[count];
                    List<string> aiChnNames = AioModule[0];
                    foreach (string aiChnName in aiChnNames)
                        dictAi.Remove(aiChnName);
                    List<string> aoChnNames = AioModule[1];
                    foreach (string aoChnName in aoChnNames)
                        dictAo.Remove(aoChnName);
                    dictDevices[devID].AioModules.RemoveAt(count);
                }
            }
            else //需要添加新的dio模块
            {
                while (dictDevices[devID].AioModules.Count < count)
                {
                    List<List<string>> aioModule = new List<List<string>>();
                    aioModule.Add(new List<string>());//DI
                    aioModule.Add(new List<string>());//DO
                    dictDevices[devID].AioModules.Add(aioModule);
                }
            }
        }

        public int GetAiChannelCount(string devID, int moduleIndex)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            if (dictDevices[devID].AioModules == null)
                return 0;
            if (dictDevices[devID].AioModules.Count < moduleIndex)
                return 0;
            return dictDevices[devID].AioModules[moduleIndex][0].Count;
        }

        public void SetAiChannelCount(string devID, int moduleIndex, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetAiChannelCount(string devID, int moduleIndex,int count) failed by:count = " + count);
            if (dictDevices[devID].AioModules[moduleIndex][0].Count == count)
                return;
            else if (dictDevices[devID].AioModules[moduleIndex][0].Count > count) //需要删除现有channel
            {
                while (dictDevices[devID].AioModules[moduleIndex][0].Count > count)
                {
                    string aiName = dictDevices[devID].AioModules[moduleIndex][0][count];
                    dictDevices[devID].AioModules[moduleIndex][0].RemoveAt(count);
                    dictAi.Remove(aiName);
                }
            }
            else//需要添加新的
            {
                //int startIdx = dictDevices[devID].AioModules[moduleIndex][0].Count;
                //for (int i = startIdx; i < startIdx + count; i++)
                //{
                //    string aiName = devID + "_M" + moduleIndex.ToString("D2") + "_Ai" + i.ToString("D2");
                //    dictDevices[devID].AioModules[moduleIndex][0].Add(aiName);
                //    dictAi.Add(aiName, new JFDevCellInfo(devID, moduleIndex, i));
                //}
                while(dictDevices[devID].AioModules[moduleIndex][0].Count < count)
                {
                    int nameSuffix = dictDevices[devID].AioModules[moduleIndex][0].Count;
                    string aiName = devID + "_M" + moduleIndex.ToString("D2") + "_Ai" + nameSuffix.ToString("D2");
                    while (dictAi.ContainsKey(aiName))
                        aiName = devID + "_M" + moduleIndex.ToString("D2") + "_Ai" + (++nameSuffix).ToString("D2");
                    dictAi.Add(aiName, new JFDevCellInfo(devID, moduleIndex, dictDevices[devID].AioModules[moduleIndex][0].Count));
                    dictDevices[devID].AioModules[moduleIndex][0].Add(aiName);
                }
            }
        }

        public string GetAiName(string devID, int moduleIndex, int index)
        {
            return dictDevices[devID].AioModules[moduleIndex][0][index];
        }

        public void SetAiName(string devID, int moduleIndex, int index, string name)
        {
            if (dictDevices[devID].AioModules[moduleIndex][0][index] == name)
                return;
            string orgName = dictDevices[devID].AioModules[moduleIndex][0][index];
            if (dictAi.ContainsKey(orgName))
                dictAi[orgName] = new JFDevCellInfo(devID, moduleIndex, index);
            else
                dictAi.Add(name, new JFDevCellInfo(devID, moduleIndex, index));
            dictDevices[devID].AioModules[moduleIndex][0][index] = name;
        }

        public bool SetAiNames(string devID, int moduleIndex, string[] names, out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (names.Length != GetAiChannelCount(devID, moduleIndex))
            {
                errorInfo = "Ai名称列表数量:" + names.Length + "  与Ai通道数:" + GetAiChannelCount(devID, moduleIndex) + "不相等";
                return false;
            }
            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorInfo = "Ai名称列表中含有空字符串";
                    return false;
                }
                if (hs.Contains(name))
                {
                    errorInfo = "Ai名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            //检查是否和其他模块中的Ai名称冲突
            List<string> existAiNames = new List<string>();//已存在的Ai名称，不包含正在设置的当前模块
            string[] allMotionDaqDevices = AllMotionDaqDevices();
            foreach (string dvID in allMotionDaqDevices)
            {
                int aioModuleCount = GetAioModuleCount(dvID);
                if (aioModuleCount > 0)
                    for (int mdIndex = 0; mdIndex < aioModuleCount; mdIndex++)
                    {
                        if (dvID == devID && mdIndex == moduleIndex) //排除当前正在设置的DI名称列表
                            continue;
                        int aiCount = GetAiChannelCount(dvID, mdIndex);
                        if (aiCount == 0)
                            continue;
                        existAiNames.AddRange(dictDevices[dvID].AioModules[mdIndex][0]);
                    }
            }
            var q = from a in existAiNames join b in names on a equals b select a;
            if (q.Count() > 0)
            {
                StringBuilder sbError = new StringBuilder();
                foreach (string aiName in q)
                {
                    JFDevCellInfo aiInfo = GetAiCellInfo(aiName);
                    sbError.AppendFormat("Ai名称\"{0}\"已存在于DevID:{1},ModuleIndex:{2},AiChannel:{3}\n", aiName, aiInfo.DeviceID, aiInfo.ModuleIndex, aiInfo.ChannelIndex);
                    errorInfo = sbError.ToString();
                }
                return false;
            }
            for (int i = 0; i < names.Length; i++)
                SetAiName(devID, moduleIndex, i, names[i]);
            errorInfo = "Success";
            return true;

        }



        public JFDevCellInfo GetAiCellInfo(string name)
        {
            if (!dictAi.ContainsKey(name))
                return null;
            return dictAi[name];
        }


        public int GetAoChannelCount(string devID, int moduleIndex)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            if (dictDevices[devID].AioModules == null)
                return 0;
            if (dictDevices[devID].AioModules.Count < moduleIndex)
                return 0;
            return dictDevices[devID].AioModules[moduleIndex][1].Count;
        }

        public void SetAoChannelCount(string devID, int moduleIndex, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetAoChannelCount(string devID, int moduleIndex,int count) failed by:count = " + count);
            if (dictDevices[devID].AioModules[moduleIndex][1].Count == count)
                return;
            else if (dictDevices[devID].AioModules[moduleIndex][1].Count > count) //需要删除现有channel
            {
                while (dictDevices[devID].AioModules[moduleIndex][1].Count > count)
                {
                    string aoName = dictDevices[devID].AioModules[moduleIndex][1][count];
                    dictDevices[devID].AioModules[moduleIndex][1].RemoveAt(count);
                    dictAo.Remove(aoName);
                }
            }
            else//需要添加新的
            {
                //int startIdx = dictDevices[devID].AioModules[moduleIndex][1].Count;
                //for (int i = startIdx; i < startIdx + count; i++)
                //{
                //    string aoName = devID + "_M" + moduleIndex.ToString("D2") + "_Ai" + i.ToString("D2");
                //    dictDevices[devID].AioModules[moduleIndex][1].Add(aoName);
                //    dictAo.Add(aoName, new JFDevCellInfo(devID, moduleIndex, i));
                //}
                while (dictDevices[devID].AioModules[moduleIndex][1].Count < count)
                {
                    int nameSuffix = dictDevices[devID].AioModules[moduleIndex][1].Count;
                    string aoName = devID + "_M" + moduleIndex.ToString("D2") + "_Ao" + nameSuffix.ToString("D2");
                    while (dictAo.ContainsKey(aoName))
                        aoName = devID + "_M" + moduleIndex.ToString("D2") + "_Ao" + (++nameSuffix).ToString("D2");
                    dictAo.Add(aoName, new JFDevCellInfo(devID, moduleIndex, dictDevices[devID].AioModules[moduleIndex][1].Count));
                    dictDevices[devID].AioModules[moduleIndex][1].Add(aoName);
                }
            }
        }

        public string GetAoName(string devID, int moduleIndex, int index)
        {
            return dictDevices[devID].AioModules[moduleIndex][1][index];
        }

        public void SetAoName(string devID, int moduleIndex, int index, string name)
        {
            if (dictDevices[devID].AioModules[moduleIndex][1][index] == name)
                return;
            string orgName = dictDevices[devID].AioModules[moduleIndex][1][index];
            if (dictAo.ContainsKey(orgName))
                dictAo[orgName] = new JFDevCellInfo(devID, moduleIndex, index);
            else
                dictAo.Add(name, new JFDevCellInfo(devID, moduleIndex, index));
            dictDevices[devID].AioModules[moduleIndex][1][index] = name;
        }

        public bool SetAoNames(string devID, int moduleIndex, string[] names, out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (names.Length != GetAoChannelCount(devID, moduleIndex))
            {
                errorInfo = "Ao名称列表数量:" + names.Length + "  与Ao通道数:" + GetAoChannelCount(devID, moduleIndex) + "不相等";
                return false;
            }
            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorInfo = "Ao名称列表中含有空字符串";
                    return false;
                }
                if (hs.Contains(name))
                {
                    errorInfo = "Ao名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            //检查是否和其他模块中的Ai名称冲突
            List<string> existAoNames = new List<string>();//已存在的Ai名称，不包含正在设置的当前模块
            string[] allMotionDaqDevices = AllMotionDaqDevices();
            foreach (string dvID in allMotionDaqDevices)
            {
                int aioModuleCount = GetAioModuleCount(dvID);
                if (aioModuleCount > 0)
                    for (int mdIndex = 0; mdIndex < aioModuleCount; mdIndex++)
                    {
                        if (dvID == devID && mdIndex == moduleIndex) //排除当前正在设置的DI名称列表
                            continue;
                        int aoCount = GetAoChannelCount(dvID, mdIndex);
                        if (aoCount == 0)
                            continue;
                        existAoNames.AddRange(dictDevices[dvID].AioModules[mdIndex][1]);
                    }
            }
            var q = from a in existAoNames join b in names on a equals b select a;
            if (q.Count() > 0)
            {
                StringBuilder sbError = new StringBuilder();
                foreach (string aoName in q)
                {
                    JFDevCellInfo aoInfo = GetAoCellInfo(aoName);
                    sbError.AppendFormat("Ao名称\"{0}\"已存在于DevID:{1},ModuleIndex:{2},AoChannel:{3}\n", aoName, aoInfo.DeviceID, aoInfo.ModuleIndex, aoInfo.ChannelIndex);
                    errorInfo = sbError.ToString();
                }
                return false;
            }
            for (int i = 0; i < names.Length; i++)
                SetAoName(devID, moduleIndex, i, names[i]);
            errorInfo = "Success";
            return true;

        }


        public JFDevCellInfo GetAoCellInfo(string name)
        {
            if (!dictAo.ContainsKey(name))
                return null;
            return dictAo[name];
        }

        public int GetAxisModuleCount(string devID)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            if (null == dictDevices[devID].MotionModules)
                return 0;
            return dictDevices[devID].MotionModules.Count;
        }
        public void SetAxisModuleCount(string devID, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetAxisModuleCount(string devID, int count) failed By count = " + count);
            if (null == dictDevices[devID].MotionModules)
            {
                if (0 == count)
                    return;
                dictDevices[devID].MotionModules = new List<List<string>>();
            }
            if (count == dictDevices[devID].MotionModules.Count)
                return;
            else if (count < dictDevices[devID].MotionModules.Count) //需要删除已存在的Module
            {
                while (dictDevices[devID].MotionModules.Count > count)
                {
                    List<string> AxisModule = dictDevices[devID].MotionModules[count];
                    foreach (string axisName in AxisModule)
                        dictAxis.Remove(axisName);
                    dictDevices[devID].MotionModules.RemoveAt(count);
                }
            }
            else //需要添加新的dio模块
            {
                while (dictDevices[devID].MotionModules.Count < count)
                {
                    List<string> axisModule = new List<string>();
                    dictDevices[devID].MotionModules.Add(axisModule);
                }
            }
        }

        public int GetAxisCount(string devID, int moduleIndex)
        {
            if(!dictDevices.ContainsKey(devID))
                return 0;
            if (dictDevices[devID].MotionModules == null)
                return 0;
            return dictDevices[devID].MotionModules[moduleIndex].Count;
        }

        public void SetAxisCount(string devID, int moduleIndex, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetAxisCount(string devID, int moduleIndex, int count) failed by:count = " + count);
            if (!dictDevices.ContainsKey(devID))
                return ;
            if(dictDevices[devID].MotionModules == null)
            {
                if (count == 0)
                    return;
                dictDevices[devID].MotionModules = new List<List<string>>();
            }
            if (count == dictDevices[devID].MotionModules[moduleIndex].Count)
                return;
            else if(dictDevices[devID].MotionModules[moduleIndex].Count > count) //需要删除轴
            {
                while(dictDevices[devID].MotionModules[moduleIndex].Count > count)
                {
                    string axisName = dictDevices[devID].MotionModules[moduleIndex][count];
                    dictAxis.Remove(axisName);
                    dictDevices[devID].MotionModules[moduleIndex].RemoveAt(count);
                }
            }
            else //需要添加轴
            {
                while (dictDevices[devID].MotionModules[moduleIndex].Count < count)
                {
                    int nameSuffix = dictDevices[devID].MotionModules[moduleIndex].Count;
                    string axisName = devID + "_M" + moduleIndex.ToString("D2") + "_Axis" + nameSuffix.ToString("D2");
                    while(dictAxis.ContainsKey(axisName))
                    {
                        nameSuffix++;
                        axisName = devID + "_M" + moduleIndex.ToString("D2") + "_Axis" + nameSuffix.ToString("D2");
                    }
                    
                    dictAxis.Add(axisName, new JFDevCellInfo(devID, moduleIndex, dictDevices[devID].MotionModules[moduleIndex].Count));
                    dictDevices[devID].MotionModules[moduleIndex].Add(axisName);
                }
            }
        }

        public string GetAxisName(string devID, int moduleIndex, int index)
        {
            return dictDevices[devID].MotionModules[moduleIndex][index];
        }

        public void SetAxisName(string devID, int moduleIndex, int index, string name)
        {
            if (name == dictDevices[devID].MotionModules[moduleIndex][index])
                return;
            string orgName = dictDevices[devID].MotionModules[moduleIndex][index];
            if (dictAxis.ContainsKey(orgName))
                dictAxis.Remove(orgName);
            if (dictAxis.ContainsKey(name))
                dictAxis[name] = new JFDevCellInfo(devID, moduleIndex, index);
            else
                dictAxis.Add(name, new JFDevCellInfo(devID, moduleIndex, index));
            dictDevices[devID].MotionModules[moduleIndex][index] = name;
        }

        public bool SetAxisNames(string devID, int moduleIndex, string[] names, out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (names.Length != GetAxisCount(devID, moduleIndex))
            {
                errorInfo = "Axis名称列表数量:" + names.Length + "  与Axis数:" + GetAxisCount(devID, moduleIndex) + "不相等";
                return false;
            }
            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorInfo = "Axis名称列表中含有空字符串";
                    return false;
                }
                if (hs.Contains(name))
                {
                    errorInfo = "Axis名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            //检查是否和其他模块中的Ai名称冲突
            List<string> existAxisNames = new List<string>();//已存在的Ai名称，不包含正在设置的当前模块
            string[] allMotionDaqDevices = AllMotionDaqDevices();
            foreach (string dvID in allMotionDaqDevices)
            {
                int MotionCount = GetAxisModuleCount(dvID);
                if (MotionCount > 0)
                    for (int mdIndex = 0; mdIndex < MotionCount; mdIndex++)
                    {
                        if (dvID == devID && mdIndex == moduleIndex) //排除当前正在设置的DI名称列表
                            continue;
                        int axisCount = GetAxisCount(dvID, mdIndex);
                        if (axisCount == 0)
                            continue;
                        existAxisNames.AddRange(dictDevices[dvID].MotionModules[mdIndex]);
                    }
            }
            var q = from a in existAxisNames join b in names on a equals b select a;
            if (q.Count() > 0)
            {
                StringBuilder sbError = new StringBuilder();
                foreach (string axisName in q)
                {
                    JFDevCellInfo axisInfo = GetAxisCellInfo(axisName);
                    sbError.AppendFormat("Axis名称\"{0}\"已存在于DevID:{1},ModuleIndex:{2},AxisChannel:{3}\n", axisName, axisInfo.DeviceID, axisInfo.ModuleIndex, axisInfo.ChannelIndex);
                    errorInfo = sbError.ToString();
                }
                return false;
            }
            for (int i = 0; i < names.Length; i++)
                SetAxisName(devID, moduleIndex, i, names[i]);
            errorInfo = "Success";
            return true;

        }




        public JFDevCellInfo GetAxisCellInfo(string name)
        {
            if (!dictAxis.ContainsKey(name))
                return null;
            return dictAxis[name];
        }

        public int GetCmpTrigModuleCount(string devID)
        {
            if (!dictDevices.ContainsKey(devID))
                return 0;
            if(dictDevices[devID].CmpTrigModles == null)
                return 0;
            return dictDevices[devID].CmpTrigModles.Count;
        }
        public void SetCmpTrigModuleCount(string devID, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetTrigModuleCount(string devID, int count) failed by:count = " + count);
            if(dictDevices[devID].CmpTrigModles ==null)
            {
                if (count == 0)
                    return;
                dictDevices[devID].CmpTrigModles = new List<List<string>>();
            }
            if (dictDevices[devID].CmpTrigModles.Count == count)
                return;
            else if(dictDevices[devID].CmpTrigModles.Count > count)//需要删除模块
            {
                while(dictDevices[devID].CmpTrigModles.Count > count)
                {
                    List<string> module = dictDevices[devID].CmpTrigModles[count];
                    if (null != module)
                        foreach (string trgName in module)
                            dictCmpTrig.Remove(trgName);
                    dictDevices[devID].CmpTrigModles.RemoveAt(count);
                }
            }
            else//需要添加模块
            {
                while (dictDevices[devID].CmpTrigModles.Count < count)
                {
                    dictDevices[devID].CmpTrigModles.Add(new List<string>());
                }
            }

        }

        public int GetCmpTrigCount(string devID, int moduleIndex)
        {
            return dictDevices[devID].CmpTrigModles[moduleIndex].Count;
            
        }

        public void SetCmpTrigCount(string devID, int moduleIndex, int count)
        {
            if (count < 0)
                throw new ArgumentException("SetTrigCount(string devID, int moduleIndex, int count) failed by count = " + count);
            if(null == dictDevices[devID].CmpTrigModles[moduleIndex])
            {
                if (count == 0)
                    return;
                dictDevices[devID].CmpTrigModles[moduleIndex] = new List<string>();
            }
            if (dictDevices[devID].CmpTrigModles[moduleIndex].Count == count)
                return;
            else if(dictDevices[devID].CmpTrigModles[moduleIndex].Count > count) //需要删除现有
            {
                while(dictDevices[devID].CmpTrigModles[moduleIndex].Count > count)
                {
                    string trgName = dictDevices[devID].CmpTrigModles[moduleIndex][count];
                    dictCmpTrig.Remove(trgName);
                    dictDevices[devID].CmpTrigModles[moduleIndex].RemoveAt(count);
                }
            }
            else//需要添加
            {
                while(dictDevices[devID].CmpTrigModles[moduleIndex].Count < count)
                {
                    int nameSuffix = dictDevices[devID].CmpTrigModles[moduleIndex].Count;
                    string trigName = devID + "_M" + moduleIndex.ToString("D2") + "_Trig" + nameSuffix.ToString("D2");
                    while (dictCmpTrig.ContainsKey(trigName))
                    {
                        nameSuffix++;
                        trigName = devID + "_M" + moduleIndex.ToString("D2") + "_Trig" + nameSuffix.ToString("D2");
                    }

                    dictCmpTrig.Add(trigName, new JFDevCellInfo(devID, moduleIndex, dictDevices[devID].CmpTrigModles[moduleIndex].Count));
                    dictDevices[devID].CmpTrigModles[moduleIndex].Add(trigName);
                }
            }
        }

        public string GetCmpTrigName(string devID, int moduleIndex, int index)
        {
            return dictDevices[devID].MotionModules[moduleIndex][index];
        }

        public void SetCmpTrigName(string devID, int moduleIndex, int index, string name)
        {
            dictDevices[devID].MotionModules[moduleIndex][index] = name;
        }

        public bool SetCmpTrigNames(string devID, int moduleIndex, string[] names, out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (names.Length != GetCmpTrigCount(devID, moduleIndex))
            {
                errorInfo = "Trig名称列表数量:" + names.Length + "  与Trig数:" + GetCmpTrigCount(devID, moduleIndex) + "不相等";
                return false;
            }
            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorInfo = "Trig名称列表中含有空字符串";
                    return false;
                }
                if (hs.Contains(name))
                {
                    errorInfo = "Trig名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            //检查是否和其他模块中的Ai名称冲突
            List<string> existTrigNames = new List<string>();//已存在的Ai名称，不包含正在设置的当前模块
            string[] allMotionDaqDevices = AllMotionDaqDevices();
            foreach (string dvID in allMotionDaqDevices)
            {
                int TrigModuleCount = GetCmpTrigModuleCount(dvID);
                if (TrigModuleCount > 0)
                    for (int mdIndex = 0; mdIndex < TrigModuleCount; mdIndex++)
                    {
                        if (dvID == devID && mdIndex == moduleIndex) //排除当前正在设置的DI名称列表
                            continue;
                        int axisCount = GetAxisCount(dvID, mdIndex);
                        if (axisCount == 0)
                            continue;
                        existTrigNames.AddRange(dictDevices[dvID].CmpTrigModles[mdIndex]);
                    }
            }
            var q = from a in existTrigNames join b in names on a equals b select a;
            if (q.Count() > 0)
            {
                StringBuilder sbError = new StringBuilder();
                foreach (string trigName in q)
                {
                    JFDevCellInfo trigInfo = GetAxisCellInfo(trigName);
                    sbError.AppendFormat("Trig名称\"{0}\"已存在于DevID:{1},ModuleIndex:{2},TrigChannel:{3}\n", trigName, trigInfo.DeviceID, trigInfo.ModuleIndex, trigInfo.ChannelIndex);
                    errorInfo = sbError.ToString();
                }
                return false;
            }
            for (int i = 0; i < names.Length; i++)
                SetCmpTrigName(devID, moduleIndex, i, names[i]);
            errorInfo = "Success";
            return true;

        }


        public JFDevCellInfo GetCmpTrigCellInfo(string name)
        {
            return dictCmpTrig[name];
        }

        /// <summary>
        /// 获取所有AI通道名称
        /// </summary>
        /// <returns></returns>
        public string[] AllAiNames()
        {
            return dictAi.Keys.ToArray();
        }

        public string[] AllAoNames()
        {
            return dictAo.Keys.ToArray();
        }

        public string[] AllDiNames()
        {
            return dictDi.Keys.ToArray();
        }

        public string[] AllDoNames()
        {
            return dictDo.Keys.ToArray();
        }

        public string[] AllAxisNames()
        {
            return dictAxis.Keys.ToArray();
        }


        public string[] AllCmpTrigNames()
        {
            return dictCmpTrig.Keys.ToArray();
        }


        public bool ContainAiName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return dictAi.ContainsKey(name);
        }

        public bool ContainAoName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return dictAo.ContainsKey(name);
        }

        public bool ContainDiName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return dictDi.ContainsKey(name);
        }

        public bool ContainDoName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return dictDo.ContainsKey(name);
        }

        public bool ContainAxisName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return dictAxis.ContainsKey(name);
        }


        public bool ContainCmpTrigName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return dictCmpTrig.ContainsKey(name);
        }

        #region    多通道设备命名表
        static string DC_LightCtrl = "LightCtrl"; //Device Category : 光源控制器(带触发功能)
        static string DC_TrigCtrl = "TrigCtrl";//Device Category : (触发板)光源触发控制器
        /// <summary>
        /// 获取指定类型设备的所有实例名称
        /// </summary>
        /// <param name="devCategoty"></param>
        /// <returns></returns>
        List<string> AllMChnDevIDs(string devCategoty)
        {
            if (!dictMultiChannelDevices.ContainsKey(devCategoty))
                return null;
            return dictMultiChannelDevices[devCategoty].Keys.ToList();
        }

        /// <summary>
        /// devCategoty类别的设备名称表是否包含devID
        /// </summary>
        /// <param name="devCategoty"></param>
        /// <param name="devID"></param>
        /// <returns></returns>
        bool ContainMChnDev(string devCategoty,string devID)
        {
            if (!dictMultiChannelDevices.ContainsKey(devCategoty))
                return false;
            return dictMultiChannelDevices[devCategoty].ContainsKey(devID);
        }

        /// <summary>
        /// 添加一个多通道设备实例
        /// </summary>
        /// <param name="devCategoty"></param>
        /// <param name="devID"></param>
        void AddMChnDev(string devCategoty,string devID)
        {
            if (!dictMultiChannelDevices.ContainsKey(devCategoty))
                dictMultiChannelDevices.Add(devCategoty, new JFXmlSortedDictionary<string, List<string>>());
            if (dictMultiChannelDevices[devCategoty].ContainsKey(devID))
                return;
            dictMultiChannelDevices[devCategoty].Add(devID, new List<string>());
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="devCategoty"></param>
        /// <param name="devID"></param>
        void RemoveMChnDev(string devCategoty, string devID)
        {
            if (!dictMultiChannelDevices.ContainsKey(devCategoty))
                return;
            if (!dictMultiChannelDevices[devCategoty].ContainsKey(devID))
                return;
            dictMultiChannelDevices[devCategoty].Remove(devID);
        }
        

        List<string> AllChannelNamesInDev(string devCategoty,string devID)
        {
            if (!dictMultiChannelDevices.ContainsKey(devCategoty))
                return null;
            if (!dictMultiChannelDevices[devCategoty].ContainsKey(devID))
                return null;
            return dictMultiChannelDevices[devCategoty][devID];
        }

        int GetMChnDevChannelCount(string devCategoty, string devID)
        {
            return dictMultiChannelDevices[devCategoty][devID].Count;
        }

        void SetMChnDevChannelCount(string devCategoty, string devID , int chnCount)
        {
            int currCount = dictMultiChannelDevices[devCategoty][devID].Count;
            if (chnCount == currCount)
                return;
            else if(chnCount < currCount) //需要删除现有通道
            {
                dictMultiChannelDevices[devCategoty][devID].RemoveRange(chnCount, currCount - chnCount);
            }
            else //需要新增通道
            {
                for(int i = currCount; i < chnCount;i++)
                {
                    int nSuffix = i;
                    string newChnName = devID + "_通道" + nSuffix;
                    List<string> allChannelNames = AllChannelNamesInDev(devCategoty, devID);
                    //if (null == allChannelNames)
                    //{

                    //}
                    //else
                    {
                        while (AllChannelNamesInDev(devCategoty, devID).Contains(newChnName))
                        {
                            nSuffix++;
                            newChnName = devID + "_通道" + nSuffix;
                        }
                    }
                    dictMultiChannelDevices[devCategoty][devID].Add(newChnName);
                }
            }
        }


        string GetMChnDevChannelName(string devCategoty, string devID,int chnIndex)
        {
            return dictMultiChannelDevices[devCategoty][devID][chnIndex];
        }

        void SetMChnDevChannelName(string devCategoty, string devID, int chnIndex,string name)
        {
            dictMultiChannelDevices[devCategoty][devID][chnIndex] = name;
        }

        // 根据通道名称获取通道信息（ModuleIndex 字段无效）
        JFDevCellInfo GetMChnDevCellInfo(string devCategoty,string chnName)
        {
            if (!dictMultiChannelDevices.ContainsKey(devCategoty))
                return null;
            JFXmlSortedDictionary<string, List<string>> devs = dictMultiChannelDevices[devCategoty];
            foreach (KeyValuePair<string, List<string>> dev in devs)
            {
                List<string> chnNames = dev.Value;
                for (int i = 0; i < chnNames.Count; i++)
                    if (chnNames[i] == chnName)
                        return new JFDevCellInfo(dev.Key, 0, i);
            }
            return null;
        }
        #endregion

        /// <summary>
        ///  所有开关式光源控制器设备名称
        /// </summary>
        public string[] AllLightCtrlDevs()
        {
            List<string> ret = AllMChnDevIDs(DC_LightCtrl);
            if (null == ret)
                return null;
            return ret.ToArray();
        }


        public bool ContainLightCtrlDev(string devID)
        {
            return ContainMChnDev(DC_LightCtrl, devID);
        }


        public void AddLightCtrlDev(string devID)
        {
            AddMChnDev(DC_LightCtrl, devID);
        }

        public void RemoveLightCtrlDev(string devID)
        {
            RemoveMChnDev(DC_LightCtrl, devID);
        }


        /// <summary>
        /// 获取所有已命名的光源通道名称
        /// </summary>
        /// <returns></returns>
        public string[] AllLightChannelNames()
        {
            List<string> ret = new List<string>();
            string[] devIDs = AllLightCtrlDevs();
            if(null != devIDs)
                foreach(string devID in devIDs)
                {
                    string[] chnNames = AllChannelNamesInLightCtrlDev(devID);
                    if (null != chnNames)
                        ret.AddRange(chnNames);
                }
            return ret.ToArray();
        }

        public bool ContainLightChannelName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            string[] devIDs = AllLightCtrlDevs();
            if (null != devIDs)
                foreach (string devID in devIDs)
                {
                    List<string> ret = AllChannelNamesInDev(DC_LightCtrl, devID);
                    if (ret != null && ret.Contains(name))
                        return true;
                }
            return false;
        }




        public string[] AllChannelNamesInLightCtrlDev(string devID)
        {
            List<string> ret = AllChannelNamesInDev(DC_LightCtrl, devID);
            if (null == ret)
                return null;
            return ret.ToArray();
        }

        public int GetLightCtrlChannelCount( string devID)
        {
            return GetMChnDevChannelCount(DC_LightCtrl, devID);
            
        }

        public void SetLightCtrlChannelCount(string devID, int chnCount)
        {

            SetMChnDevChannelCount(DC_LightCtrl, devID, chnCount);
           
        }


        public string GetLightCtrlChannelName(string devID, int chnIndex)
        {
            return GetMChnDevChannelName(DC_LightCtrl, devID, chnIndex);
        }

        public void SetLightCtrlChannelName( string devID, int chnIndex, string name)
        {
            SetMChnDevChannelName(DC_LightCtrl, devID, chnIndex, name);
            
        }

        // 根据通道名称获取通道信息（ModuleIndex 字段无效）
        public JFDevCellInfo GetLightCtrlChannelInfo( string chnName)
        {
            return GetMChnDevCellInfo(DC_LightCtrl, chnName);
        }

        public bool SetLightCtrlChannelNames(string devID, string[] names, out string errInfo)
        {
            return SetMChnDevChannelNames(DC_LightCtrl, devID, names, out errInfo);
        }



        /// <summary>
        /// 多通道触发式控制器
        /// </summary>
        /// <returns></returns>
        public string[] AllTrigCtrlDevs()
        {
            List<string> ret = AllMChnDevIDs(DC_TrigCtrl);
            if (null == ret)
                return null;
            return ret.ToArray();
        }


        public bool ContainTrigCtrlDev(string devID)
        {
            return ContainMChnDev(DC_TrigCtrl, devID);
        }


        public void AddTrigCtrlDev(string devID)
        {
            AddMChnDev(DC_TrigCtrl, devID);
        }

        public void RemoveTrigCtrlDev(string devID)
        {
            RemoveMChnDev(DC_TrigCtrl, devID);
        }

        /// <summary>
        /// 所有触发式光源控制器的通道名称
        /// </summary>
        /// <returns></returns>
        public string[] AllTrigChannelNames()
        {
            List<string> ret = new List<string>();
            string[] devIDs = AllTrigCtrlDevs();
            if (null != devIDs)
                foreach (string devID in devIDs)
                {
                    string[] chnNames = AllChannelNamesInTrigCtrlDev(devID);
                    if (null != chnNames)
                        ret.AddRange(chnNames);
                }
            return ret.ToArray();
        }

        /// <summary>
        /// 是否包含光源触发通道名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainTrigChannelName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            string[] devIDs = AllTrigCtrlDevs();
            if (null != devIDs)
                foreach (string devID in devIDs)
                {
                    List<string> ret = AllChannelNamesInDev(DC_TrigCtrl, devID);
                    if (null != ret && ret.Contains(name))
                        return true;
                }
            return false;
        }

        public string[] AllChannelNamesInTrigCtrlDev( string devID)
        {
            List<string> ret = AllChannelNamesInDev(DC_TrigCtrl, devID);
            if (null == ret)
                return null;
            return ret.ToArray();
        }

        public int GetTrigCtrlChannelCount(string devID)
        {
            return GetMChnDevChannelCount(DC_TrigCtrl, devID);

        }

        public void SetTrigCtrlChannelCount(string devID, int chnCount)
        {

            SetMChnDevChannelCount(DC_TrigCtrl, devID, chnCount);

        }


        public string GetTrigCtrlChannelName(string devID, int chnIndex)
        {
            return GetMChnDevChannelName(DC_TrigCtrl, devID, chnIndex);
        }

        //设置单个通道的名称
        public void SetTrigCtrlChannelName(string devID, int chnIndex, string name)
        {
            SetMChnDevChannelName(DC_TrigCtrl, devID, chnIndex, name);
        }


        public  bool SetMChnDevChannelNames(string category,string devID, string[] names, out string errInfo)
        {
            errInfo = "Success";
            if (!ContainMChnDev(category,devID))
            {
                errInfo = "名称表中未包含设备:" + devID;
                return false;
            }

            if (names == null || names.Length != GetMChnDevChannelCount(category,devID))
            {
                errInfo = "通道数量错误！";
                return false;
            }

            HashSet<string> hs = new HashSet<string>();
            foreach (string name in names) //判断有无空字串,有无重复名称
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errInfo = "名称列表中含有空字符串";
                    return false;
                }
                if (hs.Contains(name))
                {
                    errInfo = "名称列表中含有相同项:" + name;
                    return false;
                }
                hs.Add(name);
            }
            
            ////检测是否和其他控制器通道名称冲突
            List<string> existDevIDs = AllMChnDevIDs(category); // 所有同类型设备名称
            foreach(string existDevID in existDevIDs)
            {
                if (existDevID == devID)
                    continue;
                List<string> existDevChannelNames = AllChannelNamesInDev(category, existDevID);
                for(int i = 0; i < names.Length;i++)
                {
                    string name = names[i];
                    if (existDevChannelNames.Contains(name))
                    {
                        errInfo = "通道名称：" + name + " 与设备：" + existDevID + "  通道序号:" + i + " 冲突";
                        return false;
                    }
                }

            }
            for (int i = 0; i < names.Length; i++)
                SetMChnDevChannelName(category, devID, i, names[i]);
            return true;
        }

        /// <summary>
        /// 一次设置整个设备的所有通道名称（带错误检查）
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="names"></param>
        public bool SetTrigCtrlChannelNames(string devID,string[] names,out string errInfo)
        {
            return SetMChnDevChannelNames(DC_TrigCtrl, devID, names, out errInfo);
        }

        // 根据通道名称获取通道信息（ModuleIndex 字段无效）
        public JFDevCellInfo GetTrigCtrlChannelInfo(string chnName)
        {
            return GetMChnDevCellInfo(DC_TrigCtrl, chnName);
        }
    }
}
