using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class TransDataIntegrationModel
    {
        public int? EnviromentId { get; set; }
        public string? Note { get; set; }
        public int? MonitoredStatus { get; set; }
        public Guid? TransactionKey { get; set; }
        public short? ReProcess { get; set; }
        public int? RowID { get; set; }
    }
}
