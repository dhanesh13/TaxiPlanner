import { Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { DashboardService } from 'src/app/services/dashboard/dashboard.service';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ChangeDateFormat } from '../../utils/time';

export interface RegionData {
  region_name: string;
  dailyregionbookingns: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  _loading: boolean = true;
  cards_count_obj = {};
  bookings_status = [];
  bookings_week: any = [];
  bookings_region: any = [];
  breakpoint;
  totalBookingsToday;
  totalUsers;
  hasData: boolean = false;

  displayedColumns: string[] = ['region_name', 'dailyregionbookings'];
  dataSource: MatTableDataSource<RegionData>;

  // @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false })
  set sort(status: MatSort) {
    if (this.dataSource) this.dataSource.sort = status;
  }

  constructor(private dashboardtService: DashboardService) {}

  ngOnInit(): void {
    this.breakpoint = window.innerWidth <= 400 ? 1 : 6;

    const calls = forkJoin([
      this.dashboardtService.getStats(),
      this.dashboardtService.getWeekly(),
      // this.dashboardtService.getRegion(),
    ]);

    calls.subscribe(
      (data: any) => {
        const {
          total_num_bookings,
          total_num_users,
          total_num_bookings_pending,
          total_num_bookings_approved,
          total_num_bookings_rejected,
        } = data[0];
        this.hasData = data.length > 0 && total_num_bookings > 0;
        this.totalBookingsToday = total_num_bookings;
        this.totalUsers = total_num_users;

        this.bookings_status.push({
          name: 'Rejected',
          y: +total_num_bookings_rejected,
        });
        this.bookings_status.push({
          name: 'Approved',
          y: +total_num_bookings_approved,
        });
        this.bookings_status.push({
          name: 'Pending',
          y: +total_num_bookings_pending,
        });

        this.bookings_week = data[1].map((a) => {
          return { ...a, date: ChangeDateFormat(a.date_time) };
        });

        // this.bookings_region = data[2];
        // console.log(data[2])
        // this.dataSource = new MatTableDataSource(data[2]);
        // // this.dataSource.paginator = this.paginator;
        // this.dataSource.sort = this.sort;

        this._loading = false;
      },
      (err) => {
        console.log(err);
        this._loading = false;
      }
    );
  }
}
