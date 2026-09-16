import { Room } from '../../../models/room';
import { Component } from '@angular/core';
import { Hotel } from 'src/models/hotel';
import { GoogleMap } from '@angular/google-maps';
import { RoomService } from '../bookings/card-room/service/room.service';
import { HomeService } from './service/home.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    standalone: false
})
export class HomeComponent {
  fullName = '';
  _room: Room | null = null;
  _hoteles: Hotel[] = [];
  isBodyVisible: boolean = true;
  isNoResultsPopupVisible = false;

  constructor(private roomService: RoomService,
    private homeService: HomeService) {

  }

  ngOnInit(): void {
    const lastSearchResults = this.homeService.getLastSearchResults();
    if (lastSearchResults !== null) {
      this.loadHotels(lastSearchResults);
      return;
    }

    this.loadHotelsByDefault();
  }

  loadHotelsByDefault(): void {
    this.homeService.getHomeHotels().subscribe({
      next: hotels => {
        this.homeService.setLastSearchResults(hotels ?? [], '');
        this.loadHotels(hotels ?? []);
        if ((hotels ?? []).length !== 0) {
          this.isNoResultsPopupVisible = false;
          this.isBodyVisible = false;
        }
      },
      error: error => {
        console.error('Error loading home hotels', error);
        this.loadHotels([]);
      }
    });
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
