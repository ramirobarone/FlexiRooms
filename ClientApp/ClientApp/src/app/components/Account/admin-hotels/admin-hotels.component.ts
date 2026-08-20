import { Component, OnInit } from '@angular/core';
import { Hotel } from '../../../../models/hotel';
import { HotelService } from '../../../../services/HotelService/hotel.service';

@Component({
  selector: 'app-admin-hotels',
  templateUrl: './admin-hotels.component.html',
  styleUrls: ['./admin-hotels.component.css']
})
export class AdminHotelsComponent implements OnInit {
  hotels: Hotel[] = [];
  selectedHotel: Hotel = this.createEmptyHotel();
  isEditing = false;

  constructor(private hotelService: HotelService) {}

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.hotelService.getHotels('hotel').subscribe({
      next: (response) => this.hotels = response ?? [],
      error: () => this.hotels = []
    });
  }

  editHotel(hotel: Hotel): void {
    this.selectedHotel = JSON.parse(JSON.stringify(hotel));
    this.isEditing = true;
  }

  newHotel(): void {
    this.selectedHotel = this.createEmptyHotel();
    this.isEditing = false;
  }

  saveHotel(): void {
    const request = this.isEditing
      ? this.hotelService.updateHotel(this.selectedHotel)
      : this.hotelService.createHotel(this.selectedHotel);

    request.subscribe({
      next: () => {
        this.newHotel();
        this.loadHotels();
      }
    });
  }

  deleteHotel(id: number): void {
    this.hotelService.deleteHotel(id).subscribe({
      next: () => this.loadHotels()
    });
  }

  private createEmptyHotel(): Hotel {
    return {
      id: 0,
      name: '',
      description: '',
      metaDescription: '',
      addressHotel: { id: 0, idHotel: 0, street: '', number: 0, postalCode: '', phone: '', latitud: '', longitud: '', idCity: 0 },
      email: '',
      codeArea: 0,
      phoneNumber: 0,
      postalCode: '',
      latitud: '',
      longitud: '',
      image: '',
      pictures: []
    };
  }
}
