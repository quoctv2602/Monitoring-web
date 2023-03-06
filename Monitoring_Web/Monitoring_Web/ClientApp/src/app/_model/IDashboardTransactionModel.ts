import { IDashboardTransaction_ColumnChartModel } from './IDashboardTransaction-ColumnChartModel';
import { IDashboardTrasaction_PendingGraphModel } from './IDashboardTransaction-PendingGraphModel';
import { IDashboardTransaction_TableModel } from './IDashboardTransaction-TableModel';

export interface IDashboardTransactionModel {
  tableData: IDashboardTransaction_TableModel[];
  columnChartData: IDashboardTransaction_ColumnChartModel[];
  pendingGraphData: IDashboardTrasaction_PendingGraphModel[];
  thresholdPendingGraph: number | null;
}
