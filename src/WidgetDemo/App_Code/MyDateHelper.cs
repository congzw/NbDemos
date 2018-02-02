using System;

namespace WidgetDemo
{
    public interface IMyDateHelper
    {
        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        DateTime Now();
    }

    public class MyDateHelper : IMyDateHelper
    {
        #region for di extensions

        private static Func<IMyDateHelper> _resolve = () => ResolveAsSingleton.Resolve<MyDateHelper, IMyDateHelper>();
        public static Func<IMyDateHelper> Resolve
        {
            get { return _resolve; }
            set { _resolve = value; }
        }

        #endregion

        public DateTime Now()
        {
            return DateTime.Now;
        }
    }

    public class MockMyDateHelper : IMyDateHelper
    {
        public DateTime Now()
        {
            return _enabled ? _getMockDateFunc() : DateTime.Now;
        }

        //init
        static MockMyDateHelper()
        {
            //read from config
            var mockSetting = MyConfigHelper.Resolve().GetAppSettingValue(Config_Common_MockDate, null);
            if (string.IsNullOrWhiteSpace(mockSetting))
            {
                _getMockDateFunc = () => DateTime.Now;
                return;
            }

            var values = mockSetting.Split(',');
            _mockDateYear = TryGetIntValue(values, 0);
            _mockDateMonth = TryGetIntValue(values, 1);
            _mockDateDay = TryGetIntValue(values, 2);
            _mockDateHour = TryGetIntValue(values, 3);
            _mockDateMinute = TryGetIntValue(values, 4);
            _mockDateSecond = TryGetIntValue(values, 5);

            _getMockDateFunc = () => new DateTime(
                _mockDateYear ?? DateTime.Now.Year
                , _mockDateMonth ?? DateTime.Now.Month
                , _mockDateDay ?? DateTime.Now.Day
                , _mockDateHour ?? DateTime.Now.Hour
                , _mockDateMinute ?? DateTime.Now.Minute
                , _mockDateSecond ?? DateTime.Now.Second);
        }

        public static void Enabled()
        {
            _enabled = true;
            _backupFunc = MyDateHelper.Resolve;
            MyDateHelper.Resolve = () => MockLazy.Value;
        }

        public static void Disabled()
        {
            _enabled = false;
            MyDateHelper.Resolve = _backupFunc;
        }

        private static Func<IMyDateHelper> _backupFunc = null;
        private static readonly Lazy<MockMyDateHelper> MockLazy = new Lazy<MockMyDateHelper>(() => new MockMyDateHelper());
        private static string Config_Common_MockDate = "Config.Common.MockDate";
        private static int? _mockDateYear = null;
        private static int? _mockDateMonth = null;
        private static int? _mockDateDay = null;
        private static int? _mockDateHour = null;
        private static int? _mockDateMinute = null;
        private static int? _mockDateSecond = null;
        private static Func<DateTime> _getMockDateFunc = null;
        private static bool _enabled = false;
        private static int? TryGetIntValue(string[] values, int index)
        {
            if (values.Length < index + 1)
            {
                return null;
            }
            int value;
            var tryParse = int.TryParse(values[index], out value);
            if (!tryParse)
            {
                return null;
            }
            return value;
        }
    }
}