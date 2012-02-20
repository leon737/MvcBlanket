using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.Models
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
