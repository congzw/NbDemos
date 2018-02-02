using System;
using System.Configuration;

namespace WidgetDemo
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public interface IMyConfigHelper
    {
        /// <summary>
        /// 读取配置（AppSetting）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string GetAppSettingValue(string key, string defaultValue);

        /// <summary>
        /// 读取配置（AppSetting）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool GetAppSettingValueAsBool(string key, bool defaultValue);
    }

    /// <summary>
    /// 配置帮助类
    /// </summary>
    public class MyConfigHelper : IMyConfigHelper
    {
        #region for di extensions

        private static Func<IMyConfigHelper> _resolve = () => ResolveAsSingleton.Resolve<MyConfigHelper, IMyConfigHelper>();
        public static Func<IMyConfigHelper> Resolve
        {
            get { return _resolve; }
            set { _resolve = value; }
        }

        #endregion

        /// <summary>
        /// 读取配置（AppSetting）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetAppSettingValue(string key, string defaultValue)
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
        public bool GetAppSettingValueAsBool(string key, bool defaultValue)
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
    }
}
