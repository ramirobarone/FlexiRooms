import { Component, OnInit } from '@angular/core';
import { Room } from '../../../../models/room';
import { Hotel } from '../../../../models/hotel';
import { RoomService } from '../../bookings/card-room/service/room.service';
import { HotelService } from '../../../../services/HotelService/hotel.service';

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

  constructor(
    private roomService: RoomService,
    private hotelService: HotelService
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
