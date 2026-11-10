import { Component, OnInit, ChangeDetectionStrategy, inject } from '@angular/core';
import { ConfirmService } from '../../Shared/confirm-dialog/confirm.service';
import { AdminBookingsService } from '../../../admin/services/admin-bookings.service';
import { AdminBooking } from '../../../admin/models/admin-booking.model';

@Component({
    selector: 'app-admin-bookings',
    templateUrl: './admin-bookings.component.html',
    styleUrls: ['./admin-bookings.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AdminBookingsComponent implements OnInit {
  bookings: AdminBooking[] = [];
  selectedBooking?: AdminBooking;
  private readonly confirmService = inject(ConfirmService);

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

  async deleteBooking(id: number): Promise<void> {
    // Destructive and irreversible: confirm before deleting.
    const confirmed = await this.confirmService.confirm({
      title: `¿Eliminar la reserva #${id}?`,
      message: 'Esta acción no se puede deshacer.',
      confirmText: 'Eliminar',
      destructive: true
    });
    if (!confirmed) {
      return;
    }

    this.adminBookingsService.deleteBooking(id).subscribe({
      next: () => {
        this.selectedBooking = undefined;
        this.loadBookings();
      }
    });
  }
}
