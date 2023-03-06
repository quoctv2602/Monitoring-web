export interface IDashboardSystemHealthRequest {
  nodeSettings: {
    ID: number;
    environmentID: number;
    nodeName: string;
    environmentName: string;
  }[];
  monitoringType: number;
  connectionId: string;
}
