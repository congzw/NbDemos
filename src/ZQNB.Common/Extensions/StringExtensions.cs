using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQNB.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 截取等宽中英文字符串
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="length">要截取的中文字符长度</param>
        /// <param name="appendStr">截取后后追加的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string CutStr(this string str, int length, string appendStr="")
        {
            if (str == null) return string.Empty;

            var len = length * 2;
            int aequilateLength = 0, cutLength = 0;
            var encoding = Encoding.GetEncoding("gb2312");

            var cutStr = str;
            var strLength = cutStr.Length;
            byte[] bytes;
            for (int i = 0; i < strLength; i++)
            {
                bytes = encoding.GetBytes(cutStr.Substring(i, 1));
                if (bytes.Length >= 2)//不是英文
                    aequilateLength += 2;
                else
                    aequilateLength++;

                if (aequilateLength <= len) cutLength += 1;

                if (aequilateLength > len)
                    return cutStr.Substring(0, cutLength) + appendStr;
            }
            return cutStr;
        }
        /// <summary>
        /// 如果为空  显示未知
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string IsEmptyStr(this string str)
        {
            string newStr = str;
            if (String.IsNullOrEmpty(str))
            {
                newStr = "未知";
            }
            return newStr;
        }
        public static string FormatSelf(this string self, params object[] args)
        {
            Assertion.NotNull(self);
            return string.Format(self, args);
        }

        public static bool IsNullOrWhiteSpace(this string self)
        {

            //return string.IsNullOrEmpty(self) || self.Trim().Length == 0;
            return string.IsNullOrWhiteSpace(self);
        }
    }
}
