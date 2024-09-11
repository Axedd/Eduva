using Microsoft.EntityFrameworkCore;
using SchoolSystem.Models;

namespace SchoolSystem.Data
{
	public class SchoolSystemDbContext : DbContext
	{
		public SchoolSystemDbContext(DbContextOptions<SchoolSystemDbContext> options) : base(options)
		{
		}

		public DbSet<Student> Students { get; set; }
		public DbSet<SchoolClass> SchoolClasses { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Student>()
				.ToTable("student")
				.HasOne(s => s.SchoolClass)
				.WithMany(sc => sc.Students)
				.HasForeignKey(s => s.ClassId);

			modelBuilder.Entity<SchoolClass>()
				.ToTable("schoolclass")
				.HasKey(sc => sc.ClassId); // Define ClassId as the primary key

			base.OnModelCreating(modelBuilder);
		}
	}
}