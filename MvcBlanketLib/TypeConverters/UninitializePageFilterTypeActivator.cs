﻿/*
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
using System.Runtime.Serialization;

namespace MvcBlanketLib.TypeConverters
{
    internal static class UninitializePageFilterTypeActivator
    {
        public static object CreateUnitializedObject(Type targetType)
        {
            if (targetType == typeof(string)) return null;
            if (targetType.IsGenericType) 
            {
                return null;
            }
            return FormatterServices.GetUninitializedObject(targetType);
        }
    }
}
