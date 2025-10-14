using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Meliasoft.Cores
{
    public static class DisplayValidationMessage
    {
        public static IHtmlString DisplayValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var displayName = helper.ValidationMessageFor(expression).ToString();

            return new HtmlString(displayName);
        }
    }
}