using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ZQNB.Common.CrudDatas
{
    /// <summary>
    /// 数据导入的列定义
    /// </summary>
    public class ClassColumnInfo
    {
        /// <summary>
        /// 数据导入的列定义
        /// </summary>
        public ClassColumnInfo()
        {
            Name = string.Empty;
            Caption = string.Empty;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 创建数据导入的列定义
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<ClassColumnInfo> Create(Type type)
        {
            var columnInfos = new List<ClassColumnInfo>();
            var properties = type.GetProperties();
            var index = 0;
            foreach (var property in properties)
            {
                var columnInfo = new ClassColumnInfo();
                columnInfo.Index = index;
                index++;
                columnInfo.Name = property.Name;
                var descriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    columnInfo.Caption = descriptionAttribute.Description;
                }
                else
                {
                    var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayNameAttribute != null)
                    {
                        columnInfo.Caption = displayNameAttribute.DisplayName;
                    }
                }
                columnInfos.Add(columnInfo);
            }
            return columnInfos;
        }
    }
}
