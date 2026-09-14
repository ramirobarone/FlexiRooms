import { Component, Output, EventEmitter } from '@angular/core';
import { Hotel } from 'src/models/hotel';
import { HotelService } from 'src/services/HotelService/hotel.service';
import { HomeService } from '../service/home.service';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.css']
})
export class SearchBarComponent {

  @Output() _hotelesEmitter = new EventEmitter<Hotel[]>();
  _hoteles: Hotel[] = [];
  currentString: string = '';
  isNoResultsPopupVisible = false;

  constructor(private hotelService: HotelService,
              private homeService: HomeService) {
    this.currentString = this.homeService.getLastSearchTerm();
  }

  BuscarHotel(): void {

    if (this.currentString === '')
      return;

    this.hotelService.getHotels(this.currentString).subscribe(res => {
      
      this._hoteles = res ?? [];
      this.homeService.setLastSearchResults(this._hoteles, this.currentString);
      if (res === null || (res && this._hoteles.length === 0)) {
        this.isNoResultsPopupVisible = true;
      }
      this._hotelesEmitter.emit(this._hoteles);

    });

  }
}