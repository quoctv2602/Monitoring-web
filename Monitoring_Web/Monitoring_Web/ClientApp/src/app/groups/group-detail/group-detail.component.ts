import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  UntypedFormControl,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { SaveGroupResult, UserType } from 'src/app/_common/_enum';
import { IActionModel } from 'src/app/_model/IActionModel';
import { IGroupModel } from 'src/app/_model/IGroupModel';
import { IPermissionModel } from 'src/app/_model/IPermissionModel';
import { IUserProfileModel } from 'src/app/_model/IUserProfileModel';
import { GroupService } from 'src/app/_service/group.service';
import { PermissionService } from 'src/app/_service/permission.service';
import { UserProfileService } from 'src/app/_service/user-profile.service';

@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.css'],
})
export class GroupDetailComponent extends BaseComponent implements OnInit {
  title: string = 'Create Group';

  groupFormSave: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(128)]],
    description: '',
    members: [],
  });

  groupDetail: IGroupModel = { members: [] };

  listUserProfile: IUserProfileModel[] = [];

  submitted: boolean = false;

  isLoading: boolean = false;

  permission: IPermissionModel[] = [];

  isAssignAll: boolean = false;

  groupId!: number;

  showSaveButton: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private _groupService: GroupService,
    private fb: FormBuilder,
    private _userProfileService: UserProfileService,
    private toastService: ToastrService,
    private router: Router,
    private _permissionService: PermissionService
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this._userProfileService
      .getUsers({ userType: UserType.Normal, isDelete: false })
      .subscribe(
        (respone) => {
          this.listUserProfile = respone;
        },
        (err) => {},
        () => {
          let id = (this.route.snapshot.paramMap.get('id') ?? 0) as number;
          if (id !== 0) {
            this.handleLoadData();
            //Get detail group
            let role = this.roles.indexOf(
              this.globalEnumResult.ActionEnum.userPermissionManageGroup
            );
            if (role === -1) {
              this.router.navigateByUrl('/access-denied');
            }
            this.handleGetDetailGroup(id);
          } else {
            let role = this.roles.indexOf(
              this.globalEnumResult.ActionEnum.userPermissionManageGroup
            );
            if (role === -1) {
              this.router.navigateByUrl('/access-denied');
            }
            this.inItForm();
          }
        }
      );
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

  get g(): { [key: string]: AbstractControl } {
    return this.groupFormSave.controls;
  }

  inItForm() {
    this.groupFormSave.controls['name'].setValue(this.groupDetail?.groupName);
    this.groupFormSave.controls['description'].setValue(
      this.groupDetail?.groupDescription
    );
    this.groupFormSave.controls['members'].setValue(
      this.groupDetail?.members.slice().map((item) => {
        return item.id;
      })
    );
  }

  handleGetDetailGroup(id: number) {
    this.isLoading = true;
    this.title = 'Edit Group';
    this._groupService.getById(id).subscribe(
      (respone) => {
        this.groupDetail = respone;
      },
      (err) => {
        this.isLoading = false;
        this.toastService.error('An unexpected error has occurred');
      },
      () => {
        this.isLoading = false;
        this.inItForm();
      }
    );
  }

  handleSaveGroup() {
    this.submitted = true;
    if (this.groupFormSave.invalid) {
      return;
    }
    this.isLoading = true;
    const formValue = this.groupFormSave.controls;
    this.groupDetail.groupName = formValue['name'].value;
    this.groupDetail.groupDescription = formValue['description'].value;
    if (formValue['members'].value !== null)
      this.groupDetail.members = this.listUserProfile.filter((a) =>
        (formValue['members'].value as number[]).includes(a.id)
      );
    this.groupDetail.totalMembers = this.groupDetail.members.length;
    this._groupService.saveGroup(this.groupDetail).subscribe(
      (respone) => {
        const rowsAffect = respone;
        if (rowsAffect > 0) {
          this.toastService.success('Save group successfully');
          this.router.navigate(['/user-permission']);
        } else {
          if (rowsAffect === SaveGroupResult.Exist) {
            this.toastService.error(
              this.groupDetail.groupName + ' has already existed'
            );
          } else this.toastService.error('An unexpected error has occurred');
        }
      },
      (err) => {
        this.isLoading = false;
        this.toastService.error('An unexpected error has occurred');
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
}
