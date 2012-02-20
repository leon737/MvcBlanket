using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.Models
{
    internal interface IPagination<T> : IEnumerable<T>
    {
        int TotalEntries { get; set; }
    }
}
