using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace ZQNB.Common.Db
{
    /// <summary>
    /// 用来快速异步测试数据库连接是否可用的辅助类
    /// </summary>
    public static class MySqlConnectionTester
    {
        static bool result = false;

        /// <summary>
        /// 异步的测试方法
        /// </summary>
        /// <param name="connectionString">连接字符串的内容</param>
        /// <param name="millsecond">超时的毫秒数</param>
        /// <param name="connectionStateMessage"></param>
        /// <returns></returns>
        public static bool RunTest(string connectionString, int millsecond, out string connectionStateMessage)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionStateMessage = "目前没有可用的数据库连接配置，请添加";
                return false;
            }
            else
            {
                MySqlConnectionTestTool testTool = new MySqlConnectionTestTool(connectionString);
                testTool.RunTest(millsecond, out result, out connectionStateMessage);
                return result;
            }
        }
    }

    #region internal MySqlConnectionTestTool

    internal class MySqlConnectionTestTool
    {
        private bool testResult = false;
        private string testMessage = string.Empty;
        private string connString = string.Empty;
        private AutoResetEvent sleepSync = new AutoResetEvent(false);

        public MySqlConnectionTestTool(string connString)
        {
            this.connString = connString;
        }

        private void Test()
        {
            if (string.IsNullOrEmpty(this.connString))
            {
                testResult = false;
            }
            else
            {
                try
                {
                    //DbConnectionStringBuilder builder = new DbConnectionStringBuilder(false);

                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connString);

                    IPHostEntry iphost = new IPHostEntry();

                    builder.ConnectionString = connString;

                    string hostName = builder.DataSource;
                    if (hostName.StartsWith("."))
                    {
                        ////string selfName = ".\\SQLEXPRESS";
                        ////string selfName = ".";
                        ////string selfName = "127.0.0.1";
                        //string selfName = iphost.HostName.ToUpper();
                        //可能是自己的主机
                        hostName = "127.0.0.1";
                    }
                    iphost.HostName = hostName;
                    Ping ping = new Ping();
                    PingReply reply = ping.Send(iphost.HostName, 200);

                    if (reply.Status == IPStatus.Success)
                    {
                        using (SqlConnection conn = new SqlConnection(this.connString))
                        {
                            try
                            {
                                conn.Open();
                                testMessage = "测试完毕，配置正确。";
                                testResult = true;
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                testMessage = "测试完毕，连接异常。" + ex.Message;
                                testResult = false;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    testMessage = "非法的连接字符串。";// + ex.Message;
                    testResult = false;
                }
            }
            sleepSync.Set();
        }

        public void RunTest(int millsecond, out bool result, out string message)
        {
            Thread controlThread = new Thread(new ThreadStart(Test));
            controlThread.Start();
            if (!sleepSync.WaitOne(millsecond, false))
            {
                testMessage = "连接超时";
                testResult = false;
            }
            result = testResult;
            message = testMessage;
        }

    }

    #endregion
}