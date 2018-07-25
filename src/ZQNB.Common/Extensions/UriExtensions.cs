using System;
using System.Collections.Generic;
using System.Web;

namespace ZQNB.Common.Extensions
{
    /// <summary>
    /// 获取地址的参数
    /// todo 写到一半发现System.Net.Http 提供了一个类似方法 先用着 有时间写完
    /// </summary>
    public  static class UriExtensions
    {
        public static string Query(this Uri uri, string queryKey)
        {
            if (uri==null)
            {
                return null;
            }
            var query = uri.Query;
            if (query==""||query=="?")
            {
                return null;
            }
            if (query.StartsWith("?"))
            {
                query = query.Substring(0, 1);
                return Query(query, queryKey);
            }
            return null;
        }

        private static string Query(string queryString,string queryKey)
        {
            //todo 这样应该会有个缓存 存住处理过的字符串
            var query = new Dictionary<string, string>();

            var queryStringSets = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var set in queryStringSets)
            {
                var equalsIndex = set.IndexOf("=");
                if (equalsIndex == -1)
                {
                    query[HttpUtility.UrlDecode(set)] = String.Empty;
                    continue;
                }
                var key = HttpUtility.UrlDecode(set.Substring(0, equalsIndex));
                var value = HttpUtility.UrlDecode(set.Substring(equalsIndex + 1));
                query[key] = value;
            }
            if (query.ContainsKey(queryKey))
            {
                return query[queryKey];
            }
            return null;
        }
    }
}
