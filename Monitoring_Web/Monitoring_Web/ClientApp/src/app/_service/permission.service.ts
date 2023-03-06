import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { globalsettings } from 'src/assets/globalsetting';
import { IPermissionModel } from '../_model/IPermissionModel';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private apiUrl = globalsettings.apiUrl;

  private controllerName: string = 'permission';

  constructor(private httpClient: HttpClient) {}

  permissionByGroup(groupId: number) {
    return this.httpClient.get<IPermissionModel[]>(
      this.apiUrl +
        this.controllerName +
        '/PermissionByGroup?groupId=' +
        groupId
    );
  }

  savePermission(saveModel: {
    groupId: number;
    permissions: IPermissionModel[];
  }) {
    return this.httpClient.post<number>(
      this.apiUrl + this.controllerName + '/SavePermission',
      saveModel
    );
  }
}
