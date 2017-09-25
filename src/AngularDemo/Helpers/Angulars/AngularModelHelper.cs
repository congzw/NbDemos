using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HtmlTags;

namespace AngularDemo.Helpers.Angulars
{
    public class AngularModelHelper<TModel>
    {
        protected HtmlHelper Helper;
        private readonly string _expressionPrefix;

        public AngularModelHelper(HtmlHelper helper, string expressionPrefix)
        {
            Helper = helper;
            _expressionPrefix = expressionPrefix;
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
        
        // [x => x.Name] ----->  [_expressionPrefix.name]
        private string ExpressionForInternal<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var camelCaseName = property.Name();

            var expression = !string.IsNullOrEmpty(_expressionPrefix)
                ? _expressionPrefix + "." + camelCaseName
                : camelCaseName;

            return expression;
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
    }
}