import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/User';
import { AuthenticationService } from '../services/authentication.service'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  
  user: any = {};
  loading = false;
  error = '';

  constructor(private router: Router,
    private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.authenticationService.logout();
    console.log("To login");
  }

  logIn(): void {
    this.loading = true;
    this.authenticationService.login(this.user.username, this.user.password)
        .subscribe(result => {
            if (result === true) {
                // login successful
                this.router.navigate(['/']);
            } else {
                // login failed
                this.error = 'Username or password is incorrect';
                this.loading = false;
            }
          });

 console.log(this.user.username + this.user.password)
  }

}
