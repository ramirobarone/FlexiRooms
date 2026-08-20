import { HttpClient } from '@angular/common/http';
import { Injectable, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { RoomDto } from 'src/models/bookingDto';
import { Room } from 'src/models/room';

@Injectable({
  providedIn: 'root'
})
export class AvialableRoomsService {

  constructor(private http: HttpClient) { }

  baseurl: string = 'https://localhost:7291/api/';

  getRoom(idhotel: string | undefined): Observable<Room[]> {
    return this.http.get<Room[]>(this.baseurl + 'room/GetRoomById?idhotel=' + idhotel);
  }
  getTimesFree(idRoom: number, date: string): Observable<any> {

    return this.http.get(this.baseurl + `Bookings/GetSchedulesByRoom?idRoom=` + idRoom + "&date=" + date);
  }
  public CheckTemporalAvaiabilityRoom (bookingDto: RoomDto): Observable<boolean> {
    return this.http.post<boolean>(this.baseurl + 'bookings/CheckTemporalAvaiabilityRoom', bookingDto);
  }
}
