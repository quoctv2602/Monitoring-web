namespace Monitoring_Common
{
    public class Enum
    {
        public enum MonitoringType
        {
            CPU = 1,
            RAM = 2,
            Storage = 3,
            ProcessTime_EDItoASCII = 4,
            FreeDisk = 5,
            SharedStorageRunningTime = 6,
            FailedTransaction = 7,
            IntergrationErrorTransaction = 8,
            PendingTransaction = 9
        }
        public enum DashboardType
        {
            TransactionBased = 1,
            SystemHeath = 2
        }
        public enum MonitoredStatus
        {
            Resolved = 1,
            Un_Resolved = 2,
            Informed = 3
        }
        public enum ErrorStatus
        {
            Success = 69,
            Error = 70,
            Pending = 71,
            UnrecognizedFormat = 152,
            Warning = 157,
            IntegrationWarning = 179,
            IntegrationError = 180
        }
        public enum TransactionLogStatus
        {
            Both = -1,
            Succeeded = 0,
            Failed = 1,
            Warning = 2
        }

        public enum ResponseStatus
        {
            Success = 1,
            Error = 0
        }

        public enum UserTypeEnum
        {
            Admin = 1,
            Normal = 2
        }
        public enum UseCIPEnum
        {
            Batch = 0,
            Partial = 1,
            CIP = 2
        }

        public enum ActionEnum
        {
            #region KPI Settings
            kpiSettingsManageNode = 12,
            kpiSettingsSetKPI = 13,
            kpiSettingsUpdateKPI = 14,
            kpiSettingsEnableDisableKPI = 15,
            kpiSettingsImportExportKPI = 16,
            #endregion

            #region Dashboards
            dashboardViewSystemBased = 21,
            dashboardViewTransactionBased = 22,
            #endregion

            #region Transactions
            transactionsViewLog = 31,
            transactionsViewDataContent = 32,
            transactionsViewConfig = 33,
            transactionsMonitoringAction = 34,
            transactionsNotes = 35,
            #endregion

            #region UserPermission
            userPermissionManageGroup = 42,
            userPermissionAssignPermission = 43,
            userPermissionSetDefaultGroup = 44,
            // User
            userManage = 46,
            #endregion

            #region Notification Setup
            notificationSettingsManage = 52,
            notificationSettingsOnOff = 53,
            #endregion
        }
    }
}