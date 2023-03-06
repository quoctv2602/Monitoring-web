import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-permissions',
  templateUrl: './user-permissions.component.html',
  styleUrls: ['./user-permissions.component.css'],
})
export class UserPermissionsComponent implements OnInit, OnDestroy {
  activeTab: string = 'group';

  constructor() {}

  ngOnInit() {
    const isBackFromSubMenu = sessionStorage.getItem('isBackFromSubMenu')
      ? parseInt(sessionStorage.getItem('isBackFromSubMenu') as string)
      : 0;
    const _activeTab = sessionStorage.getItem('user_permission_active_tab');
    if (_activeTab !== null && isBackFromSubMenu === 1) {
      this.activeTab = _activeTab.toLowerCase();
    }
    sessionStorage.removeItem('isBackFromSubMenu');
  }

  ngOnDestroy(): void {
    const isBackFromSubMenu = sessionStorage.getItem('isBackFromSubMenu')
      ? parseInt(sessionStorage.getItem('isBackFromSubMenu') as string)
      : 0;
    if (isBackFromSubMenu === 1) {
      sessionStorage.removeItem('user_permission_active_tab');
    }
  }

  onChangeTab(tabName: string) {
    this.activeTab = tabName;
    sessionStorage.setItem('user_permission_active_tab', tabName);
  }
}
