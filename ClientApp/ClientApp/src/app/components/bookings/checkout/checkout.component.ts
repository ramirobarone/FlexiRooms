import { AfterViewInit, Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { RoomDto } from '../../../../models/bookingDto';
import { CheckoutService, MercadoPagoPaymentData } from './service/checkout.service';
import { LocalDataBookingService } from '../local-data-booking.service';

declare global {
  interface Window {
    MercadoPago: new (publicKey: string, options?: { locale: string }) => { bricks: () => { create: (brick: string, containerId: string, settings: object) => Promise<{ unmount: () => void }> } };
  }
}

@Component({
    selector: 'app-checkout',
    templateUrl: './checkout.component.html',
    styleUrls: ['./checkout.component.css'],
    standalone: false
})
export class CheckoutComponent implements AfterViewInit, OnDestroy {
  isLoading = true;
  errorMessage = '';
  successMessage = '';
  private brickController?: { unmount: () => void };

  constructor(
    private checkoutService: CheckoutService,
    private localDataBooking: LocalDataBookingService,
    private router: Router
  ) { }

  ngAfterViewInit(): void {
    const booking = this.localDataBooking.getBookingDto();
    if (!booking || booking.IdRoom <= 0) {
      this.showError('No hay una reserva seleccionada para pagar.');
      return;
    }

    this.checkoutService.getPaymentConfiguration(booking.IdRoom).subscribe({
      next: configuration => this.loadSdk().then(() => this.renderBrick(configuration.publicKey, configuration.amount, booking)),
      error: () => this.showError('No se pudo iniciar el medio de pago.')
    });
  }

  ngOnDestroy(): void {
    this.brickController?.unmount();
  }

  processPayment(payment: MercadoPagoPaymentData): Promise<void> {
    const booking = this.localDataBooking.getBookingDto();
    if (!booking) {
      return Promise.reject(new Error('No hay una reserva seleccionada.'));
    }

    this.errorMessage = '';
    return new Promise((resolve, reject) => {
      this.checkoutService.processPayment(booking, payment).subscribe({
        next: response => {
          this.successMessage = 'El pago fue aprobado y la reserva fue confirmada.';
          const navigation = response.status === 'approved'
            ? this.router.navigate(['/MisReservas'])
            : Promise.resolve(false);
          navigation.then(() => resolve()).catch(reject);
        },
        error: error => {
          this.errorMessage = error.error?.message || 'No se pudo procesar el pago. Verifique los datos e intente nuevamente.';
          reject(error);
        }
      });
    });
  }

  private renderBrick(publicKey: string, amount: number, booking: RoomDto): void {
    const mercadoPago = new window.MercadoPago(publicKey, { locale: 'es-AR' });
    mercadoPago.bricks().create('cardPayment', 'cardPaymentBrick_container', {
      initialization: {
        amount,
        payer: { email: this.localDataBooking.getBillingData()?.email || '' }
      },
      callbacks: {
        onReady: () => this.isLoading = false,
        onSubmit: (formData: MercadoPagoPaymentData) => this.processPayment(formData),
        onError: () => this.showError('El formulario de pago no pudo cargarse.')
      }
    }).then(controller => this.brickController = controller)
      .catch(() => this.showError('El formulario de pago no pudo cargarse.'));
  }

  private loadSdk(): Promise<void> {
    if (window.MercadoPago) {
      return Promise.resolve();
    }

    return new Promise((resolve, reject) => {
      const script = document.createElement('script');
      script.src = 'https://sdk.mercadopago.com/js/v2';
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('No se pudo cargar Mercado Pago.'));
      document.head.appendChild(script);
    });
  }

  private showError(message: string): void {
    this.isLoading = false;
    this.errorMessage = message;
  }
}
