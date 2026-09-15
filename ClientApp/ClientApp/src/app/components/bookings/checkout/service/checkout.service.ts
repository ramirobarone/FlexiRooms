import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RoomDto } from 'src/models/bookingDto';
import { CheckOut } from '../../steps-checkout/Models/checkout';
import { CheckOutResponse } from '../../Models/CheckoutResponse';
import { EnvironmentsService } from 'src/app/ServicesShared/environments.service';

export interface PaymentConfiguration {
  publicKey: string;
  amount: number;
}

export interface MercadoPagoPaymentData {
  token: string;
  payment_method_id: string;
  issuer_id?: string;
  installments: number;
  payer: {
    email: string;
    identification?: { type: string; number: string; };
  };
}

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  constructor(private http: HttpClient, private environmentService: EnvironmentsService) { }

  baseurl: string = this.environmentService.getUrlBase();

  createBooking(booking: CheckOut): Observable<CheckOutResponse> {
    return this.http.post<CheckOutResponse>(this.baseurl + 'Bookings/CreateBooking', booking);
  }
  getBookings(): Observable<RoomDto[]> {
    return this.http.get<RoomDto[]>(this.baseurl + 'Bookings/GetBookings');
  }

  getPaymentConfiguration(roomId: number): Observable<PaymentConfiguration> {
    return this.http.get<PaymentConfiguration>(this.baseurl + 'Payments/public-key', { params: { roomId } });
  }

  processPayment(roomDto: RoomDto, payment: MercadoPagoPaymentData): Observable<{ paymentId: number; status: string; }> {
    return this.http.post<{ paymentId: number; status: string; }>(this.baseurl + 'Payments', {
      roomDto,
      payment: {
        token: payment.token,
        paymentMethodId: payment.payment_method_id,
        issuerId: payment.issuer_id,
        installments: payment.installments,
        payerEmail: payment.payer.email,
        identificationType: payment.payer.identification?.type,
        identificationNumber: payment.payer.identification?.number
      }
    });
  }
}
