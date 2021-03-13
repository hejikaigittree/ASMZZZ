using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFUI;
using JFHub;
using JFInterfaceDef;
using HalconDotNet;
using LFAOIRecipe;
using LFAOIReview;

namespace DLAF
{
    /// <summary>
    /// DLAF项目主工站所使用的实时界面
    /// </summary>
    public partial class UcDLAFMainStation : UserControl
    {
        public UcDLAFMainStation()
        {
            InitializeComponent();

            _dctDieStateColor.Add("未检测", Color.SkyBlue);
            _dctDieStateColor.Add("合格", Color.Green);
            _dctDieStateColor.Add("不合格", Color.Red);
            ////dic_DieState_Color.Add("复看合格", Color.Yellow);
            //dic_DieState_Color.Add("无芯片", Color.Purple); 
            _dctDieStateColor.Add("匹配失败", Color.Pink);
        }


        MainStation _ms = null;

        //string _currRecipeID = null;
        JFDLAFProductRecipe _currRecipe = null;

        // List<string> _lstTaskImgShows = new List<string>();//需要显示的Task图像通道
        string _taskImgShow = null;
        public void SetMainStation(MainStation ms)
        {
            _ms = ms;
            if (Created)
                AdjustStationView();
        }

        //更新界面
        void AdjustStationView()
        {
            if(_ms == null)
            {
                lbProdID.Text = "MainStation is not Set";
                Enabled = false;
                return;
            }
            UpdateMappingByProdID(_ms.CurrRecipeID);

        }

        /// <summary>
        /// 根据当前所选ProductID更新Map控件
        /// </summary>
        /// <param name="recipeID"></param>
        void UpdateMappingByProdID(string recipeID)
        {
            if(string.IsNullOrEmpty(recipeID))
            {
                mappCtrl.Initial(0, 0, _dctDieStateColor, "未检测");
                mappCtrl.IsShowBottomBar = true;
                return;
            }
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            _currRecipe = rm.GetRecipe(MainStation.SCN_CategotyProd, recipeID) as JFDLAFProductRecipe;
            if(null == _currRecipe)
            {
                mappCtrl.Initial(0, 0, _dctDieStateColor, "未检测");
                mappCtrl.IsShowBottomBar = true;

                return;
            }

  
                int dieRow = _currRecipe.RowNumber; ; //料片中的die行数
                int dieCol = _currRecipe.ColumnNumber * _currRecipe.BlockNumber; ;


                mappCtrl.Initial(dieRow, dieCol, _dctDieStateColor, "未检测");
                mappCtrl.MinCellHeight = 5;
                mappCtrl.MinCellWidth = 5;
                mappCtrl.IsShowBottomBar = true;
            


            cbTaskImgShow.Items.Clear();
            List<string> allTaskNames = new List<string>();
            string[] allFovNames = _currRecipe.FovNames();
            if (null != allFovNames)
            {
                List<string> taskNamesExisted = new List<string>();
                foreach (string fovName in allFovNames)
                {
                    string[] taskNames = _currRecipe.TaskNames(fovName);
                    if (taskNames != null)
                        foreach (string taskName in taskNames)
                            if (!taskNamesExisted.Contains(taskName))
                            {
                                taskNamesExisted.Add(taskName);
                                cbTaskImgShow.Items.Add(taskName);
                            }
                }
            }

            if (cbTaskImgShow.Items.Count > 0)
                cbTaskImgShow.SelectedIndex = 0;

            



        }


        Dictionary<string, Color> _dctDieStateColor = new Dictionary<string, Color>();
        private void UcDLAFMainStation_Load(object sender, EventArgs e)
        {
            htWindowControl1.RefreshWindow(null, null, "fit");
            fmTips.Show();
            fmTips.Hide();

            mappCtrl.OnSelectedDieChanged += MappCtrl_OnSelectedDieChanged;

            AdjustStationView();
        }

