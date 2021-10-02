import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { BookingService } from 'src/app/services/booking/booking.service';

import Swal from 'sweetalert2';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from 'src/app/services/storage/storage.service';
import { forkJoin } from 'rxjs';
interface BookingData {
  booking_id: number;
  user_id: number;
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
@Component({
  selector: 'managebooking',
  styleUrls: ['managebooking.component.css'],
  templateUrl: 'managebooking.component.html',
})
export class ManageBookingComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false })
  set sort(status: MatSort) {
    if (this.dataSource) this.dataSource.sort = status;
  }

  emp_role;
  emp_id;

  // CONSTANTS
  displayedColumns: string[] = [
    'emp_id',
    'name',
    'date',
    'time',
    'costcenter_name',
    'reason',
    // 'team_name',

    'status',
  ];
  bookingTimes = [
    // '01:00',
    // '02:00',
    // '03:00',
    // '04:00',
    // '05:00',
    // '19:30',
    // '20:00',
    // '20:30',
    // '21:00',
    // '21:30',
    // '22:00',
    // '22:30',
    // '23:00',
    // '23:30',
  ];

  _showApproveAllButtons = false;

  // COMPONENT STATE
  dataSource: MatTableDataSource<BookingData>;

  // FILTERS
  _search: string = '';
  _selectedTeam: string = '';
  _selectedCostCenter: string = '';
  _selectedStatus: number = -1;

  allBookings: Array<BookingData>;
  _loading: boolean = true;
  _showClearButton: boolean = false;
  _loadingMessage = 'Loading Bookings';
  _forToday = false;

  // DROPDOWNS
  _teamOptions: Dropdown[];
  _costCenterOptions: Dropdown[];
  _statusOptions;

  constructor(
    private bookingService: BookingService,
    private route: ActivatedRoute,
    private router: Router,
    private storageService: StorageService
  ) {
    this.emp_role = this.storageService.getCookie('role');
    this.emp_id = this.storageService.getCookie('id');
    if (this.emp_role == 'hr') {
      this.displayedColumns = this.displayedColumns.filter(
        (a) => a != 'status'
      );
    }
  }

  ngOnInit() {
    this.fetchBookings();
  }

  fetchBookings(): void {
    const requests = forkJoin([
      this.bookingService.getAllBookings(),
      this.bookingService.getBookingTimes(),
    ]);
    requests.subscribe(
      (data: any) => {
        this.allBookings = data[0];
        this._showApproveAllButtons =
          data[0].filter((a) => a.status == 0).length > 0;
        this.dataSource = new MatTableDataSource(data[0]);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;

        // set dropdowns options
        this._teamOptions = this.getKeys(this.allBookings, 'team_name');
        this._costCenterOptions = this.getKeys(
          this.allBookings,
          'costcenter_name'
        );
        this._statusOptions = [
          { value: 0, viewValue: 'Pending' },
          { value: 1, viewValue: 'Approved' },
          { value: 2, viewValue: 'Rejected' },
        ];

        if (this.route.snapshot.queryParams.status)
          this._selectedStatus = this.getStatusNumber(
            this.route.snapshot.queryParams.status
          );

        // change booking times
        this.bookingTimes = data[1]
          .filter((a: any) => a.schedule_type == 2)
          .map((a) => a.time.slice(11, 16));

        this._loading = false;
        this._loadingMessage = '';
        this.filterBookings();
      },
      (err) => console.log(err)
    );
  }

  clearFilters() {
    this._search = '';
    this._selectedTeam = '';
    this._selectedCostCenter = '';
    this._selectedStatus = -1;
    this._showClearButton = false;
    this.dataSource = new MatTableDataSource(this.allBookings);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.router.navigate(['/managebookings']);
  }

  handleTimeChange(e, id: number, date_time: string): void {
    const initialTime = this.getTime(date_time);
    Swal.fire({
      title: 'Change booking time?',
      text: 'The booking will be approved. Are you sure?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes',
    }).then((result) => {
      if (result.value) {
        const afterMidnight = ['01:00', '02:00', '03:00', '04:00', '05:00'];
        let updated_date_time;
        this._loading = true;
        const prev_date = date_time.slice(11, 16);

        if (
          // increase date
          afterMidnight.indexOf(e.value) > -1 &&
          afterMidnight.indexOf(prev_date) == -1
        ) {
          const new_date = Number(date_time.slice(8, 10)) + 1;
          updated_date_time =
            date_time.slice(0, 8) + new_date + 'T' + e.value + ':00';
        } else if (
          // decrease date
          afterMidnight.indexOf(prev_date) > -1 &&
          afterMidnight.indexOf(e.value) == -1
        ) {
          const new_date = Number(date_time.slice(8, 10)) - 1;
          updated_date_time =
            date_time.slice(0, 8) + new_date + 'T' + e.value + ':00';
        } else {
          // same date
          updated_date_time = date_time.slice(0, 11) + e.value + ':00';
        }
        const body = [
          {
            booking_id: id,
            bookingslot_date_time: date_time,
            updated_date_time,
          },
        ];
        this._loadingMessage = 'Changing Booking Time';
        this.bookingService.changeStatus(body).subscribe(
          (data) => {
            this.clearFilters();
            this.fetchBookings();
          },
          (err) => console.log(err)
        );
      } else {
        e.source.value = initialTime;
      }
    });
  }

  handleDropdownChange(e, type): void {
    this._showClearButton = true;
    if (type === 'team') {
      this._selectedTeam = e.value;
    } else if (type === 'costcenter_name') {
      this._selectedCostCenter = e.value;
    } else if (type == 'status') {
      this._selectedStatus = e.value;
      // this.router.navigate([`/managebookings/${this.getStatus(e.value)}`]);
    }
    this.filterBookings();
  }

  filterBookings(): void {
    let filteredBookings: BookingData[] = [...this.allBookings];

    if (this._selectedCostCenter) {
      filteredBookings = filteredBookings.filter(
        (b) => b.costcenter_name == this._selectedCostCenter
      );
    }

    if (this._selectedTeam) {
      filteredBookings = filteredBookings.filter(
        (b) => b.team_name == this._selectedTeam
      );
    }
    if (this._selectedStatus > -1) {
      filteredBookings = filteredBookings.filter(
        (b) => b.status == this._selectedStatus
      );
    }

    if (this.route.snapshot.queryParams.date) {
      const date = this.route.snapshot.queryParams.date;
      if (date == 'today') {
        filteredBookings = filteredBookings.filter(
          (a) =>
            this.formatDate(a.date_time) ==
            this.formatDate(new Date().toString())
        );
      }
    }

    this._showApproveAllButtons =
      filteredBookings.filter((a) => a.status == 0).length > 0;
    this.dataSource = new MatTableDataSource(filteredBookings);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event): void {
    const filterValue: string = (event.target as HTMLInputElement).value;
    this._search = filterValue;
    this._showClearButton = true;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  changeStatus(id: number, date_time: Date, status: number): void {
    const body = [
      {
        status,
        booking_id: id,
        bookingslot_date_time: date_time,
        approved_by_id: this.emp_id,
      },
    ];
    this._loading = true;
    this._loadingMessage =
      status == 1 ? 'Approving Booking' : 'Declining Booking';
    this.bookingService.changeStatus(body).subscribe(
      (data) => {
        this.fetchBookings();
        this.clearFilters();
      },
      (err) => console.log(err)
    );
  }

  getKeys(data: Array<BookingData>, key: string): Array<Dropdown> {
    let keys_set: Set<string> = new Set();
    data.forEach((d: BookingData) => {
      keys_set.add(d[key]);
    });
    return Array.from(keys_set).map((a) => {
      return { value: a, viewValue: a };
    });
  }

  getStatus(status: number): string {
    if (status == 0) return 'pending';
    if (status == 1) return 'approved';
    if (status == 2) return 'rejected';
  }

  getStatusNumber(status: string): number {
    if (status == 'pending') return 0;
    if (status == 'approved') return 1;
    if (status == 'rejected') return 2;
  }

  formatDate(d: string): String {
    let d2: Date = new Date(d);
    const dd = String(d2.getDate()).padStart(2, '0');
    const mm = String(d2.getMonth() + 1).padStart(2, '0'); //January is 0!
    const yyyy = d2.getFullYear();
    return dd + '-' + mm + '-' + yyyy;
  }

  getTime(d: string): string {
    return d.slice(11, 16);
  }

  approveAll() {
    Swal.fire({
      title: 'Approve all bookings?',
      text: 'Are you sure?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes',
    }).then((result) => {
      if (result.value) {
        const approve_bookings: BookingData[] = this.allBookings.filter(
          (b) => b.status == 0
        );
        let body: Object[] = [];

        approve_bookings.forEach((b) => {
          body.push({
            status: 1,
            booking_id: b.booking_id,
            bookingslot_date_time: b.date_time,
            approved_by_id: this.emp_id,
          });
        });

        this._loading = true;
        this._loadingMessage = 'Approving Bookings';
        this.bookingService.changeStatus(body).subscribe(
          (data) => {
            console.log(data);
            this.fetchBookings();
            this.clearFilters();
          },
          (err) => console.log(err)
        );
      }
    });
  }

  checkIfValidDate(d) {
    // check if d >= today's date
    return new Date(d) >= new Date();
  }
  showComment(comment: string): void {
    Swal.fire({
      title: 'Reason',
      text: comment,
      cancelButtonColor: '#d33',
    });
  }
}
