import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { AuthenticationService } from './authentication.service';
import { Http, Headers, RequestOptions, Response, RequestMethod } from '@angular/http';
import { Observable } from 'rxjs';
import { Settings } from '../models/settings';


/** Retrieve Info from the GameServer and Send Requests to restart and update server */
@Injectable()
export class ServerSettingsService {

  private apiURL = this.appSettings.getApiURL();
  private gameServerURL = this.appSettings.getGameServerURL();

  constructor(private http: Http, public appSettings: AppSettingsService, private authenticationService: AuthenticationService) { }

/** Send server request to restart
 * Send token in the header.
 */
restartServer(): Observable<boolean> {
    let headers = new Headers();
    headers.append( 'Authorization', this.authenticationService.token);
    let options = new RequestOptions({ headers: headers });
    return this.http.get(this.gameServerURL + 'restartServer', options)
        .map((response: Response) => {
            let res = response.json();
            if (res.success) {
                return true;
            } else {
                console.log(res.msg);
                return false;
            }
        });
}
/** Change server Settings */
changeSettings( port : number , concurrent_players : number, restart : boolean): Observable<boolean> {
    let headers = new Headers();
    headers.append( 'Authorization', this.authenticationService.token);
    let options = new RequestOptions({
      headers: headers
    });
  return this.http.post(this.gameServerURL + 'changeSettings', { port : port, concurrent_players: concurrent_players, restart : restart, token: this.authenticationService.token },options)
      .map((response: Response) => {
          let res = response.json();
          if (res.success) {
              return true;
          } else {
              console.log(res.msg);
              return false;
          }
      });
}
/** Retrieve Settings from server. */
getCurrentSettings(): Observable<any> {
    let headers = new Headers();
    headers.append( 'Authorization', this.authenticationService.token);
    let options = new RequestOptions({ headers: headers , method: RequestMethod.Get});
    return this.http.get(this.gameServerURL + 'getSettings', options)
        .map((response: Response) => {
            console.log(response);
            let res = response.json();
            return res;
        });
  }

}
