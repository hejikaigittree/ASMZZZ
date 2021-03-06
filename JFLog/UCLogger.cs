using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using JFInterfaceDef;

namespace JFLog
{
    public partial class UCLogger : UserControl
    {
        private delegate void DisplayDateGridView(System.Windows.Forms.DataGridView dataGridView, DateTime dateTime, JFLogLevel level, string info);
        private delegate void DisplayComboBox(System.Windows.Forms.ComboBox comboBox);
        private delegate void DisplayButton(System.Windows.Forms.Button button, bool flag);

        private string comboBoxvalue = string.Empty;

        /// <summary>
        /// SQLite对象实例化
        /// </summary>
        private JFSQLiteDB jFSQLiteDB = new JFSQLiteDB();
        private Thread threadSelect;
        private Thread threadTxtOut;
        /// <summary>
        /// 保存Log查询信息
        /// </summary>
        private DataTable dt = new DataTable();

        private UCLogger _ui;

        /// <summary>
        /// 数据库建立连接信息
        /// </summary>
        private static string connectstring = "Data Source=" + System.Environment.CurrentDirectory + "\\HTTech.db";
        /// <summary>
        /// 该DataTabel最后一条纪录的Index
        /// </summary>
        private int nMax = 0;
        /// <summary>
        /// Page的数量
        /// </summary>
        private int pageCount = 0;
        /// <summary>
        /// 当前的页码
        /// </summary>
        private int pageCurrent = 1;
        /// <summary>
        /// 当前纪录的Index
        /// </summary>
        private int nCurrent = 0;
        /// <summary>
        /// 当前页开始的Index
        /// </summary>
        private int nStartPos = 0;
        /// <summary>
        /// 当前页结束的Index
        /// </summary>
        private int nEndPos = 0;
        /// <summary>
        /// 控件宽度
        /// </summary>
        private float X = 0;
        /// <summary>
        /// 控件高度
        /// </summary>
        private float Y = 0;

        /// <summary>
        /// 日志界面纪录上限
        /// </summary>
        private int _LimitCount = 1000;
        [DefaultValue(1000)]
        [Description("日志界面纪录上限")]
        public int LimitCount
        {
            get { return _LimitCount; }
            set { _LimitCount = value; }
        }

        /// <summary>
        /// 一页显示的最多纪录
        /// </summary>
        private int pageSize = 50;
        [DefaultValue(50)]
        [Description("一页显示的最多纪录")]
        public int PageSize
        {
            get { return pageSize; }
            set => pageSize = value;
        }

        /// <summary>
        /// 表名称
        /// </summary>
        private string _tablename = "LoggerManager";
        [DefaultValue("LoggerManager")]
        [Description("表名称")]
        public string TableName
        {
            get { return _tablename; }
            set => _tablename = value;
        }

        public UCLogger()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add UI到指定的窗体
        /// </summary>
        /// <param name="container">控件/窗体的名称</param>
        /// <param name="tablename">表名称</param>
        public void Ui(Control container,string tablename)
        {
            this._ui = null;
            this._ui = this;
            this._ui.TableName = tablename;
            this._ui.Load += new EventHandler(this.UCLogger_Load);
            this._ui.Dock = DockStyle.Fill;
            this._ui.Visible = true;
            container.Controls.Add(this._ui);
        }

        private void UCLogger_Load(object sender, EventArgs e)
        {
            //日志低等级下拉菜单初始化
            cbMinLevel.Items.Add("FATAL");
            cbMinLevel.Items.Add("ERROR");
            cbMinLevel.Items.Add("WARN");
            cbMinLevel.Items.Add("INFO");
            cbMinLevel.Items.Add("DEBUG");
            cbMinLevel.SelectedIndex = 4;

            //日志高等级下拉菜单初始化
            cbMaxLevel.Items.Add("FATAL");
            cbMaxLevel.Items.Add("ERROR");
            cbMaxLevel.Items.Add("WARN");
            cbMaxLevel.Items.Add("INFO");
            cbMaxLevel.Items.Add("DEBUG");
            cbMaxLevel.SelectedIndex = 0;

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Black;
            columnHeaderStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
            dgvLogShow.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            //dgvLogShow初始化
            dgvLogShow.Columns.Add("DateTime", "DateTime");
            dgvLogShow.Columns.Add("LogLevel", "LogLevel");
            dgvLogShow.Columns.Add("Message", "Message");

            //dgvLogShow自动列宽
            dgvLogShow.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogShow.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogShow.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            toolStripTextBoxCurrentPage.Text = "0";

            X = 559;
            Y = 428;
            setTag(this);
            this.Resize += new EventHandler(UCLogger_Resize);
            this.Height = this.Height - 1;
        }

