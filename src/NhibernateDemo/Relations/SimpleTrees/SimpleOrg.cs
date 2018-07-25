using System;
using FluentNHibernate.Mapping;
using ZQNB.Common.Data.Model;

namespace NhibernateDemo.Relations.SimpleTrees
{
    public class SimpleOrg : NbEntity<SimpleOrg>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual double SortNum { get; set; }
        /// <summary>
        /// 父亲id
        /// </summary>
        public virtual Guid? ParentId { get; set; }
    }

    public class SimpleOrgMap : ClassMap<SimpleOrg>
    {
        public SimpleOrgMap()
        {
            Table("Lib_Relations_SimpleOrg");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Name).Index("Index_Category_Name");
            Map(x => x.SortNum).Index("Index_Category_SortNum");
            Map(x => x.ParentId).Index("Index_Category_ParentId");
        }
    }
}
