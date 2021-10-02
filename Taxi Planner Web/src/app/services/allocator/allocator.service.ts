import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AllocatorService {

  constructor(private http:HttpClient) { 
  }

  getAllocations(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/allocator`);
  }
  postAllocations(data): Observable<any> {
    return this.http.post(`${environment.apiUrl}/allocator`,data);
  }
}
