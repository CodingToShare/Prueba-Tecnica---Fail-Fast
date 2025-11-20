using Microsoft.EntityFrameworkCore;
using Erp.Documents.Domain.Entities;

namespace Erp.Documents.Infrastructure.Data
{
    /// <summary>
    /// DbContext para la solución de gestión de documentos.
    /// </summary>
    public class ErpDocumentsDbContext : DbContext
    {
        public ErpDocumentsDbContext(DbContextOptions<ErpDocumentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentValidationFlow> DocumentValidationFlows { get; set; }
        public DbSet<ValidationStep> ValidationSteps { get; set; }
        public DbSet<ValidationAction> ValidationActions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.HasMany(e => e.Documents)
                    .WithOne(d => d.Company)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Document
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(512);
                entity.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BucketKey).IsRequired().HasMaxLength(1024);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityId).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedByUserId).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Hash).HasMaxLength(256);

                entity.HasIndex(e => e.CompanyId);
                entity.HasIndex(e => new { e.CompanyId, e.EntityType, e.EntityId });
                entity.HasIndex(e => e.BucketKey).IsUnique();

                entity.HasOne(e => e.ValidationFlow)
                    .WithOne(vf => vf.Document)
                    .HasForeignKey<Document>(e => e.ValidationFlowId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración de DocumentValidationFlow
            modelBuilder.Entity<DocumentValidationFlow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.Steps)
                    .WithOne(vs => vs.Flow)
                    .HasForeignKey(vs => vs.FlowId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Actions)
                    .WithOne(va => va.Flow)
                    .HasForeignKey(va => va.FlowId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de ValidationStep
            modelBuilder.Entity<ValidationStep>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ApproverUserId).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.FlowId);
            });

            // Configuración de ValidationAction
            modelBuilder.Entity<ValidationAction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ActorUserId).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Reason).HasMaxLength(1024);
                entity.HasIndex(e => e.FlowId);
            });
        }
    }
}
