import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styles: [
  ]
})
export class CardComponent implements OnInit {

  constructor() { }

  @Input() totalBookingsToday;
  @Input() totalUsers;

  ngOnInit(): void {
  }

}
