export enum ServiceStatusEnum {
  Running = 'Running',
  Stopped = 'Stopped',
  Nothing = 'Nothing',
  Paused = 'Paused',
  Starting = 'Starting',
  Default = 'Status Changing',
}

export enum MonitoringType {
  CPU = 1,
  RAM = 2,
  Storage = 3,
  ProcessTime = 4,
  FreeDisk = 5,
  SharedStorageRunningTime = 6,
}

export enum DashboardType {
  TransactionBased = 1,
  SystemHealth = 2,
}

export enum NodeType {
  TransactionBased = 1,
  SystemHealth = 2,
}

export enum MonitoredStatus {
  Resolved = 1,
  UnResolved = 2,
  Informed = 3,
}

export enum DirectionTransaction {
  Inbound = 67,
}

export enum UserType {
  Admin = 1,
  Normal = 2,
}

export enum SaveGroupResult {
  Exist = -2,
}

export enum SaveUserResult {
  Exist = -2,
}

export enum ActionEnum {
  ///KPI Settings
  kpiSettingsManageNode = 12,
  kpiSettingsSetKPI = 13,
  kpiSettingsUpdateKPI = 14,
  kpiSettingsEnableDisableKPI = 15,
  kpiSettingsImportExportKPI = 16,

  //Dashboards
  dashboardViewSystemBased = 21,
  dashboardViewTransactionBased = 22,

  //Transactions
  transactionsViewLog = 31,
  transactionsViewDataContent = 32,
  transactionsViewConfig = 33,
  transactionsMonitoringAction = 34,
  transactionsNotes = 35,

  //UserPermission
  userPermissionManageGroup = 42,
  userPermissionAssignPermission = 43,
  userPermissionSetDefaultGroup = 44,

  //Notification Setup
  notificationSettingsManage = 52,
  notificationSettingsOnOff = 53,

  // User
  userManage = 46,
}
