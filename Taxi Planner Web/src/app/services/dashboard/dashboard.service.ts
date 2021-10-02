import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(private http: HttpClient) {}

  getStats() {
    return this.http.get(`${environment.apiUrl}/statistics?choice=1`);
  }

  getWeekly() {
    return this.http.get(`${environment.apiUrl}/Statistics?choice=2`);
  }

  getRegion() {
    return this.http.get(`${environment.apiUrl}/Statistics?choice=3`);
  }
}
