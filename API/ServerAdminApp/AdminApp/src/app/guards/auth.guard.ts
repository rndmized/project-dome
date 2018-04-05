import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { Observable } from 'rxjs/Observable';

/** AuthGuard Classs
 * 
 * When added to a route, it will check on the token,
 * if there is no token it will redirect user to login page,
 * thus protecting certain routes to be accesed without login
 * in.
 */
@Injectable()
export class AuthGuard implements CanActivate {
  
  constructor(private router: Router) { }

  canActivate() {
    if (localStorage.getItem('currentUser')) {
        // logged in so return true
        return true;
    }
    // not logged in so redirect to login page
    this.router.navigate(['/login']);
    return false;
}
}
