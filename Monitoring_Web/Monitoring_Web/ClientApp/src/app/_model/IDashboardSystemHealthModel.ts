export interface IDashboardSystemHealthModel {
  nodeID: number;
  environmentID: number;
  environmentName: string;
  machineName: string;
  data: number;
  label: string;
  requestID: string;
  dateString: string;
  threshold: number;
}
