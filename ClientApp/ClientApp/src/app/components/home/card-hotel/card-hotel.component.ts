import { Component, Input } from '@angular/core';
import { Room } from 'src/models/room';
import { Router } from '@angular/router';
import { hotelPicture } from 'src/models/hotelPicture';

@Component({
  selector: 'app-card-hotel',
  templateUrl: './card-hotel.component.html',
  styleUrls: ['./card-hotel.component.css']
})
export class CardHotelComponent {

  @Input()
  name: string = '';
  @Input()
  description: string = ''
  @Input()
  images: hotelPicture[] = []
  @Input()
  address: string = '';
  @Input()
  availableRooms: number = 0;
  @Input()
  stateHotel: boolean = true;
  @Input()
  idHotel: string = '';

  constructor(private router: Router) {

  }

  getImageSource(path: string): string {
    if (!path) {
      return '';
    }

    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }

    return `${window.location.origin}${path}`;
  }

  setCurrentRoom(): void {
    
    let id = Number.parseInt(this.idHotel)

    let currentRoom = { id: id, name: this.name, description: this.description, urlPictures: "" }

    this.router.navigate(['/steps', { id: this.idHotel }]);

    //this.RoomService.setCurrentRoom(currentRoom)
  }

}
