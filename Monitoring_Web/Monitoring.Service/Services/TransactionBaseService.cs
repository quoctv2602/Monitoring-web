using Monitoring.Data.IRepository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Service.Services
{
    public class TransactionBaseService : ITransactionBaseService
    {
        private readonly ITransactionBaseRepository _transactionBaseRepository;
        public TransactionBaseService(ITransactionBaseRepository transactionBaseRepository)
        {
            this._transactionBaseRepository = transactionBaseRepository;
        }
        public Task<List<TransDataIntergrationMappedModel>> GetCIPReporting(CIPReportingModel param, int EnvironmentID)
        {
            return  _transactionBaseRepository.GetCIPReporting(param, EnvironmentID);
        }
        public Task<ResponseData> GetReportByTransactionKey(ReportbyTransactionModel param, int EnvironmentID)
        {
            return _transactionBaseRepository.GetReportByTransactionKey(param, EnvironmentID);
        }  
        public Task<ResponseData> GetViewLogs(GetViewLogsModel param, int EnvironmentID) 
        {
            return _transactionBaseRepository.GetViewLogs(param, EnvironmentID);
        }
        public Task<ResponseData> GetContentView(GetContentViewModel param, int EnvironmentID)
        {
            return _transactionBaseRepository.GetContentView(param, EnvironmentID);
        }
        public Task<ResponseInfo> DownloadFileContent( int EnvironmentID)
        {
            return _transactionBaseRepository.DownloadFileContent( EnvironmentID);
        }
        public Task<ResponseData> ViewCIPConfiguration(CIPConfig param, int EnvironmentID)
        {
            return _transactionBaseRepository.ViewCIPConfiguration(param, EnvironmentID);
        }
        public Task<bool> CreateTransDataIntegration(List<TransDataIntegrationModel> param, int? MonitoredStatus)
        {
            return _transactionBaseRepository.CreateTransDataIntegration(param, MonitoredStatus);
        }
    }
}
