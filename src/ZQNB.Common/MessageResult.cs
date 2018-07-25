using System.Collections.Generic;

namespace ZQNB.Common
{
    /// <summary>
    /// 自定义返回结果
    /// </summary>
    public class MessageResult
    {
        public MessageResult()
        {
        }

        public MessageResult(bool success, string message, object data = null)
        {
            _success = success;
            _message = message;
            _data = data;
        }

        private bool _success = false;
        private string _message = string.Empty;
        private object _data = null;

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public virtual object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public static MessageResult CreateByCrudFlag(object flag, string message = "")
        {
            int flagNum = (int)flag;
            MessageResult result = new MessageResult();
            if (flagNum > 0)
            {
                result.Success = true;
                result.Message = "保存成功";
            }
            else
            {
                result.Success = false;
                result.Message = "保存失败";
            }

            if (!string.IsNullOrEmpty(message))
            {
                result.Message = message;
            }
            return result;
        }

        public static MessageResult ValidateResult(bool validateSuccess = false, string successMessage = "验证通过", string failMessage = "验证失败")
        {
            MessageResult vr = new MessageResult();
            vr.Message = validateSuccess ? successMessage : failMessage;
            vr.Success = validateSuccess;
            return vr;
        }

        public static MessageResult CreateMessageResult(bool success, string successMessage = "成功", string failMessage = "失败")
        {
            MessageResult mr = new MessageResult();
            mr.Message = success ? successMessage : failMessage;
            mr.Success = success;
            return mr;
        }

        public override string ToString()
        {
            return string.Format("Success:{0}, Message:{1}, Data:{2}", Success, Message, Data);
        }
    }
    /// <summary>
    /// 移动接口接收数据模型
    /// </summary>
    public class MobileMessageResult : MessageResult
    {
        public class TemplateData
        {
            public string Type { get; set; }
            public virtual IEnumerable<object> Items { get; set; }
        }
        public virtual new List<TemplateData> Data { get; set; }
    }
}
