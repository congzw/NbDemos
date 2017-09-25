using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AngularDemo.Helpers.Angulars
{
    public static class AngularHelperExtension
    {
        public static AngularHelper<TModel> Angular<TModel>(this HtmlHelper<TModel> helper)
        {
            return new AngularHelper<TModel>(helper);
        }
    }

    public class AngularHelper<TModel>
    {
        private readonly HtmlHelper<TModel> _helper;

        public AngularHelper(HtmlHelper<TModel> helper)
        {
            _helper = helper;
        }

        public AngularModelHelper<TModel> ModelFor(string expressionPrefix)
        {
            return new AngularModelHelper<TModel>(_helper, expressionPrefix);
        }

        //Constructs a lambda of the form x => x.PropName
        private object MakeLambda(PropertyInfo prop)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var property = Expression.Property(parameter, prop);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TModel), prop.PropertyType);

            //x => x.PropName
            return Expression.Lambda(funcType, property, parameter);
        }
    }
}
