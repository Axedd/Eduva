using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthWebApp.Models;
using SchoolSystem.Models; // Ensure this matches your namespace for ApplicationUser

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
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<SubjectTeachers> SubjectTeachers { get; set; }
        public DbSet<StudentClassSubjects> StudentClassSubjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<UsernameCount> UsernameCounts { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // StudentClassSubjects relationships
            builder.Entity<StudentClassSubjects>()
                .HasKey(sc => new { sc.StudentClassId, sc.SubjectId });

            builder.Entity<StudentClassSubjects>()
                .HasOne(sc => sc.StudentClass)
                .WithMany(c => c.StudentClassSubjects)
                .HasForeignKey(sc => sc.StudentClassId);

            builder.Entity<StudentClassSubjects>()
                .HasOne(sc => sc.Subject)
                .WithMany(s => s.StudentClassSubjects)
                .HasForeignKey(sc => sc.SubjectId);

            // Student relationships
            builder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne() // Assuming one-to-one relationship
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Student>()
                .HasOne(s => s.StudentClass)
                .WithMany(sc => sc.Students)
                .HasForeignKey(s => s.StudentClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Teacher relationships
            builder.Entity<SubjectTeachers>()
                .HasKey(st => new { st.SubjectId, st.TeacherId });

            builder.Entity<SubjectTeachers>()
                .HasOne(st => st.Subject)
                .WithMany(s => s.SubjectTeachers)
                .HasForeignKey(st => st.SubjectId);

            builder.Entity<SubjectTeachers>()
                .HasOne(st => st.Teacher)
                .WithMany(t => t.SubjectTeachers)
                .HasForeignKey(st => st.TeacherId);
        }
    }
}