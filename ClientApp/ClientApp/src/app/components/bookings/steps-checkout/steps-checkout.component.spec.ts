import { Router } from '@angular/router';

import { StepsCheckoutComponent } from './steps-checkout.component';

describe('StepsCheckoutComponent', () => {
  function createComponent(): StepsCheckoutComponent {
    const router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    return new StepsCheckoutComponent(router);
  }

  it('should navigate back to the hotel search', () => {
    const router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    const component = new StepsCheckoutComponent(router);

    component.backToHotelSearch();

    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('reveals billing after completing room selection', () => {
    const component = createComponent();

    component.checkRoom(true);

    expect(component.isBillingDataEnabled).toBeTrue();
  });

  it('reveals payment after completing billing data', () => {
    const component = createComponent();

    component.checkBillingData(true);

    expect(component.paymentDataEnabled).toBeTrue();
  });
});
