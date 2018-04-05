import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { AppSettingsService } from './app-settings.service';
import { Observable } from 'rxjs/Observable';
import { User } from '../models/User';
import { AuthenticationService } from './authentication.service';

/** Data service Retrieves Player/Users data from Both database and game Server. */
@Injectable()
export class DataService {

  private apiURL = this.appSettings.getApiURL();
  private gameServerURL = this.appSettings.getGameServerURL();

  constructor(private http: Http, public appSettings: AppSettingsService, private authenticationService: AuthenticationService) {}

  /** Returns a list of Registered Users  */
  getRegisteredUsers(): Observable < any > {

    let headers = new Headers({
      'Authorization': 'Bearer ' + this.authenticationService.token
    });
    let options = new RequestOptions({
      headers: headers
    });
    return this.http.get(this.apiURL + 'getRegisteredUsers', options)
      .map((response: Response) => {
        let res = response.json();
        let users = [];
        res.forEach(element => {
          users.push(element);
        });
        return users;
      });
  }

  /**************************************** */
  /**************************************** */
  /**  BEWARE - Further testing required!! */
  /**************************************** */
  /**************************************** */
  /** Resturn a list of Online Players */
  listPlayers(): Observable < any > {
    let headers = new Headers({
      'Authorization': 'Bearer ' + this.authenticationService.token
    });
    let options = new RequestOptions({
      headers: headers
    });
    return this.http.post(this.gameServerURL + 'listPlayers', options)
      .map((response: Response) => {
        let res = response.json();
        let onlinePlayers = [];
        res.forEach(element => {
          onlinePlayers.push(element);
        });
        return onlinePlayers;
      });
  }

  /** Kick a given player from game server */
  kickPlayer(player_ID: string, char_ID: string): Observable < boolean > {
    return this.http.post(this.gameServerURL + 'kick', {
        player_ID: player_ID,
        char_ID: char_ID,
        token: this.authenticationService.token
      })
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

  /** Ban a player from login in again */
  banPlayer(username: string): Observable < boolean > {
    let headers = new Headers({
      'Authorization': 'Bearer ' + this.authenticationService.token
    });
    let options = new RequestOptions({
      headers: headers
    });
    return this.http.post(this.apiURL + 'banUser', {
        username: username
      }, options)
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

/** Remove the ban from a player */
  pardonPlayer(username: string): Observable < boolean > {
    let headers = new Headers({
      'Authorization': 'Bearer ' + this.authenticationService.token
    });
    let options = new RequestOptions({
      headers: headers
    });
    return this.http.post(this.apiURL + 'pardonUser', {
        username: username
      }, options)
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

