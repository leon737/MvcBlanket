using System;
using System.Linq;
using System.Web.Mvc;
using MvcBlanketLib.ViewModels;

namespace MvcBlanketLib.ActionFilters
{
    public class SortMappingAttribute : ActionFilterAttribute
    {
        public string Mapping { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Mapping == null) return;
            filterContext.HttpContext.Items[SortMapping.ContentItemName] = Mapping.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(set => set.Split('='))
                .Select(subsets => new SortMapping {CommonName = subsets[0], ColumnNames = subsets[1].Split(',')})
                .ToList();
        }
    }
}
