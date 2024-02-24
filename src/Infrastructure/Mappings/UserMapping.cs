using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasMany(u => u.Projects)
            .WithOne(p => p.CreatedByUser)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AssignedTasks)
            .WithOne(t => t.AssignedUser)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.TaskAuditTrails)
            .WithOne(t => t.ModifiedByUser)
            .HasForeignKey(t => t.ModifiedByUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
