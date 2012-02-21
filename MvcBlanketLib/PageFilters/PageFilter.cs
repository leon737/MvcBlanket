using System;

namespace MvcBlanketLib.PageFilters
{
    public class PageFilter<T>
    {
        public PageFilter(T value, bool selected, Exception formatException, T notSelectedValue)
        {
            this.value = value;
            this.selected = selected;
            this.formatException = formatException;
            this.notSelectedValue = notSelectedValue;
        }

        private readonly T value;
        public T Value
        {
            get { return value; }
        }

        private readonly bool selected;
        public bool Selected
        {
            get { return selected; }
        }

        private readonly Exception formatException;
        public Exception FormatException
        {
            get { return formatException; }
        }

        private readonly T notSelectedValue;
        internal T NotSelectedValue
        {
            get { return notSelectedValue; }
        }
    }
}
