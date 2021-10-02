import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth/auth.service';
import { StorageService } from 'src/app/services/storage/storage.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit {
  _role;

  constructor(
    private storageService: StorageService,
    private authService: AuthService
  ) {
    this._role = this.storageService.getCookie('taxi_role');
  }

  logout(): void {
    this.authService.logout();
  }

  ngOnInit(): void {}
}
