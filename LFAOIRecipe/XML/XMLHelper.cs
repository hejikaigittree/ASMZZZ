using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using HalconDotNet;
using System.Xml.XPath;
using System.Collections.ObjectModel;

namespace LFAOIRecipe
{
    class XMLHelper
    {
        public static bool CheckIdentifier(string xmlPath, string identifyString)
        {
            try
            {
                XElement root = XElement.Load(xmlPath);
                if (root == null 
                    || root.Element("Identifier") == null 
                    || !root.Element("Identifier").Value.ToString().Equals(identifyString))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void AddIdentifier(XElement root, string identifyString)
        {
            root.Add(new XElement("Identifier", identifyString));
        }

        public static void AddRegion(XElement fatherNode, IEnumerable<UserRegion> userRegions, string name,
                                     bool isSaveParameter = false,
                                     bool isSaveAlgoParameter = false,
                                     bool isSaveModelAgloParameter = false,
                                     bool isSaveRegAlgoParameter = false,//AroundBond周围检测参数保存
                                     bool isSaveBondAlgoParameter = false//bond 检测区域内检测参数
                                    )
        {
            if (userRegions.Count() == 0) return;
            else
            {
                XElement xElement = new XElement(name);
                foreach (var userRegion in userRegions)
                {
                    xElement.Add(userRegion.ToXElement("Region", isSaveParameter, isSaveAlgoParameter, isSaveModelAgloParameter, isSaveRegAlgoParameter, isSaveBondAlgoParameter));
                }
                fatherNode.Add(xElement);
            }
        }
        //add by wj 2020-10-22
        public static void AddOnRecipes(XElement fatherNode, ObservableCollection<OnRecipe> onRecipes, string name)
        {
            if (onRecipes.Count() == 0) return;
            else
            {
                XElement xElement = new XElement(name);

                foreach (var onRecipe in onRecipes)
                {
                    xElement.Add(onRecipe.ToXElement("ToRecipe"));

                 }
                fatherNode.Add(xElement);
            }
        }

        public static void AddRegionOne(XElement fatherNode, UserRegion userRegion, string name, bool isSaveParameter = false)//
        {
            if (userRegion == null) return;
            else
            {
                XElement xElement = new XElement(name);
                xElement.Add(userRegion.ToXElement("Region", isSaveParameter));
                fatherNode.Add(xElement);
            }
        }

        public static void AddRegionsIsEnable(IEnumerable<UserRegion> userRegions, string name, string IdentifyString = "",int RefineUserRegionsIdx = 0 )//
        {
            if (userRegions.Count() == 0) return;
            HTuple DieRegionsTemp = new HTuple();
            HTuple ICRegionsTemp = new HTuple();
            HTuple RefineUserRegionsTempt = new HTuple();

            foreach (var userRegion in userRegions)
            {
            if (IdentifyString == "ICRECIPEXML")
                {
                    ICRegionsTemp = ICRegionsTemp.TupleConcat(userRegion.IsEnable ? 1 : 0);
                }
                else if (IdentifyString == "BONRECIPEXML")
                {
                    RefineUserRegionsTempt = RefineUserRegionsTempt.TupleConcat(userRegion.IsEnable ? 1 : 0);
                }
            }
             if (IdentifyString == "ICRECIPEXML")
            {
                //HOperatorSet.WriteTuple(ICRegionsTemp, FilePath.PathMainIC + name + "_" + "IsEnable.tup");
            }
            else if (IdentifyString == "BONRECIPEXML")
            {
                //HOperatorSet.WriteTuple(RefineUserRegionsTempt, FilePath.PathBondWire + "第" + RefineUserRegionsIdx + "套精炼区" + "_" + "IsEnable.tup");
            }
        }

        public static void AddParameters(XElement fatherNode, IParameter parameter, string IdentifyString = "")//
        {
            Type type = parameter.GetType();
            PropertyInfo[] pis = type.GetProperties();
            XElement xElement;

            foreach (var pi in pis)
            {
                xElement = new XElement(pi.Name);         
                System.Diagnostics.Debug.WriteLine(pi.Name);

                switch (pi.PropertyType.ToString())
                {
                    case "System.Boolean":
                        xElement.Value = (bool)pi.GetValue(parameter) ? "1" : "0";
                        break;

                    case "System.Int32"://修改
                        xElement.Value = pi.GetValue(parameter).ToString();//
                        break;

                    case "System.Double"://
                        xElement.Value = pi.GetValue(parameter).ToString();//
                        break;

                    case "System.Double[]":
                        xElement.Add(((double[])(pi.GetValue(parameter))).Select(l => new XElement("para", l)));   
                        break;

                    case "System.String[]":
                        xElement.Add(((string[])(pi.GetValue(parameter))).Select(l => new XElement("para", l)));
                        break;
                    case "System.Int32[]":
                        xElement.Add(((int[])(pi.GetValue(parameter))).Select(l => new XElement("para", l)));
                        break;
                    default:
                        xElement.Value = pi.GetValue(parameter).ToString();
                        break;
                }
                fatherNode.Add(xElement);
            }
        }

        public static void ReadParameters(XElement fatherNode, IParameter parameter)
        {
            if (fatherNode == null) return;
            Type type = parameter.GetType();
            PropertyInfo[] pis = type.GetProperties();
            foreach (var pi in pis)
            {
                XElement xElement = fatherNode.Element(pi.Name);
                if (xElement == null) continue;
                string value = xElement.Value;

                try
                {
                    switch (pi.PropertyType.ToString())
                    {
                        case "System.String":
                            pi.SetValue(parameter, value);
                            break;

                        case "System.Boolean":
                            pi.SetValue(parameter, value.Equals("1") ? true : false);
                            break;

                        case "System.Int32":
                            pi.SetValue(parameter, int.Parse(value));
                            break;

                        case "System.Nullable`1[System.Int32]":
                             pi.SetValue(parameter, int.Parse(value));
                             break;
                            
                        case "System.Double":
                            pi.SetValue(parameter, double.Parse(value));
                            break;

                        case "System.String[]":
                            string[] para_string = xElement.Descendants("para").Select(x => x.Value).ToArray();
                            pi.SetValue(parameter, para_string);
                            break;

                        case "System.Double[]":
                            double[] para_double = Array.ConvertAll(xElement.Descendants("para").Select(x => x.Value).ToArray(), new Converter<string, double>(Double.Parse));
                            pi.SetValue(parameter, para_double);
                            break;
                            //2020-10-21 add by wj
                        case "System.Int32[]":
                            int[] para_int = Array.ConvertAll(xElement.Descendants("para").Select(x => x.Value).ToArray(), new Converter<string, int>(Int32.Parse));
                            pi.SetValue(parameter, para_int);
                            break;
                        case "LFAOIRecipe.RegionType":
                            pi.SetValue(parameter, (RegionType)Enum.Parse(typeof(RegionType), value));
                            break;
                        case "LFAOIRecipe.RegionOperatType":
                            pi.SetValue(parameter, (RegionOperatType)Enum.Parse(typeof(RegionOperatType), value));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Value={value}");
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    continue;
                }
            }
        }

        public static void ReadRegion(XElement fatherNode,
                                      IList<UserRegion> userRegions, 
                                      string name, 
                                      double rowOffset = 0,
                                      double columnOffset = 0,
                                      bool isReadParameter = false,
                                      bool isReadAlgoParameter=false,
                                      bool isReadModelAlgoParameter = false,
                                      bool isReadRegAlgoParameter = false,//读取AroundBond检测参数
                                      bool IsReadBondAlgoParameter = false)//读取Bond检测参数
        {
            XElement node = fatherNode.Element(name);
            if (node == null) return;
            foreach (var element in node.Elements())
            {
                userRegions.Add(UserRegion.FromXElement(element, isReadParameter, rowOffset, columnOffset, isReadAlgoParameter, isReadModelAlgoParameter, isReadRegAlgoParameter, IsReadBondAlgoParameter));
            }
            return;
        }
        //add by wj 2020-10-22
        public static void ReadOnRecipes(XElement fatherNode, ObservableCollection<OnRecipe> onRecipes, string name)
        {
            XElement node = fatherNode.Element(name);
            if (node == null) return;
            foreach (var element in node.Elements())
            {
                onRecipes.Add(OnRecipe.FromXElement(element));
            }
            return;
        }

        public static void ReadRegionOne(XElement fatherNode,
                                      UserRegion userRegion,
                                      string name,
                                      double rowOffset = 0,
                                      double columnOffset = 0,
                                      bool isReadParameter = false)//
        {
            XElement node = fatherNode.Element(name);
            if (node == null) return;
            foreach (var element in node.Elements())
            {
                userRegion=(UserRegion.FromXElement(element, isReadParameter, rowOffset, columnOffset));
            }
            return;
        }


    }
}
