import { Injectable } from '@angular/core';
import { UserLoginDto } from '../components/Account/login/Models/userDto';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private readonly userKey = 'flexirooms.user';
  private readonly tokenKey = 'token';
  private readonly fullNameKey = 'fullName';
  private readonly userGuidKey = 'userGuid';
  private readonly profileImageKey = 'profileImage';

  saveLogin(user: UserLoginDto): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
    localStorage.setItem(this.tokenKey, user.token);
    localStorage.setItem(this.fullNameKey, user.fullName);
    localStorage.setItem(this.userGuidKey, user.userGuid);
  }

  getUser(): UserLoginDto | null {
    const storedUser = localStorage.getItem(this.userKey);
    if (storedUser) {
      try {
        return JSON.parse(storedUser) as UserLoginDto;
      } catch {
        this.clearLogin();
        return null;
      }
    }

    const token = localStorage.getItem(this.tokenKey);
    const fullName = localStorage.getItem(this.fullNameKey);
    const userGuid = localStorage.getItem(this.userGuidKey);
    if (!token || !fullName || !userGuid) {
      return null;
    }

    const user: UserLoginDto = { token, fullName, rolId: 0, role: '', userGuid };
    localStorage.setItem(this.userKey, JSON.stringify(user));
    return user;
  }

  getToken(): string | null {
    return this.getUser()?.token ?? localStorage.getItem(this.tokenKey);
  }

  updateUser(user: UserLoginDto): void {
    this.saveLogin(user);
  }

  setProfileImage(url: string): void {
    localStorage.setItem(this.profileImageKey, url);
  }

  getProfileImage(): string | null {
    return localStorage.getItem(this.profileImageKey);
  }

  removeProfileImage(): void {
    localStorage.removeItem(this.profileImageKey);
  }

  clearLogin(): void {
    localStorage.removeItem(this.userKey);
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.fullNameKey);
    localStorage.removeItem(this.userGuidKey);
    this.removeProfileImage();
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  getRoles(): string[] {
    const user = this.getUser();
    const tokenRoles = user ? this.getTokenRoles(user.token) : [];
    const storedRole = user?.role ? [user.role] : [];

    return [...new Set([...storedRole, ...tokenRoles].map(role => role.toLowerCase()))];
  }

  isOwner(): boolean {
    return this.getRoles().includes('owner');
  }

  isAdmin(): boolean {
    return this.getRoles().includes('admin');
  }

  isUser(): boolean {
    return this.getRoles().includes('user');
  }

  private getTokenRoles(token: string): string[] {
    try {
      const tokenParts = token.split('.');
      if (tokenParts.length < 2) {
        return [];
      }

      const payload = JSON.parse(this.decodeBase64Url(tokenParts[1])) as Record<string, unknown>;
      const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        ?? payload['role']
        ?? payload['roles'];
      const roles = Array.isArray(roleClaim) ? roleClaim : [roleClaim];

      return roles.filter((role): role is string => typeof role === 'string');
    } catch {
      return [];
    }
  }

  private decodeBase64Url(value: string): string {
    const normalizedValue = value.replace(/-/g, '+').replace(/_/g, '/');
    return atob(normalizedValue.padEnd(normalizedValue.length + (4 - normalizedValue.length % 4) % 4, '='));
  }
}
