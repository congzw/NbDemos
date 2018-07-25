using System;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ZQNB.Common.Serialize
{
    public static class MySerializeXmlHelper
    {
        static MySerializeXmlHelper() { }

        private static object _lock = new object();

        #region 泛型支持

        //为不支持动态参数的老接口保留，请勿删除此方法
        /// <summary>
        /// 使用XmlSerializer序列化对象
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型，不必须声明[Serializable]特征</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToXmlText<T>(T obj)
        {
            Type t = obj.GetType();
            return SerializeToXmlText(obj, t);
        }

        /// <summary>
        /// 使用XmlSerializer序列化对象
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型，不必须声明[Serializable]特征</typeparam>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="ignoreNamespace">是否忽略命名空间（默认true）</param>
        /// <returns></returns>
        public static string SerializeToXmlText<T>(T obj, bool ignoreNamespace = true)
        {
            //Type t = typeof(T);
            Type t = obj.GetType();
            return SerializeToXmlText(obj, t, ignoreNamespace);
        }

        /// <summary>
        /// 使用XmlSerializer序列化对象
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型，不必须声明[Serializable]特征</typeparam>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="filePath"></param>
        public static void SerializeFile<T>(T obj, string filePath)
        {
            //Type t = typeof(T);
            Type t = obj.GetType();
            SerializeFile(obj, t, filePath);
        }

        /// <summary>
        /// 使用XmlSerializer序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <param name="ignoreNamespace"></param>
        public static void SerializeFile<T>(T obj, string filePath, bool ignoreNamespace = true)
        {
            //Type t = typeof(T);
            Type t = obj.GetType();
            SerializeFile(obj, t, filePath, ignoreNamespace);
        }
        
        /// <summary>
        /// 使用XmlSerializer反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlOfObject">需要反序列化的xml字符串</param>
        /// <returns></returns>
        public static T DeserializeFromXmlText<T>(string xmlOfObject)
        {
            Type t = typeof(T);
            return (T)DeserializeFromXmlText(xmlOfObject, t);
        }

        /// <summary>
        /// 使用XmlSerializer反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeSerializeFile<T>(string filePath)
        {
            Type t = typeof(T);
            return (T)DeSerializeFile(filePath, t);
        }

        #endregion

        #region 非泛型支持

        //为不支持动态参数的老接口保留，请勿删除此方法
        /// <summary>
        /// 使用XmlSerializer序列化对象（不必须声明[Serializable]特征）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SerializeToXmlText(object obj, Type t)
        {
            return SerializeToXmlText(obj, t, true);
        }
        /// <summary>
        /// 使用XmlSerializer序列化对象（不必须声明[Serializable]特征）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <param name="ignoreNamespace"></param>
        /// <returns>序列化的xml字符串</returns>
        public static string SerializeToXmlText(object obj, Type t, bool ignoreNamespace = true)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(t);
                if (ignoreNamespace)
                {
                    //使其不生成命名空间属性
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    serializer.Serialize(ms, obj, ns);
                }
                else
                {
                    serializer.Serialize(ms, obj);
                }

                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        //为不支持动态参数的老接口保留，请勿删除此方法
        /// <summary>
        /// 使用XmlSerializer序列化对象（不必须声明[Serializable]特征）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <param name="t"></param>
        public static void SerializeFile(object obj, Type t, string filePath)
        {
            SerializeFile(obj, t, filePath, true);
        }
        /// <summary>
        /// 使用XmlSerializer序列化对象（不必须声明[Serializable]特征）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <param name="t"></param>
        /// <param name="ignoreNamespace"></param>
        public static void SerializeFile(object obj, Type t, string filePath, bool ignoreNamespace = true)
        {
            string temp = SerializeToXmlText(obj, t, ignoreNamespace);
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(temp);
                sw.Close();
            }
        }

        /// <summary>
        /// 使用XmlSerializer反序列化对象
        /// </summary>
        /// <param name="xmlOfObject">需要反序列化的xml字符串</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object DeserializeFromXmlText(string xmlOfObject, Type t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sr = new StreamWriter(ms, Encoding.UTF8))
                {
                    sr.Write(xmlOfObject);
                    sr.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    XmlSerializer serializer = new XmlSerializer(t);
                    return serializer.Deserialize(ms);
                }
            }
        }

        /// <summary>
        /// 使用XmlSerializer反序列化对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object DeSerializeFile(string filePath, Type t)
        {
            string temp = File.ReadAllText(filePath, Encoding.UTF8);
            return DeserializeFromXmlText(temp, t);
        }

        #endregion
    }
}
