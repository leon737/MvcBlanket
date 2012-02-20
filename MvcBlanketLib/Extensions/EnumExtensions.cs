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
