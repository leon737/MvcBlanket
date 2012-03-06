using System;
using System.Linq;
using System.Linq.Expressions;
using MvcContrib.Sorting;

namespace MvcBlanketLib.Extensions
{
    public static class SortExtensions
    {

        private static string GetSortMethod (SortDirection direction, bool first)
        {
            return (first ? "OrderBy" : "ThenBy") + (direction == SortDirection.Descending ? "Descending" : "");
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> datasource, string propertyName, SortDirection direction, bool first)
        {
            if (string.IsNullOrEmpty(propertyName))
                return datasource;
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException(string.Format("Could not find a property called '{0}' on type {1}", propertyName, type));
            var parameterExpression = Expression.Parameter(type, "p");
            var lambdaExpression = Expression.Lambda(Expression.MakeMemberAccess(parameterExpression, property), new[] { parameterExpression });
            var methodCallExpression = Expression.Call(typeof(Queryable), GetSortMethod(direction, first), new[] { type, property.PropertyType}, 
                new[] { datasource.Expression, Expression.Quote(lambdaExpression) });
            return datasource.Provider.CreateQuery<T>(methodCallExpression);
        }
    }
}
