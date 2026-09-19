import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentsService } from '../../../ServicesShared/environments.service';

export interface Review {
  id: number;
  reviewText: string;
  value: number;
  createDate: string;
  createBy: string;
  updateDate?: string;
  updateBy?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewsService {
  private readonly baseUrl: string;

  constructor(private readonly http: HttpClient, environmentsService: EnvironmentsService) {
    this.baseUrl = `${environmentsService.getUrlBase()}Reviews/`;
  }

  getMyReviews(): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.baseUrl}GetMyReviews`);
  }

  createReview(reviewText: string, value: number, bookingId: number): Observable<Review> {
    return this.http.post<Review>(this.baseUrl, { reviewText, value, bookingId });
  }
}
