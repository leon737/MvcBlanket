using System.Data.Linq;
using System;

namespace MvcBlanket.Security.Models
{
    internal abstract class RepositoryBase<TDataContext>
        where TDataContext : DataContext, new()
    {

        readonly Lazy<TDataContext> context = new Lazy<TDataContext>();

        protected TDataContext Context
        {
            get { return context.Value; }
        }

        protected void Submit()
        {
            Context.SubmitChanges();
        }
    }
}
