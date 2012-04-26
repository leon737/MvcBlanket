using System;
using System.Globalization;

namespace MvcBlanketLib.PageFilters
{
    public class LocaleAttribute : Attribute
    {
        public string LocaleName { get; set; }
        public bool UseClientLocale { get; set; }

        public LocaleAttribute()
        {
            UseClientLocale = true;
        }

        public LocaleAttribute(string localeName)
        {
            LocaleName = localeName;
            UseClientLocale = false;
        }

        public CultureInfo GetSelectedCultureInfo ()
        {
            if (UseClientLocale)
                return CultureInfo.CurrentUICulture;
            return CultureInfo.CreateSpecificCulture(LocaleName);
        }
    }
}
