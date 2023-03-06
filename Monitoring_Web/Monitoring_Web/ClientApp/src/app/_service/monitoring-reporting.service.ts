import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { globalsettings } from 'src/assets/globalsetting';
import {
  ICIPReportingModel,
  ICIPTransactionKeyModel,
} from '../_model/ICIPReportingModel';
import { IDataExportExcelModel } from '../_model/IDataExportExcelModel';
import { IMonitoringReportingModel } from '../_model/IMonitoringReportingModel';
import { ITransDataIntegrationModel } from '../_model/ITransDataIntegrationModel';
import { IViewConfigurationRequestModel } from '../_model/IViewConfigurationRequestModel';
import { IDownloadFileRequestModel, IViewDataRequestModel } from '../_model/IViewDataRequestModel';
import { IViewLogsRequestModel } from '../_model/IViewLogsRequestModel';
import { ApiResponse } from '../_model/node-settings-model';

@Injectable({
  providedIn: 'root',
})
export class MonitoringReportingService {
  private apiUrl = globalsettings.apiUrl;

  private controllerName = 'TransactionBase';

  constructor(private http: HttpClient) {}

  GetTransactionList(
    requestBody: ICIPReportingModel,
    envrionmentID?: number | null
  ) {
    return this.http.post<{
      message: string;
      data: IMonitoringReportingModel[];
    }>(
      this.apiUrl +
        this.controllerName +
        '/GetCIPReporting?EnvironmentID=' +
        envrionmentID,
      requestBody
    );
  }

  markTransactions(
    requestBody: ITransDataIntegrationModel[],
    monitoredStatus?: number | null
  ) {
    if (monitoredStatus)
      return this.http.post<ApiResponse>(
        this.apiUrl +
          this.controllerName +
          '/createTransDataIntegration?MonitoredStatus=' +
          monitoredStatus,
        requestBody
      );
    else {
      return this.http.post<ApiResponse>(
        this.apiUrl + this.controllerName + '/createTransDataIntegration',
        requestBody
      );
    }
  }

  exportReporting(requestBody: IDataExportExcelModel[]) {
    return this.http
      .post(
        this.apiUrl + this.controllerName + '/exportFileExcel',
        requestBody,
        { observe: 'response', responseType: 'blob' }
      )
      .pipe(
        map((res) => {
          if (res.body != null) {
            const filename = res.headers
              ?.get('Content-Disposition')
              ?.split('filename=')[1]
              .split(';')[0];
            var blob = new Blob([res.body], {
              type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,',
            });
            var url = window.URL.createObjectURL(blob);
            var anchor = document.createElement('a');
            anchor.download = filename ?? 'FileExcelExport_FromMornitoring';
            anchor.href = url;
            anchor.click();
          }
        })
      );
  }
  GetTransactionKey(
    requestBody: ICIPTransactionKeyModel,
    envrionmentID?: number | null
  ) {
    return this.http.post<{
      message: string;
      data: IMonitoringReportingModel[];
    }>(
      this.apiUrl +
        this.controllerName +
        '/GetReportByTransactionKey?EnvironmentID=' +
        envrionmentID,
      requestBody
    );
  }

  viewLogsByTransactionKey(
    requestBody: IViewLogsRequestModel,
    environmentID?: number | null
  ) {
    return this.http.post<any>(
      this.apiUrl +
        this.controllerName +
        '/GetViewLogs?EnvironmentID=' +
        environmentID,
      requestBody
    );
  }
  viewDataServerFile(
    requestBody: IViewDataRequestModel,
    environmentID?: number | null
  ) {
    return this.http.post<any>(
      this.apiUrl +
        this.controllerName +
        '/GetContentView?EnvironmentID=' +
        environmentID,
      requestBody
    );
  }

  viewConfigByTransactionKey(
    requestBody: IViewConfigurationRequestModel,
    environmentID?: number | null
  ) {
    return this.http.post<any>(
      this.apiUrl +
        this.controllerName +
        '/ViewCIPConfiguration?EnvironmentID=' +
        environmentID,
      requestBody
    );
  }
  downloadFileContent(requestBody: IDownloadFileRequestModel,environmentID?: number | null) {
    return this.http
      .post(
        this.apiUrl + this.controllerName + '/DownloadFileContent?EnvironmentID='+environmentID,
        requestBody,
        { observe: 'response', responseType: 'blob' }
      )
      .pipe(
        map((res) => {
          if (res.body != null) {
            const filename = res.headers
              ?.get('Content-Disposition')
              ?.split('filename=')[1]
              .split(';')[0];
            var blob = new Blob([res.body], {
              type: 'application/octet-stream;base64,',
            });
            var url = window.URL.createObjectURL(blob);
            var anchor = document.createElement('a');
            anchor.download = filename ?? 'DownloadFileContent_FromMornitoring';
            anchor.href = url;
            anchor.click();
          }
        })
      );
  }
}
