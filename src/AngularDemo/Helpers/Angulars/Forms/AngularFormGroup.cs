using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HtmlTags;

namespace AngularDemo.Helpers.Angulars.Forms
{
    public interface IAngularFormGroup : IHtmlString
    {
        HtmlTag FormGroup { get; set; }
        HtmlTag Label { get; set; }
        HtmlTag Input { get; set; }
        HtmlHelper HtmlHelper { get; set; }
    }
    public interface IAngularFormGroupBuilder
    {
        HtmlTag FormGroup { get; set; }
        HtmlTag Label { get; set; }
        HtmlTag Input { get; set; }
        IAngularFormGroupBuilder ConfigForBootstrap();
        IAngularFormGroupBuilder Config(Func<HtmlTag, HtmlTag> configFunc);
        IAngularFormGroup Build();
    }
    public class AngularFormGroup : IAngularFormGroup
    {
        public AngularFormGroup(HtmlTag formGroup, HtmlTag label, HtmlTag input, HtmlHelper htmlHelper)
        {
            FormGroup = formGroup;
            Label = label;
            Input = input;
            HtmlHelper = htmlHelper;
        }

        public HtmlTag FormGroup { get; set; }
        public HtmlTag Label { get; set; }
        public HtmlTag Input { get; set; }
        public HtmlHelper HtmlHelper { get; set; }
        public string ToHtmlString()
        {
            return FormGroup.ToHtmlString();
        }
    }
    public class AngularFormGroupBuilder<TModel, TProp> : IAngularFormGroupBuilder
    {
        private readonly Expression<Func<TModel, TProp>> _property;
        private readonly HtmlHelper _helper;
        private readonly string _expressionPrefix;

        public AngularFormGroupBuilder(Expression<Func<TModel, TProp>> property, HtmlHelper helper, string expressionPrefix)
        {
            _property = property;
            _helper = helper;
            _expressionPrefix = expressionPrefix;
        }

        public HtmlTag FormGroup { get; set; }
        public HtmlTag Label { get; set; }
        public HtmlTag Input { get; set; }

        public IAngularFormGroupBuilder ConfigForBootstrap()
        {
            FormGroup = new HtmlTag("div");
            Label = new HtmlTag("label");
            Input = new HtmlTag("input");

            var metadata = ModelMetadata.FromLambdaExpression(_property, new ViewDataDictionary<TModel>());
            var name = ExpressionHelper.GetExpressionText(_property);
            var expression = ExpressionForInternal(_property);

            //<div class="form-group has-feedback" form-group-validation="Name">
            FormGroup = FormGroup.TagName("div")
                .AddClasses("form-group", "has-feedback")
                .Attr("form-group-validation", name);

            var inputId = ExpressionIdForInternal(_property);

            //<label class="control-label" for="Name">Name</label>
            var labelText = metadata.DisplayName ?? name;
            Label = Label.TagName("label")
                .AddClass("control-label")
                .Attr("for", inputId)
                .Text(labelText);

            //<input ng-model="expression" class="form-control" name="Name" type="text" >
            var tagName = metadata.DataTypeName == "MultilineText"
                ? "textarea"
                : "input";
            var placeholder = metadata.Watermark ?? (labelText + "...");
            Input = Input.TagName(tagName)
                .AddClass("form-control")
                .Attr("ng-model", expression)
                .Attr("name", name)
                .Attr("type", "text")
                .Attr("id", inputId)
                .Attr("placeholder", placeholder);

            ApplyValidationToInput(Input, metadata);

            FormGroup = FormGroup.Append(Label).Append(Input);
            return this;
        }

        public IAngularFormGroupBuilder Config(Func<HtmlTag, HtmlTag> configFunc)
        {
            FormGroup = configFunc(FormGroup);
            return this;
        }

        public IAngularFormGroup Build()
        {
            return new AngularFormGroup(FormGroup, Label, Input, _helper);
        }

