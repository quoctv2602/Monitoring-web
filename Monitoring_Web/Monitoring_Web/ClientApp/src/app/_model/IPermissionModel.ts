import { IActionModel } from './IActionModel';

export interface IPermissionModel {
  pageId: number;
  pageName: string | null;
  actions: IActionModel[];
  isSelected: boolean;
}
