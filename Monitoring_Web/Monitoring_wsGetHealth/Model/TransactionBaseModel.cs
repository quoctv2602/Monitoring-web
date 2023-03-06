using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring_wsGetHealth.Model
{
    public class TransactionBaseModel
    {
        public string requestID { get; set; }
        public int ErrorNumbers { get; set; }
        public int IntergrationErrorNumbers { get; set; }
        public int PendingNumbers { get; set; }
    }
    public class TransactionBaseListError
    {
        public string requestID { get; set; }
        public List<TransactionError> listTransactionError { get; set; }

    }
    public class TransactionError
    {
        public string Guid { get; set; }
        public int ErrorStatus { get; set; }
        public string CreatedDate { get; set; }

    }
}

