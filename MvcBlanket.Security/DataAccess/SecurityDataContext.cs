using System.Linq;
using MvcBlanket.Security.DataAccess.DataContexts;

namespace MvcBlanket.Security.DataAccess
{
    internal partial class SecurityDataContext
    {
        public IQueryable<User> Individuals
        {
            get
            {
                return Users.Where(u => !u.IsGroup);
            }
        }
    }
}
