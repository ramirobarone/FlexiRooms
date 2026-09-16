import { Component } from '@angular/core';

@Component({
    selector: 'app-control-room',
    templateUrl: './control-room.component.html',
    styleUrls: ['./control-room.component.css'],
    standalone: false
})
export class ControlRoomComponent {
  activeTab: 'summary' | 'hotels' | 'rooms' | 'bookings' = 'summary';
}
