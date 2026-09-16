import { Room } from 'src/models/room';
import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { RoomService } from '../bookings/card-room/service/room.service';

@Component({
    selector: 'app-reserved',
    templateUrl: './reserved.component.html',
    styleUrls: ['./reserved.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ReservedComponent {


  currentRoom: number = 0;
  dateSelected: Date = new Date();
  idHour: number = 0;
  cost: number = 0;

  selectedRoom: Room  = new Room();

  constructor(private roomService: RoomService) {

  }


  getRoomSelected(): void {


  }

  getHoursAvailable(): void {

  }
  getCost(idRoom: number): void {

  }
  nextToPay(): void {

  }

}
