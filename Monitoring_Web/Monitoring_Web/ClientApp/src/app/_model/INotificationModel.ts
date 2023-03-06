export interface INotificationModel {
    pageIndex: number;
    pageSize: number;
    totalRecords: number;
    pageCount: number;
    listItem: NotificationModel[];
}export interface NotificationAddRequest {
    name: string;
    notificationOption: number | null;
    kPI: Monitoring[];
    emails: string;
    notificationAlias: string;
    createdBy: number | null;
}
export interface Monitoring {
    monitoringId: number | null;
}
export interface NotificationListRequest {
    name: string | null;
    pageIndex: number;
    pageSize: number;
}
export interface NotificationModel {
    id: number;
    name: string;
    kpi: ListKPI[];
    notificationOption: number | null;
    emails: string;
    notificationAlias: string | null;
    isActive: boolean | null;
    isCheck: boolean;
}
export interface ListKPI {
    kpiName: string;
}export interface NotificationEditRequest {
    id: number;
    name: string;
    notificationOption: number | null;
    kpi: MonitoringEdit[];
    emails: string;
    notificationAlias: string;
    updateBy: number | null;
}

export interface MonitoringEdit {
    id: number | null;
    monitoringId: number | null;
}export interface ToggleNotificationRequest {
    id: number;
    isActive: boolean;
}