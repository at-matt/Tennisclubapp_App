using Microsoft.EntityFrameworkCore;
using TennisCoach.Migrations;
using TennisCoach.Models;

namespace TennisCoach.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }




        // Add DbSets for your entities (tables)
        public DbSet<Coaches> Coaches { get; set; }
        public DbSet<Schedules> Schedules { get; set; }
        public DbSet<Members> Members { get; set; }
        public DbSet<Enrollments> Enrollments { get; set; }
        public DbSet<Admins> Admins { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<Coaches>()
                .HasKey(c => c.CoachId);

            modelBuilder.Entity<Enrollments>()
        .HasKey(e => e.EnrollmentId);

            modelBuilder.Entity<Enrollments>()
                .HasOne(e => e.Schedule)
                .WithMany(s => s.Enrollments) 
                .HasForeignKey(e => e.ScheduleId);

            modelBuilder.Entity<Enrollments>()
                .HasOne(e => e.Member)
                .WithMany(m => m.Enrollments) 
                .HasForeignKey(e => e.MemberId);

            // Configure entity tables
            modelBuilder.Entity<Coaches>().ToTable("Coaches");
            modelBuilder.Entity<Schedules>().ToTable("Schedules");
            modelBuilder.Entity<Members>().ToTable("Members");
            modelBuilder.Entity<Enrollments>().ToTable("Enrollments");
            modelBuilder.Entity<Admins>().ToTable("Admins");

        }
    }
}
