import { Injectable } from '@angular/core';
import { BillingData } from './billing-data/models/billingData';
import { RoomDto } from 'src/models/bookingDto';
import { CreditCard } from './checkout/Models/creditCard';

@Injectable({
  providedIn: 'root'
})
export class LocalDataBookingService {

  constructor() { }
  billingData: BillingData = { customerName: '', address: '', city: '', state: '', zipCode: '', country: '', phoneNumber: '', email: '' };
  bookingDto: RoomDto = { IdRoom: 0, CheckInTimeId: 0, Date: '', userGuid:'' };
  creditCard: CreditCard = { cardNumber: '', expirationDate: '', cvv: 0, fullName: '' };

  setBillingData(billingData: BillingData): void {
    this.billingData = billingData;
  }
  setBookingDto(bookingDto: RoomDto): void {
    this.bookingDto = bookingDto;
  }
  setCreditCard(creditCard: CreditCard): void {
    this.creditCard = creditCard;
  }
  getBillingData(): BillingData | undefined {
    return this.billingData;
  }
  getBookingDto(): RoomDto | undefined {
    return this.bookingDto;
  }
  getCreditCard(): CreditCard | undefined {
    return this.creditCard;
  }

}
