using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularDemo.Helpers.Angulars
{


    //public class AngularHtmlTag : HtmlTag
    //{
    //    public AngularHtmlTag(string tag) : base(tag)
    //    {
    //    }

    //    public AngularHtmlTag(string tag, Action<HtmlTag> configure) : base(tag, configure)
    //    {
    //    }

    //    public AngularHtmlTag(string tag, HtmlTag parent) : base(tag, parent)
    //    {
    //    }
    //}
    //public static class AngularHtmlTagExtensions
    //{
    //    public static AngularHtmlTag WithFormGroupFor<TModel, TProp>(Expression<Func<TModel, TProp>> property)
    //    {
    //        var metadata = ModelMetadata.FromLambdaExpression(property, new ViewDataDictionary<TModel>());
    //        var name = ExpressionHelper.GetExpressionText(property);
    //        var expression = ExpressionForInternal(property);

    //        //<div class="form-group has-feedback" form-group-validation="Name">
    //        var formGroup = new AngularHtmlTag("div")
    //            .AddClasses("form-group", "has-feedback")
    //            .Attr("form-group-validation", name);

    //        var labelText = metadata.DisplayName ?? name;

    //        //<label class="control-label" for="Name">Name</label>
    //        var label = new HtmlTag("label")
    //            .AddClass("control-label")
    //            .Attr("for", name)
    //            .Text(labelText);

    //        var tagName = metadata.DataTypeName == "MultilineText"
    //            ? "textarea"
    //            : "input";

    //        var placeholder = metadata.Watermark ?? (labelText + "...");
    //        //<input ng-model="expression" class="form-control" name="Name" type="text" >
    //        var input = new AngularHtmlTag(tagName)
    //            .AddClass("form-control")
    //            .Attr("ng-model", expression)
    //            .Attr("name", name)
    //            .Attr("type", "text")
    //            .Attr("placeholder", placeholder);

    //        ApplyValidationToInput(input, metadata);
    //        return input;
    //    }

    //    /// <summary>
    //    /// 自定义属性
    //    /// </summary>
    //    /// <param name="angularHtmlTag"></param>
    //    /// <param name="attribute"></param>
    //    /// <param name="value"></param>
    //    public static void WithAttr(this AngularHtmlTag angularHtmlTag, string attribute, object value)
    //    {
    //        angularHtmlTag.Attr(attribute, value);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="angularHtmlTag"></param>
    //    public static void Required(this AngularHtmlTag angularHtmlTag)
    //    {
    //        WithAttr(angularHtmlTag, "required", "");
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="angularHtmlTag"></param>
    //    public static void Email(this AngularHtmlTag angularHtmlTag)
    //    {
    //        WithAttr(angularHtmlTag, "type", "email");
    //    }


    //}
}
