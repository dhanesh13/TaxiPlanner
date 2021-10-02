import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import Swal from 'sweetalert2';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { addDays } from '@progress/kendo-date-math';

import { startWith, map } from 'rxjs/operators';
import { UsersService } from 'src/app/services/users/users.service';
import { DelegateroleService } from 'src/app/services/delegaterole/delegaterole.service';
import { stringify } from '@angular/compiler/src/util';
import { StorageService } from 'src/app/services/storage/storage.service';

export interface UserGroup {
  letter: string;
  names: string[];
  emp_id: any[];
}

interface UserData {
  user_id: number;
  user_name: string;
}

interface Delegate {
  value: any;
  viewValue: string;
}

export const _filter = (opt: any[], value: any): any[] => {
  const filterValue = value;
  return opt.filter((item) => {
    return item.toString().toLowerCase().indexOf(filterValue) === 0;
  });
};

@Component({
  selector: 'app-delegate',
  templateUrl: './delegate.component.html',
  styleUrls: ['./delegate.component.css'],
})
export class DelegateComponent implements OnInit {
  public range = { start: null, end: null };
  employeeForm: FormGroup;
  userGroups: UserGroup[];
  usersOptions: Observable<UserGroup[]>;
  superior_id: number;
  delegator_id: number;
  data;
  _loading = true;
  _loadingMessage = 'Delegate Role';

  constructor(
    private usersService: UsersService,
    private DelegateroleService: DelegateroleService,
    private router: Router,
    private storageService: StorageService
  ) { }

  ngOnInit(): void {
    this.employeeForm = new FormGroup({
      employee: new FormControl('', [Validators.required]),
    });
    this.usersService.getUsers().subscribe(
      (data: Array<UserData>) => {
        this._loading = false;
        let usernameByAlphabets = {};
        let userI = {};
        data = data.filter(a => a.user_name.length > 0)
        // console.log(data)
        this.data = data.sort((a, b) => {
          if (a.user_name.toLowerCase() < b.user_name.toLowerCase()) {
            return -1;
          }
          if (a.user_name.toLowerCase() > b.user_name.toLowerCase()) {
            return 1;
          }
        });

        for (let user of data) {
          const start_alphabet = user.user_name[0].toLowerCase();
          const start_id = user.user_id[0];

          if (start_alphabet in usernameByAlphabets) {
            usernameByAlphabets[start_alphabet].push(user.user_name);
            userI[start_alphabet].push(user.user_id);
          } else {
            usernameByAlphabets[start_alphabet] = [user.user_name];
            userI[start_alphabet] = [user.user_id];
          }
        }

        this.userGroups = Object.keys(usernameByAlphabets).map((a) => {
          return {
            letter: a,
            names: usernameByAlphabets[a],
            emp_id: userI[a],
          };
        });

        this.usersOptions = this.employeeForm
          .get('employee')!
          .valueChanges.pipe(
            startWith(''),
            map((value) => this._filterGroup(value))
          );
      },
      (err) => {
        console.log(err);
        this._loading = false;
      }
    );
  }

  min = new Date();
  public fromMinDate: Object = new Date();
  public listItems: Array<String> = ['Department', 'Team'];
  error = '';
  current_category: string = 'Delegate To';

  public valueChange(value: any): void {
    this.current_category = value;
  }

  checkErrors() {
    let errors = '';
    if (!this.employeeForm.valid) {
      errors = 'Please choose an employee ';
    } else if (this.range.start == null || this.range.end == null) {
      errors = 'Please choose a date range ';
    }

    this.error = errors;
    return errors.length == 0;
  }

  private _filterGroup(value: any): UserGroup[] {
    if (value) {
      return this.userGroups
        .map((group) => ({
          letter: group.letter,
          names: _filter(group.names, value),
          emp_id: _filter(group.emp_id, value),
        }))
        .filter((group) => group.names.length > 0);
    }

    return this.userGroups;
  }

  confirm() {
    if (this.checkErrors()) {
      if (this.range.start.getTime() != this.range.end.getTime()) {
        this.range.start.setHours(this.range.start.getHours() + 4);
        this.range.end.setHours(this.range.end.getHours() + 4);
        var debut =
          this.range.start.toISOString().substr(0, 10) + ' ' + '00:00:00';
        var fin = this.range.end.toISOString().substr(0, 10) + ' ' + '00:00:00';
      } else {
        this.range.start.setHours(this.range.start.getHours() + 4);
        this.range.end.setHours(this.range.end.getHours() + 4);
        var debut =
          this.range.start.toISOString().substr(0, 10) + ' ' + '00:00:00';
        var fin = this.range.end.toISOString().substr(0, 10) + ' ' + '00:00:00';
      }

      Swal.fire({
        title: 'Are you sure you want to give this person access rights?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
      }).then((result) => {
        if (result.value) {
          this._loading = true;
          this._loadingMessage = 'Delegation in progress';
          const name = this.employeeForm.value.employee;
          const id = Number(this.storageService.getCookie('id'));

          const user = this.data.filter((a) => a.user_name == name);
          this.DelegateroleService.delegaterole(
            id,
            debut,
            fin,
            user[0].user_id
          ).subscribe(
            (data) => {
              Swal.fire('Success!', 'Delegation completed.', 'success').then(
                () => {
                  this.range = { start: null, end: null };
                  this.employeeForm.reset();
                  this._loading = false;
                  this._loadingMessage = 'Delegate Role';
                }
              );
            },
            (err) => {
              console.log(err);
              this.error = err;
              this._loadingMessage = 'Delegate Role';
              this._loading = false;
            }
          );
        }
      });
    }
  }
}
