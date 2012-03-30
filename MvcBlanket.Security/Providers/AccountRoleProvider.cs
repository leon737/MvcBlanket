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
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using MvcBlanket.Security.DataAccess.DataContexts;
using MvcBlanket.Security.Models;

namespace MvcBlanket.Security.Providers
{
	public class AccountRoleProvider : RoleProvider
	{		


		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			AccountMembershipProvider membershipProvider = (AccountMembershipProvider)Membership.Provider;

			foreach (var username in usernames)
			{
				if (username == null)
					throw new ArgumentNullException();
				if (username.Length == 0)
					throw new ArgumentException();
				AccountMembershipUser user = (AccountMembershipUser)membershipProvider.GetUser(username, false);
				if (user == null)
					throw new ProviderException("User not found");

				foreach (var roleName in roleNames)
				{
					if (roleName == null)
						throw new ArgumentNullException();
					if (roleName.Length == 0)
						throw new ArgumentException();
					if (!user.AddToRole(roleName))
						throw new ProviderException("Role not found");
				}
			}
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			AccountMembershipProvider membershipProvider = (AccountMembershipProvider)Membership.Provider;

			foreach (var username in usernames)
			{
				if (username == null)
					throw new ArgumentNullException();
				if (username.Length == 0)
					throw new ArgumentException();
				AccountMembershipUser user = (AccountMembershipUser)membershipProvider.GetUser(username, false);
				if (user == null)
					throw new ProviderException("User not found");

				foreach (var roleName in roleNames)
				{
					if (roleName == null)
						throw new ArgumentNullException();
					if (roleName.Length == 0)
						throw new ArgumentException();
					if (!user.RemoveFromRole(roleName))
						throw new ProviderException("Role not found");
				}
			}
		}

		string applicationName;
		public override string ApplicationName
		{
			get { return applicationName; }
			set { applicationName = value; }
		}

		public override void CreateRole(string roleName)
		{
            var repository = new SecurityRepository();
            var role = repository.GetRole(roleName);
			if (role != null)
				throw new ProviderException("Specified role already exists");
			role = new Role { Name = roleName };
            repository.SaveRole(role);
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
            var repository = new SecurityRepository();
            var role = repository.GetRole(roleName);
			if (role == null)
				throw new ProviderException("Specified role not found");
			if (!throwOnPopulatedRole)
			{
                if (role.User2Roles.Count() > 0)
					throw new ProviderException("Role is populated");
			}
            repository.DeleteRole(role);
			return true;
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			if (!RoleExists(roleName))
				throw new ProviderException("Specified role not found");

            var logins = new SecurityRepository().GetLoginsAssignedToRole(roleName);

			if (!string.IsNullOrEmpty(usernameToMatch))
				logins = logins.Where(v => v.Contains(usernameToMatch));
			
			return logins.OrderBy(v => v).ToArray();
		}

		public override string[] GetAllRoles()
		{
            return new SecurityRepository().GetRoles().Select(r => r.Name).ToArray();
		}

		public override string[] GetRolesForUser(string username)
		{
			if (username == null)
				throw new ArgumentNullException();
			if (username.Length == 0)
				throw new ArgumentException();
			AccountMembershipProvider membershipProvider = (AccountMembershipProvider)Membership.Provider;
			AccountMembershipUser user = (AccountMembershipUser)membershipProvider.GetUser(username, false);
			return user.Roles;
		}

		public override string[] GetUsersInRole(string roleName)
		{
			return FindUsersInRole(roleName, null);
		}

		public override bool IsUserInRole(string username, string roleName)
		{
			return GetRolesForUser(username).Contains(roleName);
		}

		public override bool RoleExists(string roleName)
		{
			return GetAllRoles().Contains(roleName);
		}
	}

}