        #region 日志的显示
        /// <summary>
        /// DataGridView控件数据更新
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="dateTime"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void DataGridViewFunction(System.Windows.Forms.DataGridView dataGridView, DateTime dateTime, JFLogLevel level, string info)
        {
            string _level = string.Empty;
            if (dataGridView.InvokeRequired)
            {
                DisplayDateGridView ss = new DisplayDateGridView(DataGridViewFunction);
                Invoke(ss, new object[] { dataGridView, dateTime, level, info});
            }
            else
            {
                int index = dataGridView.Rows.Add();
                if (index > LimitCount && index>=1)
                {
                    dataGridView.Rows.RemoveAt(0);
                    index--;
                }
                dataGridView.Rows[index].Cells[0].Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                dataGridView.Rows[index].Cells[1].Value = TransLogLevelToString(level);
                dataGridView.Rows[index].Cells[2].Value = info;
                dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.Rows.Count - 1;
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// 数据类型转换： JFLogLevel->string
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string TransLogLevelToString(JFLogLevel level)
        {
            string _level = string.Empty;
            switch (level)
            {
                case JFLogLevel.FATAL:
                    _level = "FATAL";
                    break;
                case JFLogLevel.ERROR:
                    _level = "ERROR";
                    break;
                case JFLogLevel.WARN:
                    _level = "WARN";
                    break;
                case JFLogLevel.DEBUG:
                    _level = "DEBUG";
                    break;
                case JFLogLevel.INFO:
                    _level = "INFO";
                    break;
            }
            return _level;
        }

        /// <summary>
        /// 显示日志内容
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="info">日志信息</param>
        public void ShowMsg(JFLogger.LogRecord rcd)
        {
            DataGridViewFunction(dgvLogShow, rcd.Time, rcd.Level, rcd.Info);
        }
        #endregion

        #region 日志导出成Txt
        /// <summary>
        /// 导出日志为.txt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtOut_Click(object sender, EventArgs e)
        {
            btSelect.Enabled = false;
            btOut.Enabled = false;
            threadTxtOut = new Thread(DataOutToTxt);
            threadTxtOut.Start();
        }

        /// <summary>
        /// 数据导出到Txt
        /// </summary>
        private void DataOutToTxt()
        {
            try
            {
                string _OutTxtString = string.Empty;
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        _OutTxtString += (string.Format("{0},{1},{2}", dt.Rows[i]["Time"].ToString(), dt.Rows[i]["LogLevel"].ToString(), dt.Rows[i]["Message"].ToString())+"\r\n");
                    }
                    WriteLog(_OutTxtString);
                }
                ButtonFunction(btOut, true);
                ButtonFunction(btSelect, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">写入的信息内容</param>
        private void WriteLog(string message)
        {
            try
            {
                string mTime = string.Empty;
                string mPath = string.Empty;
                string mDate = string.Empty;

                mDate = DateTime.Now.ToString("yyyy-MM-dd");
                mTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                mPath = System.Environment.CurrentDirectory + "\\HTTech_Log";

                //HTTech_Log文件夹是否存在，不存在则创建
                if (!Directory.Exists(mPath))
                {
                    Directory.CreateDirectory(mPath);
                }

                //以日期命名的文件夹是否存在，不存在则创建
                mPath = mPath + "\\" + mDate;
                if (!Directory.Exists(mPath))
                {
                    Directory.CreateDirectory(mPath);
                }

                //以时间命名的文件是否存在，不存在则创建
                if (!File.Exists(mPath + "\\" + mTime + ".txt"))
                {
                    StreamWriter sw = new StreamWriter(mPath + "\\" + mTime + ".txt", true);
                    sw.Write(message);
                    sw.Close();
                }
                else
                {
                    StreamWriter sw = new StreamWriter(mPath + "\\" + mTime + ".txt", true);
                    sw.Write(message);
                    sw.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Exception",MessageBoxButtons.OK);
            }
        }
        #endregion

        #region 日志查询
        /// <summary>
        /// 日志查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSelect_Click(object sender, EventArgs e)
        {
            pageCount = 0;
            pageCurrent = 1;
            nCurrent = 0;
            nMax = 0;
            nStartPos = 0;
            nEndPos = 0;

            btSelect.Enabled = false;
            btOut.Enabled = false;

            string connectionCreatTableString = string.Format("CREATE TABLE IF NOT EXISTS {0}(Time string,LogLevel string,Message string)", TableName);
            jFSQLiteDB.CreateDB(connectstring);
            jFSQLiteDB.CreateOrDeleteTable(connectionCreatTableString);

            threadSelect = new Thread(SelectFromSQLiteDB);
            threadSelect.Start();
        }

        /// <summary>
        /// ComboBox控件操作
        /// </summary>
        /// <param name="comboBox"></param>
        private void ComboBoxFunction(System.Windows.Forms.ComboBox comboBox)
        {
            if (comboBox.InvokeRequired)
            {
                DisplayComboBox ss = new DisplayComboBox(ComboBoxFunction);
                Invoke(ss, new object[] { comboBox });
            }
            else
            {
                comboBoxvalue = comboBox.Text;
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// Button控件操作
        /// </summary>
        /// <param name="button"></param>
        /// <param name="flag"></param>
        private void ButtonFunction(System.Windows.Forms.Button button, bool flag)
        {
            if (button.InvokeRequired)
            {
                DisplayButton ss = new DisplayButton(ButtonFunction);
                Invoke(ss, new object[] { button, flag });
            }
            else
            {
                button.Enabled = flag;
            }
        }

        /// <summary>
        /// 从数据库中查询资料
        /// </summary>
        private void SelectFromSQLiteDB()
        {
            try
            {
                string dateTimeStart = dtpStart.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string dateTimeEnd = dtpEnd.Value.ToString("yyyy-MM-dd HH:mm:ss");

                ComboBoxFunction(cbMinLevel);
                string minLevel = comboBoxvalue;

                ComboBoxFunction(cbMaxLevel);
                string maxLevel = comboBoxvalue;

                if (minLevel.IndexOf("DEBUG")<0 && minLevel.IndexOf("INFO") < 0 && minLevel.IndexOf("WARN") < 0 && 
                    minLevel.IndexOf("ERROR") < 0 && minLevel.IndexOf("FATAL") < 0)
                {
                    ButtonFunction(btSelect, true);
                    ButtonFunction(btOut, true);
                    MessageBox.Show("请选择正确的日志低等级","日志等级",MessageBoxButtons.OK);
                    return;
                }
                if (maxLevel.IndexOf("DEBUG") < 0 && maxLevel.IndexOf("INFO") < 0 && maxLevel.IndexOf("WARN") < 0 &&
                    maxLevel.IndexOf("ERROR") < 0 && maxLevel.IndexOf("FATAL") < 0)
                {
                    ButtonFunction(btSelect, true);
                    ButtonFunction(btOut, true);
                    MessageBox.Show("请选择正确的日志高等级", "日志等级", MessageBoxButtons.OK);
                    return;
                }

                //获取符合条件的等级范围
                string Level = "DEBUG,INFO,WARN,ERROR,FATAL".Substring("DEBUG,INFO,WARN,ERROR,FATAL".IndexOf(minLevel));
                string[] _Level = (Level.IndexOf(maxLevel) >= 0 ? Level.Substring(0, Level.IndexOf(maxLevel) + maxLevel.Length) : "").Split(',');

                string LastLevel = string.Empty;
                for (int i = 0; i < _Level.Length; i++)
                {
                    if (i == (_Level.Length - 1))
                        LastLevel = " And (" + LastLevel + string.Format("LogLevel='{0}'", _Level[i]) + ")";
                    else
                        LastLevel += string.Format("LogLevel='{0}' Or ", _Level[i]);
                }
                //查询条件
                string commandText = string.Empty;
                commandText += string.Format("select * from {0} where {1} Between '{2}' and '{3}'", TableName, "Time" ,dateTimeStart,dateTimeEnd);
                commandText += string.Format("{0}", LastLevel);
                dt = jFSQLiteDB.ExecuteDataset(connectstring, CommandType.Text, commandText).Tables[0];
                InitDataSet();
            }
            catch(Exception ex) 
            { 
                MessageBox.Show(ex.ToString(),"Exception",MessageBoxButtons.OK); 
            }
            ButtonFunction(btSelect, true);
            ButtonFunction(btOut, true);
        }
        #endregion

        #region Page管理
        /// <summary>
        /// 日志查询初始化显示
        /// </summary>
        private void InitDataSet()
        {
            if (dt != null)
                nMax = dt.Rows.Count;
            else
                nMax = 0;

            pageCount = (nMax / pageSize);

            if ((nMax % pageSize) > 0) pageCount++;

            if (pageCount == 0)
            {
                pageCurrent = 0;
            }
            else
            {
                pageCurrent = 1;
            }
            nCurrent = 0;
            LoadData(); 
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void LoadData()
        {
            try
            {
                nStartPos = 0;
                nEndPos = 0;

                if (pageCurrent == pageCount)
                    nEndPos = nMax;
                else
                    nEndPos = pageSize * pageCurrent;

                nStartPos = nCurrent;

                MethodInvoker In = new MethodInvoker(UpdateControl);
                this.BeginInvoke(In);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 委托
        /// </summary>
        private void UpdateControl()
        {
            DataTable dtTemp = dt.Clone();
            //Page
            toolStripTextBoxTotalPage.Text = "/" + pageCount.ToString();
            toolStripTextBoxCurrentPage.Text = Convert.ToString(pageCurrent);

            //DataGridViewFunction(dgvSelect, "", "", "", false);
            for (int i = nStartPos; i < nEndPos; i++)
            {
                dtTemp.ImportRow(dt.Rows[i]);
                nCurrent++;
            }
            bdsInfo.DataSource = dtTemp;
            bdnPageManager.BindingSource = bdsInfo;
            dgvSelect.DataSource = bdsInfo;
            bindingNavigatorCountItem.Text = "/"+bdsInfo.Count.ToString();
        }

        /// <summary>
        /// 点击BlinderNavigator控件的项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BdnPageManager_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //前一页
            if (e.ClickedItem.Name == "toolStripLabelPreviousPage")
            {
                pageCurrent--;
                if (pageCurrent <= 0)
                {
                    pageCurrent++;
                    return;
                }
                else
                {
                    nCurrent = pageSize * (pageCurrent - 1);
                }
                LoadData();
            }
            //下一页
            if (e.ClickedItem.Name == "toolStripLabelNextPage")
            {
                pageCurrent++;
                if (pageCurrent > pageCount)
                {
                    pageCurrent--;
                    return;
                }
                else
                {
                    nCurrent = pageSize * (pageCurrent - 1);
                }
                LoadData();
            }
        }
        #endregion

        #region 窗体自适应大小
        private void UCLogger_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            setControls(newx, newy, this);
        }

        /// <summary>
        /// 重新设置控件
        /// </summary>
        /// <param name="cons"></param>
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }

        /// <summary>
        /// 重新设置控件属性
        /// </summary>
        /// <param name="newx"></param>
        /// <param name="newy"></param>
        /// <param name="cons"></param>
        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });

                if (con == bindingNavigatorStartPositionItem.Control || con == toolStripTextBoxCurrentPage.Control )
                {
                    float a = Convert.ToSingle(mytag[0]) * (float)1.0;
                    con.Width = (int)a;
                    a = Convert.ToSingle(mytag[1]) * (float)1.0;
                    con.Height = (int)(a);
                    a = Convert.ToSingle(mytag[2]) * (float)1.0;
                    con.Left = (int)(a);
                    a = Convert.ToSingle(mytag[3]) * newy;
                    con.Top = (int)(a);
                    Single currentSize = (Convert.ToSingle(mytag[4])) * (float)1.0;
                    //Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                    //con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                }
                else
                {
                    float a = Convert.ToSingle(mytag[0]) * newx;
                    con.Width = (int)a;
                    a = Convert.ToSingle(mytag[1]) * newy;
                    con.Height = (int)(a);
                    a = Convert.ToSingle(mytag[2]) * newx;
                    con.Left = (int)(a);
                    a = Convert.ToSingle(mytag[3]) * newy;
                    con.Top = (int)(a);
                    Single currentSize = (Convert.ToSingle(mytag[4])) * newy;
                    //Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                   // con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                }
                
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
        }
        #endregion
    }
}
