import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { Observable } from 'rxjs/Observable';
import { User } from '../models/User';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {


  ngOnInit() {
  }

  users: Observable<User[]>;
  user: User;
  showModal: boolean = false;

  constructor(
    public dataService: DataService,
  ) {
    this.dataService.getRegisteredUsers().subscribe(res => this.users = res);
  }

  public updateUserStatus(user : User){
    if(user.status == 'Banned'){
      this.dataService.pardonPlayer(user.username).subscribe();
    } else {
      this.dataService.banPlayer(user.username).subscribe();
    }
    this.dataService.getRegisteredUsers().subscribe(res => this.users = res);
  }

  openModal($event, user) {
    $event.preventDefault();
    this.user = user;
    this.showModal = true;
  }
  closeModal() {
    this.showModal = false;
    this.user = null;
  }
}
