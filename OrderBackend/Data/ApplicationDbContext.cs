using Microsoft.EntityFrameworkCore;
using OrderBackend.Models;

namespace OrderBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<MedicalOrder> MedicalOrders { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Patient entity
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired();
            
            // 設定與 MedicalOrder 的一對多關聯
            entity.HasMany(e => e.MedicalOrders)
                  .WithOne()
                  .HasForeignKey(o => o.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure MedicalOrder entity
        modelBuilder.Entity<MedicalOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.PatientId).IsRequired();
            
            // 建立索引以提升查詢效能
            entity.HasIndex(e => e.PatientId);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            
            // 建立唯一索引
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
