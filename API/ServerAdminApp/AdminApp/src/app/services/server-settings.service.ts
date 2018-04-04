import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { AuthenticationService } from './authentication.service';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class ServerSettingsService {

  private apiURL = this.appSettings.getApiURL();
  private gameServerURL = this.appSettings.getGameServerURL();

  constructor(private http: Http, public appSettings: AppSettingsService, private authenticationService: AuthenticationService) { }


  restartServer(): Observable<boolean> {
    return this.http.post(this.gameServerURL + 'restartServer', { token: this.authenticationService.token })
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

changeSettings( port : number , concurrent_players : number): Observable<boolean> {
  return this.http.post(this.gameServerURL + 'changeSettings', { port : port, concurrent_players: concurrent_players , token: this.authenticationService.token })
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

}
