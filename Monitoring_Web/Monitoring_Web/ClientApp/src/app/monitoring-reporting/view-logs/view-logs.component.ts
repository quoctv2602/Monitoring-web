import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DirectionTransaction } from 'src/app/_common/_enum';
import { Helper } from 'src/app/_common/_helper';
import { ICIPTransactionKeyModel } from 'src/app/_model/ICIPReportingModel';
import { IMonitoringReportingModel } from 'src/app/_model/IMonitoringReportingModel';
import { IPaginationModel } from 'src/app/_model/IPaginationModel';
import { IViewLogsModel } from 'src/app/_model/IViewLogsModel';
import { IViewLogsRequestModel } from 'src/app/_model/IViewLogsRequestModel';
import { MonitoringReportingService } from 'src/app/_service/monitoring-reporting.service';

@Component({
  selector: 'app-view-logs',
  templateUrl: './view-logs.component.html',
  styleUrls: ['./view-logs.component.css'],
})
export class ViewLogsComponent implements OnInit, OnDestroy {
  actionName!: string;

  errorCode!: string;

  data: IViewLogsModel[] = [];

  rootData: IViewLogsModel[] = [];

  isLoading: boolean = false;

  pagination: IPaginationModel = {
    itemFrom: 0,
    itemTo: 0,
    listPageNumber: [],
    pageNumber: 1,
    pageSize: 20,
    pageSizeList: [10, 20, 50, 100],
    totalItem: 0,
    totalPage: 0,
  };

  transacionKey!: string;

  environmentID!: number;

  direction!: number;

  viewLogsItem!: IMonitoringReportingModel;

  constructor(
    private _monitoringReportingService: MonitoringReportingService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const isBackFromSubMenu = sessionStorage.getItem('isBackFromSubMenu')
      ? parseInt(sessionStorage.getItem('isBackFromSubMenu') as string)
      : 0;
    this.viewLogsItem = sessionStorage.getItem('viewLogsItem')
      ? JSON.parse(sessionStorage.getItem('viewLogsItem') as string)
      : {};
    this.transacionKey = this.route.snapshot.paramMap.get('transactionKey')
      ? atob(this.route.snapshot.paramMap.get('transactionKey') as string)
      : '00000000-0000-0000-0000-000000000000';
    this.environmentID = this.viewLogsItem.environmentID ?? 0;
    this.direction = this.viewLogsItem.direction ?? 0;
    if (isBackFromSubMenu === 1) {
      const tempDataString = sessionStorage.getItem('tempDataViewLogs');
      const tempFilterString = sessionStorage.getItem('tempFilterViewLogs');
      const tempRootDataViewLogsString = sessionStorage.getItem(
        'tempRootDataViewLogs'
      );
      if (tempFilterString) {
        const tempFilter = JSON.parse(tempFilterString);
        this.actionName = tempFilter.actionName;
        this.errorCode = tempFilter.errorCode;
        this.pagination = tempFilter.pagination;
        sessionStorage.removeItem('tempFilterViewLogs');
      }
      if (tempDataString) {
        this.data = JSON.parse(tempDataString);
        sessionStorage.removeItem('tempDataViewLogs');
      }
      if (tempRootDataViewLogsString) {
        this.rootData = JSON.parse(tempRootDataViewLogsString);
        sessionStorage.removeItem('tempRootDataViewLogs');
      }
      if (sessionStorage.getItem('isBackFromSubMenu'))
        sessionStorage.removeItem('isBackFromSubMenu');
    } else {
      if (sessionStorage.getItem('isBackFromSubMenu'))
        sessionStorage.removeItem('isBackFromSubMenu');
      if (sessionStorage.getItem('tempDataViewLogs'))
        sessionStorage.removeItem('tempDataViewLogs');
      if (sessionStorage.getItem('tempFilterViewLogs'))
        sessionStorage.removeItem('tempFilterViewLogs');
      if (sessionStorage.getItem('tempRootDataViewLogs'))
        sessionStorage.removeItem('tempRootDataViewLogs');
      this.handleGetData();
    }
  }

