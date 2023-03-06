export interface IViewLogsModel {
  requestID: string | null;
  transactionLogID: string | null;
  transactionKey: string;
  date: string;
  actionName: string | null;
  status: string | null;
  description: string | null;
  errorCodeID: string | null;
  totalRows: number | null;
  statusString: string | null;
}
