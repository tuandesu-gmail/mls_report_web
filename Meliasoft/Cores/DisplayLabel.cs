using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Meliasoft.Cores
{
    public static class DisplayLabel
    {
        public static IHtmlString DisplayLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var displayName = metadata.DisplayName;

            if (metadata.IsRequired && false)
            {
                displayName = string.Format("<span class=\"ms-h3 ms-standardheader col-md-2\"><nobr>{0}<span class=\"ms-accentText\" title=\"This is a required field.\"> *</span></nobr></span>", displayName);
            }
            else
            {
                displayName = string.Format("<span class=\"ms-h3 ms-standardheader col-md-2\"><nobr>{0}</nobr></span>", displayName);
            }

            return new HtmlString(displayName);
        }
    }
}