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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcBlanketLib.PageFilters;

namespace MvcBlanketLib.TypeConverters
{
    internal static class PageFilterTypeConverter
    {
        public static object Convert(string stringValue, Type targetType)
        {
            if (targetType.IsGenericType)
            {
                var generic = targetType.GetGenericArguments()[0];
                var typeDefinition = targetType.GetGenericTypeDefinition();

                if (typeof(IEnumerable<>) == typeDefinition)
                {   
                    var stringValues = stringValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    var resultsListType = typeof(List<>).MakeGenericType(generic);
                    var resultsList = Activator.CreateInstance(resultsListType);
                    var addMethod = resultsListType.GetMethod("Add");
                    foreach (var stringValueEntry in stringValues)
                    {
                        var convertedValue = System.Convert.ChangeType(stringValueEntry, generic);
                        addMethod.Invoke(resultsList, new[] { convertedValue });
                    }
                    return resultsList;
                }
                if (typeof(IRange<>) == typeDefinition)
                {
                    var stringValues = stringValue.Split(',');
                    if (stringValues.Length < 2) return null;
                    var stringLowerBound = stringValues.First();
                    var stringUpperBound = stringValues.Skip(1).First();
                    var rangeType = typeof(Range<>).MakeGenericType(generic);
                    var range = Activator.CreateInstance(rangeType, new[] 
                    { System.Convert.ChangeType(stringLowerBound, generic), System.Convert.ChangeType(stringUpperBound, generic) });
                    return range;
                }
            }
            return System.Convert.ChangeType(stringValue, targetType);
        }
    }
}
