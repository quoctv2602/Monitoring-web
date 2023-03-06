using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Model.Model
{
    public class ResponseData
    {
        public int status { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }
    public class ResponseInfo {
        public string URL { get; set; }
        public string Appid { get; set; }
        public string AppKey { get; set; }
        public string token { get; set; }
        public string RequestID { get; set; }

        
    }


    public class ListTransactionErrors
    {
        public long? serverFileId { get; set; }
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
        public string? EnvironmentName { get; set; }
        public int? EnvironmentID { get; set; }
        public string? ErrorStatusString { get; set; }
        public int? Direction { get; set; }
    }
    public class ListViewLog
    {
        public string? RequestID { get; set; }
        public Guid? TransactionLogID { get; set; }
        public Guid TransactionKey { get; set; }
        public DateTime Date { get; set; }
        public string? ActionName { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? ErrorCodeID { get; set; }
        public int? TotalRows { get; set; }
        public string? StatusString
        {
            get
            {
                if (Status == null)
                {
                    return null;
                }
                else
                {
                    return Enum.GetName(typeof(TransactionLogStatus), Convert.ToInt16(Status));
                }
            }
        }
    }
    public class ListContentFile
    {
        public string? RequestID { get; set; }
        public string? FileContent { get; set; }
        public int isFile { get; set; }


    }

   

    public class CipConfiguration
    {
        public List<MailboxList> senderMailboxList { get; set; }
        public List<MailboxList> receiverMailboxList { get; set; }

    }
    public class MailboxList
    {
        public string? fileTransferUserID { get; set; }
        public string? mailboxName { get; set; }
        public string? mailboxHomeDir { get; set; }
        public string? fileTransferType { get; set; }
        public string? clientURL { get; set; }
        public string? clientConnectionProtocol { get; set; }
    }



    public class ListViewCIPConfiguration
    {
        public string? RequestID { get; set; }
        public CipConfiguration CipConfiguration { get; set; }
        public SenderCustomer senderCustomer { get; set; }
        public ReceiverCustomer receiverCustomer { get; set; }
       
    }

    public class SenderCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Isaid { get; set; }
        public string GSIn { get; set; }
        public string GSOut { get; set; }
        public string Qualifier { get; set; }
        public string CustomerRanking { get; set; }
        public string CustomerOwner { get; set; }
        public string SiteID { get; set; }
        public string UseCIP { get; set; }
        public string? UseCIPName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UseCIP))
                {
                    return null;
                }
                else
                {
                    return Enum.GetName(typeof(UseCIPEnum), Convert.ToInt16(UseCIP));
                }
            }
        }
        public string DownloadEDI_YN { get; set; }
        public string DownloadASCII_YN { get; set; }
        public string UseASCIIR9_YN { get; set; }
        public string UseDiMetrics_YN { get; set; }
    }

    public class ReceiverCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Isaid { get; set; }
        public string GSIn { get; set; }
        public string GSOut { get; set; }
        public string Qualifier { get; set; }
        public string CustomerRanking { get; set; }
        public string CustomerOwner { get; set; }
        public string SiteID { get; set; }
        public string UseCIP { get; set; }
        public string? UseCIPName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UseCIP))
                {
                    return null;
                }
                else
                {
                    return Enum.GetName(typeof(UseCIPEnum), Convert.ToInt16(UseCIP));
                }
            }
        }
        public string DownloadEDI_YN { get; set; }
        public string DownloadASCII_YN { get; set; }
        public string UseASCIIR9_YN { get; set; }
        public string UseDiMetrics_YN { get; set; }
    }

    public class ListDataExportExcel
    {
        public string? Note { get; set; }
        public string? Environment { get; set; }
        public string? TransactionKey { get; set; }
        public int? TotalOfDocs { get; set; }
        public string? DocType { get; set; }
        public string? Document { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public string? ErrorStatus { get; set; }
        public string? ReProcessed { get; set; }
        public string? MonitoredStatus { get; set; }
    }

}
