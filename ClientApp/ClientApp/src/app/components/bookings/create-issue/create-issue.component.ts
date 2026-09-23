import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IssuesService } from '../../../../services/IssuesService/issues.service';

@Component({
    selector: 'app-create-issue',
    templateUrl: './create-issue.component.html',
    styleUrls: ['./create-issue.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class CreateIssueComponent implements OnInit {
  bookingId: number | null = null;
  isSubmitting = false;
  submitError = '';
  submitSuccess = false;

  texto = '';
  imageFile?: File;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly issuesService: IssuesService
  ) {}

  ngOnInit(): void {
    const bookingIdParam = this.route.snapshot.paramMap.get('bookingId');
    this.bookingId = bookingIdParam ? Number(bookingIdParam) : null;
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.imageFile = input.files && input.files.length > 0 ? input.files[0] : undefined;
  }

  submit(): void {
    if (!this.texto.trim()) {
      this.submitError = 'Completá la descripción del reclamo.';
      return;
    }

    if (!this.bookingId || this.bookingId <= 0) {
      this.submitError = 'La reserva indicada no es válida.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = '';

    this.issuesService.createIssue(this.bookingId, this.texto.trim(), this.imageFile).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.submitSuccess = true;
      },
      error: () => {
        this.isSubmitting = false;
        this.submitError = 'No pudimos enviar tu reclamo. Intentá nuevamente.';
      }
    });
  }

  goToMyIssues(): void {
    this.router.navigateByUrl('/reclamos');
  }

  goBack(): void {
    this.router.navigateByUrl('/MisReservas');
  }
}
