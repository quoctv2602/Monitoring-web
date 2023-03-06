using MailKit;
using Notifications.DAL;
using Notifications.DAL.EFModel;
using Notifications.DAL.Enums;
using Notifications.EmailService;
using Notifications.Options;
using System;
using System.Text;

namespace Monitoring_Notifications.Jobs
{
    public class EmailHandler
    {
        public static Tuple<string, string> FormatTemplate1Email(string environment,
                string machineName, int? threshold, int? thresholdCounter,
                string KPI, string unitOfMeasure, string emailToRep, string NotificationMonitoringURL)
        {
            // format html content
            // subject : EnvironmentName, MachineName
            // Body : MachineName, threshold, repeatedcounter
            string subject = string.Format("[Monitoring Tool-{0}]: System Alert_{1}_{2}", environment, machineName, KPI);
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            //string curDir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText(curDir + "/EmailTemplates/template1.html");
            content = content.Replace("{{Rep}}", emailToRep)
                .Replace("{{KPI}}", KPI)
                .Replace("{{threshold}}", threshold.ToString())
                .Replace("{{UnitOfMeasure}}", unitOfMeasure)
                .Replace("{{NotificationMonitoringURL}}", NotificationMonitoringURL)
                .Replace("{{repeated counter}}", thresholdCounter.ToString());
            return Tuple.Create(subject, content);
        }

        public static Tuple<string, string> FormatTemplate2Email(string environment,
                            string machineName, int? threshold, int? thresholdCounter, string NotificationMonitoringURL)
        {
            // format html content
            // subject : EnvironmentName, MachineName
            // Body : MachineName, threshold, repeatedcounter
            string subject = string.Format("[Monitoring Tool-{0}]: System Alert_{1}", environment, machineName);
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            //string curDir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText(curDir + "/EmailTemplates/template2.html");
            content = content.Replace("{{Resource}}", environment)
                .Replace("{{threshold}}", threshold.ToString())
                .Replace("{{NotificationMonitoringURL}}", NotificationMonitoringURL)
                .Replace("{{repeated counter}}", (thresholdCounter * 2).ToString());
            return Tuple.Create(subject, content);
        }

        public static Tuple<string, string> FormatTemplate3Email(string environment, string machineName, string rep, string notificationMonitoringURL, string service)
        {
            // format html content
            // subject : EnvironmentName, MachineName
            // Body : MachineName, threshold, repeatedcounter
            string subject = string.Format("[Monitoring Tool-{0}]: System Alert_{1}_{2}", environment, machineName, service);
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            //string curDir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText(curDir + "/EmailTemplates/template3.html");
            content = content.Replace("{{service}}", service)
                .Replace("{{resource}}", machineName)
                .Replace("{{environment}}", environment)
                .Replace("{{NotificationMonitoringURL}}", notificationMonitoringURL)
                .Replace("{{Rep}}", rep);
            return Tuple.Create(subject, content);
        }

        public static Tuple<string, string> FormatTemplate5Email(string environment, string machineName, string rep, string notificationMonitoringURL)
        {
            // format html content
            // subject : EnvironmentName, MachineName
            // Body : MachineName, threshold, repeatedcounter
            string subject = string.Format("[Monitoring Tool-{0}]: System Alert_{1}", environment, machineName);
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            //string curDir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText(curDir + "/EmailTemplates/template5.html");
            content = content.Replace("{{resource}}", machineName)
                .Replace("{{environment}}", environment)
                .Replace("{{NotificationMonitoringURL}}", notificationMonitoringURL)
                .Replace("{{Rep}}", rep);
            return Tuple.Create(subject, content);
        }