        private void MappCtrl_OnSelectedDieChanged(HTMappingControl.DieInfo dieInfo)
        {
            if(dieInfo.DieState == "未检测")
            {
                rchDieInfo.Text = "未检测";
                return;
            }
            else if(dieInfo.DieState == "合格")
            {
                rchDieInfo.Text = "合格";
                return;
            }

            foreach(InspectionData inspData in _lstInspectData)
                if(inspData.RowIndex == dieInfo.RowIndex && inspData.ColumnIndex == dieInfo.ColumnIndex)
                {
                    //将检测结果显示在Die信息栏
                    rchDieInfo.Text = "不合格 NG信息:";
                    for(int i = 0; i < inspData.List_DefectData.Count();i++)
                    {
                        
                        DefectData dd = inspData.List_DefectData[i];
                        rchDieInfo.AppendText("Err No:" + (i + 1) +/* " Code:" + dd.DefectTypeIndex + */"Describ:" + InspectNode.DieErrorDescript(dd.DefectTypeIndex) + "Detail:" + dd.ErrorDetail + "\n");
                    }
                }

        }

        /// <summary>
        /// 切换当前产品ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetProdID_Click(object sender, EventArgs e)
        {
            FormObtainBarcode dlg = new FormObtainBarcode();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.Text = "输入产品ID";
            string lastProdID = _ms.CurrRecipeID;
            if(string.IsNullOrEmpty(lastProdID))
                lastProdID = JFHubCenter.Instance.SystemCfg.GetItemValue(MainStation.SCN_CurrentRecipeID) as string;
            dlg.SetInitBarcode(lastProdID);
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if(null != rm && rm.IsInitOK)
            {
                string[] allRecipeIDs = rm.AllRecipeIDsInCategoty(MainStation.SCN_CategotyProd);
                dlg.SetBarcodeOptions(allRecipeIDs);
            }

            dlg.ObtainMode = FormObtainBarcode.OBMode.Scanner;
            while(DialogResult.OK == dlg.ShowDialog())
            {
                string errorInfo;
                if (_ms.CurrRecipeID == dlg.Barcode && !string.IsNullOrEmpty(dlg.Barcode))
                    return;
                bool isSetProdIDOK = _ms.SetCurrRecipeID(dlg.Barcode, out errorInfo);
                if (isSetProdIDOK)
                {
                    lbProdID.Text = dlg.Barcode;
                    lbProdID.ForeColor = Color.Black;
                    UpdateMappingByProdID(dlg.Barcode);
                    //if (!string.IsNullOrEmpty(_ms.CurrLotID))
                    //    _detectResultTransfer.SetRecipeLot(_ms.CurrRecipeID, _ms.CurrLotID, out string err);
                    break;
                }
                else
                {
                    if (DialogResult.Yes != MessageBox.Show("设置产品ID失败，错误信息:" + errorInfo + "是否重新输入？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        break;
                }
                
            }


        }


        /// <summary>
        /// 切换当前流程批次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetLotID_Click(object sender, EventArgs e)
        { 
            FormObtainBarcode dlg = new FormObtainBarcode();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.Text = "输入流程批次号";
            string lastLotID = _ms.CurrLotID;
            dlg.SetInitBarcode(lastLotID);
            ////日后添加流程批次可选值

            dlg.ObtainMode = FormObtainBarcode.OBMode.Scanner;
            while (DialogResult.OK == dlg.ShowDialog())
            {
                string errorInfo;
                bool isSetLotIDOK = _ms.SetCurrLotID(dlg.Barcode, out errorInfo);
                if (isSetLotIDOK)
                {
                    lbLotID.Text = dlg.Barcode;
                    lbLotID.ForeColor = Color.Black;
                    //if(!string.IsNullOrEmpty(_ms.CurrRecipeID))
                    //    _detectResultTransfer.SetRecipeLot(_ms.CurrRecipeID, _ms.CurrLotID, out string error);
                    break;
                }
                else
                {
                    if (DialogResult.Yes != MessageBox.Show("设置流程批次号失败，错误信息:" + errorInfo + "是否重新输入？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        break;
                }

            }
        }



        bool _isShowRealtimeDetectImg = true; //是否显示实时图像
        /// <summary>
        /// 切换到实时检测图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSwitchRTImage_Click(object sender, EventArgs e)
        {
            _isShowRealtimeDetectImg = true;
        }


        /// <summary>
        /// 清空当前料片（实时）检测信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClearDetectInfo_Click(object sender, EventArgs e)
        {
            rchRDetectInfo.Text = "";
        }

        /// <summary>
        /// 手动上料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btManualFeed_Click(object sender, EventArgs e)
        {
            //料盒/左轨道工站 必须开放一个系统数据项 （上料盒送料完成）
            if(null == _ms)
                return;
            if(_ms.IsAlarming)
            {
                MessageBox.Show("无效操作:设备已报警！");
                return;
            }

            if(_ms.WorkStatus != JFInterfaceDef.JFWorkStatus.Running)
            {
                MessageBox.Show("无效操作,设备当前状态：" + _ms.WorkStatus);
                return;
            }
            string itemName = "检测工站:轨道工站送料完成";
            if (!JFHubCenter.Instance.DataPool.ContainItem(itemName))
            {
                MessageBox.Show("系统数据池未包含数据项:\" " + itemName + "\"");
                return ;
            }
            JFHubCenter.Instance.DataPool.SetItemValue(itemName, true);
            JFTipsDelayClose.Show("已设置送料信号!", 1);

        }
        FormScrolTips fmTips = new FormScrolTips();
        /// <summary>
        /// 显示运行信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btShowRunInfo_Click(object sender, EventArgs e)
        {
            fmTips.Show();
        }

        bool _isSettingChkListTaskShows = false;
        private void chkListTaskShows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isSettingChkListTaskShows)
                return;

        }


        string _workStatusTxt = "未运行";
        //响应工作状态改变消息，由MainStation发过来
        public void OnWorkStatusChanged(JFWorkStatus ws)
        {
            BeginInvoke(new Action(()=>{
                switch(ws)
                {
                    case JFWorkStatus.UnStart:// = 0,    //线程未开始运行
                        _workStatusTxt = "未开始";
                        lbRunStatus.ForeColor = Color.Black;
                        lbRunStatus.Text = _workStatusTxt;
                        break;
                    case JFWorkStatus.Running://,        //线程正在运行，未退出
                        _workStatusTxt = "运行中";
                        lbRunStatus.ForeColor = Color.Green;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        break ;
                    case JFWorkStatus.Pausing://,        //线程暂停中
                        _workStatusTxt = "已暂停";
                        lbRunStatus.ForeColor = Color.Yellow;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        return;
                    case JFWorkStatus.Interactiving://,  //人机交互 ， 等待人工干预指令
                        _workStatusTxt = "等待输入";
                        lbRunStatus.ForeColor = Color.Yellow;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        break ;
                    case JFWorkStatus.NormalEnd://,     //线程正常完成后退出
                        _workStatusTxt = "正常结束";
                        lbRunStatus.ForeColor = Color.Black;
                        lbRunStatus.Text = _workStatusTxt;
                        break;
                    case JFWorkStatus.CommandExit://,    //收到退出指令
                        _workStatusTxt = "指令结束";
                        lbRunStatus.ForeColor = Color.Black;
                        lbRunStatus.Text = _workStatusTxt;
                        break;
                    case JFWorkStatus.ErrorExit://,      //发生错误退出，（重启或人工消除错误后可恢复）
                        _workStatusTxt = "错误停止";
                        lbRunStatus.ForeColor = Color.Red;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        break ;
                    case JFWorkStatus.ExceptionExit://,  //发生异常退出 ,  (不可恢复的错误)
                        _workStatusTxt = "异常停止";
                        lbRunStatus.ForeColor = Color.Red;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        break;
                    case JFWorkStatus.AbortExit://,      //由调用者强制退出
                        _workStatusTxt = "强制退出";
                        lbRunStatus.ForeColor = Color.Red;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        break ;
                    default:
                        _workStatusTxt = ws.ToString();
                        lbRunStatus.ForeColor = Color.Red;
                        lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
                        break;
                }
                fmTips.AddOneTips("运行状态改变为:" + _workStatusTxt);
                return;
            }));
        }


        string _currCustomStatus = "";
        public void OnCustomStatusChanged(string customStatus)
        {
            _currCustomStatus = customStatus;
            BeginInvoke(new Action(() =>
            {
                fmTips.AddOneTips("工作状态改变为:" + customStatus);
                lbRunStatus.Text = _workStatusTxt + " " + _currCustomStatus;
            }));
        }

        //受到一条文本消息
        public void OnTxtMsg(string info)
        {
            fmTips.AddOneTips(info);
        }

        public static string CMC_ShowJFImage = "ShowJFImge"; //显示IJFImage对象
        public static string CMC_ShowHO = "ShowHObject";//显示Halcon对象
        public static string CMC_InspectResult = "InspectResult"; //FOV检测结果 , 后面的参数列表为 HObject/图像，
        public static string CMC_DieResults = "DieResult"; //Die检测结果 
        public static string CMC_StartNewPiece = "StartNewPiece"; //开始检测一片新料片，参数为PieceID(时间)
        public static string CMC_PieceFinished = "Piece Finished";//料片检测结束

        HObject _hoOrgImgShowing = null; //正在显示的图像,(在显示新的图像之前必须先释放此对象)
        List<InspectionData> _lstInspectData = new List<InspectionData>();

        //DLAFDetectResultTransfer _detectResultTransfer = new DLAFDetectResultTransfer();
        //受到自定义消息
        public void OnCustomizeMsg(string msgCategory, object[] msgParams)
        {
            fmTips.AddOneTips("工站消息Categoty:" + msgCategory);
            BeginInvoke(new Action(() => {
                if(msgCategory == CMC_ShowJFImage) //显示一个IJFImage对象
                {
                    object ob;
                    IJFImage ij = msgParams[0] as IJFImage;
                    if (null == ij)
                    {
                        fmTips.AddOneTips("消息参数IJFImage == null");
                        return;
                    }
                    int err = ij.GenHalcon(out ob);
                    if(err !=0)
                    {
                        fmTips.AddOneTips("参数IJFImage.GenHalcon 出错:" + ij.GetErrorInfo(err));
                        return;
                    }
                    htWindowControl1.DispImage(ob as HObject);
                }
                else if(msgCategory == CMC_ShowHO)//显示一个Halcon对象
                {
                    htWindowControl1.DispImage(msgParams[0] as HObject);
                }
                else if (CMC_StartNewPiece == msgCategory)
                {
                    rchRDetectInfo.Text = "";
                    string pieceID = msgParams[0] as string;
                    rchRDetectInfo.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + msgParams[0] + " 开始检测，PieceID = pieceID");
                    //_detectResultTransfer.SetPieceID(pieceID);
                    for (int i = 0; i < _currRecipe.RowNumber; i++)
                        for (int j = 0; j < _currRecipe.ColumnNumber * _currRecipe.BlockNumber;j++) 
                            mappCtrl.SetDieState(i, j, "未检测");
                    _lstInspectData.Clear();
                }
                else if(msgCategory == CMC_InspectResult) //一个FOV的检测结果
                {
                    IJFImage[] orgImgs = msgParams[0] as IJFImage[];
                    DlafFovDetectResult fdr = (msgParams[1] as DlafFovDetectResult);//.Clone();
                    int imgIndex2Show = 0;
                    if (_isShowRealtimeDetectImg) //需要实时显示图像
                    {
                        string taskName2Show = cbTaskImgShow.Text;
                        if(!string.IsNullOrEmpty(taskName2Show))
                            for(int i = 0; i < fdr.TaskNames.Length;i++)
                                if(fdr.TaskNames[i] == taskName2Show)
                                {
                                    imgIndex2Show = i;
                                    break;
                                }

                        //if (null != fdr.WireRegion) //老版本的数据显示功能
                        //{
                        //    htWindowControl1.ColorName = "green"; //金线为绿色显示
                        //    if(null != _hoOrgImgShowing)
                        //    {
                        //        _hoOrgImgShowing.Dispose();
                        //        _hoOrgImgShowing = null;

                        //    }

                        //    Object ob;
                        //    orgImgs[imgIndex2Show].GenHalcon(out ob);
                        //    _hoOrgImgShowing = ob as HObject;
                        //    htWindowControl1.RefreshWindow(_hoOrgImgShowing, fdr.WireRegion, "fit");
                        //}
                        //if (null != fdr.DetectIterms && fdr.DetectIterms.Count > 0)
                        //{
                        //    htWindowControl1.ColorName = "yellow"; //其他成功检测项显示为黄色
                        //    foreach (HObject ho in fdr.DetectIterms.Values)
                        //        htWindowControl1.DispRegion(ho);
                        //}

                        //if(null != fdr.DiesErrorRegions)
                        //{
                        //    htWindowControl1.ColorName = "red"; //错误区域显示为红色
                        //    foreach(HObject[] errorRegions in fdr.DiesErrorRegions)
                        //        if(null != errorRegions)
                        //            foreach(HObject ho in errorRegions)
                        //                htWindowControl1.DispRegion(ho);
                        //}


                        //新数据结构的显示
                        if (null != _hoOrgImgShowing)
                        {
                            _hoOrgImgShowing.Dispose();
                            _hoOrgImgShowing = null;
                        }

                        Object ob;
                        if (0 != orgImgs[imgIndex2Show].GenHalcon(out ob)) //图像转化出错
                        {
                            htWindowControl1.ClearHWindow();
                        }
                        else
                        {
                            _hoOrgImgShowing = (HObject)ob;
                            htWindowControl1.RefreshWindow(_hoOrgImgShowing, null, "fit"); //显示图片
                        }

                        //显示各检测项
                        if (!fdr.IsDetectSuccess || null == fdr.DieInspectResults)
                            return;

                        HObject errRegion = null;//所有错误区域
                        HOperatorSet.GenEmptyRegion(out errRegion);


                        HObject bondRegion = null; //焊点
                        HOperatorSet.GenEmptyRegion(out bondRegion);
                        HObject wireRegion = null;
                        HOperatorSet.GenEmptyRegion(out wireRegion);
                        HObject epoxyRegion = null;
                        HOperatorSet.GenEmptyRegion(out epoxyRegion);

                        bool _isShowAllItem = !chkJustShowNG.Checked;
                        foreach (InspectResultItem[] dieItems in fdr.DieInspectResults)
                            foreach (InspectResultItem item in dieItems)
                            {
                                if (!item.IsDetectOK())
                                    HOperatorSet.ConcatObj(errRegion, item.ResultRegion, out errRegion);
                                if (!_isShowAllItem)
                                    continue;

                                switch (item.InspectCategoty)
                                {
                                    case InspectResultItem.Categoty.Frame:
                                        break;
                                    case InspectResultItem.Categoty.IC:
                                        break;
                                    case InspectResultItem.Categoty.Bond:
                                        HOperatorSet.ConcatObj(bondRegion, item.ResultRegion, out bondRegion);
                                        break;
                                    case InspectResultItem.Categoty.Wire:
                                        HOperatorSet.ConcatObj(wireRegion, item.ResultRegion, out wireRegion);
                                        break;
                                    case InspectResultItem.Categoty.Epoxy:
                                        HOperatorSet.ConcatObj(epoxyRegion, item.ResultRegion, out epoxyRegion);
                                        break;
                                }
                            }
                        if (_isShowAllItem)
                        {
                            htWindowControl1.ColorName = "green";
                            htWindowControl1.DispRegion(wireRegion);
                            htWindowControl1.ColorName = "yellow";
                            htWindowControl1.DispRegion(bondRegion);
                            htWindowControl1.ColorName = "blue";
                            htWindowControl1.DispRegion(epoxyRegion);
                        }
                        htWindowControl1.ColorName = "red";
                        htWindowControl1.DispRegion(errRegion);



                    }


                    if (fdr.DetectDiesImages != null)
                        foreach (HObject ho in fdr.DetectDiesImages)
                            ho.Dispose();


                    ///错误信息显示
                    StringBuilder sbInfo = new StringBuilder();
                    sbInfo.AppendLine("Fov:" + fdr.FovName + " 检测结果:");
                    if(!fdr.IsDetectSuccess)
                        sbInfo.AppendLine("图像检测失败,ErrorInfo:" + fdr.DetectErrorInfo);
                    else
                    {
                        if(fdr.IsFovOK)
                            sbInfo.AppendLine("检测结果: OK");
                        else
                        {
                            for(int i = 0; i <fdr.DetectDiesRows.Length;i++)
                            {
                                if (fdr.IsDieOK(i))
                                    sbInfo.AppendLine("Row-" + fdr.ICRow + " Col-"+ fdr.ICCol + " Die-" + i + " OK");
                                else
                                {
                                    sbInfo.AppendLine("Row-" + fdr.ICRow + " Col-" + fdr.ICCol + " Die-" + i + " NG  错误信息:");
                                    int[] errors = fdr.DiesErrorCodes[i];
                                    //for(int j = 0; j < errors.Length;j++)
                                    //    sbInfo.AppendLine("ErrCode:" + errors[j] + " Info:" + InspectNode.DieErrorDescript(errors[j]) + "In Task:" + fdr.DiesErrorTaskNames[i][j]);
                                    
                                }
                            }
                        }
                    }


                    fmTips.AddOneTips(sbInfo.ToString());
                    rchRDetectInfo.AppendText(sbInfo.ToString());

                    //将Fov检测项填入表格
                    dgvDetectItems.Rows.Clear();
                    if(null != fdr.DieInspectResults)
                    {
                        bool _isShowAllItems = !chkJustShowNG.Checked;


                        int dieIndex = 0;
                        int dieCount = fdr.CurrColCount * fdr.CurrRowCount;
                        if (dieCount <= 0)//Fov只是Die的一部分
                            dieCount = 1;
                        //表格行 ： Die/检测项/检测结果/检测标准数据/检测结果数据/备注
                        for (dieIndex = 0; dieIndex < dieCount; dieIndex++)
                        {
                            if (!fdr.IsDetectSuccess) //视觉算子出错
                            {
                                DataGridViewRow row = new DataGridViewRow();

                                DataGridViewTextBoxCell dieIndexCell = new DataGridViewTextBoxCell();
                                dieIndexCell.Style.BackColor = Color.Red;
                                dieIndexCell.Value = dieIndex + 1;
                                row.Cells.Add(dieIndexCell);


                                DataGridViewTextBoxCell detectItemCell = new DataGridViewTextBoxCell();
                                detectItemCell.Value = "---";
                                detectItemCell.Style.ForeColor = Color.Red;
                                row.Cells.Add(detectItemCell);

                                DataGridViewTextBoxCell dieErrorCell = new DataGridViewTextBoxCell();
                                dieErrorCell.Value = "检测出错:" + fdr.DetectErrorInfo;
                                dieErrorCell.Style.ForeColor = Color.Red;
                                row.Cells.Add(dieErrorCell);




                                DataGridViewTextBoxCell dieDetectVelCell = new DataGridViewTextBoxCell();
                                dieDetectVelCell.Value = "---";
                                dieDetectVelCell.Style.ForeColor = Color.Red;
                                row.Cells.Add(dieDetectVelCell);


                                DataGridViewTextBoxCell dieRefVelCell = new DataGridViewTextBoxCell();
                                dieRefVelCell.Value = "---";
                                dieRefVelCell.Style.ForeColor = Color.Red;
                                row.Cells.Add(dieRefVelCell);

                                DataGridViewTextBoxCell dieRemarksCell = new DataGridViewTextBoxCell();
                                string rowColInfo = "Die:" + (dieIndex + 1) + " 行:" + (dieIndex / dieCount + 1) + " 列:" + (dieIndex % dieCount + 1);
                                dieRemarksCell.Value = rowColInfo;
                                dieRemarksCell.Style.ForeColor = Color.Red;
                                row.Cells.Add(dieRemarksCell);


                                dgvDetectItems.Rows.Add(row);
                            }
                            else //检测成功，列出每个检测项
                            {
                                for (int i = 0; i < fdr.DieInspectResults[dieIndex].Length; i++)
                                {
                                    if (!_isShowAllItems && fdr.DieInspectResults[dieIndex][i].IsDetectOK())
                                        continue;
                                    DataGridViewRow row = new DataGridViewRow();
                                    Color txtColor = Color.DarkGreen;
                                    if (!DlafFovDetectResult.IsInspectItemsOK(fdr.DieInspectResults[dieIndex]))
                                        txtColor = Color.DarkRed;
                                    DataGridViewTextBoxCell dieIndexCell = new DataGridViewTextBoxCell();
                                    dieIndexCell.Style.ForeColor = txtColor;
                                    dieIndexCell.Value = dieIndex + 1;
                                    row.Cells.Add(dieIndexCell);

                                    DataGridViewTextBoxCell detectItemCell = new DataGridViewTextBoxCell();
                                    detectItemCell.Value = fdr.DieInspectResults[dieIndex][i].InspectID;
                                    detectItemCell.Style.ForeColor = fdr.DieInspectResults[dieIndex][i].IsDetectOK() ? Color.DarkGreen : Color.DarkRed;
                                    row.Cells.Add(detectItemCell);

                                    DataGridViewTextBoxCell dieErrorCell = new DataGridViewTextBoxCell();
                                    dieErrorCell.Value = string.Join(" ", fdr.DieInspectResults[dieIndex][i].ErrorTexts);
                                    dieErrorCell.Style.ForeColor = fdr.DieInspectResults[dieIndex][i].IsDetectOK() ? Color.DarkGreen : Color.DarkRed;
                                    row.Cells.Add(dieErrorCell);


                                    DataGridViewTextBoxCell dieRefVelCell = new DataGridViewTextBoxCell();
                                    dieRefVelCell.Value = fdr.DieInspectResults[dieIndex][i].QualifiedDescript;
                                    //dieRefVelCell.Style.ForeColor = fdr.DieInspectResults[dieIndex][i].IsDetectOK() ? Color.DarkGreen : Color.DarkRed;
                                    row.Cells.Add(dieRefVelCell);


                                    DataGridViewTextBoxCell dieDetectVelCell = new DataGridViewTextBoxCell();
                                    dieDetectVelCell.Value = fdr.DieInspectResults[dieIndex][i].ResultDescript;
                                    row.Cells.Add(dieDetectVelCell);

                                    DataGridViewTextBoxCell dieRemarksCell = new DataGridViewTextBoxCell();
                                    string rowColInfo = "Die:" + (dieIndex + 1) + " 行:" + (dieIndex / dieCount + 1) + " 列:" + (dieIndex % dieCount + 1);
                                    dieRemarksCell.Value = rowColInfo;
                                    row.Cells.Add(dieRemarksCell);


                                    dgvDetectItems.Rows.Add(row);
                                }
                            }
                        }
                    }



                }
                else if(CMC_DieResults == msgCategory)
                {
                    InspectionData[] inspDatas = msgParams[0] as InspectionData[];
                    if (null == inspDatas)
                        return;
                    _lstInspectData.AddRange(inspDatas);
                    foreach(InspectionData inspData in inspDatas)
                    {
                        mappCtrl.SetDieState(inspData.RowIndex, inspData.ColumnIndex, inspData.InspectionResult == InspectionResults.OK ? "合格" : "不合格");
                    }
                }
                else if(CMC_PieceFinished == msgCategory)
                {
                    bool isOnLineReview = (bool)JFHubCenter.Instance.SystemCfg.GetItemValue("在线复判");
                    string recipeID = msgParams[0] as string;
                    string lotID = msgParams[1] as string;
                    string pieceID = msgParams[2] as string;
                    string inspectResultFolder = msgParams[3] as string;
                    string pictureFolder = msgParams[4] as string;
                    rchRDetectInfo.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + recipeID + " 检测完成");
                    if(!isOnLineReview)
                        fmTips.AddOneTips(msgParams[0] + " 检测完成");
                    else
                    {
                        FormDLAFReview fm = new FormDLAFReview();
                        fm.SetReviewParam(recipeID, lotID, pieceID, inspectResultFolder + "\\" + recipeID + ".db", pictureFolder);
                        fm.ShowDialog();
                        JFHubCenter.Instance.DataPool.SetItemValue("检测工站:在线复判完成", true);
                    }

                }
                else
                {
                    
                }

            }));
        }


        public void OnProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            fmTips.AddOneTips("ProductFinish: PassCount = " + passCount + " NGCount = " + ngCount);
            //添加其他处理流程
        }

        private void cbTaskImgShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTaskImgShow.SelectedIndex < 0)
                _taskImgShow = null;
            else
                _taskImgShow = cbTaskImgShow.Text;
        }




        /// <summary>
        /// 显示Review窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReview_Click(object sender, EventArgs e)
        {
            FormDLAFReview formReview = new FormDLAFReview();
            formReview.ShowDialog();
        }
    }
}
