import { DatePipe } from '@angular/common';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { ICIPReportingModel } from 'src/app/_model/ICIPReportingModel';
import { IDataExportExcelModel } from 'src/app/_model/IDataExportExcelModel';
import { IMonitoringReportingModel } from 'src/app/_model/IMonitoringReportingModel';
import { IPaginationModel } from 'src/app/_model/IPaginationModel';
import { ITransDataIntegrationModel } from 'src/app/_model/ITransDataIntegrationModel';
import { EnvironmentModel } from 'src/app/_model/node-settings-model';
import { MonitoringReportingService } from 'src/app/_service/monitoring-reporting.service';
import { NodeSettingsService } from 'src/app/_service/node-settings.service';

@Component({
  selector: 'app-monitoring-reporting-basic',
  templateUrl: './monitoring-reporting-basic.component.html',
  styleUrls: ['./monitoring-reporting-basic.component.css'],
})
export class MonitoringReportingBasicComponent extends BaseComponent implements OnInit {
  environmentList: EnvironmentModel[] = [];

  statusList: { value: string | null; label: string }[] = [
    { label: 'Any', value: null },
    { label: 'Success', value: '69' },
    { label: 'Error', value: '70' },
    { label: 'Pending', value: '71' },
    { label: 'UnrecognizedFormat', value: '152' },
    { label: 'Warning', value: '157' },
    { label: 'Integration Warning', value: '179' },
    { label: 'Integration Error', value: '180' },
  ];

  dataTypeList: { value: string | null; label: string }[] = [
    { value: null, label: 'Any' },
    { value: 'I', label: 'Inbound' },
    { value: 'O', label: 'Outbound' },
  ];
  reprocesssOpList: { value: string | null; label: string }[] = [
    { value: '1', label: 'Re-send' },
    { value: '2', label: 'Re-post' },
  ];

  platformList: { id: string; name: string; disabled: boolean }[] = [
    { id: 'CIP', name: 'CIP', disabled: true },
  ];
  reprocesssOpListChange: { value: string | null; label: string }[]=[];
  selectedenvironment!: EnvironmentModel;

  selectedStatus: { value: string | null; label: string } = {
    value: null,
    label: 'Any',
  };

  selectedDataType: { value: string | null; label: string } = {
    value: null,
    label: 'Any',
  };
  selectedReOption!: { value: string | null; label: string };
  selectedPlatform: string = 'CIP';

  isOriginalDoc: boolean = false;

  data!: IMonitoringReportingModel[];

  isSelectAll: boolean = false;
  isReProcess: boolean= true;
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

  modalRef?: BsModalRef;

  filterValue!: ICIPReportingModel;

  fromDate!: string;

  toDate!: string;

  isLoading: boolean = false;

  selectedNote!: IMonitoringReportingModel;
  listSelectedNote: ITransDataIntegrationModel[] = [];
  edittingNote: string | null | undefined;

  isLoadingNote: boolean = false;

  transactionKey: string | undefined;

  fileName!: string | null;

  constructor(
    private modalService: BsModalService,
    private _monitoringReportingService: MonitoringReportingService,
    private nodeSettingService: NodeSettingsService,
    private toastr: ToastrService,
    private router: Router,
    private activatedRouter: ActivatedRoute
  ) {super();}

