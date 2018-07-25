using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHibernate.Proxy;
using ZQNB.Common.Serialize;

namespace ZQNB.Common.NHExtensions
{
    public class NhJsonSerialize : IJsonSerialize
    {
        public string Serialize(object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false)
        {
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new NHibernateContractResolver(),
                //循环问题
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //,PreserveReferencesHandling = PreserveReferencesHandling.All
            };
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

        public void SerializeFile(string filePath, object value, NbJsonFormatting formatting = NbJsonFormatting.None,
            bool camelCase = false, Encoding encoding = null)
        {
            var jsonValue = Serialize(value, formatting, camelCase);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            File.WriteAllText(filePath, jsonValue, encoding);
        }

        public T DeserializeFile<T>(string filePath, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false,
            Encoding encoding = null)
        {
            string value = encoding != null ? File.ReadAllText(filePath, encoding) : File.ReadAllText(filePath);
            return Deserialize<T>(value, formatting, camelCase);
        }
    }

    public class NHibernateContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (typeof(INHibernateProxy).IsAssignableFrom(objectType))
            {
                return base.GetSerializableMembers(objectType.BaseType);
            }
            return base.GetSerializableMembers(objectType);
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            if (typeof(INHibernateProxy).IsAssignableFrom(objectType))
            {
                return base.CreateContract(objectType.BaseType);
            }
            return base.CreateContract(objectType);
        }
    }
}