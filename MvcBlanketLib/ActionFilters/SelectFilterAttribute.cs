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
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using MvcBlanketLib.PageFilters;

namespace MvcBlanketLib.ActionFilters
{
    public class SelectFilterAttribute : ActionFilterAttribute
    {
        public Type FiltersModel { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var filters = InitializeFilters(filterContext.HttpContext.Request);
            filterContext.Controller.ViewBag.Filters = filters;
            if (FiltersModel != null)
            {
                var filtersModel = ConvertToModel(filters);
                filterContext.Controller.ViewBag.FiltersModel = filtersModel;
            }
        }

        private dynamic ConvertToModel(Dictionary<string, string> filters)
        {
            object model = Activator.CreateInstance(FiltersModel);
            var properties = FiltersModel.GetProperties();
            foreach (var property in properties.Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(PageFilter<>)))
            {
                var targetType = property.PropertyType.GetGenericArguments()[0];
                string propertyName = property.Name.ToLowerInvariant();

                var pageFilterType = typeof(PageFilter<>).MakeGenericType(new[] { targetType });
                object targetValue = targetType == typeof(string) ? null : FormatterServices.GetUninitializedObject(targetType);
                string stringValue = string.Empty;
                string notSelectedValue = string.Empty;
                Exception exception = null;

                if (!filters.ContainsKey(propertyName) || string.IsNullOrWhiteSpace(filters[propertyName]))
                {
                    object pageFilter1 = Activator.CreateInstance(pageFilterType, new[] { targetValue, false, null, stringValue, notSelectedValue });
                    property.SetValue(model, pageFilter1, null);
                    continue;
                }

                stringValue = filters[propertyName];

                try
                {
                    targetValue = Convert.ChangeType(stringValue, targetType);

                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception == null)
                {
                    var notSelectedValueAttribute =
                        property.GetCustomAttributes(typeof(NotSelectedValueAttribute), false).FirstOrDefault() as NotSelectedValueAttribute;
                    if (notSelectedValueAttribute != null)
                        notSelectedValue = notSelectedValueAttribute.NotSelectedValue;                        
                }

                object pageFilter = Activator.CreateInstance(pageFilterType, new[] { targetValue, exception == null, exception, stringValue, notSelectedValue });
                property.SetValue(model, pageFilter, null);
            }
            return model;
        }

        protected Dictionary<string, string> InitializeFilters(HttpRequestBase request)
        {
            var filters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(request.QueryString["reset"]))
                return filters;

            foreach (string key in request.QueryString)
                if (key.StartsWith("s_"))
                {
                    var filterName = key.Substring(2).ToLowerInvariant();
                    var filterValue = request.QueryString[key];
                    if (filters.ContainsKey(filterName))
                        filters[filterName] = filters[filterName] + "," + filterValue;
                    else
                        filters.Add(filterName, filterValue);
                }
            return filters;
        }
    }
}
