using System.Collections.Generic;
using System.Collections;

namespace MvcBlanket.Security.Models
{
    internal class Pagination<T> : IPagination<T>
    {
        public int TotalEntries { get; set; }
        readonly IEnumerable<T> elements;

        public Pagination(IEnumerable<T> elements)
        {
            this.elements = elements;
        }            


        public IEnumerator<T> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)elements).GetEnumerator();
        }

    }
}
