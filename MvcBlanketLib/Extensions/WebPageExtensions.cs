using System.Web.WebPages;

namespace MvcBlanketLib.Extensions
{
	public static class WebPageHelpers
	{
		public static void PropagateSection(this WebPageBase page, string sectionName)
		{
			if (page.IsSectionDefined(sectionName))
			{
				page.DefineSection(sectionName, () => page.Write(page.RenderSection(sectionName)));
			}
		}

		public static void PropagateSections(this WebPageBase page, params string[] sections)
		{
			foreach (var s in sections)
				PropagateSection(page, s);
		}
	}
}
