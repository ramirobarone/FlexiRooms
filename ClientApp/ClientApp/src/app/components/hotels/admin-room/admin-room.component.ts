import { Component, OnInit } from '@angular/core';
import { Room } from '../../../../models/room';
import { Hotel } from '../../../../models/hotel';
import { roomPictures } from '../../../../models/roomImages';
import { RoomService } from '../../bookings/card-room/service/room.service';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { RoomImageService } from '../../../../services/RoomImageService/room-image.service';

@Component({
  selector: 'app-admin-room',
  templateUrl: './admin-room.component.html',
  styleUrls: ['./admin-room.component.css']
})
export class AdminRoomComponent implements OnInit {
  hotels: Hotel[] = [];
  selectedHotelId: number = 0;
  rooms: Room[] = [];
  selectedRoom: Room = this.createEmptyRoom();
  isEditing = false;
  imagePath: string = '';

  selectedRoomForImages: Room | null = null;
  selectedFiles: File[] = [];
  roomImages: roomPictures[] = [];
  isUploadPopupVisible = false;
  isGalleryPopupVisible = false;
  isUploadingImages = false;
  isLoadingImages = false;
  imagesErrorMessage = '';
  isDragOver = false;

  constructor(
    private roomService: RoomService,
    private hotelService: HotelService,
    private roomImageService: RoomImageService
  ) {}

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.hotelService.getMyHotels().subscribe({
      next: (response) => {
        this.hotels = response ?? [];
        if (this.hotels.length > 0) {
          this.selectedHotelId = this.hotels[0].id;
          this.loadRooms();
        }
      },
      error: () => this.hotels = []
    });
  }

  onHotelChange(): void {
    this.newRoom();
    this.loadRooms();
  }

  loadRooms(): void {
    if (!this.selectedHotelId) {
      this.rooms = [];
      return;
    }
    this.roomService.getRooms(this.selectedHotelId).subscribe({
      next: (response) => this.rooms = response ?? [],
      error: () => this.rooms = []
    });
  }

  newRoom(): void {
    this.selectedRoom = this.createEmptyRoom();
    this.imagePath = '';
    this.isEditing = false;
  }

  editRoom(room: Room): void {
    this.selectedRoom = JSON.parse(JSON.stringify(room));
    if (!this.selectedRoom.cost) {
      this.selectedRoom.cost = { id: 0, idRoom: room.id, costPerHour: 0, hour: 0 };
    }
    this.imagePath = (this.selectedRoom.roomPictures && this.selectedRoom.roomPictures.length > 0)
      ? this.selectedRoom.roomPictures[0].path
      : this.selectedRoom.path || '';
    this.isEditing = true;
  }

  saveRoom(): void {
    if (!this.selectedHotelId) {
      alert('Por favor, seleccione un hotel primero.');
      return;
    }

    if (this.imagePath) {
      this.selectedRoom.path = this.imagePath;
      this.selectedRoom.roomPictures = [{ id: 0, path: this.imagePath }];
    } else {
      this.selectedRoom.roomPictures = [];
    }

    if (this.isEditing) {
      this.roomService.updateRoom(this.selectedRoom).subscribe({
        next: () => {
          this.newRoom();
          this.loadRooms();
        }
      });
    } else {
      const roomToCreate = { ...this.selectedRoom, id: this.selectedHotelId };
      this.roomService.createRoom(roomToCreate).subscribe({
        next: () => {
          this.newRoom();
          this.loadRooms();
        }
      });
    }
  }

  deleteRoom(id: number): void {
    if (confirm('¿Está seguro de eliminar esta habitación?')) {
      this.roomService.deleteRoom(id).subscribe({
        next: () => this.loadRooms()
      });
    }
  }

  openUploadPopup(room: Room): void {
    this.selectedRoomForImages = room;
    this.selectedFiles = [];
    this.imagesErrorMessage = '';
    this.isDragOver = false;
    this.isUploadPopupVisible = true;
  }

  closeUploadPopup(): void {
    this.isUploadPopupVisible = false;
    this.selectedFiles = [];
    this.imagesErrorMessage = '';
    this.isDragOver = false;
  }

  openGalleryPopup(room: Room): void {
    this.selectedRoomForImages = room;
    this.imagesErrorMessage = '';
    this.isGalleryPopupVisible = true;
    this.loadRoomImages(room.id);
  }

  closeGalleryPopup(): void {
    this.isGalleryPopupVisible = false;
    this.roomImages = [];
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
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
    if (!event.dataTransfer?.files?.length) {
      return;
    }

    this.addFiles(event.dataTransfer.files);
  }

  addFiles(files: FileList): void {
    const validFiles = Array.from(files).filter(file => file.type.startsWith('image/'));
    if (validFiles.length === 0) {
      this.imagesErrorMessage = 'Por favor, seleccione archivos de imagen válidos (.jpg, .png, .webp).';
      return;
    }

    this.imagesErrorMessage = '';
    this.selectedFiles.push(...validFiles);
  }

  removeSelectedFile(index: number): void {
    this.selectedFiles.splice(index, 1);
  }

  uploadSelectedImages(): void {
    if (!this.selectedRoomForImages || this.selectedFiles.length === 0) {
      this.imagesErrorMessage = 'Seleccioná al menos una imagen.';
      return;
    }

    this.imagesErrorMessage = '';
    this.isUploadingImages = true;

    this.roomImageService.uploadRoomImages(this.selectedRoomForImages.id, this.selectedFiles).subscribe({
      next: () => {
        this.isUploadingImages = false;
        this.closeUploadPopup();
        this.loadRooms();
      },
      error: () => {
        this.isUploadingImages = false;
        this.imagesErrorMessage = 'No se pudieron subir las imágenes.';
      }
    });
  }

  deleteRoomImage(imageId: number): void {
    if (!this.selectedRoomForImages) return;

    if (confirm('¿Está seguro de eliminar esta imagen?')) {
      this.roomImageService.deleteRoomImage(this.selectedRoomForImages.id, imageId).subscribe({
        next: () => {
          if (this.selectedRoomForImages) {
            this.loadRoomImages(this.selectedRoomForImages.id);
            this.loadRooms();
          }
        },
        error: () => {
          this.imagesErrorMessage = 'No se pudo eliminar la imagen.';
        }
      });
    }
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

  private loadRoomImages(roomId: number): void {
    this.isLoadingImages = true;
    this.roomImageService.getRoomImages(roomId).subscribe({
      next: (images) => {
        this.roomImages = images ?? [];
        this.isLoadingImages = false;
      },
      error: () => {
        this.roomImages = [];
        this.isLoadingImages = false;
        this.imagesErrorMessage = 'No se pudieron cargar las imágenes de la habitación.';
      }
    });
  }

  private createEmptyRoom(): Room {
    return {
      id: 0,
      name: '',
      description: '',
      path: '',
      bedNumbers: 1,
      avialableNow: true,
      cost: { id: 0, idRoom: 0, costPerHour: 0, hour: 1 },
      roomPictures: []
    };
  }
}
