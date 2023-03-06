import { IDefault_TransactionBaseModel } from './IDefault_TransactionBaseModel';

export interface ICIPReportingModel extends IDefault_TransactionBaseModel {
  status?: string | null;
  cIPFlow?: string | null;
  transID?: string| null;
  docID?: string | null;
  senderCustName?: string | null;
  receiverCustName?: string | null;
  senderMailboxName?: string | null;
  receiverMailboxName?: string | null;
  iSAControl?: string | null;
  gSControl?: string | null;
  senderCustPriority?: string | null;
  receiverCustPriority?: string | null;
  transactionKey?: string | null;
  environmentID?: number| null;
  platform?: string;
  document?: string | null;
}
export interface ICIPTransactionKeyModel extends IDefault_TransactionBaseModel {
  transactionKey?: string | null;
  document?: string | null;
}
