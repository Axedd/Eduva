using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthWebApp.Models; // Ensure this matches your namespace for ApplicationUser

namespace AuthWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

		public DbSet<Student> Students { get; set; }
		public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; } // Add this line
        public DbSet<StudentClassSubjects> StudentClassSubjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentClassSubjects>()
                .HasKey(sc => new {sc.StudentClassId, sc.SubjectId});
            
            
            builder.Entity<StudentClassSubjects>()
                .HasOne(sc => sc.StudentClass)
                .WithMany(c => c.StudentClassSubjects)
                .HasForeignKey(sc => sc.StudentClassId);
            
            builder.Entity<StudentClassSubjects>()
                .HasOne(sc => sc.Subject)
                .WithMany(s => s.StudentClassSubjects)
                .HasForeignKey(sc => sc.SubjectId);

            builder.Entity<StudentClass>()
                .HasMany(s => s.StudentClassSubjects)
                .WithOne(s => s.StudentClass)
                .HasForeignKey(s => s.StudentClassId);


            // Configure the relationship between Student and ApplicationUser
            builder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne() // Assuming one-to-one relationship
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed

            // Configure the relationship between ApplicationUser and StudentClass
            builder.Entity<Student>()
                .HasOne(s => s.StudentClass)
                .WithMany(sc => sc.Students)
                .HasForeignKey(s => s.StudentClassId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust as needed

            builder.Entity<StudentClassSubjects>()
           .HasOne(scs => scs.Teacher)
           .WithMany(t => t.StudentClassSubjects)
           .HasForeignKey(scs => scs.TeacherId);
        }

        // You don't need to define DbSet<ApplicationUser> explicitly 
        // because IdentityDbContext already includes it.
        // Define other DbSets for your application's entities here.
    }
}