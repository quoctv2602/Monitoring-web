export interface IViewConfigurationModel {
  cipConfiguration: {
    senderMailboxList: IMailbox[];
    receiverMailboxList: IMailbox[];
  };
  senderCustomer:SenderCustomer;
  receiverCustomer:ReceiverCustomer;
}

export interface IMailbox {
  fileTransferUserID: string | null;
  mailboxName: string | null;
  mailboxHomeDir: string | null;
  fileTransferType: string | null;
  clientURL: string | null;
  clientConnectionProtocol: string | null;
}export interface SenderCustomer {
  customerID: number | null;
  customerName: string;
  isaid: string;
  gsIn: string;
  gsOut: string;
  qualifier: string;
  customerRanking: string;
  customerOwner: string;
  siteID: string;
  useCIP: string;
  useCIPName: string| null;
  downloadEDI_YN: string;
  downloadASCII_YN: string;
  useASCIIR9_YN: string;
  useDiMetrics_YN: string;
}
export interface ReceiverCustomer {
  customerID: number | null;
  customerName: string;
  isaid: string;
  gsIn: string;
  gsOut: string;
  qualifier: string;
  customerRanking: string;
  customerOwner: string;
  siteID: string;
  useCIP: string;
  useCIPName: string| null;
  downloadEDI_YN: string;
  downloadASCII_YN: string;
  useASCIIR9_YN: string;
  useDiMetrics_YN: string;
}