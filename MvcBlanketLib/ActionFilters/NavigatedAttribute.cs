/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

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
