import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}; 

  constructor(private auth: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.auth.login(this.model).subscribe(next => {
      console.log('Logged in!');
    }, error => {
      console.log('Error: login failed.');
    })
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token; // !! returns true for items that exist and false for those that do not. 
  }

  logout() {
    localStorage.removeItem('token'); 
    console.log('Logged out.'); 
  }

}
