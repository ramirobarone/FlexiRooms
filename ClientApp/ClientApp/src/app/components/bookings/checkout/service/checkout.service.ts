import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RoomDto } from 'src/models/bookingDto';
import { CheckOut } from '../../steps-checkout/Models/checkout';
import { CheckOutResponse } from '../../Models/CheckoutResponse';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  constructor(private http: HttpClient) { }

  baseurl: string = 'https://localhost:7291/api/';

  createBooking(booking: CheckOut): Observable<CheckOutResponse> {
    return this.http.post<CheckOutResponse>(this.baseurl + 'Bookings/CreateBooking', booking);
  }
  getBookings(): Observable<RoomDto[]> {
    return this.http.get<RoomDto[]>(this.baseurl + 'Bookings/GetBookings');
  }
}
