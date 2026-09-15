import { of } from 'rxjs';
import { Router } from '@angular/router';
import { CheckoutComponent } from './checkout.component';
import { CheckoutService, MercadoPagoPaymentData } from './service/checkout.service';
import { LocalDataBookingService } from '../local-data-booking.service';

describe('CheckoutComponent', () => {
  const payment: MercadoPagoPaymentData = {
    token: 'payment-token',
    payment_method_id: 'visa',
    installments: 1,
    payer: { email: 'guest@example.com' }
  };

  function createComponent(booking?: { IdRoom: number; Date: string; CheckInTimeId: number; userGuid: string }): {
    component: CheckoutComponent;
    checkoutService: jasmine.SpyObj<CheckoutService>;
    router: jasmine.SpyObj<Router>;
  } {
    const checkoutService = jasmine.createSpyObj<CheckoutService>('CheckoutService', ['processPayment', 'getPaymentConfiguration']);
    const localDataBooking = jasmine.createSpyObj<LocalDataBookingService>('LocalDataBookingService', ['getBookingDto', 'getBillingData']);
    const router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    localDataBooking.getBookingDto.and.returnValue(booking || undefined);
    router.navigate.and.resolveTo(true);

    return { component: new CheckoutComponent(checkoutService, localDataBooking, router), checkoutService, router };
  }

  it('sends the Brick token with the selected booking', async () => {
    const { component, checkoutService, router } = createComponent({ IdRoom: 4, Date: '2026-09-14', CheckInTimeId: 2, userGuid: 'user-guid' });
    checkoutService.processPayment.and.returnValue(of({ paymentId: 123, status: 'approved' }));

    await component.processPayment(payment);

    expect(checkoutService.processPayment).toHaveBeenCalledWith(jasmine.objectContaining({ IdRoom: 4 }), payment);
    expect(component.successMessage).toBe('El pago fue aprobado y la reserva fue confirmada.');
    expect(router.navigate).toHaveBeenCalledWith(['/MisReservas']);
  });

  it('rejects payment submission when there is no selected booking', async () => {
    const { component, checkoutService } = createComponent(undefined);

    await expectAsync(component.processPayment(payment)).toBeRejectedWithError('No hay una reserva seleccionada.');

    expect(checkoutService.processPayment).not.toHaveBeenCalled();
  });
});