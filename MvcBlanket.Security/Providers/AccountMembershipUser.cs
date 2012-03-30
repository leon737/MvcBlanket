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
using System.Linq;
using System.Web.Security;
using MvcBlanket.Security.DataAccess.DataContexts;
using MvcBlanket.Security.Helpers;
using MvcBlanket.Security.Models;
using System.Collections.Generic;

namespace MvcBlanket.Security.Providers
{
    public class AccountMembershipUser : MembershipUser
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string LastName { get; private set; }
        public byte[] Salt { get; private set; }

        readonly object syncLock = new object();
        User storedEntity;

        protected User StoredEntity
        {
            get
            {
                lock (syncLock)
                    if (storedEntity == null)
                        lock (syncLock)
                            storedEntity = new SecurityRepository().GetUser(Id);
                return storedEntity;
            }
        }

        internal AccountMembershipUser(int id, string login, byte[] salt, string firstName, string secondName, string lastName,
            string email, DateTime regDate, bool isActive)
            : base("AccountMembershipProvider",
            login, id, email, null, null, true, !isActive, regDate, DateTime.MinValue, DateTime.MinValue,
            DateTime.MinValue, DateTime.MinValue)
        {
            Id = id;
            Salt = salt;
            FirstName = firstName;
            SecondName = secondName;
            LastName = lastName;
        }

        private AccountMembershipUser()
        {

        }

        internal static AccountMembershipUser GetUserInternal(int id)
        {
            return new SecurityRepository().GetUser(id, u => new AccountMembershipUser()
            {
                Id = u.Id
            });
        }

        internal static AccountMembershipUser GetUser(int id)
        {
            return new SecurityRepository().GetUser(id, u =>
                new AccountMembershipUser(u.Id, u.Login, u.Salt, u.FirstName, u.SecondName,
                    u.LastName, u.EMail, u.RegDate, u.IsActive));
        }

        internal static AccountMembershipUser GetUser(string login)
        {
            return new SecurityRepository().GetUser(login, u =>
                new AccountMembershipUser(u.Id, u.Login, u.Salt, u.FirstName, u.SecondName,
                    u.LastName, u.EMail, u.RegDate, u.IsActive));
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            if (!ValidatePassword(oldPassword)) return false;
            SavePassword(newPassword);
            return true;
        }

        public bool ChangePassword(string newPassword)
        {
            SavePassword(newPassword);
            return true;
        }

        byte[] HashPassword(string password)
        {
            return AccountMembershipProvider.HashPassword(UserName, password, Salt);
        }

        internal bool ValidatePassword(string password)
        {
            byte[] passwordFromDb = GetPassword();
            if (passwordFromDb == null || passwordFromDb.Length == 0)
                return true;
            byte[] validatedPassword = HashPassword(password);
            return passwordFromDb.SequenceEqual(validatedPassword);
        }

        new byte[] GetPassword()
        {
            var password = new SecurityRepository().GetUser(Id, u => u.Password);
            if (password == null || password.Length != 64) return new byte[0];
            return password.ToArray();
        }

        internal void SavePassword(string passwordToSave)
        {
            byte[] password = HashPassword(passwordToSave);
            SavePassword(password);
        }

        internal void SavePassword(byte[] passwordToSave)
        {
            new SecurityRepository().SavePassword(Id, passwordToSave);
        }

        public override string ResetPassword()
        {
            string newPassword = new PasswordGenerator().Generate();
            SavePassword(newPassword);
            return newPassword;
        }

        public override bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            if (!ValidatePassword(password))
                return false;

