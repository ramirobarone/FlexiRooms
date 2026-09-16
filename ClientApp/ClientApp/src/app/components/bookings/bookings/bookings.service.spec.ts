import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { EnvironmentsService } from '../../../ServicesShared/environments.service';
import { BookingsService, UserBooking } from './bookings.service';
import { provideHttpClient, withInterceptorsFromDi, withXhr } from '@angular/common/http';

describe('BookingsService', () => {
  let service: BookingsService;
  let httpController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
    imports: [],
    providers: [
        BookingsService,
        { provide: EnvironmentsService, useValue: { getUrlBase: () => 'https://hotelis.test/api/' } },
        provideHttpClient(withXhr(), withInterceptorsFromDi()),
        provideHttpClientTesting()
    ]
});

    service = TestBed.inject(BookingsService);
    httpController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpController.verify());

  it('requests the bookings for the authenticated user', () => {
    const response: UserBooking[] = [{
      id: 12,
      roomId: 8,
      startDate: '2026-09-15T00:00:00',
      endDate: '2026-09-15T00:00:00',
      startTime: '10:00',
      endTime: '14:00',
      paymentStatus: 'approved'
    }];

    service.getMyBookings().subscribe(bookings => expect(bookings).toEqual(response));

    const request = httpController.expectOne('https://hotelis.test/api/Bookings/GetBookingsByUserGuid');
    expect(request.request.method).toBe('GET');
    request.flush(response);
  });
});