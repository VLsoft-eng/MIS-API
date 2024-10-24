using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>()
            .HasIndex(doctor => doctor.email)
            .IsUnique();

        modelBuilder.Entity<Doctor>()
            .HasIndex(doctor => doctor.phone)
            .IsUnique();
        
        modelBuilder.Entity<Speciality>()
            .Property(s => s.id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Doctor>()
            .Property(s => s.id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Doctor>().ToTable("doctors");
        modelBuilder.Entity<Speciality>().ToTable("specialities");
    }
    

}