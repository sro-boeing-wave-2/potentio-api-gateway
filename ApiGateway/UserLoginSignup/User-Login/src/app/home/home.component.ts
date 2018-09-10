import { Component, OnInit } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import { UserLoginComponent } from '../user-login/user-login.component';
import { UserSignUpComponent } from '../user-sign-up/user-sign-up.component'

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private dialog: MatDialog) { }

  ngOnInit() {
  }
  LogIn() {
    let dialog = this.dialog.open(UserLoginComponent,{ width:'600px',panelClass: 'my-centered-dialog' });
    
  }
  SignUp() {
    let dialog = this.dialog.open(UserSignUpComponent,{ width:'600px',panelClass: 'my-centered-dialog' });
    
  }

}
