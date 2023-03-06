using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Trans_System_Health")]
    public partial class Trans_System_Health
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? RequestID { get; set; }
        public int? EnvironmentID { get; set; }
        [StringLength(500)]
        public string? MachineName { get; set; }
        public byte? NotificationStatus { get; set; }
        [StringLength(50)]
        public string? IPAddress { get; set; }
        public int? CPUInfo { get; set; }
        public bool? CPUViolation { get; set; }
        public int? MemoryInfo { get; set; }
        public bool? MemoryViolation { get; set; }
        public int? StorageInfo { get; set; }
        public bool? StorageViolation { get; set; }
        public int? NumberOfTransactionFail { get; set; }
        public bool? NumberOfTransactionFailViolation { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RequestTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ResponseTime { get; set; }
        public string? ContentData { get; set; }
        public int? Status { get; set; }
        public string? Error_Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
