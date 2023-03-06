export interface IDataExportExcelModel {
  note?: string | null;
  environment?: string | null;
  transactionKey?: string | null;
  totalOfDocs?: number;
  docType?: string | null;
  document?: string | null;
  startDate?: Date;
  endDate?: Date;
  sender?: string | null;
  receiver?: string | null;
  errorStatus?: string | null;
}
