import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IssuesService, IssueType } from '../../../../services/IssuesService/issues.service';

@Component({
    selector: 'app-create-issue',
    templateUrl: './create-issue.component.html',
    styleUrls: ['./create-issue.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class CreateIssueComponent implements OnInit {
  bookingId: number | null = null;
  issueTypes: IssueType[] = [];
  isLoadingTypes = true;
  isSubmitting = false;
  submitError = '';
  submitSuccess = false;

  tipoDeReclamo = 0;
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
    this.loadIssueTypes();
  }

  loadIssueTypes(): void {
    this.isLoadingTypes = true;
    this.issuesService.getIssueTypes().subscribe({
      next: (types) => {
        this.issueTypes = types ?? [];
        this.tipoDeReclamo = this.issueTypes[0]?.id ?? 0;
        this.isLoadingTypes = false;
      },
      error: () => {
        this.issueTypes = [];
        this.isLoadingTypes = false;
      }
    });
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.imageFile = input.files && input.files.length > 0 ? input.files[0] : undefined;
  }

  submit(): void {
    if (!this.tipoDeReclamo || !this.texto.trim()) {
      this.submitError = 'Completá el tipo de reclamo y la descripción.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = '';

    this.issuesService.createIssue(this.tipoDeReclamo, this.texto.trim(), this.imageFile).subscribe({
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
