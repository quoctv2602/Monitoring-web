using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Trans_Data_Integration_Log")] 
    public partial class TransactionBase_Log
    {
        public Guid ID { get; set; }
        public Guid? RequestID { get; set; }
        public int? EnvironmentID { get; set; }
        public string? URL { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Error_Message { get; set; }
    }
}
