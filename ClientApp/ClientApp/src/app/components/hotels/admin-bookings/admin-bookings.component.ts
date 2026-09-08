import { Component, OnInit } from '@angular/core';
import { AdminBookingsService } from '../../../admin/services/admin-bookings.service';
import { AdminBooking } from '../../../admin/models/admin-booking.model';

@Component({
  selector: 'app-admin-bookings',
  templateUrl: './admin-bookings.component.html',
  styleUrls: ['./admin-bookings.component.css']
})
export class AdminBookingsComponent implements OnInit {
  bookings: AdminBooking[] = [];
  selectedBooking?: AdminBooking;

  constructor(private adminBookingsService: AdminBookingsService) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.adminBookingsService.getAdminBookings().subscribe({
      next: (response) => this.bookings = response ?? [],
      error: () => this.bookings = []
    });
  }

  showDetail(id: number): void {
    this.adminBookingsService.getBookingDetail(id).subscribe({
      next: (response) => this.selectedBooking = response
    });
  }

  deleteBooking(id: number): void {
    this.adminBookingsService.deleteBooking(id).subscribe({
      next: () => {
        this.selectedBooking = undefined;
        this.loadBookings();
      }
    });
  }
}
