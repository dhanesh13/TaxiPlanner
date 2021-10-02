import { Component, OnInit, Input } from '@angular/core';
import * as Highcharts from 'highcharts';
import { Router } from '@angular/router';

@Component({
  selector: 'report-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css'],
})
export class ChartComponent implements OnInit {
  @Input() bookings_status;
  chart_data = [];

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {};

  constructor(private router: Router) {}

  thisOriginal;

  ngOnInit(): void {
    let thisOriginal = this;
    var stats = ['rejected', 'approved', 'pending'];
    this.chart_data = this.bookings_status;

    this.chartOptions = {
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          point: {
            events: {
              click: function (event) {
                thisOriginal.router.navigate(['managebookings'], {
                  queryParams: { status: stats[this.x], date: 'today' },
                });
              },
            },
          },
          dataLabels: {
            enabled: true,
            format: '{point.name}: {point.percentage:.1f} %',
            style: {
              fontWeight: '100',
            },
          },
        },
      },
      series: [
        {
          name: 'Booking',
          dataLabels: {
            enabled: true,
            format: '<b>{point.name}</b>: {point.percentage:.0f} %',
          },
          data: this.chart_data,
          colors: ['red', 'green', 'orange'],
          type: 'pie',
        },
      ],

      title: {
        text: '<span style="font-size: 20px">Booking Status - Today</span>',
      },

      credits: {
        enabled: false,
      },
    };
  }
}
