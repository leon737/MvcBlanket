using System;
using System.Collections.Generic;
using System.Linq;
using MvcBlanket.Security.DataAccess;
using MvcBlanket.Security.DomainModels;
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
					Modified = DateTime.UtcNow,
					ActivationKey = Guid.NewGuid()
				};
				Context.Users.Add(user);
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

		public T GetUser<T>(string login, Func<User, T> selector)
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


		public AccountMembershipUser GetAccount(string login)
		{
			var user = Context.Individuals.Where(u => u.Login == login).FirstOrDefault();
			if (user == null) return null;
			return new AccountMembershipUser(user.Id, user.Login, user.Salt, user.FirstName, user.SecondName,
			                                 user.LastName, user.EMail, user.RegDate, user.IsActive);
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
			var user = Context.Individuals.Where(u => u.Id == id).FirstOrDefault();
			if (user == null) return null;
			return new AccountMembershipUser
				(user.Id, user.Login, user.Salt, user.FirstName, user.SecondName,
				 user.LastName, user.EMail, user.RegDate, user.IsActive);
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
				Context.Roles.Add(role);
			}

			role.Name = model.Name;
			role.Description = model.Description;

			Submit();

			return role;
		}

		public void DeleteRole(Role role)
		{
			Context.Roles.Remove(role);
			Submit();
		}

		public IEnumerable<string> GetLoginsAssignedToRole(string roleName)
		{
			return Context.Roles.Where(r => r.Name == roleName).First().Users.Select(u => u.Login);
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
			if (role.Users.Any(v => v.Id == userId))
				return true;
			var user = Context.Users.FirstOrDefault(u => u.Id == userId);
			if (user != null)
				role.Users.Add(user);
			else
				return false;
			Submit();
			return true;
		}

		public bool RemoveUserFromRole(int userId, string roleName)
		{
			var role = Context.Roles.FirstOrDefault(r => r.Name == roleName);
			if (role == null)
				return false;
			var user = Context.Users.FirstOrDefault(u => u.Id == userId);
			if (user != null)
				role.Users.Remove(user);
			else
				return false;
			Submit();
			return true;
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
