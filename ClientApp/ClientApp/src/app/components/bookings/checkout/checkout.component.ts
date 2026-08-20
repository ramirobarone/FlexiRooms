import { Component, Input } from '@angular/core';
import { CreditCard } from './Models/creditCard';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CheckOut } from '../steps-checkout/Models/checkout';
import { RoomDto } from '../../../../models/bookingDto';
import { BillingData } from '../billing-data/models/billingData';
import { CheckoutService } from './service/checkout.service';
import { LocalDataBookingService } from '../local-data-booking.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent {

  @Input() description = '';
  @Input() idRoom = 0;
  @Input() checkInTimeId = 0;
  @Input() date = '';


  constructor(private serviceCkeckOut: CheckoutService, private localDataBooking: LocalDataBookingService) {
    this._cardForm = new FormGroup({
      'cardNumber': new FormControl('', Validators.required),
      'cvv': new FormControl('', Validators.required),
      'fullName': new FormControl('', Validators.required),
      'expirationDate': new FormControl('', Validators.required)
    })
  }

  _cardForm: FormGroup;
  creditCard: CreditCard = { cardNumber: '', expirationDate: '', cvv: 0, fullName: '' };
  disabledButtonPay = false;


  Checkout(): void {

    if (this._cardForm.invalid)
      return; // Falta Alerta

    let completeCheckout: CheckOut = {
      roomDto: this.localDataBooking.getBookingDto(),
      billingData: this.localDataBooking.getBillingData(),
      creditCard: this.creditCard
    }

    console.log('completeCheckout', completeCheckout);

    this.serviceCkeckOut.createBooking(completeCheckout).subscribe(result => {
      console.log('checkout',result);
    });

    //console.log(completeCheckout);
  }
}
