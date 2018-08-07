using FluentNHibernate.Mapping;

namespace ZQNB.BaseLib.Dics._Maps
{
    /// <summary>
    /// 字典类型表映射
    /// </summary>
    public class DicTypeMap : ClassMap<DicType>
    {
        /// <summary>
        /// 字典类型表映射
        /// </summary>
        public DicTypeMap()
        {
            Table("Lib_Dics_DicType");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Code).Unique().Not.Nullable();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.InUse);
            Map(x => x.CanEdit);
        }
    }
}
