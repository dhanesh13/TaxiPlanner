import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StorageService } from '../storage/storage.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private http: HttpClient,
    private storageService: StorageService,
    private router: Router
  ) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth`, { email, password });
  }

  logout(): void {
    this.storageService.delete_cookie('taxi_role') ;

    window.location.href = `${environment.nucleusUrl}/dashboard`;

    window.close();
    // this.router.navigate(['/']);
  }   

  isAuthenticated(): boolean {
    const id = this.storageService.getCookie('id');
    const role = this.storageService.getCookie('role');
    const token = this.storageService.getCookie('token');
    const name = this.storageService.getCookie('name');

    return (
      id.length > 0 && role.length > 0 && token.length > 0 && name.length > 0
    );
  }
}
