using System;
using System.Collections.Generic;
using System.Text;

namespace ZQNB.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string Join<T>(this IEnumerable<T> self, string separator)
        {
            Assertion.NotNull(self);
            Assertion.NotNull(separator);

            var sb = new StringBuilder();
            foreach (var element in self)
            {
                sb.AppendFormat("{0}{1}", element, separator);
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - separator.Length, separator.Length);
            }
            return sb.ToString();
        }
        public static string ToListString<T>(this IEnumerable<T> self)
        {
            Assertion.NotNull(self);

            return string.Format("[{0}]", self.Join(", "));
        }
    }
}
