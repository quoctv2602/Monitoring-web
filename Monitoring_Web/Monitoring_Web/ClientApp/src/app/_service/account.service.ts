import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { globalsettings } from 'src/assets/globalsetting';
import { IUserLoginModel } from '../_model/IUserLoginModel';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private apiUrl = globalsettings.apiUrl;

  private controllerName: string = 'account';
  constructor(private httpClient: HttpClient) {}

  loginSuccess() {
    return this.httpClient.get<IUserLoginModel>(
      this.apiUrl + this.controllerName + '/OnLoginSuccess'
    );
  }

  logOut() {
    return this.httpClient.get<number>(
      this.apiUrl + this.controllerName + '/LogOut'
    );
  }
}
