import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuard } from './guards/auth.guard';
import { LayoutComponent } from './layout/layout.component';
import { UsersComponent } from './users/users.component';
import { InAppRootComponent } from './in-app-root/in-app-root.component';
import { ServerSettingsComponent } from './server-settings/server-settings.component';
import { PlayersComponent } from './players/players.component';

/** App routes */
const routes: Routes = [ 
  {
    path: '',
    children: [
      { path: '', redirectTo: '/dash-menu', pathMatch: 'full' },
      { path: 'dash-menu', component: InAppRootComponent, children:[
        { path: '', redirectTo: '/dashboard', pathMatch: 'full'  },
        { path: 'dashboard', component: DashboardComponent },
        { path: 'users', component: UsersComponent },
        { path: 'settings', component: ServerSettingsComponent },
        { path: 'players', component: PlayersComponent },
      ] }
    ], canActivate: [AuthGuard]
  },
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '' }];


  @NgModule({
    imports: [
      RouterModule.forRoot(routes),
    ],
    exports: [
      RouterModule,
    ],
  })
  export class AppRouting{ }
