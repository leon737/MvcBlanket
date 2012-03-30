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
