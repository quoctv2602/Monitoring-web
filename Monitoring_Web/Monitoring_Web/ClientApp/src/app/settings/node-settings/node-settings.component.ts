import { Component, OnInit, EventEmitter, TemplateRef } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { NodeType } from 'src/app/_model/integration-api-model';
import {
  ApiResponse,
  EnvironmentModel,
  IntegrationAPI,
  NodeSetingsModel,
  Service,
} from 'src/app/_model/node-settings-model';
import { IntegrationService } from 'src/app/_service/integration.service';
import { ThresholdRuleRequest } from '../../_model/node-setting-request';
import { SysEnvironment } from '../../_model/sys-environment';
import { SysMonitoring } from '../../_model/sys-monitoring';
import { NodeSettingsService } from '../../_service/node-settings.service';
@Component({
  selector: 'app-node-settings',
  templateUrl: './node-settings.component.html',
  styleUrls: ['./node-settings.component.css'],
})
export class NodeSettingsComponent extends BaseComponent implements OnInit {
  nodeSettingForm: FormGroup = this.fb.group({
    nodeName: new FormControl(),
    nodeType: new FormControl(),
    environmentID: new FormControl(),
    machineName: new FormControl(),
    description: new FormControl(),
    serviceList: new FormControl(),
    notificationEmail: new FormControl(),
    reportEmail: new FormControl(),
    notificationAlias: new FormControl(),
    reportAlias: new FormControl(),
    domain_SystemHealth: new FormControl(),
    appid: new FormControl(),
    healthMeasurementKey: new FormControl(),
    listThresholdRuleRequest: new FormArray([]),
  });
  DetailNotificationThresholds: ThresholdRuleRequest[] = [];
  cancelNodeSetting = new EventEmitter();
  sysMonitor: SysMonitoring[] = [];
  nodeSettingsModel: NodeSetingsModel = {
    listEnvironments: [],
  };
  submitted = false;
  sysEnvironment: EnvironmentModel[] = [];
  listNode: IntegrationAPI[] = [];
  listService: Service[] = [];
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  urlRegex =
    /^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/;
  isNodeType = Helper.defaultNodeType;
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
  listMonitoring: SysMonitoring[] = [];
  constructor(
    private nodeSettingsService: NodeSettingsService,
    private toastr: ToastrService,
    public fb: FormBuilder,
    private router: Router,
    private spinner: NgxSpinnerService,
    private modalService: BsModalService,
    private integrationService: IntegrationService
  ) {super();}

