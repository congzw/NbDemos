using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZQNB.Common
{

    /*模型的帮助类*/
    public class MyModelHelper
    {
        public static void TryCopyProperties(Object updatingObj, Object collectedObj, params string[] excludeProperties)
        {
            if (collectedObj != null && updatingObj != null)
            {
                //获取类型信息
                Type updatingObjType = updatingObj.GetType();
                PropertyInfo[] updatingObjPropertyInfos = updatingObjType.GetProperties();

                Type collectedObjType = collectedObj.GetType();
                PropertyInfo[] collectedObjPropertyInfos = collectedObjType.GetProperties();

                string[] fixedExPropertites = excludeProperties ?? new string[] { };

                foreach (PropertyInfo updatingObjPropertyInfo in updatingObjPropertyInfos)
                {
                    foreach (PropertyInfo collectedObjPropertyInfo in collectedObjPropertyInfos)
                    {
                        if (updatingObjPropertyInfo.Name == collectedObjPropertyInfo.Name)
                        {
                            if (fixedExPropertites.Contains(updatingObjPropertyInfo.Name,
                                StringComparer.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            object value = collectedObjPropertyInfo.GetValue(collectedObj, null);
                            if (updatingObjPropertyInfo.CanWrite)
                            {
                                updatingObjPropertyInfo.SetValue(updatingObj, value, null);
                            } 
                            //try
                            //{
                            //    object value = collectedObjPropertyInfo.GetValue(collectedObj, null);
                            //    updatingObjPropertyInfo.SetValue(updatingObj, value, null);
                            //}
                            //catch (Exception ex)
                            //{
                            //    string temp = ex.Message;
                            //}
                            break;
                        }
                    }
                }
            }
        }

        public static void SetProperties<T>(T theObjToBeUpdated, T readFromValueObj, string[] excludeProperties = null) where T : class
        {
            if (theObjToBeUpdated == null)
            {
                throw new ArgumentNullException("theObjToBeUpdated");
            }
            if (readFromValueObj == null)
            {
                throw new ArgumentNullException("readFromValueObj");
            }

            //获取类型信息
            Type t = theObjToBeUpdated.GetType();
            PropertyInfo[] propertyInfos = t.GetProperties();
            string[] fixedExPropertites = excludeProperties ?? new string[] { };

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (fixedExPropertites.Contains(propertyInfo.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }
                object value = propertyInfo.GetValue(readFromValueObj, null);
                propertyInfo.SetValue(theObjToBeUpdated, value, null);
            }

        }

        public static bool SetProperty<T>(T obj, string key, object value)
        {
            bool reslut = false;
            if (obj != null && !string.IsNullOrEmpty(key) && value != null)
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();

                foreach (PropertyInfo var in propertyInfos)
                {
                    if (var.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var.SetValue(obj, value, null);
                        reslut = true;
                        break;
                    }
                }
            }
            return reslut;
        }

        /// <summary>
        /// 查找泛型T的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetProperty<T>(T obj, string key)
        {
            object result = null;
            if (obj != null && !string.IsNullOrEmpty(key))
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();

                foreach (PropertyInfo var in propertyInfos)
                {
                    if (var.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        object value = var.GetValue(obj, null);
                        result = value;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查找obj的所有属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetProperty(Object obj, string key)
        {
            object result = null;
            if (obj != null && !string.IsNullOrEmpty(key))
            {
                //获取类型信息
                Type t = obj.GetType();
                PropertyInfo[] propertyInfos = t.GetProperties();

                foreach (PropertyInfo var in propertyInfos)
                {
                    if (var.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        object value = var.GetValue(obj, null);
                        result = value;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有的Property名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetKeys<T>()
        {
            List<string> result = new List<string>();
            //获取类型信息
            Type t = typeof(T);
            PropertyInfo[] propertyInfos = t.GetProperties();

            foreach (PropertyInfo var in propertyInfos)
            {
                result.Add(var.Name);
            }
            return result;
        }

        public static Dictionary<string, object> GetKeyValueDictionary<T>(T obj)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj != null)
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();

                foreach (PropertyInfo var in propertyInfos)
                {
                    result.Add(var.Name, var.GetValue(obj, null));
                }
            }
            return result;
        }

        public static bool SetKeyValueDictionary<T>(T obj, Dictionary<string, object> dic)
        {
            bool reslut = false;
            if (obj != null && dic != null)
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();
                foreach (PropertyInfo var in propertyInfos)
                {
                    foreach (var key in dic.Keys)
                    {
                        if (var.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                        {
                            object value = null;
                            bool temp = dic.TryGetValue(key, out value);
                            if (temp)
                            {
                                var.SetValue(obj, value, null);
                                break;
                            }
                        }
                    }
                }
            }
            return reslut;
        }

        public static void ResetAllStringEmpty<T>(T obj)
        {
            if (obj != null)
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();
                foreach (PropertyInfo var in propertyInfos)
                {
                    object value = var.GetValue(obj, null);
                    if (value == null && var.PropertyType.Name == string.Empty.GetType().Name)
                    {
                        var.SetValue(obj, string.Empty, null);
                    }
                }
            }
        }

        //包括string和datetime
        public static void ResetAllDateDefault<T>(T obj, DateTime defaultDate)
        {
            if (obj != null)
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();
                foreach (PropertyInfo var in propertyInfos)
                {
                    object value = var.GetValue(obj, null);
                    if (var.PropertyType.Name == DateTime.Now.GetType().Name)
                    {
                        var.SetValue(obj, defaultDate, null);
                    }
                }
            }
        }

        public static List<string> GetPublicMethods<T>()
        {
            List<string> result = new List<string>();
            //获取类型信息
            Type t = typeof(T);
            MethodInfo[] infos = t.GetMethods();

            foreach (MethodInfo var in infos)
            {
                result.Add(var.Name);
            }

            result.Remove("ToString");
            result.Remove("Equals");
            result.Remove("GetHashCode");
            result.Remove("GetType");
            return result;
        }

        public static string MakeIniString(Object obj, bool removeLastlastSplit = true)
        {
            string temp = MakeIniStringExt(obj, removeLastlastSplit: true);
            return temp;
        }

        public static string MakeIniStringExt(Object obj, string equalOperator = "=", string lastSplit = ";", bool removeLastlastSplit = true)
        {
            string schema = string.Format("{0}{1}{2}{3}", "{0}", equalOperator, "{1}", lastSplit);
            StringBuilder sb = new StringBuilder();
            if (obj != null)
            {
                //获取类型信息
                Type t = obj.GetType();
                PropertyInfo[] propertyInfos = t.GetProperties();
                foreach (PropertyInfo var in propertyInfos)
                {
                    object value = var.GetValue(obj, null);
                    string temp = "";

                    //如果是string，并且为null
                    if (value == null)
                    {
                        temp = "";
                    }
                    else
                    {
                        temp = value.ToString();
                    }

                    value = MyStringHelper.ReplaceString(temp, new[] { lastSplit, "=" });
                    sb.AppendFormat(schema, var.Name, value);
                }
            }
            //去掉最后的分号
            if (removeLastlastSplit)
            {
                string result = sb.ToString();
                return result.Substring(0, result.Length - 1);
            }
            else
            {
                return sb.ToString();
            }
        }

        //Model object => object[] args
        //Model class  => string args
        public static object[] MakeArgs<T>(T obj)
        {
            Dictionary<string, object> dic = GetKeyValueDictionary<T>(obj);
            object[] result = dic.Values.ToArray();
            return result;
        }

        public static string MakeSpArgs<T>()
        {
            StringBuilder sb = new StringBuilder();
            List<string> propList = GetKeys<T>();
            if (propList.Count == 0)
            {
                return "";
            }

            int length = propList.Count;
            for (int i = 0; i < length - 1; i++)
            {
                sb.Append(propList[i]);
                sb.Append(",");
                sb.AppendLine();
            }

            sb.Append(propList[length - 1]);
            return sb.ToString();
        }

        public static List<T> Where<T>(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            List<T> result = new List<T>();
            if (enumerable == null)
            {
                return result;
            }
            IEnumerable<T> temp = enumerable.Where(predicate);
            if (temp != null)
            {
                result = temp.ToList();
            }

            return result;
        }

        public static string[] MakeIdArrays<T>(IEnumerable<T> enumerable, Func<T, string> getId)
        {
            List<T> result = new List<T>();
            if (enumerable == null)
            {
                return new string[] { };
            }

            List<string> temp = new List<string>();
            foreach (var item in enumerable)
            {
                string id = getId.Invoke(item);
                temp.Add(id);
            }
            return temp.ToArray();
        }

        public static void LookupProperties(Object obj, StringBuilder sb)
        {
            if (obj != null && sb != null)
            {
                //获取类型信息
                Type collectedObjType = obj.GetType();
                PropertyInfo[] collectedObjPropertyInfos = collectedObjType.GetProperties();
                foreach (PropertyInfo collectedObjPropertyInfo in collectedObjPropertyInfos)
                {
                    object value = collectedObjPropertyInfo.GetValue(obj, null);
                    sb.AppendFormat("{0}={1};", collectedObjPropertyInfo.Name, value);
                }
            }
        }
    }
}
