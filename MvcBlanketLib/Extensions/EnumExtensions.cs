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
using System.ComponentModel;

namespace MvcBlanketLib.Extensions
{
	/// <summary>
	/// Extension methods for enums
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Get the description attribute for the enum
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string Description(this Enum e)
		{
			var da = (DescriptionAttribute[])(e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false));

			return da.Length > 0 ? da[0].Description : e.ToString();
		}

	}
}
