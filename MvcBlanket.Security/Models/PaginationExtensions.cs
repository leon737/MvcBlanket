﻿/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Linq;

namespace MvcBlanket.Security.Models
{
    internal static class PaginationExtensions
    {
        public static IPagination<T> AsPagination<T>(this IQueryable<T> src, int pageNo, int pageSize)
        {
            return new Pagination<T>(src.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList())
            {
                 TotalEntries = src.Count()
            };
        }

        public static IPagination<T> AsPagination<T>(this IQueryable<T> src)
        {
            return new Pagination<T>(src.ToList())
            {
                TotalEntries = src.Count()
            };
        }
    }
}
