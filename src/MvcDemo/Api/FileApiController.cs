using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MvcDemo.Api.AppServices;
using ZQNB.Common;

namespace MvcDemo.Api
{
    /// <summary>
    /// 文件上传下载Api（通用）
    /// </summary>
    public class FileApiController : ApiController
    {
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Download([FromUri]DownloadFileDto model)
        {
            //=> ~/api/FileApi/Download?VirtualPath=~/Content/Uploads/1.xlsx&FileName=abc.xlsx
            return DownloadAsync(model).Result;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic Upload(UploadFileDto model)
        {
            return UploadAsync(model).Result;
        }
        
        #region async

        /// <summary>
        /// 下载（异步）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadAsync([FromUri]DownloadFileDto model)
        {
            return await DownloadAsync(Request, model);
        }

        /// <summary>
        /// 上传（异步）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> UploadAsync([FromUri]UploadFileDto model)
        {
            return await UploadAsync(Request, model);
        }

        #endregion 

        #region helpers
        private async Task<HttpResponseMessage> DownloadAsync(HttpRequestMessage request, DownloadFileDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot;
            var server = HttpContext.Current.Server;
            var filePath = server.MapPath(model.VirtualPath);
            if (!File.Exists(filePath))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var result = request.CreateResponse(HttpStatusCode.OK);
            //var result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var stream = File.OpenRead(filePath);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName =
                string.IsNullOrWhiteSpace(model.FileName)
                ? Path.GetFileName(filePath)
                : model.FileName;
            return await Task.FromResult(result);
        }
        
        private async Task<dynamic> UploadAsync(HttpRequestMessage request, UploadFileDto model)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            // Check if the request contains multipart/form-data.
            bool isMime = request.Content.IsMimeMultipartContent();
            if (!isMime)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                var mr = new MessageResult();
                var fileUrls = new List<string>();
                var provider = new MultipartMemoryStreamProvider();
                await request.Content.ReadAsMultipartAsync(provider);
                
                foreach (HttpContent content in provider.Contents)
                {
                    //now read individual part into STREAM
                    var stream = await content.ReadAsStreamAsync();
                    if (stream.Length != 0)
                    {
                        string fileName = GetFileName(content.Headers);
                        if (fileName == null)
                        {
                            continue;
                        }
                        string extension = GetFileExtension(fileName);
                        bool isAllowed = CheckAllowedExtensions(extension);
                        if (!isAllowed)
                        {
                            throw new NotSupportedException("不支持的文件格式：" + extension);
                        }

                        //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot;
                        var server = HttpContext.Current.Server;
                        //var saveFolder = server.MapPath("~/App_Data/uploads"); 
                        var saveFolder = server.MapPath(model.VirtualFolder);

                        if (!string.IsNullOrWhiteSpace(model.FileName))
                        {
                            fileName = model.FileName;
                        }
                        var savePath = Path.Combine(saveFolder, fileName);
                        //Deletion exists file  
                        if (File.Exists(savePath))
                        {
                            File.Delete(savePath);
                        }

                        MakeSureFolderExist(saveFolder);
                        using (Stream saveFileStream = File.OpenWrite(savePath))
                        {
                            await stream.CopyToAsync(saveFileStream);
                            stream.Close();
                        }
                    }
                    else
                    {
                        fileUrls.Add(null);
                    }
                }
                mr.Message = "上传完成";
                mr.Success = true;
                mr.Data = fileUrls;
                return mr;
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        //获取允许的上传格式限制（todo）
        private IList<string> GetAllowedExtensions()
        {
            //todo init from config
            var allowedExtensions = new List<string>() { "*" };
            return allowedExtensions;
        }
        
        //获取文件名
        private string GetFileName(HttpContentHeaders headers)
        {
            var filename = headers.ContentDisposition.FileName;
            if (filename == null)
            {
                return null;
            }
            filename = filename.Replace("\"", string.Empty);
            return filename;
        }

        //确保文件夹存在
        private void MakeSureFolderExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        //检测是否是允许的后缀
        private bool CheckAllowedExtensions(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return false;
            }

            var allowedExtensions = GetAllowedExtensions();
            if (allowedExtensions.NbContains("*"))
            {
                return true;
            }

            var result = allowedExtensions.NbContains(extension);
            return result;
        }

