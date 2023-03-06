import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Component, ElementRef, OnDestroy, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { DatePipe, formatDate } from '@angular/common';
import { MsalBroadcastService } from '@azure/msal-angular';
import { filter, Subject, takeUntil } from 'rxjs';
import { InteractionStatus } from '@azure/msal-browser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: [],
})
export class AppComponent implements OnInit, OnDestroy {
  readonly VAPID_PUBLIC_KEY =
    'BIvUlifLNlA0PEhsH4AdtdDCkTL8QZGg1umaVX1b2krLS9ZC9TRdAlDGvYFU9sU_Nly7KBjJXbGSXgZO-ncpss4';

  title = 'monitoring-tool';
  isIframe = false;
  private readonly _destroying$ = new Subject<void>();
  constructor(
    private http: HttpClient,
    private elementRef: ElementRef,
    public _router: Router,
    private broadcastService: MsalBroadcastService
  ) {
    // if ('serviceWorker' in navigator) {
    //   // Register a service worker hosted at the root of the
    //   // site using the default scope.
    //   const pathSw = 'assets/js/sw.js';
    //   navigator.serviceWorker.register(pathSw).then(
    //     (registration) => {
    //       console.log('Service worker registration succeeded:', registration);
    //       var worker = new Worker(pathSw);
    //       // Watch for messages from the worker
    //       worker.onmessage = function (e) {
    //         // The message from the client:
    //         e.data;
    //       };
    //       worker.postMessage('start');
    //     },
    //     /*catch*/ (error) => {
    //       console.error(`Service worker registration failed: ${error}`);
    //     }
    //   );
    // } else {
    //   console.error('Service workers are not supported.');
    // }
  }

  msgs: Message[] = [];
  msgNew: any;
  Message: any = [];
  profile: any;
  public static generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(
      /[xy]/g,
      function (c) {
        var r = (Math.random() * 16) | 0,
          v = c == 'x' ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      }
    );
  }
  public static pushMsg(message: string, type: string, isRead: number) {
    if ((localStorage.getItem('message') as string) == null) {
      localStorage.setItem('message', '[]');
    }
    const msgLocal = JSON.parse(localStorage.getItem('message') as string);
    msgLocal.push({
      message: message,
      timestamp: formatDate(new Date(), 'M/dd/yyyy hh:mm:ss a', 'en'),
      id: this.generateGuid(),
      from: 'System',
      to: 'current',
      type: type,
      isRead: isRead,
    });
    localStorage.setItem('message', JSON.stringify(msgLocal));
  }
  public static notifyWindows(message: string) {
    if (!('Notification' in window)) {
      // Check if the browser supports notifications
      alert('This browser does not support desktop notification');
    } else if (Notification.permission === 'granted') {
      // Check whether notification permissions have already been granted;
      // if so, create a notification
      const notification = new Notification(message);
      // …
    } else if (Notification.permission !== 'denied') {
      // We need to ask the user for permission
      Notification.requestPermission().then((permission) => {
        // If the user accepts, let's create a notification
        if (permission === 'granted') {
          const notification = new Notification(message);
          // …
        }
      });
    }
  }
  eventClear() {
    this.msgs = [];
    localStorage.setItem('message', JSON.stringify(this.msgs));
  }
  ngOnInit() {
    this.isIframe = window !== window.parent && !window.opener;
    this.broadcastService.inProgress$
      .pipe(
        filter(
          (status: InteractionStatus) => status === InteractionStatus.None
        ),
        takeUntil(this._destroying$)
      )
      .subscribe(() => {});
  }

  ngOnDestroy(): void {
    this._destroying$.next(undefined);
    this._destroying$.complete();
  }
}

export interface Message {
  type: string;
  from: string;
  to: string;
  timestamp: string;
  message: string;
  id: string;
  isRead: number;
}
