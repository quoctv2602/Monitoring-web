export interface IntegrationAPI {
    nodeID: number | null;
    nodeName: string | null;
}export interface CreateIntegrationAPIRequest {
    environmentID: number;
    machineName: | null;
    healthMeasurementKey: string | null;
    appid: string | null;
    domain_SystemHealth: string | null;
    nodeType: number;
    isActive: boolean | null;
    serviceList: string | null;
    isDefaultNode: boolean | null;
}export interface UpdateIntegrationAPIRequest {
    id: number;
    environmentId: number;
    environmentName: string| null;
    machineName: string;
    healthMeasurementKey: string | null;
    appid: string | null;
    domain_SystemHealth: string | null;
    isActive: boolean | null;
    createDate: string | null;
    isCheck:boolean | null;
}export interface IntegrationAPIRequest {
    machineName: string;
    isActive: string;
    PageIndex: number;
    PageSize: number;
}