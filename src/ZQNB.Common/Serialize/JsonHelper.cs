using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZQNB.Common.Serialize
{
    public class JsonHelper
    {
        public static IJsonSerialize JsonSerialize { get; set; }

        static JsonHelper()
        {
            JsonSerialize = new NewtonJsonSerialize();
        }
        
        public static string Serialize(Object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false)
        {
            var result = JsonSerialize.Serialize(value, formatting, camelCase);
            return result;
        }

        public static T Deserialize<T>(string json, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false)
        {
            var result = JsonSerialize.Deserialize<T>(json, formatting, camelCase);
            return result;
        }

        /// <summary>
        /// 序列化文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="value"></param>
        /// <param name="formatting"></param>
        /// <param name="camelCase"></param>
        /// <param name="encoding">如果为空，默认Encoding.UTF8</param>
        public static void SerializeFile(string filePath, Object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false, Encoding encoding = null)
        {
            JsonSerialize.SerializeFile(filePath, value, formatting, camelCase, encoding);
        }

        /// <summary>
        /// 反序列化文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="formatting">默认不缩进</param>
        /// <param name="camelCase">默认false</param>
        /// <param name="encoding">如果为空，默认Encoding.UTF8</param>
        /// <returns></returns>
        public static T DeserializeFile<T>(string filePath, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false, Encoding encoding = null)
        {

            var result = JsonSerialize.DeserializeFile<T>(filePath, formatting, camelCase, encoding);
            return result;
        }
    }
}
