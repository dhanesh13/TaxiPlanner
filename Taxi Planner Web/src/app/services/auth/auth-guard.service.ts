import { Injectable } from '@angular/core';
import {
  CanActivate,
  CanActivateChild,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { AuthService } from './auth.service';
import { StorageService } from '../storage/storage.service';

@Injectable()
export class AuthGuard implements CanActivate, CanActivateChild {
  constructor(
    private authService: AuthService,
    private router: Router,
    private storageService: StorageService
  ) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const next_path = next.url[0].path;
    if (this.authService.isAuthenticated()) {
      const role = this.storageService.getCookie('taxi_role');

      if (next_path == 'report') {
        if (role == 'superadmin' || role == 'hr') {
          return true;
        } else {
          window.location.href = '/';
          return false;
        }
      } else if (next_path == 'dashboard') {
        if (role == 'superadmin' || role == 'hr') {
          return true;
        } else {
          window.location.href = '/';
          return false;
        }
      } else if (next_path == 'delegate') {
        if (role == 'superadmin' || role == 'approver') {
          return true;
        } else {
          window.location.href = '/';
          return false;
        }
      } else if (next_path == 'manageusers') {
        if (role == 'superadmin' || role == 'hr') {
          return true;
        } else {
          window.location.href = '/';
          return false;
        }
      } else if (next_path == 'addotherbookings') {
        if (role == 'superadmin' || role == 'approver') {
          return true;
        } else {
          window.location.href = '/';
          return false;
        }
      } else if (next_path == 'managebookings') {
        if (
          role == 'superadmin' ||
          role == 'approver' ||
          role == 'hr' ||
          role == 'delegated_approver'
        ) {
          return true;
        } else {
          window.location.href = '/';
          return false;
        }
      }
      return true;
    } else {
      localStorage.setItem('path', next_path);
      this.router.navigate(['/']);
      return false;
    }
  }

  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (this.authService.isAuthenticated()) {
      const role = this.storageService.getCookie('role');
      console.log(next.url);
      return true;
    }
    this.router.navigate(['/']);
    return false;
    // window.location.href = '/';
  }
}
