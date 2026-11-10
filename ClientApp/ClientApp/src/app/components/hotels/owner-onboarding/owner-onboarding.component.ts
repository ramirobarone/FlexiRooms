import { Component, OnInit, ChangeDetectionStrategy, inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Hotel } from '../../../../models/hotel';
import { Province } from '../../../../models/Province';
import { City } from '../../../../models/City';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { GeographyService } from '../../../../services/GeographyService/geography.service';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';

/**
 * "Sumá tu hotel": self-service sign-up for hotel owners.
 *  1. Create an account / log in (returns here via ?returnUrl)
 *  2. Load the hotel → it becomes the owner's hotel (role Owner), Pending review
 *  3. Lands in the panel, where the owner connects Mercado Pago while the hotel is reviewed
 */
@Component({
  selector: 'app-owner-onboarding',
  templateUrl: './owner-onboarding.component.html',
  styleUrls: ['./owner-onboarding.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: false
})
export class OwnerOnboardingComponent implements OnInit {
  readonly returnUrl = '/sumar-hotel';

  isLoggedIn = false;
  provinces: Province[] = [];
  cities: City[] = [];
  isSubmitting = false;
  submitted = false;
  errorMessage = '';

  hotel: Hotel = {
    id: 0,
    name: '',
    description: '',
    metaDescription: '',
    addressHotel: { id: 0, idHotel: 0, street: '', number: '', postalCode: '', latitud: '', longitud: '', idCity: 0, idProvince: 0 },
    email: '',
    codeArea: 0,
    phoneNumber: 0,
    postalCode: '',
    latitud: '',
    longitud: '',
    pictures: []
  };

  private readonly hotelService = inject(HotelService);
  private readonly geographyService = inject(GeographyService);
  private readonly localStorage = inject(LocalStorageService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.isLoggedIn = !!this.localStorage.getUser()?.token;
    if (this.isLoggedIn) {
      this.geographyService.getProvinces().subscribe({
        next: provinces => this.provinces = provinces ?? [],
        error: () => this.provinces = []
      });
    }
  }

  onProvinceChange(): void {
    this.hotel.addressHotel.idCity = 0;
    this.cities = [];
    if (this.hotel.addressHotel.idProvince) {
      this.geographyService.getCitiesByProvince(this.hotel.addressHotel.idProvince).subscribe({
        next: cities => this.cities = cities ?? [],
        error: () => this.cities = []
      });
    }
  }

  submit(form: NgForm): void {
    this.submitted = true;
    if (form.invalid || !this.hotel.addressHotel.idProvince || !this.hotel.addressHotel.idCity) {
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    // Search matches on the meta description: default it to something findable.
    const city = this.cities.find(c => c.id === this.hotel.addressHotel.idCity)?.name ?? '';
    const payload: Hotel = {
      ...this.hotel,
      metaDescription: this.hotel.metaDescription?.trim() || [this.hotel.name, city, this.hotel.addressHotel.street].filter(Boolean).join(' ')
    };

    this.hotelService.createHotel(payload).subscribe({
      next: response => {
        // The account is now Owner: store the fresh token so the panel opens right away.
        if (response?.login) {
          this.localStorage.saveLogin(response.login);
          dispatchEvent(new Event('refrescar'));
        }
        this.router.navigate(['/admin'], { queryParams: { nuevo: response?.hotelId } });
      },
      error: error => {
        this.isSubmitting = false;
        this.errorMessage = error?.status === 401
          ? 'Tu sesión expiró. Ingresá de nuevo para continuar.'
          : error?.error?.message ?? 'No pudimos registrar el hotel. Revisá los datos e intentá de nuevo.';
      }
    });
  }
}
