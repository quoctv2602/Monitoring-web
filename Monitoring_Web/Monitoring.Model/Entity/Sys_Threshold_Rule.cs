using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Threshold_Rule")]
    public partial class Sys_Threshold_Rule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Node_Setting { get; set; }
        public int EnvironmentID { get; set; }
        [StringLength(500)]
        public string? MachineName { get; set; } 
        public int MonitoringType { get; set; }
        [StringLength(255)]
        public string? MonitoringName { get; set; }
        public byte? Condition { get; set; }
        public int? Threshold { get; set; }
        public int? ThresholdCounter { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
    }
}
