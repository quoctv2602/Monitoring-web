import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { ApiResponse } from 'src/app/_model/node-settings-model';
import { SysMonitoring } from 'src/app/_model/sys-monitoring';
import { NodeSettingsService } from 'src/app/_service/node-settings.service';
import { NotificationService } from 'src/app/_service/notification.service';

@Component({
  selector: 'app-notification-settings-add',
  templateUrl: './notification-settings-add.component.html',
  styleUrls: ['./notification-settings-add.component.css'],
})
export class NotificationSettingsAddComponent
  extends BaseComponent
  implements OnInit
{
  listMonitoring: SysMonitoring[] = [];
  notificationFormAdd: FormGroup = this.fb.group({
    name: new FormControl(),
    notificationOption: new FormControl(),
    kpi: new FormControl(),
    emails: new FormControl(),
    notificationAlias: new FormControl(),
    createdBy: new FormControl(),
  });
  submitted = false;
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  KPI: any = [];

  constructor(
    private nodeSettingsService: NodeSettingsService,
    public fb: FormBuilder,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private router: Router
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    let role = this.roles.indexOf(
      this.globalEnumResult.ActionEnum.notificationSettingsManage
    );
    if (role === -1) {
      this.router.navigateByUrl('/access-denied');
    }
    this.intitializeFormSave();
    this.nodeSettingsService.getSysMonitoring().subscribe((res) => {
      this.listMonitoring = res;
    });
  }
  get f(): { [key: string]: AbstractControl } {
    return this.notificationFormAdd.controls;
  }
  intitializeFormSave() {
    this.notificationFormAdd = this.fb.group({
      name: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          Validators.maxLength(250),
        ],
      ],
      notificationOption: 1,
      kpi: ['', [Validators.required]],
      emails: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          this.emailValidator('emails'),
        ],
      ],
      notificationAlias: ['', [Validators.maxLength(500)]],
      createdBy: 0,
    });
  }
  noWhitespaceValidator(control: FormControl): ValidationErrors | null {
    const isWhitespace = (control.value || '').trim().length === 0;
    return isWhitespace ? { whitespace: true } : null;
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
  btnSave() {
    this.submitted = true;
    if (this.notificationFormAdd.invalid) {
      return;
    }
    let monitoringArr = this.notificationFormAdd.value.kpi;
    for (let i = 0; i < monitoringArr.length; i++) {
      const monitoring = {
        monitoringId: 0,
      };
      monitoring.monitoringId = monitoringArr[i];
      this.KPI.push(monitoring);
    }
    this.notificationFormAdd.value.kpi = this.KPI;
    this.spinner.show();
    this.notificationService
      .createNotification(this.notificationFormAdd.value)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.toastr.success('Add notification success');
            this.router.navigateByUrl('/notification');
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
  btnCancel() {
    this.router.navigateByUrl('/notification');
  }
}
