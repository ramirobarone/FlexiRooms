import { Component } from '@angular/core';
import { BillingData } from '../billing-data/models/billingData';
import { RoomDto } from '../../../../models/bookingDto';
import { CheckOut } from './Models/checkout';
import { CreditCard } from '../checkout/Models/creditCard';

@Component({
  selector: 'app-steps-checkout',
  templateUrl: './steps-checkout.component.html',
  styleUrls: ['./steps-checkout.component.css']
})


export class StepsCheckoutComponent {


  constructor() { }

  isBillingDataEnabled: boolean = false;
  paymentDataEnabled: boolean = false;
  

  checkRoom(event: boolean): void {
    console.log('checkRoom', event);
    this.isBillingDataEnabled = event;
  }
  checkBillingData(event: boolean): void {
    console.log('checkBillingData', event);
    this.paymentDataEnabled = event;
  }

  completeCheckout(): void {

    // this.serviceBooking.createBooking(this._booking).subscribe(res => {
    // });

  }



}