  ngOnInit() {
    super.ngOnInit();
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.kpiSettingsSetKPI);
    if(role===-1){
      this.router.navigateByUrl('/access-denied');
    }
    this.spinner.show();
    this.nodeSettingsService.getSysEnvironment().subscribe((res) => {
      this.spinner.hide();
      this.nodeSettingsModel = res;
      this.sysEnvironment = this.nodeSettingsModel.listEnvironments;
    });
    this.nodeSettingsService.getSysMonitoring().subscribe((res) => {
      this.listMonitoring = res;
      this.sysMonitor = this.listMonitoring.filter((x) => {
        return x.nodeType == Helper.defaultNodeType;
      });
    });
    this.getNodeType();
    this.intitializeForm(this.isNodeType);
  }
  get f(): { [key: string]: AbstractControl } {
    return this.nodeSettingForm.controls;
  }
  get g(): { [key: string]: AbstractControl } {
    return this.integrationFormSave.controls;
  }
  intitializeForm(kpiType: any) {
    if (kpiType == Helper.defaultNodeType) {
      this.nodeSettingForm = this.fb.group({
        nodeName: ['', Validators.required],
        nodeType: kpiType,
        environmentID: ['', Validators.required],
        machineName: '',
        description: '',
        serviceList: '',
        notificationEmail: ['', [this.emailValidator('notificationEmail')]],
        reportEmail: ['', [this.emailValidator('reportEmail')]],
        notificationAlias: '',
        reportAlias: '',
        domain_SystemHealth: '',
        appid: '',
        healthMeasurementKey: '',
        listThresholdRuleRequest: this.fb.array([
          this.fb.group({
            monitoringType: 0,
            condition: 0,
            threshold: 0,
            thresholdCounter: 0,
            unit: '',
          }),
        ]),
      });
    } else {
      this.nodeSettingForm = this.fb.group({
        nodeName: '',
        nodeType: kpiType,
        environmentID: ['', Validators.required],
        machineName: '',
        description: '',
        serviceList: '',
        notificationEmail: ['', [this.emailValidator('notificationEmail')]],
        reportEmail: ['', [this.emailValidator('reportEmail')]],
        notificationAlias: '',
        reportAlias: '',
        domain_SystemHealth: [
          '',
          [
            Validators.required,
            Validators.maxLength(500),
            Validators.pattern(this.urlRegex),
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
        healthMeasurementKey: [
          '',
          [
            Validators.required,
            this.noWhitespaceValidator,
            Validators.maxLength(4000),
          ],
        ],
        listThresholdRuleRequest: this.fb.array([
          this.fb.group({
            monitoringType: 0,
            condition: 0,
            threshold: 0,
            thresholdCounter: 0,
            unit: '',
          }),
        ]),
      });
    }
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
  createNodeSetting() {
    let sysMonitor = this.sysMonitor;
    this.submitted = true;
    if (this.nodeSettingForm.invalid) {
      return;
    }
    for (let i = 0; i < this.DetailNotificationThresholds.length; i++) {
      let monitoringtpi = this.DetailNotificationThresholds[i].monitoringType;
      let condition = this.DetailNotificationThresholds[i].condition;
      let threshold = this.DetailNotificationThresholds[i].threshold;
      let thresholdCounter =
        this.DetailNotificationThresholds[i].thresholdCounter;
      this.DetailNotificationThresholds[i].monitoringName = sysMonitor.filter(
        (x) => {
          return x.id == this.DetailNotificationThresholds[i].monitoringType;
        }
      )[0]?.name;
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
      for (let j = i + 1; j < this.DetailNotificationThresholds.length; j++) {
        let monitoringtpj = this.DetailNotificationThresholds[j].monitoringType;
        if (monitoringtpi == monitoringtpj) {
          this.toastr.error('Duplicated KPI');
          return;
        }
      }
    }

    let listNodeSetting = this.nodeSettingForm.value;
    let serviceAddStr = '';
    for (let i = 0; i < listNodeSetting.serviceList.length; i++) {
      if (serviceAddStr == '') {
        serviceAddStr = listNodeSetting.serviceList[i];
      } else {
        serviceAddStr += ';' + listNodeSetting.serviceList[i];
      }
    }
    let nodeName = this.listNode.filter((x) => {
      return x.id == listNodeSetting.nodeName;
    })[0]?.nodeName;
    listNodeSetting.serviceList = serviceAddStr;
    listNodeSetting.nodeName = nodeName;
    listNodeSetting.listThresholdRuleRequest =
      this.DetailNotificationThresholds;
    this.spinner.show();
    this.nodeSettingsService.createNodeSetting(listNodeSetting).subscribe(
      (res) => {
        this.apiResponse = res;
        this.spinner.hide();
        if (this.apiResponse.isSuccessed == true) {
          this.toastr.success('Add node setting success');
          this.router.navigateByUrl('/settings');
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
  addThresholdRule() {
    const elementThresholdRuleRequest: ThresholdRuleRequest = {
      monitoringType: 0,
      monitoringName: '',
      condition: 0,
      threshold: 0,
      thresholdCounter: 0,
      unit: '%',
    };

    this.DetailNotificationThresholds.push(elementThresholdRuleRequest);
  }
  remove(i: any) {
    this.DetailNotificationThresholds.splice(i, 1);
  }
  cancel() {
    this.router.navigateByUrl('/settings');
  }
  changeUnit(item: ThresholdRuleRequest) {
    item.unit = this.sysMonitor.filter((x) => {
      return x.id == item.monitoringType;
    })[0]?.unit;
  }
  changeEnvironments(id: any) {
    this.nodeSettingForm.controls.nodeName.setValue('');
    this.nodeSettingForm.controls.serviceList.setValue('');
    this.listNode = this.nodeSettingsModel.listEnvironments.filter((x) => {
      return x.id == id;
    })[0]?.listIntegrationAPI;
  }
  chageNodeName(id: any) {
    this.listService = this.listNode.filter((x) => {
      return x.id == id;
    })[0]?.service;
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
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.spinner.hide();
            this.toastr.success('Add node success');
            this.nodeSettingsService.getSysEnvironment().subscribe((res) => {
              this.nodeSettingsModel = res;
              this.sysEnvironment = this.nodeSettingsModel.listEnvironments;
            });
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

  emailValidator(controlName: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const emails = (control.value as string).trim();
      if (emails !== null && emails !== undefined && emails.length > 0) {
        const listEmails = emails.split(';');
        for (let i = 0; i < listEmails.length; i++) {
          const subEmail = listEmails[i].trim();
          if (subEmail) {
            const listEmails_CommaSplit = subEmail.split(',');
            for (let j = 0; j < listEmails_CommaSplit.length; j++) {
              const email = listEmails_CommaSplit[j].trim();
              if (!Helper.emailRegex.test(email)) {
                return { [controlName]: { value: control.value } };
              }
            }
          }
        }
        return null;
      } else return null;
    };
  }

  changeKPIType(nodeType: any) {
    this.submitted = false;
    this.isNodeType = nodeType;
    this.intitializeForm(nodeType);
    this.sysMonitor = this.listMonitoring.filter((x) => {
      return x.nodeType == nodeType;
    });
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
    let url = this.nodeSettingForm.value.domain_SystemHealth.trim();
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
}
