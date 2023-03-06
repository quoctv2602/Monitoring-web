import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { globalsettings } from 'src/assets/globalsetting';
import { IntegrationAPIModel, IntegrationAPIModelEdit, ListIntegrationAPI, NodeType } from '../_model/integration-api-model';
import { ApiResponse } from '../_model/node-settings-model';
import { SysEnvironment } from '../_model/sys-environment';
import { CreateIntegrationAPIRequest, IntegrationAPIRequest, UpdateIntegrationAPIRequest } from '../_model/sys-integration-api';

@Injectable({
  providedIn: 'root'
})
export class IntegrationService {
  baseUrl = globalsettings.apiUrl;
constructor(private http: HttpClient) { }
getEnvironment() {
  return this.http
    .get<SysEnvironment[]>(this.baseUrl + 'IntegrationAPI/getEnvironment')
    .pipe(
      map((res: SysEnvironment[]) => {
        return res;
      })
    );
  }
  createIntegrationAPI(createIntegrationAPIRequest: CreateIntegrationAPIRequest) {
  return this.http
    .post<ApiResponse>(
      this.baseUrl + 'IntegrationAPI/createIntegrationAPI',
      createIntegrationAPIRequest
    )
    .pipe(
      map((res) => {
        return res;
      })
    );
  }
  updateIntegrationAPI(updateIntegrationAPIRequest: IntegrationAPIModelEdit) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'IntegrationAPI/updateIntegrationAPI',
        updateIntegrationAPIRequest
      )
      .pipe(
        map((res) => {
          return res;
        })
      );
    }
    getListIntegrationAPI(integrationAPIRequest: IntegrationAPIRequest) {
      return this.http
        .post<ListIntegrationAPI>(
          this.baseUrl + 'IntegrationAPI/getListIntegrationAPI',
          integrationAPIRequest
        )
        .pipe(
          map((res: ListIntegrationAPI) => {
            return res;
          })
        );
    }
    getIntegrationAPIEdit(id: number) {
      return this.http
        .post<IntegrationAPIModelEdit>(
          this.baseUrl + 'IntegrationAPI/getIntegrationAPIEdit',
          id
        )
        .pipe(
          map((res: IntegrationAPIModelEdit) => {
            return res;
          })
        );
    }
    deleteIntegrationAPI(integrationAPIRequest: IntegrationAPIModel[]) {
      return this.http
        .post<ApiResponse>(
          this.baseUrl + 'IntegrationAPI/deleteIntegrationAPI',
          integrationAPIRequest
        )
        .pipe(
          map((res: ApiResponse) => {
            return res;
          })
        );
    }
   
    getNodeType() {
      return this.http
        .get<NodeType[]>(this.baseUrl + 'IntegrationAPI/getNodeType')
        .pipe(
          map((res: NodeType[]) => {
            return res;
          })
        );
      }
      checkEndPoint(url: string) {
        return this.http
          .post<ApiResponse>(
            this.baseUrl + 'IntegrationAPI/checkEndPoint',
            {},
            {params: {url: url}}
            
          )
          .pipe(
            map((res: ApiResponse) => {
              return res;
            })
          );
      }
}
