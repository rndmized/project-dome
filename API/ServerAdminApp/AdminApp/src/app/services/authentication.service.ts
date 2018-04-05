import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'


/** Authentication Service   */
@Injectable()
export class AuthenticationService {
    public token: string;
 
    private apiURL = this.appSettings.getApiURL();

    constructor(private http: Http, public appSettings: AppSettingsService) {
        // set token if saved in local storage
        var currentUser = JSON.parse(localStorage.getItem('currentUser'));
        if(currentUser){
            this.token = currentUser.token;
        } else {
            this.logout;
        } 
    }
 
    login(username: string, password: string): Observable<boolean> {
        return this.http.post(this.apiURL + 'loginAdmin', { username: username, password: password })
            .map((response: Response) => {
                // login successful if there's a jwt token in the response
                let token = response.json();
                if (token.success) {
                    this.token = token.token; 
                    // store username and jwt token in local storage to keep user logged in between page refreshes
                    localStorage.setItem('currentUser', JSON.stringify({ username: username, token: token.token }));
 
                    // return true to indicate successful login
                    return true;
                } else {
                    // return false to indicate failed login
                    return false;
                }
            });
    }
 
    logout(): void {
        // clear token remove user from local storage to log user out
        this.token = null;
        localStorage.removeItem('currentUser');
    }
}