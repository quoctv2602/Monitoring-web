import { Component, EventEmitter, OnInit, TemplateRef } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { NodeType } from 'src/app/_model/integration-api-model';
import {
  NodeSettingsEdit,
  ThresholdRuleEdit,
} from 'src/app/_model/node-settings-edit';
import {
  ApiResponse,
  EnvironmentModel,
  IntegrationAPI,
  NodeSetingsModel,
  Service,
} from 'src/app/_model/node-settings-model';
import { SysEnvironment } from 'src/app/_model/sys-environment';
import { SysMonitoring } from 'src/app/_model/sys-monitoring';
import { IntegrationService } from 'src/app/_service/integration.service';
import { NodeSettingsService } from 'src/app/_service/node-settings.service';

@Component({
  selector: 'app-node-settings-edit',
  templateUrl: './node-settings-edit.component.html',
  styleUrls: ['./node-settings-edit.component.css'],
})
export class NodeSettingsEditComponent extends BaseComponent implements OnInit {
  cancelNodeSetting = new EventEmitter();
  sysMonitor: SysMonitoring[] = [];
  nodeSettingsModel: NodeSetingsModel = {
    listEnvironments: [],
  };
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  servicesArr: string[] = [];
  sysEnvironment: EnvironmentModel[] = [];
  listNode: IntegrationAPI[] = [];
  listService: Service[] = [];
  nodeSettingsEdit: NodeSettingsEdit = {
    id: 0,
    nodeType: Helper.defaultNodeType,
    nodeName: '',
    environmentID: 0,
    machineName: '',
    description: '',
    serviceList: '',
    notificationEmail: '',
    reportEmail: '',
    notificationAlias: '',
    reportAlias: '',
    createDate: '',
    isActive: true,
    healthMeasurementKey: '',
    appid: '',
    domain_SystemHealth: '',
    listThresholdRuleEdit: [],
  };
  integrationFormSave: FormGroup = this.fb.group({
    environmentID: new FormControl(),
    machineName: new FormControl(),
    healthMeasurementKey: new FormControl(),
    appid: new FormControl(),
    domain_SystemHealth: new FormControl(),
    nodeType: new FormControl(),
    isActive: new FormControl(),
    serviceList: new FormControl(),
    isDefaultNode: new FormControl(),
  });
  modalRef?: BsModalRef;
  listEnvironment: SysEnvironment[] = [];
  listNodeType: NodeType[] = [];
  submitted = false;
  urlRegex =
    /^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/;

  invalidNotificationEmails: string[] = [];
  isNodeType = Helper.defaultNodeType;
  invalidReportEmails: string[] = [];
  listMonitoring: SysMonitoring[] = [];
  listThresholdRuleChange: ThresholdRuleEdit[] = [];
  nodeTypeDB = Helper.defaultNodeType;
  constructor(
    private nodeSettingsService: NodeSettingsService,
    private toastr: ToastrService,
    public fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private modalService: BsModalService,
    private integrationService: IntegrationService
  ) {super();}

