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
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using MvcBlanket.Security.DataAccess.DataContexts;
using MvcBlanket.Security.Helpers;
using MvcBlanket.Security.Models;

namespace MvcBlanket.Security.Providers
{
	public class AccountMembershipProvider : MembershipProvider
	{
        const int MinRequiredNonAlphanumericCharactersConst = 1;
        const int MinRequiredPasswordLengthConst = 6;
        const int MaxInvalidPasswordAttemptsConst = 5;
        const int PasswordAttemptWindowConst = 10;

        private string applicationName;
        private bool requiresQuestionAndAnswer;
        private int maxInvalidPasswordAttempts;
        private int passwordAttemptWindow;
        private int minRequiredNonAlphanumericCharacters;
        private int minRequiredPasswordLength;
        private string passwordStrengthRegularExpression;

        public override string ApplicationName { get { return applicationName; } set { applicationName = value; } }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ProviderException("Configuration section not found");

            base.Initialize(name, config);

            applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], MaxInvalidPasswordAttemptsConst.ToString(CultureInfo.InvariantCulture)));
            passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], PasswordAttemptWindowConst.ToString(CultureInfo.InvariantCulture)));
            minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonalphanumericCharacters"], MinRequiredNonAlphanumericCharactersConst.ToString(CultureInfo.InvariantCulture)));
            minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], MinRequiredPasswordLengthConst.ToString(CultureInfo.InvariantCulture)));
            passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], String.Empty));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));

        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			var user = (AccountMembershipUser)GetUser(username, false);
			return user != null && user.ChangePassword(oldPassword, newPassword);
		}

        public bool ChangePassword(string username, string newPassword)
        {
            var user = (AccountMembershipUser)GetUser(username, false);
            return user != null && user.ChangePassword(newPassword);
        }

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
			string newPasswordAnswer)
		{
			var user = (AccountMembershipUser)GetUser(username, false);
			return user != null && user.ChangePasswordQuestionAndAnswer(password, newPasswordQuestion, newPasswordAnswer);
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
			string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{

			if (string.IsNullOrEmpty(username))
			{
				status = MembershipCreateStatus.InvalidUserName;
				return null;
			}

			if (string.IsNullOrEmpty(email))
			{
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			if (!string.IsNullOrEmpty(password) && password.Length < MinRequiredPasswordLength)
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

            var repository = new SecurityRepository();

			if (repository.TestUserByLogin(username))
			{
				status = MembershipCreateStatus.DuplicateUserName;
				return null;
			}

			if (repository.TestUserByEmail(email))
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			var salt = SaltGenerator.GenerateSalt();

			var account = new User
			{
				Login = username,
				EMail = email,
				Salt = salt,
				FirstName = username,
				LastName = username,
				PasswordQuestion = passwordQuestion,
				PasswordAnswer = passwordAnswer
			};
            
            repository.SaveUser(account);

			var user = (AccountMembershipUser)GetUser(username, false);
            if (user != null)
            {
                user.SavePassword(password);
                status = MembershipCreateStatus.Success;
            }
            else
                status = MembershipCreateStatus.ProviderError;
			return user;
		}

		

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

		public override bool EnablePasswordReset
		{
			get { return true; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return false; }
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			return FindUsersByName(GetUserNameByEmail(emailToMatch), pageIndex, pageSize, out totalRecords);
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			AccountMembershipUser user = AccountMembershipUser.GetUser(usernameToMatch);
			var coll = new MembershipUserCollection();
			if (user != null)
				coll.Add(user);
			totalRecords = coll.Count;
			return coll;
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
            var result = new SecurityRepository().GetUsers(pageIndex, pageSize);            
            var users = result.Select(u => new AccountMembershipUser
                    (u.Id, u.Login, u.Salt, u.FirstName, u.SecondName, u.LastName, u.EMail, u.RegDate, u.IsActive));
            totalRecords = result.TotalEntries;
			var collection = new MembershipUserCollection();
			foreach (var user in users)
				collection.Add(user);
			return collection;
		}

		public override int GetNumberOfUsersOnline()
		{
            return new SecurityRepository().GetNumberOfUsersOnline(Membership.UserIsOnlineTimeWindow);
		}

		public override string GetPassword(string username, string answer)
		{
		    var user = (AccountMembershipUser)GetUser(username, false);
		    return user != null ? user.ResetPassword(answer) : string.Empty;
		}

	    public override MembershipUser GetUser(string username, bool userIsOnline)
		{
            var user = new SecurityRepository().GetAccount(username);
			if (userIsOnline)
				user.UpdateLastActivity();
			return user;
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
            var user = new SecurityRepository().GetAccount((int)providerUserKey);
			if (userIsOnline)
				user.UpdateLastActivity();
			return user;

		}

		public override string GetUserNameByEmail(string email)
		{
            return new SecurityRepository().GetUserByEmail(email, v => v.EMail);
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return Math.Max(MaxInvalidPasswordAttemptsConst, maxInvalidPasswordAttempts); }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
            get { return Math.Max(MinRequiredNonAlphanumericCharactersConst, minRequiredNonAlphanumericCharacters); }
		}

		public override int MinRequiredPasswordLength
		{
            get { return Math.Max(MinRequiredPasswordLengthConst, minRequiredPasswordLength) ; }
		}

		public override int PasswordAttemptWindow
		{
            get { return Math.Max(PasswordAttemptWindowConst, passwordAttemptWindow); }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { return MembershipPasswordFormat.Hashed; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return passwordStrengthRegularExpression; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return requiresQuestionAndAnswer; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return true; }
		}

		public override string ResetPassword(string username, string answer)
		{
			if (!EnablePasswordReset)
				throw new NotSupportedException();
			var user = AccountMembershipUser.GetUser(username);
			if (user == null)
				throw new ProviderException("Specified user not found");
			return RequiresQuestionAndAnswer ? user.ResetPassword(answer) : user.ResetPassword();
		}


		public override bool UnlockUser(string username)
		{
			var user = AccountMembershipUser.GetUser(username);
			if (user == null)
				throw new ProviderException("Specified user not found");

			return user.UnlockUser();

		}

		public override void UpdateUser(MembershipUser user)
		{
			var account = user as AccountMembershipUser;
			if (account == null) return;
            new SecurityRepository().SaveUser(account);
		}

		public override bool ValidateUser(string username, string password)
		{
			var user = ((AccountMembershipUser)GetUser(username, false));
			if (user != null && user.ValidatePassword(password) && !user.IsLockedOut)
			{
				user.UpdateLastLogon();
				return true;
			}
			return false;
		}

		static public byte[] HashPassword(string login, string password, byte[] salt)
		{
            password = login + password;
            var sha = new SHA512Managed();
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] buffer = new byte[bytes.Length + salt.Length];
            Array.Copy(bytes, buffer, bytes.Length);
            Array.Copy(salt, 0, buffer, bytes.Length, salt.Length);
            var hashed = sha.ComputeHash(buffer);
            sha.Clear();
            return hashed;
		}

		internal void SavePassword(string login, string password)
		{
			var user = (AccountMembershipUser)GetUser(login, false);
			if (user != null)
				user.SavePassword(password);
		}

		internal void SavePassword(int id, string password)
		{
			var user = (AccountMembershipUser)GetUser(id, false);
			if (user != null)
				user.SavePassword(password);
		}

		public static AccountMembershipUser GetInteractiveUser()
		{
			string user = HttpContext.Current.User.Identity.Name;
			var accountUser = (AccountMembershipUser)Membership.GetUser(user, true);
			return accountUser;
		}

        private static string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }



	}

}
