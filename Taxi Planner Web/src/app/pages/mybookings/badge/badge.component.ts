import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'bookings-badge',
  templateUrl: './badge.component.html',
  styleUrls: ['./badge.component.css']
})
export class BadgeComponent implements OnInit {

  constructor() { }

  @Input() title;

  ngOnInit(): void {
  }

}
