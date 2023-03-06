import { IDashboardTransactionRequest } from './IDashboardTransactionRequest';
import { IDashboardSystemHealthRequest } from './IDashboardSystemHealthRequest';
export interface IDashboardRequest {
  systemHealthRequest: IDashboardSystemHealthRequest | null;
  transactionRequest: IDashboardTransactionRequest | null;
  connectionId: string;
  dashboardType: number;
}
