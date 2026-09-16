import { Component, Inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { BookingsService, UserBooking } from './bookings.service';

@Component({
    selector: 'app-bookings',
    templateUrl: './bookings.component.html',
    styleUrls: ['./bookings.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class BookingsComponent implements OnInit {
  readonly pageSize = 5;
  readonly doorCode = '4826';
  bookings: UserBooking[] = [];
  currentPage = 1;
  isLoading = true;
  hasError = false;
  selectedBooking?: UserBooking;
  instructionsBooking?: UserBooking;
  termsBooking?: UserBooking;

  constructor(@Inject(BookingsService) private bookingsService: BookingsService) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  get pagedBookings(): UserBooking[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    return this.bookings.slice(startIndex, startIndex + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.bookings.length / this.pageSize);
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, index) => index + 1);
  }

  loadBookings(): void {
    this.isLoading = true;
    this.hasError = false;
    this.bookingsService.getMyBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings ?? [];
        this.currentPage = Math.min(this.currentPage, Math.max(this.totalPages, 1));
        this.isLoading = false;
      },
      error: () => {
        this.bookings = [];
        this.hasError = true;
        this.isLoading = false;
      }
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  showDoorCode(booking: UserBooking): void {
    this.selectedBooking = booking;
  }

  closeDoorCode(): void {
    this.selectedBooking = undefined;
  }

  showInstructions(booking: UserBooking): void {
    this.instructionsBooking = booking;
  }

  closeInstructions(): void {
    this.instructionsBooking = undefined;
  }

  showTerms(booking: UserBooking): void {
    this.termsBooking = booking;
  }

  closeTerms(): void {
    this.termsBooking = undefined;
  }

  paymentStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      approved: 'Pagado',
      pending: 'Pendiente',
      in_process: 'En proceso',
      rejected: 'Rechazado',
      cancelled: 'Cancelado',
      refunded: 'Reembolsado',
      unknown: 'Sin información'
    };

    return labels[status?.toLowerCase()] ?? status;
  }

  paymentStatusClass(status: string): string {
    const classes: Record<string, string> = {
      approved: 'text-bg-success',
      pending: 'text-bg-warning',
      in_process: 'text-bg-info',
      rejected: 'text-bg-danger',
      cancelled: 'text-bg-secondary',
      refunded: 'text-bg-secondary'
    };

    return classes[status?.toLowerCase()] ?? 'text-bg-secondary';
  }
}
