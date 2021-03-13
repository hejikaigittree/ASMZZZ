using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using System.IO;
using HT_Lib;
using HalconDotNet;
using JFHub;
using JFInterfaceDef;
using HTHalControl;


namespace DLAF
{
    public partial class FrmProduct : JFRealtimeUI
    {
        public static FrmProduct Instance;
        public JFDLAFProductRecipe _operation;

        public FrmProduct()
        {
            InitializeComponent();
            htWindow1.SetTablePanelVisible(false);
            htWindow1.SetStatusStripVisible(false);
            htWindow1.SetStripEnable(false);
            htWindow2.SetTablePanelVisible(false);
            htWindow2.SetStatusStripVisible(false);
            htWindow2.SetStripEnable(false);
            Instance = this;
        }

        public void SetRecipe(JFDLAFProductRecipe operation)
        {
            _operation = operation;
            if (Created)
                SetupUI();
        }

        public void SetupUI()
        {
            var pdt = ((JFDLAFRecipeManager)(JFHubCenter.Instance.RecipeManager)).GetProductList();
            lbxProductCategory.DataSource = pdt;
            prgProductMagzine.SelectedObject = _operation;
            if(_operation!=null)
                RefreshPdtUI();
        }

        public void RefreshPdtUI()
        {
            prgProductMagzine.Refresh();
            HTLog.Info(String.Format("产品名：{0}", _operation.ID));
            //FormJobs.Instance.label5.Invoke(new Action(() => { FormJobs.Instance.label5.Text = "当前产品:" + _operation.ID + "  流程状态:待机"; }));
            groupBox1.Text = "当前料片：" + _operation.ID;
            lblCurrentProd.Text = "当前产品：" + _operation.ID;

            if (File.Exists((string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + _operation.ID + "\\" + "ClipImage.tiff"))
            {
                FileStream fileStream = new FileStream((string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + _operation.ID + "\\" + "ClipImage.tiff", FileMode.Open, FileAccess.Read);
                int byteLength = (int)fileStream.Length;
                byte[] fileBytes = new byte[byteLength];
                fileStream.Read(fileBytes, 0, byteLength);
                //文件流关閉,文件解除锁定
                fileStream.Close();
                pictureBox1.BackgroundImage = Image.FromStream(new MemoryStream(fileBytes));
            }
            else
            {
                pictureBox1.BackgroundImage = null;
            }
            HObject imgActivePdt = null;
            if (File.Exists((string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + _operation.ID + "\\" + "frameMapImg.tiff"))
            {
                HOperatorSet.ReadImage(out imgActivePdt, (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + _operation.ID + "\\" + "frameMapImg.tiff");
                ShowImage(htWindow1, imgActivePdt, null);
            }
            else
            {
                imgActivePdt = null;
                ShowImage(htWindow1, imgActivePdt, null);
            }
        }

        /// <summary>
        /// 加载产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnLoadProduct_Click(object sender, EventArgs e)
        {
            string Errstring = "";
            if (lbxProductCategory.SelectedIndex == -1) return;
            btnLoadProduct.Enabled = false;

            string pdt = null;
            pdt = lbxProductCategory.SelectedValue.ToString();
            
            btnLoadProduct.Enabled = true;
            Form_Wait.ShowForm();
            await Task.Run(new Action(() =>
            {
            try
            {
                //需要加载当前产品相关的参数
                _operation = JFHubCenter.Instance.RecipeManager.GetRecipe("Product", pdt) as JFDLAFProductRecipe;
                if(_operation==null)
                {
                    HTUi.TipError("Recipe Manager中不存在配方:"+ pdt);
                    return;
                }

                if (Errstring == "")
                {
                    HTLog.Info("加载产品完成.");
                    HTUi.TipHint("加载产品完成");
                    if (_operation.MgzIdx == -1)
                    {
                        HTLog.Error("注意：当前产品未选择料盒型号，开始流程需要选择料盒型号.");
                    }
                }
                else
                {
                    HTLog.Error("加载产品失败！！！" + Errstring);
                    HTUi.TipError("加载产品失败");
                    HTUi.PopWarn("产品加载失败\n" + Errstring);
                }
                }
                catch (Exception EXP)
                {
                    HTLog.Error(String.Format("{0}加载新产品失败\n", EXP.ToString()));
                    HTUi.TipError("加载新产品失败");
                }
            }));
            if (_operation != null)
                SetupUI();
            JFHubCenter.Instance.SystemCfg.SetItemValue("CurrentID", pdt);
            JFHubCenter.Instance.SystemCfg.Save();
            Form_Wait.CloseForm();
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var pdt = lbxProductCategory.SelectedValue;
            if (!HTUi.PopYesNo("是否删除选中产品？"))
            {
                return;
            }
            if (!DeleteProduct((string)pdt))
            {
                HTUi.PopWarn("不能删除已激活的产品！");
            }
            //删除Recipe Manager中配方信息
            JFHubCenter.Instance.RecipeManager.RemoveRecipe("Product", (string)pdt);
            JFHubCenter.Instance.RecipeManager.RemoveRecipe("Box", (string)pdt);
            JFHubCenter.Instance.RecipeManager.Save();

            lbxProductCategory.DataSource = GetProductList();  
        }

        /// <summary>
        /// 创建产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateProduct_Click(object sender, EventArgs e)
        {

            String newfile = "";
            if (!MSG.Inputbox("Copy&New", "请输入新的产品名", out newfile))
            {
                return;
            }

            if (newfile == string.Empty)
            {
                HTUi.PopWarn("命名错误，名字不能为空");
                return;
            }
            //新增Product Recipe到recipe Dictionary中
            JFDLAFProductRecipe jFDLAFProductRecipe = new JFDLAFProductRecipe();
            if (JFHubCenter.Instance.RecipeManager.GetRecipe("Product", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"))!=null)
            {
                jFDLAFProductRecipe = ((JFDLAFProductRecipe)JFHubCenter.Instance.RecipeManager.GetRecipe("Product", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"))).Clone();
            }

            jFDLAFProductRecipe.ID = newfile;
            jFDLAFProductRecipe.Categoty = "Product";
            jFDLAFProductRecipe.SaveParamsToCfg();
            JFHubCenter.Instance.RecipeManager.AddRecipe(jFDLAFProductRecipe.Categoty, jFDLAFProductRecipe.ID, jFDLAFProductRecipe as IJFRecipe);


            ////新增Box Recipe到recipe Dictionary中
            JFDLAFBoxRecipe jFDLAFBoxRecipe = new JFDLAFBoxRecipe();
            if (JFHubCenter.Instance.RecipeManager.GetRecipe("Box", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")) != null)
            {
                jFDLAFBoxRecipe = ((JFDLAFBoxRecipe)JFHubCenter.Instance.RecipeManager.GetRecipe("Box", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"))).Clone();
            }
            jFDLAFBoxRecipe.MagezineBox = jFDLAFProductRecipe.MagezineBox;
            jFDLAFBoxRecipe.MgzIdx = jFDLAFProductRecipe.MgzIdx;
            jFDLAFBoxRecipe.ID = newfile;
            jFDLAFBoxRecipe.Categoty = "Box";
            jFDLAFBoxRecipe.SaveParamsToCfg();
            JFHubCenter.Instance.RecipeManager.AddRecipe(jFDLAFBoxRecipe.Categoty, jFDLAFBoxRecipe.ID, jFDLAFBoxRecipe as IJFRecipe);

            //此处后面需要保存
            JFHubCenter.Instance.RecipeManager.Save();

            var pdtlist = GetProductList();
            foreach (var pdt in pdtlist)
            {
                if (pdt == newfile)
                {
                    HTUi.PopWarn("无法新建，当前产品已存在");
                    return;
                }
            }
            string souce = (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID");
            if (!CreateProduct(souce, newfile))
            {
                HTUi.PopWarn("产品创建失败!");
                return;
            }
            HTLog.Info("产品创建成功!");
            HTUi.TipHint("产品创建成功!");
            lbxProductCategory.DataSource = GetProductList();
            return;
        }

        /// <summary>
        /// 重命名产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRename_Click(object sender, EventArgs e)
        {
            string souce = (string)lbxProductCategory.SelectedValue;
            if (souce == (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"))
            {
                HTUi.PopError("当前产品名称不可修改");
                return;
            }

            String newname = "";
            if (!MSG.Inputbox("new", "rename", out newname))
            {
                return;
            }

            if (newname == string.Empty)
            {
                HTUi.PopWarn("命名错误，名字不能为空");
                return;
            }
            var pdtlist = GetProductList();
            foreach (var pdt in pdtlist)
            {
                if (pdt == newname)
                {
                    HTUi.PopWarn("命名冲突，当前产品已存在");
                    return;
                }
            }

            //重新命名Recipe manager中recipe ID
            JFDLAFProductRecipe jFDLAFProductRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Product", (string)souce) as JFDLAFProductRecipe;
            if(jFDLAFProductRecipe==null)
            {
                HTUi.PopWarn("Recipe Manager中不存在recipe："+ souce);
                return;
            }
            jFDLAFProductRecipe.Categoty = "Product";
            jFDLAFProductRecipe.ID = newname;

            JFDLAFBoxRecipe jFDLAFBoxRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Box", (string)souce) as JFDLAFBoxRecipe;
            if(jFDLAFBoxRecipe==null)
            {
                HTUi.PopWarn("Recipe Manager中不存在recipe：" + souce);
                return;
            }
            jFDLAFBoxRecipe.Categoty = "Box";
            jFDLAFBoxRecipe.ID = newname;

            JFHubCenter.Instance.RecipeManager.AddRecipe(jFDLAFProductRecipe.Categoty, jFDLAFProductRecipe.ID, jFDLAFProductRecipe as IJFRecipe);
            JFHubCenter.Instance.RecipeManager.AddRecipe(jFDLAFBoxRecipe.Categoty, jFDLAFBoxRecipe.ID, jFDLAFBoxRecipe as IJFRecipe);
            JFHubCenter.Instance.RecipeManager.RemoveRecipe("Product", souce);
            JFHubCenter.Instance.RecipeManager.RemoveRecipe("Box", souce);
            JFHubCenter.Instance.RecipeManager.Save();

            ChangeProdcutName(newname, souce);
            HTUi.TipHint("重命名成功!");
            lbxProductCategory.DataSource = GetProductList();
            return;
        }

        /// <summary>
        /// 保存产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSave_Click(object sender, EventArgs e)
        {
            Form_Wait.ShowForm();
            await Task.Run(new Action(() =>
            {
                //需要保存当前产品的相关参数
                if(_operation!=null)
                {
                    JFDLAFBoxRecipe jFDLAFBoxRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Box", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")) as JFDLAFBoxRecipe;
                    jFDLAFBoxRecipe.MagezineBox = _operation.MagezineBox;
                    jFDLAFBoxRecipe.MgzIdx = _operation.MgzIdx;
                    jFDLAFBoxRecipe.SaveParamsToCfg();
                    _operation.SaveParamsToCfg();

                    JFHubCenter.Instance.RecipeManager.RemoveRecipe("Product", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    JFHubCenter.Instance.RecipeManager.RemoveRecipe("Box", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));

                    JFHubCenter.Instance.RecipeManager.AddRecipe(_operation.Categoty, _operation.ID, _operation as IJFRecipe);
                    JFHubCenter.Instance.RecipeManager.AddRecipe("Box", _operation.ID, jFDLAFBoxRecipe as IJFRecipe);
                }

                JFHubCenter.Instance.RecipeManager.Save();
                if (_operation != null)
                {
                    if (_operation.MgzIdx == -1)
                    {
                        HTLog.Error("注意：当前产品未选择料盒型号，开始流程需要选择料盒型号.");
                        return;
                    }
                }
            }));
            Form_Wait.CloseForm();
        }

        /// <summary>
        /// 配置轨道宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFixTrack_Click(object sender, EventArgs e)
        {
            switch (HTUi.PopSelect("请在以下选项中选择配置导轨方式。","直接配置","轴调试助手","取消"))
            {
                case 0:
                   //保存为选中产品的轨道宽度

                    break;
                case 1:
                    if (HTM.LoadUI() < 0)
                    {
                        HTUi.PopError("打开轴调试助手界面失败");
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 选中产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbxProductCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxProductCategory.SelectedIndex == -1) return;
            if (lbxProductCategory.SelectedItem.ToString() == "")
            {
                groupBox2.Text = "选中料片：" + "无";
                lblSelectProd.Text = "选中产品：" + "无";
            }
            else
            {
                HObject imgSelectPdt = null;
                lblSelectProd.Text = "选中产品：" + lbxProductCategory.SelectedItem.ToString();
                string selectPdt = lbxProductCategory.SelectedItem.ToString();
                string selectPdtPath = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + selectPdt;
                if (File.Exists(selectPdtPath + "\\" + "ClipImage.tiff"))
                {
                    FileStream fileStream = new FileStream(selectPdtPath + "\\" + "ClipImage.tiff", FileMode.Open, FileAccess.Read);
                    int byteLength = (int)fileStream.Length;
                    byte[] fileBytes = new byte[byteLength];
                    fileStream.Read(fileBytes, 0, byteLength);
                    //文件流关閉,文件解除锁定
                    fileStream.Close();
                    pictureBox2.BackgroundImage = Image.FromStream(new MemoryStream(fileBytes));
                }
                else
                {
                    pictureBox2.BackgroundImage = null;
                }
                if (File.Exists(selectPdtPath + "\\" + "frameMapImg.tiff"))
                {
                    HOperatorSet.ReadImage(out imgSelectPdt, selectPdtPath + "\\" + "frameMapImg.tiff");
                    groupBox2.Text = "选中料片：" + lbxProductCategory.SelectedItem.ToString();
                    ShowImage(htWindow2, imgSelectPdt, null);
                }
                else
                {
                    imgSelectPdt = null;
                    ShowImage(htWindow2, imgSelectPdt, null);
                }
            }
        }

        /// <summary>
        /// 料盒管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBoxManager_Click(object sender, EventArgs e)
        {
            Form_BoxManage frm = Form_BoxManage.Instance;
            if (frm == null || frm.IsDisposed)
            {
                frm = new Form_BoxManage();
                int SH = Screen.PrimaryScreen.Bounds.Height;
                int SW = Screen.PrimaryScreen.Bounds.Width;
                frm.Show();
                frm.Location = new Point((SW - frm.Size.Width) / 2, (SH - frm.Size.Height) / 2);
            }
            else
            {
                frm.Activate();
            }
        }

        private void frmProduct_Enter(object sender, EventArgs e)
        {
            prgProductMagzine.Refresh();
        }

        private void FrmProduct_Load(object sender, EventArgs e)
        {
            SetupUI();
        }



        #region 产品增删改查
        /// <summary>
        /// </summary>
        /// <returns>包含目录的指针list</returns>
        public List<String> GetProductList()
        {
            List<String> list = new List<string>();
            DirectoryInfo Path = new DirectoryInfo((string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]));
            DirectoryInfo[] Dir = Path.GetDirectories();
            foreach (DirectoryInfo d in Dir)
            {
                list.Add(d.Name);
            }
            //read product directory to fetch the list
            return list;
        }

        /// <summary>
        /// </summary>
        /// <param name="souceFile">复制源文件夹</param>
        /// <param name="destinationFile">目标文件夹</param>
        /// <returns>true 创建成功；false 创建失败</returns>
        public Boolean CreateProduct(String souceFile, String destinationFile)
        {
            string soucePath = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + souceFile;
            string destinationPath = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + destinationFile;
            try
            {
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                //子文件夹
                foreach (string sub in Directory.GetDirectories(soucePath))
                {
                    CreateProduct(sub + "\\", destinationPath + Path.GetFileName(sub) + "\\");
                }
                //文件
                foreach (string file in Directory.GetFiles(soucePath))
                {
                    File.Copy(file, destinationPath + "\\" + Path.GetFileName(file), true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 功能：删除产品
        /// </summary>
        /// <param name="pdt_name">产品名</param>
        /// <returns></returns>
        public Boolean DeleteProduct(String pdt_name)
        {
            if (pdt_name == (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"))
            {
                return false;
            }
            string path = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + pdt_name;
            DirectoryInfo Directory = new DirectoryInfo(path);
            Directory.Delete(true);//所有
            return true;
        }

        /// <summary>
        /// 功能：改变产品名
        /// </summary>
        /// <param name="destinationFile"></param>
        /// <param name="souceFile"></param>
        /// <returns></returns>
        public Boolean ChangeProdcutName(String destinationFile, String souceFile)
        {
            string soucePath = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + souceFile;
            string destinationPath = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + destinationFile;
            try
            {
                if (Directory.Exists(soucePath))
                {
                    Directory.Move(soucePath, destinationPath);
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        private delegate void ShowImageDelegate(HTWindowControl htWindow, HObject image, HObject region);
        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="htWindow">图像视窗</param>
        /// <param name="image">图像数据</param>
        /// <param name="region">区域数据</param>
        public void ShowImage(HTWindowControl htWindow, HObject image, HObject region)
        {
            if (htWindow.InvokeRequired)
            {
                htWindow.Invoke(new ShowImageDelegate(ShowImage), new object[] { htWindow, image, region });
            }
            else
            {
                lock (htWindow)
                {
                    htWindow.ColorName = "yellow";
                    htWindow.SetInteractive(false);
                    if (htWindow.Image == null)
                        htWindow.RefreshWindow(image, region, "fit");
                    else
                        htWindow.RefreshWindow(image, region, "fit");
                    htWindow.SetInteractive(true);
                    htWindow.ColorName = "green";
                }
            }
        }
        #endregion

    }
}
