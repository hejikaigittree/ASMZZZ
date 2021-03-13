using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HTHalControl;
using HalconDotNet;

namespace JFRecipe
{
    public class JFDLAFRecipe
    {
        private HTuple snapMapX;
        private HTuple snapMapY;
        private HTuple snapMapRow;
        private HTuple snapMapCol;

        private HTuple icMapX;
        private HTuple icMapY;

        private HTuple markPos1X;
        private HTuple markPos1Y;

        private HTuple markPos2X;
        private HTuple markPos2Y;

        private HTuple mark1MapX;
        private HTuple mark1MapY;

        private HTuple mark2MapX;
        private HTuple mark2MapY;

        private HObject Image;

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错//
        }

        public JFDLAFRecipe(string _recipeFolderPath,string _recipeID)
        {
            RecipeFolderPath = _recipeFolderPath;
            RecipeID = _recipeID;
        }

        public string RecipeID { get; }
        public string RecipeFolderPath { get; }

        public int ICCount { get; private set;}

        public void GetMarkPos1(out double x, out double y)
        {
            x = 0;
            y = 0;
            x = markPos1X.TupleSelect(0).D;
            y = markPos1Y.TupleSelect(0).D;
        }

        public void GetMarkPos2(out double x, out double y)
        {
            x = 0;
            y = 0;
            x = markPos2X.TupleSelect(0).D;
            y = markPos2Y.TupleSelect(0).D;
        }

        public void GetICCenter(int index, out double r, out double c,out double x, out double y)
        {
            r = 0;
            c = 0;
            x = 0;
            y = 0;
            r = snapMapRow.TupleSelect(index).D;
            c = snapMapCol.TupleSelect(index).D;
            x = snapMapX.TupleSelect(index).D;
            y = snapMapY.TupleSelect(index).D;
        }

        public int FovCount { get; private set; }


        public void GetFovOffset(int index, out double x, out double y)
        {
            x = 0;
            y = 0;
            x = icMapX.TupleSelect(index).D;
            y = icMapY.TupleSelect(index).D;
        }

        /// <summary>
        /// 加载Recipe中IC的扫描点位
        /// </summary>
        /// <param name="errInf"></param>
        /// <returns></returns>
        public int LoadRecipeScanPositonFile(out string errInf)
        {
            errInf = "";
            try
            {
                //加载标定点模板
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\markLMapX.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\markLMapX.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\markLMapY.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\markLMapY.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\markRMapX.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\markRMapX.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\markRMapY.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\markRMapY.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\markLMapX.dat", out mark1MapX);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\markLMapY.dat", out mark1MapY);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\markRMapX.dat", out mark2MapX);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\markRMapY.dat", out mark2MapY);

                if (mark1MapX.Length != mark1MapY.Length || mark2MapX.Length != mark2MapY.Length || mark1MapX.Length != mark2MapX.Length || mark1MapX.Length != 1)
                {
                    errInf = "markLMapX.dat、markLMapY.dat、markRMapX.dat、markRMapY.dat中点位数目不匹配";
                    return (int)ErrorDef.InvokeFailed;
                }

                //加载IC扫描点文件
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\snapMapX.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\snapMapX.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\snapMapY.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\snapMapY.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\snapMapRow.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\snapMapRow.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\snapMapCol.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\snapMapCol.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\snapMapX.dat", out snapMapX);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\snapMapY.dat", out snapMapY);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\snapMapRow.dat", out snapMapRow);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\snapMapCol.dat", out snapMapCol);

                if (snapMapX.Length != snapMapY.Length || snapMapRow.Length != snapMapCol.Length || snapMapX.Length != snapMapCol.Length)
                {
                    errInf = "文件snapMapX.dat、snapMapY.dat、snapMapRow.dat、snapMapCol.dat中点位数目不匹配";
                    return (int)ErrorDef.InvokeFailed;
                }
                ICCount = snapMapX.Length;


                //加载Region文件
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\icMapX.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\icMapX.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + RecipeID + "\\icMapY.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + RecipeID + "\\icMapY.dat" + "不存在";
                    return (int)ErrorDef.InvokeFailed;
                }

                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\icMapX.dat", out icMapX);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + RecipeID + "\\icMapY.dat", out icMapY);
                if (icMapX.Length != icMapY.Length)
                {
                    errInf = "文件icMapX.dat、icMapY.dat中点位数目不匹配";
                    return (int)ErrorDef.InvokeFailed;
                }
                FovCount = icMapX.Length;
                return (int)ErrorDef.Success;
            }
            catch(Exception ex)
            {
                errInf = ex.ToString();
                return (int)ErrorDef.InvokeFailed;
            }
        }
    }


    /// <summary>
    /// 配方管理类
    /// </summary>
    public class JFDLAFRecipeManager
    {
        private string _recipeFolderPath = "";

        public JFDLAFRecipeManager(string recipeFolderPath)
        {
            _recipeFolderPath = recipeFolderPath;
        }


        public string[] AllRecipeIDs()
        {
            List<String> productlist = GetProductList();
            return productlist.ToArray();
        }

        public JFDLAFRecipe GetRecipe(string recipeID,out string errInf)
        {
            errInf = "";
            JFDLAFRecipe jfDLAFRecipe = new JFDLAFRecipe(_recipeFolderPath, recipeID);

            if(jfDLAFRecipe.LoadRecipeScanPositonFile(out errInf)!=0)
            {
                return null;
            }
            return jfDLAFRecipe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>包含目录的指针list</returns>
        private List<string> GetProductList()
        {
            List<string> list = new List<string>();
            DirectoryInfo Path = new DirectoryInfo(_recipeFolderPath);
            DirectoryInfo[] Dir = Path.GetDirectories();
            foreach (DirectoryInfo d in Dir)
            {
                list.Add(d.Name);
            }
            //read product directory to fetch the list
            return list;
        }
    }
}
