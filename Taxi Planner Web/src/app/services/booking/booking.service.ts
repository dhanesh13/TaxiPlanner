import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root',
})
export class BookingService {
  constructor(private http: HttpClient) {}

  getMyBookings(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/bookingSlots?choice=2`);
  }

  addBooking(bookings) {
    const body = bookings;
    return this.http.post(
      `${environment.apiUrl}/bookingSlots`,
      JSON.stringify(body)
    );
  }

  getBookingTimes(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/ScheduleTimes`);
  }

  getAllBookings() {
    return this.http.get(`${environment.apiUrl}/bookingSlots?choice=1`);
  }

  changeStatus(body) {
    return this.http.patch(`${environment.apiUrl}/bookingSlots`, body);
  }

  setStatus(statusInfo, statusBoolean: boolean): Observable<any[]> {
    let id: number;
    id = statusInfo.booking_id;
    let myParams = new HttpParams().set('thedate', statusInfo.date_time);
    myParams.set('id', id.toString());

    let myBody = {
      status: 'REJECTED',
      booking_id: id,
      booking_timestamp: statusInfo.date_time,
    };

    if (statusBoolean) {
      myBody['status'] = 'APPROVED';
    }

    return this.http.patch<any>(`${environment.apiUrl}/bookingSlots`, myBody);
  }
}
