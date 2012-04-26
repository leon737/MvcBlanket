/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using MvcBlanketLib.ModelBinders;
using MvcBlanketLib.PageFilters;
using System.Globalization;

namespace MvcBlanketLib.Helpers
{
    public static class ViewPageHelpers
    {
        public static string GetFilter(dynamic viewBag, string key)
        {
            if (viewBag.Filters == null) return string.Empty;
            if (viewBag.Filters.ContainsKey(key))
                return viewBag.Filters[key] as string;
            return string.Empty;
        }

        public static string GetFilter(ViewDataDictionary viewData, string key)
        {
            var filters = viewData["Filters"] as Dictionary<string, string>;
            if (filters == null) return string.Empty;
            if (filters.ContainsKey(key))
                return filters[key];
            return string.Empty;
        }


        private static Tuple<object, bool> GetModelFilter(ViewDataDictionary viewData, Type modelType, string propName, Type propType)
        {
            var filtersModel = viewData["FiltersModel"];
            var property = modelType.GetProperty(propName);
            var pageFilter = property.GetValue(filtersModel, BindingFlags.GetProperty, null, null, null);
            var pageFilterType = typeof (IPageFilter<>).MakeGenericType(new[] {propType});
            var valueProperty = pageFilterType.GetProperty("Value");
            var value = valueProperty.GetValue(pageFilter, BindingFlags.GetProperty, null, null, null);
            var selectedProperty = pageFilterType.GetProperty("Selected");
            var selected = (bool)selectedProperty.GetValue(pageFilter, BindingFlags.GetProperty, null, null, null);
            return Tuple.Create(value, selected);
        }


        public static DateTime GetLocalDateTime<T>(this WebViewPage<T> page, DateTime value)
            where T : class
        {
            var tz = (TimeZoneInfo)(page.ViewBag.TimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(value, tz);

        }

        public static MvcHtmlString BeginActionForm(this HtmlHelper htmlHelper)
        {
            const string template = "<form method=\"post\" id=\"mform\"><input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />";
            return new MvcHtmlString(template);
        }

        public static MvcHtmlString EndActionForm(this HtmlHelper htmlHelper)
        {
            const string template = "</form>";
            return new MvcHtmlString(template);
        }

        public static MvcHtmlString FilterTextBox(this HtmlHelper htmlHelper, string name, string label = "", string @class = "")
        {
            var span = RenderFilterItemSpan(label);
            string renderClass = !string.IsNullOrWhiteSpace(@class) ? "class=\"" + @class + "\"" : "";
            const string template = "<input type=\"text\" name=\"s_{0}\" value=\"{1}\" {2}/>";
            string result = span + string.Format(template, name, GetFilter(htmlHelper.ViewData, name), renderClass);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString FilterTextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, PageFilter<TProperty>>> expression, string label = "", string @class = "")
            where TModel : IPageFiltersModel
        {
            var span = RenderFilterItemSpan(label);
            string renderClass = !string.IsNullOrWhiteSpace(@class) ? "class=\"" + @class + "\"" : "";
            string propName = ExpressionHelper.GetExpressionText(expression);
            MemberInfo mi = GetPageFilterPropertyType(expression);
            string name = GetPageFilterPropName(mi, propName);
            const string template = "<input type=\"text\" name=\"s_{0}\" value=\"{1}\" {2}/>";
            var filterValue = GetModelFilter(htmlHelper.ViewData, typeof(TModel), propName, typeof(TProperty));
            string result = span + string.Format(template, name, filterValue.Item2 ? filterValue.Item1 : "", renderClass);
            return new MvcHtmlString(result);
        }

        private static MemberInfo GetPageFilterPropertyType<TModel, TProperty>(Expression<Func<TModel, PageFilter<TProperty>>> expression)
            where TModel : IPageFiltersModel
        {
            var lambda = expression.Body as MemberExpression;
            if (lambda == null) throw new NotSupportedException("Only member access lambdas are supported");
            return lambda.Member;
        }


        private static string GetPageFilterPropName(MemberInfo memberInfo, string propName)
        {
            var aliasAttribute = memberInfo.GetCustomAttributes(typeof (AliasAttribute), false).FirstOrDefault() as AliasAttribute;
            if (aliasAttribute != null)
                return aliasAttribute.Name;
            return propName.ToLowerInvariant();
        }

        private static CultureInfo GetPageFilterPropCultureInfo(MemberInfo memberInfo)
        {
            var localeAttribute = memberInfo.GetCustomAttributes(typeof(LocaleAttribute), false).FirstOrDefault() as LocaleAttribute;
            if (localeAttribute == null)
                return CultureInfo.CurrentUICulture;
            return localeAttribute.GetSelectedCultureInfo();
        }

        public static MvcHtmlString FilterDropDownList<T>(this HtmlHelper htmlHelper, string name, string unsetValue, string unsetText,
            IEnumerable<T> values, Func<T, string> valueSelector, Func<T, string> labelSelector, string label = "", string @class = "")
        {
            var span = RenderFilterItemSpan(label);
            var sb = new StringBuilder();
            const string selectTemplate = "<select name=\"s_{0}\">";
            sb.AppendFormat(selectTemplate, name);
            const string unsetOptionTemplate = "<option value=\"{0}\">{1}</option>";
            sb.AppendFormat(unsetOptionTemplate, unsetValue, unsetText);
            const string optionTemplate = "<option value=\"{0}\" {1}>{2}</option>";
            foreach (var v in values)
            {
                string selectedAttr = GetFilter(htmlHelper.ViewData, name) == valueSelector(v) ? "selected=\"selected\"" : "";
                sb.AppendFormat(optionTemplate, valueSelector(v), selectedAttr, labelSelector(v));
            }
            sb.Append("</select>");
            return new MvcHtmlString(span + sb);
        }

        public static MvcHtmlString FilterDropDownList<T, TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, PageFilter<TProperty>>> expression, string unsetValue, string unsetText,
            IEnumerable<T> values, Func<T, string> valueSelector, Func<T, string> labelSelector, string label = "", string @class = "")
             where TModel : IPageFiltersModel
        {
            var span = RenderFilterItemSpan(label);
            string propName = ExpressionHelper.GetExpressionText(expression);
            MemberInfo mi = GetPageFilterPropertyType(expression);
            string name = GetPageFilterPropName(mi, propName);

            var sb = new StringBuilder();
            const string selectTemplate = "<select name=\"s_{0}\">";
            sb.AppendFormat(selectTemplate, name);
            const string unsetOptionTemplate = "<option value=\"{0}\">{1}</option>";
            sb.AppendFormat(unsetOptionTemplate, unsetValue, unsetText);
            const string optionTemplate = "<option value=\"{0}\" {1}>{2}</option>";

            var filterValue = GetModelFilter(htmlHelper.ViewData, typeof(TModel), propName, typeof(TProperty));
            string stringValue = filterValue.Item2 ? filterValue.Item1.ToString() : "";
            foreach (var v in values)
            {
                string selectedAttr = stringValue == valueSelector(v) ? "selected=\"selected\"" : "";
                sb.AppendFormat(optionTemplate, valueSelector(v), selectedAttr, labelSelector(v));
            }
            sb.Append("</select>");
            return new MvcHtmlString(span + sb);
        }

