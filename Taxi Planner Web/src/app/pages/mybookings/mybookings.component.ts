import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { BookingService } from 'src/app/services/booking/booking.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import Swal from 'sweetalert2';
import { ChangeDateFormat } from '../../utils/time';

export interface BookingElement {
  datetime: string;
  status: string;
  evaluation_date: string;
  evaluation_by: string;
}
@Component({
  selector: 'app-mybookings',
  templateUrl: './mybookings.component.html',
  styleUrls: ['./mybookings.component.css'],
})
export class MybookingsComponent implements OnInit {
  displayedColumns: string[] = ['date', 'time', 'status', 'reason'];
  dataSource = new MatTableDataSource();
  _loading: boolean = true;
  _hasBookings: boolean;
  _search: string = '';

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  @ViewChild(MatSort, { static: false })
  set sort(status: MatSort) {
    this.dataSource.sort = status;
  }
  constructor(private bookingService: BookingService) {}

  ngOnInit(): void {
    this.bookingService.getMyBookings().subscribe(
      (data) => {
        this._hasBookings = data.length > 0;
        const newData = (this.dataSource = new MatTableDataSource([
          ...data.map((a) => {
            let status;
            switch (a.status) {
              case 0:
                status = 'Pending';
                break;
              case 1:
                status = 'Approved';
                break;
              case 2:
                status = 'Rejected';
                break;

              default:
                break;
            }
            return {
              ...a,
              status,
              date: ChangeDateFormat(a.date_time),
              time: a.date_time.slice(11, 16),
            };
          }),
        ]));
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this._loading = false;
      },
      (err) => {
        console.log(err);
        this._loading = false;
      }
    );
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  showComment(comment: string): void {
    Swal.fire({
      title: 'Reason',
      text: comment,
      cancelButtonColor: '#d33',
    });
  }
}
