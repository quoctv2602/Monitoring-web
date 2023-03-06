import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IGroupModel } from '../_model/IGroupModel';
import { GroupService } from '../_service/group.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { IPaginationModel } from '../_model/IPaginationModel';
import { Helper } from '../_common/_helper';
import { createPopper } from '@popperjs/core';
import { BaseComponent } from '../base.component';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
})
export class GroupsComponent extends BaseComponent implements OnInit {
  data: IGroupModel[] = [];

  isLoading!: boolean;

  isSelectAll: boolean = false;

  confirmDeletionMessage: string = '';

  modalRef?: BsModalRef;

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

  groupName!: string;

  constructor(
    private groupService: GroupService,
    private toast: ToastrService,
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
    const groupName = this.groupName?.trim();
    const pageSize = this.pagination.pageSize;
    const pageNumber = this.pagination.pageNumber;
    this.groupService.getGroups({ groupName, pageSize, pageNumber }).subscribe(
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
        this.toast.error('An unexpected error has occurred');
      },
      () => {
        this.isLoading = false;
      }
    );
  }

  handleDeteleGroup() {
    this.modalService.hide();
    const selectedGroup = this.data.slice().filter((d) => d.isSelect === true);
    this.isLoading = true;
    this.groupService.delete(selectedGroup).subscribe(
      (respone) => {
        const rowsEffect = respone;
        if (rowsEffect > 0) {
          let successMessage = '';
          if (selectedGroup.length == 1)
            successMessage = 'Delete group successfully';
          else successMessage = 'Delete groups successfully';
          this.toast.success(successMessage);
          this.data = this.data.filter(
            (d) =>
              selectedGroup.find((g) => g.groupId === d.groupId) === undefined
          );
          this.isSelectAll = false;
        } else {
          this.toast.error('An unexpected error has occurred');
        }
      },
      (err) => {
        this.isLoading = false;
        this.toast.error('An unexpected error has occurred');
      },
      () => {
        this.isLoading = false;
      }
    );
  }

  onClickDetail(item: IGroupModel) {
    let role = this.roles.indexOf(
      this.globalEnumResult.ActionEnum.userPermissionManageGroup
    );
    if (role === -1) {
      this.toast.warning('Access denied');
      return;
    }
    this.router.navigate(['/group-detail', item.groupId]);
  }

  onClickAddGroup() {
    this.router.navigate(['/group-detail']);
  }

  onChangeSelectAll() {
    const isSelectAll = this.isSelectAll;
    this.data = this.data.map((item) => {
      return { ...item, isSelect: isSelectAll };
    });
  }

  onClickDeleteGroup(template: TemplateRef<any>) {
    const selectedGroup = this.data.slice().filter((d) => d.isSelect === true);
    const groupDefault = selectedGroup.find((g) => g.isDefault === true);
    if (groupDefault !== undefined) {
      this.toast.error(
        groupDefault.groupName +
          ' is default group. Can not delete default group'
      );
      return;
    }
    if (selectedGroup.length === 0) {
      this.toast.warning('None of groups is selected');
      return;
    }
    if (selectedGroup.length == 1) {
      this.confirmDeletionMessage = 'Do you want to delete this group?';
    } else this.confirmDeletionMessage = 'Do you want to delete these groups?';
    this.modalRef = this.modalService.show(template);
  }

  onChangeDefaultGroup(changeItem: IGroupModel) {
    const message = changeItem.isDefault === false ? 'Un' : 'Set';
    this.isLoading = true;
    this.groupService.changeDefault(changeItem).subscribe(
      (respone) => {
        const rowsEffect = respone;
        if (rowsEffect > 0) {
          this.data = this.data.map((item) => {
            return {
              ...item,
              isDefault:
                item.groupId === changeItem.groupId
                  ? changeItem.isDefault
                  : false,
            };
          });
          this.toast.success(message + ' default successfully');
        } else this.toast.error('An unexpected error has occurred');
      },
      (err) => {
        this.toast.error('An unexpected error has occurred');
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

  onClickAssignPermission() {
    const selectedGroup = this.data.slice().filter((d) => d.isSelect === true);
    if (selectedGroup.length === 0) {
      this.toast.warning('None of groups is selected');
      return;
    } else {
      if (selectedGroup.length > 1) {
        this.toast.warning('Just assign permission one group per once');
        return;
      }
    }
    const groupId = selectedGroup[0].groupId;
    this.router.navigate(['/assign-permission', groupId]);
  }

  handleShowMembers(idControl: string, idTooltip: string) {
    const popcorn = document.querySelector('#' + idControl);
    const tooltip = document.getElementById(idTooltip);
    if (popcorn != null && tooltip != null) {
      createPopper(popcorn, tooltip, {
        placement: 'top',
        modifiers: [
          {
            name: 'offset',
            options: {
              offset: [0, 8],
            },
          },
        ],
      });
    }
  }

  getEmailListString(item: IGroupModel) {
    return item.members.map((m) => m.email).join('; ');
  }
}
