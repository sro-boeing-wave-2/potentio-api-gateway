import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormArray } from '@angular/forms';
import { MatDialogRef,MatDialog } from '@angular/material';
import {UserLoginComponent} from '../user-login/user-login.component';
import { SignUpService} from '../sign-up.service';
import { User } from '../User';
@Component({
  selector: 'app-user-sign-up',
  templateUrl: './user-sign-up.component.html',
  styleUrls: ['./user-sign-up.component.css']
})
export class UserSignUpComponent implements OnInit {

  constructor( private fb: FormBuilder,private dialog: MatDialog,
    public dialogRef: MatDialogRef<UserSignUpComponent>,private UserSignUpService :SignUpService) { }

  ngOnInit() {
  }
  CreateSignUpForm = this.fb.group({
    FirstName:[''],
    LastName:[''],
    Contact:[''],
    Email : [''],
    Password: ['']
  });
  onSubmit():void{
    this.UserSignUpService.USerSignUp(this.CreateSignUpForm.value as User)
    .subscribe(result=> result.status == 201?this.GoBack(): this.Message(result.toString()));
  }
 

  GoBack(){
    this.dialogRef.close();
  }
  Message(result:string){  
  }
 
}
