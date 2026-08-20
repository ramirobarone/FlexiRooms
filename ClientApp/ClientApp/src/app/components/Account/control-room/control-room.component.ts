import { Component } from '@angular/core';

@Component({
  selector: 'app-control-room',
  templateUrl: './control-room.component.html',
  styleUrls: ['./control-room.component.css']
})
export class ControlRoomComponent {
  activeTab: 'summary' | 'hotels' | 'bookings' = 'summary';
}
