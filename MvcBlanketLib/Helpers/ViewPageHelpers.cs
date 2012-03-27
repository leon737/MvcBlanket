using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

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

        public static DateTime GetLocalDateTime<T>(this WebViewPage<T> page, DateTime value)
            where T : class
        {
            var tz = (TimeZoneInfo)(page.ViewBag.TimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(value, tz);

        }

        public static MvcHtmlString BeginActionForm(this HtmlHelper htmlHelper)
        {
            string template = "<form method=\"post\" id=\"mform\"><input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />";
            return new MvcHtmlString(template);
        }

        public static MvcHtmlString EndActionForm(this HtmlHelper htmlHelper)
        {
            string template = "</form>";
            return new MvcHtmlString(template);
        }

        public static MvcHtmlString FilterTextBox(this HtmlHelper htmlHelper, string name, string @class = "")
        {
            string renderClass = !string.IsNullOrWhiteSpace(@class) ? "class=\"" + @class + "\"" : "";
            string template = "<input type=\"text\" name=\"s_{0}\" value=\"{1}\" {2}/>";
            string result = string.Format(template, name, GetFilter(htmlHelper.ViewData, name), renderClass);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString FilterDropDownList<T>(this HtmlHelper htmlHelper, string name, string unsetValue, string unsetText,
            IEnumerable<T> values, Func<T, string> valueSelector, Func<T, string> labelSelector, string @class = "")
        {
            StringBuilder sb = new StringBuilder();
            string selectTemplate = "<select name=\"s_{0}\">";
            sb.AppendFormat(selectTemplate, name);
            string unsetOptionTemplate = "<option value=\"{0}\">{1}</option>";
            sb.AppendFormat(unsetOptionTemplate, unsetValue, unsetText);
            string optionTemplate = "<option value=\"{0}\" {1}>{2}</option>";
            foreach (var v in values)
            {
                string selectedAttr = GetFilter(htmlHelper.ViewData, name) == valueSelector(v) ? "selected=\"selected\"" : "";
                sb.AppendFormat(optionTemplate, valueSelector(v), selectedAttr, labelSelector(v));
            }
            sb.Append("</select>");
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString BeginFilterForm(this HtmlHelper htmlHelper)
        {
            string template = "<form id=\"sform\" method=\"get\">";
            return new MvcHtmlString(template);
        }

        public static MvcHtmlString FilterFormButtons(this HtmlHelper htmlHelper, string applyText, string resetText)
        {
            string template = "<input type=\"submit\" id=\"apply\" value=\"{0}\"/><input type=\"submit\" id=\"reset\" name=\"reset\" value=\"{1}\"/>";
            string result = string.Format(template, applyText, resetText);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString EndFilterForm(this HtmlHelper htmlHelper)
        {
            string template = "</form>";
            return new MvcHtmlString(template);
        }


        public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper, string name, string text, string ifNothingSelectedText = "", string confirmText = "")
        {
            string template = "<input type=\"button\" id=\"{0}\" value=\"{1}\" class=\"cmdbtn\" {2} {3} />";
            string ifNothingSelectedTemplate = "data-empty=\"{0}\"";
            string confirmTemplate = "data-confirm=\"{0}\"";
            string renderIfNothingSelected = !string.IsNullOrWhiteSpace(ifNothingSelectedText) ? string.Format(ifNothingSelectedTemplate, ifNothingSelectedText) : "";
            string renderConfirm = !string.IsNullOrWhiteSpace(confirmText) ? string.Format(confirmTemplate, confirmText) : "";
            string result = string.Format(template, name, text, renderIfNothingSelected, renderConfirm);
            return new MvcHtmlString(result);
        }
    }
}
