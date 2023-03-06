import { IDefault_TransactionBaseModel } from './IDefault_TransactionBaseModel';

export interface IReportbyTransactionModel
  extends IDefault_TransactionBaseModel {
  status: string;
  isShowAllDoc: boolean;
  docID: string;
}
