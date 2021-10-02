// Components
import { AppComponent } from './app.component';
import { UsermgtComponent } from './pages/usermgt/usermgt.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { ChartComponent } from './pages/dashboard/piechart/chart.component';
import { LinechartComponent } from './pages/dashboard/linechart/linechart/linechart.component';
import { SidebarComponent } from './shared/sidebar/sidebar.component';
import { HeaderComponent } from './shared/header/header.component';
import { CardComponent } from './pages/dashboard/cards/card.component';
import { MybookingsComponent } from './pages/mybookings/mybookings.component';
import { BadgeComponent } from './pages/mybookings/badge/badge.component';
import { FooterComponent } from './shared/footer/footer.component';
import { LoginComponent } from './pages/login/login.component';
import { AddBookingsComponent } from './pages/addbookings/addbookings.component';
import { ManageBookingComponent } from './pages/managebooking/managebooking.component';
import { ReportComponent } from './pages/report/report.component';
import { AddotherbookingsComponent } from './pages/addotherbookings/addotherbookings.component';
import { DelegateComponent } from './pages/delegate/delegate.component';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { DragComponent } from './pages/drag/drag.component';
// Modules
import { NgModule } from '@angular/core';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { CustomMaterialModule } from './material.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HighchartsChartModule } from 'highcharts-angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import {CookieService} from 'ngx-cookie-service'

// Services
import { BookingService } from './services/booking/booking.service';
import { AuthService } from './services/auth/auth.service';
import { UsersService } from './services/users/users.service';
import { ReportService } from './services/report/report.service';
import { DashboardService } from './services/dashboard/dashboard.service';
import { StorageService } from './services/storage/storage.service';
import { AllocatorService } from './services/allocator/allocator.service';
import { MyHttpInterceptorService } from './services/myHttpInterceptor.service';
import { AuthGuard } from './services/auth/auth-guard.service';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    SpinnerComponent,
    ChartComponent,
    LinechartComponent,
    SidebarComponent,
    HeaderComponent,
    FooterComponent,
    CardComponent,
    MybookingsComponent,
    BadgeComponent,
    LoginComponent,
    AddBookingsComponent,
    ReportComponent,
    UsermgtComponent,
    ManageBookingComponent,
    AddotherbookingsComponent,
    DelegateComponent,
    DragComponent,
  ],
  imports: [
    BrowserModule, DragDropModule,
    MatButtonToggleModule,
    CustomMaterialModule,
    HttpClientModule,
    HighchartsChartModule,
    AppRoutingModule,
    DateInputsModule,
    DialogsModule,
    FormsModule,
    SweetAlert2Module,
    DropDownsModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
  ],

  providers: [
    AllocatorService,
    DashboardService,
    BookingService,
    AuthService,
    UsersService,
    ReportService,
    StorageService,
    CookieService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MyHttpInterceptorService,
      multi: true,
    },
    AuthGuard,
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
