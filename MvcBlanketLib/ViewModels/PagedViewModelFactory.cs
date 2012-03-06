using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
