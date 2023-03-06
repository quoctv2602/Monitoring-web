import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HeaderComponent } from './layouts/header/header.component';
import { FooterComponent } from './layouts/footer/footer.component';
import { SidebarComponent } from './layouts/sidebar/sidebar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NodeSettingsEditComponent } from './settings/node-settings-edit/node-settings-edit.component';
import { SettingsComponent } from './settings/settings.component';
import { NodeSettingsComponent } from './settings/node-settings/node-settings.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { IntegrationComponent } from './integration/integration.component';
import { COMPONENT_Pages_Dashboard } from './dashboard';
import { DashboardComponent } from './dashboard/dashboard.component';
import { COMPONENT_Pages, Router_Page } from './Pages';
import { AuthGuard } from './_guards/auth.guard';
import { MonitoringReportingAdvanceComponent } from './monitoring-reporting/monitoring-reporting-advance/monitoring-reporting-advance.component';
import { MonitoringReportingBasicComponent } from './monitoring-reporting/monitoring-reporting-basic/monitoring-reporting-basic.component';
import { DocumentComponent } from './document/document.component';
import { ProfileComponent } from './document/profile/profile.component';
import { IntegrationAddComponent } from './integration/integration-add/integration-add.component';
import { IntegrationEditComponent } from './integration/integration-edit/integration-edit.component';
import { DocumentBytkComponent } from './monitoring-reporting/document-bytk/document-bytk.component';
import { ViewLogsComponent } from './monitoring-reporting/view-logs/view-logs.component';
import { ViewDataComponent } from './monitoring-reporting/view-data/view-data.component';
import { ViewConfigComponent } from './monitoring-reporting/view-config/view-config.component';
import { NotificationSettingsComponent } from './notification-settings/notification-settings.component';
import { NotificationSettingsAddComponent } from './notification-settings/notification-settings-add/notification-settings-add.component';
import { NotificationSettingsEditComponent } from './notification-settings/notification-settings-edit/notification-settings-edit.component';
import { UsersComponent } from './users/users.component';
import { GroupsComponent } from './groups/groups.component';
import { UserPermissionsComponent } from './user-permissions/user-permissions.component';
import { UserDetailComponent } from './users/user-detail/user-detail.component';
import { GroupDetailComponent } from './groups/group-detail/group-detail.component';
import { AssignPermissionComponent } from './user-permissions/assign-permission/assign-permission.component';
import { PopoverModule } from 'ngx-bootstrap/popover';
import {
  MsalGuard,
  MsalInterceptor,
  MsalModule,
  MsalRedirectComponent,
} from '@azure/msal-angular'; // Updated import
import {
  InteractionType,
  ProtocolMode,
  PublicClientApplication,
} from '@azure/msal-browser';
import { globalsettings } from '../assets/globalsetting';
import { LoginSuccessComponent } from './Pages/login-success/login-success.component';
import { JwtInterceptor } from './_interceptor/jwt.interceptor';
import { AccessDeniedComponent } from './access-denied/access-denied.component';
const azureAD = globalsettings.IDP;
const isIE =
  window.navigator.userAgent.indexOf('MSIE ') > -1 ||
  window.navigator.userAgent.indexOf('Trident/') > -1;

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    NodeSettingsComponent,
    COMPONENT_Pages_Dashboard,
    COMPONENT_Pages,
    HeaderComponent,
    FooterComponent,
    SidebarComponent,
    SettingsComponent,
    NodeSettingsEditComponent,
    IntegrationComponent,
    MonitoringReportingBasicComponent,
    MonitoringReportingAdvanceComponent,
    DocumentComponent,
    ProfileComponent,
    IntegrationAddComponent,
    IntegrationEditComponent,
    DocumentBytkComponent,
    ViewLogsComponent,
    ViewDataComponent,
    ViewConfigComponent,
    NotificationSettingsComponent,
    NotificationSettingsAddComponent,
    NotificationSettingsEditComponent,
    UserPermissionsComponent,
    UsersComponent,
    GroupsComponent,
    UserDetailComponent,
    GroupDetailComponent,
    AssignPermissionComponent,
    AccessDeniedComponent,
    LoginSuccessComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    NgSelectModule,
    NgxPaginationModule,
    NgxSpinnerModule,
    ModalModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      messageClass: '',
    }),
    PopoverModule.forRoot(),
    Router_Page,
    MsalModule.forRoot(
      new PublicClientApplication({
        auth: {
          authority: azureAD.Authority,
          clientId: azureAD.ClientId,
          redirectUri: azureAD.CallbackPath,
          postLogoutRedirectUri: azureAD.SignedOutCallbackPath,
          protocolMode: ProtocolMode.OIDC,
          navigateToLoginRequestUrl: false,
        },
        cache: {
          cacheLocation: 'localStorage',
          storeAuthStateInCookie: isIE,
        },
      }),
      {
        interactionType: InteractionType.Redirect,
        authRequest: {
          scopes: ['user.read'],
        },
      },
      {
        interactionType: InteractionType.Redirect, // MSAL Interceptor Configuration
        protectedResourceMap: new Map([
          ['https://graph.microsoft-ppe.com/v1.0/me', ['user.read']],
        ]),
      }
    ),
    RouterModule.forRoot([
      {
        path: '',
        component: HomeComponent,
        pathMatch: 'full',
        canActivate: [AuthGuard],
      },
      {
        path: 'counter',
        component: CounterComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'settings',
        component: SettingsComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'node-settings',
        component: NodeSettingsComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'node-settings-edit/:id',
        component: NodeSettingsEditComponent,
        canActivate: [AuthGuard],
      },
      { path: 'fetch-data', component: FetchDataComponent },
      {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'integration',
        component: IntegrationComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'monitoring-reporting-basic',
        component: MonitoringReportingBasicComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'monitoring-reporting-advance',
        component: MonitoringReportingAdvanceComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'document',
        component: ProfileComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'integration-add',
        component: IntegrationAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'integration-edit/:id',
        component: IntegrationEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'document-bytk/:token',
        component: DocumentBytkComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'view-logs/:transactionKey',
        component: ViewLogsComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'view-data/:token',
        component: ViewDataComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'view-config/:token',
        component: ViewConfigComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'notification',
        component: NotificationSettingsComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'notification-add',
        component: NotificationSettingsAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'notification-edit/:id',
        component: NotificationSettingsEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'user-permission',
        component: UserPermissionsComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'user-detail/:id',
        component: UserDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'user-detail',
        component: UserDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'group-detail',
        component: GroupDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'group-detail/:id',
        component: GroupDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'assign-permission/:id',
        component: AssignPermissionComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'SSOLogin',
        component: LoginSuccessComponent,
      },
      {
        path: 'access-denied',
        component: AccessDeniedComponent,
      },
    ]),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
})
export class AppModule {}