        public static MvcHtmlString FilterDateBox(this HtmlHelper htmlHelper, string name, string label = "", string @class = "")
        {
            var span = RenderFilterItemSpan(label);
            string renderClass = "class=\"single-datepicker" + (!string.IsNullOrWhiteSpace(@class) ? @class : "") + "\"";
            string template = "<input type=\"text\" value=\"{1}\" {2}/>";
            string result = span + string.Format(template, name, GetFilter(htmlHelper.ViewData, name), renderClass);
            template = "<input type=\"hidden\" name=\"s_{0}\" value=\"{1}\" />";
            result += string.Format(template, name, GetFilter(htmlHelper.ViewData, name));
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString FilterDateBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, 
            PageFilter<TProperty>>> expression, string label = "", string @class = "")
              where TModel : IPageFiltersModel
        {
            var span = RenderFilterItemSpan(label);
            string renderClass = "class=\"single-datepicker" + (!string.IsNullOrWhiteSpace(@class) ? @class : "") + "\"";
            string propName = ExpressionHelper.GetExpressionText(expression);
            MemberInfo mi = GetPageFilterPropertyType(expression);
            string name = GetPageFilterPropName(mi, propName);
            string template = "<input type=\"text\" value=\"{1}\" {2}/>";
            var filterValue = GetModelFilter(htmlHelper.ViewData, typeof(TModel), propName, typeof(TProperty));
            DateTime value = (DateTime)filterValue.Item1;            
            string stringValue = filterValue.Item2 ? value.ToString("d", GetPageFilterPropCultureInfo(mi)) : "";
            string result = span + string.Format(template, name, stringValue, renderClass);
            template = "<input type=\"hidden\" name=\"s_{0}\" value=\"{1}\" />";
            result += string.Format(template, name, GetFilter(htmlHelper.ViewData, name));
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString BeginFilterForm(this HtmlHelper htmlHelper)
        {
            const string template = "<form id=\"sform\" method=\"get\">";
            return new MvcHtmlString(template);
        }

        static string RenderFilterItemSpan(string label) 
        {
            if (string.IsNullOrWhiteSpace(label)) return string.Empty;
            return string.Format("<span>{0}</span>", label);
        }

        public static MvcHtmlString FilterFormButtons(this HtmlHelper htmlHelper, string applyText, string resetText)
        {
            const string template = "<input type=\"submit\" id=\"apply\" value=\"{0}\"/><input type=\"submit\" id=\"reset\" name=\"reset\" value=\"{1}\"/>";
            string result = string.Format(template, applyText, resetText);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString EndFilterForm(this HtmlHelper htmlHelper)
        {
            const string template = "</form>";
            return new MvcHtmlString(template);
        }


        public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper, string name, string text, string ifNothingSelectedText = "", string confirmText = "")
        {
            const string template = "<input type=\"button\" id=\"{0}\" value=\"{1}\" class=\"cmdbtn\" {2} {3} />";
            const string ifNothingSelectedTemplate = "data-empty=\"{0}\"";
            const string confirmTemplate = "data-confirm=\"{0}\"";
            string renderIfNothingSelected = !string.IsNullOrWhiteSpace(ifNothingSelectedText) ? string.Format(ifNothingSelectedTemplate, ifNothingSelectedText) : "";
            string renderConfirm = !string.IsNullOrWhiteSpace(confirmText) ? string.Format(confirmTemplate, confirmText) : "";
            string result = string.Format(template, name, text, renderIfNothingSelected, renderConfirm);
            return new MvcHtmlString(result);
        }

        public static string GetActionEnumString<TPage, TEnum>(this WebViewPage<TPage> page, TEnum enumValue)
        {
            return new ActionEnumConverter<TEnum>(enumValue);
        }
    }
}
