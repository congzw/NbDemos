using System;
using FluentNHibernate.Mapping;
using ZQNB.Common.Data.Model;

namespace NhibernateDemo.Relations.SingleRelationTrees
{
    public class SingleRelationOrg : NbEntity<SingleRelationOrg>
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
        public virtual SingleRelationOrg Parent { get; set; }
    }

    public class SingleRelationOrgMap : ClassMap<SingleRelationOrg>
    {
        public SingleRelationOrgMap()
        {
            Table("Lib_Relations_SingleRelationOrg");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Name).Index("Index_Lib_Relations_SingleRelationOrg_Name");
            Map(x => x.SortNum).Index("Index_Lib_Relations_SingleRelationOrg_SortNum");
            References(x => x.Parent).Column("ParentId");
        }
    }
}
