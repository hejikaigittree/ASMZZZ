using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace JFLog
{
    /// <summary>
    /// 数据库SQLite的基本操作：数据库的新增/删除，表的新增/删除，数据记录的INSERT/UPDATE/DELETE/SELECT
    /// </summary>
    public class JFSQLiteDB
    {
        /// <summary>
        /// SQLLite数据库操作加锁
        /// </summary>
        private static readonly object sqliteLock = new object();

        /// <summary>
        /// 数据库密码
        /// </summary>
        private readonly string password = "HTTech";

        /// <summary>
        /// 构造函数
        /// </summary>
        public JFSQLiteDB()
        {

        }

        /// <summary>
        /// 创建SQLite数据库
        /// </summary>
        public bool CreateDB(string connectionString)
        {
            lock (sqliteLock)
            {
                //检查数据库是否存在，如果不存在会创建一个
                if (!System.IO.File.Exists(System.Environment.CurrentDirectory + "\\HTTech.db"))
                {
                    SQLiteConnection sqlcn = new SQLiteConnection(connectionString);
                    sqlcn.Open();
                    //sqlcn.ChangePassword(password);
                    sqlcn.Close();
                } 
            }
            return true;
        }

        /// <summary>
        /// 删除SQLite数据库
        /// </summary>
        public void DeleteDB()
        {
            lock (sqliteLock)
            { 
                if (System.IO.File.Exists(System.Environment.CurrentDirectory + "\\HTTech.db"))
                {
                    System.IO.File.Delete(System.Environment.CurrentDirectory + "\\HTTech.db");
                }
            }
        }

        /// <summary>
        /// 新增/删除一张表
        /// connectionString中需要判断当前表是否存在
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SQLiteConnection</param>
        public bool CreateOrDeleteTable(string CommandString)
        {
            lock (sqliteLock)
            {
                SQLiteConnection connection = new SQLiteConnection("Data Source=" + System.Environment.CurrentDirectory + "\\HTTech.db");

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = CommandString;
                    cmd.ExecuteNonQuery();
                }
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        /// <summary>
        /// 执行SELECT指令，返回一个DataSet
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SQLiteConnection</param>
        /// <param name="commandType">The CommandType</param>
        /// <param name="commandText">The SQLLite command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            lock (sqliteLock)
            {
                // Create & open a SQLiteConnection, and dispose of it after we are done
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                DataSet ds = new DataSet();

                // Create the SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand();

                PrepareCommand(ref cmd, connection, commandType, commandText);

                // Create the DataSet
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    
                adapter.Fill(ds);

                // // Detach the SQILiteDbParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                // If the provided connection is not open, we will open it
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                // Call the overload that takes a connection in place of the connection string
                return ds;
            }
        }

        /// <summary>
        /// 执行UPDATE、INSERT 或 DELETE 以及用于连接到数据源的字符串的指令，返回影响的行数
        /// </summary>
        /// <param name="connection"> A valid SQLiteConnection</param>
        /// <param name="commandType">The CommandType</param>
        /// <param name="commandText">The SQLLite command</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            lock (sqliteLock)
            {
                // Create & open a SQLiteConnection, and dispose of it after we are done
                SQLiteConnection connection = new SQLiteConnection(connectionString);
 
                // Create the SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand();

                PrepareCommand(ref cmd, connection, commandType, commandText);

                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，返回一个SQLiteDataReader
        /// </summary>
        /// <param name="connectionString">A valid SQLiteConnection</param>
        /// <param name="commandType">The CommandType</param>
        /// <param name="commandText">The SQLLite command</param>
        /// <returns>返回SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            lock (sqliteLock)
            {
                // Create & open a SQLiteConnection, and dispose of it after we are done
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                // Create the SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand();

                PrepareCommand(ref cmd, connection, commandType, commandText);

                SQLiteDataReader val = cmd.ExecuteReader();
                cmd.Parameters.Clear();

                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并返回由查询返回的结果集中的第一行的第一列。 其他列或行将被忽略。
        /// </summary>
        /// <param name="connection"> A valid SQLiteConnection</param>
        /// <param name="commandType">The CommandType</param>
        /// <param name="commandText">The SQLLite command</param>
        /// <returns></returns>
        public object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            lock (sqliteLock)
            {
                // Create & open a SQLiteConnection, and dispose of it after we are done
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(ref cmd, connection, commandType, commandText);
         
                object val = (Int32)cmd.ExecuteScalar();
                cmd.Parameters.Clear();

                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return val;
            }
        }

        /// <summary>
        /// 在执行SQL语句之前的准备工作
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        private void PrepareCommand(ref SQLiteCommand cmd, SQLiteConnection conn, CommandType cmdType, string cmdText)
        {
            if (conn.State != ConnectionState.Open)
            {
                //conn.SetPassword(password);
                conn.Open();
            }

            cmd.Connection = conn;
            //Set the command text(stored procedure name or SQL statement)
            cmd.CommandText = cmdText;
            cmd.Transaction = null;
            //Timeout
            cmd.CommandTimeout = 1200;
            ////Set the command type
            cmd.CommandType = cmdType;
        }
    }
}
