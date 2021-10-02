import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  constructor(private http: HttpClient) {}
  getStats(dateFrom, dateTo) {
    const url = `${environment.apiUrl}/report?dateFrom=${dateFrom}&dateTo=${dateTo}`;
    return this.http.get(url);
  }
}
