import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReviewsService } from './reviews.service';

@Component({
  imports: [CommonModule, FormsModule, RouterLink],
  selector: 'app-reviews',
  styleUrl: './reviews.component.css',
  templateUrl: './reviews.component.html',
})
export class ReviewsComponent implements OnInit {
  bookingId: number | null = null;
  reviewText = '';
  value = 5;
  isSubmitting = false;
  submitError = '';
  submitSuccess = false;

  constructor(private readonly route: ActivatedRoute, private readonly reviewsService: ReviewsService) {}

  ngOnInit(): void {
    const bookingIdParam = this.route.snapshot.paramMap.get('bookingId');
    this.bookingId = bookingIdParam ? Number(bookingIdParam) : null;
  }

  submit(): void {
    if (!this.reviewText.trim()) {
      this.submitError = 'Escribí tu reseña antes de enviarla.';
      return;
    }

    if (!this.bookingId || this.bookingId <= 0) {
      this.submitError = 'La reserva indicada no es válida.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = '';
    this.reviewsService.createReview(this.reviewText.trim(), this.value, this.bookingId).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.submitSuccess = true;
      },
      error: () => {
        this.isSubmitting = false;
        this.submitError = 'No pudimos enviar tu reseña. Intentá nuevamente.';
      }
    });
  }
}
