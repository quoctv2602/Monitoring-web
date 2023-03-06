using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monitoring.Model.Entity;

namespace Monitoring.Data.Configurations
{
    public class SysMonitoringConfiguration : IEntityTypeConfiguration<SysMonitoring>
    {
        public void Configure(EntityTypeBuilder<SysMonitoring> builder)
        {
            builder.ToTable("Sys_Monitoring");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255);

        }
    }
}