            var repository = new SecurityRepository();
            var user = repository.GetUser(Id);
            if (user != null)
            {
                user.PasswordQuestion = newPasswordQuestion;
                user.PasswordAnswer = newPasswordAnswer;
                repository.SaveUser(user);
            }
            return true;
        }

        public override DateTime CreationDate
        {
            get
            {
                return StoredEntity.RegDate;
            }
        }

        public override string Email
        {
            get
            {
                return StoredEntity.EMail;
            }
            set
            {
                new SecurityRepository().UpdateUser(Id, u => u.EMail, value);
            }
        }

        public override bool IsApproved
        {
            get
            {
                return IsLockedOut;
            }
            set
            {
                new SecurityRepository().UpdateUser(Id, u => u.IsActive, value);
            }
        }

        public override bool IsLockedOut
        {
            get
            {
                var user = StoredEntity;
                return !(user.IsActive && (user.Group == null || user.Group.IsActive));
            }
        }

        public override DateTime LastActivityDate
        {
            get
            {
                return StoredEntity.LastActivity.GetValueOrDefault();
            }
            set
            {
                new SecurityRepository().UpdateUser(Id, u => u.LastActivity, value);
            }
        }

        public override DateTime LastLoginDate
        {
            get
            {
                return StoredEntity.LastLogon.GetValueOrDefault();
            }
            set
            {
                new SecurityRepository().UpdateUser(Id, u => u.LastLogon, value);
            }
        }

        public override string PasswordQuestion
        {
            get
            {
                return StoredEntity.PasswordQuestion;
            }
        }

        public override object ProviderUserKey
        {
            get
            {
                return Id;
            }
        }

        public override string GetPassword(string passwordAnswer)
        {
            throw new NotSupportedException();
        }

        public override string ResetPassword(string passwordAnswer)
        {
            if (StoredEntity.PasswordAnswer == passwordAnswer)
                return ResetPassword();
            else
                throw new MembershipPasswordException();
        }

        public bool AddToRole(string roleName)
        {
            return new SecurityRepository().AddUserToRole(Id, roleName);
        }

        public bool RemoveFromRole(string roleName)
        {
            return new SecurityRepository().RemoveUserFromRole(Id, roleName);
        }

        public string[] Roles
        {
            get
            {
                return GetRoles(StoredEntity,
                    new List<RoleInformation>(), new List<int>())
                    .Where(r => r.Access).Select(r => r.RoleName).ToArray();
            }
        }

        private IEnumerable<User> GetGroups(User user, IList<int> visitedUsers)
        {
            return user.ParentUsers.Where(v => !visitedUsers.Contains(v.ParentUserId))
                .Select(v => v.ParentUser);
        }

        private IList<RoleInformation> GetRoles(User user, IList<RoleInformation> roles,
            IList<int> visitedUsers)
        {
            var localRoles = user.User2Roles.Select(v => new RoleInformation { RoleName = v.Role.Name, Access = v.Access });
            foreach (var localRole in localRoles)
            {
                var childRole = roles.FirstOrDefault(v => v.RoleName == localRole.RoleName);
                if (childRole == null)
                {
                    roles.Add(localRole);
                    continue;
                }
                if (!childRole.Access)
                    continue;
                childRole.Access = localRole.Access;
            }
            foreach (var parentUser in GetGroups(user, visitedUsers))
            {
                visitedUsers.Add(parentUser.Id);
                roles = GetRoles(parentUser, roles, visitedUsers);
            }
            return roles;
        }

        public void UpdateLastActivity()
        {
            new SecurityRepository().UpdateUser(Id, u => u.LastActivity, DateTime.UtcNow);
        }

        public void UpdateLastLogon()
        {
            new SecurityRepository().UpdateUser(Id, u => u.LastLogon, DateTime.UtcNow);
        }

        public override bool UnlockUser()
        {
            new SecurityRepository().UpdateUser(Id, u => u.IsActive, true);
            return true;
        }

        public Guid ActivationKey
        {
            get
            {
                return StoredEntity.ActivationKey;
            }
        }

        public bool ChangeNameAndEMail(string password, string email, string firstName, string secondName, string lastName)
        {
            if (!ValidatePassword(password))
                return false;

            var repository = new SecurityRepository();
            var user = repository.GetUser(Id);
            if (user != null)
            {
                user.EMail = email;
                user.FirstName = firstName;
                user.SecondName = secondName;
                user.LastName = lastName;
                repository.SaveUser(user);
            }
            return true;
        }

    }

}
