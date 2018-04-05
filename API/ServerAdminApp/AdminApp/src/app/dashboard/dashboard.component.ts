import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ServerSettingsService } from '../services/server-settings.service';
import { DataService } from '../services/data.service';
import { OnlinePlayer } from '../models/OnlinePlayer';
import { User } from '../models/User';
import { Settings } from '../models/settings';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  /**  */
  public playersOnline = {};
  public users = {};
  public settings = new Settings();

  /** Query Serber for Relevant information */
  constructor(private serverSettingsService : ServerSettingsService, private dataService : DataService) { 
    this.dataService.listPlayers().subscribe(players => this.playersOnline = players);
    this.serverSettingsService.getCurrentSettings().subscribe(settings => this.settings = settings);
    this.dataService.getRegisteredUsers().subscribe(users => this.users = users);
  }

  ngOnInit() {
  }

}
