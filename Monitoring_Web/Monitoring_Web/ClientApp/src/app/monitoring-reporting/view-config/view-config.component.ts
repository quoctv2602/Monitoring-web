import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IMonitoringReportingModel } from 'src/app/_model/IMonitoringReportingModel';
import {
  IMailbox,
  IViewConfigurationModel,
} from 'src/app/_model/IViewConfigurationModel';
import { IViewConfigurationRequestModel } from 'src/app/_model/IViewConfigurationRequestModel';
import { MonitoringReportingService } from 'src/app/_service/monitoring-reporting.service';

@Component({
  selector: 'app-view-config',
  templateUrl: './view-config.component.html',
  styleUrls: ['./view-config.component.css'],
})
export class ViewConfigComponent implements OnInit {
  data: IViewConfigurationModel = {
    cipConfiguration: { senderMailboxList: [], receiverMailboxList: [] },
    senderCustomer:{
      customerID:null,
      customerName: '',
      isaid: '',
      gsIn: '',
      gsOut: '',
      qualifier: '',
      customerRanking: '',
      customerOwner: '',
      siteID: '',
      useCIP: '',
      useCIPName: '',
      downloadEDI_YN: '',
      downloadASCII_YN: '',
      useASCIIR9_YN: '',
      useDiMetrics_YN: '',
    },
    receiverCustomer:{
      customerID: null,
      customerName: '',
      isaid: '',
      gsIn: '',
      gsOut: '',
      qualifier: '',
      customerRanking: '',
      customerOwner: '',
      siteID: '',
      useCIP: '',
      useCIPName: '',
      downloadEDI_YN: '',
      downloadASCII_YN: '',
      useASCIIR9_YN: '',
      useDiMetrics_YN: '',
    }
  };
  environmentID!: number;
  isLoading: boolean = false;
  viewConfigItem!: IMonitoringReportingModel;
  transactionKey!: string;
  transID!: string;
  fromCustID!: number;
  toCustID!: number;
  constructor(
    private _monitoringReportingService: MonitoringReportingService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.viewConfigItem = sessionStorage.getItem('viewConfigItem')
      ? JSON.parse(sessionStorage.getItem('viewConfigItem') as string)
      : {};
    this.transactionKey = this.route.snapshot.paramMap.get('token')
      ? atob(this.route.snapshot.paramMap.get('token') as string)
      : '00000000-0000-0000-0000-000000000000';
    this.environmentID = this.viewConfigItem.environmentID;
    this.transID = this.viewConfigItem.docType;
    this.fromCustID=this.viewConfigItem.senderCustId;
    this.toCustID=this.viewConfigItem.receiverCustId;
    this.handleGetData();
  }

  ngOnDestroy(): void {
    sessionStorage.removeItem('viewConfigItem');
  }

  handleGetData() {
    this.isLoading = true;
    const requestBody: IViewConfigurationRequestModel = {
      transactionKey: this.transactionKey,
      fromCustID:this.fromCustID,
      toCustID:this.toCustID,
    };
    this._monitoringReportingService
      .viewConfigByTransactionKey(requestBody, this.environmentID)
      .subscribe(
        (respone) => {
          this.isLoading = false;
          const status = respone.data.status;
          const message = respone.data.message;
          const data = respone.data.data;
          if (status === 1) {
            if (data) {
              this.data = data;
            }
          } else {
            if (message && message.length > 0) {
              this.toastr.error(message);
            }
          }
        },
        (err) => {
          this.isLoading = false;
        },
        () => {
          this.isLoading = false;
        }
      );
  }

  btnClose() {
    let url = sessionStorage.getItem('closeRouteViewConfig');
    sessionStorage.removeItem('closeRouteViewConfig');
    sessionStorage.setItem('isBackFromSubMenu', '1');
    this.router.navigateByUrl(`/${url}`);
  }

  onChangePageSize(pageSize: number) {
    this.handleGetData();
  }

  isEmptyObject(obj: any) {
    return obj && Object.keys(obj).length === 0;
  }

  getMultiValue(propertyName: string, objectName: string) {
    let returnValue = '';
    const receiverMailboxList = this.data.cipConfiguration.receiverMailboxList;
    const senderMailboxList = this.data.cipConfiguration.senderMailboxList;
    if (objectName === 'sender') {
      if (senderMailboxList !== null && senderMailboxList !== undefined)
        senderMailboxList.slice().forEach((element) => {
          if (element[propertyName as keyof IMailbox])
            returnValue += element[propertyName as keyof IMailbox] + '/';
        });
    } else if (objectName === 'receiver') {
      if (receiverMailboxList !== null && receiverMailboxList !== undefined)
        receiverMailboxList.slice().forEach((element) => {
          if (element[propertyName as keyof IMailbox])
            returnValue += element[propertyName as keyof IMailbox] + '/';
        });
    } else if (objectName === 'both') {
      if (senderMailboxList !== null && senderMailboxList !== undefined)
        senderMailboxList.slice().forEach((element) => {
          if (element[propertyName as keyof IMailbox])
            returnValue += element[propertyName as keyof IMailbox] + '/';
        });
      if (receiverMailboxList !== null && receiverMailboxList !== undefined)
        receiverMailboxList.slice().forEach((element) => {
          if (element[propertyName as keyof IMailbox])
            returnValue += element[propertyName as keyof IMailbox] + '/';
        });
    }
    returnValue = returnValue.trim();
    if (returnValue[0] === '/')
      returnValue = returnValue.slice(1, returnValue.length);
    if (returnValue[returnValue.length - 1] === '/')
      returnValue = returnValue.slice(0, returnValue.length - 1);
    return returnValue;
  }
}
