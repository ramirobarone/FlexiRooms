import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { BookingsComponent } from './bookings.component';
import { BookingsService, UserBooking } from './bookings.service';

describe('BookingsComponent', () => {
  let component: BookingsComponent;
  let fixture: ComponentFixture<BookingsComponent>;
  let bookingsService: jasmine.SpyObj<BookingsService>;

  const bookings: UserBooking[] = Array.from({ length: 6 }, (_, index) => ({
    id: index + 1,
    roomId: 100 + index,
    startDate: '2026-09-15T00:00:00',
    endDate: '2026-09-15T00:00:00',
    startTime: '10:00',
    endTime: '12:00',
    paymentStatus: 'approved'
  }));

  beforeEach(async () => {
    bookingsService = jasmine.createSpyObj<BookingsService>('BookingsService', ['getMyBookings']);
    bookingsService.getMyBookings.and.returnValue(of(bookings));

    await TestBed.configureTestingModule({
      declarations: [ BookingsComponent ],
      providers: [
        { provide: BookingsService, useValue: bookingsService }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BookingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('loads and paginates the user bookings', () => {
    expect(bookingsService.getMyBookings).toHaveBeenCalled();
    expect(component.pagedBookings.length).toBe(5);
    expect(component.totalPages).toBe(2);

    component.goToPage(2);

    expect(component.pagedBookings.map(booking => booking.id)).toEqual([6]);
  });

  it('shows and closes the hardcoded door code', () => {
    component.showDoorCode(bookings[0]);

    expect(component.selectedBooking).toEqual(bookings[0]);
    expect(component.doorCode).toBe('4826');

    component.closeDoorCode();
    expect(component.selectedBooking).toBeUndefined();
  });

  it('reports an error when bookings cannot be loaded', () => {
    bookingsService.getMyBookings.and.returnValue(throwError(() => new Error('network error')));

    component.loadBookings();

    expect(component.hasError).toBeTrue();
    expect(component.isLoading).toBeFalse();
    expect(component.bookings).toEqual([]);
  });
});