        //获取文件后缀
        private string GetFileExtension(string filename)
        {
            if (filename.IndexOf('.') < 0)
            {
                //    throw new Exception("No extension");
                //return "jpg";
                return string.Empty;
            }

            var extension = filename.Split('.').Last();
            return extension;
        }

        ////should use this?
        ////https://stackoverflow.com/questions/16670329/how-to-access-the-current-httprequestmessage-object-globally
        //private static HttpRequestMessage GetCurrentHttpRequestMessage()
        //{
        //    var httpRequestMessage =
        //        HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
        //    return httpRequestMessage;

        //}

        #endregion
    }

    #region dtos

    /// <summary>
    /// 下载参数
    /// </summary>
    public class DownloadFileDto
    {
        /// <summary>
        /// 文件的虚拟路径（必须指定）
        /// </summary>
        public string VirtualPath { get; set; }
        /// <summary>
        /// 下载文件名（不指定使用文件的默认名称）
        /// </summary>
        public string FileName { get; set; }
    }
    
    /// <summary>
    /// 上传参数
    /// </summary>
    public class UploadFileDto
    {
        /// <summary>
        /// 文件夹的虚拟路径（不指定使用默认路径：~/Content/Upload/Templates/Excel）
        /// </summary>
        public string VirtualFolder { get; set; }
        /// <summary>
        /// 下载文件名（不指定使用文件的默认名称）
        /// </summary>
        public string FileName { get; set; }
    }

    #endregion


    #region temp

    ///// <summary>
    ///// 上传，下载相关
    ///// </summary>
    //public class FileApiController : ApiController
    //{
    //    private readonly IFileApiService _fileApiService = MyFactory.CreateFileApiService();

    //    //https://stackoverflow.com/questions/24625303/why-do-we-have-to-specify-frombody-and-fromuri

    //    //The default behavior is:
    //    //If the parameter is a primitive type (int, bool, double, ...), Web API tries to get the value from the URI of the HTTP request.
    //    //For complex types (your own object, for example: Person), Web API tries to read the value from the body of the HTTP request.

    //    //So, if you have:

    //    //a primitive type in the URI, or
    //    //a complex type in the body
    //    //...then you don't have to add any attributes (neither [FromBody] nor [FromUri]).

    //    //But, if you have a primitive type in the body, then you have to add [FromBody] in front of your primitive type parameter in your WebAPI controller method.
    //    //(Because, by default, WebAPI is looking for primitive types in the URI of the HTTP request.)
    //    //Or, if you have a complex type in your URI, then you must add [FromUri].
    //    //(Because, by default, WebAPI is looking for complex types in the body of the HTTP request by default.)

    //    public HttpResponseMessage GetTest([FromUri]DownloadFileDto model)
    //    {
    //        //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot;
    //        var server = HttpContext.Current.Server;
    //        var filePath = server.MapPath(model.VirtualPath);
    //        if (!File.Exists(filePath))
    //        {
    //            throw new HttpResponseException(HttpStatusCode.NotFound);
    //        }

    //        var result = new HttpResponseMessage(HttpStatusCode.OK);

    //        //using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    //        //{
    //        //    result.Content = new StreamContent(stream);
    //        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    //        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
    //        //    result.Content.Headers.ContentDisposition.FileName =
    //        //        string.IsNullOrWhiteSpace(model.CustomFileName)
    //        //        ? Path.GetFileName(filePath)
    //        //        : model.CustomFileName;
    //        //}

    //        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    //        result.Content = new StreamContent(stream);
    //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    //        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
    //        result.Content.Headers.ContentDisposition.FileName =
    //            string.IsNullOrWhiteSpace(model.FileName)
    //            ? Path.GetFileName(filePath)
    //            : model.FileName;

