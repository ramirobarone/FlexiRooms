import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { HotelMaintenanceService } from '../../../../services/HotelMaintenanceService/hotel-maintenance.service';
import { AdminMaintenanceComponent } from './admin-maintenance.component';

describe('AdminMaintenanceComponent', () => {
  let component: AdminMaintenanceComponent;
  let fixture: ComponentFixture<AdminMaintenanceComponent>;
  let hotelService: jasmine.SpyObj<HotelService>;
  let hotelMaintenanceService: jasmine.SpyObj<HotelMaintenanceService>;

  const hotel = { id: 1, name: 'Hotel Test' } as any;
  const maintenance = {
    id: 1,
    nameMaintenance: 'Plomero Juan',
    hotelId: 1,
    maintenanceTypes: [{ id: 2, description: 'Plomería' }],
    telephoneNumber: '123456789',
    active: true,
    createDate: '2026-09-17T00:00:00Z',
    createBy: 'admin'
  };

  beforeEach(async () => {
    hotelService = jasmine.createSpyObj<HotelService>('HotelService', ['getMyHotels']);
    hotelService.getMyHotels.and.returnValue(of([hotel]));

    hotelMaintenanceService = jasmine.createSpyObj<HotelMaintenanceService>(
      'HotelMaintenanceService',
      ['getByHotel', 'getMaintenanceTypes', 'create', 'update', 'delete']
    );
    hotelMaintenanceService.getMaintenanceTypes.and.returnValue(of([{ id: 2, description: 'Plomería' }]));
    hotelMaintenanceService.getByHotel.and.returnValue(of([maintenance]));

    await TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [AdminMaintenanceComponent],
      providers: [
        { provide: HotelService, useValue: hotelService },
        { provide: HotelMaintenanceService, useValue: hotelMaintenanceService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminMaintenanceComponent);
    component = fixture.componentInstance;
  });

  it('loads hotels and maintenances on initialization', () => {
    fixture.detectChanges();
    expect(component.hotels).toEqual([hotel]);
    expect(component.selectedHotelId).toBe(1);
    expect(component.maintenances).toEqual([maintenance]);
  });

  it('shows an error when maintenances cannot be loaded', () => {
    hotelMaintenanceService.getByHotel.and.returnValue(throwError(() => new Error('request failed')));
    fixture.detectChanges();
    expect(component.hasError).toBeTrue();
    expect(component.maintenances).toEqual([]);
  });

  it('populates the form when editing a maintenance', () => {
    fixture.detectChanges();
    component.editMaintenance(maintenance);
    expect(component.isEditing).toBeTrue();
    expect(component.selectedMaintenance.nameMaintenance).toBe('Plomero Juan');
    expect(component.selectedMaintenance.maintenanceTypeIds).toEqual([2]);
  });

  it('toggles maintenance types on and off', () => {
    fixture.detectChanges();
    component.newMaintenance();
    component.toggleMaintenanceType(2, true);
    expect(component.isMaintenanceTypeSelected(2)).toBeTrue();
    component.toggleMaintenanceType(2, false);
    expect(component.isMaintenanceTypeSelected(2)).toBeFalse();
  });

  it('creates a new maintenance and refreshes the list', () => {
    fixture.detectChanges();
    hotelMaintenanceService.create.and.returnValue(of(maintenance));
    component.selectedMaintenance = {
      nameMaintenance: 'Electricista Ana',
      hotelId: 1,
      maintenanceTypeIds: [2, 3],
      telephoneNumber: '987654321',
      active: true
    };
    component.saveMaintenance();
    expect(hotelMaintenanceService.create).toHaveBeenCalled();
    expect(component.isEditing).toBeFalse();
  });

  it('deletes a maintenance after confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    hotelMaintenanceService.delete.and.returnValue(of({}));
    fixture.detectChanges();
    component.deleteMaintenance(1);
    expect(hotelMaintenanceService.delete).toHaveBeenCalledWith(1);
  });
});
