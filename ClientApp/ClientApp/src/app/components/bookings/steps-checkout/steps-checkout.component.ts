import { Component } from '@angular/core';
import { BillingData } from '../billing-data/models/billingData';
import { RoomDto } from '../../../../models/bookingDto';
import { CheckOut } from './Models/checkout';
import { CreditCard } from '../checkout/Models/creditCard';
import { Router } from '@angular/router';

@Component({
    selector: 'app-steps-checkout',
    templateUrl: './steps-checkout.component.html',
    styleUrls: ['./steps-checkout.component.css'],
    standalone: false
})


export class StepsCheckoutComponent {


  constructor(private router: Router) { }

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

  backToHotelSearch(): void {
    this.router.navigate(['/']);
  }

  completeCheckout(): void {

    // this.serviceBooking.createBooking(this._booking).subscribe(res => {
    // });

  }



}
