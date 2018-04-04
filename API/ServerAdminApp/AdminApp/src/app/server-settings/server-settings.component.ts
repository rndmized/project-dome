import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-server-settings',
  templateUrl: './server-settings.component.html',
  styleUrls: ['./server-settings.component.css']
})
export class ServerSettingsComponent implements OnInit {

  settingsForm = new FormGroup({
    port: new FormControl('', [Validators.required, Validators.minLength(5), Validators.maxLength(10)]),
    concurrent_players: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(8)])	 
 });


  constructor() { }

  ngOnInit() {
  }

}
