using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Transactions;

namespace ZQNB.Common.Db
{
    public class SqlScriptHelper
    {
        public MessageResult CheckDbExist(string connStr, string dbName)
        {
            MessageResult mr = new MessageResult();
            string script = string.Format(
@"USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
    SELECT 0
END
ELSE
BEGIN
    SELECT 1
END", dbName);

            string newConnStr = connStr.Replace(dbName, "master");
            using (SqlConnection sqlCon = new SqlConnection(newConnStr))
            {
                var cmd = sqlCon.CreateCommand();
                cmd.Connection = sqlCon;
                ExecuteNonQuerySqlWithGo(cmd, script);
                int count = (int)ExecuteScriptScalar(sqlCon, script);
                mr.Success = count > 0;
                mr.Message = string.Format("数据库[{0}]{1}存在", dbName, count > 0 ? "已" : "不");
                mr.Data = count;
                return mr;
            }
        }

        public MessageResult CreateDbIfNotExist(string dbserver, string dbname, string dbuser, string dbpassword)
        {
            string script = string.Format(@"USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
CREATE DATABASE [{0}] 
END", dbname);
            MessageResult mr = RunScript(dbserver, "master", dbuser, dbpassword, script);
            return mr;
        }

        public MessageResult CreateDbIfNotExist(string connStr, string dbName)
        {
            string script = string.Format(
@"USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
CREATE DATABASE [{0}] 
END", dbName);

            string newConnStr = connStr.Replace(dbName, "master");
            MessageResult mr = RunScript(newConnStr, script);
            return mr;
        }

        public MessageResult ReCreateDbIfExist(string dbserver, string dbName, string dbuser, string dbpassword)
        {
            string script = string.Format(
@"USE [master]
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
DROP DATABASE [{0}] 
END
CREATE DATABASE [{0}] 
GO", dbName);

            MessageResult mr = RunScript(dbserver, "master", dbuser, dbpassword, script);
            return mr;
        }

        public MessageResult ReCreateDbIfExist(string connStr, string dbName)
        {
            string script = string.Format(
@"USE [master]
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
DROP DATABASE [{0}] 
END
CREATE DATABASE [{0}] 
GO", dbName);

            string newConnStr = connStr.Replace(dbName, "master");
            MessageResult mr = RunScript(newConnStr, script);
            return mr;
        }

        public MessageResult RunScriptFile(string dbserver, string dbname, string dbuser, string dbpassword, string scriptSqlPath, string rollbackScriptPath = null)
        {
            string scriptSql = MyIOHelper.ReadAllText(scriptSqlPath);
            string rollbackScript = null;
            if (!string.IsNullOrWhiteSpace(rollbackScriptPath))
            {
                rollbackScript = MyIOHelper.ReadAllText(rollbackScriptPath);
            }

            MessageResult mr = RunScript(dbserver, dbname, dbuser, dbpassword, scriptSql, rollbackScript);
            return mr;
        }

        public MessageResult RunScript(string dbserver, string dbname, string dbuser, string dbpassword, string scriptSql, string rollbackScript = null)
        {
            string connString = MakeConnectionString(dbserver, dbname, dbuser, dbpassword);
            //执行sql
            MessageResult mr = RunScript(connString, scriptSql, rollbackScript);
            return mr;
        }

        public MessageResult RunScriptFile(string connString, string scriptSqlPath, string rollbackScriptPath = null)
        {
            string scriptSql = MyIOHelper.ReadAllText(scriptSqlPath);
            string rollbackScript = null;
            if (!string.IsNullOrWhiteSpace(rollbackScriptPath))
            {
                rollbackScript = MyIOHelper.ReadAllText(rollbackScriptPath);
            }

            MessageResult mr = RunScript(connString, scriptSql, rollbackScript);
            return mr;
        }

        public MessageResult RunScript(string connString, string scriptSql, string rollbackScript = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connString))
            {
                MessageResult mr = ExecuteScript(sqlCon, scriptSql);
                if (!mr.Success && !string.IsNullOrWhiteSpace(rollbackScript))
                {
                    mr = ExecuteScript(sqlCon, rollbackScript);
                    mr.Success = false;
                    mr.Message = "执行回滚操作完毕";
                }

                return mr;
            }
        }

        private MessageResult ExecuteScript(SqlConnection sqlCon, string scriptSql)
        {
            MessageResult mr = new MessageResult();
            if (sqlCon == null)
            {
                mr.Success = false;
                mr.Message = "数据库连接不能空";
                return mr;
            }

            if (string.IsNullOrWhiteSpace(scriptSql))
            {
                mr.Success = false;
                mr.Message = "scriptSql不能空";
                return mr;
            }

            Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] sqls = regex.Split(scriptSql);

            SqlCommand cmd = sqlCon.CreateCommand();
            cmd.Connection = sqlCon;
            sqlCon.Open();
            foreach (string line in sqls)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    cmd.CommandText = line;
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        mr.Success = false;
                        mr.Message = ex.Message;
                        return mr;
                    }
                }
            }
            sqlCon.Close();

