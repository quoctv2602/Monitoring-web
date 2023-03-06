export interface IDashboardSystemHealthByFreeDiskModel {
  driveName: string;
  environmentName: string;
  environmentID: number;
  machineName: string;
  percentFreeSpace: number;
  percentUsedSpace: number;
  threshold: number;
  totalFreeSpace: number;
  totalSize: number;
  totalUsedSpace: number;
  volumeLabel: string;
  requestID: string;
}
