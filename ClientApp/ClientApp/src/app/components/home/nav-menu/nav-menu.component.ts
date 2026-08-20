import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  userName: string | null = '';
  isExpanded = false;
  isLoged = false;
  private readonly refreshUserHandler = () => this.refreshUser();

  constructor(private router: Router) {
  }

  ngOnInit(): void {
    window.addEventListener('refrescar', this.refreshUserHandler);
    this.refreshUser();
  }

  ngOnDestroy(): void {
    window.removeEventListener('refrescar', this.refreshUserHandler);
  }

  refreshUser(): void {
    this.userName = localStorage.getItem('fullName');
    this.isLoged = !(this.userName === null || this.userName === '');

    if (!this.isLoged) {
      this.userName = '';
    }
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('fullName');
    dispatchEvent(new Event('refrescar'));
    this.router.navigate(['/']);
  }

  collapse(): void {
    this.isExpanded = false;
  }

  toggle(): void {
    this.isExpanded = !this.isExpanded;
  }
}
