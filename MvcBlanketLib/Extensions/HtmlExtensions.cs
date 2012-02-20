/*******************************************************************\
* Module Name: Lt.Helpers
* 
* File Name: Extensions/HtmlExtensions.cs
*
* Warnings:
*
* Issues:
*
* Created:  09 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/


using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web.Routing;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace MvcBlanketLib.Extensions
{
	public static class HtmlExtensions
	{
		public static MvcHtmlString MenuItem(this HtmlHelper helper,
			  string linkText, string actionName, string controllerName)
		{
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];

			var builder = new TagBuilder("li");
			if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
				  && currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
				builder.AddCssClass("selected");
			builder.InnerHtml = helper.ActionLink(linkText, actionName, controllerName).ToHtmlString();
			return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
		}

		public static MvcHtmlString ActionQueryLink(this HtmlHelper htmlHelper,
			  string linkText, string action, object routeValues)
		{
			var newRoute = routeValues == null
				  ? htmlHelper.ViewContext.RouteData.Values
				  : new RouteValueDictionary(routeValues);

			newRoute = htmlHelper.ViewContext.HttpContext.Request.QueryString
				  .ToRouteDic(newRoute);

			return HtmlHelper.GenerateLink(htmlHelper.ViewContext.RequestContext,
				  htmlHelper.RouteCollection, linkText, null,
				  action, null, newRoute, null).ToMvcHtml();
		}

		public static MvcHtmlString ToMvcHtml(this string content)
		{
			return MvcHtmlString.Create(content);
		}

		public static RouteValueDictionary ToRouteDic(this NameValueCollection collection)
		{
			return collection.ToRouteDic(new RouteValueDictionary());
		}

		public static RouteValueDictionary ToRouteDic(this NameValueCollection collection,
			  RouteValueDictionary routeDic)
		{
			foreach (string key in collection.Keys)
			{
				if (!routeDic.ContainsKey(key))
					routeDic.Add(key, collection[key]);
			}
			return routeDic;
		}

		public static MvcHtmlString RawCheckBox(this HtmlHelper helper, string name, object value, string @class)
		{
			return MvcHtmlString.Create(string.Format("<input type='checkbox' name='{0}' id='{0}' value='{1}' class='{2}' />",
				 name, value.ToString(), @class));
		}

		public static MvcHtmlString HyperlinkQ(this HtmlHelper helper, string qsKey, string qsValue)
		{
			return HyperlinkQ(helper, new[] { new QSPair { Key = qsKey, Value = qsValue } });
		}

		public static MvcHtmlString HyperlinkQ(this HtmlHelper helper, IEnumerable<QSPair> qs)
		{
			var originalQs = helper.ViewContext.HttpContext.Request.QueryString;
			var transformedQs = new List<QSPair>(originalQs.Count);
			foreach (string key in originalQs.Keys)
				if (!string.IsNullOrEmpty(key))
					transformedQs.Add(new QSPair { Key = key, Value = originalQs[key] });
			string result = qs.Union(transformedQs).Aggregate("", (a, v) => a + (a.Length > 0 ? "&" : "") + v);
			return MvcHtmlString.Create(result);
		}

        public static MvcHtmlString TextBoxWithMaxLengthFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;
            var stringLength = member.Member
                .GetCustomAttributes(typeof(StringLengthAttribute), false)
                .FirstOrDefault() as StringLengthAttribute;

            var attributes = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);
            if (stringLength != null)
            {
                attributes.Add("maxlength", stringLength.MaximumLength);
            }
            return htmlHelper.TextBoxFor(expression, attributes);
        }


	}

	public class QSPair
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public override bool Equals(object obj)
		{
			QSPair other = obj as QSPair;
			if (other == null) return false;
			return other.Key == Key;
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", Key, Value);
		}
	}

}
