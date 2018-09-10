import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { User } from './User';
import { HttpHeaders } from '@angular/common/http';
import { Login} from './Login';
import {Router} from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class SignUpService {
  private header: HttpHeaders;
  constructor(private http: Http) {
    this.header = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });

  }

  USerSignUp(user: User) {
    return this.http.post("https://localhost:44397/api/Users/Register", user);
  }
  USerLogIn(user: Login) {
    return this.http.post("https://localhost:44397/api/Users/Login", user);
  }
  UserLogOut(){
    return this.http.post("https://localhost:44397/api/Users/Logout","");
  }
  GetDomain(){
    type ArrayOfTypeDomain = Array<{name: string}>;
    const arr: ArrayOfTypeDomain= [
      {name: 'C#'},
      {name : 'Java'},
      {name: 'Algorithms'},
      {name: 'Python'},
  ];
  return arr;
  }
}
