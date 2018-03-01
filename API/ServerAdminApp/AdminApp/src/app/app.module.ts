import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ClarityModule } from "@clr/angular";
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';

import { AppRouting } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';

import { AuthGuard } from './guards/auth.guard';
import { AuthenticationService } from './services/authentication.service'



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent
  ],
  imports: [
    BrowserModule,
    ClarityModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    AppRouting
  ],
  providers: [
    AuthGuard, 
    AuthenticationService
  ],
  bootstrap: [AppComponent]
})

export class AppModule { }
