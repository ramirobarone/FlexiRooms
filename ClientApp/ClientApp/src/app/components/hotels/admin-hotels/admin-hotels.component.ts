import { Component, OnInit } from '@angular/core';
import { Hotel } from '../../../../models/hotel';
import { Province } from '../../../../models/Province';
import { City } from '../../../../models/City';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { GeographyService } from '../../../../services/GeographyService/geography.service';

@Component({
  selector: 'app-admin-hotels',
  templateUrl: './admin-hotels.component.html',
  styleUrls: ['./admin-hotels.component.css']
})
export class AdminHotelsComponent implements OnInit {
  hotels: Hotel[] = [];
  selectedHotel: Hotel = this.createEmptyHotel();
  isEditing = false;
  provinces: Province[] = [];
  cities: City[] = [];

  constructor(private hotelService: HotelService, private geographyService: GeographyService) {}

  ngOnInit(): void {
    this.loadHotels();
    this.loadProvinces();
  }

  loadHotels(): void {
    this.hotelService.getMyHotels().subscribe({
      next: (response) => this.hotels = response ?? [],
      error: () => this.hotels = []
    });
  }

  loadProvinces(): void {
    this.geographyService.getProvinces().subscribe({
      next: (response) => this.provinces = response ?? [],
      error: () => this.provinces = []
    });
  }

  onProvinceChange(): void {
    this.selectedHotel.addressHotel.idCity = 0;
    this.cities = [];

    if (this.selectedHotel.addressHotel.idProvince) {
      this.geographyService.getCitiesByProvince(this.selectedHotel.addressHotel.idProvince).subscribe({
        next: (response) => this.cities = response ?? [],
        error: () => this.cities = []
      });
    }
  }

  editHotel(hotel: Hotel): void {
    this.selectedHotel = JSON.parse(JSON.stringify(hotel));
    this.isEditing = true;
    this.cities = [];

    if (this.selectedHotel.addressHotel.idProvince) {
      this.geographyService.getCitiesByProvince(this.selectedHotel.addressHotel.idProvince).subscribe({
        next: (response) => this.cities = response ?? [],
        error: () => this.cities = []
      });
    }
  }

  newHotel(): void {
    this.selectedHotel = this.createEmptyHotel();
    this.isEditing = false;
    this.cities = [];
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
      addressHotel: { id: 0, idHotel: 0, street: '', number: '', postalCode: '', latitud: '', longitud: '', idCity: 0, idProvince: 0 },
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
