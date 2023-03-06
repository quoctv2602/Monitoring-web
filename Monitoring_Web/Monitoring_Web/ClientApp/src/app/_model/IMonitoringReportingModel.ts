import { ITransactionError } from './ITransactionError';

export interface IMonitoringReportingModel extends ITransactionError {
  reProcessed: number;
  monitoredStatus: number;
  isSelect: boolean;
  note?: string | null;
  environmentName: string;
  environmentID: number;
}
