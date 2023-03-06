using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Trans_System_Health_Instance")]
    public partial class Trans_System_Health_Instance
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? RequestID { get; set; }
        public int? EnvironmentID { get; set; }
        [StringLength(500)]
        public string? MachineName { get; set; }
        [StringLength(50)]
        public string? IPAddress { get; set; }
        [StringLength(500)]
        public string? ProcessName { get; set; }
        public int? CPU { get; set; }
        public int? Ram { get; set; }
        public int? Storage { get; set; }
        public int? Instance { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
