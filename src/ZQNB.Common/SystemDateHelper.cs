using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQNB.Common
{
    public class SystemDateHelper
    {
        /// <summary>
        /// yyyyMMdd HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string DisplayNow()
        {
            var dateTime = Now();
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        public static DateTime Now()
        {
            var dateTime = defaultGetSystemDateFunc.Invoke();
            return dateTime;
        }

        /// <summary>
        /// 重新设置标准系统时间
        /// </summary>
        /// <param name="getSystemDateFunc"></param>
        public static void SetLogFunc(Func<DateTime> getSystemDateFunc)
        {
            if (getSystemDateFunc != null)
            {
                defaultGetSystemDateFunc = getSystemDateFunc;
            }
        }


        private static Func<DateTime> defaultGetSystemDateFunc = new Func<DateTime>(GetSystemDate);
        private static DateTime GetSystemDate()
        {
            return DateTime.Now;
        }
    }
}
