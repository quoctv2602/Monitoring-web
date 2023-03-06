import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from '../base.component';
import {
  INotificationModel,
  NotificationListRequest,
  NotificationModel,
  ToggleNotificationRequest,
} from '../_model/INotificationModel';
import { ApiResponse } from '../_model/node-settings-model';
import { NotificationService } from '../_service/notification.service';

@Component({
  selector: 'app-notification-settings',
  templateUrl: './notification-settings.component.html',
  styleUrls: ['./notification-settings.component.css'],
})
export class NotificationSettingsComponent extends BaseComponent implements OnInit {
  notificationForm: FormGroup = this.fb.group({
    Name: new FormControl(),
    PageIndex: new FormControl(),
    PageSize: new FormControl(),
  });
  notificationData: INotificationModel = {
    pageIndex: 0,
    pageSize: 0,
    totalRecords: 0,
    pageCount: 0,
    listItem: [],
  };
  toggleNotificationRequest: ToggleNotificationRequest = {
    id: 0,
    isActive: true,
  };
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  tableSizes = [10, 20, 30, 40, 50];
  submitted = false;
  isCheckallrs = false;
  pagingSize!: number;
  modalRef?: BsModalRef;
  listNotification: NotificationModel[] = [];
  constructor(
    private router: Router,
    private toastr: ToastrService,
    public fb: FormBuilder,
    private spinner: NgxSpinnerService,
    private notificationService: NotificationService,
    private modalService: BsModalService
  ) {super();}

  ngOnInit() {
    super.ngOnInit();
    this.intitializeForm();
    this.getListNotification(this.notificationForm.value);
    this.pagingSize = this.notificationForm.value.PageSize;
  }
  intitializeForm() {
    this.notificationForm = this.fb.group({
      Name: null,
      PageIndex: 1,
      PageSize: 20,
    });
  }
  getListNotification(notificationListRequest: NotificationListRequest) {
    this.spinner.show();
    this.notificationService
      .getListNotification(notificationListRequest)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.notificationData = res;
        },
        (error) => {
          this.spinner.hide();
          this.toastr.error('An unexpected error has occurred');
        }
      );
  }
  onTableDataChange(event: any) {
    this.notificationForm.value.PageIndex = event;
    this.getListNotification(this.notificationForm.value);
  }
  onTableSizeChange(event: any): void {
    this.notificationForm.value.pageSize = event;
    this.notificationForm.value.PageIndex = 1;
    this.getListNotification(this.notificationForm.value);
  }
  btnReset() {
    this.intitializeForm();
    this.isCheckallrs = false;
    this.getListNotification(this.notificationForm.value);
  }
  btnSearch() {
    this.getListNotification(this.notificationForm.value);
  }
  btnAdd() {
    this.router.navigateByUrl('/notification-add');
  }
  btnDelete() {
    this.spinner.show();
    this.notificationService
      .deleteNotification(this.listNotification)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.modalService.hide();
            this.toastr.success('Delete notification success');
            this.getListNotification(this.notificationForm.value);
          } else {
            this.toastr.error(this.apiResponse.message);
          }
        },
        (error) => {
          this.spinner.hide();
          this.toastr.error('An unexpected error has occurred');
        }
      );
  }
  isCheckAll(e: any) {
    var ischeck = e.target.checked;
    if (ischeck == true) {
      for (let i = 0; i < this.notificationData.listItem.length; i++) {
        this.notificationData.listItem[i].isCheck = true;
      }
    } else {
      for (let i = 0; i < this.notificationData.listItem.length; i++) {
        this.notificationData.listItem[i].isCheck = false;
      }
    }
  }
  toggleNotification(id: any, e: any) {
    var ischeck = e.target.checked;
    this.toggleNotificationRequest.id = id;
    this.toggleNotificationRequest.isActive = ischeck;
    this.notificationService
      .toggleNotification(this.toggleNotificationRequest)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
          } else {
            this.toastr.error(this.apiResponse.message);
          }
        },
        (error) => {
          this.toastr.error('An unexpected error has occurred');
        }
      );
  }
  editNotification(id: number): void {
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.notificationSettingsManage);
    if(role===-1){
      this.toastr.warning('Access denied');
      return
    }
    this.router.navigateByUrl(`/notification-edit/${id}`);
  }
  public onClose() {
    this.modalService.hide();
  }

  openModal(deleteNotification: TemplateRef<any>) {
    let listNotificationCheck = this.notificationData.listItem.filter(
      (item) => item.isCheck
    );
    if (listNotificationCheck.length == 0) {
      this.toastr.error('No data selected, please select!');
      return;
    }
    this.listNotification = listNotificationCheck;
    this.modalRef = this.modalService.show(deleteNotification);
  }
}
