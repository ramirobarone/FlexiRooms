import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateHotelResponse, Hotel, PendingHotel } from 'src/models/hotel';
import { hotelPicture } from 'src/models/hotelPicture';
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

  /** The creator becomes the owner; non-SuperAdmin hotels start Pending until approved. */
  createHotel(hotel: Hotel): Observable<CreateHotelResponse> {
    return this.http.post<CreateHotelResponse>(this.baseurl + 'createHotel', hotel);
  }

  // ---------------------------------------------------------- SuperAdmin review

  getPendingHotels(): Observable<PendingHotel[]> {
    return this.http.get<PendingHotel[]>(this.baseurl + 'getPendingHotels');
  }

  approveHotel(id: number): Observable<void> {
    return this.http.post<void>(`${this.baseurl}${id}/approve`, {});
  }

  rejectHotel(id: number, reason: string): Observable<void> {
    return this.http.post<void>(`${this.baseurl}${id}/reject`, { reason });
  }

  updateHotel(hotel: Hotel): Observable<unknown> {
    return this.http.put(this.baseurl + 'updateHotel', hotel);
  }

  deleteHotel(id: number): Observable<unknown> {
    return this.http.delete(this.baseurl + 'deleteHotel?id=' + id);
  }

  uploadHotelImages(hotelId: number, files: File[]): Observable<hotelPicture[]> {
    const formData = new FormData();
    files.forEach(file => formData.append('files', file));

    return this.http.post<hotelPicture[]>(`${this.environmentsService.getUrlBase()}hotelimages/${hotelId}/upload`, formData);
  }

  getHotelImages(hotelId: number): Observable<hotelPicture[]> {
    return this.http.get<hotelPicture[]>(`${this.environmentsService.getUrlBase()}hotelimages/${hotelId}`);
  }
}
