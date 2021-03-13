/****************************************************************************/
/*  File Name   :   Utils.cs                                                */
/*  Brief       :   Utils funtions including Log and message, etc.          */
/*  Date        :   2017/8/2                                                */
/*  Author      :   Tongqing CHEN	                                        */
/****************************************************************************/
using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;
using HT_Lib;

namespace Utils
{


    enum LogInfoType
    {
        TYPE_INFO = 0,
        TYPE_DEBUG,
        TYPE_ERROR,
    }
    /// <summary>
    /// 自定义InputBox 
    /// </summary>
    public class InputBox : Form
    {
        private System.Windows.Forms.TextBox txtData;
        private SelfControl.ColorButton btnCancel;
        private SelfControl.ColorButton btnConfirm;
        private System.Windows.Forms.Label lblInfo;
        private System.ComponentModel.Container components = null;
        public Boolean confirmed = false;

        public InputBox()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing) 
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

            }
            base.Dispose(disposing);
        }
        private void txtData_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    BtnConfirm_Click(null, null);
                    break;
            }
        }
        private void InitializeComponent()
        {
            this.txtData = new System.Windows.Forms.TextBox();
            this.btnCancel = new SelfControl.ColorButton();
            this.btnConfirm = new SelfControl.ColorButton();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtData.Location = new System.Drawing.Point(56, 84);
            this.txtData.Name = "txtData";
            this.txtData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtData_KeyDown);
            this.txtData.Size = new System.Drawing.Size(263, 27);
            this.txtData.TabIndex = 19;
            this.txtData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnCancel
            // 
            this.btnCancel.BorderColor = System.Drawing.Color.Empty;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.GlassEffect = true;
            this.btnCancel.Location = new System.Drawing.Point(257, 132);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius_All = 5;
            this.btnCancel.Radius_BottomLeft = 5;
            this.btnCancel.Radius_BottomRight = 5;
            this.btnCancel.Radius_TopLeft = 5;
            this.btnCancel.Radius_TopRight = 5;
            this.btnCancel.Size = new System.Drawing.Size(62, 26);
            this.btnCancel.TabIndex = 45;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BorderColor = System.Drawing.Color.Empty;
            this.btnConfirm.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConfirm.GlassEffect = true;
            this.btnConfirm.Location = new System.Drawing.Point(56, 132);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Radius_All = 5;
            this.btnConfirm.Radius_BottomLeft = 5;
            this.btnConfirm.Radius_BottomRight = 5;
            this.btnConfirm.Radius_TopLeft = 5;
            this.btnConfirm.Radius_TopRight = 5;
            this.btnConfirm.Size = new System.Drawing.Size(62, 26);
            this.btnConfirm.TabIndex = 46;
            this.btnConfirm.Text = "确认";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblInfo.Location = new System.Drawing.Point(51, 41);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(214, 23);
            this.lblInfo.TabIndex = 47;
            this.lblInfo.Text = "输入重命名或新建后的名字";
            // 
            // InputBoxUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 171);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtData);
            this.Name = "InputBoxUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputBox";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        //对键盘进行响应
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            confirmed = true;
            this.Close();
        }

        public void SetDisplayInfo(String info = "")
        {
            if (info.Trim() != string.Empty)
                this.lblInfo.Text = info;
        }
        public String GetInputData()
        {
            return this.txtData.Text.Trim();
        }
    }




    class MSG
    {

           


        public static Boolean Inputbox(String title, String info, out String data)
        {
            InputBox inputbox = new InputBox();
            inputbox.Text = title;
            inputbox.FormBorderStyle = FormBorderStyle.None;
            inputbox.SetDisplayInfo(info);
            inputbox.ShowDialog();
            data = inputbox.GetInputData();
            return inputbox.confirmed;
        }
    }

    /// <summary>
    /// 基类，含有保存和读取操作（默认保存public成员，且类型为double, int, Boolean, String类型）
    /// </summary>
    public class Base
    {
        protected String paraFile;// 所存储文件路径
        protected String paraTable;// 存储的表名
        private SQLiteConnection sqlCon;    //连接

        protected int errCode = 0;
        protected String errString = "No error";

        public String GetLastErrorString()
        {
            return String.Format("[{0}]{1}", errCode, errString);
        }
        /// <summary>
        /// 检查返回值，并将返回值赋给errcode
        /// </summary>
        /// <param name="UserFunc"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <returns></returns>
        public bool CheckErr(Func<int, double, int> UserFunc, int par1, double par2)
        {
            return ((errCode = UserFunc(par1, par2)) < 0) ? false : true;
        }
        /// <summary>
        /// 检查返回值，并将返回值赋给errcode
        /// </summary>
        /// <param name="UserFunc"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <returns></returns>
        public  bool CheckErr(Func<int, int, int> UserFunc, int par1, int par2)
        {
            return ((errCode = UserFunc(par1, par2)) < 0) ? false : true;
        }
       
        /// <summary>
        /// 检查返回值，并将返回值赋给errcode
        /// </summary>
        /// <param name="UserFunc"></param>
        /// <param name="par1"></param>
        /// <returns></returns>
        public bool CheckErr(Func<int, int> UserFunc, int par1)
        {
            return ((errCode = UserFunc(par1)) < 0) ? false : true;
        }
        /// <summary>
        /// 检查返回值，并将返回值赋给errcode
        /// </summary>
        /// <param name="UserFunc"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <param name="par3"></param>
        /// <returns></returns>
        public bool CheckErr(Func<int, int, double,int> UserFunc, int par1, int par2,double par3)
        {
            return ((errCode = UserFunc(par1, par2,par3)) < 0) ? false : true;
        }
        /// <summary>
        /// 检查返回值，并将返回值赋给errcode
        /// </summary>
        /// <param name="UserFunc"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <param name="par3"></param>
        /// <returns></returns>
        public bool CheckErr(Func<int, double, double, int> UserFunc, int par1, double par2, double par3)
        {
            return ((errCode = UserFunc(par1, par2, par3)) < 0) ? false : true;
        }
        /// <summary>
        /// 检查返回值，并将返回值赋给errcode
        /// </summary>
        /// <param name="UserFunc"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <param name="par3"></param>
        /// <param name="par4"></param>
        /// <returns></returns>
        public bool CheckErr(Func<int,int,int,double,int> UserFunc, int par1, int par2, int par3,double par4)
        {
            return ((errCode = UserFunc(par1, par2, par3,par4)) < 0) ? false : true;
        }
        //protected int runMode = 0;//0-离线 1-demo 2-online 

        //public int RunMode { set { runMode = value; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="para_file"></param>
        /// <param name="para_table"></param>
        public Base(String para_file, String para_table)//构造并初始化
        {
            paraFile = para_file;
            paraTable = para_table;
            //sqlCon = new SQLiteConnection(@"DATA SOURCE=" + para_file + @"; VERSION=3");
            //因为产品之前传的para_file是空的，后面在product.ConfigParaFile(para_file)中配置
            //所以在read save中new connection
        }
        //public virtual Boolean Save(object obj)
        //{
        //    Boolean ret = true;
            
        //    try
        //    {
        //        sqlCon = new SQLiteConnection(@"DATA SOURCE=" + paraFile + @"; VERSION=3");//改动
        //        if (sqlCon.State == System.Data.ConnectionState.Closed)
        //        {
        //            sqlCon.Open();
        //        }
        //        String sql = "CREATE TABLE IF NOT EXISTS " + paraTable + "(Para TEXT PRIMARY KEY NOT NULL, Value TEXT NOT NULL)";
        //        SQLiteCommand cmd = new SQLiteCommand(sql, sqlCon);
        //        cmd.ExecuteNonQuery();
        //            switch (obj.GetType().Name)
        //            {
        //                case "String":
        //                case "Int32":
        //                case "Boolean":
        //                case "double":
        //                    cmd.CommandText = String.Format("REPLACE INTO {0}(Para, Value) VALUES(@_Para, @_Value)", paraTable);//1234之类的？
        //                    cmd.Parameters.Add("_Para", System.Data.DbType.String).Value = fi.Name;
        //                    cmd.Parameters.Add("_Value", System.Data.DbType.Object).Value = obj;
        //                    cmd.ExecuteNonQuery();
        //                    break;
        //            }
        //        sqlCon.Close();
        //    }
        //    catch (Exception exp)
        //    {
        //        errCode = -1;
        //        errString = exp.ToString();
        //        sqlCon.Close();
        //        ret = false;
        //    }
        //    return ret;
        //}
        /// <summary>
        /// 保存参数，仅仅保存Public属性的int, Boolean, String, double四种类型参数
        /// </summary>
        /// <returns>返回bool类型表示成功或失败，如果保存失败可以通过GetErrorString获取错误信息</returns>
        public virtual Boolean Save()
        {
            Boolean ret = true;
            try
            {
                sqlCon = new SQLiteConnection(@"DATA SOURCE=" + paraFile + @"; VERSION=3");//改动
                if (sqlCon.State == System.Data.ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                String sql = "CREATE TABLE IF NOT EXISTS " + paraTable + "(Para TEXT PRIMARY KEY NOT NULL, Value TEXT NOT NULL)";
                SQLiteCommand cmd = new SQLiteCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                FieldInfo[] infos = this.GetType().GetFields();//type.GetField
                foreach (FieldInfo fi in infos)
                {
                    switch (fi.FieldType.Name)
                    {
                        case "String":
                        case "Int32":
                        case "Boolean":
                        case "double":
                            cmd.CommandText = String.Format("REPLACE INTO {0}(Para, Value) VALUES(@_Para, @_Value)", paraTable);//1234之类的？
                            cmd.Parameters.Add("_Para", System.Data.DbType.String).Value = fi.Name;
                            cmd.Parameters.Add("_Value", System.Data.DbType.Object).Value = fi.GetValue(this);
                            cmd.ExecuteNonQuery();
                            break;
                    }
                }
                sqlCon.Close();
            }
            catch (Exception exp)
            {
                errCode = -1;
                errString = exp.ToString();
                sqlCon.Close();
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// 读取参数，如果数据库中不含该参数则直接报错退出
        /// </summary>
        /// <returns>返回bool类型表示成功或失败，如果保存失败可以通过GetErrorString获取错误信息</returns>
        public virtual Boolean Read()
        {
            Boolean ret = true;
            try
            {
                sqlCon = new SQLiteConnection(@"DATA SOURCE=" + paraFile + @"; VERSION=3");//改动
                if (sqlCon.State == System.Data.ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                SQLiteDataReader reader;
                FieldInfo[] infos = this.GetType().GetFields();
                foreach (FieldInfo fi in infos)
                {
                    cmd.CommandText = String.Format("SELECT * FROM {0} WHERE [Para] = '{1}'", paraTable, fi.Name);//懂
                    reader = cmd.ExecuteReader();
                    if (!reader.HasRows)   //确保所有参数被赋值成功
                    {
                        //ret = false;
                        errString = String.Format("数据库中没有参数[{0}]", fi.Name);
                    }
                    else
                    {
                        reader.Read();
                        switch (fi.FieldType.Name)
                        {
                            case "Int32":
                                fi.SetValue(this, Convert.ToInt32(reader["Value"]));
                                break;
                            case "double":
                                fi.SetValue(this, Convert.ToDouble(reader["Value"]));
                                break;
                            case "Boolean":
                                fi.SetValue(this, Convert.ToBoolean(Convert.ToInt32(reader["Value"])));
                                break;
                            case "String":
                                fi.SetValue(this, Convert.ToString(reader["Value"]));
                                break;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception exp)
            {
                errCode = -1;
                errString = exp.ToString();
                ret = false;
            }
            sqlCon.Close();
            return ret;
        }
    }
}
