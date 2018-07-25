namespace ZQNB.Common
{
    public class NbRegistry
    {
        public string CurrentProjectPrefix = "ZQNB";
        public string Config_Common_ConnName = "Config.Common.ConnName";
        public string Config_Common_LogPrefix = "Config.Common.LogPrefix";

        static NbRegistry()
        {
            Instance = new NbRegistry();
        }
        public static NbRegistry Instance { get; set; }
    }
}