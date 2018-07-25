using System;
using System.Configuration;
using System.Xml;

namespace ZQNB.Common
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public class MyConfigHelper
    {
        /// <summary>
        /// 读取配置（AppSetting）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string key, string defaultValue)
        {
            string result = defaultValue;
            //如果后台有设置，以config的设置为准
            string settingValue = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(settingValue))
            {
                result = settingValue;
            }
            return result;
        }

        /// <summary>
        /// 读取配置（AppSetting）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetAppSettingValueAsBool(string key, bool defaultValue)
        {
            bool result = defaultValue;
            //如果后台有设置，以config的设置为准
            string settingValue = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(settingValue))
            {
                bool.TryParse(settingValue, out result);
            }
            return result;
        }

        /// <summary>
        /// 转换成T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAppSettingValueAs<T>(string key, T defaultValue)
        {
            T result = defaultValue;
            //如果后台有设置，以config的设置为准
            string settingValue = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(settingValue))
            {
                result = MyConvert<T>(settingValue);
            }
            return result;
        }


        public static T MyConvert<T>(object data)
        {
            return (T)Convert.ChangeType(data, typeof(T));
        }

        /// <summary>
        /// 修改Web.config，将会导致应用重启！
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configValue"></param>
        /// <returns></returns>
        public static MessageResult ChangeWebAppSetting(string configKey, string configValue)
        {
            try
            {
                var webConfigPath = MyPathHelper.MakeWebConfigPath();
                var xDoc = new XmlDocument();
                xDoc.Load(webConfigPath);
                var nodeList = xDoc.GetElementsByTagName("appSettings");
                var nodeAppSettings = nodeList[0].ChildNodes;
                foreach (XmlNode item in nodeAppSettings)
                {
                    if (item.Name.ToLower() == "add")
                    {
                        if (item.Attributes != null)
                        {
                            XmlAttribute key = item.Attributes["key"];
                            if (key != null && key.Value == configKey)
                            {
                                XmlAttribute value = item.Attributes["value"];
                                if (value != null)
                                {
                                    value.Value = configValue;
                                    xDoc.Save(webConfigPath);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageResult(false, string.Format("修改配置{0}失败:{1}", configKey, ex.Message));
            }
            return new MessageResult(true, string.Format("修改配置{0}成功", configKey));
        }
    }
}
