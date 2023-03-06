using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Trans_Data_Integration")]
    public partial class Trans_Data_Integration
    {
        [Key]
        public Guid Id { get; set; }
        public int? EnviromentId { get; set; }
        public string? Note { get; set; }
        public Guid? TransactionKey { get; set; }
        public int? MonitoredStatus { get; set; }
        public short? ReProcess { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public long? RowID { get; set; }
    }
}
