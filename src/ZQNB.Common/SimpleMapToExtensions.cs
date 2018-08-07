using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZQNB.Common
{
    public static class SimpleMapToExtensions
    {
        /// <summary>
        /// COPY所有简单类型的属性的值
        /// </summary>
        /// <typeparam name="TCopyTo"></typeparam>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static void TryCopyTo<TCopyTo>(this object copyFrom, TCopyTo copyTo, params string[] excludeProperties)
        {
            if (copyFrom == null)
            {
                return;
            }
            if (copyTo == null)
            {
                throw new ArgumentNullException("copyTo");
            }
            //better relace impl from reflection with expression tree, not find good impl yet. todo
            MyModelHelper.TryCopyProperties(copyTo, copyFrom, excludeProperties);
        }

        /// <summary>
        /// 转换成映射对象
        /// </summary>
        /// <typeparam name="TMapTo"></typeparam>
        /// <param name="mapFrom"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static TMapTo ToMapped<TMapTo>(this object mapFrom, params string[] excludeProperties) where TMapTo : new()
        {
            var orgDto = new TMapTo();
            //todo better relace impl from reflection with expression tree
            MyModelHelper.TryCopyProperties(orgDto, mapFrom, excludeProperties);
            return orgDto;
        }

        /// <summary>
        /// 转换成映射对象的枚举
        /// </summary>
        /// <typeparam name="TMapTo"></typeparam>
        /// <param name="froms"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static IEnumerable<TMapTo> ToMappedEnumerable<TMapTo>(this IEnumerable froms, params string[] excludeProperties) where TMapTo : new()
        {
            //foreach (var mapFrom in froms)
            //{
            //    yield return ToMapped<TMapTo>(mapFrom, excludeProperties);
            //}
            return from object mapFrom in froms select ToMapped<TMapTo>(mapFrom, excludeProperties);
        }

        /// <summary>
        /// 转换成映射对象的List
        /// </summary>
        /// <typeparam name="TMapTo"></typeparam>
        /// <param name="froms"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static IList<TMapTo> ToMappedList<TMapTo>(this IEnumerable froms, params string[] excludeProperties) where TMapTo : new()
        {
            var mappedEnumerable = ToMappedEnumerable<TMapTo>(froms, excludeProperties);
            return mappedEnumerable.ToList();
        }

        #region process NULL

        /// <summary>
        /// 转换成映射对象(Auto Null => Null)
        /// </summary>
        /// <typeparam name="TMapTo"></typeparam>
        /// <param name="mapFrom"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static TMapTo TryToMapped<TMapTo>(this object mapFrom, params string[] excludeProperties) where TMapTo : new()
        {
            if (mapFrom == null)
            {
                return default(TMapTo);
            }
            return mapFrom.ToMapped<TMapTo>(excludeProperties);
        }

        /// <summary>
        /// 转换成映射对象的枚举(Exclude Null)
        /// </summary>
        /// <typeparam name="TMapTo"></typeparam>
        /// <param name="froms"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static IEnumerable<TMapTo> TryToMappedEnumerable<TMapTo>(this IEnumerable froms, params string[] excludeProperties) where TMapTo : new()
        {
            foreach (var @from in froms)
            {
                var temp = TryToMapped<TMapTo>(@from);
                if (temp != null)
                {
                    yield return temp;
                }
            }
        }

        /// <summary>
        /// 转换成映射对象的List(Exclude Null)
        /// </summary>
        /// <typeparam name="TMapTo"></typeparam>
        /// <param name="froms"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static IList<TMapTo> TryToMappedList<TMapTo>(this IEnumerable froms, params string[] excludeProperties) where TMapTo : new()
        {
            var mappedEnumerable = TryToMappedEnumerable<TMapTo>(froms, excludeProperties);
            return mappedEnumerable.ToList();
        }

        #endregion

        #region PagerQueryResult

        ///// <summary>
        ///// 转换分页结果
        ///// </summary>
        ///// <typeparam name="TMapTo"></typeparam>
        ///// <typeparam name="TFrom"></typeparam>
        ///// <param name="pagerQueryResult"></param>
        ///// <returns></returns>
        //public static PagerQueryResult<TMapTo> ToMappedPagerQueryResult<TMapTo, TFrom>(this PagerQueryResult<TFrom> pagerQueryResult) where TMapTo : new()
        //{
        //    var mappedList = pagerQueryResult.Result.ToMappedList<TMapTo>();
        //    return PagerQueryResult<TMapTo>.Create(mappedList,
        //        pagerQueryResult.PageIndex,
        //        pagerQueryResult.PageSize,
        //        pagerQueryResult.TotalCount);
        //}

        #endregion
    }
}
