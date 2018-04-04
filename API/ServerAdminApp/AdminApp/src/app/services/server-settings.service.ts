import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { AuthenticationService } from './authentication.service';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { Settings } from '../models/settings';

@Injectable()
export class ServerSettingsService {

  private apiURL = this.appSettings.getApiURL();
  private gameServerURL = this.appSettings.getGameServerURL();

  constructor(private http: Http, public appSettings: AppSettingsService, private authenticationService: AuthenticationService) { }


restartServer(): Observable<boolean> {
    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authenticationService.token});
    let options = new RequestOptions({ headers: headers });
    return this.http.get(this.gameServerURL + 'restartServer', options)
        .map((response: Response) => {
            let res = response.json();
            if (res.success) {
              /** DISPLAY MESSAGE ?? */
                return true;
            } else {
                console.log(res.msg);
                return false;
            }
        });
}

changeSettings( port : number , concurrent_players : number, restart : boolean): Observable<boolean> {
  return this.http.post(this.gameServerURL + 'changeSettings', { port : port, concurrent_players: concurrent_players, restart : restart, token: this.authenticationService.token })
      .map((response: Response) => {
          let res = response.json();
          if (res.success) {
            /** DISPLAY MESSAGE ?? */
              return true;
          } else {
              console.log(res.msg);
              return false;
          }
      });
}

getCurrentSettings(): Observable<any> {
    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authenticationService.token});
    let options = new RequestOptions({ headers: headers });
    return this.http.get(this.gameServerURL + 'getSettings', options)
        .map((response: Response) => {
            let res = response.json();
            return res;
        });
  }

}
