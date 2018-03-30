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
    { link: ['/', 'dashboard'], icon: 'home'},
    { link: ['/', 'settings'], icon: 'cog'},
  ];

  subLinks = [
    { link : ['/', 'dash-menu','dashboard'], label: 'Dashboard' },
    { link : ['/', 'posts'], label: 'Posts' },
    { link : ['/', 'todos'], label: 'Todos' },
    { link : ['/', 'dash-menu', 'users'], label: 'Users' },
  ];

}
