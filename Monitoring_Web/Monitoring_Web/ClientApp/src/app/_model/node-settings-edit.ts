export interface ThresholdRuleEdit {
    id: number;
    node_Setting: number;
    environmentID: number;
    machineName: string;
    monitoringType: number;
    monitoringName: string | null;
    condition: number | null;
    threshold: number | null;
    thresholdCounter: number | null;
    createDate: string | null;
    unit: string | null;
}export interface NodeSettingsEdit {
    id: number;
    nodeName: string | null;
    environmentID: number;
    machineName: string| null;
    description: string | null;
    serviceList: string | null;
    notificationEmail: string | null;
    reportEmail: string | null;
    notificationAlias: string | null;
    reportAlias: string | null;
    createDate: string | null;
    isActive: boolean ;
    nodeType: number ;
    healthMeasurementKey: string | null;
    appid: string | null;
    domain_SystemHealth: string | null;
    listThresholdRuleEdit: ThresholdRuleEdit[];
}