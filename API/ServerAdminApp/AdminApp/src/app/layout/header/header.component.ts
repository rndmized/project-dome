import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  headerLinks = [
    { link: ['/', 'dash-menu', 'dashboard'], icon: 'home'},
    { link: ['/', 'dash-menu', 'settings'], icon: 'cog'},
  ];

  subLinks = [
    { link : ['/', 'dash-menu','dashboard'], label: 'Dashboard' },
    { link : ['/', 'dash-menu', 'users'], label: 'Users' },
    { link : ['/', 'dash-menu', 'settings'], label: 'Settings' },
  ];

}
