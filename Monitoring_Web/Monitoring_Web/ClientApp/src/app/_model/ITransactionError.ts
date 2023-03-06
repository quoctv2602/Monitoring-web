export interface ITransactionError {
  transactionKey: string;
  totalOfDocs: number;
  docType: string;
  document: string;
  startDate: string;
  endDate: string;
  senderCustId:number;
  senderCustName: string;
  receiverCustId:number;
  receiverCustName: string;
  errorStatus: string;
  totalRows: number;
  rowsNumber: number;
  errorStatusString: string;
  monitoredStatusString: string;
  serverFileId: number;
  rowID: number;
  direction: number;
  referToTK:string;
}
