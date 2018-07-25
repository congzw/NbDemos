using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMapperDemo.Demos.SimpleProjects
{
    /// <summary>
    /// Projection Expression Extensions 
    /// </summary>
    public static class ProjectionExpressionExtensions
    {
        /// <summary>
        /// Projection Expression Helper 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IProjectionExpression SimpleProject<TSource>(this IQueryable<TSource> source)
        {
            return new ProjectionExpression<TSource>(source);
        }

        ///// <summary>
        ///// Simple Select Project For Same Property Mapping
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TDestination"></typeparam>
        ///// <param name="sourceQuery"></param>
        ///// <returns></returns>
        //public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> sourceQuery)
        //{
        //    return sourceQuery.Project().To<TDestination>();
        //}

        ///// <summary>
        ///// Simple Select Project For Same Property Mapping
        ///// </summary>
        ///// <typeparam name="TDestination"></typeparam>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="sourceQuery"></param>
        ///// <returns></returns>
        //public static IQueryable<TDestination> ProjectTo<TDestination, TSource>(this IQueryable<TSource> sourceQuery)
        //{
        //    return sourceQuery.Project().To<TDestination>();
        //}
    }

    /// <summary>
    /// Projection Expression
    /// </summary>
    public interface IProjectionExpression
    {
        /// <summary>
        /// Project To IQueryable Of T
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        IQueryable<TResult> To<TResult>();
    }

    /// <summary>
    /// Projection Expression
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ProjectionExpression<TEntity> : IProjectionExpression
    {
        private readonly IQueryable<TEntity> _source;

        /// <summary>
        /// Projection Expression
        /// </summary>
        /// <param name="source"></param>
        public ProjectionExpression(IQueryable<TEntity> source)
        {
            _source = source;
        }

        public IQueryable<TResult> To<TResult>()
        {
            Expression<Func<TEntity, TResult>> expr = BuildExpression<TResult>();
            return _source.Select(expr);
        }

        private static Expression<Func<TEntity, TDto>> BuildExpression<TDto>()
        {
            var entityType = typeof (TEntity);
            var dtoType = typeof(TDto);
            var entityPropertyInfos = entityType.GetProperties();
            var dtoPropertyInfos = dtoType.GetProperties();

            var name = "src";
            var parameterExpression = Expression.Parameter(entityType, name);
            //var descParameterExpression = Expression.Parameter(dtoType, "desc");

            IList<MemberAssignment> memberAssignments = new List<MemberAssignment>();
            foreach (var dtoPropertyInfo in dtoPropertyInfos)
            {
                var sourcePropertyInfo = entityPropertyInfos.FirstOrDefault(pi => pi.Name == dtoPropertyInfo.Name);
                var initPropertyInfo = dtoType.GetProperty(dtoPropertyInfo.Name);
                if (initPropertyInfo == null)
                {
                    continue;
                }
                
                if (sourcePropertyInfo == null)
                {
                    //MemberBinding notExistMemberBinding =
                    //Expression.Bind(destPropertyInfo, Expression.Default(destPropertyInfo.PropertyType));

                    memberAssignments.Add(Expression.Bind(dtoPropertyInfo, Expression.Default(dtoPropertyInfo.PropertyType)));
                    //memberAssignments.Add(Expression.Bind(destPropertyInfo, Expression.Property(descParameterExpression, initPropertyInfo)));
                    //memberAssignments.Add(Expression.Bind(destPropertyInfo, Expression.Property(parameterExpression, destPropertyInfo)));
                    continue;
                }

                //do not process complex property
                var shouldAutoProject = ShouldAutoProject(sourcePropertyInfo.PropertyType);
                if (!shouldAutoProject)
                {
                    memberAssignments.Add(Expression.Bind(dtoPropertyInfo, Expression.Property(parameterExpression, initPropertyInfo)));
                    continue;
                }

                //fix dynamic problems: System.Reflection.TargetParameterCountException
                var declaringType = sourcePropertyInfo.DeclaringType;
                if (declaringType != null && declaringType != entityType)
                {
                    //do not process base class dynamic property
                    if (NotProcessPerpertyBaseTypes.Contains(declaringType))
                    {
                        memberAssignments.Add(Expression.Bind(dtoPropertyInfo, Expression.Property(parameterExpression, initPropertyInfo)));
                        continue;
                    }
                }
                memberAssignments.Add(Expression.Bind(dtoPropertyInfo, Expression.Property(parameterExpression, sourcePropertyInfo)));
            }

            var memberInitExpression = Expression.MemberInit(Expression.New(dtoType), memberAssignments);

            Console.WriteLine(memberInitExpression.ToString());


            return Expression.Lambda<Func<TEntity, TDto>>(memberInitExpression);
            
            //return Expression.Lambda<Func<TSource, TResult>>(
            //    Expression.MemberInit(
            //        Expression.New(typeof(TResult)),
            //        destPropertyInfos.Select(dest =>
            //        {
            //            return Expression.Bind(dest, Expression.Property(parameterExpression, sourcePropertyInfos.First(pi => pi.Name == dest.Name)));
            //        }).ToArray()), parameterExpression);
        }

        /// <summary>
        /// 是否属于自动映射的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool ShouldAutoProject(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return ShouldAutoProject(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
                || typeInfo.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                //|| type == typeof(Guid)
                //|| type == typeof(DateTime)
                || type.IsSubclassOf(typeof(ValueType)); //Guid, Datetime, etc...
        }


        private static IList<Type> _notProcessPerpertyBaseTypes = new List<Type>()
        {
            //todo
            //typeof(DynamicObject), typeof(Object), typeof(BaseViewModel),  typeof(BaseViewModel<>), typeof(Expando) 
            typeof(DynamicObject), typeof(Object)
        };

        /// <summary>
        /// 在这些类型中声明的属性不处理
        /// </summary>
        public static IList<Type> NotProcessPerpertyBaseTypes
        {
            get
            {
                return _notProcessPerpertyBaseTypes;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _notProcessPerpertyBaseTypes = value;
            }
        }
    }
}
