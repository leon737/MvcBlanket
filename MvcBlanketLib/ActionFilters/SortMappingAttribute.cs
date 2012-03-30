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
