import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';

@Component({
    selector: 'app-control-room',
    templateUrl: './control-room.component.html',
    styleUrls: ['./control-room.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ControlRoomComponent {
  activeTab: 'summary' | 'hotels' | 'rooms' | 'bookings' | 'issues' | 'maintenance' | 'approvals' = 'summary';
  readonly isSuperAdmin = inject(LocalStorageService).isSuperAdmin();

  constructor() {
    // Back from Mercado Pago (?mp=) or from "Sumá tu hotel" (?nuevo=): land on the hotel.
    const params = inject(ActivatedRoute).snapshot.queryParamMap;
    if (params.has('mp') || params.has('nuevo')) {
      this.activeTab = 'hotels';
    }
  }
}
