import { Component, OnInit } from '@angular/core';
import { OnlinePlayer } from '../models/OnlinePlayer';
import { Observable } from 'rxjs/Observable';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.css']
})
export class PlayersComponent implements OnInit {

  players: Observable<OnlinePlayer[]>;
  player: OnlinePlayer;

  constructor(public dataService: DataService,
  ) {
    this.dataService.listPlayers().subscribe(res => this.players = res);
  }
  ngOnInit() {
  }

}
