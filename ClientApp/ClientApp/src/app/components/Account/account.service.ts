import { Injectable } from '@angular/core';
import { EnvironmentsService } from '../../ServicesShared/environments.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UserCreateDto } from './Models/accountDto';
import { UpdateUserInformationDto, UserInformationDto } from './Models/userInformationDto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private _environment: EnvironmentsService, private http: HttpClient) { }

  createAccount(account:UserCreateDto): Observable<boolean> {

    console.log('urlbase', this._environment.getUrlBase() + 'Auth/CreateAccount');
    return this.http.post<boolean>(this._environment.getUrlBase() + 'Auth/CreateAccount', account);
  }

  getInformation(): Observable<UserInformationDto> {
    return this.http.get<UserInformationDto>(this._environment.getUrlBase() + 'Auth/GetInformation');
  }

  updateInformation(account: UpdateUserInformationDto): Observable<UserInformationDto> {
    return this.http.put<UserInformationDto>(this._environment.getUrlBase() + 'Auth/UpdateInformation', account);
  }

  updateProfileImage(file: File): Observable<void> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<void>(this._environment.getUrlBase() + 'Auth/UpdateProfileImage', formData);
  }


}
