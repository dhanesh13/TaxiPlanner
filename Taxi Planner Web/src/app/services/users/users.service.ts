import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  constructor(private http: HttpClient) {}

  getUsers(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/employees`);
  }

  getName(nameInfo): Observable<any[]> {
    return this.http.put<any>(
      `${environment.apiUrl}/employees/${nameInfo.emp_name}`,
      nameInfo
    );
  }
}
