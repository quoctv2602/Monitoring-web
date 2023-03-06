import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { ApiResponse } from 'src/app/_model/node-settings-model';
import { SysEnvironment } from 'src/app/_model/sys-environment';
import { IntegrationService } from 'src/app/_service/integration.service';

@Component({
  selector: 'app-integration-add',
  templateUrl: './integration-add.component.html',
  styleUrls: ['./integration-add.component.css'],
})
export class IntegrationAddComponent extends BaseComponent implements OnInit {
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
  submitted = false;
  listEnvironment: SysEnvironment[] = [];
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  urlRegex =
    /^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/;
  constructor(
    public fb: FormBuilder,
    private integrationService: IntegrationService,
    private router: Router,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    let role = this.roles.indexOf(
      this.globalEnumResult.ActionEnum.kpiSettingsManageNode
    );
    if (role === -1) {
      this.router.navigateByUrl('/access-denied');
    }
    this.getEnvironment();
    this.intitializeFormSave();
  }
  get f(): { [key: string]: AbstractControl } {
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

  getEnvironment() {
    this.integrationService.getEnvironment().subscribe((res) => {
      this.listEnvironment = res;
    });
  }
  btnCancel() {
    this.router.navigateByUrl('/integration');
  }
  btnSave() {
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
            this.router.navigateByUrl('/integration');
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
  btnCheckValidation() {
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
}