  ngOnDestroy(): void {
    if (!sessionStorage.getItem('isViewData')) {
      sessionStorage.removeItem('viewLogsItem');
    } else {
      sessionStorage.removeItem('isViewData');
    }
  }

  public get directionEnum(): typeof DirectionTransaction {
    return DirectionTransaction;
  }

  handleGetData() {
    this.isLoading = true;
    const requestBody: IViewLogsRequestModel = {
      transactionKey: this.transacionKey,
      pageIndex: this.pagination.pageNumber,
      pageSize: this.pagination.pageSize,
    };
    this._monitoringReportingService
      .viewLogsByTransactionKey(requestBody, this.environmentID)
      .subscribe(
        (respone) => {
          const status = respone.data.status;
          const message = respone.data.message;
          const data = respone.data.data;
          if (status === 1) {
            this.data = data;
            this.rootData = data;
            if (this.data && this.data.length > 0) {
              const totalRow =
                this.data.length > 0 ? this.data[0].totalRows : 0;
              this.pagination.totalItem = totalRow ?? 0;
              const paginationHelper = Helper.getPager(
                totalRow ?? 0,
                this.pagination.pageNumber,
                this.pagination.pageSize
              );
              this.pagination.listPageNumber = paginationHelper.pages;
              this.pagination.totalPage = paginationHelper.totalPages;
              this.caculateItemFromAndTo();
            } else {
              this.data = [];
              this.pagination.listPageNumber = [];
              this.pagination.totalPage = 0;
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

  caculateItemFromAndTo() {
    this.pagination.itemFrom =
      (this.pagination.pageNumber - 1) * this.pagination.pageSize + 1;
    this.pagination.itemTo =
      this.pagination.pageNumber * this.pagination.pageSize >
      this.pagination.totalItem
        ? this.pagination.totalItem
        : this.pagination.pageNumber * this.pagination.pageSize;
  }

  onChangePageNumber(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;
    this.handleGetData();
  }

  onChangePageSize(pageSize: number) {
    this.pagination.pageNumber = 1;
    this.handleGetData();
  }

  onClickViewData() {
    const filterOptions = {
      actionName: this.actionName,
      errorCode: this.errorCode,
      pagination: this.pagination,
    };
    sessionStorage.setItem('isViewData', '1');
    sessionStorage.setItem('tempDataViewLogs', JSON.stringify(this.data));
    sessionStorage.setItem('tempFilterViewLogs', JSON.stringify(filterOptions));
    sessionStorage.setItem(
      'tempRootDataViewLogs',
      JSON.stringify(this.rootData)
    );
    let currentRoute = '';
    const arrayUrl = this.route.snapshot.url;
    for (let i = 0; i < arrayUrl.length; i++) {
      if (i === 0) currentRoute = arrayUrl[i].path;
      else currentRoute += '/' + arrayUrl[i].path;
    }
    sessionStorage.setItem('viewDataItem', JSON.stringify(this.viewLogsItem));
    sessionStorage.setItem('closeRouteViewData', currentRoute ?? '');
    const token = btoa(this.viewLogsItem.transactionKey);
    this.router.navigateByUrl(`/view-data/${token}`);
  }

  onClickClose() {
    if (sessionStorage.getItem('isViewData'))
      sessionStorage.removeItem('isViewData');
    const route = sessionStorage.getItem('closeRouteViewLogs') ?? '';
    if (sessionStorage.getItem('closeRouteViewLogs'))
      sessionStorage.removeItem('closeRouteViewLogs');
    sessionStorage.setItem('isBackFromSubMenu', '1');
    this.router.navigate([route]);
  }

  handleFilter() {
    const actionName = this.actionName ? this.actionName.trim() : null;
    const errorCode = this.errorCode ? this.errorCode.trim() : null;
    if (actionName || errorCode)
      this.data = this.rootData
        .slice()
        .filter(
          (e) =>
            (!actionName || e.actionName?.includes(actionName)) &&
            (!errorCode || e.errorCodeID?.includes(errorCode))
        );
    else {
      this.data = this.rootData.slice();
    }
  }
}
