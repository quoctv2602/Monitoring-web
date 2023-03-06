import { IUserProfileModel } from './IUserProfileModel';

export interface IGroupModel {
  groupId?: number;
  groupName?: string;
  groupDescription?: string;
  members: IUserProfileModel[];
  totalMembers?: number;
  isDefault?: boolean;
  isSelect?: boolean;
  totalRows?: number;
}
