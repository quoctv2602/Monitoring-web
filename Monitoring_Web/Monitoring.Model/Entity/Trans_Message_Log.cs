using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Trans_Message_Log")]
    public partial class Trans_Message_Log
    {
        [Key]
        public Guid ID { get; set; }
        public int Status { get; set; }
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        public string? EmailTo { get; set; }
        public string? EmailCC { get; set; }
        public string? EmailBCC { get; set; }
        public bool? IsMailServer { get; set; }
        [StringLength(2000)]
        public string? EmailSubject { get; set; }
        [Column(TypeName = "ntext")]
        public string? EmailBody { get; set; }
        public int? EnvironmentId { get; set; }
        public int SendType { get; set; }
    }
}
