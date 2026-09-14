import { Router } from '@angular/router';

import { StepsCheckoutComponent } from './steps-checkout.component';

describe('StepsCheckoutComponent', () => {
  it('should navigate back to the hotel search', () => {
    const router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    const component = new StepsCheckoutComponent(router);

    component.backToHotelSearch();

    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});
