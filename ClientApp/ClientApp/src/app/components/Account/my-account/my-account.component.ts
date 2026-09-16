import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { AccountService } from '../account.service';
import { UpdateUserInformationDto, UserInformationDto } from '../Models/userInformationDto';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';

@Component({
    selector: 'app-my-account',
    templateUrl: './my-account.component.html',
    styleUrls: ['./my-account.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MyAccountComponent implements OnInit {
  user: UserInformationDto | null = null;
  isLoading = true;
  isSaving = false;
  isUploading = false;
  message = '';
  errorMessage = '';

  constructor(private readonly accountService: AccountService,
              private readonly localStorageService: LocalStorageService) { }

  ngOnInit(): void {
    this.loadInformation();
  }

  loadInformation(): void {
    this.isLoading = true;
    this.accountService.getInformation().subscribe({
      next: user => {
        this.user = user;
        this.storeProfileImage(user.profileImageUrl);
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'No se pudo cargar la información de la cuenta.';
        this.isLoading = false;
      }
    });
  }

  saveInformation(): void {
    if (!this.user) {
      return;
    }

    const changes: UpdateUserInformationDto = {
      name: this.user.name,
      secondName: this.user.secondName,
      lastName: this.user.lastName,
      identityNumber: this.user.identityNumber,
      codeArea: this.user.codeArea,
      phoneNumber: this.user.phoneNumber
    };

    this.isSaving = true;
    this.message = '';
    this.errorMessage = '';
    this.accountService.updateInformation(changes).subscribe({
      next: user => {
        this.user = user;
        const storedUser = this.localStorageService.getUser();
        if (storedUser) {
          this.localStorageService.updateUser({
            ...storedUser,
            fullName: `${user.name ?? ''} ${user.lastName ?? ''}`.trim() || user.email || ''
          });
        }
        this.storeProfileImage(user.profileImageUrl);
        dispatchEvent(new Event('refrescar'));
        this.message = 'La información fue actualizada.';
        this.isSaving = false;
      },
      error: () => {
        this.errorMessage = 'No se pudo actualizar la información.';
        this.isSaving = false;
      }
    });
  }

  uploadProfileImage(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file || !this.user) {
      return;
    }

    this.isUploading = true;
    this.message = '';
    this.errorMessage = '';
    this.accountService.updateProfileImage(file).subscribe({
      next: () => {
        this.loadInformation();
        this.isUploading = false;
        this.message = 'La foto de perfil fue actualizada.';
        input.value = '';
      },
      error: () => {
        this.errorMessage = 'La foto debe ser JPG, PNG o WebP y pesar menos de 5 MB.';
        this.isUploading = false;
        input.value = '';
      }
    });
  }

  private storeProfileImage(imageUrl: string | null): void {
    if (imageUrl) {
      this.localStorageService.setProfileImage(imageUrl);
    } else {
      this.localStorageService.removeProfileImage();
    }
  }
}
