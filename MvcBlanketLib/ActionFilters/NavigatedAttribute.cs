using System.Web.Mvc;
using MvcBlanketLib.Fluent;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace MvcBlanketLib.ActionFilters
{
    public class NavigatedAttribute : ActionFilterAttribute
    {
        public NavigatedAttribute()
        {
            PageSize = 10;
        }

        public int PageSize { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int page = 1;
            int pageSize = PageSize;
            var request = filterContext.HttpContext.Request;
            if (!string.IsNullOrWhiteSpace(request["page"]))
                int.TryParse(request["page"], out page);
            if (!string.IsNullOrWhiteSpace(request["size"]))
                int.TryParse(request["size"], out pageSize);
            filterContext.Controller.ViewBag.PageNumber = page;
            filterContext.Controller.ViewBag.PageSize = pageSize;
            GridSortOptions gso = new GridSortOptions();
            gso.Column = request["column"];

            gso.Direction = request.ToMaybe().With(r => r["direction"]).With(s => s.ToLower()).Value == "ascending" ? SortDirection.Ascending : SortDirection.Descending;
            filterContext.Controller.ViewBag.GridSortOptions = gso;
        }
    }
}
