using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcBlanket.Security.DomainModels
{
	[Table("Role", Schema = "Security")]
	public class Role
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public virtual ICollection<User> Users { get; set; }
	}
}
