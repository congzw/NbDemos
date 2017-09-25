using System;
using System.Web.Mvc;
using HtmlTags;

namespace AngularDemo.Helpers.Angulars
{
    public class AngularFormHelper<TModel> : AngularModelHelper<TModel>, IDisposable
    {
        private readonly HtmlHelper _helper;

        public AngularFormHelper(HtmlHelper helper, string expressionPrefix, bool horizontal = false, string validateConfig = null)
            : base(helper, expressionPrefix)
        {
            _helper = helper;
            //<form class="form-horizontal w5c-form demo-form"   role="form" novalidate name="validateForm">
            //    <div class="form-group has-feedback">
            //        <label class="col-sm-3 control-label" for="vm_entity_email">” œ‰</label>
            //        <div class="col-sm-9">
            //            <input type="email" name="email" id="vm_entity_email" ng-model="vm.entity.email" required="" class="form-control" placeholder=" ‰»Î” œ‰">
            //        </div>
            //</form>

            var formName = CreateUniqueNameWithPrefix("form");
            var htmlTag = new HtmlTag("form");
            if (horizontal)
            {
                htmlTag = htmlTag.AddClass("form-horizontal");
            }

            htmlTag = htmlTag
                .AddClass("w5c-form")
                .Attr("role", "form")
                .Attr("novalidate", "")
                .Attr("w5c-form-validate", validateConfig ?? "")
                .Attr("name", formName);
            htmlTag = htmlTag.NoClosingTag();
            _helper.ViewContext.Writer.Write(htmlTag.ToString());
        }

        void IDisposable.Dispose()
        {
            _helper.ViewContext.Writer.Write("</form>");
        }
    }
}