﻿using System;
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
                object notSelectedValue = targetType == typeof(string) ? null : FormatterServices.GetUninitializedObject(targetType);
                Exception exception = null;

                if (!filters.ContainsKey(propertyName) || string.IsNullOrWhiteSpace(filters[propertyName]))
                {
                    object pageFilter1 = Activator.CreateInstance(pageFilterType, new[] { targetValue, false, exception, notSelectedValue });
                    property.SetValue(model, pageFilter1, null);
                    continue;
                }

                string stringValue = filters[propertyName];

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
                        property.GetCustomAttributes(typeof(PageFilterNotSelectedValueAttribute), false).FirstOrDefault() as PageFilterNotSelectedValueAttribute;
                    if (notSelectedValueAttribute != null)
                    {
                        string stringNotSelectedValue = notSelectedValueAttribute.NotSelectedValue;
                        try
                        {
                            notSelectedValue = Convert.ChangeType(stringNotSelectedValue, targetType);

                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }
                }

                object pageFilter = Activator.CreateInstance(pageFilterType, new[] { targetValue, exception == null, exception, notSelectedValue });
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