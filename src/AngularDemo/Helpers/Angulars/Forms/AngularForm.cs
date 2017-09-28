using System;
using System.Web.Mvc;
using HtmlTags;

namespace AngularDemo.Helpers.Angulars.Forms
{
    public interface IAngularForm : IDisposable
    {
        HtmlTag Form { get; set; }
        HtmlHelper HtmlHelper { get; set; }
    }

    public interface IAngularFormBuilder
    {
        HtmlTag Form { get; set; }
        IAngularFormBuilder Config(Func<HtmlTag, HtmlTag> configFunc);
        IAngularForm Build();
    }

    public class AngularForm : IAngularForm
    {
        public AngularForm(HtmlTag form, HtmlHelper htmlHelper)
        {
            Form = form;
            HtmlHelper = htmlHelper;

            Init(htmlHelper);
        }

        private void Init(HtmlHelper htmlHelper)
        {
            Form = Form.NoClosingTag();
            htmlHelper.ViewContext.Writer.Write(Form.ToString());
        }

        public HtmlTag Form { get; set; }
        public HtmlHelper HtmlHelper { get; set; }

        public void Dispose()
        {
            HtmlHelper.ViewContext.Writer.Write("</form>");
        }
    }

    public class AngularFormBuilder<TModel> : AngularModelHelper<TModel>, IAngularFormBuilder
    {
        private readonly HtmlHelper _helper;

        public AngularFormBuilder(HtmlHelper helper, string expressionPrefix)
            : base(helper, expressionPrefix)
        {
            _helper = helper;
            Form = new HtmlTag("form");
        }

        public HtmlTag Form { get; set; }

        public IAngularFormBuilder Config(Func<HtmlTag, HtmlTag> configFunc)
        {
            Form = configFunc(Form);
            return this;
        }

        public IAngularForm Build()
        {
            var formName = Form.Attr("name");
            if (string.IsNullOrWhiteSpace(formName))
            {
                formName = CreateUniqueNameWithPrefix("form");
                Form = Form.Attr("name", formName);
            }
            return new AngularForm(Form, _helper);
        }
    }
}