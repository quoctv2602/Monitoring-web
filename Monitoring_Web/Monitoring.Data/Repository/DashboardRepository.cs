using DiCentral.RetrySupport._6._0.DBHelper;
using Microsoft.EntityFrameworkCore;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using static Monitoring_Common.Enum;

namespace Monitoring.Data.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public DashboardRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }

        public async Task<List<DashboardSystemHealthModel>> DataDashboardSystemHealth(List<NodeSettingModel> nodeSettings, int monitoringType, int topSelect)
        {
            string query = string.Empty;
            if (monitoringType != (int)Monitoring_Common.Enum.MonitoringType.SharedStorageRunningTime && monitoringType != (int)Monitoring_Common.Enum.MonitoringType.ProcessTime_EDItoASCII)
            {
                query = @"  DECLARE @tb as table (EnvironmentID int,MachineName nvarchar(max))
                                DECLARE @CreatedTime as Table(CreatedDate datetime)
                                DECLARE @MinDate datetime,@MaxDate datetime
                                DECLARE @ReturnTable AS TABLE(EnvironmentName nvarchar(255),EnvironmentID INT,MachineName nvarchar(255),[Data] INT,[Label] NVARCHAR(64),Threshold INT,RequestID uniqueidentifier,NodeID INT,DateString VARCHAR(10))
                               declare @MonitoringType int=" + monitoringType;

                if (nodeSettings.Count > 0)
                {
                    string detail = "";
                    for (int i = 0; i < nodeSettings.Count; i++)
                    {
                        detail += "(" + nodeSettings[i].EnvironmentID + ",N'" + nodeSettings[i].NodeName + "'),";
                    }
                    query += String.Format(@"insert into @tb values {0}", detail.Substring(0, detail.Length - 1));
                }

                query += @" insert into @CreatedTime
                        select top (" + topSelect + @") tsh.CreatedDate
                        from Trans_System_Health as tsh WITH (NOLOCK)
                        where tsh.[Status]= 1
                        AND exists (Select 1 from @tb t where tsh.EnvironmentID=t.EnvironmentID and tsh.MachineName=t.MachineName)
                        order by tsh.CreatedDate desc

                        select @MinDate=min(rt.CreatedDate),@MaxDate=max(rt.CreatedDate)
                        from @CreatedTime as rt

                        INSERT INTO @ReturnTable
                        SELECT se.Name EnvironmentName,tsh.EnvironmentID,tsh.MachineName,
                        case when @MonitoringType=1 then CPUInfo when @MonitoringType=2 then tsh.MemoryInfo when @MonitoringType=3 then tsh.StorageInfo else null end [Data]
                        ,Format(tsh.CreatedDate,'HH:mm:ss') [Label],sth.Threshold,tsh.RequestID,sth.Node_Setting NodeID,FORMAT(tsh.CreatedDate,'yyyy-MM-dd') DateString
                        from Trans_System_Health as tsh WITH (NOLOCK)
                        left join Sys_Environment as se WITH (NOLOCK) on tsh.EnvironmentID=se.ID
                        left join Sys_Threshold_Rule as sth WITH (NOLOCK) on sth.EnvironmentID=se.ID and sth.MachineName=tsh.MachineName
                        where exists (Select 1 from @tb t where tsh.EnvironmentID=t.EnvironmentID and tsh.MachineName=t.MachineName)
                        and sth.MonitoringType=@MonitoringType
                        and tsh.CreatedDate between @MinDate and @MaxDate
                        and tsh.[Status]=1
                        Order by tsh.CreatedDate asc

                        INSERT INTO @ReturnTable
                        SELECT se.Name,se.ID,t.MachineName,-1,FORMAT(ISNULL(@MinDate,GETDATE()),'HH:mm:ss'),sth.Threshold,NULL,sth.Node_Setting,FORMAT(ISNULL(@MinDate,GETDATE()),'yyyy-MM-dd') DateString
                        FROM @tb AS t
                        LEFT JOIN Sys_Environment AS se WITH (NOLOCK) ON t.EnvironmentID=se.ID 
                        LEFT JOIN Sys_Threshold_Rule AS sth WITH (NOLOCK) ON sth.EnvironmentID=t.EnvironmentID AND sth.MachineName=t.MachineName
                        WHERE NOT EXISTS (SELECT 1 FROM @ReturnTable AS rt where rt.EnvironmentID=t.EnvironmentID and rt.MachineName=t.MachineName)
                        AND sth.MonitoringType=@MonitoringType

                        SELECT *
                        FROM @ReturnTable
                    ";
            }
            else
            {
                query = @"  DECLARE @tb AS TABLE (EnvironmentID INT,MachineName nvarchar(max))
                            DECLARE @RequestTime as Table(RequestTime DATETIME)
                            DECLARE @MinDate DATETIME,@MaxDate DATETIME
                            DECLARE @ReturnData AS TABLE (EnvironmentName NVARCHAR(250),EnvironmentID INT,MachineName NVARCHAR(150),[Data] INT,[Label] VARCHAR(10),Threshold INT,RequestID uniqueidentifier,NodeID INT,DateString VARCHAR(10))
                            DECLARE @MonitoringType int=" + monitoringType;

                if (nodeSettings.Count > 0)
                {
                    string detail = "";
                    for (int i = 0; i < nodeSettings.Count; i++)
                    {
                        detail += "(" + nodeSettings[i].EnvironmentID + ",N'" + nodeSettings[i].NodeName + "'),";
                    }
                    query += String.Format(@"insert into @tb values {0}", detail.Substring(0, detail.Length - 1));
                }

                query += @" 
                        INSERT INTO @RequestTime
                        SELECT TOP (" + topSelect + @") tdh.CreatedDate
                        FROM Trans_Data_Health AS tdh WITH (NOLOCK)
                        WHERE tdh.[Status]=1
                        AND EXISTS (SELECT 1 FROM @tb AS tb WHERE tb.EnvironmentID=tdh.EnvironmentID AND tb.MachineName=tdh.MachineName)
                        ORDER BY tdh.CreatedDate DESC

                        SELECT @MinDate=MIN(rt.RequestTime),@MaxDate=MAX(rt.RequestTime)
                        FROM @RequestTime AS rt

                        INSERT INTO @ReturnData
                        SELECT se.Name EnvironmentName,tdh.EnvironmentID,tdh.MachineName,
                        CASE WHEN @MonitoringType=6 THEN tdh.TransferElapsed ELSE tdh.TransactionElapsed END [Data],
                        FORMAT(tdh.CreatedDate,'HH:mm:ss') [Label],sth.Threshold,tdh.RequestID,sth.Node_Setting NodeID,
                        FORMAT(tdh.CreatedDate,'yyyy-MM-dd') DateString
                        FROM Trans_Data_Health AS tdh WITH (NOLOCK)
                        LEFT JOIN Sys_Environment AS se WITH (NOLOCK) ON tdh.EnvironmentID=se.ID
                        LEFT JOIN Sys_Threshold_Rule AS sth WITH (NOLOCK) ON sth.EnvironmentID=se.ID AND sth.MachineName=tdh.MachineName
                        WHERE sth.MonitoringType=@MonitoringType
                        AND tdh.[Status]=1
                        AND tdh.CreatedDate BETWEEN @MinDate AND @MaxDate
                        AND EXISTS (SELECT 1 FROM @tb AS tb WHERE tb.EnvironmentID=tdh.EnvironmentID AND tb.MachineName=tdh.MachineName)
                    
                        INSERT INTO @ReturnData
						SELECT se.Name,se.ID,t.MachineName,-1,FORMAT(ISNULL(@MinDate,GETDATE()),'HH:mm:ss'),sth.Threshold,NULL,sth.Node_Setting,FORMAT(ISNULL(@MinDate,GETDATE()),'yyyy-MM-dd') DateString
						FROM @tb AS t
						LEFT JOIN Sys_Environment AS se WITH (NOLOCK) ON se.ID=t.EnvironmentID
						LEFT JOIN Sys_Threshold_Rule AS sth WITH (NOLOCK) ON sth.EnvironmentID=se.ID AND sth.MachineName=t.MachineName
						WHERE sth.MonitoringType=@MonitoringType
						AND NOT EXISTS (SELECT 1 FROM @ReturnData AS rd WHERE rd.EnvironmentID=t.EnvironmentID AND rd.MachineName=t.MachineName)

						SELECT *
						FROM @ReturnData
                        ";
            }
            var list = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.DashboardSystemHealthModels.FromSqlRaw(query).ToListAsync(), CancellationToken.None);
            return list;
        }

        public async Task<List<NodeSettingModel>> ListNodeSettings()
        {
            try
            {
                var list = await DBRetryHelper.Default.ExecuteAsync(() => (from sns in _monitoringContext.Sys_Node_Settings
                                                     //join str in _monitoringContext.Sys_Threshold_Rules.ToList() on new { sns.EnvironmentID, sns.ID } equals new { str.EnvironmentID, ID = str.Node_Setting }
                                                 join se in _monitoringContext.Sys_Environments on sns.EnvironmentID equals se.ID
                                                 join sia in _monitoringContext.Sys_Integration_APIs on new { EnvironmentID = (int?)sns.EnvironmentID, sns.MachineName } equals new { EnvironmentID = sia.EnvironmentID, sia.MachineName }
                                                 into Result
                                                 from sia in Result.DefaultIfEmpty()
                                                 where sns.IsActive
                                                 select new NodeSettingModel
                                                 {
                                                     ID = sns.ID,
                                                     NodeName = sns.NodeName,
                                                     Description = sns.Description,
                                                     EnvironmentID = sns.EnvironmentID,
                                                     NotificationEmail = sns.NotificationEmail,
                                                     ReportEmail = sns.ReportEmail,
                                                     ServiceList = sns.ServiceList,
                                                     EnvironmentName = se.Name,
                                                     NodeType = sns.NodeType,
                                                     IsDefault = sia.IsDefaultNode
                                                 }).ToListAsync(),CancellationToken.None);
                var distinctList = list.DistinctBy(a => new { a.ID, a.NodeName, a.Description, a.EnvironmentID, a.NotificationEmail, a.ReportEmail, a.ServiceList, a.EnvironmentName, a.NodeType, a.IsDefault }).ToList();
                return distinctList;
            }
            catch (Exception ex)
            {
                return new List<NodeSettingModel>();
            }
        }

        public async Task<List<ServiceModel>> ServiceList(List<ServiceListRequestModel> nodeSetting)
        {


            string query = @"
                            DECLARE @MachineList AS TABLE (EnvironmentID INT,MachineName NVARCHAR(250))
                            DECLARE @LastedDate AS TABLE (EnvironmentID INT,MachineName NVARCHAR(250),CreatedDate DATETIME)
                            DECLARE @RequestList AS TABLE (EnvironmentID INT,MachineName NVARCHAR(250),RequestID uniqueidentifier)
                            ";

            if (nodeSetting.Count > 0)
            {
                string detail = "";
                for (int i = 0; i < nodeSetting.Count; i++)
                {
                    var nodeInfo = nodeSetting[i];
                    detail += "(" + nodeInfo.EnvironmentID + ",N'" + nodeInfo.MachineName + "'),";
                }
                query += String.Format(@"insert into @MachineList values {0}", detail.Substring(0, detail.Length - 1));
            }
            query += @"
                        INSERT INTO @LastedDate
                        SELECT tsh.EnvironmentID,tsh.MachineName,MAX(tsh.CreatedDate) CreatedDate
                        FROM Trans_System_Health AS tsh WITH (NOLOCK)
                        WHERE EXISTS (SELECT 1 FROM @MachineList AS ml WHERE tsh.EnvironmentID=ml.EnvironmentID AND tsh.MachineName=ml.MachineName)
                        GROUP BY tsh.EnvironmentID,tsh.MachineName

                        INSERT INTO @RequestList
                        SELECT tsh.EnvironmentID,tsh.MachineName,tsh.RequestID
                        FROM Trans_System_Health AS tsh WITH (NOLOCK)
                        WHERE EXISTS (SELECT 1 FROM @LastedDate AS ld WHERE ld.EnvironmentID=tsh.EnvironmentID AND ld.CreatedDate=tsh.CreatedDate AND ld.MachineName=tsh.MachineName)


                        SELECT dt.NodeName,dt.ServiceName,dt.EnvironmentID,dt.RequestID,dt.Status,dt.Instance
						FROM (
                        SELECT rl.MachineName NodeName,tshi.ProcessName ServiceName,rl.EnvironmentID EnvironmentID,rl.RequestID,tshi.Status,tshi.Instance
						,case when tshi.Status='Running' then 1 else 2 end as Status_number
                        FROM @RequestList AS rl
                        LEFT JOIN Trans_System_Health_Instance AS tshi WITH (NOLOCK) ON rl.EnvironmentID=tshi.EnvironmentID AND rl.MachineName=tshi.MachineName AND rl.RequestID=tshi.RequestID
                        WHERE tshi.ProcessName IS NOT NULL
                        --ORDER BY rl.MachineName
						)dt ORDER BY dt.Status_number ,dt.NodeName ASC
                        ";


            return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.ServiceModel.FromSqlRaw(query).ToListAsync(), CancellationToken.None);
        }
        public async Task<List<DashboardSystemHealth_KPIFreeDiskModel>> DataDashboardSystemHealth_FreeDisk(List<NodeSettingModel> nodeSettings, int monitoringType, int topSelect)
        {
            string query = @"DECLARE @MasterTime AS TABLE (CreatedDate Datetime,MachineName nvarchar(250),EnvironmentID int,DriveName nvarchar(250))
                             DECLARE @ReturnData AS TABLE (EnvironmentID INT,EnvironmentName NVARCHAR(250),MachineName NVARCHAR(250),DriveName NVARCHAR(250),VolumeLabel NVARCHAR(250),TotalSize BIGINT,TotalFreeSpace BIGINT,TotalUsedSpace BIGINT,PercentFreeSpace DECIMAL(18,2),PercentUsedSpace DECIMAL(18,2),Threshold INT,RequestID uniqueidentifier)
                             DECLARE @tb AS table (EnvironmentID int,MachineName nvarchar(max))";

            if (nodeSettings.Count > 0)
            {
                string detail = "";
                for (int i = 0; i < nodeSettings.Count; i++)
                {
                    detail += "(" + nodeSettings[i].EnvironmentID + ",N'" + nodeSettings[i].NodeName + "'),";
                }
                query += String.Format(@"INSERT INTO @tb VALUES {0}", detail.Substring(0, detail.Length - 1));
            }

            query += @"     INSERT INTO @MasterTime
                            SELECT MAX(CreateDate) CreatedDate,MachineName,EnvironmentID,DriveName
                            FROM Trans_System_Health_Storage AS tshr WITH (NOLOCK)
                            WHERE EXISTS (SELECT 1 FROM @tb t WHERE tshr.EnvironmentID=t.EnvironmentID and tshr.MachineName=t.MachineName)
                            Group by MachineName,EnvironmentID,DriveName

                            INSERT INTO @ReturnData
                            SELECT tshr.EnvironmentID,se.Name EnvironmentName,tshr.MachineName,
                            tshr.DriveName,tshr.VolumeLabel,tshr.TotalSize,tshr.TotalFreeSpace,
                            tshr.TotalSize-tshr.TotalFreeSpace TotalUsedSpace,
                            CAST(ROUND(CONVERT(float,TotalFreeSpace)/TotalSize*100,2) AS decimal(18,2)) PercentFreeSpace,
                            CAST(ROUND(CONVERT(float,TotalSize-TotalFreeSpace)/TotalSize*100,2) AS decimal(18,2)) PercentUsedSpace,sth.Threshold,
                            tshr.RequestID
                            FROM Trans_System_Health_Storage AS tshr WITH (NOLOCK)
                            LEFT JOIN Sys_Environment AS se WITH (NOLOCK) on tshr.EnvironmentID=se.ID
                            LEFT JOIN Sys_Node_Setting AS sns WITH (NOLOCK) ON sns.EnvironmentID=tshr.EnvironmentID and sns.MachineName=tshr.MachineName
                            LEFT JOIN Sys_Threshold_Rule AS sth WITH (NOLOCK) on sth.Node_Setting=sns.ID
                            WHERE sth.MonitoringType=5
                            AND EXISTS (SELECT 1 FROM @MasterTime AS mt WHERE tshr.MachineName=mt.MachineName and tshr.EnvironmentID=mt.EnvironmentID and tshr.CreateDate=mt.CreatedDate)
                            ORDER BY EnvironmentID,MachineName,DriveName
                            
                            INSERT INTO @ReturnData
                            SELECT t.EnvironmentID,se.Name,t.MachineName,NULL,NULL,-1,-1,-1,-1,-1,sth.Threshold,NULL
                            FROM @tb AS t
                            LEFT JOIN Sys_Environment AS se WITH (NOLOCK) ON t.EnvironmentID=se.ID 
                            LEFT JOIN Sys_Node_Setting AS sns WITH (NOLOCK) ON sns.EnvironmentID=t.EnvironmentID and sns.MachineName=t.MachineName
                            LEFT JOIN Sys_Threshold_Rule AS sth WITH (NOLOCK) on sth.Node_Setting=sns.ID
                            WHERE sth.MonitoringType=5
                            AND NOT EXISTS (SELECT 1 FROM @ReturnData AS rd WHERE rd.EnvironmentID=t.EnvironmentID AND rd.MachineName=t.MachineName)


                            SELECT *
                            FROM @ReturnData
                        ";

            var list = await DBRetryHelper.Default.ExecuteAsync(()=> _monitoringContext.KPIFreeDiskModels.FromSqlRaw(query).ToListAsync(), CancellationToken.None);
            return list;
        }
        public async Task<List<DashboardTransaction_TableModel>> DashboardTransaction_Table(int environmentID, string cipFlow, string lastest, int topSelect)
        {
            try
            {
                string query = string.Empty;
                query = @" DECLARE @ReturnTable AS TABLE (RowNum INT,TransactionKey uniqueidentifier,ErrorStatus NVARCHAR(128),MonitoredStatus NVARCHAR(128))
                       DECLARE @MasterData AS TABLE (TransactionKey uniqueidentifier,CreatedDate DATETIME)
                       DECLARE @EnvironmentID INT=" + environmentID + @",@CIPFLOW CHAR(1)='" + cipFlow + "',@Lastest VARCHAR(64)";

                query += @"
                        INSERT INTO @MasterData
                        SELECT TOP (" + topSelect + @") t.TransactionKey,t.MaxDate
                        FROM
                        (
                            SELECT TransactionKey,max(CreatedDate) MaxDate
                            FROM Trans_Data_Status WITH (NOLOCK)
                            WHERE CONVERT(DATE,CreatedDate)=CONVERT(DATE,GETDATE())
                            AND EnvironmentID=@EnvironmentID
                            AND CIPFolow=@CIPFLOW
                            GROUP BY TransactionKey
                            
                        )t
                        ORDER BY t.MaxDate desc

                        INSERT INTO @ReturnTable
                        SELECT  ROW_NUMBER() OVER(ORDER BY CreatedDate ASC) RowNum,leb.TransactionKey,es.[Description] ErrorName,NULL MonitoredStatus
                        FROM Trans_Data_Status AS leb WITH (NOLOCK)
                        LEFT JOIN Sys_ErrorStatus AS es ON leb.ErrorStatus=es.ErrorStatus
                        WHERE 
                         es.[Description] IN ('Error','Integration Error')   
                        AND EXISTS (SELECT 1 FROM @MasterData AS md WHERE leb.TransactionKey=md.TransactionKey AND leb.CreatedDate=md.CreatedDate)
                       

                        SELECT *
                        FROM @ReturnTable
                        
                    ";
                var dataReturn = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.DashboardTransaction_TableModels.FromSqlRaw(query).ToListAsync(),CancellationToken.None);
                return dataReturn;
            }
            catch (Exception ex)
            {
                return new List<DashboardTransaction_TableModel>();
            }
        }
        public async Task<List<DashboardTransaction_ColumnChartModel>> DashboardTransaction_ColumnChart(int environmentID, string cipFlow, string lastest)
        {
            try
            {
                var query = string.Empty;
                query = @" DECLARE @MasterData AS TABLE (RequestID uniqueidentifier,Error INT,IntegrationError INT)
                       DECLARE @ReturnData AS TABLE (RequestID uniqueidentifier,ErrorStatus NVARCHAR(128),NumberOfTransactions INT,[Index] INT)
                       DECLARE @EnvironmentID INT=" + environmentID + ",@CIPFlow CHAR(1)='" + cipFlow + "',@LastestRequestID uniqueidentifier";

                query += @"
                        SELECT TOP 1 @LastestRequestID= tb.RequestID
                        FROM Trans_Data_Summary AS tb WITH (NOLOCK)
                        WHERE tb.[Status]=1
                        AND CONVERT(DATE,CreatedDate)=CONVERT(DATE,GETDATE())
                        AND tb.CIPFolow=@CIPFlow
                        AND tb.EnvironmentID=@EnvironmentID
                        ORDER BY RequestTime DESC

                        INSERT INTO @MasterData
                        SELECT RequestID,ErrorNumbers,IntergrationErrorNumbers
                        FROM Trans_Data_Summary AS tb WITH (NOLOCK)
                        WHERE tb.RequestID=@LastestRequestID

                        INSERT INTO @ReturnData
                        SELECT RequestID,ErrorStatus,NumberOfTransactions,1 [Index]
                        FROM (
	                          SELECT *
	                          FROM @MasterData
	                          ) r
                        UNPIVOT(
	                        NumberOfTransactions FOR ErrorStatus IN (Error,IntegrationError)
                        ) AS unpv

                        --INSERT INTO @ReturnData
                        --SELECT @LastestRequestID,'Total',SUM(Quantity),0
                        --FROM @ReturnData 

                        SELECT RequestID,ErrorStatus,NumberOfTransactions
                        FROM @ReturnData
                        ORDER BY [Index] ASC";
                var dataReturn = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.DashboardTransaction_ColumnChartModels.FromSqlRaw(query).ToListAsync(),CancellationToken.None);
                return dataReturn;
            }
            catch (Exception ex)
            {
                return new List<DashboardTransaction_ColumnChartModel>();
            }
        }
        public async Task<List<DashboardTrasaction_PendingGraphModel>> DashboardTransaction_PendingGraph(int environmentID, string cipFlow, string lastest, int topSelect)
        {
            try
            {
                var query = string.Empty;
                query = @"DECLARE @MaserData AS TABLE (ID uniqueidentifier)
                      DECLARE @EnvironmentID INT=" + environmentID + ",@CIPFlow CHAR(1)='" + cipFlow + "',@LastestRequestID uniqueidentifier ";

                query += @"INSERT INTO @MaserData
                    SELECT TOP (" + topSelect + @") tb.ID
                    FROM Trans_Data_Summary AS tb WITH (NOLOCK)
                    WHERE [Status]=1
                    AND CONVERT(DATE,CreatedDate)=CONVERT(DATE,GETDATE())
                    AND EnvironmentID=@EnvironmentID
                    AND CIPFolow=@CIPFlow
                    ORDER BY tb.CreatedDate DESC

                    SELECT CAST(FORMAT(tb.CreatedDate,'HH:mm:ss') AS VARCHAR) Label,tb.PendingTransactions Data
                    FROM Trans_Data_Summary AS tb WITH (NOLOCK)
                    WHERE EXISTS (SELECT 1 FROM @MaserData AS mt WHERE mt.ID=tb.ID)
                    ORDER BY tb.CreatedDate ASC
             ";
                var dataReturn = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.DashboardTrasaction_PendingGraphModels.FromSqlRaw(query).ToListAsync(),CancellationToken.None);
                return dataReturn;
            }
            catch (Exception ex)
            {
                return new List<DashboardTrasaction_PendingGraphModel>();
            }
        }

        public async Task<int?> DashboardTransaction_ThresholdPendingGraph(int environmentID)
        {
            try
            {
                var query = await DBRetryHelper.Default.ExecuteAsync(() => (from str in _monitoringContext.Sys_Threshold_Rules
                             join sns in _monitoringContext.Sys_Node_Settings on str.Node_Setting equals sns.ID
                             where sns.EnvironmentID == environmentID && sns.IsActive == true && str.MonitoringType == (int)MonitoringType.PendingTransaction
                             select str).FirstOrDefaultAsync(),CancellationToken.None);
                return query?.Threshold ?? 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<List<EnvironmentModel>> Environments_ByNodeType(int? nodeType)
        {
            try
            {
                var query = await DBRetryHelper.Default.ExecuteAsync(() => (from sia in _monitoringContext.Sys_Integration_APIs
                                                  join se in _monitoringContext.Sys_Environments on sia.EnvironmentID equals se.ID
                                                  where sia.IsActive == true && (nodeType == null || sia.NodeType == nodeType)
                                                  select new EnvironmentModel
                                                  {
                                                      ID = se.ID,
                                                      Comment = se.Comment,
                                                      Name = se.Name,
                                                      ListIntegrationAPI = new List<IntegrationAPI>()
                                                  }).ToListAsync(),CancellationToken.None);
                return query;
            }
            catch (Exception ex)
            {
                return new List<EnvironmentModel>();
            }
        }
    }
}
