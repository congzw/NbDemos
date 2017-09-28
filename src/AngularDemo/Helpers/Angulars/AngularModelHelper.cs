using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AngularDemo.Helpers.Angulars.Forms;
using HtmlTags;

namespace AngularDemo.Helpers.Angulars
{
    public class AngularModelHelper<TModel>
    {
        public HtmlHelper Helper;
        public string ExpressionPrefix { get; private set; }

        public AngularModelHelper(HtmlHelper helper, string expressionPrefix)
        {
            Helper = helper;
            ExpressionPrefix = expressionPrefix;
        }

        /// <summary>
        /// Converts an lambda expression into a camel-cased string, prefixed
        /// with the helper's configured prefix expression, ie:
        /// vm.model.parentProperty.childProperty
        /// </summary>
        public IHtmlString ExpressionFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var expressionText = ExpressionForInternal(property);
            return new MvcHtmlString(expressionText);
        }

        /// <summary>
        /// Converts a lambda expression into a camel-cased AngularJS binding expression, ie:
        /// {{vm.model.parentProperty.childProperty}} 
        /// </summary>
        public IHtmlString BindingFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return MvcHtmlString.Create("{{" + ExpressionForInternal(property) + "}}");
        }

        /// <summary>
        /// 生成boostrap的标准表单
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public IHtmlString FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var metadata = ModelMetadata.FromLambdaExpression(property, new ViewDataDictionary<TModel>());
            var name = ExpressionHelper.GetExpressionText(property);
            var expression = ExpressionForInternal(property);

            //<div class="form-group has-feedback" form-group-validation="Name">
            var formGroup = new HtmlTag("div")
                .AddClasses("form-group", "has-feedback")
                .Attr("form-group-validation", name);

            var labelText = metadata.DisplayName ?? name;

            //<label class="control-label" for="Name">Name</label>
            var label = new HtmlTag("label")
                .AddClass("control-label")
                .Attr("for", name)
                .Text(labelText);

            var tagName = metadata.DataTypeName == "MultilineText"
                ? "textarea"
                : "input";

            var placeholder = metadata.Watermark ?? (labelText + "...");
            //<input ng-model="expression" class="form-control" name="Name" type="text" >
            var input = new HtmlTag(tagName)
                .AddClass("form-control")
                .Attr("ng-model", expression)
                .Attr("name", name)
                .Attr("type", "text")
                .Attr("placeholder", placeholder);

            ApplyValidationToInput(input, metadata);

            //todo add custom validate rules

            return formGroup
                .Append(label)
                .Append(input);
        }

        /// <summary>
        /// ng-repeat directive
        /// </summary>
        /// <typeparam name="TSubModel"></typeparam>
        /// <param name="property"></param>
        /// <param name="variableName"></param>
        /// <param name="warpTagName"></param>
        /// <param name="hasClosingTag"></param>
        /// <returns></returns>
        public AngularNgRepeatHelper<TSubModel> Repeat<TSubModel>(Expression<Func<TModel, IEnumerable<TSubModel>>> property, string variableName, string warpTagName = "div", bool hasClosingTag = true)
        {
            var propertyExpression = ExpressionForInternal(property);
            return new AngularNgRepeatHelper<TSubModel>(Helper, variableName, propertyExpression, warpTagName, hasClosingTag);
        }

        //public AngularFormBuilder<TModel> AngularFormBuilder()
        //{
        //    return new AngularFormBuilder<TModel>(Helper, ExpressionPrefix);
        //}

        //public AngularFormGroupBuilder<TModel, TProp> AngularFormGroupBuilder<TProp>(Expression<Func<TModel, TProp>> property)
        //{
        //    return new AngularFormGroupBuilder<TModel, TProp>(property, Helper, ExpressionPrefix);
        //}

        public AngularFormHelper<TModel> W5CForm(bool horizontal = false, string w5cFormValidate = null)
        {
            return new AngularFormHelper<TModel>(Helper, ExpressionPrefix, horizontal, w5cFormValidate);
        }
        
        // [x => x.Name] ----->  [_expressionPrefix.name]
        private string ExpressionForInternal<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var camelCaseName = property.Name();

            var expression = !string.IsNullOrEmpty(ExpressionPrefix)
                ? ExpressionPrefix + "." + camelCaseName
                : camelCaseName;

            return expression;
        }

        // [x => x.Name] ----->  [Model_name]
        protected string CreateUniqueNameWithPrefix(string name, string repalceDotWith = "_")
        {
            var uniqueName = ExpressionPrefix + "." + name;
            if (string.IsNullOrWhiteSpace(repalceDotWith))
            {
                return uniqueName;
            }
            return uniqueName.Replace(".", "_");
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
        
        /// <summary>
        /// 生成boostrap的标准表单
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public AngularFormGroup<TModel, TProp> WithFormGroupFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return new AngularFormGroup<TModel, TProp>(property, ExpressionPrefix);
        }
    }

    public class AngularFormGroup<TModel, TProp> : IHtmlString
    {
        private readonly string _expressionPrefix;

        public AngularFormGroup(Expression<Func<TModel, TProp>> property, string expressionPrefix)
        {
            _expressionPrefix = expressionPrefix;
            InitForm(property);
        }

        internal HtmlTag FormGroup { get; set; }
        internal HtmlTag Label { get; set; }
        internal HtmlTag Input { get; set; }

        public string ToHtmlString()
        {
            return FormGroup
                .ToHtmlString();
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
            var domId = typeof (TModel).Name + "_" + camelCaseName.Replace(".", "_");
            return domId;
        }

        //create default style forms
        protected void InitForm(Expression<Func<TModel, TProp>> property)
        {
            FormGroup = new HtmlTag("div");
            Label = new HtmlTag("label");
            Input = new HtmlTag("input");
            
            var metadata = ModelMetadata.FromLambdaExpression(property, new ViewDataDictionary<TModel>());
            var name = ExpressionHelper.GetExpressionText(property);
            var expression = ExpressionForInternal(property);

            //<div class="form-group has-feedback" form-group-validation="Name">
            FormGroup = FormGroup.TagName("div")
                .AddClasses("form-group", "has-feedback")
                .Attr("form-group-validation", name);

            var inputId = ExpressionIdForInternal(property);

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
        }
    }
    
    public static class AngularFormGroupExtensions
    {
        /// <summary>
        /// 更改为横向样式
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="angularFormGroup"></param>
        /// <returns></returns>
        public static AngularFormGroup<TModel, TProp> Horizontal<TModel, TProp>(this AngularFormGroup<TModel, TProp> angularFormGroup)
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
            angularFormGroup.Input.WrapWith(wrap);
            //angularFormGroup.Input = angularFormGroup.Input.WrapWith(wrap);
            //angularFormGroup.FormGroup.ReplaceChildren(angularFormGroup.Label, angularFormGroup.Input);
            return angularFormGroup;
        }
        
        /// <summary>
        /// 自定义属性
        /// </summary>
        /// <param name="angularFormGroup"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public static AngularFormGroup<TModel, TProp> WithAttr<TModel, TProp>(this AngularFormGroup<TModel, TProp> angularFormGroup, string attribute, object value)
        {
            angularFormGroup.Input.Attr(attribute, value);
            return angularFormGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angularFormGroup"></param>
        public static AngularFormGroup<TModel, TProp> Required<TModel, TProp>(this AngularFormGroup<TModel, TProp> angularFormGroup)
        {
            return WithAttr(angularFormGroup, "required", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angularFormGroup"></param>
        public static AngularFormGroup<TModel, TProp> Email<TModel, TProp>(this AngularFormGroup<TModel, TProp> angularFormGroup)
        {
            return WithAttr(angularFormGroup, "type", "email");
        }


    }
}