import { Room } from '../../../models/room';
import { Component } from '@angular/core';
import { Hotel } from 'src/models/hotel';
import { GoogleMap } from '@angular/google-maps';
import { RoomService } from '../bookings/card-room/service/room.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  fullName = '';
  _room: Room | null = null;
  _hoteles: Hotel[] = [];
  isBodyVisible: boolean = true;
  isNoResultsPopupVisible = false;

  constructor(private roomService: RoomService) {

  }
  loadHotels(_hotels: Hotel[]) {
    this._hoteles = _hotels;
    if (this._hoteles.length > 0)
      this.isBodyVisible = false;

    console.log('Home hoteles: ', this._hoteles);
  }

  showNoResultsPopup(): void {
    this.isNoResultsPopupVisible = true;
  }
}
