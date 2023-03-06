using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Trans_Data_Health")]
    public partial class Trans_Data_Health
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
        public string? AppName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BeginTransaction { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTransaction { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BeginFileTransfer { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndFileTransfer { get; set; }
        public int? Status { get; set; }
        public string? Error_Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
