import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { MybookingsComponent } from './pages/mybookings/mybookings.component';
import { LoginComponent } from './pages/login/login.component';
import { ReportComponent } from './pages/report/report.component';
import { UsermgtComponent } from './pages/usermgt/usermgt.component';
import { ManageBookingComponent } from './pages/managebooking/managebooking.component';
import { AddotherbookingsComponent } from './pages/addotherbookings/addotherbookings.component';
import { AddBookingsComponent } from './pages/addbookings/addbookings.component';
import { DelegateComponent } from './pages/delegate/delegate.component';
import { DragComponent } from './pages/drag/drag.component';
import { AuthGuard } from './services/auth/auth-guard.service';

const routes = [
  {
    path: '',
    component: LoginComponent,
  },
  {
    path: 'mybookings',
    component: MybookingsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'addbookings',
    component: AddBookingsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'managebookings',
    component: ManageBookingComponent,
    canActivate: [AuthGuard],
  },
  { path: 'report', component: ReportComponent, canActivate: [AuthGuard] },
  {
    path: 'manageusers',
    component: UsermgtComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'drag',
    component: DragComponent,
    canActivate: [AuthGuard],
  },
  { path: 'delegate', component: DelegateComponent, canActivate: [AuthGuard] },
  {
    path: 'addotherbookings',
    component: AddotherbookingsComponent,
    canActivate: [AuthGuard],
  },
  { path: '**', redirectTo: '', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot([...routes])],
  exports: [RouterModule],
})
export class AppRoutingModule { }
