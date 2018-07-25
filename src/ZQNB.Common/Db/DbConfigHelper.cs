using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ZQNB.Common.Db
{
    /// <summary>
    /// 数据库配置的帮助类
    /// </summary>
    public class DbConfigHelper
    {
        /// <summary>
        /// 获取连接名称
        /// </summary>
        /// <returns></returns>
        public static string FindConnName()
        {
            //没有额外配置，则用项目前缀去作为默认名称
            var nbRegistry = NbRegistry.Instance;
            var projectPrefix = nbRegistry.CurrentProjectPrefix;
            var connName = MyConfigHelper.GetAppSettingValue(nbRegistry.Config_Common_ConnName, projectPrefix);
            return connName;
        }

        /// <summary>
        /// FindDbNameFromConnection
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public static string FindDbNameFromConnection(string connName)
        {
            if (string.IsNullOrWhiteSpace(connName))
            {
                throw new ArgumentException("必须指定connName");
            }

            var connectionStringSetting = ConfigurationManager.ConnectionStrings[connName];
            if (connectionStringSetting == null)
            {
                //没有找到
                throw new Exception(string.Format("没有从配置中找到名为{0}的数据库连接！", connName));
            }

            var dbName = FindDbNameFromConnectionString(connectionStringSetting.ConnectionString);
            return dbName;
        }

        /// <summary>
        /// FindDbNameFromConnection
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static string FindDbNameFromConnectionString(string connString)
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ArgumentException("必须指定connString");
            }

            //如果配置名相符，就将数据库连接字符串中的InitialCatalog取出，作为DbName
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connString);
            var dbName = sqlConnectionStringBuilder.InitialCatalog;
            return dbName;
        }

        /// <summary>
        /// 查找ConnectionString
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public static string FindConnectionString(string connName)
        {
            if (string.IsNullOrWhiteSpace(connName))
            {
                throw new ArgumentException("必须指定connName");
            }

            var connectionStringSetting = ConfigurationManager.ConnectionStrings[connName];
            if (connectionStringSetting == null)
            {
                //没有找到
                throw new Exception(string.Format("没有从配置中找到名为{0}的数据库连接！", connName));
            }

            //如果配置名相符，就将数据库连接字符串中的InitialCatalog取出，作为DbName
            return connectionStringSetting.ConnectionString;
        }
        
        /// <summary>
        /// 重新创建数据库
        /// </summary>
        /// <param name="connName"></param>
        public static void RecreateDb(string connName)
        {
            if (string.IsNullOrWhiteSpace(connName))
            {
                throw new ArgumentException("必须指定connName");
            }

            var dbName = FindDbNameFromConnection(connName);
            string connStr = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
            var helper = new SqlScriptHelper();
            helper.ReCreateDbIfExist(connStr, dbName);
        }

        /// <summary>
        /// 检测数据库是否存在
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public static bool CheckDbExist(string connName)
        {
            if (string.IsNullOrWhiteSpace(connName))
            {
                throw new ArgumentException("必须指定connName");
            }

            var dbName = FindDbNameFromConnection(connName);
            string connStr = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
            var helper = new SqlScriptHelper();
            var exist = helper.CheckDbExist(connStr, dbName);
            return exist.Success;
        }

        /// <summary>
        /// 确保数据库存在，没有则创建
        /// </summary>
        /// <param name="connName"></param>
        public static void MakeSureDbExist(string connName)
        {
            var checkDbExist = CheckDbExist(connName);
            if (!checkDbExist)
            {
                RecreateDb(connName);
            }
        }
    }
}