  caculateItemFromAndTo() {
    this.pagination.itemFrom =
      (this.pagination.pageNumber - 1) * this.pagination.pageSize + 1;
    this.pagination.itemTo =
      this.pagination.pageNumber * this.pagination.pageSize >
      this.pagination.totalItem
        ? this.pagination.totalItem
        : this.pagination.pageNumber * this.pagination.pageSize;
  }
  getDate(MonthorDay: any) {
    if (parseInt(MonthorDay) < 10) {
      return '0' + MonthorDay;
    } else {
      return MonthorDay;
    }
  }
  convertDateString(DateFrom: Date, DateTo: Date) {
    this.fromDate =
      DateFrom.getFullYear() +
      '-' +
      this.getDate(1 + parseInt(DateFrom.getMonth().toString())) +
      '-' +
      this.getDate(DateFrom.getDate());
    this.toDate =
      DateTo.getFullYear() +
      '-' +
      this.getDate(1 + parseInt(DateTo.getMonth().toString())) +
      '-' +
      this.getDate(DateTo.getDate());
  }
  ngOnInit(): void {
    super.ngOnInit();
    this.reprocesssOpListChange=this.reprocesssOpList;
    const isBackFromSubMenu = sessionStorage.getItem('isBackFromSubMenu')
      ? parseInt(sessionStorage.getItem('isBackFromSubMenu') as string)
      : 0;
    this.convertDateString(new Date(), new Date());
    this.nodeSettingService.getSysEnvironment().subscribe(
      (respone) => {
        this.environmentList = respone.listEnvironments;
        this.selectedenvironment = this.environmentList[0];
      },
      (err) => {
        if (sessionStorage.getItem('isBackFromSubMenu'))
          sessionStorage.removeItem('isBackFromSubMenu');
        if (sessionStorage.getItem('tempFilterMonitoringReporting'))
          sessionStorage.removeItem('tempFilterMonitoringReporting');
        if (sessionStorage.getItem('tempDataMonitoringReporting'))
          sessionStorage.removeItem('tempDataMonitoringReporting');
      },
      () => {
        if (isBackFromSubMenu === 1) {
          const filterOptionsString = sessionStorage.getItem(
            'tempFilterMonitoringReporting'
          );
          const tempDataString = sessionStorage.getItem(
            'tempDataMonitoringReporting'
          );
          if (filterOptionsString) {
            const filterOptions = JSON.parse(filterOptionsString);
            this.selectedenvironment = filterOptions.selectedEnvironment;
            this.fromDate = filterOptions.fromDate;
            this.toDate = filterOptions.toDate;
            this.selectedStatus = filterOptions.selectedStatus;
            this.selectedDataType = filterOptions.selectedDataType;
            this.selectedPlatform = filterOptions.selectedPlatform;
            this.transactionKey = filterOptions.transacionKey;
            this.fileName = filterOptions.fileName;
            this.pagination = filterOptions.pagination;
            sessionStorage.removeItem('tempFilterMonitoringReporting');
          }
          if (tempDataString) {
            this.data = JSON.parse(tempDataString);
            sessionStorage.removeItem('tempDataMonitoringReporting');
          }
          if (sessionStorage.getItem('isBackFromSubMenu'))
            sessionStorage.removeItem('isBackFromSubMenu');
        } else {
          if (sessionStorage.getItem('isBackFromSubMenu'))
            sessionStorage.removeItem('isBackFromSubMenu');
          if (sessionStorage.getItem('tempFilterMonitoringReporting'))
            sessionStorage.removeItem('tempFilterMonitoringReporting');
          if (sessionStorage.getItem('tempDataMonitoringReporting'))
            sessionStorage.removeItem('tempDataMonitoringReporting');
          this.handleGetData();
        }
      }
    );
  }
  isUUID(uuid: any) {
    let s = '' + uuid;
    let guid = s.match(
      '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$'
    );
    if (guid === null) {
      return false;
    }
    return true;
  }
  btnreset() {
    this.selectedenvironment = this.environmentList[0];
    this.convertDateString(new Date(), new Date());
    this.selectedStatus = {
      value: null,
      label: 'Any',
    };
    this.selectedDataType = {
      value: null,
      label: 'Any',
    };
    this.transactionKey = undefined;
    this.handleGetData();
  }
  handleGetData() {
    sessionStorage.removeItem('tempDataMonitoringReporting');
    sessionStorage.removeItem('tempFilterMonitoringReporting');
    if (this.transactionKey == '') {
      this.transactionKey = undefined;
    }
    if (this.transactionKey != null || this.transactionKey != undefined) {
      let transactionKey = this.isUUID(this.transactionKey);
      if (transactionKey == false) {
        this.toastr.warning('Transaction Key Malformed');
        return;
      }
    }
    const requestBody = this.handleGetFilterValue();
    const startDate = requestBody.startDate as string;
    const endDate = requestBody.endDate as string;
    if (!startDate) {
      this.toastr.warning('FromDate can not be empty');
      return;
    }
    if (!endDate) {
      this.toastr.warning('ToDate can not be empty');
      return;
    }
    const _startDate = new Date(startDate);
    const _endDate = new Date(endDate);
    const diffTime = Math.abs((_endDate as any) - (_startDate as any));
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    if (_startDate.getTime() > _endDate.getTime()) {
      this.toastr.warning('ToDate must be greater than FromDate');
      return;
    }
    const quarter_StartDate = Math.floor((_startDate.getMonth() + 3) / 3);
    const quarter_EndDate = Math.floor((_endDate.getMonth() + 3) / 3);
    if (quarter_StartDate !== quarter_EndDate) {
      this.toastr.warning(
        'Search supports in one quarter and limits in two days'
      );
      return;
    }
    if (diffDays > 2) {
      this.toastr.warning(
        'Search supports in one quarter and limits in two days'
      );
      return;
    }
    this.isSelectAll = false;
    this.isLoading = true;
    this._monitoringReportingService
      .GetTransactionList(requestBody, requestBody.environmentID)
      .subscribe(
        (respone) => {
          this.data = respone.data;
          this.isLoading = false;
        },
        (err) => {
          this.isLoading = false;
        },
        () => {
          if (this.data && this.data.length > 0) {
            const totalRow = this.data.length > 0 ? this.data[0].totalRows : 0;
            this.pagination.totalItem = totalRow;
            const paginationHelper = Helper.getPager(
              totalRow,
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
            this.pagination.totalItem = 0;
            this.pagination.itemFrom = 0;
            this.pagination.itemTo = 0;
          }
        }
      );
  }

  onChangeenvironment() {
    console.log(this.selectedenvironment);
  }

  onChangePageNumber(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;
    this.handleGetData();
  }

  onChangePageSize(pageSize: number) {
    this.pagination.pageNumber = 1;
    this.handleGetData();
  }

  public onClose() {
    this.modalService.hide();
  }
 onChangeDataDirection(value :any){
  this.handleGetData();
  if(value !==null && value!== undefined){
    this.isReProcess=false;
  }else{
    this.isReProcess=true;
  }
 }
  openModal(
    NameModal: TemplateRef<any>,idModal :any
  ) {
    this.listSelectedNote = this.data
      .slice()
      .filter((item) => item.isSelect === true)
      .map((item) => {
        return {
          enviromentId: item.environmentID,
          monitoredStatus:item.monitoredStatus,
          note: item.note,
          reProcess: item.reProcessed,
          transactionKey: item.transactionKey,
          rowID: item.rowID,
        };
      });
    if (this.listSelectedNote.length <= 0) {
      this.toastr.warning('No record is selected to mark');
      return;
    }
    
    if(idModal!=1){
    if(this.selectedDataType.value==='I'){
      this.reprocesssOpList = this.reprocesssOpListChange.filter(item => item.value !== '2');
      this.selectedReOption = {
        value: '1',
        label: 'Re-send',
      };
    }else{
      this.reprocesssOpList = this.reprocesssOpListChange.filter(item => item.value !== '1');
      this.selectedReOption = {
        value: '2',
        label: 'Re-post',
      };
    }
  }
    this.modalRef = this.modalService.show(NameModal);
  }

  handleGetFilterValue(): ICIPReportingModel {
    const environmentID = this.selectedenvironment.id;
    const startDate = this.fromDate + 'T00:00:00.000Z';
    const endDate = this.toDate + 'T23:59:59.000Z';
    const status = this.selectedStatus.value;
    const cipFlow = this.selectedDataType.value;
    const platform = this.selectedPlatform;
    const pageNumber = this.pagination.pageNumber;
    const pageSize = this.pagination.pageSize;
    const transactionKey = this.transactionKey;
    const document = this.fileName;
    const returnValue: ICIPReportingModel = {
      environmentID,
      startDate,
      endDate,
      status,
      cIPFlow: cipFlow,
      platform,
      pageNumber,
      pageSize,
      transactionKey,
      document,
    };
    return returnValue;
  }

  handleGetFilterOption() {
    const selectedEnvironment = this.selectedenvironment;
    const fromDate = this.fromDate;
    const toDate = this.toDate;
    const selectedStatus = this.selectedStatus;
    const selectedDataType = this.selectedDataType;
    const selectedPlatform = this.selectedPlatform;
    const transactionKey = this.transactionKey;
    const fileName = this.fileName;
    const pagination = this.pagination;
    return {
      selectedEnvironment,
      selectedDataType,
      fromDate,
      toDate,
      selectedStatus,
      selectedPlatform,
      transactionKey,
      fileName,
      pagination,
    };
  }

  onSelectAll(isSelectAll: boolean) {
    this.data = this.data.slice().map((item) => {
      return { ...item, isSelect: isSelectAll };
    });
  }

  onMarkTransactions(monitoredStatus: number) {
    let listSelectedTransactions: ITransDataIntegrationModel[] = [];
    listSelectedTransactions = this.data
      .slice()
      .filter((item) => item.isSelect === true)
      .map((item) => {
        return {
          enviromentId: item.environmentID,
          monitoredStatus,
          note: item.note,
          reProcess: item.reProcessed,
          transactionKey: item.transactionKey,
          rowID: item.rowID,
        };
      });
    if (listSelectedTransactions.length <= 0) {
      this.toastr.warning('No record is selected to mark');
      return;
    }
    this.isLoading = true;
    this._monitoringReportingService
      .markTransactions(listSelectedTransactions, monitoredStatus)
      .subscribe(
        (respone) => {
          const isSucess = respone.isSuccessed;
          if (isSucess) {
            this.handleGetData();
            this.toastr.success('Success');
            this.data.forEach((element) => {
              if (
                listSelectedTransactions.find(
                  (a) => a.rowID === element.rowID
                ) !== undefined
              ) {
                element.monitoredStatus = monitoredStatus;
              }
            });
          } else {
            this.toastr.error('An unexpected error occurred');
          }
        },
        (err) => {
          this.isLoading = false;
          this.toastr.error('An unexpected error occurred');
        },
        () => {
          this.isLoading = false;
        }
      );
  }

  onSaveNote() {
    this.isLoadingNote = true;
    this.listSelectedNote.forEach(element => {
      element.note=this.edittingNote
    });
    this._monitoringReportingService
      .markTransactions(this.listSelectedNote, null)
      .subscribe(
        (respone) => {
          const isSuccess = respone.isSuccessed;
          if (isSuccess) {
            this.toastr.success('Save note successfully');
            this.data.forEach((element) => {
              if (
                this.listSelectedNote.find(
                  (a) => a.rowID === element.rowID
                ) !== undefined
              ) {
                element.note = this.edittingNote;
                element.isSelect=false;
              }
            });
          } else {
            this.toastr.error('An unexpected error occurred');
          }
          this.edittingNote=null;
        },
        (err) => {
          this.isLoadingNote = false;
          this.toastr.error('An unexpected error occurred');
        },
        () => {
          this.isLoadingNote = false;
          this.onClose();
        }
      );
  }

  onClickExport() {
    let listSelectedTransactions: IDataExportExcelModel[] = [];
    listSelectedTransactions = this.data
      .slice()
      .filter((item) => item.isSelect === true)
      .map((item) => {
        return {
          docType: item.docType,
          document: item.document,
          endDate: new Date(item.endDate),
          environment: item.environmentName,
          errorStatus: item.errorStatusString,
          note: item.note,
          receiver: item.receiverCustName,
          sender: item.senderCustName,
          startDate: new Date(item.startDate),
          totalOfDocs: item.totalOfDocs,
          transactionKey: item.transactionKey,
          reProcessed: item.reProcessed,
          monitoredStatus: item.monitoredStatusString,
        };
      });
    if (listSelectedTransactions.length < 1) {
      this.toastr.info('No record is selected to export');
      return;
    }
    this.isLoading = true;
    this._monitoringReportingService
      .exportReporting(listSelectedTransactions)
      .subscribe(
        (respone) => {},
        (err) => {
          this.isLoading = false;
          this.toastr.error('An unexpected error occurred');
        },
        () => {
          this.isLoading = false;
        }
      );
  }

  onClickReset() {
    this.transactionKey = undefined;
    this.fileName = null;
    this.selectedenvironment = this.environmentList[0];
    this.selectedDataType = { label: 'Any', value: null };
    this.selectedStatus = { label: 'Any', value: null };
    this.handleGetData();
  }
  loadDocumentByTK(transactionKey: string, fromDate: string, toDate: string) {
    var datePipe = new DatePipe('en-US');
    const currentRoute = this.activatedRouter.snapshot.routeConfig?.path;
    const filterOptions = this.handleGetFilterOption();
    const token = encodeURIComponent(
      JSON.stringify({
        fromDate: datePipe.transform(fromDate, 'yyyy-MM-dd') ?? '1900-01-01',
        toDate: datePipe.transform(toDate, 'yyyy-MM-dd') ?? '1900-01-01',
        environment: this.selectedenvironment.id,
        transactionKey: transactionKey,
      })
    );
    sessionStorage.setItem('closeRouteDocsByTK', currentRoute ?? '');
    sessionStorage.setItem(
      'tempFilterMonitoringReporting',
      JSON.stringify(filterOptions)
    );
    sessionStorage.setItem(
      'tempDataMonitoringReporting',
      JSON.stringify(this.data)
    );
    this.router.navigateByUrl(`/document-bytk/${token}`);
  }

  onClickAction(actionName: string, rowInfo: IMonitoringReportingModel) {
    const transactionKey = rowInfo.transactionKey;
    const currentRoute = this.activatedRouter.snapshot.routeConfig?.path;
    const filterOptions = this.handleGetFilterOption();
    sessionStorage.setItem(
      'tempFilterMonitoringReporting',
      JSON.stringify(filterOptions)
    );
    sessionStorage.setItem(
      'tempDataMonitoringReporting',
      JSON.stringify(this.data)
    );
    if (actionName === 'ViewLog') {
      sessionStorage.setItem('viewLogsItem', JSON.stringify(rowInfo));
      sessionStorage.setItem('closeRouteViewLogs', currentRoute ?? '');
      const encryptTransactionKey = btoa(transactionKey);
      this.router.navigate(['view-logs', encryptTransactionKey]);
    } else if (actionName === 'ViewData') {
      sessionStorage.setItem('viewDataItem', JSON.stringify(rowInfo));
      sessionStorage.setItem('closeRouteViewData', currentRoute ?? '');
      const token = btoa(transactionKey);
      this.router.navigateByUrl(`/view-data/${token}`);
    } else if (actionName === 'ViewConfig') {
      sessionStorage.setItem('viewConfigItem', JSON.stringify(rowInfo));
      sessionStorage.setItem('closeRouteViewConfig', currentRoute ?? '');
      const token = btoa(transactionKey);
      this.router.navigateByUrl(`/view-config/${token}`);
    }
  }
  onClickCIPProcess(){
    console.log('selectedReOption',this.selectedReOption.value);
    console.log('selectedDataType',this.selectedDataType.value);
    console.log('listSelectedNote',this.listSelectedNote);
  }
}
