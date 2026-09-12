import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { roomPictures } from 'src/models/roomImages';
import { EnvironmentsService } from '../../app/ServicesShared/environments.service';

@Injectable({
  providedIn: 'root'
})
export class RoomImageService {
  private baseUrl: string;

  constructor(private http: HttpClient, private environmentsService: EnvironmentsService) {
    this.baseUrl = `${this.environmentsService.getUrlBase()}roomimages/`;
  }

  uploadRoomImages(roomId: number, files: File[]): Observable<roomPictures[]> {
    const formData = new FormData();
    files.forEach(file => formData.append('files', file));

    return this.http.post<roomPictures[]>(`${this.baseUrl}${roomId}/upload`, formData);
  }

  getRoomImages(roomId: number): Observable<roomPictures[]> {
    return this.http.get<roomPictures[]>(`${this.baseUrl}${roomId}`);
  }

  deleteRoomImage(roomId: number, imageId: number): Observable<unknown> {
    return this.http.delete(`${this.baseUrl}${roomId}/${imageId}`);
  }
}
