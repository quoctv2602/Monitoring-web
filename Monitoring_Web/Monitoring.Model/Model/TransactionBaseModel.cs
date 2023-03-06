using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class Default_TransactionBaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? PageNo { get; set; }
        public int PageSize { get; set; }
        public int? PageNumber { get; set; }
    }

    public class CIPReportingModel : Default_TransactionBaseModel
    {
        public string? Status { get; set; }
        public string? CIPFlow { get; set; }
        public string? TransID { get; set; }
        public string? DocID { get; set; }
        public string? SenderCustName { get; set; }
        public string? ReceiverCustName { get; set; }
        public string? SenderMailboxName { get; set; }
        public string? ReceiverMailboxName { get; set; }
        public string? ISAControl { get; set; }
        public string? GSControl { get; set; }
        public string? SenderCustPriority { get; set; }
        public string? ReceiverCustPriority { get; set; }
        public Guid TransactionKey { get; set; }
        public string? document { get; set; }

    }

    public class ReportbyTransactionModel : Default_TransactionBaseModel
    {
        public Guid TransactionKey { get; set; }
        public string? Document { get; set; }

    }
    public class GetViewLogsModel {
        public Guid TransactionKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class GetContentViewModel
    {
        public long ServerFileID { get; set; }
    }
    public class GetFileContentViewModel
    {
        public string fullLocalPath { get; set; }
    }

    public class ContentView
    {
        public string? CIPFlow { get; set; }
        public string? TransactionKey { get; set; }
        public string? FileName { get; set; }
    }
    public class CIPConfig
    {
        public string? TransactionKey { get; set; }
        public int fromCustID { get; set; }
        public int toCustID { get; set; }

      
    }

    



}
