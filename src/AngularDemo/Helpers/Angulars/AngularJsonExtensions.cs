using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AngularDemo.Helpers.Angulars
{
    /// <summary>
    /// 序列化Json
    /// </summary>
    public static class AngularJsonExtensions
    {
        /// <summary>
        /// 序列化Json(CamelCase)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="includeNull"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, bool includeNull = true)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] { new StringEnumConverter() },
                NullValueHandling = includeNull ? NullValueHandling.Include : NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 转换名称
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <param name="toCamelCaseName"></param>
        /// <returns></returns>
        public static string Name<TModel, TProp>(this Expression<Func<TModel, TProp>> property, bool toCamelCaseName = false)
        {
            //x => x.SomeProperty.SomeValue => "SomeProperty.SomeValue"
            var pascalCaseName = ExpressionHelper.GetExpressionText(property);
            if (!toCamelCaseName)
            {
                return pascalCaseName;
            }
            //"SomeProperty.SomeValue" => "someProperty.someValue"
            var camelCaseName = ConvertFullNameToCamelCase(pascalCaseName);
            return camelCaseName;
        }

        /// <summary>
        /// 转换名称（CamelCase）
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ToCamelCaseName<TModel, TProp>(this Expression<Func<TModel, TProp>> property)
        {
            return Name(property, true);
        }

        //Some.PropertyName => some.propertyName
        private static string ConvertFullNameToCamelCase(string pascalCaseName)
        {
            var parts = pascalCaseName.Split('.')
                .Select(ConvertToCamelCase);

            return string.Join(".", parts);
        }

        //Borrowed from JSON.NET. Turns a single name into camel case.
        private static string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (!char.IsUpper(s[0]))
                return s;
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }
            return new string(chars);
        }
    }
}
