using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings;
public class TaskAuditTrailMapping : IEntityTypeConfiguration<ProjectTaskAuditTrail>
{
    public void Configure(EntityTypeBuilder<ProjectTaskAuditTrail> builder)
    {
        builder.ToTable("TaskAuditTrails");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.ChangedField).IsRequired();
        builder.Property(t => t.PreviousValue).IsRequired();
        builder.Property(t => t.NewValue).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasOne(t => t.ModifiedByUser)
            .WithMany(u => u.TaskAuditTrails)
            .HasForeignKey(t => t.ModifiedByUserId);

        builder.HasOne(t => t.Task)
            .WithMany(t => t.AuditTrails)
            .HasForeignKey(t => t.TaskId);
    }
}
