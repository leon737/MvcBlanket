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
using System.Collections.Generic;
using System.Linq;
using MvcBlanket.Security.DataAccess;
using MvcBlanket.Security.DataAccess.DataContexts;
using MvcBlanket.Security.Providers;
using System.Linq.Expressions;
using System.Reflection;

namespace MvcBlanket.Security.Models
{
    internal class SecurityRepository : RepositoryBase<SecurityDataContext>
    {
        public bool TestUserByLogin(string login)
        {
            return Context.Individuals.Any(v => v.Login == login);
        }

        public bool TestUserByEmail(string email)
        {
            return Context.Individuals.Any(v => v.EMail == email);
        }

        public User SaveUser(User model)
        {
            var user = Context.Individuals.FirstOrDefault(v => v.Id == model.Id);
            if (user == null)
            {
                user = new User
                {
                    RegDate = DateTime.UtcNow,
                    ActivationKey = Guid.NewGuid()
                };
                Context.Users.InsertOnSubmit(user);
            }

            user.Login = model.Login;
            user.EMail = model.EMail;
            if (model.Salt != null) user.Salt = model.Salt;
            user.FirstName = model.FirstName;
            user.SecondName = model.SecondName;
            user.LastName = model.LastName;
            if (!string.IsNullOrEmpty(model.PasswordQuestion))
                user.PasswordQuestion = model.PasswordQuestion;
            if (!string.IsNullOrEmpty(model.PasswordAnswer))
                user.PasswordAnswer = model.PasswordAnswer;
            user.IsActive = model.IsActive;
            user.IsGroup = model.IsGroup;
            user.Modified = DateTime.UtcNow;
            
            Submit();

            return user;
        }

        public void SaveUser(AccountMembershipUser account)
        {
            var updatedAccount = GetUser(account.Id);
            updatedAccount.FirstName = account.FirstName;
            updatedAccount.SecondName = account.SecondName;
            updatedAccount.LastName = account.LastName;
            updatedAccount.EMail = account.Email;
            updatedAccount.Modified = DateTime.UtcNow;
            updatedAccount.IsActive = !account.IsLockedOut;
            SaveUser(updatedAccount);
        }

        public IPagination<User> GetUsers()
        {
            return Context.Individuals.AsPagination();
        }


        public IPagination<User> GetUsers(int pageNo, int pageSize)
        {
            return Context.Individuals.AsPagination(pageNo, pageSize);
        }

        public int GetNumberOfUsersOnline(int intervalInMinutes)
        {
            return Context.Individuals.Count(u => u.LastActivity > DateTime.UtcNow.AddMinutes(-intervalInMinutes));
        }

        public User GetUser(string login)
        {
            return Context.Individuals.FirstOrDefault(v => v.Login == login);
        }

        public User GetUserByLogin(string login)
        {
            return GetUser(login);
        }

        public User GetUserByEmail(string email)
        {
            return Context.Individuals.FirstOrDefault(v => v.EMail == email);
        }

        public T GetUser<T> (string login, Func<User, T> selector) 
        {
            return Context.Individuals.Where(v => v.Login == login).Select(selector).FirstOrDefault();
        }

        public T GetUserByLogin<T>(string login, Func<User, T> selector)
        {
            return GetUser(login, selector);
        }

        public T GetUserByEmail<T>(string email, Func<User, T> selector)
        {
            return Context.Individuals.Where(v => v.EMail == email).Select(selector).FirstOrDefault();
        }


        public AccountMembershipUser GetAccount (string login) 
        {
            return Context.Individuals.Where(u => u.Login == login)
                .Select(u => new AccountMembershipUser(u.Id, u.Login, u.Salt, u.FirstName, u.SecondName,
                    u.LastName, u.EMail, u.RegDate, u.IsActive)).FirstOrDefault();
        }

        public User GetUser(int id)
        {
            return Context.Individuals.FirstOrDefault(v => v.Id == id);
        }

        public T GetUser<T>(int id, Func<User, T> selector)
        {
            return Context.Individuals.Where(v => v.Id == id).Select(selector).FirstOrDefault();
        }

        public AccountMembershipUser GetAccount(int id)
        {
            return Context.Individuals.Where(u => u.Id == id)
                .Select(u => new AccountMembershipUser(u.Id, u.Login, u.Salt, u.FirstName, u.SecondName,
                    u.LastName, u.EMail, u.RegDate, u.IsActive)).FirstOrDefault();
        }

        public Role GetRole(string roleName)
        {
            return Context.Roles.FirstOrDefault(r => r.Name == roleName);
        }

        public Role SaveRole(Role model)
        {
            var role = Context.Roles.FirstOrDefault(r => r.Id == model.Id);
            if (role == null)
            {
                role = new Role();
                Context.Roles.InsertOnSubmit(role);
            }

            role.Name = model.Name;
            role.Description = model.Description;

            Submit();

            return role;
        }

        public void DeleteRole(Role role)
        {
            Context.User2Roles.DeleteAllOnSubmit(role.User2Roles);
            Context.Roles.DeleteOnSubmit(role);

            Submit();
        }

        public IEnumerable<string> GetLoginsAssignedToRole(string roleName)
        {
            return Context.Roles.First(r => r.Name == roleName)
                .User2Roles.Select(ur => ur.User.Login);
        }

        public IEnumerable<Role> GetRoles()
        {
            return Context.Roles;
        }

        public void SavePassword(int userId, byte[] passwordToSave)
        {
            var user = GetUser(userId);
            user.Password = passwordToSave;
            user.Modified = DateTime.UtcNow;
            Submit();
        }

        public bool AddUserToRole(int userId, string roleName)
        {
            var role = Context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
                return false;
            if (role.User2Roles.Any(v => v.UserId == userId))
                return true;
            var user2Role = new User2Role { Role = role, UserId = userId };
            Context.User2Roles.InsertOnSubmit(user2Role);
            Submit();
            return true;
        }

        public bool RemoveUserFromRole(int userId, string roleName)
        {
            var role = Context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
                return false;
            var user2Role = role.User2Roles.FirstOrDefault(v => v.UserId == userId);
            if (user2Role != null)
            {
                Context.User2Roles.DeleteOnSubmit(user2Role);
                Submit();
                return true;
            }
            return false;
        }


        public User UpdateUser<T>(int userId, Expression<Func<User, object>> expression, T value)
        {
            var user = GetUser(userId);
            if (expression.Body is MemberExpression)
            {
                ((PropertyInfo)((MemberExpression)expression.Body).Member).SetValue(user, value, null);
            }
            else
            {
                var memberExpression = ((UnaryExpression) (expression.Body)).Operand as MemberExpression;
                if (memberExpression != null)
                    ((PropertyInfo)(memberExpression.Member)).SetValue(user, value, null);
            }
            return SaveUser(user);
        }



    }
}
