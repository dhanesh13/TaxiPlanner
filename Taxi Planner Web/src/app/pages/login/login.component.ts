import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth/auth.service';
import { StorageService } from 'src/app/services/storage/storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  LoginForm: FormGroup;
  _error: boolean = false;
  _loading: boolean = false;
  _current_date: Date = new Date();

  constructor(
    private authService: AuthService,
    private storageService: StorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.LoginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
    });
  }

  loginUser() {
    this._error = false;

    if (this.LoginForm.valid) {
      this._loading = true;
      const email = this.LoginForm.value.email;
      const pwd = this.LoginForm.value.password;

      this.authService.login(email, pwd).subscribe(
        (data) => {
          console.log(data);
          const { role, token, name, id } = data;

          // Save cookies
          this.storageService.createCookie('id', id, 1);
          this.storageService.createCookie('name', name, 1);
          this.storageService.createCookie('role', role, 1);
          this.storageService.createCookie('token', token, 1);

          // Redirect to initial path
          if (localStorage.getItem('path')) {
            const path = localStorage.getItem('path');
            localStorage.removeItem('path');
            window.location.href = `/${path}`;
          } else {
            //Redirect user depending on role
            window.location.href = '/';
            // if (
            //   role == 'employee' ||
            //   role == 'delegated_approver' ||
            //   role == 'approver'
            // ) {
            //   window.location.href = '/mybookings';
            //   // this.router.navigate(['/mybookings']);
            // } else if (role == 'hr' || role == 'superadmin') {
            //   // this.router.navigate(['/dashboard']);
            //   window.location.href = '/dashboard';
            // }
          }
          this._loading = false;
        },
        (err) => {
          this._error = true;
          this._loading = false;
        }
      );
    }
  }

  get email() {
    return this.LoginForm.get('email') as FormControl;
  }

  get password() {
    return this.LoginForm.get('password') as FormControl;
  }
}
