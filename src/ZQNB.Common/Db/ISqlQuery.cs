using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ZQNB.Common.Db
{
    public interface ISqlQuery
    {
        #region Sql

        object ExecuteSqlScalar(string sql, params IDataParameter[] cmdParms);

        /// <summary>
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="cmdParms"></param>
        /// <returns>SqlDataReader</returns>
        SqlDataReader ExecuteSqlReader(string sql, params IDataParameter[] cmdParms);

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="cmdParms"></param>
        /// <returns>DataSet</returns>
        DataSet ExecuteSqlDataSet(string sql, params IDataParameter[] cmdParms);

        #endregion

        #region �洢���̲���

        object ExecuteScalar(string storedProcName, params IDataParameter[] cmdParms);

        /// <summary>
        /// ִ�д洢���̣�����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlDataReader</returns>
        SqlDataReader ExecuteReader(string storedProcName, IDataParameter[] parameters);

        /// <summary>
        /// ִ�д洢����
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="srcTable">DataSet����еı���</param>
        /// <returns>DataSet</returns>
        DataSet ExecuteDataSet(string storedProcName, IDataParameter[] parameters, string srcTable);

        #endregion

        #region ��չ

        T ExecuteScalar<T>(string sql, IDataParameter[] cmdParms);

        IList<T> QueryWithPage<T>(string innerSql, string orderBy, int pageIndex, int pageSize, IDataParameter[] cmdParms, out int totalCount) where T : new();

        IList<T> QueryAll<T>(string innerSql, string orderBy, IDataParameter[] cmdParms) where T : new();

        #endregion
    }

    public class SqlQuery : ISqlQuery
    {
        #region Factory
        
        private SqlQuery(MyDbHelper dbHelper)
        {
            _myDbHelper = dbHelper;
        }

        public static ISqlQuery CreateWithConnString(string connString)
        {
            return new SqlQuery(MyDbHelper.CallWithConnString(connString));
        }

        #endregion

        private readonly MyDbHelper _myDbHelper;

        public SqlQuery(string connName)
        {
            _myDbHelper = MyDbHelper.Call(connName);
        }

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="cmdParms"></param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public int ExecuteSql(string sql, params IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteSql(sql, cmdParms);
            return result;
        }

        public object ExecuteSqlScalar(string sql, params IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteSqlScalar(sql, cmdParms);
            return result;
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="cmdParms"></param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteSqlReader(string sql, params IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteSqlReader(sql, cmdParms);
            return result;
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <param name="cmdParms"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteSqlDataSet(string sql, params IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteSqlDataSet(sql, cmdParms);
            return result;
        }

        public object ExecuteScalar(string storedProcName, params IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteScalar(storedProcName, cmdParms);
            return result;
        }

        /// <summary>
        /// ִ�д洢���̣�����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string storedProcName, IDataParameter[] parameters)
        {
            var result = _myDbHelper.ExecuteReader(storedProcName, parameters);
            return result;
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
            var result = _myDbHelper.ExecuteDataSet(storedProcName, parameters, srcTable);
            return result;
        }

        public T ExecuteScalar<T>(string sql, IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteSqlScalar(sql, cmdParms);
            return (T)result;
        }

        public IList<T> QueryWithPage<T>(string innerSql, string orderBy, int pageIndex, int pageSize, IDataParameter[] cmdParms, out int totalCount) where T : new()
        {
            var result = _myDbHelper.QueryWithPage<T>(innerSql, orderBy, pageIndex, pageSize, cmdParms, out totalCount);
            return result;
        }

        public IList<T> QueryAll<T>(string innerSql, string orderBy, IDataParameter[] cmdParms) where T : new()
        {
            var result = _myDbHelper.QueryAll<T>(innerSql, orderBy,cmdParms);
            return result;
        }
    }

    public static class SqlQueryExtensions
    {
        #region MappingHelper

        public static IList<T> ToList<T>(this IDataReader reader) where T : new()
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

        public static IList<T> ToList<T>(this DataSet ds) where T : new()
        {
            var result = new List<T>();
            if (ds == null || ds.Tables.Count == 0)
            {
                return result;
            }

            return ToList<T>(ds.Tables[0]);
        }

        public static IList<T> ToList<T>(this DataTable table) where T : new()
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

    //, int page, int row, out int count)
}