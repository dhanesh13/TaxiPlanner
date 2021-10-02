import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
} from '@angular/common/http';
import { StorageService } from './storage/storage.service';
import { Injectable } from '@angular/core';

@Injectable()
export class MyHttpInterceptorService implements HttpInterceptor {
  constructor(private storageService: StorageService) {}
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = this.storageService.getCookie('token');
    const new_req = req.clone({
      headers: req.headers
        .append('Authorization', `Bearer ${token}`)
        .append('Content-Type', 'application/json'),
    });
    return next.handle(new_req);
  }
}
