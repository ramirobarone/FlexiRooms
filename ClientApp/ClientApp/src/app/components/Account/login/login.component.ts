import { Component, ElementRef, EventEmitter, Output, Renderer2, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { UserDto, UserLoginDto } from './Models/userDto';
import { AccountService } from './Service/account.service';
import { Router } from '@angular/router';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';
import { finalize } from 'rxjs';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class LoginComponent {
  @ViewChild('fullNameLabel') fullNameChild!: ElementRef;

  constructor(private serviceAuthenticat: AccountService,
              private router: Router,
              private renderer: Renderer2,
              private localStorageService: LocalStorageService) {
  }
  userDto: UserDto = { email: 'ramiro_barone@hotmail.com', password: '!Jazmin1811' };
  isLoading = false;
  errorMessage = '';

  login(): void {
    if (this.isLoading) {
      return;
    }

    this.errorMessage = '';
    this.isLoading = true;
    this.serviceAuthenticat.authenticat(this.userDto).pipe(finalize(() => this.isLoading = false)).subscribe({
      next: (res) => {
        this.localStorageService.saveLogin(res);
        
        this.changeFullName(res.fullName);
        this.router.navigate(['/']);
        dispatchEvent(new Event('refrescar'));
      },
      error: (err) => {
        console.error('Error authenticating', err);
        this.errorMessage = 'No se ingresó correctamente la cuenta o la contraseña.';
      }
    });
  }
  changeFullName(name: string): void {
 
  }
}