    //        return result;


    //        //var server = HttpContext.Current.Server;
    //        //var path = server.MapPath("~/Content/uploads/1.xlsx");
    //        //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
    //        //var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
    //        //result.Content = new StreamContent(stream);
    //        //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    //        //return result;
    //    }

    //    [HttpGet]
    //    public HttpResponseMessage Download([FromUri]DownloadFileDto model)
    //    {
    //        return DownloadAsync(model).Result;
    //    }

    //    [HttpPost]
    //    public dynamic Upload(UploadFileDto model)
    //    {
    //        return UploadAsync(model).Result;
    //    }

    //    #region async

    //    [HttpPost]
    //    public async Task<dynamic> UploadAsync(UploadFileDto model)
    //    {
    //        return await _fileApiService.UploadAsync(Request, model);
    //    }

    //    [HttpGet]
    //    public async Task<HttpResponseMessage> DownloadAsync([FromUri]DownloadFileDto model)
    //    {
    //        return await _fileApiService.DownloadAsync(Request, model);
    //    }


    //    #endregion
    //}

    ///// <summary>
    ///// 上传参数
    ///// </summary>
    //public class UploadFileDto
    //{
    //    /// <summary>
    //    /// 文件夹的虚拟路径（不指定使用默认路径：~/Content/Upload/Templates/Excel）
    //    /// </summary>
    //    public string VirtualFolder { get; set; }
    //    /// <summary>
    //    /// 下载文件名（不指定使用文件的默认名称）
    //    /// </summary>
    //    public string FileName { get; set; }
    //}

    ///// <summary>
    ///// 下载参数
    ///// </summary>
    //public class DownloadFileDto
    //{
    //    /// <summary>
    //    /// 文件的虚拟路径（不指定则尝试使用默认路径和自定义下载名称查找）
    //    /// </summary>
    //    public string VirtualPath { get; set; }
    //    /// <summary>
    //    /// 下载文件名（不指定使用文件的默认名称）
    //    /// </summary>
    //    public string FileName { get; set; }
    //}

    ///// <summary>
    ///// 基于WebApi的上传下载
    ///// </summary>
    //public interface IFileApiService
    //{
    //    /// <summary>
    //    /// 上传
    //    /// </summary>
    //    /// <param name="request"></param>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    Task<dynamic> UploadAsync(HttpRequestMessage request, UploadFileDto model);

    //    /// <summary>
    //    /// 下载
    //    /// </summary>
    //    /// <param name="request"></param>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    Task<HttpResponseMessage> DownloadAsync(HttpRequestMessage request, DownloadFileDto model);
    //}

    //public class FileApiService : IFileApiService
    //{
    //    public async Task<dynamic> UploadAsync(HttpRequestMessage request, UploadFileDto model)
    //    {
    //        if (request == null)
    //        {
    //            throw new ArgumentNullException("request");
    //        }
    //        if (model == null)
    //        {
    //            throw new ArgumentNullException("model");
    //        }

    //        // Check if the request contains multipart/form-data.
    //        bool isMime = request.Content.IsMimeMultipartContent();
    //        if (!isMime)
    //        {
    //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
    //        }

    //        try
    //        {
    //            var mr = new MessageResult();
    //            var fileUrls = new List<string>();
    //            var provider = new MultipartMemoryStreamProvider();
    //            await request.Content.ReadAsMultipartAsync(provider);


    //            foreach (HttpContent content in provider.Contents)
    //            {
    //                //now read individual part into STREAM
    //                var stream = await content.ReadAsStreamAsync();
    //                if (stream.Length != 0)
    //                {
    //                    string fileName = GetFileName(content.Headers);
    //                    if (fileName == null)
    //                    {
    //                        continue;
    //                    }
    //                    string extension = GetFileExtension(fileName);
    //                    bool isAllowed = CheckAllowedExtensions(extension);
    //                    if (!isAllowed)
    //                    {
    //                        throw new NotSupportedException("不支持的文件格式：" + extension);
    //                    }

