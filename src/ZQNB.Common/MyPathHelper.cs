using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZQNB.Common
{
    public class MyPathHelper
    {
        /// <summary>
        /// web\\App_Data
        /// </summary>
        /// <returns></returns>
        public static string MakeAppDataDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "App_Data");
            return dirPath;
        }

        /// <summary>
        /// web\\bin
        /// </summary>
        /// <returns></returns>
        public static string MakeWebBinDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "bin");
            return dirPath;
        }

        /// <summary>
        /// web\\web.config
        /// </summary>
        /// <returns></returns>
        public static string MakeWebConfigPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "web.config");
            return dirPath;
        }

        /// <summary>
        /// web\\Content
        /// </summary>
        /// <returns></returns>
        public static string MakeWebContentDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "Content");
            return dirPath;
        }

        /// <summary>
        /// web\\Areas
        /// </summary>
        /// <returns></returns>
        public static string MakeWebAreasDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "Areas");
            return dirPath;
        }

        /// <summary>
        /// web\\Areas\\Content
        /// </summary>
        /// <returns></returns>
        public static string MakeWebAreasContentDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "Content");
            return dirPath;
        }

        /// <summary>
        /// web\\Areas\\Views
        /// </summary>
        /// <returns></returns>
        public static string MakeWebAreasViewsDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string dirPath = string.Format(@"{0}{1}", temp, "Views");
            return dirPath;
        }

        public static string MakeWebRootPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            return temp;
        }

        /// <summary>  
        /// 获取程序执行的根目录
        /// Web：IIS Site目录
        /// EXE：current.exe同目录
        /// </summary>
        /// <returns></returns>
        public static string MakeExeDirPath()
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            return temp;
        }

        /// <summary>
        /// 获取程序执行的根目录
        /// 网站：IIS Site目录
        /// EXE：current.exe同目录
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string MakeFilePathInBaseDirectory(string fileName)
        {
            string temp = FixDirPath(AppDomain.CurrentDomain.BaseDirectory);
            string filePath = string.Format("{0}{1}", temp, fileName);
            return filePath;
        }

        /// <summary>
        /// app_data\xxx
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string MakeAppDataFilePath(string fileName)
        {
            string dirPath = MakeAppDataDirPath();
            string temp = FixDirPath(dirPath);
            string filePath = string.Format(@"{0}{1}", temp, fileName);
            return filePath;
        }

        /// <summary>
        /// 如果后面没有\\，补上\\
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FixDirPath(string value)
        {
            var temp = value.EndsWith("\\") ? value : value + "\\";
            return temp;
        }

        /// <summary>
        /// 连接路径
        /// c:\\test + a.b => c:\\test\\a.b
        /// c:\\test\\ + a.b => c:\\test\\a.b
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="subFolderOrFile"></param>
        /// <returns></returns>
        public static string JoinPath(string folder, string subFolderOrFile)
        {
            string temp = FixDirPath(folder);
            string fixedPath = string.Format(@"{0}{1}", temp, subFolderOrFile);
            return fixedPath;
        }

        /// <summary>
        /// 返回服务器的物理路径
        /// </summary>
        /// <param name="virtualPath">虚拟路径，如~/xxx</param>
        /// <returns></returns>
        public static string MapIISServerPath(string virtualPath)
        {
            string thePath = HttpContext.Current.Server.MapPath(virtualPath);
            return thePath;
        }
        
        /// <summary>
        /// 返回文件夹路径信息
        /// c:\a\b\c.exe => c:\a\b
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFolderPath(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            return dirPath;
        }

        /// <summary>
        /// 返回文件的名称
        /// c:\a\b\c.exe => c.exe 或者c
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="withExtension">true带后缀，否则不带</param>
        /// <returns></returns>
        public static string GetFileName(string filePath, bool withExtension = true)
        {
            var name = withExtension ? Path.GetFileName(filePath) : Path.GetFileNameWithoutExtension(filePath);
            return name;
        }

        /// <summary>
        /// 返回文件的名称
        /// c:\a\b\c.exe => exe
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="toLower">true，统一小写</param>
        /// <returns></returns>
        public static string GetFileExtension(string filePath, bool toLower = true)
        {
            var name = Path.GetExtension(filePath);
            if (toLower && !string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
            }
            return name;
        }

        public static string GetUploadFilePath(string areaName )
        {
            var webPath = MakeWebRootPath();
            var saveFileFolder = JoinPath(webPath, "Content\\Upload\\Modules\\"+areaName);
            return saveFileFolder;
        }

    }
}
