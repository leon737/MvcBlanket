using System.Data.Entity;
using System.Linq;
using MvcBlanket.Security.DomainModels;

namespace MvcBlanket.Security.DataAccess
{
	public class SecurityDataContext : DbContext
	{
		public SecurityDataContext() : base ("Security") {}

		public DbSet<User> Users { get; set; }

		public DbSet<Role> Roles { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<User>().HasMany(u => u.Roles).WithMany(r => r.Users)
				.Map(m =>
				{
					m.MapLeftKey("UserId");
					m.MapRightKey("RoleId");
					m.ToTable("User2Role", "Security");
				});

			modelBuilder.Entity<User>().HasMany(u => u.Parents).WithMany(r => r.Children)
				.Map(m =>
				{
					m.MapLeftKey("UserId");
					m.MapRightKey("ParentId");
					m.ToTable("User2ParentUser", "Security");
				});
		}

		public IQueryable<User> Individuals
		{
			get
			{
				return Users.Where(u => !u.IsGroup);
			}
		}
	}
}
