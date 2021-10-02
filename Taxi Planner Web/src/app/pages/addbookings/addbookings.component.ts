import { Component, ViewChild } from '@angular/core';
import { ViewEncapsulation } from '@angular/core';
import { PreventableEvent } from '@progress/kendo-angular-dateinputs';
import { BookingService } from 'src/app/services/booking/booking.service';
import { Router } from '@angular/router';
import { Time, time } from 'highcharts';
import Swal from 'sweetalert2';
import { min } from 'rxjs/operators';
import { FormGroup, FormControl } from '@angular/forms';
import { addDays } from '@progress/kendo-date-math';
import { MatTableDataSource } from '@angular/material/table';
//import { MatPaginator } from '@angular/material/paginator';
import { ListItem } from '@progress/kendo-angular-dateinputs/dist/es2015/timepicker/models/list-item.interface';
import { JsonPipe, DatePipe } from '@angular/common';
import { StorageService } from 'src/app/services/storage/storage.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-booking',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './addbookings.component.html',
  styleUrls: ['./addbookings.component.css'],
})
export class AddBookingsComponent {
  public preventOpen: boolean;
  public preventClose: boolean;
  public dialog_opened = false;

  public disabledDates: Date[] = [];

  selectedValue: string;
  selectedCar: string;

  format = 'EEEE, MMMM d, y';
  today = new Date();

  form = new FormGroup({
    today: new FormControl(),
  });

  firstDay = new Date();
  public range = { start: this.firstDay, end: new Date() };

  min = new Date();
  current_date: Date = new Date();

  public currentYear: number = this.today.getFullYear();
  public currentMonth: number = this.today.getMonth();

  public fromMinDate: Object = new Date();
  public fromMaxDate: Object = new Date(
    this.currentYear,
    this.currentMonth + 3
  );

  public toMinDate: Object = this.current_date;
  public toMaxDate: Object = new Date(this.currentYear, this.currentMonth + 3);

  public myhour: number;
  public mymin: number;
  public deadline;

  public counter: number;
  public date_test;
  public tomr;

  MyDates: Date[];

  today_time = new Date().getTime();

  max_time = new Date();
  // this.today.getFullYear(),
  // this.today.getMonth(),
  // this.today.getDate(),

  public events: string[] = [];
  public listItems: Array<String> = [];

  error = '';
  bookings = [];
  displayedColumns: string[] = ['Date', 'Time', 'Comment', 'Remove'];
  current_booking_date: Date = new Date();
  current_booking_comment = '';
  current_booking_time = 'hr/min';

  // pageSizeOptions: number[] = [6, 12];
  // @ViewChild(MatPaginator, { static: false })
  // set paginator(value: MatPaginator) {
  //   this.dataSource.paginator = value;
  // }
  // @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  dataSource: MatTableDataSource<any>;

  constructor(
    private bookingService: BookingService,
    private router: Router,
    private storageService: StorageService
  ) {}

  _loading = true;
  _loadingMsg = '';
  ngOnInit(): void {
    const requests = forkJoin(
      this.bookingService.getBookingTimes(),
      this.bookingService.getMyBookings()
    );
    requests.subscribe(
      (data) => {
        this.disabledDates = data[1].map((d) => new Date(d.date_time));

        // generate times
        this.listItems = data[0]
          .filter((a) => a.schedule_type == 0)
          .map((a) => a.time.slice(11, 16));

        const deadline = data[0].find((a: any) => a.schedule_type == 1)['time'];
        this.myhour = +deadline.slice(11, 13);
        this.mymin = +deadline.slice(14, 16);

        this.max_time.setHours(this.myhour);
        this.max_time.setMinutes(this.mymin);

        // booking deadlin

        this._loading = false;
      },
      (err) => console.log(err)
    );
  }

  showTable() {
    return this.bookings.length > 0;
  }

  removeAll() {
    this.bookings = [];
    this.error = '';
    this.current_booking_date = new Date();
    this.range = { start: new Date(), end: new Date() };
    this.range.start.setHours(this.range.start.getHours() + 4);
    this.range.end.setHours(this.range.end.getHours() + 4);
  }

  removeBooking(index) {
    this.bookings = this.bookings.filter((b) => b.index != index);
    this.dataSource = new MatTableDataSource([]);
    this.dataSource = new MatTableDataSource(this.bookings);
    this.error = '';
    // this.dataSource.paginator = this.paginator;
  }

  public close(status) {
    if (status == 'cancel') {
      this.current_booking_comment = '';
    }
    this.dialog_opened = false;
  }

  public addComment() {
    this.dialog_opened = true;
  }

  public handlePreventableEvent(
    event: PreventableEvent,
    eventName: string,
    preventDefault: boolean
  ): void {
    if (preventDefault) {
      event.preventDefault();
    }

    this.log(eventName);
  }

  checkIfDateExists(d: Date) {
    return (
      this.bookings.filter(
        (x) => new Date(x.date_time).toDateString() == d.toDateString()
      ).length > 0
    );
  }

  public log(event: string, value?: Date): void {
    if (value) {
      this.current_booking_date = value;

      this.hasDateErrors(value);
    }
  }

