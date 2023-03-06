export interface IUserProfileModel {
  id: number;
  email?: string;
  userType?: number;
  groupId?: number;
  isSelect?: boolean;
  userTypeName?: string;
  groupName?: string;
  totalRows?: number;
  isDelete?: boolean;
}
