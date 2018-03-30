import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { AppSettingsService } from './app-settings.service';
import { Observable } from 'rxjs/Observable';
import { User } from '../models/User';
import { AuthenticationService } from './authentication.service';

@Injectable()
export class DataService {

  private apiURL = this.appSettings.getApiURL();

  constructor(private http: Http, public appSettings: AppSettingsService, private authenticationService: AuthenticationService) { }

  getRegisteredUsers(): Observable<any> {

    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authenticationService.token});
    let options = new RequestOptions({ headers: headers });
    return this.http.get(this.apiURL + 'getRegisteredUsers', options)
        .map((response: Response) => {
            let token = response.json();
            let users = [];
            token.forEach(element => {
              users.push(element);
            });
            return users;
        });
}

}
