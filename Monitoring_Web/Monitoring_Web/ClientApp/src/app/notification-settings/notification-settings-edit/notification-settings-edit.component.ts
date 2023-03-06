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
import { ActivatedRoute, Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { Helper } from 'src/app/_common/_helper';
import { MonitoringEdit } from 'src/app/_model/INotificationModel';
import { ApiResponse } from 'src/app/_model/node-settings-model';
import { SysMonitoring } from 'src/app/_model/sys-monitoring';
import { NodeSettingsService } from 'src/app/_service/node-settings.service';
import { NotificationService } from 'src/app/_service/notification.service';

@Component({
  selector: 'app-notification-settings-edit',
  templateUrl: './notification-settings-edit.component.html',
  styleUrls: ['./notification-settings-edit.component.css'],
})
export class NotificationSettingsEditComponent extends BaseComponent implements OnInit {
  listMonitoring: SysMonitoring[] = [];
  notificationFormEdit: FormGroup = this.fb.group({
    id: new FormControl(),
    name: new FormControl(),
    notificationOption: new FormControl(),
    kpi: new FormControl(),
    emails: new FormControl(),
    notificationAlias: new FormControl(),
    updateBy: new FormControl(),
  });
  submitted = false;
  apiResponse: ApiResponse = {
    isSuccessed: false,
    message: '',
  };
  KPI: any = [];
  ListDetail: MonitoringEdit[] = [];
  constructor(
    private nodeSettingsService: NodeSettingsService,
    public fb: FormBuilder,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute
  ) {super();}

  ngOnInit() {
    super.ngOnInit();
    let role=this.roles.indexOf(this.globalEnumResult.ActionEnum.notificationSettingsManage);
    if(role===-1){
      this.router.navigateByUrl('/access-denied');
    }
    let id = this.route.snapshot.paramMap.get('id') ?? '0';
    this.nodeSettingsService.getSysMonitoring().subscribe((res) => {
      this.listMonitoring = res;
    });
    this.getNotificationEdit(parseInt(id));
    this.intitializeFormSave();
  }
  get f(): { [key: string]: AbstractControl } {
    return this.notificationFormEdit.controls;
  }
  intitializeFormSave() {
    this.notificationFormEdit = this.fb.group({
      id: 0,
      name: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          Validators.maxLength(250),
        ],
      ],
      notificationOption: 1,
      kpi: [[], [Validators.required]],
      emails: [
        '',
        [
          Validators.required,
          this.noWhitespaceValidator,
          this.emailValidator('emails'),
        ],
      ],
      notificationAlias: ['', [Validators.maxLength(500)]],
      updateBy: 0,
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
  getNotificationEdit(id: number) {
    this.spinner.show();
    this.notificationService.getNotificationEdit(id).subscribe((res) => {
      this.ListDetail = res.kpi;
      this.notificationFormEdit.patchValue({
        ...res,
        kpi: res.kpi.map((item) => {
          return item.monitoringId;
        }),
      });
      this.spinner.hide();
    },(error) => {
      this.spinner.hide();
      this.toastr.error('An unexpected error has occurred');
    }
    );
  }
  btnSave() {
    this.submitted = true;
    if (this.notificationFormEdit.invalid) {
      return;
    }
    let monitoringArr = this.notificationFormEdit.value.kpi;
    for (let i = 0; i < monitoringArr.length; i++) {
      const monitoring = {
        id: 0,
        monitoringId: 0,
      };
      monitoring.monitoringId = monitoringArr[i];
      let id =
        this.ListDetail.filter((x) => {
          return x.monitoringId == monitoringArr[i];
        })[0]?.id ?? 0;
      monitoring.id = id;
      this.KPI.push(monitoring);
    }
    this.notificationFormEdit.value.kpi = this.KPI;
    this.spinner.show();
    this.notificationService
      .updateNotification(this.notificationFormEdit.value)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.apiResponse = res;
          if (this.apiResponse.isSuccessed == true) {
            this.toastr.success('Update notification success');
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
