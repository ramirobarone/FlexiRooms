import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { IssuesService } from '../../../../services/IssuesService/issues.service';
import { AdminIssuesComponent } from './admin-issues.component';

describe('AdminIssuesComponent', () => {
  let component: AdminIssuesComponent;
  let fixture: ComponentFixture<AdminIssuesComponent>;
  let issuesService: jasmine.SpyObj<IssuesService>;

  beforeEach(async () => {
    issuesService = jasmine.createSpyObj<IssuesService>('IssuesService', ['getHotelIssues', 'updateHotelIssueStatus']);
    issuesService.getHotelIssues.and.returnValue(of([]));
    await TestBed.configureTestingModule({ declarations: [AdminIssuesComponent], providers: [{ provide: IssuesService, useValue: issuesService }] }).compileComponents();
    fixture = TestBed.createComponent(AdminIssuesComponent);
    component = fixture.componentInstance;
  });

  it('loads hotel issues on initialization', () => {
    const issue = { id: 1, bookingId: 4, tipoDeReclamo: 1, tipoDeReclamoNombre: 'Limpieza', texto: 'Detalle', estado: 'Pendiente', createdAtUtc: '2026-09-17T00:00:00Z' };
    issuesService.getHotelIssues.and.returnValue(of([issue]));
    fixture.detectChanges();
    expect(component.issues).toEqual([issue]);
    expect(component.isLoading).toBeFalse();
  });

  it('updates the selected issue status', () => {
    const issue = { id: 1, bookingId: 4, tipoDeReclamo: 1, tipoDeReclamoNombre: 'Limpieza', texto: 'Detalle', estado: 'Pendiente', createdAtUtc: '2026-09-17T00:00:00Z' };
    issuesService.updateHotelIssueStatus.and.returnValue(of({ ...issue, estado: 'Resuelto' }));
    component.updateStatus(issue, 'Resuelto');
    expect(issuesService.updateHotelIssueStatus).toHaveBeenCalledWith(1, 'Resuelto');
    expect(issue.estado).toBe('Resuelto');
  });

  it('shows an error when hotel issues cannot be loaded', () => {
    issuesService.getHotelIssues.and.returnValue(throwError(() => new Error('request failed')));
    fixture.detectChanges();
    expect(component.hasError).toBeTrue();
    expect(component.isLoading).toBeFalse();
  });
});