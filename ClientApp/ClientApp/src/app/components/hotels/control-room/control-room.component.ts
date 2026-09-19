import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-control-room',
    templateUrl: './control-room.component.html',
    styleUrls: ['./control-room.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ControlRoomComponent {
  activeTab: 'summary' | 'hotels' | 'rooms' | 'bookings' | 'issues' | 'maintenance' = 'summary';
}
