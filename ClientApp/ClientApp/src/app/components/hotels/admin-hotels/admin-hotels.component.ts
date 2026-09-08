import { Component, OnInit } from '@angular/core';
import { Hotel } from '../../../../models/hotel';
import { Province } from '../../../../models/Province';
import { City } from '../../../../models/City';
import { hotelPicture } from '../../../../models/hotelPicture';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { GeographyService } from '../../../../services/GeographyService/geography.service';

@Component({
  selector: 'app-admin-hotels',
  templateUrl: './admin-hotels.component.html',
  styleUrls: ['./admin-hotels.component.css']
})
export class AdminHotelsComponent implements OnInit {
  hotels: Hotel[] = [];
  selectedHotel: Hotel = this.createEmptyHotel();
  isEditing = false;
  provinces: Province[] = [];
  cities: City[] = [];

  selectedHotelForImages: Hotel | null = null;
  selectedFiles: File[] = [];
  hotelImages: hotelPicture[] = [];
  isUploadPopupVisible = false;
  isGalleryPopupVisible = false;
  isUploadingImages = false;
  isLoadingImages = false;
  imagesErrorMessage = '';

  constructor(private hotelService: HotelService, private geographyService: GeographyService) { }

  ngOnInit(): void {
    this.loadHotels();
    this.loadProvinces();
  }

  loadHotels(): void {
    this.hotelService.getMyHotels().subscribe({
      next: (response) => this.hotels = response ?? [],
      error: () => this.hotels = []
    });
  }

  loadProvinces(): void {
    this.geographyService.getProvinces().subscribe({
      next: (response) => this.provinces = response ?? [],
      error: () => this.provinces = []
    });
  }

  onProvinceChange(): void {
    this.selectedHotel.addressHotel.idCity = 0;
    this.cities = [];

    if (this.selectedHotel.addressHotel.idProvince) {
      this.geographyService.getCitiesByProvince(this.selectedHotel.addressHotel.idProvince).subscribe({
        next: (response) => this.cities = response ?? [],
        error: () => this.cities = []
      });
    }
  }

  editHotel(hotel: Hotel): void {
    this.selectedHotel = JSON.parse(JSON.stringify(hotel));
    this.isEditing = true;
    this.cities = [];

    if (this.selectedHotel.addressHotel.idProvince) {
      this.geographyService.getCitiesByProvince(this.selectedHotel.addressHotel.idProvince).subscribe({
        next: (response) => this.cities = response ?? [],
        error: () => this.cities = []
      });
    }
  }

  newHotel(): void {
    this.selectedHotel = this.createEmptyHotel();
    this.isEditing = false;
    this.cities = [];
  }

  saveHotel(): void {
    const request = this.isEditing
      ? this.hotelService.updateHotel(this.selectedHotel)
      : this.hotelService.createHotel(this.selectedHotel);

    request.subscribe({
      next: () => {
        this.newHotel();
        this.loadHotels();
      }
    });
  }

  deleteHotel(id: number): void {
    this.hotelService.deleteHotel(id).subscribe({
      next: () => this.loadHotels()
    });
  }

  openUploadPopup(hotel: Hotel): void {
    this.selectedHotelForImages = hotel;
    this.selectedFiles = [];
    this.imagesErrorMessage = '';
    this.isUploadPopupVisible = true;
  }

  closeUploadPopup(): void {
    this.isUploadPopupVisible = false;
    this.selectedFiles = [];
    this.imagesErrorMessage = '';
  }

  openGalleryPopup(hotel: Hotel): void {
    this.selectedHotelForImages = hotel;
    this.imagesErrorMessage = '';
    this.isGalleryPopupVisible = true;
    this.loadHotelImages(hotel.id);
  }

  closeGalleryPopup(): void {
    this.isGalleryPopupVisible = false;
    this.hotelImages = [];
    this.imagesErrorMessage = '';
  }

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files) {
      return;
    }

    this.addFiles(input.files);
    input.value = '';
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    if (!event.dataTransfer?.files?.length) {
      return;
    }

    this.addFiles(event.dataTransfer.files);
  }

  removeSelectedFile(index: number): void {
    this.selectedFiles.splice(index, 1);
  }

  uploadSelectedImages(): void {
    if (!this.selectedHotelForImages || this.selectedFiles.length === 0) {
      this.imagesErrorMessage = 'Seleccioná al menos una imagen.';
      return;
    }

    this.imagesErrorMessage = '';
    this.isUploadingImages = true;

    this.hotelService.uploadHotelImages(this.selectedHotelForImages.id, this.selectedFiles).subscribe({
      next: () => {
        this.isUploadingImages = false;
        this.closeUploadPopup();
      },
      error: () => {
        this.isUploadingImages = false;
        this.imagesErrorMessage = 'No se pudieron subir las imágenes.';
      }
    });
  }

  getImageSource(path: string): string {
    if (!path) {
      return '';
    }

    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }

    return `${window.location.origin}${path}`;
  }

  private loadHotelImages(hotelId: number): void {
    this.isLoadingImages = true;
    this.hotelService.getHotelImages(hotelId).subscribe({
      next: (images) => {
        this.hotelImages = images ?? [];
        this.isLoadingImages = false;
      },
      error: () => {
        this.hotelImages = [];
        this.isLoadingImages = false;
        this.imagesErrorMessage = 'No se pudieron cargar las imágenes.';
      }
    });
  }

  private addFiles(fileList: FileList): void {
    Array.from(fileList).forEach(file => {
      const alreadyExists = this.selectedFiles.some(existing => existing.name === file.name && existing.size === file.size);
      if (!alreadyExists) {
        this.selectedFiles.push(file);
      }
    });
  }

  private createEmptyHotel(): Hotel {
    return {
      id: 0,
      name: '',
      description: '',
      metaDescription: '',
      addressHotel: { id: 0, idHotel: 0, street: '', number: '', postalCode: '', latitud: '', longitud: '', idCity: 0, idProvince: 0 },
      email: '',
      codeArea: 0,
      phoneNumber: 0,
      postalCode: '',
      latitud: '',
      longitud: '',
      pictures: []
    };
  }
}
