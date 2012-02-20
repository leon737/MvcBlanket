/*******************************************************************\
* Module Name: Lt.Helpers
* 
* File Name: Extensions/StringExtensions.cs
*
* Warnings:
*
* Issues:
*
* Created:  09 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/


namespace MvcBlanketLib.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}
