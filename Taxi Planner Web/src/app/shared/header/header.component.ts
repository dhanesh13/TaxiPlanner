import { Component, OnInit } from '@angular/core';
import { StorageService } from 'src/app/services/storage/storage.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styles: [],
})
export class HeaderComponent implements OnInit {
  _name;
  _role;
  constructor(private storageService: StorageService) {}

  ngOnInit(): void {
    this._name = this.storageService.getCookie('name');
  }
}
