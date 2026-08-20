import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto, UserLoginDto } from '../Models/userDto';
import { EnvironmentsService } from '../../../../ServicesShared/environments.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseurl: string;

  constructor(private http: HttpClient, private environmentsService: EnvironmentsService) {
    this.baseurl = `${this.environmentsService.getUrlBase()}auth/`;
  }

  authenticat(userDto: UserDto): Observable<UserLoginDto> {
    return this.http.post<UserLoginDto>(this.baseurl + 'Authenticat', userDto);
  }
}
