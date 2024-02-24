using Domain.Entities;
using Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<ProjectTaskAuditTrail> TaskAuditTrails { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified && e.Entity.GetType() == typeof(ProjectTask))
            .ToList();

        foreach (var entry in modifiedEntries)
        {
            var entity = entry.Entity as ProjectTask;
            if (entity != null)
            {                
                foreach (var prop in entry.OriginalValues.Properties)
                {
                    var originalValue = entry.OriginalValues[prop];
                    var currentValue = entry.CurrentValues[prop];                    

                    if (!object.Equals(originalValue, currentValue))
                    {
                        var auditTrail = new ProjectTaskAuditTrail
                        {
                            ChangedField = prop.Name,
                            PreviousValue = originalValue?.ToString(),
                            NewValue = currentValue?.ToString(),
                            CreatedAt = DateTime.Now, 
                            TaskId = entity.Id, 
                            ModifiedByUserId = entity.ModifiedByUserId ?? int.MinValue
                        };                        

                        TaskAuditTrails.Add(auditTrail);
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProjectMapping());
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new TaskMapping());
        modelBuilder.ApplyConfiguration(new TaskAuditTrailMapping());
    }
}