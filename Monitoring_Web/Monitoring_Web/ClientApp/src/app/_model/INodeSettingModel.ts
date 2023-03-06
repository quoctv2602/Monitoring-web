export interface INodeSettingModel {
  id: number;
  nodeName: string;
  environmentID: number;
  environmentName: string;
  description: string;
  serviceList: string;
  notificationEmail: string;
  reportEmail: string;
}
