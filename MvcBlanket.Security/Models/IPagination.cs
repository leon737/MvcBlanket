using System.Collections.Generic;

namespace MvcBlanket.Security.Models
{
    internal interface IPagination<T> : IEnumerable<T>
    {
        int TotalEntries { get; set; }
    }
}
