export interface IntegrationAPIModel {
    id: number;
    environmentId:number;
    environmentName: string | null;
    machineName: string | null;
    healthMeasurementKey: string | null;
    appid: string | null;
    domain_SystemHealth: string | null;
    nodeType: number;
    isActive: boolean | null;
    createDate: string | null;
    isCheck: boolean | null;
    isDefaultNode: boolean | null;
}export interface ListIntegrationAPI {
    pageIndex: number;
    pageSize: number;
    totalRecords: number;
    pageCount: number;
    listItem: IntegrationAPIModel[];
}
export interface NodeType{
    id: number;
    nodeType: string | null;
    description: string | null;
}export interface IntegrationAPIModelEdit {
    id: number;
    environmentID: number | null;
    machineName: string | null;
    healthMeasurementKey: string | null;
    appid: string | null;
    domain_SystemHealth: string | null;
    nodeType: number ;
    nodeTypeName: string | null;
    isActive: boolean | null;
    createDate: string | null;
    serviceList: string | null;
    isDefaultNode: boolean | null;
}