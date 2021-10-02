import { Component, OnInit } from '@angular/core';
import { addDays } from '@progress/kendo-date-math';
import { addWeeks } from '@progress/kendo-date-math';
import { addMonths } from '@progress/kendo-date-math';
import { convertToCSV } from '../../utils/generateCSV';
import { ReportService } from '../../services/report/report.service';

const date = new Date();
var day = addDays(date, -1);

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css'],
})
export class ReportComponent {
  error = false;
  _loading: boolean = false;

  constructor(private reportService: ReportService) {}

  firstDay = new Date(); //returns first date of the month, `2000-11-1`
  weekly = (this.firstDay = addDays(date, -7));

  public range = { start: day, end: day };

  public changeWeek(x: any): void {
    this.range = { start: addWeeks(day, -x), end: day };
  }
  public changeMonth(y: any): void {
    this.range = { start: addMonths(day, -y), end: day };
  }

  generate_csv() {
    this.error = false;
    this._loading = true;
    if (this.range.start.getTime() != this.range.end.getTime()) {
      this.range.start.setHours(this.range.start.getHours() + 4);
      this.range.end.setHours(this.range.end.getHours() + 4);
      var from =
        this.range.start.toISOString().substr(0, 10) + ' ' + '00:00:00';
      var to = this.range.end.toISOString().substr(0, 10) + ' ' + '00:00:00';
    } else {
      var from =
        this.range.start.toISOString().substr(0, 10) + ' ' + '00:00:00';
      var to = this.range.end.toISOString().substr(0, 10) + ' ' + '00:00:00';
    }

    this.reportService.getStats(from, to).subscribe(
      (data: any) => {
        let reports = data;
        this._loading = false;
        if (reports.length > 0) {
          let r2 = reports.map((r) => ({
            'Employee ID': r.emp_id,
            'Employee Name': r.name,
            'Region': r.region,
            'Number of times travelled': r.travel_count,
            'Cost Center': r.costcenter_name,
            'Segment': r.segment_name,
          }));
          convertToCSV(r2, `${from} to ${to}.csv`);
        } else {
          this.error = true;
        }
      },
      (err) => {
        console.log(err);
        this._loading = false;
      }
    );
  }

  min = new Date('1/1/2015');

  max = addMonths(date, 3);

  ngOnInit(): void {
    // const {total_num_bookings,total_num_users,total_num_bookings_pending,total_num_bookings_approved,total_num_bookings_rejected} = data[0];
    // this.totalBookingsToday =total_num_bookings ;
    // this.totalUsers = total_num_users;
    // this.bookings_status.push({name:"Rejected",y:+total_num_bookings_rejected})
    // this.bookings_status.push({name:"Approved",y:+total_num_bookings_approved})
    // this.bookings_status.push({name:"Pending",y:+total_num_bookings_pending})
    // this.bookings_week = data[1]
    // this._loading=false;
  }
}
