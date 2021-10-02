import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class DelegateroleService {
  constructor(private http: HttpClient) {}
  delegaterole(
    delegator_id: number,
    start: string,
    end: string,
    emp_id: number
  ): Observable<any> {
    const body = {
      delegator: delegator_id,
      date_from: start,
      date_to: end,
      user_id: emp_id,
      app_id:2
      
    };
    return this.http.post(
      `${environment.apiUrl}/delegation`,
      JSON.stringify(body)
    );
  }
}
