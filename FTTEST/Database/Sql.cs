using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace FTTEST.Database
{
    public class Sql
    {
        static SqlConnection conn = new SqlConnection();
        public static string err_msg = string.Empty;
        public static event Database.ErrorsHandler Errors;
        /// <summary>
        /// 初始化数据库连接,ConnectionString="Data Source=192.168.158.29;Initial Catalog=RAKEN_TE; User ID=te;Password=te"
        /// </summary>
        /// <returns></returns>
        public static bool Open(string ConnectionString)
        {
            err_msg = string.Empty;
            /*
             Data Source=192.168.158.29;Initial Catalog=RAKEN_TE; User ID=te;Password=te
             */
            try
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
            }
            catch (Exception e)
            {
                err_msg = e.Message;
                Errors?.Invoke(new ErrorsEvent(e.Message));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public static void Close()
        {
            if (conn != null)
            {
                if (conn.State.ToString() == "Open")
                { conn.Close(); }
            }
        }
        
        /// <summary>
        /// 执行数据库查询操作,并返回结果
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="reader">结果集</param>
        /// <returns></returns>
        public static bool ServerExecute(string sql, out SqlDataReader reader)
        {
            reader = null;
            err_msg = string.Empty;
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(err_msg);
                    err_msg = e.Message;
                    Errors?.Invoke(new ErrorsEvent(e.Message));
                    return false;
                }
            }
            return true;
        }
        public static string queryData_original(string sqlStr, string keyvalue)
        {
            SqlConnection sqlConnection1 = new SqlConnection("Data Source=192.168.158.29 ;Database=RAKEN_te;User ID=te;PWD=te");//创建数据库连接
            sqlConnection1.Open();
             
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter(sqlStr, sqlConnection1);//利用已创建好的sqlConnection1,创建数据适配器sqlDataAdapter1
            DataSet dataSet1 = new DataSet();  //创建数据集对象
            sqlDataAdapter1.Fill(dataSet1);    //执行查询,查询的结果存放在数据集里
            string m = "";
            if (dataSet1.Tables[0].Rows.Count > 0)
            {
                m = dataSet1.Tables[0].Rows[0][keyvalue].ToString(); //把查询结果的第一行指定列下的数据以string类型返回

            }
            return m;
        }
        public static string queryData(string sqlStr, string keyvalue)
        {
            // 將連線字串獨立出來，方便管理
            string connectionString = "Data Source=192.168.158.29;Database=RAKEN_te;User ID=te;PWD=te";

            // 將回傳值變數在 using 區塊外宣告
            string m = "";

            // 使用 using 語句來管理 SqlConnection，確保它在區塊結束時會自動關閉
            using (SqlConnection sqlConnection1 = new SqlConnection(connectionString))
            {
                // 使用 using 語句來管理 SqlDataAdapter
                using (SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter(sqlStr, sqlConnection1))
                {
                    // DataSet 也建議用 using 管理，確保資源釋放
                    using (DataSet dataSet1 = new DataSet())
                    {
                        try
                        {
                            // 不需要在 using 裡手動開啟連線，.Fill() 方法會自動處理
                            // 如果連線是關閉的，.Fill() 會開啟它，完成後再關閉它。
                            sqlDataAdapter1.Fill(dataSet1);

                            // 增加對 Tables 存在的檢查，使程式碼更穩健
                            if (dataSet1.Tables.Count > 0 && dataSet1.Tables[0].Rows.Count > 0)
                            {
                                m = dataSet1.Tables[0].Rows[0][keyvalue].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            // 建議加入錯誤處理，方便追蹤問題
                            Console.WriteLine("查詢資料時發生錯誤: " + ex.Message);
                            // 或者可以將錯誤記錄到日誌檔案中
                        }
                    } // dataSet1 會在這裡被自動釋放
                } // sqlDataAdapter1 會在這裡被自動釋放
            } // sqlConnection1 會在這裡被自動關閉和釋放

            return m;
        }
        public static DataTable ExeSqlForDataTable(string commandText)
        {
            DataTable dataTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(commandText, "Data Source=192.168.158.29 ;Database=RAKEN_te;User ID=te;PWD=te"))
            {
                //Application.DoEvents();
                adapter.Fill(dataTable);
                Application.DoEvents();
            }
            return dataTable;
        }
        /// <summary>
        /// 执行数据库更新操作,无返回值
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns></returns>
        public static bool UpdateServer(string sql)
        {
            err_msg = string.Empty;
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    err_msg = e.Message;
                    //MessageBox.Show(err_msg);
                    Errors?.Invoke(new ErrorsEvent(e.Message));
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 限制可供使用的数据库
        /// </summary>
        public enum DatabaseOpt { RAKEN_TE = 0, RAKEN_SFIS, MES, NULL }
        /// <summary>
        /// 查询或设置默认数据库
        /// </summary>
        public static DatabaseOpt DefaultDatabase
        {
            get
            {
                if (conn.Database == "RAKEN_TE")
                {
                    return DatabaseOpt.RAKEN_TE;
                }
                else if (conn.Database == "RAKEN_SFIS")
                {
                    return DatabaseOpt.RAKEN_SFIS;
                }
                else if (conn.Database == "MES")
                {
                    return DatabaseOpt.MES;
                }
                else
                {
                    return DatabaseOpt.NULL;
                }
            }
            set
            {
                if (DatabaseOpt.RAKEN_SFIS == value)
                {
                    conn.ChangeDatabase("RAKEN_SFIS");
                }
                else if (DatabaseOpt.RAKEN_TE == value)
                {
                    conn.ChangeDatabase("RAKEN_TE");
                }
                else if (DatabaseOpt.MES == value)
                {
                    conn.ChangeDatabase("MES");
                }
            }
        }
    }
}