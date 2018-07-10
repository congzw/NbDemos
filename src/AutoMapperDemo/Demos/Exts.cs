using Newtonsoft.Json;

namespace AutoMapperDemo.Demos
{
    public static class Exts
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
