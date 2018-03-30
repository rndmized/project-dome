import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { AppSettingsService } from './app-settings.service';
import { Observable } from 'rxjs/Observable';
import { Response } from '@angular/http/src/static_response';
import { User } from '../models/User';

@Injectable()
export class DataService {

  private apiURL = this.appSettings.getApiURL();

  constructor(private http: Http, public appSettings: AppSettingsService) { }

  getRegisteredUsers(): Observable<any> {
    return this.http.get(this.apiURL + 'getRegisteredUsers')
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
