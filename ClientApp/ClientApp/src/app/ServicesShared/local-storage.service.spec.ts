import { TestBed } from '@angular/core/testing';

import { LocalStorageService } from './local-storage.service';

describe('LocalStorageService', () => {
  let service: LocalStorageService;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({});
    service = TestBed.inject(LocalStorageService);
  });

  afterEach(() => {
    localStorage.clear();
  });

  function tokenWithExpiration(expiration: number): string {
    const payload = btoa(JSON.stringify({ exp: expiration }))
      .replace(/=/g, '')
      .replace(/\+/g, '-')
      .replace(/\//g, '_');

    return `header.${payload}.signature`;
  }

  it('should report an active session for a token that has not expired', () => {
    const expiration = Math.floor(Date.now() / 1000) + 60;
    localStorage.setItem('token', tokenWithExpiration(expiration));

    expect(service.isSessionExpired()).toBeFalse();
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('should report an expired session when the token expiration has passed', () => {
    const expiration = Math.floor(Date.now() / 1000) - 60;
    localStorage.setItem('token', tokenWithExpiration(expiration));

    expect(service.isSessionExpired()).toBeTrue();
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('should report an expired session for an invalid or missing token', () => {
    expect(service.isSessionExpired()).toBeTrue();

    localStorage.setItem('token', 'invalid-token');

    expect(service.isSessionExpired()).toBeTrue();
  });
});