    //                    //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot;
    //                    var server = HttpContext.Current.Server;
    //                    //var saveFolder = server.MapPath("~/App_Data/uploads"); 
    //                    var saveFolder = server.MapPath(model.VirtualFolder);

    //                    if (!string.IsNullOrWhiteSpace(model.FileName))
    //                    {
    //                        fileName = model.FileName;
    //                    }
    //                    var savePath = Path.Combine(saveFolder, fileName);
    //                    //Deletion exists file  
    //                    if (File.Exists(savePath))
    //                    {
    //                        File.Delete(savePath);
    //                    }

    //                    MakeSureFolderExist(saveFolder);
    //                    using (Stream saveFileStream = File.OpenWrite(savePath))
    //                    {
    //                        await stream.CopyToAsync(saveFileStream);
    //                        stream.Close();
    //                    }
    //                }
    //                else
    //                {
    //                    fileUrls.Add(null);
    //                }
    //            }
    //            mr.Message = "上传完成";
    //            mr.Success = true;
    //            mr.Data = fileUrls;
    //            return mr;
    //        }
    //        catch (Exception ex)
    //        {
    //            return request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
    //        }
    //    }

    //    public async Task<HttpResponseMessage> DownloadAsync(HttpRequestMessage request, DownloadFileDto model)
    //    {
    //        //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot;
    //        var server = HttpContext.Current.Server;
    //        var filePath = server.MapPath(model.VirtualPath);
    //        if (!File.Exists(filePath))
    //        {
    //            throw new HttpResponseException(HttpStatusCode.NotFound);
    //        }

    //        var result = new HttpResponseMessage(HttpStatusCode.OK);

    //        //var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    //        var stream = File.OpenRead(filePath);
    //        result.Content = new StreamContent(stream);
    //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    //        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
    //        result.Content.Headers.ContentDisposition.FileName =
    //            string.IsNullOrWhiteSpace(model.FileName)
    //            ? Path.GetFileName(filePath)
    //            : model.FileName;
    //        return await Task.FromResult(result);
    //    }

    //    #region helpers

    //    //should use this?
    //    //https://stackoverflow.com/questions/16670329/how-to-access-the-current-httprequestmessage-object-globally
    //    private static HttpRequestMessage GetCurrentHttpRequestMessage()
    //    {
    //        var httpRequestMessage =
    //            HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
    //        return httpRequestMessage;

    //    }

    //    private IList<string> GetAllowedExtensions()
    //    {
    //        //todo init from config
    //        var allowedExtensions = new List<string>() { "*" };
    //        return allowedExtensions;
    //    }

    //    //检测是否是允许的后缀
    //    private bool CheckAllowedExtensions(string extension)
    //    {
    //        var allowedExtensions = GetAllowedExtensions();
    //        if (allowedExtensions.NbContains("*"))
    //        {
    //            return true;
    //        }

    //        var result = allowedExtensions.NbContains(extension);
    //        return result;
    //    }

    //    //获取文件后缀
    //    private string GetFileExtension(string filename)
    //    {
    //        if (filename.IndexOf('.') < 0)
    //        {
    //            //    throw new Exception("No extension");
    //            return "jpg";
    //        }

    //        var extension = filename.Split('.').Last();
    //        return extension;
    //    }

    //    //获取文件名
    //    private string GetFileName(HttpContentHeaders headers)
    //    {
    //        var filename = headers.ContentDisposition.FileName;
    //        if (filename == null)
    //        {
    //            return null;
    //        }
    //        filename = filename.Replace("\"", string.Empty);
    //        return filename;
    //    }

    //    private void MakeSureFolderExist(string folderPath)
    //    {
    //        if (!Directory.Exists(folderPath))
    //        {
    //            Directory.CreateDirectory(folderPath);
    //        }
    //    }

    //    #endregion
    //}

    #endregion
}
