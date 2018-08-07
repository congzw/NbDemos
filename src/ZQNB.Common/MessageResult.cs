using System.Collections.Generic;
using System.Linq;

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


    /// <summary>
    /// 批量操作的结果对象
    /// </summary>
    public class BatchMessageResult : MessageResult
    {
        public BatchMessageResult()
        {
            Data = new List<MessageResult>();
        }
        public new virtual IList<MessageResult> Data { get; set; }
    }

    public static class MessageResultExtensions
    {
        /// <summary>
        /// 全都成功
        /// </summary>
        /// <param name="messageResults"></param>
        /// <returns></returns>
        public static bool AllSuccess(this IEnumerable<MessageResult> messageResults)
        {
            return messageResults.All(x => x.Success);
        }

        /// <summary>
        /// 全都失败
        /// </summary>
        /// <param name="messageResults"></param>
        /// <returns></returns>
        public static bool AllFailed(this IEnumerable<MessageResult> messageResults)
        {
            return messageResults.All(x => !x.Success);
        }

        /// <summary>
        /// 合并成一个结果，结果集合存于Data中
        /// </summary>
        /// <param name="messageResults"></param>
        /// <returns></returns>
        public static MessageResult ToSingleResult(this IEnumerable<MessageResult> messageResults)
        {
            var results = messageResults.ToList();
            var validateResult = MessageResult.ValidateResult();
            var countSuccess = results.Count(x => x.Success);
            var countFail = results.Count(x => !x.Success);
            var countAll = results.Count();

            validateResult.Success = countAll == countSuccess;
            validateResult.Data = results;
            validateResult.Message = string.Format("成功：{0}, 失败：{1}， 共计:{2}", countSuccess, countFail, countAll);
            return validateResult;
        }

        /// <summary>
        /// 合并成一个结果，结果集合存于Data中
        /// </summary>
        /// <param name="messageResults"></param>
        /// <returns></returns>
        public static BatchMessageResult ToBatchMessageResult(this IEnumerable<MessageResult> messageResults)
        {
            var results = messageResults.ToList();
            var batchMessageResult = new BatchMessageResult();
            var countSuccess = results.Count(x => x.Success);
            var countFail = results.Count(x => !x.Success);
            var countAll = results.Count;

            batchMessageResult.Success = countAll == countSuccess;
            batchMessageResult.Data = results;
            batchMessageResult.Message = string.Format("成功：{0}, 失败：{1}， 共计:{2}", countSuccess, countFail, countAll);
            return batchMessageResult;
        }
        /// <summary>
        /// 合并成一个结果，结果集合存于Data中
        /// </summary>
        /// <param name="messageResult"></param>
        /// <returns></returns>
        public static BatchMessageResult ToBatchMessageResult(this MessageResult messageResult)
        {
            var batchMessageResult = new BatchMessageResult();
            batchMessageResult.Success = messageResult.Success;
            batchMessageResult.Message = messageResult.Message;

            var data = messageResult.Data as IEnumerable<MessageResult>;
            if (data != null)
            {
                batchMessageResult.Data = data.ToList();
            }
            return batchMessageResult;
        }

        /// <summary>
        /// 生成验证结果列表
        /// </summary>
        /// <param name="messageResult"></param>
        /// <param name="countSuccess"></param>
        /// <param name="countFail"></param>
        /// <returns></returns>
        public static IList<MessageResult> ToResults(this MessageResult messageResult, int countSuccess, int countFail)
        {
            var results = new List<MessageResult>();
            for (int i = 0; i < countSuccess; i++)
            {
                results.Add(MessageResult.ValidateResult(true));
            }
            for (int i = 0; i < countFail; i++)
            {
                results.Add(MessageResult.ValidateResult(false));
            }
            return results;
        }
    }
}
