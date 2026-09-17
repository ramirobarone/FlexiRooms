import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../../ServicesShared/environments.service';

export interface UserBooking {
  id: number;
  roomId: number;
  startDate: string;
  endDate: string;
  startTime: string;
  endTime: string;
  paymentStatus: string;
  terminosYCondiciones?: string;
  instruccionesDeUso?: string;
  hotelWhatsAppNumber?: string;
}

@Injectable({ providedIn: 'root' })
export class BookingsService {
  private readonly bookingsUrl: string;

  constructor(private http: HttpClient, environmentsService: EnvironmentsService) {
    this.bookingsUrl = `${environmentsService.getUrlBase()}Bookings/GetBookingsByUserGuid`;
  }

  getMyBookings(): Observable<UserBooking[]> {
    return this.http.get<UserBooking[]>(this.bookingsUrl);
  }
}