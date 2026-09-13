import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';


@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {

  constructor(private router: Router, private localStorageService: LocalStorageService) { }

  ngOnInit(): void {
    this.logout();
  }
  logout(): void {
    console.log('logout');
    dispatchEvent(new Event('refrescar'));
    this.localStorageService.clearLogin();
    this.router.navigate(['/']);
  }

}
