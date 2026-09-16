import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css'],
    standalone: false
})
export class NavMenuComponent implements OnInit, OnDestroy {
  userName: string | null = '';
  profileImageUrl = 'assets/exterior.jpg';
  isExpanded = false;
  isLoged = false;
  canAccessControlPanel = false;
  private readonly refreshUserHandler = () => this.refreshUser();

  constructor(private router: Router, private localStorageService: LocalStorageService) {
  }

  ngOnInit(): void {
    window.addEventListener('refrescar', this.refreshUserHandler);
    this.refreshUser();
  }

  ngOnDestroy(): void {
    window.removeEventListener('refrescar', this.refreshUserHandler);
  }

  refreshUser(): void {

    if (this.localStorageService.isSessionExpired()) {
      this.localStorageService.clearLogin();
    }

    const user = this.localStorageService.getUser();
    this.userName = user?.fullName ?? '';
    this.isLoged = this.localStorageService.isLoggedIn();
    this.profileImageUrl = this.localStorageService.getProfileImage() || 'assets/exterior.jpg';
    this.canAccessControlPanel = this.localStorageService.isOwner() || this.localStorageService.isAdmin();

    if (!this.isLoged) {
      this.userName = '';
      this.profileImageUrl = 'assets/exterior.jpg';
      this.canAccessControlPanel = false;
    }
  }

  logout(): void {
    this.localStorageService.clearLogin();
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
