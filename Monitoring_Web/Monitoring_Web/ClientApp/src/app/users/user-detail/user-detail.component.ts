import { Component, Input, OnDestroy, OnInit } from '@angular/core';
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
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { SaveUserResult, UserType } from 'src/app/_common/_enum';
import { Helper } from 'src/app/_common/_helper';
import { IGroupModel } from 'src/app/_model/IGroupModel';
import { IUserProfileModel } from 'src/app/_model/IUserProfileModel';
import { GroupService } from 'src/app/_service/group.service';
import { UserProfileService } from 'src/app/_service/user-profile.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css'],
})
export class UserDetailComponent
  extends BaseComponent
  implements OnInit, OnDestroy
{
  userDetail: IUserProfileModel = { id: 0 };

  title: string = 'Add User';

  userFormSave: FormGroup = this.fb.group({
    email: new FormControl(),
    isAdmin: new FormControl(),
    groupId: new FormControl(),
  });

  submitted: boolean = false;

  listGroups: IGroupModel[] = [];

  isLoading: boolean = false;

  groupIdDefault: number | null | undefined = null;

  showGroup: boolean = true;

  constructor(
    public fb: FormBuilder,
    private route: ActivatedRoute,
    private _groupService: GroupService,
    private _userService: UserProfileService,
    private _toastService: ToastrService,
    private router: Router
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    let id = (this.route.snapshot.paramMap.get('id') ?? 0) as number;
    this._groupService.getGroups({}).subscribe(
      (respone) => {
        this.listGroups = respone;
        if (id !== 0) {
        } else
          this.groupIdDefault = this.listGroups.find(
            (g) => g.isDefault == true
          )?.groupId;
      },
      (err) => {},
      () => {
        if (id !== 0) {
          let role = this.roles.indexOf(
            this.globalEnumResult.ActionEnum.userManage
          );
          if (role === -1) {
            this.router.navigateByUrl('/access-denied');
          }
          //Get detail user
          this.handleGetDetailUser(id);
        } else {
          let role = this.roles.indexOf(
            this.globalEnumResult.ActionEnum.userManage
          );
          if (role === -1) {
            this.router.navigateByUrl('/access-denied');
          }
          this.inItForm();
        }
      }
    );
  }

  ngOnDestroy(): void {}

  get g(): { [key: string]: AbstractControl } {
    return this.userFormSave.controls;
  }

  handleGetDetailUser(id: number) {
    this.isLoading = true;
    this.title = 'Edit User';
    this._userService.getById(id).subscribe(
      (respone) => {
        this.userDetail = respone;
        if (this.userDetail.userType === UserType.Admin) this.showGroup = false;
      },
      (err) => {
        this.isLoading = false;
        this._toastService.error('An expected error has occured');
      },
      () => {
        this.isLoading = false;
        this.inItForm();
      }
    );
  }

  inItForm() {
    this.userFormSave = this.fb.group({
      email: [
        this.userDetail?.email,
        [
          Validators.required,
          Validators.email,
          Validators.pattern('^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[a-z]{2,4}$'),
        ],
      ],
      isAdmin: this.userDetail?.userType === UserType.Admin ? true : false,
      groupId: this.userDetail?.groupId
        ? this.userDetail.groupId
        : this.groupIdDefault,
    });
  }

  emailValidator(controlName: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const email = (control.value as string)?.trim();

      if (email !== null && email !== undefined && email.length > 0) {
        if (!Helper.emailRegex.test(email)) {
          return { invalidEmail: true };
        }
        return null;
      } else return null;
    };
  }

  handleSaveUser() {
    this.submitted = true;
    if (this.userFormSave.invalid) {
      return;
    }
    this.isLoading = true;
    const formValue = this.userFormSave.controls;
    this.userDetail.email = formValue['email'].value;
    this.userDetail.userType = (formValue['isAdmin'].value as boolean)
      ? UserType.Admin
      : UserType.Normal;
    this.userDetail.groupId = formValue['groupId'].value;
    this._userService.save(this.userDetail).subscribe(
      (respone) => {
        const rowsAffect = respone;
        if (rowsAffect > 0) {
          this._toastService.success('Save user successfully');
          this.router.navigate(['/user-permission']);
        } else {
          if (rowsAffect === SaveUserResult.Exist) {
            this._toastService.error(
              this.userDetail.email + ' has already existed'
            );
          } else this._toastService.error('An unexpected error has occurred');
        }
      },
      (err) => {
        this.isLoading = false;
        this._toastService.error('An unexpected error has occurred');
      },
      () => {
        this.isLoading = false;
      }
    );
  }

  onChangeIsAdmin() {
    const isAdmin: boolean = this.userFormSave.controls['isAdmin'].value;
    if (isAdmin) {
      this.showGroup = false;
      this.userFormSave.controls['groupId'].setValue(null);
    } else {
      this.showGroup = true;
      this.userFormSave.controls['groupId'].setValue(this.groupIdDefault);
    }
  }

  onClickCancel() {
    sessionStorage.setItem('isBackFromSubMenu', '1');
    this.router.navigate(['/user-permission']);
  }
}
