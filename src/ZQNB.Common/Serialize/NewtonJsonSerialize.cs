using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ZQNB.Common.Serialize
{
    public class NewtonJsonSerialize : IJsonSerialize
    {
        public string Serialize(Object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false)
        {
            var setting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            if (camelCase)
            {
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            return JsonConvert.SerializeObject(value, (Formatting)formatting, setting);
        }

        public T Deserialize<T>(string json, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false)
        {
            var setting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            setting.Formatting = (Formatting)formatting;

            if (camelCase)
            {
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            return JsonConvert.DeserializeObject<T>(json, setting);
        }

        /// <summary>
        /// 序列化文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="value"></param>
        /// <param name="formatting"></param>
        /// <param name="camelCase"></param>
        /// <param name="encoding">如果为空，默认Encoding.UTF8</param>
        public void SerializeFile(string filePath, Object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false, Encoding encoding = null)
        {
            var setting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            if (camelCase)
            {
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            string jsonValue = JsonConvert.SerializeObject(value, (Formatting)formatting, setting);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            File.WriteAllText(filePath, jsonValue, encoding);
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
        public T DeserializeFile<T>(string filePath, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false, Encoding encoding = null)
        {
            var setting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            setting.Formatting = (Formatting)formatting;

            if (camelCase)
            {
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            string json = File.ReadAllText(filePath, encoding);
            return JsonConvert.DeserializeObject<T>(json, setting);
        }
    }
}