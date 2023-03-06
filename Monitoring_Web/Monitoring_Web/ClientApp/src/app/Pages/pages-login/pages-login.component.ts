// import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { ProtocolMode, PublicClientApplication } from '@azure/msal-browser';
// // import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
// import { filter } from 'rxjs/operators';
// const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';
const isIE =
  window.navigator.userAgent.indexOf('MSIE ') > -1 ||
  window.navigator.userAgent.indexOf('Trident/') > -1;
@Component({
  selector: 'app-pages-login',
  templateUrl: './pages-login.component.html',
  styleUrls: ['./pages-login.component.css'],
})
export class PagesLoginComponent implements OnInit {
  constructor(
    private router: Router,
    private authService: MsalService //     private msalBroadcastService: MsalBroadcastService, //     private http: HttpClient
  ) {}

  ngOnInit(): void {
    localStorage.setItem('message', '[]');
    const isDelete = localStorage.getItem('storage_i_d')
      ? parseInt(localStorage.getItem('storage_i_d') as string)
      : 0;
    if (isDelete === 1) {
      return;
    } else {
      if (this.authService.instance.getAllAccounts().length > 0) {
        const pageDefault = localStorage.getItem('storage_p_d');
        this.router.navigate(['/' + pageDefault ?? 'dashboard']);
      }
    }
  }

  login() {
    this.authService.loginRedirect();
  }
}
