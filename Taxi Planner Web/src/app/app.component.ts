import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { StorageService } from './services/storage/storage.service';
import { AuthService } from './services/auth/auth.service';
import {CookieService} from 'ngx-cookie-service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'TaxiPlanner';
  show: boolean = false;

  constructor(
    location: Location,
    private router: Router,
    private storageService: StorageService,
    private authService: AuthService,
    public cookieService:CookieService
  ) {
    router.events.subscribe((val) => {
      this.show = location.path() != '' ? true : false;
    });
  }

  ngOnInit() {

    

    this.GetQueryStringParameter()
   

    // Redirect user if cookie is valid
    if (this.authService.isAuthenticated() && window.location.pathname == '/') {
      const role = this.storageService.getCookie('taxi_role');
      if (
        role == 'employee' ||
        role == 'delegated_approver' ||
        role == 'approver'
      ) {
        this.router.navigate(['/mybookings']);
      } else if (role == 'hr' || role == 'superadmin') {
        this.router.navigate(['/dashboard']);
      }
    }
  }

  getQueryStringValue (key:string) {  
    return decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));  
  }  
  
  GetQueryStringParameter(){

    // console.log("id is: "+this.getQueryStringValue("i")); 
    // console.log("name is: "+this.getQueryStringValue("n")); 
    // console.log("role is: "+this.getQueryStringValue("r")); 
    // console.log("token is: "+this.getQueryStringValue("t")); 
    console.log("Value i:" + this.getQueryStringValue("i"));
    if(this.getQueryStringValue("i") != ''){
      this.storageService.createCookie('id', this.getQueryStringValue("i"), 1);
    }
    if(this.getQueryStringValue("n") != ''){
      this.storageService.createCookie('name',this.getQueryStringValue("n"), 1);
    }
    if(this.getQueryStringValue("r") != ''){
      this.storageService.createCookie('role', this.getQueryStringValue("r"), 1);
      this.storageService.createCookie('taxi_role', this.getQueryStringValue("r"), 1);
    }
    if(this.getQueryStringValue("t") != ''){
      this.storageService.createCookie('token', this.getQueryStringValue("t"), 1);
    }


    // const id = this.storageService.getCookie('id');
    // const role = this.storageService.getCookie('role');
    // const token = this.storageService.getCookie('token');
    // const name = this.storageService.getCookie('name');
    // const taxiRole = this.storageService.getCookie('taxi_role');
  }
}
