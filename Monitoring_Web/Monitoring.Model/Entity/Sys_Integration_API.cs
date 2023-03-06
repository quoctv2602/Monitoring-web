using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Integration_API")]
    public partial class Sys_Integration_API
    {
        [Key]
        public int ID { get; set; }
        public int? EnvironmentID { get; set; }
        [StringLength(500)]
        public string? MachineName { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? domain_SystemHealth { get; set; }
        public Int16? NodeType { get; set; }
        public bool? IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(4000)]
        public string? ServiceList { get; set; }
        public bool? IsDefaultNode { get; set; }
    }
}
