using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcBlanket.Security.DomainModels
{
	[Table("User", Schema = "Security")]
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int? GroupId { get; set; }

		public string Login { get; set; }

		public byte[] Password { get; set; }

		public byte[] Salt { get; set; }

		public string PasswordQuestion { get; set; }

		public string PasswordAnswer { get; set; }

		public string FirstName { get; set; }

		public string SecondName { get; set; }

		public string LastName { get; set; }

		public string EMail { get; set; }

		public DateTime RegDate { get; set; }

		public DateTime Modified { get; set; }

		public DateTime? LastLogon { get; set; }

		public DateTime? LastActivity { get; set; }

		public bool IsActive { get; set; }

		public bool IsGroup { get; set; }

		public Guid ActivationKey { get; set; }

		public virtual ICollection<Role> Roles { get; set; }

		public virtual User Group { get; set; }

		public virtual ICollection<User> Parents { get; set; }

		public virtual ICollection<User> Children { get; set; }



	}
}
