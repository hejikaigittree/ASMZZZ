using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using JFToolKits;

namespace JFRecipe
{
    /// <summary>
    /// 通用的配置管理类
    /// </summary>
    [JFDisplayName("JF通用配方管理器")]
    [JFVersion("1.0.0.0")]
    public class JFCommonRecipeManager:IJFRecipeManager,IJFRealtimeUIProvider
    {
        public JFCommonRecipeManager()
        {
            _ui.SetManager(this);
        }

        JFXCfg _cfg = new JFXCfg(); //用于读取/保存配置的对象
        Dictionary<string, Dictionary<string, JFCommonRecipe>> _dctRecipes = new Dictionary<string, Dictionary<string, JFCommonRecipe>>(); 



        #region IJFInitializable's API
        static string[] _initParamNames = new string[] { "配置文件路径","文件不存在时" };
        string[] _initParamValues = new string[] { "", "" };
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { return _initParamNames; } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == _initParamNames[0])
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.FilePath, null);
            else if(name == _initParamNames[1])
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, new string[] { "新创建","报错"});
            
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (name == _initParamNames[0])
                return _initParamValues[0];
            else if(name == _initParamNames[1])
                return _initParamValues[1];
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            if (name == _initParamNames[0])
            {
                _initParamValues[0] = Convert.ToString(value);
                return true;
            }
            else if(name == _initParamNames[1])
            {
                string sv = Convert.ToString(value);
                if (sv == "新创建" || sv =="报错")
                {
                    _initParamValues[1] = sv;
                    return true;
                }
                return false;
            }

            throw new ArgumentOutOfRangeException();
        }


        bool _isInitOK = false;
        string _initErrorInfo = "None-Opt";
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            _isInitOK = false;
            _initErrorInfo = "Unknown-Error";
            do
            {
                if (string.IsNullOrEmpty(_initParamValues[0]))
                {
                    _initErrorInfo = _initParamNames[0] + " 未设置/空值";
                    break;
                }



                if (string.IsNullOrEmpty(_initParamValues[1]))
                {
                    _initErrorInfo = _initParamNames[1] + " 未设置/空值";
                    break;
                }

                bool isCreateWhenFileNotExist = false;
                if (_initParamValues[1] == "新创建")
                    isCreateWhenFileNotExist = true;
                else if(_initParamValues[1] == " 报错")
                    isCreateWhenFileNotExist = false;
                else
                {
                    _initErrorInfo = _initParamNames[1] + " 参数错误,Value = " + _initParamValues[1] + "不存在于可选值列表[\"新创建\",\"报错\"]";
                    break;
                }

                if(!File.Exists(_initParamValues[0]))
                {
                    if(!isCreateWhenFileNotExist)
                    {
                        _initErrorInfo = _initParamNames[0] + " = \"" + _initParamValues[0] + "\"文件不存在";
                        break;
                    }
                }

                try
                {
                    _cfg.Load(_initParamValues[0], isCreateWhenFileNotExist);

                    
                    if (!_cfg.ContainsItem("Categoties"))///保存所有的产品类别()
                        _cfg.AddItem("Categoties", new List<string>());

                    if(!_cfg.ContainsItem("Cate-Recipes")) ///
                        _cfg.AddItem("Cate-Recipes", new JFXmlDictionary<string, List<string[]>>());
                    //.............................................  类别->Recipe[ID, innerTxt]      
                    string errInfo;
                    if(!_load(out errInfo))
                    {
                        _initErrorInfo = "加载配置文件出错:" + errInfo;
                        break;
                    }

                    JFXmlDictionary<string, List<string[]>> dctCateRecipes = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
                    

                }
                catch(Exception ex)
                {
                    _initErrorInfo = "加载配置文件发生异常:" + ex.Message;
                    break;
                }


                _isInitOK = true;
                _initErrorInfo = "Success";
            } while (false);
            return _isInitOK;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get { return _isInitOK; } }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }
        #endregion


        /// <summary>
        /// 将配置数据转化为内存对象
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        bool _load(out string errorInfo)
        {
            _dctRecipes.Clear();
            JFXmlDictionary<string, List<string[]>> cateRecipeInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            foreach (KeyValuePair<string, List<string[]>> cr in cateRecipeInCfg)
            {
                if (!_dctRecipes.ContainsKey(cr.Key))
                    _dctRecipes.Add(cr.Key, new Dictionary<string, JFCommonRecipe>());
                Dictionary<string, JFCommonRecipe> dctInCate = _dctRecipes[cr.Key];
                foreach (string[] idAndTxt in cr.Value)
                {
                    JFCommonRecipe recipe = new JFCommonRecipe();
                    recipe.Categoty = cr.Key;
                    recipe.ID = idAndTxt[0];
                    try
                    {
                        recipe.Dict = JFFunctions.FromXTString(idAndTxt[1], recipe.Dict.GetType()) as JFXmlDictionary<string, object>;
                    }
                    catch(Exception ex)
                    {
                        errorInfo = "Categoty = " + cr.Key + ", RecipeID = " + idAndTxt[0] + " FromXTString() Exception:" + ex.Message;
                        return false;
                    }
                    
                    dctInCate.Add(idAndTxt[0], recipe);
                }

            }
            errorInfo = "Success";
            return true;


        }


        #region   IJFRecipeManager's API
        /// <summary>
        /// 载入(/重新载入)所有产品,配方
        /// </summary>
        public bool Load()
        {
            if (!IsInitOK)
                return false;

            _cfg.Load();
            string error;
            if (!_load(out error))
                return false;
            return true;

        }

        public void Save()
        {
            if (!IsInitOK)
                return;
            JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;

            foreach (string categoty in _dctRecipes.Keys)
            {
                 Dictionary<string, JFCommonRecipe> dctMmry = _dctRecipes[categoty]; //所有recipe内存对象
                List<string[]> lstInCfg = dctRecipesInCfg[categoty];
                foreach(KeyValuePair<string, JFCommonRecipe> kv in dctMmry)
                {
                    foreach(string[] idAndTxt in lstInCfg)
                        if(idAndTxt[0] == kv.Key)
                        {
                            string xmlTxt = null;
                            string typeTxt = null;
                            JFFunctions.ToXTString(kv.Value.Dict, out xmlTxt, out typeTxt);
                            idAndTxt[1] = xmlTxt;
                            break;
                        }
                }

            }
            _cfg.Save();
        }

        /// <summary>
        /// 获取所有产品/配方 类别
        /// </summary>
        /// <returns></returns>
        public string[] AllCategoties()
        {
            if (!IsInitOK)
                return null;
          

            return (_cfg.GetItemValue("Categoties") as List<string>).ToArray();

        }

        ///// <summary>
        ///// 添加一个产品类别
        ///// </summary>
        ///// <param name="recipeCategoty"></param>
        //void AddRecipeCategoty(string categoty);

        /// <summary>
        /// 移除一个类别(下的所有RecipeID)
        /// </summary>
        /// <param name="recipeCategoty"></param>
        public void RemoveCategoty(string categoty)
        {
            if (!_cfg.ContainsItem("Categoties"))
                return;
            //移除配置参数
            List<string> categoties = _cfg.GetItemValue("Categoties") as List<string>;
            if (!categoties.Contains(categoty))
                return;
            categoties.Remove(categoty);
            JFXmlDictionary<string, List<string[]>> recipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            if (recipesInCfg.ContainsKey(categoty))
                recipesInCfg.Remove(categoty);

            //移除内存对象
            if (_dctRecipes.ContainsKey(categoty))
                _dctRecipes.Remove(categoty);


        }



        /// <summary>
        /// 获取指定类别下的所有产品/配方 ID
        /// </summary>
        /// <returns></returns>
        public string[] AllRecipeIDsInCategoty(string categoty)
        {
            JFXmlDictionary<string, List<string[]>> cateRecipes = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            if (!cateRecipes.ContainsKey(categoty))
                return null;
            List<string[]> recipes = cateRecipes[categoty];
            List<string> ret = new List<string>();
            foreach (string[] sa in recipes)
                ret.Add(sa[0]);
            return ret.ToArray();
        }

        /// <summary>
        /// 获取一个产品/配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        public IJFRecipe GetRecipe(string categoty, string recipeID)
        {
            if (!_dctRecipes.ContainsKey(categoty))
                return null;
            Dictionary<string, JFCommonRecipe> dct = _dctRecipes[categoty];
            if (!dct.ContainsKey(recipeID))
                return null;
            return dct[recipeID];
        }

        /// <summary>
        /// 添加一个产品/配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <param name="recipe"></param>
        public bool AddRecipe(string categoty, string recipeID,IJFRecipe recipe = null)
        {
            if (string.IsNullOrEmpty(categoty))
                return false;
            if (string.IsNullOrEmpty(recipeID))
                return false;
            if (recipe != null && recipe.GetType() != typeof(JFCommonRecipe))
                return false;
            if (GetRecipe(categoty, recipeID) != null) //已已存在同名Recipe
                return false;


            JFCommonRecipe cmRecipe = recipe as JFCommonRecipe;
            if (null == cmRecipe)
                cmRecipe = new JFCommonRecipe();
            cmRecipe.ID = recipeID;
            cmRecipe.Categoty = categoty;

            List<string> lstCatesInCfg = _cfg.GetItemValue("Categoties") as List<string>;
            if (!lstCatesInCfg.Contains(categoty))
                lstCatesInCfg.Add(categoty);

            JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            if (!dctRecipesInCfg.ContainsKey(categoty))
                dctRecipesInCfg.Add(categoty, new List<string[]>());
            List<string[]> lstIDAndTxt = dctRecipesInCfg[categoty];
            lstIDAndTxt.Add(new string[] { recipeID, cmRecipe.Dict.ToString() });

            if (!_dctRecipes.ContainsKey(categoty))
                _dctRecipes.Add(categoty, new Dictionary<string, JFCommonRecipe>());
            Dictionary<string, JFCommonRecipe> dctInMmry = _dctRecipes[categoty];
            dctInMmry.Add(recipeID, cmRecipe);
            return true;
            
        }

        /// <summary>
        /// 移出一个产品配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        public IJFRecipe RemoveRecipe(string categoty, string recipeID)
        {
            if (string.IsNullOrEmpty(categoty))
                return null;
            if (string.IsNullOrEmpty(recipeID))
                return null;

            IJFRecipe ret = GetRecipe(categoty, recipeID);
            if (ret == null) //已已存在同名Recipe
                return ret;

            Dictionary<string, JFCommonRecipe> dctInMmry = _dctRecipes[categoty];
            dctInMmry.Remove(recipeID);
            if (dctInMmry.Count == 0)
                _dctRecipes.Remove(categoty);


            JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            List<string[]> lstIDAndTxt = dctRecipesInCfg[categoty];
            for(int i = 0; i < lstIDAndTxt.Count; i ++)
                if(lstIDAndTxt[i][0] == recipeID)
                {
                    lstIDAndTxt.RemoveAt(i);
                    break;
                }

            if(lstIDAndTxt.Count == 0 )
            {
                dctRecipesInCfg.Remove(categoty);
                List<string> lstCatesInCfg = _cfg.GetItemValue("Categoties") as List<string>;
                lstCatesInCfg.Remove(categoty);
            }
            return ret;
           
        }

        public void Dispose()
        {
            return;
        }


        JFUcCmRecipeMgrRT _ui = new JFUcCmRecipeMgrRT();
        public JFRealtimeUI GetRealtimeUI()
        {
            //throw new NotImplementedException();
            return _ui;

        }

        #endregion
    }
}
