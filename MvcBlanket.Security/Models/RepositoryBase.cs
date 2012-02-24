using System.Data.Entity;
using System;

namespace MvcBlanket.Security.Models
{
    internal abstract class RepositoryBase<TDataContext>
        where TDataContext : DbContext, new()
    {

        readonly Lazy<TDataContext> context = new Lazy<TDataContext>();

        protected TDataContext Context
        {
            get { return context.Value; }
        }

        protected void Submit()
        {
            Context.SaveChanges();
        }
    }
}
