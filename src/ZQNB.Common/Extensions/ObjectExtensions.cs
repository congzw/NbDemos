using ZQNB.Common.Serialize;

namespace ZQNB.Common.Extensions
{
    /// <summary>
    /// ObjectExtensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// ÐòÁÐ»¯³ÉJson×Ö·û´®
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="camelCase"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool camelCase = false)
        {
            if (obj == null)
            {
                return null;
            }
            var serialize = JsonHelper.Serialize(obj, NbJsonFormatting.Indented, camelCase);
            return serialize;
        }
    }
}