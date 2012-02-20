using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Security.Models
{
    internal class Pagination<T> : IPagination<T>
    {
        public int TotalEntries { get; set; }
        IEnumerable<T> elements;

        public Pagination(IEnumerable<T> elements)
        {
            this.elements = elements;
        }            


        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)elements).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)elements).GetEnumerator();
        }

    }
}
