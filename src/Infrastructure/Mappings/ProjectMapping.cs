using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;

public class ProjectMapping : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects"); 
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();
       
        builder.HasOne(p => p.CreatedByUser)             
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.CreatedByUserId)
            .IsRequired();
    }
}
