import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DirectionTransaction } from 'src/app/_common/_enum';
import { IMonitoringReportingModel } from 'src/app/_model/IMonitoringReportingModel';
import { IDownloadFileRequestModel, IViewDataRequestModel } from 'src/app/_model/IViewDataRequestModel';
import { MonitoringReportingService } from 'src/app/_service/monitoring-reporting.service';

@Component({
  selector: 'app-view-data',
  templateUrl: './view-data.component.html',
  styleUrls: ['./view-data.component.css'],
})
export class ViewDataComponent implements OnInit, OnDestroy {
  data: any = [];
  serverFileID!: number;
  environmentID!: number;
  dataDirection!: string;
  document!: string;
  isLoading: boolean = false;
  isDownload: boolean = false;
  viewDataItem!: IMonitoringReportingModel;
  localPath!: string;
  constructor(
    private _monitoringReportingService: MonitoringReportingService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.viewDataItem = sessionStorage.getItem('viewDataItem')
      ? JSON.parse(sessionStorage.getItem('viewDataItem') as string)
      : {};
    this.environmentID = this.viewDataItem.environmentID;
    this.serverFileID = this.viewDataItem.rowID;
    this.document = this.viewDataItem.document;
    let dataDirection = this.viewDataItem.direction;
    if (dataDirection == DirectionTransaction.Inbound) {
      this.dataDirection = 'Inbound';
    } else {
      this.dataDirection = 'Outbound';
    }
    this.handleGetData();
  }
  ngOnDestroy(): void {
    sessionStorage.removeItem('viewDataItem');
  }
  checkJSON(str:string) {
    try {
        return (JSON.parse(str) && !!str);
    } catch (e) {
        return false;
    }
}
  handleGetData() {
    this.isLoading = true;
    const requestBody: IViewDataRequestModel = {
      serverFileID: this.serverFileID,
    };
    this._monitoringReportingService
      .viewDataServerFile(requestBody, this.environmentID)
      .subscribe((respone) => {
        this.isLoading = false;
        const status = respone.data.status;
        const message = respone.data.message;
        const data = respone.data.data;
        if (status === 1) {
          if (data == null || data == '') {
            this.data = {isFile:0,fileContent:''};
          } else {
            this.data = data;
            let isFile=this.data.isFile;
            if(isFile!=0){
              this.localPath=this.data.fileContent;
              this.data.fileContent='File content is too large, please download into your local.'
            }
          }
        } else {
          if (message && message.length > 0) {
            this.data = {isFile:0,fileContent:''};
            this.toastr.error(message);
          }
        }
      });
  }
  btnClose() {
    let url = sessionStorage.getItem('closeRouteViewData');
    sessionStorage.removeItem('closeRouteViewData');
    sessionStorage.setItem('isBackFromSubMenu', '1');
    this.router.navigateByUrl(`/${url}`);
  }
  btnDownload(){
    let isFile=this.data.isFile;
    if(isFile==0){
      this.downloadLocal();
    }else{
      this.downloadDiconnect();
    }
  }
  downloadDiconnect(){
    const requestBody: IDownloadFileRequestModel = {
      fullLocalPath: this.localPath,
    };
    this._monitoringReportingService
      .downloadFileContent(requestBody, this.environmentID)
      .subscribe((respone) => {
      },(err) => {
        this.toastr.error('An unexpected error occurred');
      },);
  }
  downloadLocal(){
    const currentTime = new Date();
    const filename =
      'Mornitoring' +
      currentTime.getFullYear().toString() +
      (currentTime.getMonth() + 1) +
      currentTime.getDate() +
      currentTime
        .toLocaleTimeString()
        .replace(/[ ]|[,]|[:]/g, '')
        .trim() +
      '.txt';
        const fileSave = new Blob([this.data.fileContent], {
          type: 'text/plain',
        });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(fileSave);
        link.download = filename;
        link.click();
        link.remove();
  }
}
