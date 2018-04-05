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

  /** Log out on navigation this page. */
  ngOnInit() {
    this.authenticationService.logout();
  }

  /** Login call Authentication service to verify whether the provided
   * user data matches the one in the database.
   */
  logIn(): void {
    this.loading = true;
    this.authenticationService.login(this.user.username, this.user.password)
        .subscribe(result => {
            if (result === true) {
                // On login successful navigate to default route.
                this.router.navigate(['/']);
            } else {
                // On login failed display error message.
                this.error = 'Username or password is incorrect';
                this.loading = false;
            }
          });
  }

}
