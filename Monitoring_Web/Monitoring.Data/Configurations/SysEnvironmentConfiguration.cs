using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monitoring.Model.Entity;

namespace Monitoring.Data.Configurations
{
    public class SysEnvironmentConfiguration : IEntityTypeConfiguration<SysEnvironment>
    {
        public void Configure(EntityTypeBuilder<SysEnvironment> builder)
        {
            builder.ToTable("Sys_Environment");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.Comment).HasMaxLength(500);

        }
    }
}
