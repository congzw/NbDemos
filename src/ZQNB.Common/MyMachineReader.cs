using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ZQNB.Common
{
    public class MyMachineReader
    {
        private static object _lock = new object();

        public static string ReadAllMacAndMakeString()
        {
            List<String> list = ReadAllMac();
            if (list == null || list.Count == 0)
            {
                return "";
            }
            list.Sort();
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        public static string ReadFirstMac()
        {
            UtilsLogger.LogMessage(string.Format("ReadFirstMac()"));
            List<String> list = ReadAllMac();
            string code = SortListAndReadFirst(list);
            UtilsLogger.LogMessage(string.Format("ReadFirstMac() => code: {0}", code));
            return code;
        }

        public static string ReadFirstCpu()
        {
            UtilsLogger.LogMessage(string.Format("ReadFirstCpu()"));
            List<String> list = ReadAllCpu();
            string code = SortListAndReadFirst(list);
            UtilsLogger.LogMessage(string.Format("ReadFirstCpu() => code: {0}", code));
            return code;
        }

        public static string ReadFirstIp()
        {
            UtilsLogger.LogMessage(string.Format("ReadFirstIp()"));
            List<String> list = ReadAllIp();
            string code = SortListAndReadFirst(list);
            UtilsLogger.LogMessage(string.Format("ReadFirstIp() => code: {0}", code));
            return code;
        }

        public static string ReadFirstDisk()
        {
            UtilsLogger.LogMessage(string.Format("ReadFirstDisk()"));
            List<String> list = ReadAllDisk();
            string code = SortListAndReadFirst(list);
            UtilsLogger.LogMessage(string.Format("ReadFirstDisk() => code: {0}", code));
            return code;
        }

        public static string SortListAndReadFirst(List<String> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            list.Sort();
            return list[0];
        }

        public static void SortList(ref List<String> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            list.Sort();
        }

        //mac
        public static List<String> ReadAllMac()
        {
            List<String> list = new List<string>();

            //解决多个mac冲突的问题
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nis)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    PhysicalAddress pa = ni.GetPhysicalAddress();
                    string mac = pa.ToString();
                    list.Add(mac);
                }
            }
            return list;
        }

        //cpu
        public static List<String> ReadAllCpu()
        {
            lock (_lock)
            {
                List<String> list = new List<string>();
                try
                {
                    using (ManagementObjectSearcher s = new ManagementObjectSearcher("SELECT * FROM Win32_PRocessor"))
                        foreach (ManagementObject mo in s.Get())
                        {
                            try
                            {
                                list.Add(mo.Properties["ProcessorId"].Value.ToString());
                            }
                            catch (Exception ex)
                            {
                                UtilsLogger.LogMessage(string.Format("ex in ReadAllCpu() =>{0}", ex.Message));
                            }
                        }

                    //using (ManagementClass mc = new ManagementClass("Win32_Processor"))
                    //{
                    //    ManagementObjectCollection moc = mc.GetInstances();
                    //    foreach (ManagementObject mo in moc)
                    //    {
                    //        try
                    //        {
                    //            list.Add(mo.Properties["ProcessorId"].Value.ToString());
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            UtilsLogger.LogMessage(string.Format("ex in ReadAllCpu() =>{0}", ex.Message));
                    //        }
                    //    }
                    //}
                    UtilsLogger.LogMessage(string.Format("ReadAllCpu()=>{0}", list.Count));
                    foreach (var item in list)
                    {
                        UtilsLogger.LogMessage(string.Format("cpu =>{0}", item));
                    }
                    UtilsLogger.LogMessage(string.Format("first cpu =>{0}", list[0]));
                }
                catch (Exception exAll)
                {
                    UtilsLogger.LogMessage(string.Format("exAll in ReadAllCpu() =>{0}", exAll.Message));
                }
                return list;
            }
        }

        //ip
        public static List<String> ReadAllIp()
        {
            List<String> list = new List<string>();
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            using (ManagementObjectCollection moc = mc.GetInstances())
            {
                foreach (ManagementObject mo in moc)
                {
                    try
                    {
                        System.Array array = (System.Array)(mo.Properties["IpAddress"].Value);
                        string info = array.GetValue(0).ToString();
                        list.Add(info);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return list;
        }

        //硬盘信息
        public static List<String> ReadAllDisk()
        {
            List<String> list = new List<string>();
            using (ManagementClass mc = new ManagementClass("Win32_DiskDrive"))
            using (ManagementObjectCollection moc = mc.GetInstances())
            {
                foreach (ManagementObject mo in moc)
                {
                    try
                    {
                        string info = mo.Properties["Model"].Value.ToString();
                        list.Add(info);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return list;
        }

        //win32_Name的的PropertyName信息，不保证获取成功
        public static List<String> ReadAllPropertyInfoByWin32Name(string win32_Name, string PropertyName)
        {
            List<String> list = new List<string>();
            using (ManagementClass mc = new ManagementClass("Win32_DiskDrive"))
            using (ManagementObjectCollection moc = mc.GetInstances())
            {
                foreach (ManagementObject mo in moc)
                {
                    try
                    {
                        string info = mo.Properties["Model"].Value.ToString();
                        list.Add(info);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return list;
        }


        private static void TraceMessage(string message)
        {
            UtilsLogger.LogMessage(message);
        }
    }
}
