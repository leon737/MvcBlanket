using System;
using System.Configuration.Provider;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Security.DataAccess.DataContexts;
using Security.Models;

namespace Security.Providers
{
	public class AccountMembershipProvider : MembershipProvider
	{
        const int MinRequiredNonAlphanumericCharactersConst = 1;
        const int MinRequiredPasswordLengthConst = 6;

		string applicationName;
		public override string ApplicationName
		{
			get { return applicationName; }
			set { applicationName = value; }
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			AccountMembershipUser user = (AccountMembershipUser)GetUser(username, false);
			if (user != null)
				return user.ChangePassword(oldPassword, newPassword);
			else
				return false;
		}

        public bool ChangePassword(string username, string newPassword)
        {
            AccountMembershipUser user = (AccountMembershipUser)GetUser(username, false);
            if (user != null)
                return user.ChangePassword(newPassword);
            else
                return false;
        }

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
			string newPasswordAnswer)
		{
			AccountMembershipUser user = (AccountMembershipUser)GetUser(username, false);
			if (user != null)
				return user.ChangePasswordQuestionAndAnswer(password, newPasswordQuestion, newPasswordAnswer);
			else
				return false;
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

			string salt = SaltGenerator.GenerateSalt();

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
            account = repository.SaveUser(account);

			var user = (AccountMembershipUser)GetUser(username, false);
			if (user != null)
				status = MembershipCreateStatus.Success;
			else
				status = MembershipCreateStatus.ProviderError;
			user.SavePassword(password);
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
			AccountMembershipUser user = AccountMembershipUser.GetUser(this.GetUserNameByEmail(emailToMatch));
			MembershipUserCollection coll = new MembershipUserCollection();
			if (user != null)
				coll.Add(user);
			totalRecords = coll.Count;
			return coll;
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			AccountMembershipUser user = AccountMembershipUser.GetUser(usernameToMatch);
			MembershipUserCollection coll = new MembershipUserCollection();
			if (user != null)
				coll.Add(user);
			totalRecords = coll.Count;
			return coll;
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
            var result = new SecurityRepository().GetUsers(pageIndex, pageSize);            
            var users = result.Select(u => new AccountMembershipUser
                    (u.Id, u.Login, u.Salt, u.FirstName, u.SecondName,
					u.LastName, u.EMail, u.RegDate, u.IsActive));
            totalRecords = result.TotalEntries;
			MembershipUserCollection collection = new MembershipUserCollection();
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
			AccountMembershipUser user = (AccountMembershipUser)GetUser(username, false);
			if (user != null)
				return user.ResetPassword(answer);
			else
				return string.Empty;
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
			get { throw new NotImplementedException(); }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
            get { return MinRequiredNonAlphanumericCharactersConst; }
		}

		public override int MinRequiredPasswordLength
		{
            get { return MinRequiredPasswordLengthConst; }
		}

		public override int PasswordAttemptWindow
		{
			get { throw new NotImplementedException(); }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { return MembershipPasswordFormat.Hashed; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { throw new NotImplementedException(); }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return true; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return true; }
		}

		public override string ResetPassword(string username, string answer)
		{
			if (!EnablePasswordReset)
				throw new NotSupportedException();
			AccountMembershipUser user = AccountMembershipUser.GetUser(username);
			if (user == null)
				throw new ProviderException("Specified user not found");
			if (RequiresQuestionAndAnswer)
				return user.ResetPassword(answer);
			else
				return user.ResetPassword();
		}


		public override bool UnlockUser(string username)
		{
			AccountMembershipUser user = AccountMembershipUser.GetUser(username);
			if (user == null)
				throw new ProviderException("Specified user not found");

			return user.UnlockUser();

		}

		public override void UpdateUser(MembershipUser user)
		{
			AccountMembershipUser account = user as AccountMembershipUser;
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

		static public byte[] HashPassword(string login, string password, string salt)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			return md5.ComputeHash(Encoding.Unicode.GetBytes(login + password + salt));
		}

		public void SavePassword(string login, string password)
		{
			AccountMembershipUser user = (AccountMembershipUser)GetUser(login, false);
			if (user != null)
				user.SavePassword(password);
		}

		public void SavePassword(int Id, string password)
		{
			AccountMembershipUser user = (AccountMembershipUser)GetUser((object)Id, false);
			if (user != null)
				user.SavePassword(password);
		}

		public static AccountMembershipUser GetInteractiveUser()
		{
			string user = HttpContext.Current.User.Identity.Name;
			AccountMembershipUser accountUser = (AccountMembershipUser)Membership.GetUser(user, true);
			return accountUser;
		}


	}

}
