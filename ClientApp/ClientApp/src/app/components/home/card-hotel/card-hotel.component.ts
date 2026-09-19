import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { Room } from 'src/models/room';
import { Router } from '@angular/router';
import { hotelPicture } from 'src/models/hotelPicture';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-card-hotel',
    templateUrl: './card-hotel.component.html',
    styleUrls: ['./card-hotel.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
  @Input()
  reviewScore: number = 0;

  showCopiedMessage: boolean = false;

  get fullStars(): number[] {
    return Array(Math.floor(this.reviewScore)).fill(0);
  }

  get hasHalfStar(): boolean {
    return this.reviewScore % 1 >= 0.5;
  }

  get emptyStars(): number[] {
    const filled = Math.floor(this.reviewScore) + (this.hasHalfStar ? 1 : 0);
    return Array(5 - filled).fill(0);
  }

  constructor(private router: Router) {

  }

  getImageSource(path: string): string {

    console.log('environment', environment);
    if (!environment.production) {
      return 'https://flexirooms.com.ar/hotel-images/6/carlos-paz-2.png';
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

  shareHotel(event: Event): void {
    event.stopPropagation();

    const url = `${window.location.origin}/steps/${this.idHotel}`;

    navigator.clipboard.writeText(url).then(() => {
      this.showCopiedMessage = true;
      setTimeout(() => this.showCopiedMessage = false, 2000);
    });
  }

}
