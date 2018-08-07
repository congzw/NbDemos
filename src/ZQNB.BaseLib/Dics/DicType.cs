using System.Collections.Generic;
using System.Linq;
using ZQNB.Common;
using ZQNB.Common.Data.Model;

namespace ZQNB.BaseLib.Dics
{
    /// <summary>
    /// 字典类型
    /// </summary>
    public class DicType : NbEntity<DicType>, IDicType
    {
        private bool _inUse;
        private bool _canEdit;

        /// <summary>
        /// 字典类型
        /// </summary>
        public DicType()
        {
            _inUse = true;
            _canEdit = false;
        }

        /// <summary>
        /// 作为唯一键使用，不可更改：Grade,Subject等
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        ///  显示名：学科，年级
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool InUse
        {
            get { return _inUse; }
            set { _inUse = value; }
        }

        /// <summary>
        /// 某些字典类型下的条目不需要提供修改功能
        /// </summary>
        public virtual bool CanEdit
        {
            get { return _canEdit; }
            set { _canEdit = value; }
        }

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="canEdit"></param>
        /// <param name="name"></param>
        /// <param name="inUse"></param>
        /// <returns></returns>
        public static DicType Create(string code, bool canEdit, string name, bool inUse = true)
        {
            DicType dicType = new DicType()
            {
                Code = code,
                CanEdit = canEdit,
                Name = name,
                InUse = inUse
            };
            return dicType;
        }
    }

    #region Abstract

    /// <summary>
    /// 字典类型的接口
    /// </summary>
    public interface IDicType
    {
        /// <summary>
        /// 作为唯一键使用，不可更改：Grade,Subject等
        /// </summary>
        string Code { get; set; }

        /// <summary>
        ///  显示名：学科，年级
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        bool InUse { get; set; }

        /// <summary>
        /// 某些字典类型下的条目不需要提供修改功能
        /// </summary>
        bool CanEdit { get; set; }
    }

    /// <summary>
    /// DicTypeExtensions
    /// </summary>
    public static class DicTypeExtensions
    {
        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="dicType"></param>
        /// <returns></returns>
        public static MessageResult Validate(this IDicType dicType)
        {
            var messageResult = new MessageResult();
            if (dicType == null)
            {
                messageResult.Success = false;
                messageResult.Message = "不能为空";
                return messageResult;
            }

            if (string.IsNullOrWhiteSpace(dicType.Code))
            {
                messageResult.Success = false;
                messageResult.Message = "Code不能为空";
                return messageResult;
            }

            if (string.IsNullOrWhiteSpace(dicType.Name))
            {
                messageResult.Success = false;
                messageResult.Message = "Name不能为空";
                return messageResult;
            }

            messageResult.Success = true;
            messageResult.Message = "OK";
            return messageResult;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="dicTypes"></param>
        /// <returns></returns>
        public static IEnumerable<MessageResult> ValidateBatch(this IEnumerable<IDicType> dicTypes)
        {
            return dicTypes.Select(Validate);
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="dicTypes"></param>
        /// <returns></returns>
        public static MessageResult ValidateBatch2(this IEnumerable<IDicType> dicTypes)
        {
            var messageResults = ValidateBatch(dicTypes);
            var singleResult = messageResults.ToSingleResult();
            return singleResult;
        }
    }

    #endregion
}
