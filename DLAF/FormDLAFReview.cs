using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LFAOIReview;
using JFUI;

namespace DLAF
{

    /// <summary>
    /// 检测数据复判窗口
    /// </summary>
    public partial class FormDLAFReview : Form
    {
        public FormDLAFReview()
        {
            InitializeComponent();
        }

        private void FormReview_Load(object sender, EventArgs e)
        {
            Form_Review form_Review = null;
            if (!string.IsNullOrEmpty(_dbFilePath))
            {
                DataShow _ds = new DataShow();
                _ds.ShowInspectData(_dbFilePath, _pieceID, _lotID, _picFolder, _recipeID);
                form_Review = new Form_Review(_ds);
            }
           else
                form_Review = new Form_Review();
            form_Review.TopLevel = false;
            form_Review.Dock = DockStyle.Fill;
            Controls.Add(form_Review);
            form_Review.Visible = true;
            form_Review.TopMost = true;
        }

        
        string _recipeID = null;
        string _lotID = null;
        string _pieceID = null;
        string _dbFilePath = null;
        string _picFolder = null;
        /// <summary>
        ///  设置复判参数
        /// </summary>
        /// <param name="recipeID">产品类别</param>
        /// <param name="lotID">批次号</param>
        /// <param name="pieceID">料片ID</param>
        /// <param name="dbFilePath">数据库文件路径</param>   //@"C:\Users\J0021\Desktop\review\喷墨标定专用.db";//
        /// <param name="picFolder">图片文件夹</param>       //App.obj_Vision.imageFolder + "\\" + "Result";
        public void SetReviewParam(string recipeID,string lotID,string pieceID,string dbFilePath,string picFolder)
        {
            //_ds.DbFilePath = dbFilePath;
            //_ds.imageDirectory = picFolder;
            //_ds.product = recipeID;
            //_ds.lotName = lotID;
            _recipeID = recipeID;
            _lotID = lotID;
            _pieceID = pieceID;
            _dbFilePath = dbFilePath;
            _picFolder = picFolder;
        }

        private void FormDLAFReview_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(0!= JFSelectTipsUI.ShowDialog("确认操作",new string[] { "已保存复判结果","重新复判"},0,-1))
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
