import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { globalsettings } from 'src/assets/globalsetting';
import { IGroupFilterRequestModel } from '../_model/IGroupFilterRequestModel';
import { IGroupModel } from '../_model/IGroupModel';
@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private apiUrl = globalsettings.apiUrl;

  private controllerName: string = 'group';

  constructor(private httpClient: HttpClient) {}

  getGroups(filterModel: IGroupFilterRequestModel) {
    return this.httpClient.post<IGroupModel[]>(
      this.apiUrl + this.controllerName + '/GetGroups',
      filterModel
    );
  }

  getById(id: number) {
    return this.httpClient.get<IGroupModel>(
      this.apiUrl + this.controllerName + '/ById?id=' + id
    );
  }

  saveGroup(saveModel: IGroupModel) {
    return this.httpClient.post<number>(
      this.apiUrl + this.controllerName + '/Save',
      saveModel
    );
  }

  changeDefault(changeModel: IGroupModel) {
    return this.httpClient.post<number>(
      this.apiUrl + this.controllerName + '/ChangeDefault',
      changeModel
    );
  }

  delete(deleteModels: IGroupModel[]) {
    return this.httpClient.post<number>(
      this.apiUrl + this.controllerName + '/Delete',
      deleteModels
    );
  }
}
