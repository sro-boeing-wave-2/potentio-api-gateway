import { Component, OnInit } from '@angular/core';
import { SignUpService } from '../sign-up.service';
import {Router} from '@angular/router';
@Component({
  selector: 'app-dashboard-header',
  templateUrl: './dashboard-header.component.html',
  styleUrls: ['./dashboard-header.component.css']
})
export class DashboardHeaderComponent implements OnInit {

  constructor(private signupservice :SignUpService,private router :Router) { }

  ngOnInit() {
  }
  Logout():void{
    this.signupservice.UserLogOut()
    .subscribe(result=> result.status == 200?this.AfterLogout(): this.Message(result.toString()));
  }

  AfterLogout(){
    this.router.navigate(['']);
  }
  Message(result :String){}
}
