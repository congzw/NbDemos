using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZQNB.Common
{
    public static class MyStringHelper
    {
        private static readonly object _lock = new object();
        private static readonly char[] _separator = { ',', '，', '；', ';' };
        
        //移除字符串
        public static string TrimString(this String value, string str2,  StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (str2 == null)
            {
                throw new ArgumentNullException("str2");
            }

            int index = value.IndexOf(str2, stringComparison);
            return value.Substring(0, index);
        }
        
        //扩展的字符串比较
        public static bool NbEquals(this String value, string str2, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase, bool trimSpaceBeforeCompare = true)
        {
            if (value == null)
            {
                return false;
            }

            if (str2 == null)
            {
                return false;
            }

            if (trimSpaceBeforeCompare)
            {
                return value.Trim().Equals(str2.Trim(), stringComparison);
            }

            return value.Equals(str2, stringComparison);
        }

        public static bool NbContains(this string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            if (source == null)
            {
                return false;
            }
            return source.IndexOf(toCheck, comp) >= 0;
        }

        //是否有默认分隔符
        /// <summary>
        /// 是否有默认分隔符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool NbHasSplitChar(this string source)
        {
            return source.IndexOfAny(_separator) > 0;
        }

        //用默认分割符分割
        /// <summary>
        /// 用默认分割符分割
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] NbSplit(this string source)
        {
            var splitString = SplitString(source);
            return splitString;
        }

        //扩展的字符串包含比较
        /// <summary>
        /// 比较是否包含，默认忽略大小写，忽略各种空差异
        /// </summary>
        /// <param name="list"></param>
        /// <param name="toCheck"></param>
        /// <param name="comparer"></param>
        /// <param name="trimSpaceBeforeCompare"></param>
        /// <returns></returns>
        public static bool NbContains(this IEnumerable<string> list, string toCheck, StringComparer comparer = null, bool trimSpaceBeforeCompare = true)
        {
            if (comparer == null)
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }

            string fixString = string.Empty;
            if (toCheck != null)
            {
                fixString = trimSpaceBeforeCompare ? toCheck.Trim() : toCheck;
            }

            return list.Contains(fixString, comparer);
        }


        //null == null
        //"" == null
        public static bool MyEquals(string str1, string str2, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase, bool trimSpaceBeforeCompare = true)
        {
            if (str1 == null)
            {
                str1 = "";
            }
            if (str2 == null)
            {
                str2 = "";
            }

            if (trimSpaceBeforeCompare)
            {
                return str1.Trim().Equals(str2.Trim(), stringComparison);
            }

            return str1.Equals(str2, stringComparison);


        }

        //role0000=>role0000,role0001,role0002
        public static bool IsInStrings(string key, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            bool result = false;
            string[] temp = SplitString(value);
            if (temp != null && temp.Length > 0)
            {
                foreach (var item in temp)
                {
                    if (item.Equals(key, stringComparison))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        //role=>role=role0000
        public static bool IsInIniStrings(string key, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            bool result = false;
            string[] temp = SplitString(value);
            if (temp != null && temp.Length > 0)
            {
                foreach (var item in temp)
                {
                    if (stringComparison == StringComparison.InvariantCultureIgnoreCase || stringComparison == StringComparison.CurrentCultureIgnoreCase || stringComparison == StringComparison.OrdinalIgnoreCase)
                    {
                        if (item.ToLower().Contains(key.ToLower()))
                        {
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        if (item.Contains(key))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        //读取ini字符流中的某个属性的值
        public static string ReadIniStringKeyValue(string iniStrings, string key)
        {
            return ReadIniStringKeyValue(iniStrings, key, _separator);
        }

        //读取ini字符流中的某个属性的值
        public static string ReadIniStringKeyValue(string iniStrings, string key, char[] separator)
        {
            string result = null;
            string[] temp = MyStringHelper.SplitString(iniStrings, separator);

            if (temp != null && temp.Length > 0)
            {
                foreach (var item in temp)
                {
                    if (item.ToLower().Contains(key.ToLower()))
                    {
                        result = item.Substring(key.Length + 1);
                        //break;
                        return result;
                    }
                }
            }
            return result;
        }

        public static string[] SplitString(string value)
        {
            return string.IsNullOrWhiteSpace(value) ?
                new string[] { } : value.Split(_separator);
        }

        public static string[] SplitString(string value, char[] separator)
        {
            return value.Split(separator);
        }

        public static string MakeSplitString(string[] value, char separator)
        {
            lock (_lock)
            {
                StringBuilder sb = new StringBuilder();
                if (value == null || value.Length == 0)
                {
                    sb.AppendFormat("");
                }
                else
                {
                    foreach (var item in value)
                    {
                        sb.AppendFormat("{0}{1}", separator, item);
                    }
                    sb.Remove(0, 1);
                }
                return sb.ToString();
            }
        }

        public static string MakeSplitString(string[] value)
        {
            return MakeSplitString(value, ',');
        }

        public static string ReplaceString(string value, string oldV, string newV = ".")
        {
            return value.Replace(oldV, newV);
        }

        public static string ReplaceString(string value, string[] oldV, string newV = ".")
        {
            string result = value;
            foreach (var old in oldV)
            {
                result = ReplaceString(result, old, newV);
            }
            return result;
        }
    }
}