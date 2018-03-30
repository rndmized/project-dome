import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ClarityModule } from "@clr/angular";
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';

import { AppRouting } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';

import { AuthGuard } from './guards/auth.guard';
import { AuthenticationService } from './services/authentication.service'
import { AppSettingsService } from './services/app-settings.service';
import { LayoutComponent } from './layout/layout.component';
import { HeaderComponent } from './layout/header/header.component';
import { SidebarComponent } from './layout/sidebar/sidebar.component';
import { MainComponent } from './layout/main/main.component';
import { UsersComponent } from './users/users.component';
import { InAppRootComponent } from './in-app-root/in-app-root.component';
import { ServerSettingsComponent } from './server-settings/server-settings.component';
import { DataService } from './services/data.service';
import { PlayersComponent } from './players/players.component';




@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    LayoutComponent,
    HeaderComponent,
    SidebarComponent,
    MainComponent,
    UsersComponent,
    InAppRootComponent,
    ServerSettingsComponent,
    PlayersComponent
  ],
  imports: [
    BrowserModule,
    ClarityModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    AppRouting,
    HttpModule
  ],
  providers: [
    AuthGuard, 
    AuthenticationService,
    AppSettingsService,
    DataService
  ],
  bootstrap: [AppComponent]
})

export class AppModule { }
