using System;
using System.Web.Mvc;
using HtmlTags;

namespace AngularDemo.Helpers.Angulars
{
    public class AngularNgRepeatHelper<TModel> : AngularModelHelper<TModel>, IDisposable
    {
        private readonly string _warpTag;
        private readonly bool _hasClosingTag;

        public AngularNgRepeatHelper(HtmlHelper helper, string variableName, string propertyExpression, string warpTag = "div", bool hasClosingTag = true)
            : base(helper, variableName)
        {
            _warpTag = warpTag;
            _hasClosingTag = hasClosingTag;


            var htmlTag = HtmlTag.Placeholder()
                .Name(warpTag)
                .Attr("ng-repeat", string.Format("{0} in {1}", variableName, propertyExpression));
            if (_hasClosingTag)
            {
                htmlTag = htmlTag.NoClosingTag();
            }
            Helper.ViewContext.Writer.Write(htmlTag.ToString());
        }

        void IDisposable.Dispose()
        {
            if (!_hasClosingTag)
            {
                Helper.ViewContext.Writer.Write("</{0}>", _warpTag);
            }
        }
    }
}