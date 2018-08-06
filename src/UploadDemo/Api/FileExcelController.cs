using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace UploadDemo.Api
{
    /// <summary>
    /// 支持上传数据、解析验证、数据预览、模板下载的数据模型（Excel等）
    /// </summary>
    public class FileImportController : FileApiController
    {
        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        public HttpResponseMessage Download(string fileName)
        {
            var fileServer = FileServer.Current();
            var templateFolderVirtualPath = "~/Content/Upload/Templates/Excel";
            if (!fileServer.ExistVirtualFolder(templateFolderVirtualPath))
            {
                fileServer.CreateVirtualFolder(templateFolderVirtualPath);
            }

            var fileVirtualPath = string.Format("{0}/{1}", templateFolderVirtualPath, fileName);
            var existVirtualFile = fileServer.ExistVirtualFile(fileVirtualPath);
            if (!existVirtualFile)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "文件不存在： " + fileName);
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var mapToPhysicalPath = fileServer.MapToPhysicalPath(fileVirtualPath);
            var readFile = fileServer.ReadFile(mapToPhysicalPath);
            result.Content = new StreamContent(readFile);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return result;
        }
    }
    
    /// <summary>
    /// 支持上传数据、解析验证、数据预览、模板下载的数据模型（Excel等）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFileImportDto<T> where T : IFileImportDto<T>
    {
        //ImportOrg
        //ImportUser
        //ImportCategory
    }
    
    //public interface IExcelImportFile
    //{
    //    /// <summary>
    //    /// 导入的数据类型
    //    /// </summary>
    //    string ImportDataType { get; set; }

    //    //string GetUploadDataFileName();
    //    //string GetDownloadTemplateFileName();
    //}
    
    /// <summary>
    /// 批量导入的数据类型
    /// </summary>
    public interface IFileImportType
    {
        /// <summary>
        /// 批量导入的类型
        /// </summary>
        string DataType { get; set; }
    }

    /// <summary>
    /// 批量导入的下载参数
    /// </summary>
    public class FileImportDownloadDto : IFileImportType
    {
        public string DataType { get; set; }
    }

}
