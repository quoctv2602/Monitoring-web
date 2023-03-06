import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from '../base.component';
import { settingsRequest } from '../_model/node-setting-request';
import { ApiResponse } from '../_model/node-settings-model';
import { PagedResultBase } from '../_model/sys-settings';
import { NodeSettingsService } from '../_service/node-settings.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css'],
})
export class SettingsComponent  extends BaseComponent implements OnInit {
  nodeSettingsData: PagedResultBase = {
    pageIndex: 0,
    pageSize: 0,
    totalRecords: 0,
    pageCount: 0,
    listItem: [],
  };
  tableSizes = [10, 20, 30, 40, 50];
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  settingsForm: FormGroup = this.fb.group({
    NodeName: new FormControl(),
    IsActive: new FormControl(),
    PageIndex: new FormControl(),
    PageSize: new FormControl(),
  });
  modalRef?: BsModalRef;
  fileImportJson: any;
  isCheckallrs = false;
  pagingSize!: number;
  constructor(
    private nodeSettingsService: NodeSettingsService,
    private router: Router,
    private toastr: ToastrService,
    public fb: FormBuilder,
    private modalService: BsModalService,
    private spinner: NgxSpinnerService
  ) {super();}

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  ngOnInit() {
    super.ngOnInit();
    this.intitializeForm();
    this.pagingSize = this.settingsForm.value.PageSize;
    this.getSysNodeSetting(this.settingsForm.value);
  }
  intitializeForm() {
    this.settingsForm = this.fb.group({
      NodeName: null,
      IsActive: '',
      PageIndex: 1,
      PageSize: 20,
    });
  }
  getSysNodeSetting(settingsRequest: settingsRequest) {
    this.spinner.show();
    this.nodeSettingsService.getSysNodeSetting(settingsRequest).subscribe(
      (res) => {
        this.nodeSettingsData = res;
        this.spinner.hide();
      },
      (error) => {
        this.spinner.hide();
        this.toastr.error('An unexpected error has occurred');
      }
    );
  }
  onTableDataChange(event: any) {
    this.settingsForm.value.PageIndex = event;
    this.getSysNodeSetting(this.settingsForm.value);
  }
  onTableSizeChange(event: any): void {
    this.settingsForm.value.pageSize = event;
    this.settingsForm.value.PageIndex = 1;
    this.getSysNodeSetting(this.settingsForm.value);
  }
  btnSearch() {
    this.getSysNodeSetting(this.settingsForm.value);
  }
  btnAdd(): void {
    this.router.navigateByUrl('/node-settings');
  }
  btnEdit(id: number): void {
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.kpiSettingsUpdateKPI);
    if(role===-1){
      this.toastr.warning('Access denied');
      return
    }
    this.router.navigateByUrl(`/node-settings-edit/${id}`);
  }
  btnReset() {
    this.intitializeForm();
    this.isCheckallrs = false;
    this.getSysNodeSetting(this.settingsForm.value);
  }
  btnExport(): void {
    let listSettingCheck = this.nodeSettingsData.listItem.filter(
      (item) => item.isCheck
    );
    if (listSettingCheck.length == 0) {
      this.toastr.error('No data selected, please select!');
      return;
    }
    let NodeSettingId = '';
    for (let i = 0; i < listSettingCheck.length; i++) {
      if (NodeSettingId == '') {
        NodeSettingId = listSettingCheck[i].id.toString();
      } else {
        NodeSettingId += ',' + listSettingCheck[i].id.toString();
      }
    }
    this.spinner.show();
    this.nodeSettingsService.exportFileJson(NodeSettingId).subscribe(
      (res) => {
        this.spinner.hide();
        this.apiResponse = res;
        if (this.apiResponse.isSuccessed == true) {
          this.toastr.success('Export file json success');
        }
      },
      (error) => {
        this.spinner.hide();
        this.toastr.error('Export file json error');
      }
    );
  }
  btnIsActiveNode(id: number, e: any): void {
    var ischeck = e.target.checked;
    let listSettingCheck = this.nodeSettingsData.listItem.filter((x) => {
      return x.id == id;
    })[0];
    listSettingCheck.isActive = ischeck;
    this.nodeSettingsService.updateIsActive(listSettingCheck).subscribe(
      (res) => {
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
  onSelectFile(event: any) {
    if (event.target.files && event.target.files[0]) {
      const reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]); // read file as data url
      const file = event.target.files[0];
      // check file name extension
      const fileNameExtension = event.target.files[0].name.split('.').pop();
      if (fileNameExtension != 'json') {
        this.toastr.error('Please select a file .json ');
        return;
      }
      this.fileImportJson = file;
    }
  }
  btnImportFile() {
    var fileTmport = this.fileImportJson;
    if (fileTmport === 'undefined' || fileTmport == null || fileTmport == '') {
      this.toastr.error('Please select a file .json ');
      return;
    }
    this.spinner.show();
    this.nodeSettingsService.importFileJson(this.fileImportJson).subscribe(
      (res) => {
        this.spinner.hide();
        this.apiResponse = res;
        if (this.apiResponse.isSuccessed == true) {
          this.toastr.success('Import node success');
          this.modalRef?.hide();
          this.getSysNodeSetting(this.settingsForm.value);
        } else {
          this.toastr.error('Node import error : ' + this.apiResponse.message);
        }
      },
      (error) => {
        this.spinner.hide();
        this.toastr.error('Json file is malformed');
      }
    );
  }
  isCheckAll(e: any) {
    var ischeck = e.target.checked;
    if (ischeck == true) {
      for (let i = 0; i < this.nodeSettingsData.listItem.length; i++) {
        this.nodeSettingsData.listItem[i].isCheck = true;
      }
    } else {
      for (let i = 0; i < this.nodeSettingsData.listItem.length; i++) {
        this.nodeSettingsData.listItem[i].isCheck = false;
      }
    }
  }
}
