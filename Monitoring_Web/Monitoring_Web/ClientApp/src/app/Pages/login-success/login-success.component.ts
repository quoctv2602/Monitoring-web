import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { AccountInfo, EventMessage, EventType } from '@azure/msal-browser';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { filter } from 'rxjs';
import { AccountService } from 'src/app/_service/account.service';

@Component({
  selector: 'app-login-success',
  templateUrl: './login-success.component.html',
  styleUrls: ['./login-success.component.css'],
})
export class LoginSuccessComponent implements OnInit {
  modalRef?: BsModalRef;

  @ViewChild('dialog') input: any;

  constructor(
    private msalBroadcastService: MsalBroadcastService,
    private _accountService: AccountService,
    private router: Router,
    private spinner: NgxSpinnerService,
    private authService: MsalService,
    private toasService: ToastrService,
    public modalService: BsModalService
  ) {}

  ngOnInit() {
    this.spinner.show();
    const isDelete = localStorage.getItem('storage_i_d')
      ? parseInt(localStorage.getItem('storage_i_d') as string)
      : 0;
    if (
      this.authService.instance.getAllAccounts().length > 0 &&
      isDelete !== 1
    ) {
      const pageDefault = localStorage.getItem('storage_p_d');
      this.spinner.hide();
      this.router.navigate(['/' + pageDefault ? pageDefault : 'dashboard']);
    } else {
      if (this.authService.instance.getAllAccounts().length > 0) {
        var defaultPage: string = 'dashboard';
        var isDeleteRespone: boolean = false;
        this._accountService.loginSuccess().subscribe(
          (respone) => {
            isDeleteRespone = respone.isDelete ?? false;
            localStorage.setItem(
              'storage_p',
              respone.permission ? JSON.stringify(respone.permission) : '[]'
            );
            if (respone.defaultPage) defaultPage = respone.defaultPage;
          },
          (err) => {},
          () => {
            this.spinner.hide();
            if (isDeleteRespone === false) {
              localStorage.removeItem('storage_i_d');
              localStorage.setItem('storage_p_d', defaultPage);
              this.router.navigate(['/' + defaultPage]);
            } else {
              localStorage.setItem('storage_i_d', '1');
              // this.toasService.error('This user is inactivated', '', {
              //   timeOut: 3000,
              // });
              // this.router.navigate(['/login']);
              this.modalRef = this.modalService.show(this.input);
            }
          }
        );
      } else
        this.msalBroadcastService.msalSubject$
          .pipe(
            filter(
              (msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS
            )
          )
          .subscribe((result: EventMessage) => {
            const AccountInfo = result.payload as any;
            const token = AccountInfo.idToken ?? '';
            const name = AccountInfo.account.name;
            localStorage.setItem('storage_t', token);
            localStorage.setItem(
              'storage_profile',
              JSON.stringify({ userName: name, name: name })
            );
            var defaultPage: string = 'dashboard';
            var isDelete: boolean = false;
            this._accountService.loginSuccess().subscribe(
              (respone) => {
                isDelete = respone.isDelete ?? false;
                localStorage.setItem(
                  'storage_p',
                  respone.permission ? JSON.stringify(respone.permission) : '[]'
                );
                if (respone.defaultPage) defaultPage = respone.defaultPage;
              },
              (err) => {},
              () => {
                this.spinner.hide();
                if (isDelete === false) {
                  localStorage.removeItem('storage_i_d');
                  localStorage.setItem('storage_p_d', defaultPage);
                  this.router.navigate(['/' + defaultPage]);
                } else {
                  localStorage.setItem('storage_i_d', '1');
                  // this.toasService.error('This user is inactivated', '', {
                  //   timeOut: 3000,
                  // });
                  this.modalRef = this.modalService.show(this.input);
                }
              }
            );
          });
    }
  }

  onClickInactiveDialog() {
    this.modalService.hide();
    setTimeout(() => {
      this.router.navigate(['/login']);
    }, 200);
  }
}
