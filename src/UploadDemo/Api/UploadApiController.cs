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
using ZQNB.Common;

namespace UploadDemo.Api
{
    /// <summary>
    /// 上传，下载相关
    /// </summary>
    public class FileApiController : ApiController
    {
        private readonly IFileApiService _fileApiService;

        public FileApiController(IFileApiService fileApiService)
        {
            _fileApiService = fileApiService;
        }
        
        public async Task<dynamic> UploadAsync(UploadFileDto model)
        {
            return await _fileApiService.UploadAsync(Request, model);
        }

        public async Task<HttpResponseMessage> DownloadAsync(HttpRequestMessage request, DownloadFileDto model)
        {
            return await _fileApiService.DownloadAsync(Request, model);
        }
    }

    public class UploadFileDto
    {
        public string VirtualFolder { get; set; }
        public string CustomFileName { get; set; }
    }

    public class DownloadFileDto
    {
        public string VirtualPath { get; set; }
        public string CustomFileName { get; set; }
    }

    /// <summary>
    /// 基于WebApi的上传下载
    /// </summary>
    public interface IFileApiService
    {
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<dynamic> UploadAsync(HttpRequestMessage request, UploadFileDto model);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DownloadAsync(HttpRequestMessage request, DownloadFileDto model);
    }

    public class FileApiService
    {
        public async Task<dynamic> UploadAsync(HttpRequestMessage request, UploadFileDto model)
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
                        var server = System.Web.HttpContext.Current.Server;
                        //var saveFolder = server.MapPath("~/App_Data/uploads"); 
                        var saveFolder = server.MapPath(model.VirtualFolder);

                        if (!string.IsNullOrWhiteSpace(model.CustomFileName))
                        {
                            fileName = model.CustomFileName;
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

        public async Task<HttpResponseMessage> DownloadAsync(HttpRequestMessage request, DownloadFileDto model)
        {
            //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot;
            var server = System.Web.HttpContext.Current.Server;
            var filePath = server.MapPath(model.VirtualPath);
            if (!File.Exists(filePath))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                if (!string.IsNullOrWhiteSpace(model.CustomFileName))
                {
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = model.CustomFileName
                    };
                }
                return await Task.FromResult(result);
            }
        }

        #region helpers

        //should use this?
        //https://stackoverflow.com/questions/16670329/how-to-access-the-current-httprequestmessage-object-globally
        private static HttpRequestMessage GetCurrentHttpRequestMessage()
        {
            var httpRequestMessage =
                HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
            return httpRequestMessage;

        }

        private IList<string> GetAllowedExtensions()
        {
            //todo init from config
            var allowedExtensions = new List<string>() { "*" };
            return allowedExtensions;
        }

        //检测是否是允许的后缀
        private bool CheckAllowedExtensions(string extension)
        {
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
                return "jpg";
            }

            var extension = filename.Split('.').Last();
            return extension;
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

        private void MakeSureFolderExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        #endregion
    }
}
