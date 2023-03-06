import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Helper } from 'src/app/_common/_helper';
import { ICIPTransactionKeyModel } from 'src/app/_model/ICIPReportingModel';
import { IMonitoringReportingModel } from 'src/app/_model/IMonitoringReportingModel';
import { IPaginationModel } from 'src/app/_model/IPaginationModel';
import { MonitoringReportingService } from 'src/app/_service/monitoring-reporting.service';

@Component({
  selector: 'app-document-bytk',
  templateUrl: './document-bytk.component.html',
  styleUrls: ['./document-bytk.component.css']
})
export class DocumentBytkComponent implements OnInit {
  pagination: IPaginationModel = {
    itemFrom: 0,
    itemTo: 0,
    listPageNumber: [],
    pageNumber: 1,
    pageSize: 50,
    pageSizeList: [20, 50, 100],
    totalItem: 0,
    totalPage: 0,
  };
  fromDate!: string;
  toDate!: string;
  dtfromDate!: string;
  dttoDate!: string;
  document!:string| null;
  isLoading: boolean = false;
  data!: IMonitoringReportingModel[];
  transactionKey! :string;
  environmentID! :number;
  constructor(
    private _monitoringReportingService: MonitoringReportingService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    ) { }
  caculateItemFromAndTo() {
    this.pagination.itemFrom =
      (this.pagination.pageNumber - 1) * this.pagination.pageSize + 1;
    this.pagination.itemTo =
      this.pagination.pageNumber * this.pagination.pageSize >
      this.pagination.totalItem
        ? this.pagination.totalItem
        : this.pagination.pageNumber * this.pagination.pageSize;
  }
  getDate(MonthorDay : any){
    if(parseInt(MonthorDay) < 10){
      return '0'+ MonthorDay;
    }else{
      return MonthorDay;
    }
  }
  convertDateString(DateFrom: Date, DateTo: Date) {
    this.fromDate =
      DateFrom.getFullYear() +
      '-' + 
      this.getDate((1 + parseInt(DateFrom.getMonth().toString()))) +
      '-' +
      this.getDate(DateFrom.getDate());
    this.toDate =
      DateTo.getFullYear() +
      '-' +
      this.getDate((1 + parseInt(DateTo.getMonth().toString()))) +
      '-' +
      this.getDate(DateTo.getDate());
  }
  ngOnInit() {
    try {
      const param = JSON.parse(
        this.route.snapshot.paramMap.get('token') as string
      );
      this.environmentID = param.environment??'0';
      this.transactionKey = param.transactionKey??'00000000-0000-0000-0000-000000000000';
      this.dtfromDate = param.fromDate;
      this.dttoDate = param.toDate;
      
      this.convertDateString(
        new Date(this.dtfromDate),
        new Date(this.dttoDate)
      );
      this.handleGetData();
    } catch (error) {}
  }
  handleGetFilterValue(): ICIPTransactionKeyModel {
    const startDate = this.fromDate + "T00:00:00.000Z";
    const endDate = this.toDate + "T23:59:59.000Z";
    const pageNo = this.pagination.pageNumber;
    const pageSize = this.pagination.pageSize;
    const transactionKey = this.transactionKey;
    const document = this.document;
    const returnValue: ICIPTransactionKeyModel = {
      startDate,
      endDate,
      pageNo,
      pageSize,
      transactionKey,
      document,
    };
    return returnValue;
  }
  handleGetData() {
    const requestBody = this.handleGetFilterValue();
    let docs=requestBody.document??'';
    if(docs.length>4000){
      this.toastr.warning('Document max 4000 characters')
      return;
    }
    this.isLoading = true;
    this._monitoringReportingService
      .GetTransactionKey(requestBody, this.environmentID)
      .subscribe(
        (respone) => {
          const status = (respone.data as any).status;
          const message = (respone.data as any).message;
          const data = (respone.data as any).data;
          if(status==1){
            if(data==null||data==''){
              this.data=[];
            }else{
              this.data = data;
            }
            this.isLoading = false;
          }
          else{
            this.toastr.error(message)
            this.isLoading = false;
          }
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
          }
        }
      );
  }
  onChangePageNumber(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;
    this.handleGetData();
  }

  onChangePageSize(pageSize: number) {
    this.handleGetData();
  }
  btnReset(){
    this.document=null;
    this.handleGetData();
  }
  btnClose() {
    let url = sessionStorage.getItem('closeRouteDocsByTK')??'';
    sessionStorage.removeItem('closeRouteDocsByTK');
    sessionStorage.setItem('isBackFromSubMenu', '1');
    this.router.navigateByUrl(`/${url}`);
  }
}
