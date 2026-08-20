import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../ServicesShared/environments.service';
import { AdminBooking } from '../models/admin-booking.model';
import { RoomDto } from '../../../models/bookingDto';

@Injectable({
  providedIn: 'root'
})
export class AdminBookingsService {
  private readonly baseUrl: string;

  constructor(private http: HttpClient, environmentsService: EnvironmentsService) {
    this.baseUrl = `${environmentsService.getUrlBase()}bookings/`;
  }

  getAdminBookings(): Observable<AdminBooking[]> {
    return this.http.get<AdminBooking[]>(this.baseUrl + 'getadminbookings');
  }

  getBookingDetail(id: number): Observable<AdminBooking> {
    return this.http.get<AdminBooking>(this.baseUrl + 'getbookingdetail?id=' + id);
  }

  createBooking(booking: RoomDto): Observable<AdminBooking> {
    return this.http.post<AdminBooking>(this.baseUrl + 'createbooking', booking);
  }

  deleteBooking(id: number): Observable<unknown> {
    return this.http.delete(this.baseUrl + 'deletebooking?id=' + id);
  }
}
