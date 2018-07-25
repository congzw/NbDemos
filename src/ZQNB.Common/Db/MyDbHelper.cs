using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ZQNB.Common.Db
{
    /// <summary>
    /// ���ݿ�Ĳ���������
    /// </summary>
    public class MyDbHelper
    {
        private static readonly object _lock = new object();
        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        public static MyDbHelper Call(string dbConnectionName)
        {
            lock (_lock)
            {
                MyDbHelper instance = new MyDbHelper(dbConnectionName);
                return instance;
            }
        }
        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static MyDbHelper CallWithConnString(string connString)
        {
            lock (_lock)
            {
                MyDbHelper instance = new MyDbHelper();
                instance._connString = connString;
                return instance;
            }
        }

        private string _connString;
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="dbConnectionName"></param>
        protected MyDbHelper(string dbConnectionName)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[dbConnectionName];
            if (connectionStringSettings == null)
            {
                throw new ArgumentException(string.Format("���ݿ�����û���ҵ���{0}������config�ļ��е�connectionStrings���ýڡ�", dbConnectionName));
            }
            _connString = connectionStringSettings.ConnectionString;
        }
        /// <summary>
        /// CTOR
        /// </summary>
        protected MyDbHelper()
        {
        }

        #region  ִ�м�SQL���
        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public int ExecuteSql(string sql)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="sqls">����SQL���</param>        
        public int ExecuteSqlsInTran(List<String> sqls)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < sqls.Count; n++)
                    {
                        string strsql = sqls[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="sql">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
        public object ExecuteSqlScalar(string sql)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteSqlReader(string sql)
        {
            SqlConnection connection = new SqlConnection(_connString);
            SqlCommand cmd = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="srcTable"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteSqlDataSet(string sql, string srcTable = "Table1")
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(sql, connection);
                    command.Fill(ds, srcTable);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        #endregion

        #region ִ�д�������SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="cmdParms"></param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public int ExecuteSql(string sql, params IDataParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareSqlCommand(cmd, connection, null, sql, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="sqls">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����IDataParameter[]��</param>
        public void ExecuteSqlInTran(Hashtable sqls)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        //ѭ��
                        foreach (DictionaryEntry myDE in sqls)
                        {
                            string cmdText = myDE.Key.ToString();
                            IDataParameter[] cmdParms = (IDataParameter[])myDE.Value;
                            PrepareSqlCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="sql">�����ѯ������</param>
        /// <param name="cmdParms"></param>
        /// <returns>��ѯ�����object��</returns>
        public object ExecuteSqlScalar(string sql, params IDataParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareSqlCommand(cmd, connection, null, sql, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="cmdParms"></param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteSqlReader(string sql, params IDataParameter[] cmdParms)
        {
            SqlConnection connection = new SqlConnection(_connString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareSqlCommand(cmd, connection, null, sql, cmdParms);
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
            //            finally
            //            {
            //                cmd.Dispose();
            //                connection.Close();
            //            }    

        }

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="cmdParms"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteSqlDataSet(string sql, params IDataParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareSqlCommand(cmd, connection, null, sql, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        public IList<T> QueryWithPage<T>(string innerSql, string orderBy, int pageIndex, int pageSize, IDataParameter[] cmdParms, out int totalCount) where T : new()
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentNullException("orderBy");
            }

            var sqlTotalCountSql = string.Format(
@"SELECT COUNT(1) AS TotalCount
FROM 
(
    {0}
) TempInnerTable", innerSql);

            totalCount = (int)ExecuteSqlScalar(sqlTotalCountSql, cmdParms);
            //totalCount = (int)ExecuteSqlScalar(sqlTotalCountSql);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            var start = (pageIndex - 1) * pageSize + 1;
            var end = pageIndex * pageSize;

            var sqlWithPage = string.Format(
@"SELECT * 
FROM 
(
    SELECT *, ROW_NUMBER() OVER ( ORDER BY {0} ) AS RowNumber
    FROM 
    (
        {1}
    ) TempInnerTable
) TempOuterTable
WHERE RowNumber BETWEEN @RowNumber_Begin AND @RowNumber_End",
                                 orderBy, innerSql);

            var newCmdParams = new List<IDataParameter>();
            if (cmdParms != null)
            {
                newCmdParams.AddRange(cmdParms);
            }

            newCmdParams.Add(MakeIDataParameter("@RowNumber_Begin", start));
            newCmdParams.Add(MakeIDataParameter("@RowNumber_End", end));

            var sqlDataReader = ExecuteSqlReader(sqlWithPage, newCmdParams.ToArray());
            var list = ToList<T>(sqlDataReader);
            return list;
        }

        public IList<T> QueryAll<T>(string innerSql, string orderBy, IDataParameter[] cmdParms) where T : new()
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentNullException("orderBy");
            }

            var sqlTotalCountSql = string.Format(
@"SELECT COUNT(1) AS TotalCount
FROM 
(
    {0}
) TempInnerTable", innerSql);

            var sqlWrapper = string.Format(
@"SELECT * 
FROM 
(
    SELECT *, ROW_NUMBER() OVER ( ORDER BY {0} ) AS RowNumber
    FROM 
    (
        {1}
    ) TempInnerTable
) TempOuterTable",
                                 orderBy, innerSql);

            var sqlDataReader = ExecuteSqlReader(sqlWrapper, cmdParms);
            var list = ToList<T>(sqlDataReader);
            return list;
        }

        private static void PrepareSqlCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, IDataParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (IDataParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region �洢���̲���

        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="storedProcName">��ѯ���SP</param>
        /// <param name="cmdParms"></param>
        /// <returns>��ѯ�����object��</returns>
        public object ExecuteScalar(string storedProcName, params IDataParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                object result;
                connection.Open();
                var command = BuildQuerySpCommand(connection, storedProcName, cmdParms);
                result = command.ExecuteScalar();
                return result;
            }
        }

        /// <summary>
        /// ִ�д洢���̣�����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(_connString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQuerySpCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;

        }

        /// <summary>
        /// ִ�д洢����
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="srcTable">DataSet����еı���</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string storedProcName, IDataParameter[] parameters, string srcTable)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQuerySpCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, srcTable);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// ִ�д洢���̣�����Ӱ�������        
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="rowsAffected">Ӱ�������</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntSpCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// ���� SqlCommand ����ʵ��(��������һ������ֵ)    
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlCommand ����ʵ��</returns>
        private static SqlCommand BuildIntSpCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQuerySpCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        /// <summary>
        /// ���� SqlCommand ����(��������һ���������������һ������ֵ)
        /// </summary>
        /// <param name="connection">���ݿ�����</param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQuerySpCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (IDataParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // ���δ����ֵ���������,���������DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }
        #endregion

        #region һЩ���õĸ�������

        private const string killThreadSqlFormat = @"DECLARE @dbname sysname
SET @dbname = N'{0}'

DECLARE @spid int
SELECT @spid = min(spid) from master.dbo.sysprocesses where dbid = db_id(@dbname)
WHILE @spid IS NOT NULL
BEGIN
--print 'KILL ' + rtrim(@spid)
EXECUTE ('KILL ' + @spid)
SELECT @spid = min(spid) from master.dbo.sysprocesses where dbid = db_id(@dbname) AND spid > @spid
END";

        /// <summary>
        /// ��ԭ���ݿ�
        /// </summary>
        /// <param name="databaseName">Ҫ��ԭ�����ݿ�����</param>
        /// <param name="backupFile">���ݿ��ļ�</param>
        /// <returns>��ԭ�ɹ������ʾ</returns>
        public bool RestoreDb(string databaseName, string backupFile)
        {
            //Kill������
            PrepareKill(databaseName);
            //��ԭ
            string restoreSql = String.Format("RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE", databaseName, backupFile);
            ExecuteDatabaseSql(_connString, restoreSql, "master");
            return true;
        }

        /// <summary>
        /// �������ݿ�
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="dbFileName"></param>
        /// <returns>�Ƿ񱸷ݳɹ�</returns>
        public bool BackupDb(string databaseName, string dbFileName)
        {
            //Kill������
            PrepareKill(databaseName);
            //����
            string backupSql = string.Format("backup database [{0}] to disk = '{1}'", databaseName, dbFileName);
            ExecuteDatabaseSql(_connString, backupSql, "master");
            return true;
        }

        /// <summary>
        /// �½������ݿ�
        /// </summary>
        /// <param name="databaseName">���ݿ���</param>
        public bool CreateDb(string databaseName)
        {
            bool checkDbExist = CheckDbExist(databaseName);
            if (checkDbExist)
            {
                throw new Exception("���ݿ��Ѵ��ڣ�" + databaseName);
            }

            string createDbScript = string.Format(
            @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
    CREATE DATABASE [{0}]
END
ELSE
BEGIN
    SELECT 1
END", databaseName);

            //�������ݿ� 
            ExecuteDatabaseSql(_connString, createDbScript, "master");
            return true;
        }

        /// <summary>
        /// ɾ�����ݿ�
        /// </summary>
        public bool DropDb(string databaseName)
        {
            //ɾ��ǰ��Kill������
            PrepareKill(databaseName);
            //ɾ�����ݿ� 
            string dropDbScript = string.Format(
@"IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
    DROP DATABASE [{0}]
END
ELSE
BEGIN
    SELECT 1
END", databaseName);

            ExecuteDatabaseSql(_connString, dropDbScript, "master");
            return true;
        }

        /// <summary>
        /// ������ݿ��Ƿ����
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public bool CheckDbExist(string database)
        {
            MessageResult mr = new MessageResult();
            string script = string.Format(
@"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
    SELECT 0
END
ELSE
BEGIN
    SELECT 1
END", database);

            int count = (int)ExecuteDatabaseSqlScalar(_connString, script, "master");
            return count > 0;
        }

        /// <summary>
        /// ���Ƿ����
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool CheckTableExists(string tableName)
        {
            string sql = string.Format(
                @"select count(*) from  sysobjects where id = object_id(N'[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1;", tableName);

            object obj = ExecuteSqlScalar(sql);

            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }

            return cmdresult != 0;
        }

        /// <summary>
        /// �ж��Ƿ����ĳ���ĳ���ֶ�
        /// </summary>
        /// <param name="tableName">������</param>
        /// <param name="columnName">������</param>
        /// <returns>�Ƿ����</returns>
        public bool CheckColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = ExecuteSqlScalar(sql);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }

        /// <summary>
        /// ȡ���ID+1
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetNextMaxId(string tableName, string columnName)
        {
            string strsql = "select max(" + columnName + ")+1 from " + tableName;
            object obj = ExecuteSqlScalar(strsql);
            if (obj == null)
            {
                return 1;
            }

            return int.Parse(obj.ToString());
        }

        /// <summary>
        /// ȡ���ID+1
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public long GetNextMaxIdLong(string tableName, string columnName)
        {
            string strsql = "select max(" + columnName + ")+1 from " + tableName;
            object obj = ExecuteSqlScalar(strsql);
            if (obj == null)
            {
                return 1;
            }

            return long.Parse(obj.ToString());
        }

        /// <summary>
        /// databaseName-yyyyMMddHHmmss.bak
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public string MakeBackupFileName(string databaseName)
        {
            string result = string.Format("{0}{1}.bak", databaseName, DateTime.Now.ToString("-yyyyMMddHHmmss"));
            return result.ToLower().Replace(".bak.bak", ".bak");
        }

        /// <summary>
        /// ��ȡ�����е������ַ�����������ò����ڣ��׳�ArgumentException�쳣��
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        public string GetConnectionString(string dbConnectionName)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionName))
            {
                throw new ArgumentNullException("dbConnectionName");
            }

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[dbConnectionName];
            if (connectionStringSettings == null)
            {
                throw new ArgumentException(string.Format("���ݿ�����û���ҵ���{0}������config�ļ��е�connectionStrings���ýڡ�", dbConnectionName));
            }

            return connectionStringSettings.ConnectionString;
        }

        private void PrepareKill(string databaseName)
        {
            //Kill������
            //����һ��Ҫ��master���ݿ⣬��������Ҫ��ԭ�����ݿ⣬��Ϊ��������������������ռ�������ݿ⡣
            string killThreadSql = string.Format(killThreadSqlFormat, databaseName);
            ExecuteDatabaseSql(_connString, killThreadSql, "master");
        }


        //-------------��̬��������-----------------

        /// <summary>
        /// ִ�����ݿ�ű�
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="sqlScript"></param>
        /// <param name="changeDatabase"></param>
        public static void ExecuteDatabaseSql(string connString, string sqlScript, string changeDatabase = "")
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ArgumentNullException("conn");
            }

            if (string.IsNullOrWhiteSpace(sqlScript))
            {
                throw new ArgumentNullException("sqlScript");
            }

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                if (!string.IsNullOrWhiteSpace(changeDatabase))
                {
                    sqlConnection.ChangeDatabase(changeDatabase);
                }

                Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string[] sqls = regex.Split(sqlScript);
                foreach (var sql in sqls)
                {
                    SqlCommand command = new SqlCommand(sql, sqlConnection);
                    command.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }

        /// <summary>
        /// ִ�����ݿ�ű�
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="sqlScript"></param>
        /// <param name="changeDatabase"></param>
        /// <returns></returns>
        public static object ExecuteDatabaseSqlScalar(string connString, string sqlScript, string changeDatabase = "")
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ArgumentNullException("conn");
            }

            if (string.IsNullOrWhiteSpace(sqlScript))
            {
                throw new ArgumentNullException("sqlScript");
            }

            object executeScalar = null;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                if (!string.IsNullOrWhiteSpace(changeDatabase))
                {
                    sqlConnection.ChangeDatabase(changeDatabase);
                }

                Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string[] sqls = regex.Split(sqlScript);
                foreach (var sql in sqls)
                {
                    SqlCommand command = new SqlCommand(sql, sqlConnection);
                    executeScalar = command.ExecuteScalar();

                }

                sqlConnection.Close();
            }
            return executeScalar;
        }

        /// <summary>
        /// ����sql����
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IDataParameter MakeIDataParameter(string parameterName, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            IDataParameter p = new SqlParameter(parameterName, value) { Direction = direction };
            return p;
        }

        ////todo...
        ///// <summary>   
        ///// ��ȡ��ȫ��SQL�ַ���
        ///// </summary>   
        ///// <param name="sql���"></param>   
        ///// <returns></returns>   
        //public static string ReplaceSqlString(string sql)
        //{
        //    string safeSql =
        //    sql.Replace(",", "��")
        //    .Replace(".", "��")
        //    .Replace("(", "��")
        //    .Replace(")", "��")
        //    .Replace(">", "��")
        //    .Replace("<", "��")
        //    .Replace("-", "��")
        //    .Replace("+", "��")
        //    .Replace("=", "��")
        //    .Replace("?", "��")
        //    .Replace("*", "��")
        //    .Replace("|", "��")
        //    .Replace("&", "��");

        //    return safeSql;
        //}

        #endregion

        #region MappingHelper

        public static IList<T> ToList<T>(IDataReader reader) where T : new()
        {
            var type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            IList<T> list = new List<T>();
            while (reader.Read())
            {
                var item = new T();
                foreach (PropertyInfo property in properties)
                {
                    int ordinal = reader.GetOrdinal(property.Name);
                    object theValue = reader.GetValue(ordinal);
                    if (theValue != DBNull.Value)
                    {
                        if (property.PropertyType == typeof(DayOfWeek))
                        {
                            var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), theValue.ToString());
                            property.SetValue(item, day, null);
                        }
                        else
                        {
                            property.SetValue(item, theValue, null);
                        }
                    }
                }
                list.Add(item);
            }
            return list;
        }

        public static IList<T> ToList<T>(DataSet ds) where T : new()
        {
            var result = new List<T>();
            if (ds == null || ds.Tables.Count == 0)
            {
                return result;
            }

            return ToList<T>(ds.Tables[0]);
        }

        public static IList<T> ToList<T>(DataTable table) where T : new()
        {
            var result = new List<T>();
            if (table == null || table.Rows.Count == 0)
            {
                return result;
            }

            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IEnumerable<PropertyInfo> properties) where T : new()
        {
            var item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DayOfWeek))
                {
                    var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {
                    property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }

        #endregion
    }
}