            mr.Success = true;
            mr.Message = "执行完毕";
            return mr;
        }

        /// <summary>
        /// 创建数据库字符串
        /// </summary>
        /// <param name="dbserver"></param>
        /// <param name="dbname"></param>
        /// <param name="dbuser"></param>
        /// <param name="dbpassword"></param>
        /// <param name="connFormat"></param>
        /// <returns></returns>
        public string MakeConnectionString(string dbserver, string dbname, string dbuser, string dbpassword, string connFormat = "Data Source=#dbserver#;Initial Catalog=#dbname#;Persist Security Info=True;User ID=#dbuser#;Password=#dbpassword#")
        {
            if (string.IsNullOrWhiteSpace(dbserver))
            {
                throw new ArgumentException("必须指定dbserver");
            }
            if (string.IsNullOrWhiteSpace(dbname))
            {
                throw new ArgumentException("必须指定dbname");
            }
            if (string.IsNullOrWhiteSpace(dbuser))
            {
                throw new ArgumentException("必须指定dbuser");
            }
            if (string.IsNullOrWhiteSpace(dbpassword))
            {
                throw new ArgumentException("必须指定dbpassword");
            }

            string connString = connFormat.Replace("#dbserver#", dbserver)
                                .Replace("#dbname#", dbname)
                                .Replace("#dbuser#", dbuser)
                                .Replace("#dbpassword#", dbpassword);
            return connString;
        }

        public object RunScriptFileScalar(string connString, string scriptSqlPath)
        {
            string scriptSql = MyIOHelper.ReadAllText(scriptSqlPath);
            object result = RunScriptScalar(connString, scriptSql);
            return result;
        }
        public object RunScriptScalar(string connString, string scriptSql)
        {
            using (SqlConnection sqlCon = new SqlConnection(connString))
            {
                object mr = ExecuteScriptScalar(sqlCon, scriptSql);
                return mr;
            }
        }
        public object ExecuteScriptScalar(IDbConnection sqlCon, string scriptSql)
        {
            object result = null;
            if (sqlCon == null)
            {
                throw new ArgumentNullException("sqlCon");
            }

            if (string.IsNullOrWhiteSpace(scriptSql))
            {
                throw new ArgumentNullException("scriptSql");
            }

            Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] sqls = regex.Split(scriptSql);
            var cmd = sqlCon.CreateCommand();
            sqlCon.Open();

            //执行并返回最后一个Scalar
            foreach (var sql in sqls)
            {
                string line = sql;
                if (!string.IsNullOrEmpty(line))
                {
                    cmd.CommandText = line;
                    cmd.CommandType = CommandType.Text;
                    result = cmd.ExecuteScalar();
                }
            }

            sqlCon.Close();
            return result;
        }
        public void ExecuteNonQuerySqlWithGo(IDbCommand cmd, string scriptSql)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (string.IsNullOrWhiteSpace(scriptSql))
            {
                throw new ArgumentNullException("scriptSql");
            }

            Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] sqls = regex.Split(scriptSql);

            bool needClosed = false;
            if (cmd.Connection.State != ConnectionState.Open)
            {
                needClosed = true;
                cmd.Connection.Open();
            }

            foreach (string line in sqls)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    cmd.CommandText = line;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }

            if (needClosed)
            {
                cmd.Connection.Close();
            }
        }
    }
}
