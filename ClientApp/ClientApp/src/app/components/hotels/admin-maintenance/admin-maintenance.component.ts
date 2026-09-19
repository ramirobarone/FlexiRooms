import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Hotel } from '../../../../models/hotel';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import {
  CreateHotelMaintenance,
  HotelMaintenance,
  HotelMaintenanceService,
  MaintenanceType
} from '../../../../services/HotelMaintenanceService/hotel-maintenance.service';

@Component({
  selector: 'app-admin-maintenance',
  templateUrl: './admin-maintenance.component.html',
  styleUrls: ['./admin-maintenance.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: false
})
export class AdminMaintenanceComponent implements OnInit {
  hotels: Hotel[] = [];
  selectedHotelId = 0;
  maintenanceTypes: MaintenanceType[] = [];
  maintenances: HotelMaintenance[] = [];
  selectedMaintenance: CreateHotelMaintenance & { id?: number } = this.createEmptyMaintenance();
  isEditing = false;
  isLoading = false;
  hasError = false;

  constructor(
    private readonly hotelService: HotelService,
    private readonly hotelMaintenanceService: HotelMaintenanceService
  ) {}

  ngOnInit(): void {
    this.loadHotels();
    this.loadMaintenanceTypes();
  }

  loadHotels(): void {
    this.hotelService.getMyHotels().subscribe({
      next: (response) => {
        this.hotels = response ?? [];
        if (this.hotels.length > 0) {
          this.selectedHotelId = this.hotels[0].id;
          this.loadMaintenances();
        }
      },
      error: () => this.hotels = []
    });
  }

  loadMaintenanceTypes(): void {
    this.hotelMaintenanceService.getMaintenanceTypes().subscribe({
      next: (response) => this.maintenanceTypes = response ?? [],
      error: () => this.maintenanceTypes = []
    });
  }

  onHotelChange(): void {
    this.newMaintenance();
    this.loadMaintenances();
  }

  loadMaintenances(): void {
    if (!this.selectedHotelId) {
      this.maintenances = [];
      return;
    }

    this.isLoading = true;
    this.hasError = false;
    this.hotelMaintenanceService.getByHotel(this.selectedHotelId).subscribe({
      next: (response) => {
        this.maintenances = response ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.maintenances = [];
        this.hasError = true;
        this.isLoading = false;
      }
    });
  }

  newMaintenance(): void {
    this.selectedMaintenance = this.createEmptyMaintenance();
    this.isEditing = false;
  }

  editMaintenance(maintenance: HotelMaintenance): void {
    this.selectedMaintenance = {
      id: maintenance.id,
      nameMaintenance: maintenance.nameMaintenance,
      hotelId: maintenance.hotelId,
      maintenanceTypeIds: maintenance.maintenanceTypes.map(type => type.id),
      telephoneNumber: maintenance.telephoneNumber,
      active: maintenance.active
    };
    this.isEditing = true;
  }

  isMaintenanceTypeSelected(typeId: number): boolean {
    return this.selectedMaintenance.maintenanceTypeIds.includes(typeId);
  }

  toggleMaintenanceType(typeId: number, checked: boolean): void {
    if (checked) {
      if (!this.isMaintenanceTypeSelected(typeId)) {
        this.selectedMaintenance.maintenanceTypeIds.push(typeId);
      }
    } else {
      this.selectedMaintenance.maintenanceTypeIds = this.selectedMaintenance.maintenanceTypeIds.filter(id => id !== typeId);
    }
  }

  saveMaintenance(): void {
    if (!this.selectedHotelId || this.selectedMaintenance.maintenanceTypeIds.length === 0) {
      return;
    }

    this.selectedMaintenance.hotelId = this.selectedHotelId;

    const request = this.isEditing
      ? this.hotelMaintenanceService.update({ ...this.selectedMaintenance, id: this.selectedMaintenance.id! })
      : this.hotelMaintenanceService.create(this.selectedMaintenance);

    request.subscribe({
      next: () => {
        this.newMaintenance();
        this.loadMaintenances();
      }
    });
  }

  deleteMaintenance(id: number): void {
    if (confirm('¿Está seguro de eliminar este responsable de mantenimiento?')) {
      this.hotelMaintenanceService.delete(id).subscribe({
        next: () => this.loadMaintenances()
      });
    }
  }

  private createEmptyMaintenance(): CreateHotelMaintenance & { id?: number } {
    return {
      nameMaintenance: '',
      hotelId: this.selectedHotelId,
      maintenanceTypeIds: [],
      telephoneNumber: '',
      active: true
    };
  }
}
