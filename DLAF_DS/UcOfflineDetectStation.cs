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
using JFHub;
using DLAF;
using System.IO;
using HalconDotNet;
using LFAOIRecipe;

namespace DLAF_DS
{
    //离线检测工站使用的界面
    public partial class UcOfflineDetectStation : JFRealtimeUI, IJFStationMsgReceiver
    {
        public UcOfflineDetectStation()
        {
            InitializeComponent();
            _dctDieStateColor.Add("未检测", Color.SkyBlue);
            _dctDieStateColor.Add("合格", Color.Green);
            _dctDieStateColor.Add("不合格", Color.Red);
            ////dic_DieState_Color.Add("复看合格", Color.Yellow);
            //dic_DieState_Color.Add("无芯片", Color.Purple); 
            _dctDieStateColor.Add("匹配失败", Color.Pink);

            _rbTaskNames = new RadioButton[] { rbTask0, rbTask1, rbTask2, rbTask3, rbTask4, rbTask5 };
            mapDetectCells.OnSelectedDieChanged += MapDetectCells_OnSelectedDieChanged;
        }

        string _picFolder = null; //由料片开始检测消息带过来

        private void MapDetectCells_OnSelectedDieChanged(HTMappingControl.DieInfo dieInfo)
        {
            if (_currRecipe == null)
                return;
            if(string.IsNullOrEmpty(_picFolder))
            {
                MessageBox.Show("未开始检测/图片文件夹信息未设置");
                return;
            }
            _currICRow = dieInfo.RowIndex;
            _currICCol = dieInfo.ColumnIndex / _currRecipe.FovCount;
            _currFov = _currRecipe.FovNames()[dieInfo.ColumnIndex % _currRecipe.FovCount];
            //载入图象
            string fovSubFolder = "Row_" + _currICRow + "-" + "Col_" + _currICCol + "-Fov_" + _currFov;
            string fovFolder = _picFolder + "\\" + fovSubFolder;//存储图片的文件夹
            if (!Directory.Exists(fovFolder))
            {
                MessageBox.Show( "Fov图片文件夹:\"" + fovFolder + "\"不存在");
                return;
            }
           

            string[] filesInFovFolder = Directory.GetFiles(fovFolder);//Fov文件夹中现有的文件
            if (null == filesInFovFolder || filesInFovFolder.Length == 0)
            {
                MessageBox.Show("Fov图片文件夹:\"" + fovFolder + "\"中没有文件");
                return;
            }


            string[] taskNames = _currRecipe.TaskNames(_currFov);
            if(_currTaskImgs != null)
            {
                foreach (HObject ho in _currTaskImgs)
                    ho.Dispose();
                _currTaskImgs = null;
            }
            _currTaskImgs = new List<HObject>();
            foreach (string taskName in taskNames)
            {
                HObject hoImg;
                HOperatorSet.GenEmptyObj(out hoImg);
                string taskImgFile = null;
                foreach (string s in filesInFovFolder)
                {
                    string exn = Path.GetExtension(s);
                    if (string.Compare(exn, ".bmp", true) != 0 &&
                        string.Compare(exn, ".tiff", true) != 0 &&
                        string.Compare(exn, ".tif", true) != 0 &&
                        string.Compare(exn, ".jpg", true) != 0 &&
                        string.Compare(exn, ".jpeg", true) != 0 &&
                        string.Compare(exn, ".png", true) != 0)
                        continue;//先过滤掉非图像文件
                    string fnn = Path.GetFileNameWithoutExtension(s); //不带后缀的文件名
                    if (fnn.LastIndexOf(taskName) >= 0 &&
                        fnn.Length == (fnn.LastIndexOf(taskName) + taskName.Length))
                    {
                        taskImgFile = s;
                        break;
                    }
                }
                if (null == taskImgFile)
                {
                    MessageBox.Show("Task = " + taskName + "  图片文件不存在");
                    return;
                }
                    

                HOperatorSet.ReadImage(out hoImg, taskImgFile);
                _currTaskImgs.Add(hoImg);
               
            }
            ShowFovImageResult();

            //更新错误列表
            int frIndex = (_currICRow * _currRecipe.ColCount + _currICCol) * _currRecipe.FovCount + _FovNameIndex(_currFov);
            if (frIndex < _lstFovResults.Count)
                UpdateErrorDataGrid(_lstFovResults[frIndex]);
        }

