import { Injectable } from '@angular/core';

const CONFIG = {
  apiURL: 'http://127.0.0.1:3000/'
}

@Injectable()
export class AppSettingsService {



  constructor() { }

  public getApiURL() {
    return CONFIG.apiURL;
}

}
