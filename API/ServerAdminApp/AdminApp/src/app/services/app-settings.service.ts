import { Injectable } from '@angular/core';

const CONFIG = {
  apiURL: 'http://127.0.0.1:3000/',
  gameServerURL: 'http://127.0.0.1:5500/'
}

@Injectable()
export class AppSettingsService {



  constructor() { }

  public getApiURL() {
      return CONFIG.apiURL;
  }

  public getGameServerURL() {
    return CONFIG.gameServerURL;
  }

  public setGameServerURL( GameURL: string) {
    CONFIG.gameServerURL = GameURL;
  }

}
