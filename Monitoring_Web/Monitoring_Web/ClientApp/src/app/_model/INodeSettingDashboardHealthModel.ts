import { INodeSettingModel } from './INodeSettingModel';
export interface INodeSettingDashboardHealthModel extends INodeSettingModel {
  selecting?: boolean;
  utilization: string;
  requestID: string;
  nodeType: number;
  isDefault?: boolean;
}
