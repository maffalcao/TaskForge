using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Mappings;
public class TaskMapping : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.DueDate);
        builder.Property(t => t.Comment);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.Priority).IsRequired();
        builder.Property(t => t.DeletedAt);
        builder.Property(t => t.ModifiedByUserId);

        builder.HasOne(t => t.AssignedUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedUserId);
                
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .IsRequired();

        //builder.HasMany(t => t.AuditTrails)
        //    .WithOne()
        //    .HasForeignKey(t => t.TaskId);

    }
}