        public static Tuple<string, string> FormatTemplate4Email(string environment,
                string machineName, List<TransMessageLog> messageLogs,
                string CPU, int cpuAverage, string cpuUnitOfMeasure,
                string memory, int memoryAverage, string memoryUnitOfMeasure,
                string storage, int storageAverage, string storageUnitOfMeasure, string alias, string NotificationMonitoringURL,
                SysMonitoring FreeDisk, List<StatisticsDTO_FreeDisk> FreeDiskStatistics,
                 SysMonitoring EDItoASCIIMon, StatisticsDTO EDItoASCIIStatistics,
                 SysMonitoring FileTransferMon, StatisticsDTO FileTransferStatistics,
                 SysMonitoring ErrorNumbersMon, StatisticsDTO ErrorNumbersStatistics, SysMonitoring IntergrationErrorNumbersMon, StatisticsDTO IntergrationErrorNumbersStatistics, SysMonitoring PendingTransactionsMon, StatisticsDTO PendingTransactionsStatistics



            )
        {
            // format html content
            // subject : EnvironmentName, MachineName
            // Body : MachineName, threshold, repeatedcounter
            string tableHeader = @"<table><tr style='background-color: #4ca2f9; color: #FFF; '><th style='text-align: center;width:30px;'>Subject</th><th style='text-align: center'>Priority</th><th style='text-align: center;width:150px;'>Send Time</th><th style='text-align: center'>Content</th><th style='text-align: center;width:50px;'>Status</th></tr>";
            string tableFooter = @"</table>";
            string bodyTemplate = @"<tr><th>{{Subject}}</th><th  style='text-align: center'>{{Priority}}</th><th style='text-align: center;'>{{Sent Time}}</th><th>{{Content}}</th><th  style='text-align: center;'>{{Status}}</th></tr>";
            string subject = string.Format("[Monitoring Tool-{0}]: Daily Statistic Report", environment);
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            //string curDir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText(curDir + "/EmailTemplates/template4.html");
            StringBuilder builder = new StringBuilder();
            builder.Append(tableHeader);
            foreach (TransMessageLog log in messageLogs)
            {
                string Status = log.Status == 1 ? "Sent" : "Pending";
                string Priority = log.Priority == 1 ? "Normal" : "Urgent";


                builder.Append(bodyTemplate.Replace("{{Subject}}", log.EmailSubject)
                            .Replace("{{Priority}}", Priority)
                            .Replace("{{Sent Time}}", log.CreatedDate.ToString())
                            .Replace("{{Content}}", log.EmailBody)

                            .Replace("{{Status}}", Status));
            }
            if (messageLogs.Count == 0)
            {
                builder.Append(@"<tr><th colspan='5' style='text-align: center'>No data</th></tr>");
            }
            builder.Append(tableFooter);
            content = content.Replace("{{ContentTable}}", builder.ToString()).Replace("{{NotificationMonitoringURL}}", NotificationMonitoringURL); ;

            string tableStatisticsHeader = @"<table><tr style='background-color: #4ca2f9; color: #FFF; '><th style='text-align: center'>Node</th><th style='text-align: center'>KPI</th><th style='text-align: center'>Average</th><th style='text-align: center'>Unit Of Measure</th></tr>";
            string tableStatisticsFooter = @"</table>";
            string bodyStatisticsTemplate = @"<tr><th>{{Node}}</th><th>{{KPI}}</th><th  style='text-align: center'>{{Average}}</th><th  style='text-align: center'>{{UnitOfMeasure}}</th></tr>";

            StringBuilder statisticsBuilder = new StringBuilder();
            statisticsBuilder.Append(tableStatisticsHeader);
            // CPU
            string cpuStatistics = bodyStatisticsTemplate.Replace("{{Node}}", machineName)
                            .Replace("{{KPI}}", CPU)
                            .Replace("{{Average}}", cpuAverage.ToString())
                            .Replace("{{UnitOfMeasure}}", cpuUnitOfMeasure);
            //Memory
            string memoryStatistics = bodyStatisticsTemplate.Replace("{{Node}}", machineName)
                            .Replace("{{KPI}}", memory)
                            .Replace("{{Average}}", memoryAverage.ToString())
                            .Replace("{{UnitOfMeasure}}", memoryUnitOfMeasure);
            //Storage
            string storageStatistics = bodyStatisticsTemplate.Replace("{{Node}}", machineName)
                .Replace("{{KPI}}", storage)
                .Replace("{{Average}}", storageAverage.ToString())
                .Replace("{{UnitOfMeasure}}", storageUnitOfMeasure);

            //FreeDisk
            string detailFreeDisk = "";
            foreach (var item in FreeDiskStatistics)
            {
                detailFreeDisk += item.DriveName + ": " + item.Average + "<br>";
            }
            string FreeDiskStatistic = bodyStatisticsTemplate.Replace("{{Node}}", machineName)
              .Replace("{{KPI}}", FreeDisk.Name)
              .Replace("{{Average}}", detailFreeDisk)
              .Replace("{{UnitOfMeasure}}", FreeDisk.Unit);

            //EDItoASCII
            string EDItoASCIIStatistic = bodyStatisticsTemplate.Replace("{{Node}}", machineName)
                .Replace("{{KPI}}", EDItoASCIIMon.Name)
                .Replace("{{Average}}", EDItoASCIIStatistics.Average.ToString())
                .Replace("{{UnitOfMeasure}}", EDItoASCIIMon.Unit);

            //FileTransfer
            string FileTransferStatistic = bodyStatisticsTemplate.Replace("{{Node}}", machineName)
                .Replace("{{KPI}}", FileTransferMon.Name)
                .Replace("{{Average}}", FileTransferStatistics.Average.ToString())
                .Replace("{{UnitOfMeasure}}", FileTransferMon.Unit);


            string ErrorNumbers = bodyStatisticsTemplate.Replace("{{Node}}", "N/A")
                .Replace("{{KPI}}", ErrorNumbersMon.Name)
                .Replace("{{Average}}", ErrorNumbersStatistics.Average.ToString())
                .Replace("{{UnitOfMeasure}}", ErrorNumbersMon.Unit);

            string IntergrationErrorNumbers = bodyStatisticsTemplate.Replace("{{Node}}", "N/A")
                .Replace("{{KPI}}", IntergrationErrorNumbersMon.Name)
                .Replace("{{Average}}", IntergrationErrorNumbersStatistics.Average.ToString())
                .Replace("{{UnitOfMeasure}}", IntergrationErrorNumbersMon.Unit);

            string PendingTransactions = bodyStatisticsTemplate.Replace("{{Node}}", "N/A")
                .Replace("{{KPI}}", PendingTransactionsMon.Name)
                .Replace("{{Average}}", PendingTransactionsStatistics.Average.ToString())
                .Replace("{{UnitOfMeasure}}", PendingTransactionsMon.Unit);


            statisticsBuilder.Append(cpuStatistics);
            statisticsBuilder.Append(memoryStatistics);
            statisticsBuilder.Append(storageStatistics);
            statisticsBuilder.Append(FreeDiskStatistic);
            statisticsBuilder.Append(EDItoASCIIStatistic);
            statisticsBuilder.Append(FileTransferStatistic);
            statisticsBuilder.Append(ErrorNumbers);
            statisticsBuilder.Append(IntergrationErrorNumbers);
            statisticsBuilder.Append(PendingTransactions);
            statisticsBuilder.Append(tableStatisticsFooter);


            content = content.Replace("{{StatisticsTable}}", statisticsBuilder.ToString())
                    .Replace("{{date}}", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"))
                    .Replace("{{Alias}}", alias);

            return Tuple.Create(subject, content);
        }

        public static void SendEmail(EmailConfig emailConfig, string emailTo,
                string subject, string content, IEmailProvider emailService)
        {
            emailService.Send(emailConfig, emailTo, subject, content);
        }
    }
}
