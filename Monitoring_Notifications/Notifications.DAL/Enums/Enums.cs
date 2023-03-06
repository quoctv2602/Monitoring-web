using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Enums
{
    public enum MonitoringType
    {
        CPU = 1,
        Memory = 2,
        Storage = 3,
        Transaction = 4,
        FreeDisk = 5,
        FileTransfer = 6,

        FailedTransaction = 7,
        IntergrationErrorTransaction = 8,
        PendingTransaction = 9


        /*
            ID	Name	Unit
            1	CPU       	%
            2	RAM       	%
            3	Storage   	%
            4	Process Time EDItoASCII	Miliseconds
            5	Free Disk	%
            6	Shared Storage Running Time	Miliseconds
            ---------------------------------------------
            ID	Name	                                Unit
            7	Failed Transaction	                    Transactions
            8	Intergration Error Transaction	        Transactions
            9	Pending Transaction	                    Transactions
         */
    }
    public enum SendType
    {
        Notification = 1,
        DailyReport = 2
    }
    public enum ProcessType
    {
        First = 1,
        Second = 2
    }
    public enum MessageLogStatus
    {
        SentButError = 0,
        SuccesfulySent = 1
    }
    public enum MessagePriority
    {
        Normal = 1,
        Urgent = 2
    }
}
