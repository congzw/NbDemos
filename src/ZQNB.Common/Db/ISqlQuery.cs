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
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="cmdParms"></param>
        /// <returns>SqlDataReader</returns>
        SqlDataReader ExecuteSqlReader(string sql, params IDataParameter[] cmdParms);

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="cmdParms"></param>
        /// <returns>DataSet</returns>
        DataSet ExecuteSqlDataSet(string sql, params IDataParameter[] cmdParms);

        #endregion

        #region 存储过程操作

        object ExecuteScalar(string storedProcName, params IDataParameter[] cmdParms);

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        SqlDataReader ExecuteReader(string storedProcName, IDataParameter[] parameters);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="srcTable">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        DataSet ExecuteDataSet(string storedProcName, IDataParameter[] parameters, string srcTable);

        #endregion

        #region 扩展

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
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms"></param>
        /// <returns>影响的记录数</returns>
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
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="cmdParms"></param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteSqlReader(string sql, params IDataParameter[] cmdParms)
        {
            var result = _myDbHelper.ExecuteSqlReader(sql, cmdParms);
            return result;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
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
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string storedProcName, IDataParameter[] parameters)
        {
            var result = _myDbHelper.ExecuteReader(storedProcName, parameters);
            return result;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="srcTable">DataSet结果中的表名</param>
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