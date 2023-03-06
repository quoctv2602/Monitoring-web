export interface NodeSettings {
    id: number;
    nodeName: string | null;
    environmentID: number;
    environmentName: string | null;
    machineName: string;
    description: string | null;
    serviceList: string | null;
    notificationEmail: string | null;
    reportEmail: string | null;
    createDate: string | null;
    isActive: boolean | null;
    nodeType: number | null;
    nodeTypeName: string | null;
    isCheck: boolean | null;
}
export interface PagedResultBase {
    pageIndex: number;
    pageSize: number;
    totalRecords: number;
    pageCount: number;
    listItem: NodeSettings[];
}