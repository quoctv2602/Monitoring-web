using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Trans_Request_History")]
    public partial class Trans_Request_History
    {
        [Key]
        public Guid ID { get; set; }
        public int? EnvironmentID { get; set; }
        [StringLength(250)]
        public string? MachineName { get; set; }
        public int? Status { get; set; }
        public string? Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
    }
}
