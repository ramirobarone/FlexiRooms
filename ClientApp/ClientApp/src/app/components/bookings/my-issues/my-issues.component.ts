import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { IssuesService, Issue } from '../../../../services/IssuesService/issues.service';

@Component({
    selector: 'app-my-issues',
    templateUrl: './my-issues.component.html',
    styleUrls: ['./my-issues.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MyIssuesComponent implements OnInit {
  issues: Issue[] = [];
  isLoading = true;
  hasError = false;

  constructor(private readonly issuesService: IssuesService) {}

  ngOnInit(): void {
    this.loadIssues();
  }

  loadIssues(): void {
    this.isLoading = true;
    this.hasError = false;
    this.issuesService.getMyIssues().subscribe({
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

  getImageSource(path?: string): string {
    if (!path) {
      return '';
    }

    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }

    return `${window.location.origin}${path}`;
  }

  statusClass(status: string): string {
    const classes: Record<string, string> = {
      pendiente: 'text-bg-warning',
      'en proceso': 'text-bg-info',
      resuelto: 'text-bg-success',
      rechazado: 'text-bg-danger'
    };

    return classes[status?.toLowerCase()] ?? 'text-bg-secondary';
  }
}
