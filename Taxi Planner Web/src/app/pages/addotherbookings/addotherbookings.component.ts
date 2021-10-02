//Start #F04-Khidir
import { Component, OnInit, ViewChild, ɵConsole } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { BookingService } from 'src/app/services/booking/booking.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Observable, forkJoin } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

import { ViewEncapsulation } from '@angular/core';
import { PreventableEvent } from '@progress/kendo-angular-dateinputs';
import { Router } from '@angular/router';
import { Time } from 'highcharts';
import Swal from 'sweetalert2';

import { UsersService } from 'src/app/services/users/users.service';
import { MatTableDataSource } from '@angular/material/table';
import { addDays } from '@progress/kendo-date-math';
import { StorageService } from 'src/app/services/storage/storage.service';

interface BookingData {
  booking_id: number;
  emp_id: number;
  name: string;
  date_time: string;
  reason: string;
  status: number;
  team_name: string;
  costcenter_name: string;
}

interface Dropdown {
  value: string;
  viewValue: string;
}

interface User {
  name: string;
}

export interface UserData {
  emp_name: string;
}
@Component({
  selector: 'app-addotherbookings',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './addotherbookings.component.html',
  styleUrls: ['./addotherbookings.component.css'],
})
export class AddotherbookingsComponent implements OnInit {
  nameInfo: string;

  employeeControl = new FormControl(null, Validators.required);
  options: User[] = [];
  filteredOptions: Observable<User[]>;

  displayedNames: string[] = [];

  public preventOpen: boolean;
  public preventClose: boolean;
  public dialog_opened = false;

  firstDay = new Date();
  public range = { start: this.firstDay, end: new Date() };

  format = 'EEEE, MMMM d, y';
  today = new Date();

  form = new FormGroup({
    today: new FormControl(),
  });

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

  max_time2: Date = new Date(
    this.today.getFullYear(),
    this.today.getMonth(),
    this.today.getDate(),
    20,
    30,
    0,
    0
  );

  public events: string[] = [];
  public listItems: Array<String> = [];

  error = '';
  bookings = [];
  displayedColumns: string[] = ['Date', 'Time', 'Comment', 'Remove'];
  current_booking_date: Date = new Date();
  current_booking_comment = '';
  current_booking_time = 'hr/min';

  pageSizeOptions: number[] = [6, 12];
  @ViewChild(MatPaginator, { static: false })
  set paginator(value: MatPaginator) {
    this.dataSource.paginator = value;
  }
  // @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  // COMPONENT STATE
  dataSource: MatTableDataSource<any>;
  // FILTERS
  _search: string = '';
  _selectedTeam: string = '';
  _selectedCostCenter: string = '';
  _selectedStatus: number = -1;
  allBookings: Array<BookingData>;
  _loading: boolean = true;
  _showClearButton: boolean = false;
  _loadingMessage = 'Taxi Planner';
  // DROPDOWNS
  _teamOptions: Dropdown[];
  _nameOptions: Dropdown[];
  _costCenterOptions: Dropdown[];
  _statusOptions: Dropdown[];
  employeeForm: FormGroup;
  _disableForm: boolean = false;

  constructor(
    private bookingService: BookingService,
    private usersService: UsersService,
    private router: Router,
    private storageService: StorageService
  ) { }

  getName(): Observable<any> {
    return this.usersService.getName(this.displayedNames);
  }

  ngOnInit() {
    this.employeeForm = new FormGroup({
      employee: new FormControl('', [Validators.required]),
    });

    const services = forkJoin([
      this.usersService.getUsers(),
      this.bookingService.getBookingTimes(),
    ]);

    services.subscribe((data) => {
      this._loading = false;

      console.log(data)

      // generate times
      this.listItems = data[1]
        .filter((a) => a.schedule_type == 0)
        .map((a) => a.time.slice(11, 16));

      this.filteredOptions = this.employeeControl.valueChanges.pipe(
        startWith(''),
        map((value) => (value ? this._filter(value) : this.options.slice()))
      );

      const mydata = data[0].filter(
        (d) => d.user_id != this.storageService.getCookie('id')
      );

      this.options = mydata.map((a) => {
        return {
          name: '#' + a.user_id + '\xa0 \xa0' + a.user_name,
          value: a.user_id,
        };
      });
    });
  }

  displayFn(user: User): string {
    return user && user.name ? user.name : '';
  }

  private _filter(name: string): User[] {
    const filterValue = name.toLowerCase();

    return this.options.filter(
      (option) => option.name.toLowerCase().indexOf(filterValue) > -1
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
    this._disableForm = false;
    this.employeeControl.reset();

    this.filteredOptions = this.employeeControl.valueChanges.pipe(
      startWith(''),
      map((value) => (value ? this._filter(value) : this.options.slice()))
    );
  }

  removeBooking(index) {
    this.bookings = this.bookings.filter((b) => b.index != index);
    this.dataSource = new MatTableDataSource([]);
    this.dataSource = new MatTableDataSource(this.bookings);
    this.dataSource.paginator = this.paginator;
    this.error = '';
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
        if (!this.employeeControl.valid) {
          this.error = 'Please choose an employee.';
        } else {
          // console.log('working');
          this._disableForm = true;
          const date_item = new Date(r[index]);
          const mydate =
            this.ChangeDateFormat(date_item) +
            'T' +
            this.current_booking_time +
            ':00';

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
          // console.log(this.bookings)
          this.dataSource = new MatTableDataSource(this.bookings);
        }
      }
    }

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

  handleChange() {
    if (this.employeeForm.valid) {
      // this._disableForm = true;
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
        this._loading = true;
        this._loadingMessage = 'Adding Booking';
        var result2 = this.bookings.map((b) => ({
          date_time: this.formatDate(b.date_time),
          reason: b.reason,
          emp_id: this.employeeControl.value.value,
        }));

        console.log(result2);

        this.bookingService.addBooking(result2).subscribe(
          (data) => {
            Swal.fire(
              'Success!',
              'Your booking has been saved.',
              'success'
            ).then(() => {
              // this.router.navigateByUrl('/managebookings');
              // window.location.href = '/managebookings';
              this.employeeControl.reset();
              this.removeAll();
              this._loading = false;
              this._loadingMessage = '';
            });
          },
          (err) => {
            console.log(err);
            this._loading = false;
            if (err.status == 403) {
              this.error =
                'Error : There is already a booking made on a submitted date. The bookings can be found in "Booking Management" section.';
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
//End #F04-Khidir
