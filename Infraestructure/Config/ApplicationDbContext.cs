using Microsoft.EntityFrameworkCore;
using PetSafe.Domain.Models;

namespace PetSafe.Config;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Vaccine> Vaccines { get; set; } = null!;
    public DbSet<WeightRecord> WeightRecords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique(); // e-mail único
            entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
        });

        // Pet
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Species).HasMaxLength(50).IsRequired();

            entity
                .HasOne(p => p.Owner)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Vaccine
        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Name).HasMaxLength(100).IsRequired();

            entity
                .HasOne(v => v.Pet)
                .WithMany(p => p.Vaccines)
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WeightRecord
        modelBuilder.Entity<WeightRecord>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity
                .HasOne(w => w.Pet)
                .WithMany(p => p.Weights)
                .HasForeignKey(w => w.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}