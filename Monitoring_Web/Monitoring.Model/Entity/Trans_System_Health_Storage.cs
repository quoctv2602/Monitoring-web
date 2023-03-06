using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Trans_System_Health_Storage")]
    public partial class Trans_System_Health_Storage
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? RequestID { get; set; }
        public int? EnvironmentID { get; set; }
        [StringLength(500)]
        public string? MachineName { get; set; }
        [StringLength(50)]
        public string? IPAddress { get; set; }
        [StringLength(50)]
        public string? DriveName { get; set; }
        [StringLength(50)]
        public string? VolumeLabel { get; set; }
        [Column(TypeName = "bigint")]
        public Int64? TotalSize { get; set; }
        [Column(TypeName = "bigint")]
        public Int64? TotalFreeSpace { get; set; }
        [Column(TypeName = "bit")]
        public bool? Violation { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
