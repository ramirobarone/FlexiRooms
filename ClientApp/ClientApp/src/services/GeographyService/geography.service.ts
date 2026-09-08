import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Province } from 'src/models/Province';
import { City } from 'src/models/City';
import { EnvironmentsService } from '../../app/ServicesShared/environments.service';

@Injectable({
  providedIn: 'root'
})
export class GeographyService {
  baseurl: string;

  constructor(private http: HttpClient, private environmentsService: EnvironmentsService) {
    this.baseurl = `${this.environmentsService.getUrlBase()}Geography/`;
  }

  getProvinces(): Observable<Province[]> {
    return this.http.get<Province[]>(this.baseurl + 'GetProvinces');
  }

  getCitiesByProvince(provinceId: number): Observable<City[]> {
    return this.http.get<City[]>(this.baseurl + 'GetCitiesByProvince?provinceId=' + provinceId);
  }
}
