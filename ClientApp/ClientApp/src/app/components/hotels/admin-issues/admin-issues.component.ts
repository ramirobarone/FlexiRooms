import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Issue, IssuesService } from '../../../../services/IssuesService/issues.service';

@Component({
  selector: 'app-admin-issues',
  templateUrl: './admin-issues.component.html',
  styleUrls: ['./admin-issues.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: false
})
export class AdminIssuesComponent implements OnInit {
  readonly statuses = ['Resuelto', 'En Proceso', 'Anulado'];
  issues: Issue[] = [];
  isLoading = true;
  hasError = false;
  updatingIssueId?: number;

  constructor(private readonly issuesService: IssuesService) {}

  ngOnInit(): void {
    this.loadIssues();
  }

  loadIssues(): void {
    this.isLoading = true;
    this.hasError = false;
    this.issuesService.getHotelIssues().subscribe({
      next: (issues) => {
        this.issues = issues ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.issues = [];
        this.hasError = true;
        this.isLoading = false;
      }
    });
  }

  updateStatus(issue: Issue, status: string): void {
    if (issue.estado === status || this.updatingIssueId) {
      return;
    }

    this.updatingIssueId = issue.id;
    this.issuesService.updateHotelIssueStatus(issue.id, status).subscribe({
      next: (updatedIssue) => {
        issue.estado = updatedIssue.estado;
        this.updatingIssueId = undefined;
      },
      error: () => this.updatingIssueId = undefined
    });
  }

  statusClass(status: string): string {
    const classes: Record<string, string> = {
      pendiente: 'text-bg-warning',
      'en proceso': 'text-bg-info',
      resuelto: 'text-bg-success',
      anulado: 'text-bg-secondary'
    };

    return classes[status?.toLowerCase()] ?? 'text-bg-secondary';
  }
}