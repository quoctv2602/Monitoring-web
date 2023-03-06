import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from '../base.component';
import { ListIntegrationAPI, NodeType } from '../_model/integration-api-model';
import { ApiResponse } from '../_model/node-settings-model';
import { SysEnvironment } from '../_model/sys-environment';
import { IntegrationAPIRequest } from '../_model/sys-integration-api';
import { IntegrationService } from '../_service/integration.service';

@Component({
  selector: 'app-integration',
  templateUrl: './integration.component.html',
  styleUrls: ['./integration.component.css'],
})
export class IntegrationComponent extends BaseComponent implements OnInit {
  integrationData: ListIntegrationAPI = {
    pageIndex: 0,
    pageSize: 0,
    totalRecords: 0,
    pageCount: 0,
    listItem: [],
  };

  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  integrationForm: FormGroup = this.fb.group({
    machineName: new FormControl(),
    environmentName: new FormControl(),
    PageIndex: new FormControl(),
    PageSize: new FormControl(),
  });
  tableSizes = [10, 20, 30, 40, 50];
  submitted = false;
  modalRef?: BsModalRef;
  listEnvironment: SysEnvironment[] = [];
  listNodeType: NodeType[] = [];
  isCheckallrs = false;
  pagingSize!: number;
  constructor(
    private router: Router,
    private toastr: ToastrService,
    public fb: FormBuilder,
    private integrationService: IntegrationService,
    private spinner: NgxSpinnerService
  ) {super();}

  ngOnInit() {
    super.ngOnInit();
    this.intitializeForm();
    this.getListIntegrationAPI(this.integrationForm.value);
    this.pagingSize = this.integrationForm.value.PageSize;
  }

  intitializeForm() {
    this.integrationForm = this.fb.group({
      machineName: null,
      environmentName: null,
      PageIndex: 1,
      PageSize: 20,
    });
  }

  onTableDataChange(event: any) {
    this.integrationForm.value.PageIndex = event;
    this.getListIntegrationAPI(this.integrationForm.value);
  }
  onTableSizeChange(event: any): void {
    this.integrationForm.value.pageSize = event;
    this.integrationForm.value.PageIndex = 1;
    this.getListIntegrationAPI(this.integrationForm.value);
  }
  getListIntegrationAPI(integrationAPIRequest: IntegrationAPIRequest) {
    this.spinner.show();
    this.integrationService
      .getListIntegrationAPI(integrationAPIRequest)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.integrationData = res;
        },
        (error) => {
          this.spinner.hide();
          this.toastr.error('An unexpected error has occurred');
        }
      );
  }
  btnReset() {
    this.intitializeForm();
    this.isCheckallrs = false;
    this.getListIntegrationAPI(this.integrationForm.value);
  }
  btnSearch() {
    this.getListIntegrationAPI(this.integrationForm.value);
  }
  btnAdd() {
    this.router.navigateByUrl('/integration-add');
  }
  getEnvironment() {
    this.integrationService.getEnvironment().subscribe((res) => {
      this.listEnvironment = res;
    });
  }
  btnEdit(id: number): void {
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.kpiSettingsManageNode);
    if(role===-1){
      this.toastr.warning('Access denied');
      return
    }
    this.router.navigateByUrl(`/integration-edit/${id}`);
  }

  btnDelete() {
    let listIntegrationCheck = this.integrationData.listItem.filter(
      (item) => item.isCheck
    );
    if (listIntegrationCheck.length == 0) {
      this.toastr.error('No data selected, please select!');
      return;
    }
    this.spinner.show();
    this.integrationService
      .deleteIntegrationAPI(listIntegrationCheck)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.toastr.success('Delete node success');
            this.getListIntegrationAPI(this.integrationForm.value);
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
      for (let i = 0; i < this.integrationData.listItem.length; i++) {
        this.integrationData.listItem[i].isCheck = true;
      }
    } else {
      for (let i = 0; i < this.integrationData.listItem.length; i++) {
        this.integrationData.listItem[i].isCheck = false;
      }
    }
  }
}