        //auto add validation for ModelMetadata such like "IsRequired", "EmailAddress", "PhoneNumber", ...
        private void ApplyValidationToInput(HtmlTag input, ModelMetadata metadata)
        {
            if (metadata.IsRequired)
                input.Attr("required", "");

            if (metadata.DataTypeName == "EmailAddress")
                input.Attr("type", "email");

            if (metadata.DataTypeName == "PhoneNumber")
                input.Attr("pattern", @"[\ 0-9()-]+");
        }

        // [x => x.Name] ----->  [_expressionPrefix.name]
        private string ExpressionForInternal(Expression<Func<TModel, TProp>> property)
        {
            var camelCaseName = property.ToCamelCaseName();
            var expression = !string.IsNullOrEmpty(_expressionPrefix)
                ? _expressionPrefix + "." + camelCaseName
                : camelCaseName;

            return expression;
        }

        // [x => x.Name] ----->  [Model_name]
        private string ExpressionIdForInternal(Expression<Func<TModel, TProp>> property)
        {
            var camelCaseName = property.ToCamelCaseName();
            var domId = typeof(TModel).Name + "_" + camelCaseName.Replace(".", "_");
            return domId;
        }
    }
    public static class AngularFormGroupBuilderExtensions
    {
        /// <summary>
        /// 更改为横向样式
        /// </summary>
        /// <param name="angularFormGroup"></param>
        /// <returns></returns>
        public static IAngularFormGroupBuilder Horizontal(this IAngularFormGroupBuilder angularFormGroup)
        {
            //<form class="form-horizontal w5c-form demo-form" role="form" w5c-form-validate="vm.validateOptions" novalidate name="validateForm">
            //    <div class="form-group has-feedback">
            //        <label class="col-sm-3 control-label" for="vm_entity_email">邮箱</label>
            //        <div class="col-sm-9">
            //            <input type="email" name="email" id="vm_entity_email" ng-model="vm.entity.email" required="" class="form-control" placeholder="输入邮箱">
            //        </div>
            //</form>

            angularFormGroup.Label = angularFormGroup.Label.AddClass("col-sm-3");
            var wrap = new HtmlTag("div");
            wrap = wrap.AddClass("col-sm-9");
            angularFormGroup.Input = angularFormGroup.Input.WrapWith(wrap);
            return angularFormGroup;
        }


        public static IAngularFormGroupBuilder WithInputAttr(this IAngularFormGroupBuilder angularFormGroup, string attribute, object value)
        {
            angularFormGroup.Input = angularFormGroup.Input.Attr(attribute, value);
            return angularFormGroup;
        }

        public static IAngularFormGroupBuilder Required(this IAngularFormGroupBuilder angularFormGroup)
        {
            return WithInputAttr(angularFormGroup, "required", "");
        }

        public static IAngularFormGroupBuilder Label(this IAngularFormGroupBuilder angularFormGroup, string text)
        {
            angularFormGroup.Label = angularFormGroup.Label.Text(text);
            return angularFormGroup;
        }

        public static IAngularFormGroupBuilder Email(this IAngularFormGroupBuilder angularFormGroup)
        {
            return WithInputAttr(angularFormGroup, "type", "email");
        }
    }

    public static class AngularFormBuilderExtensions
    {
        public static AngularFormBuilder<TModel> AngularFormBuilder<TModel>(this AngularModelHelper<TModel> angularModelHelper)
        {
            return new AngularFormBuilder<TModel>(angularModelHelper.Helper, angularModelHelper.ExpressionPrefix);
        }

        public static AngularFormGroupBuilder<TModel, TProp> AngularFormGroupBuilder<TModel,TProp>(this AngularModelHelper<TModel> angularModelHelper, Expression<Func<TModel, TProp>> property)
        {
            return new AngularFormGroupBuilder<TModel, TProp>(property, angularModelHelper.Helper,angularModelHelper.ExpressionPrefix);
        }
    }

}
