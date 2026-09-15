import { ActivatedRoute, Router } from '@angular/router';
import { CardRoomComponent } from './card-room.component';
import { AvialableRoomsService } from 'src/services/AvialableRooms/avialable-rooms.service';
import { LocalDataBookingService } from '../local-data-booking.service';

describe('CardRoomComponent', () => {
  function createComponent(): CardRoomComponent {
    const roomService = jasmine.createSpyObj<AvialableRoomsService>('AvialableRoomsService', ['getRoom']);
    const route = { snapshot: { paramMap: { get: () => '1' } } } as unknown as ActivatedRoute;
    const localDataBooking = jasmine.createSpyObj<LocalDataBookingService>('LocalDataBookingService', ['setBookingDto']);
    const router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    return new CardRoomComponent(roomService, route, localDataBooking, router);
  }

  it('shows the departure time based on the room duration', () => {
    const component = createComponent();
    component.cost.hour = 3;
    component.times = [{ id: 1, inTime: '10:30' }];
    component.timeSelected = '1';

    component.SelectTime();

    expect(component.checkoutTime).toBe('13:30');
  });

  it('indicates when the departure time is on the following day', () => {
    const component = createComponent();

    expect(component.calculateCheckoutTime('22:30', 3)).toBe('01:30 del día siguiente');
  });
});