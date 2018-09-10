import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent} from './dashboard/dashboard.component';
import {HomeComponent} from './home/home.component';

const route: Routes = [
    {  path: '', component: HomeComponent },
    { path: 'home', component: DashboardComponent }
   
]

@NgModule({
    imports: [
        RouterModule.forRoot(route)
    ],
    exports: [RouterModule],
    declarations: []
})
export class AppRoutingModule { }