  hasDateErrors(v) {
    v = new Date(v);

    if (this.checkIfDateExists(v)) {
      this.error = "You can't have 2 bookings in one day.";
      return true;
    } else if (v.getDay() == 0) {
      this.error = 'No bookings can be made on Sundays';

      return true;
    } else if (this.current_booking_time == 'hr/min') {
      this.error = 'Please select a time.';

      return true;
    } else if (v.toDateString() === this.today.toDateString()) {
      // console.log(this.max_time.getHours());

      if (this.today_time > this.max_time.getTime()) {
        this.error =
          'Booking for today is already closed (15:30). Please contact your reporting line.';

        return true;
      }
    } else {
      this.error = '';
      return false;
    }
  }

  /*hasTimeErrors(t)
  {
    if(t=="hr/min")
    {
      this.error="Please select a time."
      return true;
    }
    else
    {
      this.error=""
      return false;
    }
  }*/

  calculateDateDifferenceInDays(d1, d2) {
    const diffTime = Math.abs(d2 - d1);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }

  ChangeDateFormat(d: Date): String {
    const d2: Date = new Date(d);
    const dd = String(d2.getDate()).padStart(2, '0');
    const mm = String(d2.getMonth() + 1).padStart(2, '0'); // January is 0!
    const yyyy = d2.getFullYear();
    return yyyy + '-' + mm + '-' + dd;
  }
  ChangeViewDateFormat(d: Date): String {
    const d2: Date = new Date(d);
    const dd = String(d2.getDate()).padStart(2, '0');
    const mm = String(d2.getMonth() + 1).padStart(2, '0'); // January is 0!
    const yyyy = d2.getFullYear();
    return dd + '-' + mm + '-' + yyyy;
  }

  addBooking() {
    // console.log(getDaysArray(this.range.start, this.range.end));
    // alert(this.max_time);
    // alert(this.range.start);
    //   alert(this.range.end);
    // alert(this.range);
    let today = new Date();
    let r;

    if (this.range.start.getTime() != this.range.end.getTime()) {
      if (this.range.start.getDate() == today.getDate()) {
        var s = addDays(this.range.end, 1);
        r = getDaysArray(this.range.start, s);
        // alert("else if");
      } else {
        r = getDaysArray(this.range.start, this.range.end);
        // alert("else else");
      }
    } else {
      r = [this.range.end];
      // alert("else ");
    }

    console.log(r);

    // this.bookings = [];

    for (let index = 0; index < r.length; index++) {
      // tslint:disable-next-line: align

      if (!this.hasDateErrors(r[index])) {
        // console.log('working');
        const date_item = new Date(r[index]);
        const mydate =
          this.ChangeDateFormat(date_item) +
          'T' +
          this.current_booking_time +
          ':00';
        // date_item.toISOString().substr(0, 11) +
        // this.current_booking_time +
        // ':00';

        console.log(mydate);

        console.log(date_item);
        if (date_item.getDay() != 0) {
          const bookings_num = this.bookings.length;
          this.bookings = [
            ...this.bookings,
            {
              index: bookings_num,
              date_time: mydate,
              //time:mydate,
              // time: this.current_booking_time,
              reason: this.current_booking_comment,
              emp_id: this.storageService.getCookie('id'),
            },
          ];

          // this.current_booking_date = new Date();
          // this.current_booking_comment = '';
          // this.current_booking_time = 'hr/min';
          this.error = '';
          // this.dataSource = new MatTableDataSource(this.bookings);
          //  this.dataSource.paginator = this.paginator();
        }
        // else
        // {
        //   this.error="Bookings cannot be made on Sundays."
        // }
      }
    }
    // console.log(this.bookings)
    this.dataSource = new MatTableDataSource(this.bookings);

    function getDaysArray(start, end) {
      for (
        var arr = [], dt = new Date(start);
        dt <= end;
        dt.setDate(dt.getDate() + 1)
      ) {
        arr.push(new Date(dt));
      }
      return arr;
    }
  }

  saveBookings() {
    Swal.fire({
      title: 'Confirm Booking?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes',
    }).then((result) => {
      if (result.value) {
        const result2 = this.bookings.map((b) => ({
          date_time: this.formatDate(b.date_time),
          reason: b.reason,
          emp_id: this.storageService.getCookie('id'),
        }));
        this._loading = true;
        this._loadingMsg = 'Saving your booking';
        this.bookingService.addBooking(result2).subscribe(
          (data) => {
            Swal.fire(
              'Success!',
              'Your booking has been sent.',
              'success'
            ).then(() => {
              this.router.navigateByUrl('/mybookings');
            });
          },
          (err) => {
            console.log(err);
            this._loading = false;
            if (err.status == 403) {
              this.error =
                'Error : There is already a booking made on a submitted date. Please contact your report line if you want to make any change. ';
            } else {
              this.error = err.error;
            }
          }
        );
      }
    });
  }
  formatDate(d) {
    const r = d.toLocaleString('en-US', { timeZone: 'Asia/Dubai' });
    return r;
  }
}
