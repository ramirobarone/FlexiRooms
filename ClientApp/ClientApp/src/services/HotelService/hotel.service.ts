import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Hotel } from 'src/models/hotel';
import { EnvironmentsService } from '../../app/ServicesShared/environments.service';

@Injectable({
  providedIn: 'root'
})
export class HotelService {
  baseurl: string;

  constructor(private http: HttpClient, private environmentsService: EnvironmentsService) {
    this.baseurl = `${this.environmentsService.getUrlBase()}hotel/`;
  }

  getHotels(keyword: string): Observable<Hotel[]> {
    return this.http.get<Hotel[]>(this.baseurl + 'getHotels?searchKey=' + keyword);
  }
getMyHotels(): Observable<Hotel[]> {
    return this.http.get<Hotel[]>(this.baseurl + 'getMyHotels');
}
  getHotel(id: number): Observable<Hotel> {
    return this.http.get<Hotel>(this.baseurl + 'getHotel?idHotel=' + id);
  }

  createHotel(hotel: Hotel): Observable<unknown> {
    console.log('Creating hotel:', hotel);
    return this.http.post(this.baseurl + 'createHotel', hotel);
  }

  updateHotel(hotel: Hotel): Observable<unknown> {
    return this.http.put(this.baseurl + 'updateHotel', hotel);
  }

  deleteHotel(id: number): Observable<unknown> {
    return this.http.delete(this.baseurl + 'deleteHotel?id=' + id);
  }
}
