import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { globalsettings } from 'src/assets/globalsetting';
import {
  NodeSettingRequest,
  settingsRequest,
} from '../_model/node-setting-request';
import {
  NodeSettingsEdit,
  ThresholdRuleEdit,
} from '../_model/node-settings-edit';
import { ApiResponse, NodeSetingsModel } from '../_model/node-settings-model';
import { SysMonitoring } from '../_model/sys-monitoring';
import { NodeSettings, PagedResultBase } from '../_model/sys-settings';

@Injectable({
  providedIn: 'root',
})
export class NodeSettingsService {
  baseUrl = globalsettings.apiUrl;

  constructor(private http: HttpClient) {}

  getSysMonitoring() {
    return this.http
      .get<SysMonitoring[]>(this.baseUrl + 'NodeSetting/getSysMonitoring')
      .pipe(
        map((res: SysMonitoring[]) => {
          return res;
        })
      );
  }

  getSysEnvironment() {
    return this.http
      .get<NodeSetingsModel>(this.baseUrl + 'NodeSetting/getSysEnvironment')
      .pipe(
        map((res: NodeSetingsModel) => {
          return res;
        })
      );
  }

  createNodeSetting(nodeSettingRequest: NodeSettingRequest) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'NodeSetting/createNodeSetting',
        nodeSettingRequest
      )
      .pipe(
        map((res) => {
          return res;
        })
      );
  }
  updateNodeSetting(nodeSettingRequest: NodeSettingsEdit) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'NodeSetting/updateNodeSetting',
        nodeSettingRequest
      )
      .pipe(
        map((res) => {
          return res;
        })
      );
  }

  getSysNodeSetting(settingsRequest: settingsRequest) {
    return this.http
      .post<PagedResultBase>(
        this.baseUrl + 'NodeSetting/getListSysNodeSetting',
        settingsRequest
      )
      .pipe(
        map((res: PagedResultBase) => {
          return res;
        })
      );
  }
  getNodeSettingsEdit(id: number) {
    return this.http
      .post<NodeSettingsEdit>(
        this.baseUrl + 'NodeSetting/getSysNodeSetting',
        id
      )
      .pipe(
        map((res: NodeSettingsEdit) => {
          return res;
        })
      );
  }
  getThresholdRuleEdit(id: number) {
    return this.http
      .post<ThresholdRuleEdit[]>(
        this.baseUrl + 'NodeSetting/getSysThresholdRule',
        id
      )
      .pipe(
        map((res: ThresholdRuleEdit[]) => {
          return res;
        })
      );
  }
  updateIsActive(isActiveNodeRequest: NodeSettings) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'NodeSetting/updateIsActiveNode',
        isActiveNodeRequest
      )
      .pipe(
        map((res) => {
          return res;
        })
      );
  }
  exportFileJson(nodeSettingId: string) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'NodeSetting/exportFileJson',
        {},
        {params: {nodeSettingId: nodeSettingId}}
      )
      .pipe(
        map((res) => {
          var sJson = JSON.stringify(res);
          var element = document.createElement('a');
          element.setAttribute(
            'href',
            'data:text/json;charset=UTF-8,' + encodeURIComponent(sJson)
          );
          const currentTime = new Date();
          const filename =
            'FilejsonExport_Node_Mornitoring' +
            currentTime.getFullYear().toString() +
            (currentTime.getMonth() + 1) +
            currentTime.getDate() +
            currentTime
              .toLocaleTimeString()
              .replace(/[ ]|[,]|[:]/g, '')
              .trim() +
            '.json';
          element.setAttribute('download', filename);
          element.style.display = 'none';
          document.body.appendChild(element);
          element.click();
          return res;
        })
      );
  }
  importFileJson(fileImportJson: any) {
    const formData = new FormData();
    formData.append('files', fileImportJson);
    return this.http.post<ApiResponse>(
      this.baseUrl + 'NodeSetting/importFileJson',
      formData
    );
  }
}
