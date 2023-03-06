export interface IMonitoringReportingAdvanceModel {
  note: string | null;
  environmentName: string;
  custPrority: string;
  transactionKey: string;
  totalOfDocs: number;
  docType: string;
  document: string;
  startDate: string;
  endDate: string;
  sender: string;
  receiver: string;
  errorStatus: string | null;
  errorCodes: string | null;
  reProcessed: string;
  monitoredStatus: string;
  isSelect: boolean;
}
