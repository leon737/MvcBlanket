/*******************************************************************\
* Module Name: Lt.Helpers
* 
* File Name: Extensions/QueryExtension.cs
*
* Warnings:
*
* Issues:
*
* Created:  09 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/


using System.Linq;
using System.Web.Mvc;

namespace MvcBlanketLib.Extensions
{
	public static class QueryExtention
	{
		public static SelectList ToSelectList<T>(this IQueryable<T> query, string dataValueField, string dataTextField, object selectedValue)
		{
			return new SelectList(query, dataValueField, dataTextField, selectedValue ?? -1);
		}
	}

}
