import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../../../ServicesShared/environments.service';
import { Room } from '../../../../../models/room';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  baseurl: string;

  constructor(private http: HttpClient, private serviceEnvironment: EnvironmentsService) {
    this.baseurl = `${this.serviceEnvironment.getUrlBase()}room/`;
  }

  public getRooms(id: number): Observable<Room[]> {
    return this.http.get<Room[]>(this.baseurl + 'getroombyid?idHotel=' + id);
  }

  public getRoom(id: number): Observable<Room> {
    return this.http.get<Room>(this.baseurl + 'getroom?id=' + id);
  }

  public createRoom(room: Room): Observable<unknown> {
    return this.http.post(this.baseurl + 'createroom', room);
  }

  public updateRoom(room: Room): Observable<unknown> {
    return this.http.put(this.baseurl + 'updateroom', room);
  }

  public deleteRoom(id: number): Observable<unknown> {
    return this.http.delete(this.baseurl + 'deleteroom?id=' + id);
  }
}
