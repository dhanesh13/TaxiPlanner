import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UsersService } from 'src/app/services/users/users.service';
import { UserModel } from './user.model';

@Component({
  selector: 'app-usermgt',
  templateUrl: './usermgt.component.html',
  styleUrls: ['./usermgt.component.css'],
})
export class UsermgtComponent implements OnInit {
  _loading: boolean = true;
  displayedColumns: string[] = [
    'emp_id',
    'emp_name',
    'emp_email',
    'emp_role',
    'department',
    'costcenter',
    'emp_region',
    'emp_address',
  ];

  dataSource: MatTableDataSource<UserModel>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false })
  set sort(status: MatSort) {
    if (this.dataSource) this.dataSource.sort = status;
  }

  constructor(private usersService: UsersService) {}

  ngOnInit() {
    
    this.usersService.getUsers().subscribe(
      
      (data: Array<UserModel>) => {
        console.log(data)
        this.dataSource = new MatTableDataSource(data);
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
}
