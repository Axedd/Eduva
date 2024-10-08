﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Identity; // Ensure this matches your namespace for ApplicationUser

namespace SchoolSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Agenda> Agenda { get; set; }
        public DbSet<ScheduleModulePreferences> ScheduleModulePreferences { get; set; }

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

            builder.Entity<IdentityRole>()
                .ToTable("AspNetRoles")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<IdentityRole>("IdentityRole");

            builder.Entity<ApplicationUser>()
            .ToTable("AspNetUsers") // Ensure the table name is correct
            .HasKey(u => u.Id); // Specify that Id is the primary key

            builder.Entity<Agenda>(entity =>
            {
                entity.HasKey(a => a.AgendaId);

                entity.HasOne(a => a.Subject)
                    .WithMany(s => s.Agendas)
                    .HasForeignKey(a => a.SubjectId);


                entity.HasOne(a => a.StudentClass)
                    .WithMany(sc => sc.Agendas)
                    .HasForeignKey(a => a.StudentClassId);

				entity.HasOne(a => a.Teacher)
					.WithMany(sc => sc.Agendas)
					.HasForeignKey(a => a.TeacherId);
			});

            builder.Entity<ScheduleModulePreferences>()
                .HasKey(smp => smp.Id);


                
        }
    }
}