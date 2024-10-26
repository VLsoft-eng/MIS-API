using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public DbSet<Icd> Icds { get; set; }
    public DbSet<Patient> Patients { get; set; }
    
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

        modelBuilder.Entity<Token>()
            .HasIndex(token => token.tokenValue)
            .IsUnique();
        
        modelBuilder.Entity<Token>()
            .HasIndex(token => token.id)
            .IsUnique();
        
        modelBuilder.Entity<Token>()
            .HasIndex(token => token.tokenValue)
            .IsUnique();
        
        modelBuilder.Entity<Doctor>().ToTable("doctors");
        modelBuilder.Entity<Speciality>().ToTable("specialities");
        modelBuilder.Entity<Token>().ToTable("banned_tokens");
        modelBuilder.Entity<Icd>().ToTable("ICD_10");
        modelBuilder.Entity<Patient>().ToTable("patient");
    }
}