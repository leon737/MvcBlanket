using System.Web.Mvc;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;

namespace MvcBlanketLib.ViewModels
{
    public static class PagedViewModelFactory
    {
        public static PagedViewModel<T> Create<T>(ViewDataDictionary viewData, string defaultSortColumn = "", SortDirection defaultSortDirection = SortDirection.Ascending)
        {
            return new PagedViewModel<T>
                   {
                       ViewData = viewData,
                       GridSortOptions = viewData["GridSortOptions"] as GridSortOptions,
                       DefaultSortColumn = defaultSortColumn,
                       DefaultSortDirection = defaultSortDirection,
                       Page = (int) viewData["PageNumber"],
                       PageSize = (int) viewData["PageSize"]
                   };
        }

        public static PagedViewModel<T> Create<T>(ControllerContext context, string defaultSortColumn = "", SortDirection defaultSortDirection = SortDirection.Ascending)
        {
            return new PagedViewModel<T>
            {
                ControllerContext = context,
                GridSortOptions = context.Controller.ViewBag.GridSortOptions,
                DefaultSortColumn = defaultSortColumn,
                DefaultSortDirection = defaultSortDirection,
                Page = context.Controller.ViewBag.PageNumber,
                PageSize = context.Controller.ViewBag.PageSize
            };
        }
    }
}
