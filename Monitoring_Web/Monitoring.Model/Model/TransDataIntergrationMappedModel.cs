using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Model.Model
{
    public class BaseTransDataIntergrationMappedModel
    {
        public long? RowID { get; set; }
        //public Guid? TransactionKey { get; set; }
        //public int? TotalOfDocs { get; set; }
        //public string? DocType { get; set; }
        //public string? Document { get; set; }
        //public string? StartDate { get; set; }
        //public string? EndDate { get; set; }
        //public string? SenderCustName { get; set; }
        //public string? ReceiverCustName { get; set; }
        //public int? ErrorStatus { get; set; }
        //public int? TotalRows { get; set; }
        //public int? RowsNumber { get; set; }
        public string? EnvironmentName { get; set; }
        //public int? EnvironmentID { get; set; }
        public string? ErrorStatusString { get; set; }
        //public string? MonitoredStatusString { get; set; }
        public int? MonitoredStatus { get; set; }
        public short? ReProcess { get; set; }
        public string? Note { get; set; }
        //public int? Direction { get; set; }

    }
    
    public class TransDataIntergrationMappedModel : BaseTransDataIntergrationMappedModel
    {
        public string? TransactionKey { get; set; }
        public int? TotalOfDocs { get; set; }
        public string? DocType { get; set; }
        public string? Document { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? senderCustId { get; set; }
        public string? SenderCustName { get; set; }
        public int? receiverCustId { get; set; }
        public string? ReceiverCustName { get; set; }
        public int? ErrorStatus { get; set; }
        public int? TotalRows { get; set; }
        public int? RowsNumber { get; set; }
        public int? EnvironmentID { get; set; }
        public string? MonitoredStatusString
        {
            get
            {
                if (MonitoredStatus == null)
                {
                    return null;
                }
                else
                {
                    return Enum.GetName(typeof(MonitoredStatus), MonitoredStatus);
                }
            }
        }
        public int? Direction { get; set; }
    }
}
