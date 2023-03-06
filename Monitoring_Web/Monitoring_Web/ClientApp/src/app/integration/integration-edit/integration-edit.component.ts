import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { ApiResponse } from 'src/app/_model/node-settings-model';
import { SysEnvironment } from 'src/app/_model/sys-environment';
import { IntegrationService } from 'src/app/_service/integration.service';

@Component({
  selector: 'app-integration-edit',
  templateUrl: './integration-edit.component.html',
  styleUrls: ['./integration-edit.component.css'],
})
export class IntegrationEditComponent extends BaseComponent implements OnInit {
  integrationFormEdit: FormGroup = this.fb.group({
    id: new FormControl({ value: 1, disabled: true }),
    environmentID: new FormControl(),
    machineName: new FormControl({ value: '', disabled: true }),
    healthMeasurementKey: new FormControl(),
    appid: new FormControl(),
    domain_SystemHealth: new FormControl(),
    isActive: new FormControl(),
    createDate: new FormControl(),
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
  isSave = false;

  isDefault: boolean = false;
  constructor(
    private integrationService: IntegrationService,
    private router: Router,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private route: ActivatedRoute,
    public fb: FormBuilder
  ) {super();}

  ngOnInit() {
    super.ngOnInit();
    let id = this.route.snapshot.paramMap.get('id') ?? '0';
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.kpiSettingsManageNode);
    if(role===-1){
      this.router.navigateByUrl('/access-denied');
    }
    this.intitializeFormSave();
    this.getEnvironment();
    this.getNodeEdit(parseInt(id));
  }
  get f(): { [key: string]: AbstractControl } {
    return this.integrationFormEdit.controls;
  }
  intitializeFormSave() {
    this.integrationFormEdit = this.fb.group({
      id: 1,
      environmentID: [{ value: 1 }, Validators.required],
      machineName: [
        { value: '', disabled: true },
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
      nodeType: [
        { value: Helper.defaultNodeType, disabled: true },
        Validators.required,
      ],
      isActive: true,
      createDate: null,
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
  getNodeEdit(id: number) {
    this.spinner.show();
    this.integrationService.getIntegrationAPIEdit(id).subscribe((res) => {
      this.integrationFormEdit.patchValue(res);
      this.isSave = this.integrationFormEdit.value.isActive;
      this.isDefault = this.integrationFormEdit.value.isDefaultNode;
      this.spinner.hide();
    });
  }
  getEnvironment() {
    this.integrationService.getEnvironment().subscribe((res) => {
      this.listEnvironment = res;
    });
  }
  btnCancel() {
    this.router.navigateByUrl('/integration');
  }
  btnSaveEdit() {
    this.submitted = true;
    if (this.integrationFormEdit.invalid) {
      return;
    }
    if (this.integrationFormEdit.value.isActive === false) {
      if (
        !this.isDefault &&
        this.integrationFormEdit.value.isDefaultNode === true
      ) {
        this.toastr.error('This node does not activate yet');
        return;
      }
    }
    this.spinner.show();
    this.integrationService
      .updateIntegrationAPI(this.integrationFormEdit.value)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.toastr.success('Save node name success');
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
    let url = this.integrationFormEdit.value.domain_SystemHealth.trim();
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
