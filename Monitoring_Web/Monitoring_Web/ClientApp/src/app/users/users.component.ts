import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from '../base.component';
import { Helper } from '../_common/_helper';
import { IPaginationModel } from '../_model/IPaginationModel';
import { IUserProfileModel } from '../_model/IUserProfileModel';
import { UserProfileService } from '../_service/user-profile.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
})
export class UsersComponent extends BaseComponent implements OnInit {
  isLoading: boolean = false;

  data: IUserProfileModel[] = [];

  isSelectAll: boolean = false;

  confirmDeletionMessage: string = '';

  modalRef?: BsModalRef;

  email!: string;

  pagination: IPaginationModel = {
    itemFrom: 0,
    itemTo: 0,
    listPageNumber: [],
    pageNumber: 1,
    pageSize: 20,
    pageSizeList: [10, 20, 50, 100],
    totalItem: 0,
    totalPage: 0,
  };

  userDelete!: IUserProfileModel;

  messageDelete: string = '';

  constructor(
    private _userService: UserProfileService,
    private toastService: ToastrService,
    private router: Router,
    public modalService: BsModalService
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.handleLoadData();
  }

  handleLoadData() {
    this.isLoading = true;
    const email = this.email;
    const pageNumber = this.pagination.pageNumber;
    const pageSize = this.pagination.pageSize;
    this._userService.getUsers({ email, pageNumber, pageSize }).subscribe(
      (respone) => {
        this.data = respone;
        if (this.data && this.data.length > 0) {
          const totalRow =
            this.data.length > 0 ? this.data[0].totalRows ?? 0 : 0;
          this.pagination.totalItem = totalRow;
          const paginationHelper = Helper.getPager(
            totalRow,
            this.pagination.pageNumber,
            this.pagination.pageSize
          );
          this.pagination.listPageNumber = paginationHelper.pages;
          this.pagination.totalPage = paginationHelper.totalPages;
          this.caculateItemFromAndTo();
        } else {
          this.data = [];
          this.pagination.listPageNumber = [];
          this.pagination.totalPage = 0;
          this.pagination.totalItem = 0;
          this.pagination.itemFrom = 0;
          this.pagination.itemTo = 0;
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

  onClickDetail(clickedItem: IUserProfileModel) {
    let role = this.roles.indexOf(this.globalEnumResult.ActionEnum.userManage);
    if (role === -1) {
      this.toastService.warning('Access denied');
      return;
    }
    this.router.navigate(['/user-detail', clickedItem.id]);
  }

  onClickAddUser() {
    this.router.navigate(['/user-detail']);
  }

  onChangeSelectAll() {
    const isSelectAll = this.isSelectAll;
    this.data = this.data.map((item) => {
      return { ...item, isSelect: isSelectAll };
    });
  }

  onClickDeleteUser(template: TemplateRef<any>, userDelete: IUserProfileModel) {
    this.userDelete = userDelete;
    const message = userDelete.isDelete === true ? 'Active' : 'In-Active';
    this.userDelete.isDelete = userDelete.isDelete == true ? false : true;
    this.messageDelete = message;
    this.confirmDeletionMessage = 'Do you want to ' + message + ' this user';
    this.modalRef = this.modalService.show(template);
  }

  handleDeleteUser(userDelete: IUserProfileModel) {
    const message = userDelete.isDelete === true ? 'Active' : 'In-Active';
    this.messageDelete = message;
    this.modalService.hide();
    userDelete.isDelete = !userDelete.isDelete;
    // const selectedUser = this.data.slice().filter((d) => d.isSelect === true);
    this.isLoading = true;
    this._userService.delete(userDelete).subscribe(
      (respone) => {
        const rowsEffect = respone;
        if (rowsEffect > 0) {
          let successMessage = this.messageDelete + ' user successfully';

          this.toastService.success(successMessage);
          this.isSelectAll = false;
        } else {
          this.toastService.error('An unexpected error has occurred');
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

  caculateItemFromAndTo() {
    this.pagination.itemFrom =
      (this.pagination.pageNumber - 1) * this.pagination.pageSize + 1;
    this.pagination.itemTo =
      this.pagination.pageNumber * this.pagination.pageSize >
      this.pagination.totalItem
        ? this.pagination.totalItem
        : this.pagination.pageNumber * this.pagination.pageSize;
  }

  onChangePageNumber(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;
    this.handleLoadData();
  }

  onChangePageSize(pageSize: number) {
    this.pagination.pageNumber = 1;
    this.handleLoadData();
  }

  onCloseDelete() {
    this.modalService.hide();
    const userDelete = this.userDelete;
    this.data = this.data.map((item) => {
      return {
        ...item,
        isDelete:
          userDelete.id === item.id ? !userDelete.isDelete : item.isDelete,
      };
    });
  }
}
