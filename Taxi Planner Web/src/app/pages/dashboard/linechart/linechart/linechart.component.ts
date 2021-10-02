import { Component, OnInit, Input } from '@angular/core';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'report-linechart',
  templateUrl: './linechart.component.html',
  styleUrls: ['./linechart.component.css'],
})
export class LinechartComponent implements OnInit {
  @Input() bookings_week;
  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {};
  chart_y = [];
  chart_x = [];

  constructor() {}

  ngOnInit(): void {
    this.bookings_week.forEach((b) => {
      this.chart_y.push(b['totalSent']);
      this.chart_x.push(b['date']);
    });

    this.chartOptions = {
      series: [
        {
          dataLabels: {
            enabled: true,
            format: '{point.y}',
          },

          data: [...this.chart_y],

          type: 'line',
        },
      ],
      title: {
        text:
          '<span style="font-size: 20px">Approved bookings – Last 7 days</span>',
      },

      xAxis: { categories: [...this.chart_x] },

      credits: {
        enabled: false,
      },
      yAxis: {
        title: null,
      },

      legend: {
        enabled: false,
      },
    };
  }
}
