using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Node_Setting")]
    public partial class Sys_Node_Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(64)]
        public string? NodeName { get; set; }
        public int EnvironmentID { get; set; }
        [StringLength(500)]
        public string? MachineName { get; set; } 
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(4000)]
        public string? ServiceList { get; set; }
        public string? NotificationEmail { get; set; }
        public string? ReportEmail { get; set; }
        public string? NotificationAlias { get; set; }
        public string? ReportAlias { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public bool IsActive { get; set; }
        public Int16? NodeType { get; set; }
    }
}
