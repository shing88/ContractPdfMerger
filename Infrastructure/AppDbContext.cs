using ContractPdfMerger.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContractPdfMerger.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<SupplementalDocument> SupplementalDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentTypes", "dbo");
            entity.HasKey(e => e.TypeCode);
            entity.Property(e => e.TypeCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TypeName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<SupplementalDocument>(entity =>
        {
            entity.ToTable("SupplementalDocuments", "dbo", t => t.HasCheckConstraint("CK_FileContent_Size", "DATALENGTH(FileContent) <= 1048576"));
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).UseIdentityColumn();
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.TypeCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(d => d.DocumentType)
                  .WithMany(p => p.SupplementalDocuments)
                  .HasForeignKey(d => d.TypeCode)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}