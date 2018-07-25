using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace ZQNB.Common.NHExtensions
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPageCount { get; private set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {

            get
            {
                return (PageIndex < TotalPageCount);
            }
        }

        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (pageIndex < 0 || pageSize < 0 || pageSize > totalCount)
            {
                // Check: check if pageSize > totalCount.
                // Check: check if int parameters < 0.
                throw new ArgumentException("非法的参数");
            }


            AddRange(source);

            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }

    public static class QueryableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            int totalCount = query.Count();
            IQueryable<T> collection = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<T>(collection, pageIndex, pageSize, totalCount);
        }
        
        /// <summary>
        /// EagerLoad
        /// </summary>
        /// <typeparam name="TOriginating"></typeparam>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="query"></param>
        /// <param name="relatedObjectSelector"></param>
        /// <returns></returns>
        public static IQueryable<TOriginating> EagerLoad<TOriginating, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            return query.Fetch(relatedObjectSelector);
        }

        public static IQueryable<TOriginating> EagerLoadMany<TOriginating, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return query.FetchMany(relatedObjectSelector);
        }
    }
}