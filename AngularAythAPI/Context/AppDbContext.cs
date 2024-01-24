using AngularAythAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Labor> Labors { get; set; }
        public DbSet<Commission> Commissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Theme>().ToTable("Themes");
            modelBuilder.Entity<Commission>().ToTable("Commisions");
            modelBuilder.Entity<Labor>().ToTable("Labors");

            modelBuilder.Entity<Labor>()
                .HasOne(l => l.Profesor)
                .WithMany()
                .HasForeignKey(l => l.ProfesorId);

            modelBuilder.Entity<Labor>()
                .HasOne(l => l.Student)
                .WithMany()
                .HasForeignKey(l => l.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Theme>()
               .HasOne(t => t.Profesor)
               .WithMany()
               .HasForeignKey(t => t.ProfesorId);
            modelBuilder.Entity<Theme>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<User>()
                .HasOne(u => u.Profesor)
                .WithMany()
                .HasForeignKey(u => u.ProfesorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.StudentThemes)
                .WithMany()
                .HasForeignKey(u => u.StudentThemesId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.StudentsLabor)
                .WithMany()
                .HasForeignKey(u => u.StudentsLaborId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Commission>()
                .HasMany(c => c.CommissionMembers)
                .WithMany(u => u.Commissions)
                .UsingEntity(j => j.ToTable("CommissionMembers"));
        }

    }
}
