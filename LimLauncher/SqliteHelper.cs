using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimLauncher
{
    public class SqliteHelper
    {
        #region 常量、变量定义

        // 创建一个Sqlite数据库操作对象
        private static SqliteHelper _SqliteHelper = null;

        // 数据库连接字符串
        private const string ConnectStringFormat = "Data Source={0}";

        private SqliteConnection Connection { get; set; }


        #endregion 常量、变量定义

        #region 构造函数

        /// <summary>
        /// 构造函数:建立数据库连接
        /// </summary>
        /// <param name="serverIP">服务器IP</param>
        /// <param name="uid">用户ID</param>
        /// <param name="pwd">密码</param>
        /// <param name="dbname">数据库名</param>
        /// <param name="port">端口号</param>
        private SqliteHelper(string fileName)
        {
            Connection = new SqliteConnection(string.Format(ConnectStringFormat, fileName));
            Connection.Open();
        }

        /// <summary>
        /// 实例化一个数据库连接
        /// </summary>
        public static SqliteHelper Instance
        {
            get
            {
                // 建立数据库连接
                if (_SqliteHelper == null)
                    _SqliteHelper = new SqliteHelper("filelist.db");

                return _SqliteHelper;
            }
        }

        #endregion 构造函数

        #region 数据库操作

        private readonly object mutex = new object();

        /// <summary>
        /// 执行数据库的非查询类操作(插入、更新、删除)
        /// </summary>
        /// <param name="sqlContent">sql语句内容</param>
        /// <param name="sqlParams">sql语句参数</param>
        /// <returns>SQL影响的数据件数</returns>
        public int ExecuteNonQuery(string sqlContent, Dictionary<string, object> sqlParams = null)
        {
            // 互斥锁
            lock (mutex)
            {
                try
                {
                    // 创建数据库命令
                    string tempSql = sqlContent;
                    var command = new SqliteCommand(tempSql, Connection);

                    // 设置SQL的参数
                    if (sqlParams != null && sqlParams.Count > 0)
                    {
                        foreach (string strKey in sqlParams.Keys)
                        {
                            SqliteParameter sqlParam = new SqliteParameter(strKey, sqlParams[strKey]);
                            command.Parameters.Add(sqlParam);
                        }
                    }

                    // 执行数据库更新
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // 数据库更新失败后，关闭数据库连接并抛出异常。
                    throw new Exception("执行数据库更新操作失败!", ex);
                }
                finally
                {
                }
            }
        }

        
        /// <summary>
        /// 执行数据库的查询类操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlContent">sql语句内容</param>
        /// <param name="sqlParams">sql语句参数</param>
        /// <returns>查询结果集</returns>
        public List<T> ExecuteQuery<T>(string sqlContent, Dictionary<string, object> sqlParams = null)
        {
            // 互斥锁
            lock (mutex)
            {

                SqliteDataReader reader = null;

                try
                {
                    // 查询结果集
                    List<T> queryList = new List<T>();
                    string tempSql = sqlContent;
                    // 创建数据库命令
                    var command = new SqliteCommand(tempSql, Connection);

                    // 设置SQL的参数
                    if (sqlParams != null && sqlParams.Count > 0)
                    {
                        foreach (string strKey in sqlParams.Keys)
                        {
                            SqliteParameter sqlParam = new SqliteParameter(strKey, sqlParams[strKey]);
                            command.Parameters.Add(sqlParam);
                        }
                    }

                    // 执行数据库查询操作
                    reader = command.ExecuteReader();

                    // 读取每行数据
                    while (reader.Read())
                    {
                        T obj = ConvertReaderToBean<T>(reader);
                        queryList.Add(obj);
                    }

                    return queryList;
                }
                catch (Exception)
                {
                    // 数据库查询失败后，关闭数据库连接并抛出异常。
                    throw;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }

        
        /// <summary>
        /// 检索单个值
        /// </summary>
        /// <param name="sqlContent">sql语句内容</param>
        /// <param name="sqlParams">sql语句参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlContent, Dictionary<string, object> sqlParams = null)
        {
            lock (mutex)
            {
                try
                {
                    var command = new SqliteCommand(sqlContent, Connection);
                    if (sqlParams != null && sqlParams.Count > 0)
                    {
                        foreach (string strKey in sqlParams.Keys)
                        {
                            SqliteParameter sqlParam = new SqliteParameter(strKey, sqlParams[strKey]);
                            command.Parameters.Add(sqlParam);
                        }
                    }
                    return command.ExecuteScalar();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                }
            }
        }

        public static T ConvertReaderToBean<T>(IDataReader reader)
        {
            Type targetType = typeof(T);
            T instace = (T)Activator.CreateInstance(targetType);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader[i];
                if (!Equals(val, DBNull.Value))
                    targetType.GetProperty(reader.GetName(i))?.SetValue(instace, val);
            }
            return instace;
        }

        #endregion 数据库操作
    }
}
