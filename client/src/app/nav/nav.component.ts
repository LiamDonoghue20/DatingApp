import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}


  //service set as public so it can be used in the template
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    
    }), error => {
      console.log(error)
    }
  }

  logout(){
    this.accountService.logout();
  }



}
