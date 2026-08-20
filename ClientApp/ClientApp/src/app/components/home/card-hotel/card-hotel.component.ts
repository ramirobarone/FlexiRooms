import { Component, Input } from '@angular/core';
import { Room } from 'src/models/room';
import { Router } from '@angular/router';

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
  image: string = ''
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

  setCurrentRoom(): void {
    
    let id = Number.parseInt(this.idHotel)

    let currentRoom = { id: id, name: this.name, description: this.description, urlPictures: "" }

    this.router.navigate(['/steps', { id: this.idHotel }]);

    //this.RoomService.setCurrentRoom(currentRoom)
  }

}
