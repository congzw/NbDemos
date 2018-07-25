using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZQNB.Common.Serialize
{
    /// <summary>
    /// DynamicObjectHelperdynamic跨类库调用的帮助类
    /// </summary>
    public class DynamicObjectHelper
    {
        /// <summary>
        /// 解决跨类库dynamic数据类型不可见的问题
        /// （dynamic调用时报RuntimeBinderException：“object”未包含“xxx”的定义错误）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public dynamic ConvertDynamic(dynamic obj)
        {
            var serializeObject = JsonConvert.SerializeObject(obj);
            dynamic deserializeObject = JsonConvert.DeserializeObject(serializeObject);
            return deserializeObject;
        }
    }
}
