import { Component, OnInit } from '@angular/core';
import {SignUpService} from '../sign-up.service';
import {Domain} from '../Domain';
@Component({
  selector: 'app-dashboard-body',
  templateUrl: './dashboard-body.component.html',
  styleUrls: ['./dashboard-body.component.css']
})
export class DashboardBodyComponent implements OnInit {

  constructor(private signupservice: SignUpService ) { 
   
  }
  Domains :Domain[];

  ngOnInit() {
   this.Domains= [
      {name: 'C#'},
      {name : 'Java'},
      {name: 'Algorithms'},
      {name: 'Python'},
  ];
  console.log(this.Domains);
  }

}
