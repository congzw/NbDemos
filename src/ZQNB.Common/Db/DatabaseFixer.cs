using System;
using System.Collections.Generic;

namespace ZQNB.Common.Db
{
    /// <summary>
    /// 数据库修正帮助类，用于开发调试时方便的修改数据库Scheme
    /// </summary>
    public class DatabaseFixer
    {
        /// <summary>
        /// 修正的sql字典
        /// </summary>
        public static Dictionary<string, string> Sqls = new Dictionary<string, string>();

        /// <summary>
        /// 修正的脚本，脚本要负责保证重复执行的情况
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sql"></param>
        public static void AddOrReplaceFixSql(string name, string sql)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException("sql");
            }
            var key = CreateKey(name);
            Sqls[key] = sql;
        }

        /// <summary>
        /// 执行数据库的修正
        /// </summary>
        public static void RunFix(string connName)
        {
            if (string.IsNullOrWhiteSpace(connName))
            {
                throw new ArgumentException("必须指定connName");
            }
            var connStr = DbConfigHelper.FindConnectionString(connName);
            SqlScriptHelper helper = new SqlScriptHelper();
            foreach (var sql in Sqls.Values)
            {
                helper.RunScript(connStr, sql, null);
            }
        }

        #region 友好封装
        
        /// <summary>
        /// 增加新列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="columnType"></param>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static string CreateScriptForAddColumn(string table, string column, string columnType, bool nullable = true)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentNullException(table);
            }
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException(column);
            }
            if (string.IsNullOrWhiteSpace(columnType))
            {
                throw new ArgumentNullException(columnType);
            }

            var scriptForAddColumn = @"IF NOT EXISTS(select * from syscolumns where id=object_id('{table}') and name='{column}')
            BEGIN
            ALTER Table {table} ADD {column} {columnType} {null}
            END"
                .Replace("{table}", table)
                .Replace("{column}", column)
                .Replace("{columnType}", columnType)
                .Replace("{null}", nullable ? "NULL" : "NOT NULL");

            return scriptForAddColumn;
        }

        /// <summary>
        /// 删除某列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string CreateScriptForRemoveColumn(string table, string column)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentNullException(table);
            }
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException(column);
            }

            var scriptForRemoveColumn = @"IF EXISTS(select * from syscolumns where id=object_id('{table}') and name='{column}') )
            BEGIN
            ALTER TABLE {table} DROP COLUMN {column} ; 
            END"
                .Replace("{table}", table)
                .Replace("{column}", column);
            return scriptForRemoveColumn;
        }

        #endregion

        ////todo...
        //public static string CreateScriptForRenameColumn()
        //{
        //}
        ////todo...
        //public static string CreateScriptForAddTable()
        //{
        //}
        
        // 
        private static string CreateKey(string name)
        {
            return name.ToLower();
        }
    }
}