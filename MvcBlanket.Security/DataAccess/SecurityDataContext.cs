using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security.DataAccess.DataContexts;

namespace Security.DataAccess.DataContexts
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
