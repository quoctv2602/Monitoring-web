import { Injectable } from '@angular/core';
import { globalsettings } from 'src/assets/globalsetting';
import { HttpClient } from '@angular/common/http';
import {
  INotificationModel,
  NotificationAddRequest,
  NotificationEditRequest,
  NotificationListRequest,
  NotificationModel,
  ToggleNotificationRequest,
} from '../_model/INotificationModel';
import { ApiResponse } from '../_model/node-settings-model';
import { map } from 'rxjs';
import { IPagedResultModel } from '../_model/IPagedResultModel';
@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  baseUrl = globalsettings.apiUrl;
  constructor(private http: HttpClient) {}
  createNotification(notificationAddRequest: NotificationAddRequest) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'Notification/createNotification',
        notificationAddRequest
      )
      .pipe(
        map((res) => {
          return res;
        })
      );
  }
  getListNotification(notificationListRequest: NotificationListRequest) {
    return this.http
      .post<IPagedResultModel<NotificationModel>>(
        this.baseUrl + 'Notification/getListNotification',
        notificationListRequest
      )
      .pipe(
        map((res: IPagedResultModel<NotificationModel>) => {
          return res;
        })
      );
  }
  getNotificationEdit(id: number) {
    return this.http
      .post<NotificationEditRequest>(
        this.baseUrl + 'Notification/getNotificationEdit',
        id
      )
      .pipe(
        map((res: NotificationEditRequest) => {
          return res;
        })
      );
  }
  deleteNotification(notificationRequest: NotificationModel[]) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'Notification/deleteNotification',
        notificationRequest
      )
      .pipe(
        map((res: ApiResponse) => {
          return res;
        })
      );
  }
  toggleNotification(toggleNotificationRequest: ToggleNotificationRequest) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'Notification/toggleNotification',
        toggleNotificationRequest
      )
      .pipe(
        map((res: ApiResponse) => {
          return res;
        })
      );
  }
  updateNotification(notificationEditRequest: NotificationEditRequest) {
    return this.http
      .post<ApiResponse>(
        this.baseUrl + 'Notification/updateNotification',
        notificationEditRequest
      )
      .pipe(
        map((res) => {
          return res;
        })
      );
  }
}
