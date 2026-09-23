import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../app/ServicesShared/environments.service';

export interface IssueType {
  id: number;
  issue: string;
}

export interface Issue {
  id: number;
  bookingId?: number;
  tipoDeReclamo?: number;
  tipoDeReclamoNombre?: string;
  texto: string;
  imagen?: string;
  estado: string;
  createdAtUtc: string;
}

@Injectable({
  providedIn: 'root'
})
export class IssuesService {
  private readonly baseUrl: string;

  constructor(private http: HttpClient, environmentsService: EnvironmentsService) {
    this.baseUrl = `${environmentsService.getUrlBase()}Issues/`;
  }

  getIssueTypes(): Observable<IssueType[]> {
    return this.http.get<IssueType[]>(`${this.baseUrl}GetIssueTypes`);
  }

  getMyIssues(): Observable<Issue[]> {
    return this.http.get<Issue[]>(`${this.baseUrl}GetMyIssues`);
  }

  getHotelIssues(): Observable<Issue[]> {
    return this.http.get<Issue[]>(`${this.baseUrl}GetHotelIssues`);
  }

  updateHotelIssueStatus(id: number, estado: string): Observable<Issue> {
    return this.http.patch<Issue>(`${this.baseUrl}UpdateHotelIssueStatus?id=${id}`, { estado });
  }

  createIssue(bookingId: number, texto: string, imagen?: File): Observable<Issue> {
    const formData = new FormData();
    formData.append('bookingId', bookingId.toString());
    formData.append('texto', texto);
    if (imagen) {
      formData.append('imagen', imagen);
    }

    return this.http.post<Issue>(this.baseUrl, formData);
  }
}
