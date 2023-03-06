export interface NodeSettingRequest {
    nodeType: number;
    nodeName: string | null;
    environmentID: number;
    description: string | null;
    serviceList: string | null;
    notificationEmail: string | null;
    reportEmail: string | null;
    notificationAlias: string | null;
    reportAlias: string | null;
    domain_SystemHealth: string | null;
    appid: string | null;
    healthMeasurementKey: string | null;
    listThresholdRuleRequest: ThresholdRuleRequest[];
}

export interface ThresholdRuleRequest {
    monitoringType: number;
    monitoringName: string | null;
    condition: number | null;
    threshold: number | null;
    thresholdCounter: number | null;
    unit: string | null;
}

export interface monitoringType {
    value: number;
    name: string | null;
}
export interface settingsRequest {
    NodeName: string;
    IsActive: string;
    PageIndex:number;
    PageSize:number;
}