using FluentNHibernate.Mapping;

namespace ZQNB.Common.NHExtensions
{
    public static class FluentNhibernateExtensions
    {
        /// <summary>
        /// nvarchar(max)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static PropertyPart MaxString(this PropertyPart part)
        {
            return part.CustomSqlType("nvarchar(max)").Length(int.MaxValue);
        }

        /// <summary>
        /// varbinary(max)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static PropertyPart MaxBinary(this PropertyPart part)
        {
            return part.CustomSqlType("varbinary(max)").Length(int.MaxValue);
        }
    }
}