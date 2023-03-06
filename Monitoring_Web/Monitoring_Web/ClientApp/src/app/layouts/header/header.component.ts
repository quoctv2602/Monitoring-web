import {
  Component,
  OnInit,
  Inject,
  Input,
  SimpleChanges,
  Output,
  EventEmitter,
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { MsalService } from '@azure/msal-angular';
import { AccountService } from 'src/app/_service/account.service';
import { ToastrService } from 'ngx-toastr';
import { ConfigService } from 'src/app/_service/config.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent implements OnInit {
  @Input() msg: any;
  @Input() msgNew: any;
  @Output() Clear: any = new EventEmitter();
  tempModalView: any;
  tempModalViewId: any;

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private authService: MsalService,
    private _accountService: AccountService,
    private _toastService: ToastrService
  ) {}
  profile = {
    name: 'User',
    username: 'UserName',
  };

  ngOnInit(): void {
    this.document.body.classList.toggle('toggle-sidebar'); // khi nao lam menu thi xoa doan nay
    try {
      const user = localStorage.getItem('storage_profile');
      if (user) this.profile = JSON.parse(user as string);
    } catch (er) {}

    setInterval(() => {
      try {
        this.msg = JSON.parse(localStorage.getItem('message') as string);
        this.msg = this.msg.sort(
          (objA: any, objB: any) =>
            new Date(objB.timestamp).getTime() -
            new Date(objA.timestamp).getTime()
        );
      } catch (error) {}
    }, 1000);
  }

  ngOnChanges(changes: SimpleChanges): void {
    //Called before any other lifecycle hook. Use it to inject dependencies, but avoid any serious work here.
    //Add '${implements OnChanges}' to the class.
    this.msg = this.msg.sort(
      (objA: any, objB: any) =>
        new Date(objB.timestamp).getTime() - new Date(objA.timestamp).getTime()
    );
    // this.isShow = 1
    // setTimeout(() => {
    //   this.isShow = 0
    // }, 10000);
  }
  getClass(item: any, type: any) {
    //bi-x-circle
    //bi-exclamation-circle
    //bi-check-circle
    //bi-info-circle
    if (type == 'icon') {
      let icon = '';
      if (item == 'info') {
        icon = 'info';
      } else if (item == 'danger') {
        icon = 'x';
      } else if (item == 'success') {
        icon = 'check';
      } else if (item == 'warning') {
        icon = 'exclamation';
      }
      return 'bi bi-' + icon + '-circle text-' + item;
    } else return 'alert alert-' + item + ' alert-dismissible fade show m-auto';
  }
  CountNotification(isRead: number) {
    try {
      const CountRead = this.msg.filter((m: any) => m.isRead == isRead);
      return CountRead.length;
    } catch (error) {
      return 0;
    }
  }
  ReadAll() {
    this.msg.forEach((m: any) => (m.isRead = 1));
    localStorage.setItem('message', JSON.stringify(this.msg));
  }
  ClearNotification(tempModalViewId: any) {
    this.msg.forEach((m: any) => {
      if (m.id == tempModalViewId) {
        this.msg = this.msg.filter((item: any) => item.id !== m.id);
      }
    });
    localStorage.setItem('message', JSON.stringify(this.msg));

    //this.data = this.data.filter(item => item !== data_item);
  }
  ClearAll() {
    this.msg = undefined;

    this.Clear.emit('clear');
  }
  viewNotification(item: any) {
    this.tempModalView = item.message;
    this.tempModalViewId = item.id;
    item.isRead = 1;
    localStorage.setItem('message', JSON.stringify(this.msg));
  }

  sidebarToggle() {
    //toggle sidebar function
    this.document.body.classList.toggle('toggle-sidebar');
  }

  logOut() {
    this._accountService.logOut().subscribe(
      (respone) => {
        if (respone > 0) {
          if (localStorage.getItem('storage_t'))
            localStorage.removeItem('storage_t');
          if (localStorage.getItem('storage_p'))
            localStorage.removeItem('storage_p');
          if (localStorage.getItem('storage_p_d'))
            localStorage.removeItem('storage_p_d');
          if (localStorage.getItem('storage_profile'))
            localStorage.removeItem('storage_profile');
          this.authService.logoutRedirect();
        }
      },
      (err) => {
        this._toastService.error('An unexpected error has occurred');
      },
      () => {}
    );
  }
}