  ngOnInit() {
    super.ngOnInit();
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.kpiSettingsUpdateKPI);
    if(role===-1){
      this.router.navigateByUrl('/access-denied');
    }
    this.spinner.show();
    let id = this.route.snapshot.paramMap.get('id') ?? '0';
    this.nodeSettingsService.getSysMonitoring().subscribe((res) => {
      this.sysMonitor = res;
      this.listMonitoring = res;
    });
    this.getNodeType();
    this.nodeSettingsService.getSysEnvironment().subscribe((res) => {
      this.nodeSettingsModel = res;
      this.sysEnvironment = this.nodeSettingsModel.listEnvironments;
      this.getNodeSettingsEdit(parseInt(id));
    });
  }
  get g(): { [key: string]: AbstractControl } {
    return this.integrationFormSave.controls;
  }
  intitializeFormSave() {
    this.integrationFormSave = this.fb.group({
      environmentID: [1, Validators.required],
      machineName: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          Validators.maxLength(500),
        ],
      ],
      healthMeasurementKey: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          Validators.maxLength(4000),
        ],
      ],
      appid: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          Validators.maxLength(4000),
        ],
      ],
      domain_SystemHealth: [
        '',
        [
          Validators.required,
          Validators.maxLength(500),
          Validators.pattern(this.urlRegex),
        ],
      ],
      nodeType: Helper.defaultNodeType,
      isActive: true,
      serviceList: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          Validators.maxLength(4000),
        ],
      ],
      isDefaultNode: false,
    });
  }
  noWhitespaceValidator(control: FormControl): ValidationErrors | null {
    const isWhitespace = (control.value || '').trim().length === 0;
    return isWhitespace ? { whitespace: true } : null;
  }
  getNodeSettingsEdit(id: number) {
    this.nodeSettingsService.getNodeSettingsEdit(id).subscribe(
      (res) => {
        this.spinner.hide();
        this.nodeSettingsEdit = res;
        this.servicesArr =
          this.nodeSettingsEdit.serviceList == null
            ? []
            : this.nodeSettingsEdit.serviceList.split(';');
        this.listNode = this.nodeSettingsModel.listEnvironments.filter((x) => {
          return x.id == this.nodeSettingsEdit.environmentID;
        })[0]?.listIntegrationAPI;
        let nodeid = this.listNode.filter((x) => {
          return x.nodeName == this.nodeSettingsEdit.nodeName;
        })[0]?.id;
        this.listService = this.listNode.filter((x) => {
          return x.id == nodeid;
        })[0]?.service;
        this.isNodeType = this.nodeSettingsEdit.nodeType;
        this.nodeTypeDB = this.nodeSettingsEdit.nodeType;
        this.sysMonitor = this.sysMonitor.filter((x) => {
          return x.nodeType == this.isNodeType;
        });
        this.listThresholdRuleChange =
          this.nodeSettingsEdit.listThresholdRuleEdit;
      },
      (error) => {
        this.spinner.hide();
        this.toastr.error('An unexpected error has occurred');
      }
    );
  }
  chageNodeName(nodeName: any) {
    this.listService = this.listNode.filter((x) => {
      return x.nodeName == nodeName;
    })[0]?.service;
  }
  cancel() {
    this.router.navigateByUrl('/settings');
  }

  addThresholdRule() {
    const elementThresholdRuleRequest: ThresholdRuleEdit = {
      id: 0,
      node_Setting: 0,
      environmentID: 0,
      machineName: '',
      monitoringType: 0,
      monitoringName: '',
      condition: 0,
      threshold: 0,
      thresholdCounter: 0,
      createDate: null,
      unit: '%',
    };

    this.nodeSettingsEdit.listThresholdRuleEdit.push(
      elementThresholdRuleRequest
    );
  }
  changeUnit(item: ThresholdRuleEdit) {
    item.unit = this.sysMonitor.filter((x) => {
      return x.id == item.monitoringType;
    })[0]?.unit;
  }
  remove(i: any) {
    this.nodeSettingsEdit.listThresholdRuleEdit.splice(i, 1);
  }

  updateNodeSetting() {
    this.invalidNotificationEmails = [];
    this.invalidReportEmails = [];
    const notificationEmails = this.nodeSettingsEdit.notificationEmail?.trim();
    const reportEmails = this.nodeSettingsEdit.reportEmail?.trim();
    if (this.nodeSettingsEdit.nodeType == 2) {
      if (
        this.nodeSettingsEdit.nodeName == null ||
        this.nodeSettingsEdit.nodeName == ''
      ) {
        this.toastr.error('Node Name is required');
        return;
      }
    }
    if (this.nodeSettingsEdit.nodeType != 2) {
      if (
        this.nodeSettingsEdit.domain_SystemHealth == null ||
        this.nodeSettingsEdit.domain_SystemHealth.trim() == ''
      ) {
        this.toastr.error('End-point is required');
        return;
      }
    }
    if (this.nodeSettingsEdit.nodeType != 2) {
      if (
        this.nodeSettingsEdit.appid == null ||
        this.nodeSettingsEdit.appid.trim() == ''
      ) {
        this.toastr.error('Private Key is required');
        return;
      }
    }
    if (this.nodeSettingsEdit.nodeType != 2) {
      if (
        this.nodeSettingsEdit.healthMeasurementKey == null ||
        this.nodeSettingsEdit.healthMeasurementKey.trim() == ''
      ) {
        this.toastr.error('Public Key is required');
        return;
      }
    }
    if (
      notificationEmails !== null &&
      notificationEmails !== undefined &&
      notificationEmails.length > 0
    ) {
      const listEmails = notificationEmails.split(';');
      for (let i = 0; i < listEmails.length; i++) {
        const subEmail = listEmails[i].trim();
        if (subEmail) {
          const listEmails_CommaSplit = subEmail.split(',');
          for (let j = 0; j < listEmails_CommaSplit.length; j++) {
            const email = listEmails_CommaSplit[j].trim();
            if (!Helper.emailRegex.test(email)) {
              this.invalidNotificationEmails.push(email);
            }
          }
        }
      }
    }
    if (
      reportEmails !== null &&
      reportEmails !== undefined &&
      reportEmails.length > 0
    ) {
      const listEmails = reportEmails.split(';');
      for (let i = 0; i < listEmails.length; i++) {
        const subEmail = listEmails[i].trim();
        if (subEmail) {
          const listEmails_CommaSplit = subEmail.split(',');
          for (let j = 0; j < listEmails_CommaSplit.length; j++) {
            const email = listEmails_CommaSplit[j].trim();
            if (!Helper.emailRegex.test(email)) {
              this.invalidReportEmails.push(email);
            }
          }
        }
      }
    }
    if (
      this.invalidNotificationEmails &&
      this.invalidNotificationEmails.length > 0
    ) {
      this.toastr.error('Email To Alert Stopped Service invalid format');
      return;
    }
    if (this.invalidReportEmails && this.invalidReportEmails.length > 0) {
      this.toastr.error('Report Emails invalid format');
      return;
    }
    let sysMonitor = this.sysMonitor;
    for (
      let i = 0;
      i < this.nodeSettingsEdit.listThresholdRuleEdit.length;
      i++
    ) {
      let monitoringtpi =
        this.nodeSettingsEdit.listThresholdRuleEdit[i].monitoringType;
      let condition = this.nodeSettingsEdit.listThresholdRuleEdit[i].condition;
      let threshold = this.nodeSettingsEdit.listThresholdRuleEdit[i].threshold;
      let thresholdCounter =
        this.nodeSettingsEdit.listThresholdRuleEdit[i].thresholdCounter;
      this.nodeSettingsEdit.listThresholdRuleEdit[i].monitoringName =
        sysMonitor.filter((x) => {
          return (
            x.id ==
            this.nodeSettingsEdit.listThresholdRuleEdit[i].monitoringType
          );
        })[0]?.name;
      if (monitoringtpi == 0) {
        this.toastr.error('KPI is required');
        return;
      }
      if (condition == 0) {
        this.toastr.error('Condition is required');
        return;
      }
      if (threshold == 0) {
        this.toastr.error('Threshold not suitable');
        return;
      }
      if (thresholdCounter == 0) {
        this.toastr.error('Threshold Counter not suitable');
        return;
      }
      for (
        let j = i + 1;
        j < this.nodeSettingsEdit.listThresholdRuleEdit.length;
        j++
      ) {
        let monitoringtpj =
          this.nodeSettingsEdit.listThresholdRuleEdit[j].monitoringType;
        if (monitoringtpi == monitoringtpj) {
          this.toastr.error('Duplicated KPI');
          return;
        }
      }
    }

    let serviceAddStr = '';
    for (let i = 0; i < this.servicesArr.length; i++) {
      if (serviceAddStr == '') {
        serviceAddStr = this.servicesArr[i];
      } else {
        serviceAddStr += ';' + this.servicesArr[i];
      }
    }

    this.nodeSettingsEdit.serviceList = serviceAddStr;
    this.nodeSettingsEdit.createDate = null;
    this.spinner.show();
    this.nodeSettingsService.updateNodeSetting(this.nodeSettingsEdit).subscribe(
      (res) => {
        this.spinner.hide();
        this.apiResponse = res;
        if (this.apiResponse.isSuccessed == true) {
          this.toastr.success('Update node setting success');
          this.submitted = false;
          this.router.navigateByUrl('/settings');
        } else {
          this.toastr.error(this.apiResponse.message);
        }
      },
      (error) => {
        this.spinner.hide();
        this.toastr.error('An unexpected error has occurred');
      },
      () => {
        this.invalidNotificationEmails = [];
        this.invalidReportEmails = [];
      }
    );
  }
  getEnvironment() {
    this.integrationService.getEnvironment().subscribe((res) => {
      this.listEnvironment = res;
    });
  }
  public onClose() {
    this.modalService.hide();
    this.submitted = false;
  }
  btnSaveAdd() {
    this.submitted = true;
    if (this.integrationFormSave.invalid) {
      return;
    }
    this.spinner.show();
    this.integrationService
      .createIntegrationAPI(this.integrationFormSave.value)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.toastr.success('Add node success');
            this.modalService.hide();
            this.submitted = false;
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
  openModal(template: TemplateRef<any>) {
    this.intitializeFormSave();
    this.getEnvironment();
    this.modalRef = this.modalService.show(template);
  }
  getNodeType() {
    this.integrationService.getNodeType().subscribe((res) => {
      this.listNodeType = res;
    });
  }
  changeKPIType(nodeType: any) {
    this.isNodeType = nodeType;
    this.sysMonitor = this.listMonitoring.filter((x) => {
      return x.nodeType == nodeType;
    });
    if (nodeType != this.nodeTypeDB) {
      this.nodeSettingsEdit.listThresholdRuleEdit = [];
    } else {
      this.nodeSettingsEdit.listThresholdRuleEdit =
        this.listThresholdRuleChange;
    }
  }
  btnCheckValidationModal() {
    let url = this.integrationFormSave.value.domain_SystemHealth.trim();
    if (url == '') {
      this.toastr.error('End-point is required');
      return;
    }
    this.integrationService.checkEndPoint(url).subscribe(
      (res) => {
        this.apiResponse = res;
        if (this.apiResponse.isSuccessed == true) {
          this.toastr.success('End-point Validated');
        } else {
          this.toastr.warning(this.apiResponse.message);
        }
      },
      (error) => {
        this.toastr.error('An unexpected error has occurred');
      }
    );
  }
  btnCheckValidation() {
    let url = this.nodeSettingsEdit.domain_SystemHealth ?? '';
    if (url.trim() == '') {
      this.toastr.error('End-point is required');
      return;
    }
    this.integrationService.checkEndPoint(url).subscribe(
      (res) => {
        this.apiResponse = res;
        if (this.apiResponse.isSuccessed == true) {
          this.toastr.success('End-point Validated');
        } else {
          this.toastr.warning(this.apiResponse.message);
        }
      },
      (error) => {
        this.toastr.error('An unexpected error has occurred');
      }
    );
  }
}
