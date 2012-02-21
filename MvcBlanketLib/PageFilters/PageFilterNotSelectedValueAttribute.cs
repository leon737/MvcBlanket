using System;

namespace MvcBlanketLib.PageFilters
{
    public class PageFilterNotSelectedValueAttribute : Attribute
    {
        public string NotSelectedValue { get; set; }
    }
}
