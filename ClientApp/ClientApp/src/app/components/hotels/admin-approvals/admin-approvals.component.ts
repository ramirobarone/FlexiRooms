import { Component, OnInit, ChangeDetectionStrategy, inject } from '@angular/core';
import { PendingHotel } from '../../../../models/hotel';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { ConfirmService } from '../../Shared/confirm-dialog/confirm.service';

/** SuperAdmin queue: hotels registered by owners, waiting to be published. */
@Component({
  selector: 'app-admin-approvals',
  templateUrl: './admin-approvals.component.html',
  styleUrls: ['./admin-approvals.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: false
})
export class AdminApprovalsComponent implements OnInit {
  pending: PendingHotel[] = [];
  isLoading = true;
  hasError = false;
  busyId: number | null = null;
  rejectingId: number | null = null;
  rejectReason = '';
  notice: { kind: 'success' | 'warning' | 'danger'; text: string } | null = null;

  private readonly hotelService = inject(HotelService);
  private readonly confirmService = inject(ConfirmService);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.hasError = false;
    this.hotelService.getPendingHotels().subscribe({
      next: hotels => {
        this.pending = hotels ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.hasError = true;
        this.isLoading = false;
      }
    });
  }

  async approve(hotel: PendingHotel): Promise<void> {
    const confirmed = await this.confirmService.confirm({
      title: `¿Publicar "${hotel.name}"?`,
      message: 'Va a aparecer en las búsquedas y podrá recibir reservas y pagos.',
      confirmText: 'Publicar'
    });
    if (!confirmed) {
      return;
    }

    this.busyId = hotel.id;
    this.hotelService.approveHotel(hotel.id).subscribe({
      next: () => this.done(hotel, { kind: 'success', text: `"${hotel.name}" ya está publicado.` }),
      error: () => this.fail('No pudimos aprobar el hotel. Volvé a intentarlo.')
    });
  }

  startReject(hotel: PendingHotel): void {
    this.rejectingId = hotel.id;
    this.rejectReason = '';
  }

  cancelReject(): void {
    this.rejectingId = null;
    this.rejectReason = '';
  }

  confirmReject(hotel: PendingHotel): void {
    const reason = this.rejectReason.trim();
    if (!reason) {
      return;
    }

    this.busyId = hotel.id;
    this.hotelService.rejectHotel(hotel.id, reason).subscribe({
      next: () => this.done(hotel, { kind: 'warning', text: `"${hotel.name}" fue rechazado. El dueño verá el motivo en su panel.` }),
      error: () => this.fail('No pudimos rechazar el hotel. Volvé a intentarlo.')
    });
  }

  private done(hotel: PendingHotel, notice: AdminApprovalsComponent['notice']): void {
    this.busyId = null;
    this.rejectingId = null;
    this.pending = this.pending.filter(h => h.id !== hotel.id);
    this.notice = notice;
  }

  private fail(text: string): void {
    this.busyId = null;
    this.notice = { kind: 'danger', text };
  }
}
