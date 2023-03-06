using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data.IRepository
{
    public interface ITransactionBaseRepository
    {
        Task<List<TransDataIntergrationMappedModel>> GetCIPReporting(CIPReportingModel param, int EnvironmentID);
        Task<ResponseData> GetReportByTransactionKey(ReportbyTransactionModel param, int EnvironmentID);
        Task<ResponseData> GetViewLogs(GetViewLogsModel param, int EnvironmentID);
        Task<ResponseData> GetContentView(GetContentViewModel param, int EnvironmentID);
        Task<ResponseInfo> DownloadFileContent(int EnvironmentID);
        Task<ResponseData> ViewCIPConfiguration(CIPConfig param, int EnvironmentID);

        Task<bool> CreateTransDataIntegration(List<TransDataIntegrationModel> param, int? MonitoredStatus);
        Task<IntegrationAPIModel> GetTransactionIntegrationAPI(int EnvironmentID);
        Task<Guid> UpdateDataIntegrationLog(Guid RequestID, int EnvironmentID, string URL, int? Status, string Error, DateTime? StartTime, DateTime? EndTime);
        Task<List<ErrorStatusModel>> GetErrorStatusModels();
    }
}
