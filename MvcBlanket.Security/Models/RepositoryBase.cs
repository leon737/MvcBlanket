using System.Data.Linq;

namespace Security.Models
{
    internal abstract class RepositoryBase<TDataContext>
        where TDataContext : DataContext, new()
    {
        object syncLock = new object();
        TDataContext context;

        protected TDataContext Context
        {
            get
            {
                if (context == null)
                    lock (syncLock)
                        if (context == null)
                            context = new TDataContext();
                return context;
            }
        }

        protected void Submit()
        {
            Context.SubmitChanges();
        }
    }
}
