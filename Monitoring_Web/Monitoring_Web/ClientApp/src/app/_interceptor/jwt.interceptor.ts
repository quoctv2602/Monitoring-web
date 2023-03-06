import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // add authorization header with jwt token if available

    const token = localStorage.getItem('storage_t')
      ? localStorage.getItem('storage_t')
      : null;
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    } //else this.router.navigate(['/login']);
    return next.handle(request).pipe(
      catchError((err) => {
        if (err.status === 401) {
          localStorage.clear();
          this.router.navigate(['/login']);
        }
        const error = err.error ? err.error.message : err.statusText;
        return throwError(error);
      })
    );
  }
}
