import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hotel } from '../../../../models/hotel';
import { EnvironmentsService } from '../../../ServicesShared/environments.service';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  private readonly baseUrl: string;
  private lastSearchResults: Hotel[] | null = null;
  private lastSearchTerm = '';

  constructor(private readonly http: HttpClient,
              environmentsService: EnvironmentsService) {
    this.baseUrl = `${environmentsService.getUrlBase()}hotel/`;
  }

  //_rooms: Room[] = [
  //  { id: 1, name: "Habitacion 1", description: "3 personas", urlPictures: "https://dosflorines.com.ar/wp-content/uploads/2020/07/contenedores-habitables.png" },
  //  { id: 1, name: "Habitacion 2", description: "2 personas", urlPictures: "https://dosflorines.com.ar/wp-content/uploads/2020/07/contenedores-habitables.png" },
  //  { id: 1, name: "Habitacion 3", description: "4 personas", urlPictures: "https://dosflorines.com.ar/wp-content/uploads/2020/07/contenedores-habitables.png" }
  //];


  public getHotels(hotel:string): Hotel[] {

    let hoteles: Hotel[] = [];


    return hoteles;
  }

  getHomeHotels(pageNumber = 1): Observable<Hotel[]> {
    return this.http.get<Hotel[]>(`${this.baseUrl}getHomeHotels?pageNumber=${pageNumber}`);
  }

  setLastSearchResults(hotels: Hotel[], searchTerm: string): void {
    this.lastSearchResults = hotels;
    this.lastSearchTerm = searchTerm;
  }

  getLastSearchResults(): Hotel[] | null {
    return this.lastSearchResults;
  }

  getLastSearchTerm(): string {
    return this.lastSearchTerm;
  }
}
