import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../app/ServicesShared/environments.service';

export interface HotelPaymentAccountStatus {
  hotelId: number;
  connected: boolean;
  mercadoPagoUserId: number | null;
  liveMode: boolean;
  connectedAtUtc: string | null;
}

export interface MyPaymentAccounts {
  /** False when the server has no Marketplace credentials: connecting is impossible. */
  marketplaceConfigured: boolean;
  hotels: HotelPaymentAccountStatus[];
}

/** Mercado Pago Marketplace: connect each hotel's own account so payments go to the hotel. */
@Injectable({ providedIn: 'root' })
export class MercadoPagoAccountService {
  private readonly baseurl: string;

  constructor(private http: HttpClient, environmentsService: EnvironmentsService) {
    this.baseurl = `${environmentsService.getUrlBase()}MercadoPagoAccounts`;
  }

  getMyAccounts(): Observable<MyPaymentAccounts> {
    return this.http.get<MyPaymentAccounts>(this.baseurl);
  }

  /** Returns the Mercado Pago authorization URL the browser must navigate to. */
  connect(hotelId: number): Observable<{ authorizationUrl: string }> {
    return this.http.post<{ authorizationUrl: string }>(`${this.baseurl}/${hotelId}/connect`, {});
  }

  disconnect(hotelId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseurl}/${hotelId}`);
  }
}
