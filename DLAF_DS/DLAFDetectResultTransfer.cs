using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using DLAF;
using LFAOIReview;
using HalconDotNet;

namespace DLAF_DS
{
    /// <summary>
    /// 检测结果处理类
    /// 负责将输入的FovDetect转化为DieResult
    /// 
    /// </summary>
    public class DLAFDetectResultTransfer
    {


        public DLAFDetectResultTransfer()
        {

        }



        string _recipeID = null; //产品型号
        string _lotID = null;//批次号
        string _pieceID = null; //产品ID（条码号或时间戳）
        JFDLAFProductRecipe _recipe = null;



        /// <summary>
        /// 设置产品信号/批次号
        /// </summary>
        /// <param name="recipeID"></param>
        /// <param name="lotID"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetRecipeLot(string recipeID, string lotID, out string errorInfo)
        {
            if (string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "参数RecipeID为空字串";
                _recipeID = null;
                return false;
            }
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if (null == rm || !rm.IsInitOK)
            {
                errorInfo = "配方管理器未设置/未初始化！";
                _recipeID = null;
                return false;
            }
            string[] allRecipeIDS = rm.AllRecipeIDsInCategoty("Product");
            if (null == allRecipeIDS || !allRecipeIDS.Contains(recipeID))
            {
                errorInfo = "RecipeID:\"" + recipeID + "\"在配方管理器中不存在！";
                _recipeID = null;
                return false;
            }
            _recipeID = recipeID;
            _recipe = rm.GetRecipe("Product", _recipeID) as JFDLAFProductRecipe;


            if (string.IsNullOrEmpty(lotID))
            {
                _lotID = null;
                errorInfo = "参数项LotID为空值";
                return false;
            }

            _lotID = lotID;

            int fovCountInPiece = _recipe.FovCount * _recipe.ICCount;//料片所有Fov数量
            _fovResults = new DlafFovDetectResult[fovCountInPiece];
            _fovCounteds = new bool[fovCountInPiece];
            
            


            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 设置产品ID(用于向Reviewe数据库写记录) 
        /// (并清空已有的所有Fov检测结果)
        /// </summary>
        /// <param name="pieceID"></param>
        public void SetPieceID(string pieceID)
        {
            if (string.IsNullOrEmpty(pieceID))
                throw new Exception("DLAFDetectResultTransfer.SetPieceID(string pieceID) faield by pieceID is null or empty");
            _pieceID = pieceID;
            
            ClearFovResults();

        }



        /// <summary>
        /// 存储所有输入的Fov测试结果
        /// </summary>
        DlafFovDetectResult[] _fovResults = null;
        bool[] _fovCounteds = null; //对应的fovResult是否被统计过


        /// <summary>
        /// 获取所有已输入的Fov测试结果(可能有空值)
        /// </summary>
        public DlafFovDetectResult[] FovDetectResults { get { return _fovResults; } }

        /// <summary>
        /// 输入一条Fov的检测结果
        /// </summary>
        /// <param name="fovResult">Fov检测结果</param>
        public void EntryFovResult(DlafFovDetectResult fr/*, out bool isLastFovInPiece*/)
        {
            if (null == fr)
                return;
            int fovIndexInDie = 0;
            string[] allFovNames = _recipe.FovNames();
            for(int i = 0;i< allFovNames.Length;i++)
            {
                if(fr.FovName == allFovNames[i])
                {
                    fovIndexInDie = i;
                    break;
                }
            }

            int idx = (fr.ICRow * _recipe.ColCount + fr.ICCol)*_recipe.FovCount + fovIndexInDie;
            _fovResults[idx] = fr;
            _fovCounteds[idx] = false;

        }

        /// <summary>
        /// 清空所有保存的Fov检测结果
        /// </summary>
        public void ClearFovResults()
        {
            if (null == _fovResults)
                return;
            for(int i = 0; i < _fovResults.Length;i++)
            {
                _fovResults[i] = null;
                _fovCounteds[i] = false;
            }

        }

        /// <summary>
        /// 是否已收到所有Fov检测结果
        /// </summary>
        public bool AllFovRecieved
        {
            get 
            {
                if (null == _recipe)
                    return false;
                foreach (DlafFovDetectResult fr in _fovResults)
                    if (null == fr)
                        return false;
                return true;
            }
        }

        //当前料片Die结果是否全部被提取
        public bool AllFovDischarged
        {
            get 
            {
                if (null == _fovResults)
                    return true;
                foreach (bool b in _fovCounteds)
                    if (!b)
                        return false;
                return true;
            }
        }

       
        int  _ImgCount()
        {
            if (null == _recipe)
                return -1;
            int count = 0;
            string[] fovNames = _recipe.FovNames();
            foreach (string fov in fovNames)
                count += _recipe.TaskNames(fov).Length;
            return count;

        }

        /// <summary>
        /// 用于计算指定的task在平铺后数组中的序号 , 最小值为1 for halcon
        /// </summary>
        /// <param name="fovName"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        int _ImgIndex(string fovName,string taskName)
        {
            if (_recipe == null)
                return 0;
            int preTaskCount = 0;
            string[] allFovNames = _recipe.FovNames();
            foreach (string fov in allFovNames)
            {
                if (fov == fovName)
                    break;
                else
                    preTaskCount += _recipe.TaskNames(fov).Length;
            }

            string[] taskNames = _recipe.TaskNames(fovName);
            foreach(string task in taskNames)
            {
                if (task == taskName)
                    break;
                else
                    preTaskCount++;
            }
            return preTaskCount+1;

        }

        /// <summary>
        /// 尝试将现有队列中的Fov结果合并为Die结果，并输出
        /// （已经被计算合并过的FovResult将不再参与计算）
        /// </summary>
        /// <returns></returns>
        public InspectionData[] DisChargeDieResults()
        {
            if (null == _fovResults)
                return null;
            List<InspectionData> ret = new List<InspectionData>();
           
            bool isSmallDie = _recipe.DieRowInFov* _recipe.DieColInFov > 0;//一个Fov中是否含有多个Die
            if (isSmallDie) //小Die
            {
                for (int i = 0; i < _fovResults.Length; i++)
                {
                    DlafFovDetectResult fovResult = _fovResults[i];
                    if (null == fovResult) //fov检测结果还没有输入
                        continue;
                    if (_fovCounteds[i]) //fov检测结果已经被统计过
                        continue;
                    for (int j = 0; j < fovResult.DetectDiesRows.Length; j++)
                    {
                        int dieRow = _recipe.DieRowInFov * fovResult.ICRow + fovResult.DetectDiesRows[j]; //料片中的die行列值
                        int dieCol = _recipe.DieColInFov * fovResult.ICCol + fovResult.DetectDiesCols[j];
                        InspectionData inspData = new InspectionData();
                        inspData.RecipeID = _recipeID;
                        inspData.LotID = _lotID;
                        inspData.FrameID = _pieceID;
                        inspData.FovNames = _recipe.FovNames();
                        inspData.TaskNamesInFovs = new string[inspData.FovNames.Length][];
                        for (int h = 0; h < inspData.FovNames.Length; h++)
                            inspData.TaskNamesInFovs[h] = _recipe.TaskNames(inspData.FovNames[h]);
                        inspData.Code2D = null;
                        inspData.ColumnIndex = dieCol;
                        inspData.RowIndex = dieRow;
   
                        if (null == fovResult.DetectDiesImages) //可能无图，（作为工站/界面交互数据）
                            inspData.Image = null;
                        else
                            inspData.Image = fovResult.DetectDiesImages[j].Clone();
                        if (!fovResult.IsDetectSuccess) //图象检测失败
                        {
                            inspData.InspectionResult = InspectionResults.NG;
                        }
                        else //图象检测成功
                        {
                            //金线区域
                            inspData.Wire = null;
                            if (null != fovResult.WireRegion)
                            {
                                HObject wireRegion = fovResult.WireRegion;//fovResult.DetectIterms["WireRegions"];
                                string wrieTaskName = fovResult.WireRegionTaskName;//fovResult.DetectItemTaskNames["WireRegions"];
                                int taskIdx = 0;
                                for (int h = 0; h < inspData.TaskNamesInFovs[0].Length; h++)
                                    if (inspData.TaskNamesInFovs[0][h] == wrieTaskName)
                                    {
                                        taskIdx = h;
                                        break;
                                    }
                                HObject emptyRegion;
                                HOperatorSet.GenEmptyRegion(out emptyRegion);
                                HObject hoConcatReg;
                                if (taskIdx == 0)
                                {
                                    hoConcatReg = wireRegion.Clone();
                                    for (int h = 1; h < inspData.TaskNamesInFovs[0].Length; h++)
                                        HOperatorSet.ConcatObj(hoConcatReg, emptyRegion, out hoConcatReg);
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyRegion(out hoConcatReg);
                                    for (int h = 1; h < inspData.TaskNamesInFovs[0].Length; h++)
                                    {
                                        if (taskIdx == h)
                                            HOperatorSet.ConcatObj(hoConcatReg, wireRegion.Clone(), out hoConcatReg);
                                        else
                                            HOperatorSet.ConcatObj(hoConcatReg, emptyRegion, out hoConcatReg);
                                    }
                                }

                                inspData.Wire = hoConcatReg;
                            }

                            inspData.Region = null; //保存所有出错的Region , 一维长度和序号
                            if (fovResult.DiesErrorRegions != null && 
                                fovResult.DiesErrorRegions[j] != null&&
                                fovResult.DiesErrorRegions[j].Length > 0)
                            {
                                HObject ho = fovResult.DiesErrorRegions[j][0].Clone();

                                for (int h = 1; h < fovResult.DiesErrorRegions[j].Length; h++)
                                    HOperatorSet.ConcatObj(ho, fovResult.DiesErrorRegions[j][h], out ho);
                                inspData.Region = ho;

                            }

                            int[] errorCodes = fovResult.DiesErrorCodes[j];
                            if (errorCodes.Length == 1 && errorCodes[0] == 0) //检测合格
                            {
                                inspData.InspectionResult = InspectionResults.OK;
                                inspData.List_DefectData = null;
                            }
                            else
                            {
                                inspData.InspectionResult = InspectionResults.NG;
                                inspData.List_DefectData = new List<DefectData>();
                                for (int h = 0; h < fovResult.DiesErrorCodes.Length; h++)
                                {
                                    DefectData dd = new DefectData();
                                    dd.DefectTypeIndex = errorCodes[h];
                                    dd.ErrorDetail = fovResult.DiesErrorDetails[j][h];
                                    dd.ImageIndex = _ImgIndex(fovResult.FovName, fovResult.DiesErrorTaskNames[j][h]);

                                    inspData.List_DefectData.Add(dd);
                                }

                            }
                            
                            inspData.SuccessRegion = null; //所有检测成功区域
                            inspData.SuccessRegionTypes = null;
                            inspData.SuccessRegionImageIndex = null;
                            if(null != fovResult.DetectIterms && fovResult.DetectIterms.Count > 0)  //存在除了金线之外的其他检测项
                            {
                                List<HObject> itemRegs = new List<HObject>();
                                List<string> itemTyps = new List<string>();
                                List<int> itemImgIdx = new List<int>();
                                foreach(string itemName in fovResult.DetectIterms.Keys)
                                {
                                    itemRegs.Add(fovResult.DetectIterms[itemName]);
                                    itemTyps.Add(itemName);
                                    string tskName = fovResult.DetectItemTaskNames[itemName];
                                    for (int k = 0; k < fovResult.TaskNames[0].Length; k++)
                                        if (tskName == fovResult.TaskNames[k])
                                        {
                                            itemImgIdx.Add(k);
                                            break;
                                        }
                                }

                                if (itemRegs.Count() == 1)
                                    inspData.SuccessRegion = itemRegs[0];
                                else
                                {
                                    HObject hoReg = itemRegs[0].Clone();
                                    for (int h = 1; h < itemRegs.Count(); h++)
                                        HOperatorSet.ConcatObj(hoReg, itemRegs[h], out hoReg);
                                    inspData.SuccessRegion = hoReg;
                                }
                                inspData.SuccessRegionTypes = itemTyps.ToArray();
                                inspData.SuccessRegionImageIndex = itemImgIdx.ToArray();
                            }
                            if (null == inspData.List_DefectData)
                                inspData.List_DefectData = new List<DefectData>();
                            ret.Add(inspData);
                        }
                        _fovCounteds[i] = true;
                    }
                }
            }
            else //大Die,一颗Die包含多个Fov
            {
                int fovCountInDie = _recipe.FovCount;
                for (int i = 0; i < _fovResults.Length;i+= fovCountInDie)
                {
                    bool isFovsCan2Die = true; //多个Fov是否能集成为一个Die
                    for(int j = 0; j < fovCountInDie;j++)
                        if(null == _fovResults[i+j] || _fovCounteds[i])
                        {
                            isFovsCan2Die = false;
                            break;
                        }
                    if (!isFovsCan2Die)
                        continue;

                    
                    InspectionData inspData = new InspectionData();
                    inspData.RecipeID = _recipeID;
                    inspData.LotID = _lotID;
                    inspData.FrameID = _pieceID;
                    inspData.FovNames = _recipe.FovNames();
                    inspData.TaskNamesInFovs = new string[inspData.FovNames.Length][];
                    for (int h = 0; h < inspData.FovNames.Length; h++)
                        inspData.TaskNamesInFovs[h] = _recipe.TaskNames(inspData.FovNames[h]);
                    inspData.Code2D = null;
                    inspData.ColumnIndex = _fovResults[i].ICCol;
                    inspData.RowIndex = _fovResults[i].ICRow;

                    HObject hoImg = null; ////将Die所属的所有多通道的图平铺成一维数组
                    HObject hoWire = null;
                    HObject hoErrorRegions = null;
                    bool isDieOK = true; //整颗die的检测结果是否OK
                    List<DefectData> lstDefectData = new List<DefectData>();
                    HObject hoSuccessRegion = null;
                    List<string> lstSuccessRegionTypes = new List<string>();
                    List<int> lstSuccessImgIndex = new List<int>();
                    for (int j = i; j < i+fovCountInDie; j++) //将多个Fov合为一个Die
                    {
                        DlafFovDetectResult fovRet = _fovResults[j];
                        
                        HObject hoTaskImgs = fovRet.DetectDiesImages[0]; //每个Fov中只有一个Die（的一部分） 
                        if (null == hoImg) //第一个
                            hoImg = hoTaskImgs.Clone();
                        else
                            for (int k = 0; k < hoTaskImgs.CountObj(); k++)
                                HOperatorSet.ConcatObj(hoImg,hoTaskImgs.CopyObj(k + 1, 1),out hoImg);

                        HObject hoRegion = fovRet.WireRegion;//将所有Fov中的WireRegion平铺成一维数组
                        if (null == hoRegion)
                            HOperatorSet.GenEmptyRegion(out hoRegion);
                        if(null == hoWire)
                        {
                            hoWire = hoRegion.Clone();
                            for (int k = 1; k < fovRet.TaskNames.Length; k++)
                                HOperatorSet.ConcatObj(hoWire, hoRegion.Clone(), out hoWire);
                        }
                        else
                            for (int k = 0; k < fovRet.TaskNames.Length; k++)
                                HOperatorSet.ConcatObj(hoWire, hoRegion.Clone(), out hoWire);

                        if (!fovRet.IsDetectSuccess)//视觉检测失败
                        {

                        }
                        else
                        {
                            ///错误区域
                            if (fovRet.DiesErrorCodes[0].Length != 1 || fovRet.DiesErrorCodes[0][0] == 0)
                            {
                                isDieOK = false;
                                DefectData df = new DefectData();
                                df.DefectTypeIndex = fovRet.DiesErrorCodes[0][0];
                                df.ErrorDetail = fovRet.DiesErrorDetails[0][0];
                                df.ImageIndex = _ImgIndex(fovRet.FovName, fovRet.DiesErrorTaskNames[0][0]);
                                lstDefectData.Add(df);
                                if (null == hoErrorRegions)
                                    hoErrorRegions = fovRet.DiesErrorRegions[0][0].Clone();
                                else
                                    HOperatorSet.ConcatObj(hoErrorRegions, fovRet.DiesErrorRegions[0][0], out hoErrorRegions);
                                for(int k = 1; k < fovRet.DiesErrorCodes[0].Length;k++)
                                {
                                    df = new DefectData();
                                    df.DefectTypeIndex = fovRet.DiesErrorCodes[0][k];
                                    df.ErrorDetail = fovRet.DiesErrorDetails[0][k];
                                    df.ImageIndex = _ImgIndex(fovRet.FovName, fovRet.DiesErrorTaskNames[0][k]);
                                    lstDefectData.Add(df);
                                    HOperatorSet.ConcatObj(hoErrorRegions, fovRet.DiesErrorRegions[0][k], out hoErrorRegions);
                                }
                            }


                            /*public HObject*/
                            if(fovRet.DetectIterms != null && fovRet.DetectIterms.Count > 0)
                                foreach(KeyValuePair<string,HObject> kv in fovRet.DetectIterms)
                                {
                                    if (null == hoSuccessRegion)
                                        hoSuccessRegion = kv.Value.Clone();
                                    else
                                        HOperatorSet.ConcatObj(hoSuccessRegion, kv.Value, out hoSuccessRegion);
                                    lstSuccessRegionTypes.Add(kv.Key);
                                    lstSuccessImgIndex.Add(_ImgIndex(fovRet.FovName, kv.Key));
                                }
                           
                        }
                        _fovCounteds[j] = true;
                    }
                    inspData.Image = hoImg;
                    inspData.Wire = hoWire;
                    inspData.Region = hoErrorRegions;
                    if (isDieOK)
                    {
                        inspData.InspectionResult = InspectionResults.OK;
                        inspData.List_DefectData = null;
                    }
                    else
                    {
                        inspData.InspectionResult = InspectionResults.NG;
                        inspData.List_DefectData = lstDefectData;
                    }

                    if(null != hoSuccessRegion)
                    {
                        inspData.SuccessRegion = hoSuccessRegion; //所有检测成功区域
                        inspData.SuccessRegionTypes = lstSuccessRegionTypes.ToArray();
                        inspData.SuccessRegionImageIndex = lstSuccessImgIndex.ToArray();
                    }
                    if (null == inspData.List_DefectData)
                        inspData.List_DefectData = new List<DefectData>();
                    ret.Add(inspData);
                }
            }


            if(ret.Count() == 0 )
                return null;
            return ret.ToArray();
        }

        /// <summary>
        /// 获取当前已存储的所有Fov转化成的DieResults
        /// </summary>
        /// <returns></returns>
        public InspectionData[]  OverAllDieResults()
        {
            return null;
        }





    }
}
