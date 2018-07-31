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
        public dynamic Upload([FromUri]UploadFileDto model)
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

            if (string.IsNullOrWhiteSpace(model.VirtualPath))
            {
                throw new InvalidOperationException("无效的虚拟目录");
            }

            //todo check allowed location from config 防止恶意下载
            if (!model.VirtualPath.ToLower().StartsWith("~/content/upload"))
            {
                throw new InvalidOperationException("不支持的下载路径");
            }

            //var virtualPathRoot = request.GetRequestContext().VirtualPathRoot; // => /
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
        
        private async Task<dynamic> UploadAsync(HttpRequestMessage request, [FromUri]UploadFileDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            try
            {
                var mr = new MessageResult();
                var fileUrls = new List<string>();
                var httpRequest = HttpContext.Current.Request;
                foreach (string httpFile in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[httpFile];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        var fileName = postedFile.FileName;
                        string extension = GetFileExtension(fileName);
                        bool isAllowed = CheckAllowedExtensions(extension);
                        if (!isAllowed)
                        {
                            mr.Message = "不支持的文件格式：" + extension;
                            mr.Success = false;
                            return await Task.FromResult(mr);
                        }

                        int maxM = 10;
                        int MaxContentLength = 1024*1024*maxM; //Size = 10 MB   //todo
                        if (postedFile.ContentLength > MaxContentLength)
                        {
                            mr.Message = string.Format("超出上传最大限制：{0}M", maxM);
                            mr.Success = false;
                            return await Task.FromResult(mr);
                        }

                        var server = HttpContext.Current.Server;
                        var saveFolder = server.MapPath(model.VirtualFolder.TrimEnd('/'));

                        var theFileName = fileName;
                        if (!string.IsNullOrWhiteSpace(model.FileName))
                        {
                            theFileName = model.FileName;
                        }
                        var savePath = Path.Combine(saveFolder, theFileName);
                        //Deletion exists file  
                        if (File.Exists(savePath))
                        {
                            File.Delete(savePath);
                        }
                        MakeSureFolderExist(saveFolder);

                        postedFile.SaveAs(savePath);
                        var fileUrl = string.Format("{0}/{1}", model.VirtualFolder.TrimEnd('/'), theFileName);
                        fileUrls.Add(fileUrl);
                    }
                    else
                    {
                        fileUrls.Add(null);
                    }
                }
                mr.Message = "上传完成";
                mr.Success = true;
                mr.Data = fileUrls;
                return await Task.FromResult(mr);
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
}
