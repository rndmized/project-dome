import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ServerSettingsService } from '../services/server-settings.service';
import { Settings } from '../models/settings';

/** Server Settings Display and Updates Server Settings info. */
@Component({
  selector: 'app-server-settings',
  templateUrl: './server-settings.component.html',
  styleUrls: ['./server-settings.component.css']
})
export class ServerSettingsComponent implements OnInit {

  private isValidFormSubmitted = null;
  private settings = new Settings();

  settingsForm = new FormGroup({
    port: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(5)]),
    concurrent_players: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(3)])	 
 });


  constructor(private serverSettingsService : ServerSettingsService) {
    this.serverSettingsService.getCurrentSettings().subscribe(res => this.settings = res);
   }

  ngOnInit() {
  }

  /** Validate Input from form */
  onFormSubmit() {
    this.isValidFormSubmitted = false;
    if (this.settingsForm.invalid) {
       return;
    }
    this.isValidFormSubmitted = true;
    this.settings = this.settingsForm.value;
    this.serverSettingsService.changeSettings(this.settings.port, this.settings.concurrent_players, this.settings.restart);
    this.settingsForm.reset();
 }
  /** Reload Settings */
 discardChanges(){
  this.serverSettingsService.getCurrentSettings().subscribe(res => this.settings = res);
 }

}
