import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuard } from './guards/auth.guard';
import { LayoutComponent } from './layout/layout.component';


const routes: Routes = [ 
  { path: 'login', component: LoginComponent },
  { path: '', component: LayoutComponent, canActivate: [AuthGuard] },

// otherwise redirect to home
{ path: '**', redirectTo: '' }];

export const AppRouting = RouterModule.forRoot(routes);