        private void UcOfflineDetectStation_Load(object sender, EventArgs e)
        {
            AdjustStationView();

        }

        RadioButton[] _rbTaskNames = null;

        bool _isLstBoxPieceIDSetting = false;
        DLAFOfflineSetectStation _station = null;
        Dictionary<string, Color> _dctDieStateColor = new Dictionary<string, Color>();

        bool _isParamEditting = false;
        internal bool ParamEditEnabled 
        {
            get { return _isParamEditting; }
            set
            {
                _isParamEditting = value;
                btEditCancel.Enabled = _isParamEditting;
                cbLotID.Enabled = _isParamEditting;
                cbRecipeID.Enabled = _isParamEditting;
                btSetPicFolder.Enabled = _isParamEditting;
                btEditSave.Text = _isParamEditting ? "保存" : "编辑";
                if (null != _rbTaskNames)
                    foreach (RadioButton rb in _rbTaskNames)
                        rb.Enabled = !_isParamEditting;
            }
         }

        public void SetStation(DLAFOfflineSetectStation station)
        {
            _station = station;
            if (Created)
                AdjustStationView();
        }

        void AdjustStationView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustStationView));
                return;
            }
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if(null == rm || !rm.IsInitOK)
            {
                btEditSave.Enabled = false;
                cbRecipeID.Text = "配方管理器无效";
                rchDetectInfo.Text = "配方管理器无效";
                lstBoxPieceIDs.Enabled = false;
                btUpdateInspect.Enabled = false;
                return;
            }

            if(_station != null)
            {
                cbRecipeID.Text = _station.RecipeID;
                cbLotID.Text = _station.LotID;
                rchPicFolder.Text = _station.TestPicFolder;
            }

            UpdateParamPanel();
            UpdateMapCtrl();
            UpdatePieceIDList();

        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateParamPanel()
        {
            if(_station == null)
            {
                cbLotID.Text = "";
                cbRecipeID.Text = "";
                rchPicFolder.Text = "";
                return;

            }
            string crrSelRecipeID = cbRecipeID.Text;
            List<string> recipeIDsInCb = new List<string>();
            foreach (object o in cbRecipeID.Items)
                recipeIDsInCb.Add(o.ToString());

            string currSelLotID = cbLotID.Text;
            List<string> lotIDsInCb = new List<string>();
            foreach (string o in cbLotID.Items)
                lotIDsInCb.Add(o.ToString());

            string currSelPicFolder = rchPicFolder.Text;

            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            string[] allRecipeIDs = rm.AllRecipeIDsInCategoty("Product");
            if(null == allRecipeIDs || 0 == allRecipeIDs.Length)
            {
                cbRecipeID.Items.Clear();
                cbRecipeID.Text = currSelPicFolder;
                cbLotID.Items.Clear();
                cbRecipeID.Text = currSelLotID;
                lstBoxPieceIDs.Items.Clear();
                return;
            }

            bool isNeedResetRecipeCb = false;
            do
            {
                if(allRecipeIDs.Length != recipeIDsInCb.Count())
                {
                    isNeedResetRecipeCb = true;
                    break;
                }
                for(int i = 0; i < allRecipeIDs.Length;i++)
                    if(allRecipeIDs[i] != recipeIDsInCb[i])
                    {
                        isNeedResetRecipeCb = true;
                        break;
                    }
            } while (false);
            if(isNeedResetRecipeCb)
            {
                cbRecipeID.Items.Clear();
                foreach (string s in allRecipeIDs)
                    cbRecipeID.Items.Add(s);
                if (!string.IsNullOrWhiteSpace(crrSelRecipeID))
                    cbRecipeID.Text = crrSelRecipeID;
            }

            if(string.IsNullOrWhiteSpace(cbRecipeID.Text))
            {
                cbLotID.Items.Clear();
                cbLotID.Text = currSelLotID;
                lstBoxPieceIDs.Items.Clear();
                return;
            }
            if(string.IsNullOrWhiteSpace(currSelPicFolder))
            {
                cbLotID.Items.Clear();
                lstBoxPieceIDs.Items.Clear();
                cbLotID.Text = currSelLotID;
                return;
            }

            if(!Directory.Exists(currSelPicFolder))
            {
                cbLotID.Items.Clear();
                lstBoxPieceIDs.Items.Clear();
                cbLotID.Text = currSelLotID;
                return;
            }

            string recipeFolder = currSelPicFolder + "\\" + cbRecipeID.Text;
            if (!Directory.Exists(recipeFolder))
            {
                cbLotID.Items.Clear();
                lstBoxPieceIDs.Items.Clear();
                cbLotID.Text = currSelLotID;
                return;
            }
            string[] lotsFolders = Directory.GetDirectories(recipeFolder);
            if(null == lotsFolders || 0 == lotsFolders.Length)
            {
                cbLotID.Items.Clear();
                lstBoxPieceIDs.Items.Clear();
                cbLotID.Text = currSelLotID;
                return;
            }

            bool isNeedResetLotCb = false;
            do
            {
                if (lotsFolders.Length != lotIDsInCb.Count())
                {
                    isNeedResetLotCb = true;
                    break;
                }
                for (int i = 0; i < lotsFolders.Length; i++)
                    if (lotsFolders[i] != lotIDsInCb[i])
                    {
                        isNeedResetLotCb = true;
                        break;
                    }
            } while (false);

            if (isNeedResetLotCb)
            {
                cbLotID.Items.Clear();
                foreach (string s in lotsFolders)
                    cbLotID.Items.Add(s.Substring(s.LastIndexOf("\\")+1));
                if (lotsFolders.Contains(currSelLotID))
                    for (int i = 0; i < lotsFolders.Length; i++)
                        if (lotsFolders[i] == currSelLotID)
                        {
                            cbLotID.SelectedIndex = i;
                            return;
                        }
                cbLotID.Text = currSelLotID;
            }

        }

        HObject _hoFullImg = null; //Recipe全图

        void UpdateMapCtrl()
        {
            string currSelRecipeID = cbRecipeID.Text;
            if (string.IsNullOrEmpty(currSelRecipeID))
            {
                mapDetectCells.Initial(1, 1, _dctDieStateColor, "未检测");
                htFullImg.Refresh();
                return;
            }
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if(null == rm || !rm.IsInitOK)
            {
                mapDetectCells.Initial(1, 1, _dctDieStateColor, "未检测");
                htFullImg.Refresh();
                return;
            }
            JFDLAFProductRecipe recipe = rm.GetRecipe("Product", currSelRecipeID) as JFDLAFProductRecipe;
            if(null == recipe)
            {
                mapDetectCells.Initial(1, 1, _dctDieStateColor, "未检测");
                htFullImg.Refresh();
                return;

            }

            {
                int row = recipe.RowCount;
                int col = recipe.ColCount*recipe.FovCount;
                mapDetectCells.Initial(row, col, _dctDieStateColor, "未检测");
            }

            string fullImgPath = recipe.GetFrameMapImgFullPath(currSelRecipeID);
            if(string.IsNullOrEmpty(fullImgPath))
            {
                htFullImg.Refresh();
                return;
            }

            if(!File.Exists(fullImgPath))
            {
                htFullImg.Refresh();
                return;
            }

            if(null != _hoFullImg)
            {
                _hoFullImg.Dispose();
                _hoFullImg = null;
            }
            HOperatorSet.GenEmptyObj(out _hoFullImg);
            HOperatorSet.ReadImage(out _hoFullImg, fullImgPath);
            htFullImg.DispImage(_hoFullImg);

        }

        void UpdatePieceIDList()
        {
            string picFolder = rchPicFolder.Text;
            string recipeID = cbRecipeID.Text;
            string lotID = cbLotID.Text;
            string currPieceID = lstBoxPieceIDs.SelectedItem == null ? null : lstBoxPieceIDs.SelectedItem.ToString();
            if(string.IsNullOrEmpty(picFolder) || !Directory.Exists(picFolder))
            {
                lstBoxPieceIDs.Items.Clear();
                    return;
            }
            if(string.IsNullOrEmpty(recipeID) || !Directory.Exists(picFolder+ "\\" + recipeID))
            {
                lstBoxPieceIDs.Items.Clear();
                return;
            }

            if(string.IsNullOrEmpty(lotID) || !Directory.Exists(picFolder + "\\" + recipeID +"\\" + lotID))
            {
                lstBoxPieceIDs.Items.Clear();
                return;
            }
            lstBoxPieceIDs.Items.Clear();
            string[] pieceFolders = Directory.GetDirectories(picFolder + "\\" + recipeID + "\\" + lotID);
            if(null != pieceFolders)
            {
                for(int i = 0; i < pieceFolders.Length;i++)
                {
                    lstBoxPieceIDs.Items.Add(pieceFolders[i].Substring(pieceFolders[i].LastIndexOf("\\")+1));
                    if (pieceFolders[i] == currPieceID)
                    {

                        _isLstBoxPieceIDSetting = true;
                        lstBoxPieceIDs.SelectedIndex = i;
                        _isLstBoxPieceIDSetting = false;
                    }
                }
            }

            if (lstBoxPieceIDs.SelectedIndex < 0 && lstBoxPieceIDs.Items.Count > 0)
                lstBoxPieceIDs.SelectedIndex = 0;

        }

        /// <summary>
        /// 编辑/保存检测参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditSave_Click(object sender, EventArgs e)
        {
            if(null == _station)
            {
                MessageBox.Show("无效操作，工站未设置！");
                return;
            }
            if(!ParamEditEnabled)
            {
                if(_station.IsWorking())
                {
                    MessageBox.Show("工站正在运行,不能修改参数！");
                    return;
                }
                ParamEditEnabled = true;
            }
            else
            {
                if(SaveParam2Station())
                {
                    ParamEditEnabled = false;
                    _currRecipe = (JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager).GetRecipe("Product", cbRecipeID.Text) as JFDLAFProductRecipe;
                    return;
                }
                _currRecipe = null;
            }
        }

        bool SaveParam2Station()
        {
            if(null == _station)
            {
                MessageBox.Show("工站未设置");
                return false;
            }
            string recipeID = cbRecipeID.Text;
            if(string.IsNullOrEmpty(recipeID))
            {
                MessageBox.Show("RecipeID 参数未设置");
                return false;
            }


            string lotID = cbLotID.Text;
            if(string.IsNullOrEmpty(lotID))
            {
                MessageBox.Show("LotID 参数未设置");
                return false;
            }

            string picFolder = rchPicFolder.Text;
            if(string.IsNullOrEmpty(picFolder))
            {
                MessageBox.Show("结果图像文件夹 参数未设置");
                return false;
            }

            string errorInfo;
            if(!_station.SetRecipeID(recipeID,out errorInfo))
            {
                MessageBox.Show(errorInfo);
                    return false;
            }

            if(!_station.SetLotID(lotID,out errorInfo))
            {
                MessageBox.Show(errorInfo);
                return false;
            }

            if(!_station.SetTestPidFolder(picFolder,out errorInfo))
            {
                MessageBox.Show(errorInfo);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 取消检测参数的编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditCancel_Click(object sender, EventArgs e)
        {
            if(_station!= null)
            {
                cbRecipeID.Text = _station.RecipeID;
                cbLotID.Text = _station.LotID;
                rchPicFolder.Text = _station.TestPicFolder;
            }
            ParamEditEnabled = false;
        }

        /// <summary>
        /// 设置结果图片路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetPicFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择结果图片存放目录";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                //string sPath = folder.SelectedPath;
                rchPicFolder.Text = folder.SelectedPath;
                UpdatePieceIDList();
            }
            
        }

        /// <summary>
        /// 更新视觉算子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateInspect_Click(object sender, EventArgs e)
        {
            if(_station != null && _station.IsWorking())
            {
                MessageBox.Show("工站正在运行，不能更新算子");
                return;
            }
            if(ParamEditEnabled)
            {
                MessageBox.Show("请先完成参数设置！");
                return;
            }
            string recipeID = cbRecipeID.Text;
            if(string.IsNullOrEmpty(recipeID))
            {
                MessageBox.Show("请先选择RecipeID");
                return;
            }
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if(null == rm || !rm.IsInitOK)
            {
                MessageBox.Show("配方管理器未设置/未初始化！");
                return;
            }
            JFDLAFInspectionManager inspMgr = JFDLAFInspectionManager.Instance;
            inspMgr.Clear();
            string errorInfo;
            if (!inspMgr.InitInspectNode(recipeID, out errorInfo))
            {
                MessageBox.Show("更新视觉算子失败，ErrorInfo:" + errorInfo);
                return;
            }
            MessageBox.Show("视觉算子已更新");

        }

        




        private void cbRecipeID_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParamPanel();
            UpdateMapCtrl();
        }

        private void cbLotID_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePieceIDList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstBoxPieceIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLstBoxPieceIDSetting)
                return;
            if (lstBoxPieceIDs.SelectedIndex < 0)
                return;
            if (_station == null || _station.IsWorking())
                return;
            string errInfo;
            if(!_station.SetPieceID(lstBoxPieceIDs.SelectedItem.ToString(),out errInfo))
            {
                MessageBox.Show(errInfo);
                _isLstBoxPieceIDSetting = true;
                lstBoxPieceIDs.SelectedItem = _station.PieceID;
                _isLstBoxPieceIDSetting = false;
            }
        }


        #region StationMsgRecv's Implement
        public void OnWorkStatusChanged(JFWorkStatus currWorkStatus) //工作状态变化
        {
            BeginInvoke(new Action(()=>{
                if (_station.IsWorking())
                {
                    ParamEditEnabled = false;
                    btEditSave.Enabled = false;
                    btUpdateInspect.Enabled = false;
                    rchDetectInfo.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + "   开始离线检测\n");
                }
                else
                {
                    btUpdateInspect.Enabled = true;
                    btEditSave.Enabled = true;
                }
            }));
            
        }

        public void OnCustomStatusChanged(int currCustomStatus) //业务逻辑变化
        {
            BeginInvoke(new Action(() => {
                rchDetectInfo.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + "   " +  _station.GetCustomStatusName(currCustomStatus)  +"\n");
            }));
        }

        public void OnTxtMsg(string txt) //接受一条文本消息
        {
            BeginInvoke(new Action(() => {
                rchDetectInfo.AppendText(txt + "\n");
            }));
        }

        public void OnProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo) //产品加工完成
        {
        }

        List<DlafFovDetectResult> _lstFovResults = new List<DlafFovDetectResult>(); //保存当前料片的测试结果

        public void OnCustomizeMsg(string msgCategory, object[] msgParams) //其他自定义消息
        {
            BeginInvoke(new Action(() =>
            {
                if (msgCategory == DLAFOfflineSetectStation.SCM_PieceStart) //开始了一个料片检测
                {
                    if(_currRecipe == null)
                        _currRecipe = (JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager).GetRecipe("Product", cbRecipeID.Text) as JFDLAFProductRecipe;

                    _lstFovResults.Clear();
                    mapDetectCells.InitaialDieState();
                    rchDetectInfo.Text = "";
                    rchDetectInfo.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + "    开始检测");

                    _currICRow = -1; //当前显示的ICRow
                    _currICCol = -1;
                    _currFov = null; //当前显示的Fov
                    _picFolder = msgParams[0] as string;
                }
                else if (msgCategory == DLAFOfflineSetectStation.SCM_PieceEnd) //一个料片检测完成
                {

                }
                else if (msgCategory == DLAFOfflineSetectStation.SCM_FovDetectResult) //一个Fov检测完成 ， 显示结果
                {
                    if(null != _currTaskImgs)
                    {
                        foreach (HObject ho in _currTaskImgs)
                            ho.Dispose();
                        _currTaskImgs = null;
                    }
                    _currTaskImgs = msgParams[0] as List<HObject>; //从Fov中读出的原图
                    DlafFovDetectResult fdr = (msgParams[1] as DlafFovDetectResult).Clone();
                    _lstFovResults.Add(fdr);

                    _currICRow = fdr.ICRow;
                    _currICCol = fdr.ICCol;
                    _currFov = fdr.FovName;



                    ShowFovImageResult(); //画图
                    UpdateErrorDataGrid(fdr); //将错误信息更新到DatagridView
                    UpdateDetectMapping(fdr); //将检测结果更新到Map控件


                    rchDetectInfo.AppendText("ICRow:" + fdr.ICRow + " ICCol:" + fdr.ICCol + " Fov:" + fdr.FovName + "-----\n");
                    rchDetectInfo.AppendText("检测" + (fdr.IsDetectSuccess ? "完成" : ("失败:" + fdr.DetectErrorInfo)) + "\n");
                 

                }
            }));
        }
        #endregion

        int _currICRow = -1; //当前显示的ICRow
        int _currICCol = -1;
        string _currFov = null; //当前显示的Fov
        int _currTask = 0;//当前显示的图层
        List<HObject> _currTaskImgs = null;
        JFDLAFProductRecipe _currRecipe = null;


        

        private void rbButton_ChangeTaskShow(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            _currTask = Convert.ToInt32(rb.Tag);
            if(_currICRow > -1)
            {
                return;
            }
            if (null == _currTaskImgs)
                return;
            if (_currTask < 0)
                _currTask = 0;
            else if (_currTask >= _currTaskImgs.Count)
                _currTask = 0;
            //htDetectImg.RefreshWindow(_currTaskImgs[_currTask],null,"fit");
            ShowFovImageResult();

        }

        int _FovNameIndex(string fovName)
        {
            if (_currRecipe == null)
                return -1;
            for (int i = 0; i < _currRecipe.FovCount; i++)
                if (_currRecipe.FovNames()[i] == fovName)
                    return i;
            return -1;
        }
        

        //显示图像/以及测试结果
        void ShowFovImageResult()
        {
            if (null == _currRecipe)
                return;
            if (null == _currTaskImgs)
                return;

            int idxInRetList = (_currICRow * _currRecipe.ColCount  +_currICCol) * _currRecipe.FovCount + _FovNameIndex(_currFov);
            if(null == _lstFovResults || idxInRetList >= _lstFovResults.Count)
            {
                //htDetectImg.RefreshWindow(_currTaskImgs[_currTask], null, "fit");
                return;
            }
            DlafFovDetectResult fdr = _lstFovResults[idxInRetList];
#if false //老版本算子的显示
            htDetectImg.ColorName = "green";
            bool isShowImg = fdr.WireRegion != null; //fdr.DetectIterms != null && fdr.DetectIterms.ContainsKey("WireRegions");
            if (isShowImg)
                htDetectImg.RefreshWindow(_currTaskImgs[_currTask], fdr.WireRegion, "fit");


            htDetectImg.ColorName = "yellow";

            HObject OKRegion = null;
            HOperatorSet.GenEmptyRegion(out OKRegion);
            if (fdr.DetectIterms != null)
            {
                foreach (KeyValuePair<string, HObject> kvRegion in fdr.DetectIterms)
                {
                    if (kvRegion.Key == "WireRegions")
                        continue;
                    if (null != kvRegion.Value)
                        HOperatorSet.ConcatObj(OKRegion, kvRegion.Value, out OKRegion);
                }
                if (!isShowImg)
                {
                    htDetectImg.RefreshWindow(_currTaskImgs[_currTask], OKRegion, "fit");
                    isShowImg = true;
                }
                else
                    htDetectImg.DispRegion(OKRegion);
            }
            //显示ErrorRegion
            htDetectImg.ColorName = "red";
            HObject hoFailedRegion = null;
            HOperatorSet.GenEmptyRegion(out hoFailedRegion);
            if (null != fdr.DiesErrorRegions)
                foreach (HObject[] hoRegs in fdr.DiesErrorRegions)
                    if (null != hoRegs)
                        foreach (HObject hoReg in hoRegs)
                            if (null != hoReg)
                                HOperatorSet.ConcatObj(hoFailedRegion, hoReg, out hoFailedRegion);
            if (null != hoFailedRegion)
            {
                if (!isShowImg)
                    htDetectImg.RefreshWindow(_currTaskImgs[_currTask], hoFailedRegion, "fit");
                else
                    htDetectImg.DispRegion(hoFailedRegion);
            }
#endif
            bool _isShowAllItem = chkShowAllItems.Checked;
            htDetectImg.RefreshWindow(_currTaskImgs[_currTask], null, "fit");
            if (!fdr.IsDetectSuccess || null == fdr.DieInspectResults)
                return;

            HObject errRegion = null;//所有错误区域
            HOperatorSet.GenEmptyRegion(out errRegion);


            HObject bondRegion = null;
            HOperatorSet.GenEmptyRegion(out bondRegion);
            HObject wireRegion = null;
            HOperatorSet.GenEmptyRegion(out wireRegion);
            HObject epoxyRegion = null;
            HOperatorSet.GenEmptyRegion(out epoxyRegion);


            foreach (InspectResultItem[] dieItems in fdr.DieInspectResults)
                foreach(InspectResultItem item in dieItems)
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
                htDetectImg.ColorName = "green";
                htDetectImg.DispRegion(wireRegion);
                htDetectImg.ColorName = "yellow";
                htDetectImg.DispRegion(bondRegion);
                htDetectImg.ColorName = "blue";
                htDetectImg.DispRegion(epoxyRegion);
            }
            htDetectImg.ColorName = "red";
            htDetectImg.DispRegion(errRegion);




        }


        /// <summary>
        /// 将错误信息更新到列表中
        /// </summary>
        /// <param name="fdr"></param>
        void UpdateErrorDataGrid(DlafFovDetectResult fdr)
        {
            bool _isShowAllItems = chkShowAllItems.Checked;
            lbSelectedInfo.Text = "选中单元: Row=" + (fdr.ICRow + 1) + " Col=" + (fdr.ICCol+1) + " FovName:" + fdr.FovName + " 结果:" + (fdr.IsFovOK ? "合格" : "不合格");
            dgvInspectResultsInFov.Rows.Clear();
            
            int dieIndex = 0;
            int dieCount = fdr.CurrColCount * fdr.CurrRowCount;
            if (dieCount <= 0)//Fov只是Die的一部分
                dieCount = 1;
            //表格行 ： Die/检测项/检测结果/检测标准数据/检测结果数据/备注
            for (dieIndex = 0; dieIndex < dieCount; dieIndex ++)
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

                    DataGridViewTextBoxCell dieRefVelCell = new DataGridViewTextBoxCell();
                    dieRefVelCell.Value = "---";
                    dieRefVelCell.Style.ForeColor = Color.Red;
                    row.Cells.Add(dieRefVelCell);


                    DataGridViewTextBoxCell dieDetectVelCell = new DataGridViewTextBoxCell();
                    dieDetectVelCell.Value = "---";
                    dieDetectVelCell.Style.ForeColor = Color.Red;
                    row.Cells.Add(dieDetectVelCell);

                    DataGridViewTextBoxCell dieRemarksCell = new DataGridViewTextBoxCell();
                    string rowColInfo = "Die:" +(dieIndex + 1) + " 行:" + (dieIndex / dieCount + 1) + " 列:" + (dieIndex % dieCount + 1);
                    dieRemarksCell.Value = rowColInfo;
                    dieRemarksCell.Style.ForeColor = Color.Red;
                    row.Cells.Add(dieRemarksCell);


                    dgvInspectResultsInFov.Rows.Add(row);
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
                        string rowColInfo = "Die:" + (dieIndex + 1) +  " 行:" + (dieIndex / dieCount + 1) + " 列:" + (dieIndex % dieCount + 1);
                        dieRemarksCell.Value = rowColInfo;
                        row.Cells.Add(dieRemarksCell);


                        dgvInspectResultsInFov.Rows.Add(row);
                    }
                }
            }


        }

        /// <summary>
        /// 更新Map控件
        /// </summary>
        /// <param name="fdr"></param>
        void UpdateDetectMapping(DlafFovDetectResult fdr)
        {
            int colInMap = fdr.ICCol * _currRecipe.FovCount + _FovNameIndex(fdr.FovName);
            mapDetectCells.SetDieState(fdr.ICRow, colInMap, fdr.IsFovOK ? "合格" : "不合格");
        }

        void UpdateTaskNameButtons(DlafFovDetectResult fdr)
        {
            if (null == _currRecipe)
                return;
            string[] taskNames = fdr.TaskNames;
            int i = 0;
            for (i = 0; i < taskNames.Length; i++)
                if (i < _rbTaskNames.Length)
                {
                    _rbTaskNames[i].Visible = true;
                    _rbTaskNames[i].Enabled = true;
                    _rbTaskNames[i].Text = taskNames[i];
                }
            for(; i < _rbTaskNames.Length;i++)
                _rbTaskNames[i].Visible = false;
        }

        private void chkShowAllItems_CheckedChanged(object sender, EventArgs e)
        {
            if (null == _currRecipe)
                return;
            int frIndex = (_currICRow * _currRecipe.ColCount + _currICCol) * _currRecipe.FovCount + _FovNameIndex(_currFov);
            if (frIndex < _lstFovResults.Count)
                UpdateErrorDataGrid(_lstFovResults[frIndex]);
            ShowFovImageResult();
        }
    }
}
