using System;
using System.Linq;
using System.Linq.Expressions;

namespace MvcBlanketLib.PageFilters
{
    public static class LinqFilterExtensions
    {
        public static IQueryable<TSource> Where<TSource, TFilterType>(this IQueryable<TSource> query, PageFilter<TFilterType> filter, Expression<Func<TSource, bool>> predicate)
        {
            return (filter.Selected && !filter.Value.Equals(filter.NotSelectedValue)) ? query.Where(predicate) : query; 
        }
    }
}
