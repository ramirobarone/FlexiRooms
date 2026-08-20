import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BillingData } from './models/billingData';
import { LocalDataBookingService } from '../local-data-booking.service';

@Component({
  selector: 'app-billing-data',
  templateUrl: './billing-data.component.html',
  styleUrls: ['./billing-data.component.css']
})
export class BillingDataComponent {

  billingData: BillingData = { customerName: '', address: '', city: '', state: '', zipCode: '', country: '', phoneNumber: '', email: '' };
  billingForm: FormGroup;

  constructor(private localdataBooking: LocalDataBookingService) {

    const forms = document.querySelectorAll('.needs-validation')

    this.billingForm = new FormGroup({
      'name': new FormControl('Ramiro', Validators.required),
      'address': new FormControl('Publica 15, 4321', Validators.required),
      'city': new FormControl('Cordoba', Validators.required),
      'state': new FormControl('Cordoba', Validators.required),
      'zip': new FormControl('5010', [Validators.required, Validators.pattern('^[0-9]{3}$')]),
      'country': new FormControl('Argentina', Validators.required),
      'email': new FormControl('ramiro_barone@hotmail.com', [Validators.required, Validators.email]),
      'phone': new FormControl('3517572518', [Validators.required])
    });
  }

  @Output() checkOut: EventEmitter<boolean> = new EventEmitter();

  onSubmit(): void {
    console.log('billing form', this.billingForm.valid)
    if (this.billingForm.valid) {
      this.checkOut.emit(this.billingForm.valid);
      this.localdataBooking.setBillingData(this.billingData);
      // localStorage.setItem('billingData', JSON.stringify(this.billingData));
    }

  }
}
