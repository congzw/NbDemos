using System.Text;
using System.Web.WebPages;

namespace RequireJsDemoMvc
{
    public static class WebPageBaseExtensions
    {
        public static void SetAngularSupport(this WebPageBase webPageBase, bool value)
        {
            webPageBase.SetValueFor("AngularSupport", value);
        }

        public static bool GetAngularSupport(this WebPageBase webPageBase)
        {
            return webPageBase.GetValueFor("AngularSupport", true);
        }


        public static void SetDebugMode(this WebPageBase webPageBase, bool value)
        {
            webPageBase.SetValueFor("DebugMode", value);
        }

        public static bool GetDebugMode(this WebPageBase webPageBase)
        {
            return webPageBase.GetValueFor("DebugMode", true);
        }

        #region common helper

        public static void SetValueFor(this WebPageBase webPageBase, string name, bool value)
        {
            var nameKey = name.ToLower();
            //webPageBase.PageData[nameKey] = value;
            webPageBase.Context.Items[nameKey] = value;
        }

        public static bool GetValueFor(this WebPageBase webPageBase, string name, bool defaultValue)
        {
            var nameKey = name.ToLower();
            //if (webPageBase.PageData[nameKey] == null)
            //{
            //    return defaultValue;
            //}
            //return webPageBase.PageData[nameKey];
            if (webPageBase.Context.Items[nameKey] == null)
            {
                return defaultValue;
            }
            return webPageBase.Context.Items[nameKey].ToString().ToLower() == "true";
        }

        #endregion

        /// <summary>
        /// 截取汉字
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int count, string append = "")
        {

            if (value == null) return string.Empty;

            var len = count * 2;
            int aequilateLength = 0, cutLength = 0;
            var encoding = Encoding.GetEncoding("gb2312");

            var cutStr = value;
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
                    return cutStr.Substring(0, cutLength) + append;
            }
            return cutStr;
        }

        /// <summary>
        /// 截取组织名
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string TruncateOrgName(this string value, int count = 10, string append = "...")
        {
            return Truncate(value, count, append);
        }
    }
}