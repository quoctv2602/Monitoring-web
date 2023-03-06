import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { globalsettings } from 'src/assets/globalsetting';
import { IDashboardRequest } from '../_model/IDashboardRequest';
import { INodeSettingDashboardHealthModel } from '../_model/INodeSettingDashboardHealthModel';
import { IServiceListRequestModel } from '../_model/IServiceListRequestModel';
import { IServiceModel } from '../_model/IServiceModel';
import { SysEnvironment } from '../_model/sys-environment';
@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private apiUrl = globalsettings.apiUrl;

  constructor(private httpClient: HttpClient) {}

  
  getListNodes() {
    return this.httpClient.get<INodeSettingDashboardHealthModel[]>(
      this.apiUrl + 'dashboard/nodes'
    );
  }

  getDataSystemHealth(dataBody: IDashboardRequest) {
    return this.httpClient.post<{ message: string; data: any }>(
      this.apiUrl + 'dashboard/SystemHealth',
      dataBody
    );
  }

  getListServices(paramBody: IServiceListRequestModel[]) {
    return this.httpClient.post<IServiceModel[]>(
      this.apiUrl + 'dashboard/ServiceList',
      paramBody
    );
  }

  getDurationSlideShowDashboard() {
    return this.httpClient.get<number>(
      this.apiUrl + 'dashboard/DurationSlideShowDashboard'
    );
  }

  getEnvironmentsListByNodeType(nodeType?: number) {
    if (nodeType == null) {
      return this.httpClient.get<SysEnvironment[]>(
        this.apiUrl + 'dashboard/getSysEnvironment'
      );
    } else {
      return this.httpClient.get<SysEnvironment[]>(
        this.apiUrl + 'dashboard/getSysEnvironment?nodeType=' + nodeType
      );
    }
  }
}
