using System;
using System.Text;

using Newtonsoft.Json;

namespace ZQNB.Common.Serialize
{
    public interface IJsonSerialize
    {
        string Serialize(Object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false);

        T Deserialize<T>(string json, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false);

        void SerializeFile(string filePath, Object value, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false,
            Encoding encoding = null);

        T DeserializeFile<T>(string filePath, NbJsonFormatting formatting = NbJsonFormatting.None, bool camelCase = false,
            Encoding encoding = null);
    }
}