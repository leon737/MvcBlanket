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
