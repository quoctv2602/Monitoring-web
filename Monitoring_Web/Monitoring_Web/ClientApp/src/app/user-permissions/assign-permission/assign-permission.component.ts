import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IActionModel } from 'src/app/_model/IActionModel';
import { IPermissionModel } from 'src/app/_model/IPermissionModel';
import { PermissionService } from 'src/app/_service/permission.service';

@Component({
  selector: 'app-assign-permission',
  templateUrl: './assign-permission.component.html',
  styleUrls: ['./assign-permission.component.css'],
})
export class AssignPermissionComponent implements OnInit {
  permission: IPermissionModel[] = [];

  isAssignAll: boolean = false;

  groupId!: number;

  isLoading: boolean = false;

  showSaveButton: boolean = true;

  constructor(
    private _permissionService: PermissionService,
    private route: ActivatedRoute,
    private toastService: ToastrService,
    private router: Router
  ) {}

  ngOnInit() {
    this.handleLoadData();
  }

  handleLoadData() {
    let id = (this.route.snapshot.paramMap.get('id') ?? 0) as number;
    if (id > 0) {
      this.isLoading = true;
      this.groupId = id;
      this._permissionService.permissionByGroup(id).subscribe(
        (respone) => {
          this.permission = respone;
          this.showSaveButton =
            this.permission.find(
              (p) => p.actions.find((a) => a.isSelected === true) !== undefined
            ) !== undefined;
        },
        (err) => {
          (this.isLoading = false),
            this.toastService.error('An unexpected error has occured');
        },
        () => {
          this.isLoading = false;
        }
      );
    } else {
      this.toastService.error('Invalid url');
    }
  }

  onChangeSelectAllByMenu(menuItem: IPermissionModel) {
    const isSelectedByMenu = menuItem.isSelected;
    this.permission = this.permission.map((item) => {
      return {
        ...item,
        actions: item.actions.map((a) => {
          return {
            ...a,
            isSelected:
              item.pageId == menuItem.pageId ? isSelectedByMenu : a.isSelected,
          };
        }),
      };
    });
    if (!this.permission.find((p) => p.isSelected === false))
      this.isAssignAll = true;
    else this.isAssignAll = false;

    this.showSaveButton =
      this.permission.find(
        (p) => p.actions.find((a) => a.isSelected === true) !== undefined
      ) !== undefined;
  }

  onChangeAssignAll() {
    this.permission = this.permission.map((item) => {
      return {
        ...item,
        isSelected: this.isAssignAll,
        actions: item.actions.map((a) => {
          return {
            ...a,
            isSelected: this.isAssignAll,
          };
        }),
      };
    });

    this.showSaveButton =
      this.permission.find(
        (p) => p.actions.find((a) => a.isSelected === true) !== undefined
      ) !== undefined;
  }

  onChangeSelectByAction(menuItem: IPermissionModel, actionItem: IActionModel) {
    const isSelectAction = actionItem.isSelected;
    const isSelectMenu = menuItem.isSelected;
    if (isSelectMenu === true) {
      if (isSelectAction === false) {
        menuItem.isSelected = false;
        this.isAssignAll = false;
      }
    } else {
      if (isSelectAction === true) {
        if (!menuItem.actions.find((a) => a.isSelected === false))
          menuItem.isSelected = true;
        if (!this.permission.find((p) => p.isSelected === false))
          this.isAssignAll = true;
      }
    }

    this.showSaveButton =
      this.permission.find(
        (p) => p.actions.find((a) => a.isSelected === true) !== undefined
      ) !== undefined;
  }

  handleSavePermission() {
    const saveModel = {
      groupId: this.groupId,
      permissions: this.permission,
    };
    this.isLoading = true;
    this._permissionService.savePermission(saveModel).subscribe(
      (respone) => {
        const rowsAffect = respone;
        if (rowsAffect > 0) {
          this.toastService.success('Assign permission successfully');
        } else {
          this.toastService.error('An expected error has occured');
        }
      },
      (err) => {
        this.isLoading = false;
        this.toastService.error('An expected error has occured');
      },
      () => {
        this.isLoading = false;
      }
    );
  }

  onClickCancel() {
    sessionStorage.setItem('isBackFromSubMenu', '1');
    this.router.navigate(['/user-permission']);
  }
}
