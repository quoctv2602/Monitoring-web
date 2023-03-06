using Microsoft.EntityFrameworkCore;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;

namespace Monitoring.Data
{
    public partial class MonitoringContext : DbContext
    {
        public MonitoringContext()
        {
        }

        public MonitoringContext(DbContextOptions<MonitoringContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Sys_EmailServer> Sys_EmailServers { get; set; } = null!;
        public virtual DbSet<Sys_Environment> Sys_Environments { get; set; } = null!;
        public virtual DbSet<Sys_Integration_API> Sys_Integration_APIs { get; set; } = null!;
        public virtual DbSet<Sys_Monitoring> Sys_Monitorings { get; set; } = null!;
        public virtual DbSet<Sys_Node_Setting> Sys_Node_Settings { get; set; } = null!;
        public virtual DbSet<Sys_NodeType> Sys_NodeTypes { get; set; } = null!;
        public virtual DbSet<Sys_Notification> Sys_Notifications { get; set; } = null!;
        public virtual DbSet<Sys_Notification_Detail> Sys_Notification_Details { get; set; } = null!;
        public virtual DbSet<Sys_Threshold_Rule> Sys_Threshold_Rules { get; set; } = null!;
        public virtual DbSet<Trans_Data_Health> Trans_Data_Healths { get; set; } = null!;
        public virtual DbSet<Trans_Message_Log> Trans_Message_Logs { get; set; } = null!;
        public virtual DbSet<Trans_Request_History> Trans_Request_Histories { get; set; } = null!;
        public virtual DbSet<Trans_System_Health> Trans_System_Healths { get; set; } = null!;
        public virtual DbSet<Trans_System_Health_Instance> Trans_System_Health_Instances { get; set; } = null!;
        public virtual DbSet<Trans_System_Health_Storage> Trans_System_Health_Storage { get; set; } = null!;
        public virtual DbSet<TransactionBase_Log> TransactionBase_Log { get; set; }

        public DbSet<DashboardSystemHealthModel> DashboardSystemHealthModels { get; set; } = null!;
        public DbSet<ServiceModel> ServiceModel { get; set; } = null!;
        public DbSet<ListTransactionErrors> ListTransactionErrors { get; set; } = null!;
        public DbSet<DashboardSystemHealth_KPIFreeDiskModel> KPIFreeDiskModels { get; set; } = null!;
        public DbSet<DashboardTransaction_TableModel> DashboardTransaction_TableModels { get; set; } = null!;
        public DbSet<DashboardTransaction_ColumnChartModel> DashboardTransaction_ColumnChartModels { get; set; } = null!;
        public virtual DbSet<Trans_Data_Integration> Trans_Data_Integrations { get; set; } = null!;
        public DbSet<DashboardTrasaction_PendingGraphModel> DashboardTrasaction_PendingGraphModels { get; set; } = null!;
        public DbSet<BaseTransDataIntergrationMappedModel> BaseTransDataIntergrationMapped { get; set; } = null!;
        public DbSet<Sys_Group> Sys_Group { get; set; } = null!;
        public DbSet<Sys_UserProfile> Sys_UserProfile { get; set; } = null!;
        public DbSet<Sys_Action> Sys_Action { get; set; } = null!;
        public DbSet<Sys_UserAction> Sys_UserAction { get; set; } = null!;
        public DbSet<Sys_Pages> Sys_Pages { get; set; } = null!;
        public DbSet<Sys_ErrorStatus> Sys_ErrorStatus { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                // optionsBuilder.UseSqlServer("Server=172.16.0.106;Database=Monitoring;MultipleActiveResultSets=true;User Id=diconnect;Password=diconnect");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sys_EmailServer>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.EnableSSL).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Sys_Integration_API>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Sys_Monitoring>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Sys_Node_Setting>(entity =>
            {
                entity.HasKey(e => e.ID);
            });
            modelBuilder.Entity<Sys_NodeType>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });
            modelBuilder.Entity<Sys_Notification>(entity =>
            {
                entity.HasKey(e => e.ID);
            });
            modelBuilder.Entity<Sys_Notification_Detail>(entity =>
            {
                entity.HasKey(e => e.ID);
            });
            modelBuilder.Entity<Sys_Threshold_Rule>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Trans_Data_Health>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Trans_Message_Log>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Trans_Request_History>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Trans_System_Health>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Trans_System_Health_Instance>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Trans_System_Health_Storage>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Sys_Action>(entity =>
            {
                entity.Property(e => e.ActionId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Sys_Group>(entity =>
            {
                entity.Property(e => e.IsDefault).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Sys_Pages>(entity =>
            {
                entity.Property(e => e.PageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Sys_UserAction>(entity =>
            {
                entity.Property(e => e.GroupId).ValueGeneratedNever();

                entity.Property(e => e.ActionId).ValueGeneratedNever();

                entity.Property(e => e.PageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Sys_UserAction>().HasKey(m => new { m.GroupId, m.ActionId, m.PageId });

            modelBuilder.Entity<Sys_UserProfile>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DashboardSystemHealthModel>().HasNoKey();
            modelBuilder.Entity<ServiceModel>().HasNoKey();
            modelBuilder.Entity<DashboardSystemHealth_KPIFreeDiskModel>().HasNoKey();
            modelBuilder.Entity<ListTransactionErrors>().HasNoKey();

            modelBuilder.Entity<TransactionBase_Log>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });
            modelBuilder.Entity<DashboardTransaction_TableModel>().HasNoKey();
            modelBuilder.Entity<DashboardTransaction_ColumnChartModel>().HasNoKey();
            modelBuilder.Entity<BaseTransDataIntergrationMappedModel>().HasNoKey();
            modelBuilder.Entity<Trans_Data_Integration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
            modelBuilder.Entity<DashboardTrasaction_PendingGraphModel>().HasNoKey();
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
