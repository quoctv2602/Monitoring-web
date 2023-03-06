import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { globalsettings } from 'src/assets/globalsetting';
import { IUserProfileFilterModel } from '../_model/IUserProfileFilterRequestModel';
import { IUserProfileModel } from '../_model/IUserProfileModel';
@Injectable({
  providedIn: 'root',
})
export class UserProfileService {
  private apiUrl = globalsettings.apiUrl;

  private controllerName: string = 'userprofile';

  constructor(private httpClient: HttpClient) {}

  getUsers(filterModel: IUserProfileFilterModel) {
    return this.httpClient.post<IUserProfileModel[]>(
      this.apiUrl + this.controllerName + '/UserList',
      filterModel
    );
  }

  getById(id: number) {
    return this.httpClient.get<IUserProfileModel>(
      this.apiUrl + this.controllerName + '/ById?id=' + id
    );
  }

  save(saveModel: IUserProfileModel) {
    return this.httpClient.post<number>(
      this.apiUrl + this.controllerName + '/Save',
      saveModel
    );
  }

  delete(deleteModels: IUserProfileModel) {
    return this.httpClient.post<number>(
      this.apiUrl + this.controllerName + '/Delete',
      deleteModels
    );
  }
}
