using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MvcDemo.Api.Libs
{
    public class ObjectHashHelper
    {
        /// <summary>
        /// 获取对象Hash值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string CreateObjectHash(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            var json = SerializeObjectFunc(obj);
            string hashString = EasyMD5.Hash(json);
            return hashString;
        }
        /// <summary>
        /// 校验对象Hash值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyObjectHash(object obj, string hash)
        {
            var json = SerializeObjectFunc(obj);
            return EasyMD5.Verify(json, hash);
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="to"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static bool PublicPropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                var type = typeof(T);
                var ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        var propertyInfo = type.GetProperty(pi.Name);
                        if (propertyInfo == null)
                        {
                            continue;
                        }
                        object selfValue = propertyInfo.GetValue(self, null);
                        object toValue = propertyInfo.GetValue(to, null);
                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }

        #region support customize 

        private static Func<object, string> _serializeObjectFunc = o => JsonConvert.SerializeObject(o);
        public static Func<object, string> SerializeObjectFunc
        {
            get { return _serializeObjectFunc; }
            set { _serializeObjectFunc = value; }
        }

        #endregion

        internal class EasyMD5
        {
            public static string Hash(string data)
            {
                using (var md5 = MD5.Create())
                {
                    return GetMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)));
                }
            }
            public static bool Verify(string data, string hash)
            {
                using (var md5 = MD5.Create())
                {
                    return VerifyMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)), hash);
                }
            }

            //helpers
            private static string GetMd5Hash(byte[] data)
            {
                var sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            private static bool VerifyMd5Hash(byte[] data, string hash)
            {
                return 0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(data), hash);
            }
        }
    }

    #region extensions

    public static class ObjectHashExtensions
    {
        /// <summary>
        /// 获取对象Hash值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string CreateObjectHash(this object obj)
        {
            return ObjectHashHelper.CreateObjectHash(obj);
        }

        /// <summary>
        /// 校验对象Hash值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyObjectHash(this object obj, string hash)
        {
            return ObjectHashHelper.VerifyObjectHash(obj, hash);
        }
    }

    #endregion
}
