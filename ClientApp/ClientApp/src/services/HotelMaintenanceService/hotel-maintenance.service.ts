import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../app/ServicesShared/environments.service';

export interface MaintenanceType {
  id: number;
  description: string;
}

export interface HotelMaintenance {
  id: number;
  nameMaintenance: string;
  hotelId: number;
  maintenanceTypes: MaintenanceType[];
  telephoneNumber: string;
  active: boolean;
  createDate: string;
  createBy: string;
  updateDate?: string;
  updateBy?: string;
}

export interface CreateHotelMaintenance {
  nameMaintenance: string;
  hotelId: number;
  maintenanceTypeIds: number[];
  telephoneNumber: string;
  active: boolean;
}

export interface UpdateHotelMaintenance extends CreateHotelMaintenance {
  id: number;
}

@Injectable({
  providedIn: 'root'
})
export class HotelMaintenanceService {
  private readonly baseUrl: string;

  constructor(private http: HttpClient, environmentsService: EnvironmentsService) {
    this.baseUrl = `${environmentsService.getUrlBase()}HotelMaintenance/`;
  }

  getByHotel(hotelId: number): Observable<HotelMaintenance[]> {
    return this.http.get<HotelMaintenance[]>(`${this.baseUrl}GetByHotel?hotelId=${hotelId}`);
  }

  getMaintenanceTypes(): Observable<MaintenanceType[]> {
    return this.http.get<MaintenanceType[]>(`${this.baseUrl}GetMaintenanceTypes`);
  }

  create(request: CreateHotelMaintenance): Observable<HotelMaintenance> {
    return this.http.post<HotelMaintenance>(`${this.baseUrl}Create`, request);
  }

  update(request: UpdateHotelMaintenance): Observable<HotelMaintenance> {
    return this.http.put<HotelMaintenance>(`${this.baseUrl}Update`, request);
  }

  delete(id: number): Observable<unknown> {
    return this.http.delete(`${this.baseUrl}Delete?id=${id}`);
  }
}
