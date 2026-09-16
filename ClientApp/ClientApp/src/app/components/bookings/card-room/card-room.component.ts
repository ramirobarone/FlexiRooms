import { AvialableRoomsService } from 'src/services/AvialableRooms/avialable-rooms.service';
import { Component, Injectable, Input, Output, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Room } from 'src/models/room';
import { Cost } from 'src/models/Cost';
import { RoomDto } from 'src/models/bookingDto';
import { Schedule } from 'src/models/Schedule';
import { roomPictures } from 'src/models/roomImages';
import { EventEmitter } from '@angular/core';
import { LocalDataBookingService } from '../local-data-booking.service';

@Component({
    selector: 'app-card-room',
    templateUrl: './card-room.component.html',
    styleUrls: ['./card-room.component.css'],
    standalone: false
})
export class CardRoomComponent implements OnInit {


  idHotel: string | undefined = '';
  name: string = '';
  description: string = '';
  path: string = '';
  roompictures: roomPictures[] = [];
  selectedRoom: any;
  selectedDate: any;
  timeSelected: any;
  currenRoom: any;
  _timesEntry: any;
  showRoomMessage = false;
  CheckTemporalAvaiabilityRoomMessage = false;
  disabledTime = true;
  disabledDate = true;
  btnBookingDisabled = true;
  showCheckout = false;
  checkoutTime = '';
  bedNumbers = 0;

  times: Schedule[] = [];
  _rooms: Room[] = [];
  _booking: RoomDto | undefined;
  costs: Cost[] = [];
  cost: Cost = { id: 0, idRoom: 0, costPerHour: 0, hour: 0 };
  selectedCostId: any;


  @Output() nextToBillingData: EventEmitter<any> = new EventEmitter();

  constructor(private roomService: AvialableRoomsService,
    private route: ActivatedRoute,
    private localDataBookin: LocalDataBookingService,
    private router: Router) {
    this.idHotel = this.route.snapshot.paramMap.get('id')?.toString();
  }
  
  ngOnInit(): void {
    this.getRoom(this.idHotel);
  }


  getRoom(idHotel: string | undefined): void {
    console.log('idHotel', idHotel);
    this.roomService.getRoom(idHotel).subscribe(res => {
      this._rooms = res;
      this.selectFirst();

      // con una sola habitación el select no dispara (change), hay que habilitar la fecha manualmente
      if (this._rooms.length === 1) {
        this.fillRoomFinded(this._rooms[0].id);
      }
    });
  }
  onChange() {
    this.fillRoomFinded(this.currenRoom)
  }

  selectFirst(): void {
    if (this._rooms.length > 0) {
      this.selectedRoom = this._rooms[0];
      this.currenRoom = this._rooms[0].id;
      this.name = this._rooms[0].name;
      this.description = this._rooms[0].description;
      this.bedNumbers = this._rooms[0].bedNumbers;
      this.selectCostOptions(this._rooms[0].costs);
      this.roompictures = this._rooms[0].roomPictures || this._rooms[0].roomPictures || [];
      if (this.roompictures.length > 0) {
        this.path = this.roompictures[0].path;
      } else {
        this.path = this._rooms[0].path || '';
      }
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

  changeDate(): void {
    this.disabledTime = this.selectedDate !== undefined;

    this.disabledTime = false;
    this.getTimesFree();
  }
  getTimesFree(): void {
    if (this.selectedDate === undefined || this.selectedRoom === undefined)
      return;

    this.roomService.getTimesFree(this.selectedRoom.id, this.selectedDate).subscribe(res => {

      this.times = res;
      this.showRoomMessage = this.times.length == 0;
      this.disabledTime = this.times.length == 0;
    });
  }

  fillRoomFinded(id: number): void {
    for (let i = 0; i < this._rooms.length; i++) {
      if (this._rooms[i].id == id) {

        this.selectedRoom = this._rooms[i];
        this.name = this._rooms[i].name;
        this.description = this._rooms[i].description;
        this.bedNumbers = this._rooms[i].bedNumbers;
        this.selectCostOptions(this._rooms[i].costs);

        this.disabledDate = false;

        this.roompictures = this._rooms[i].roomPictures || this._rooms[i].roomPictures || [];
        if (this.roompictures.length > 0) {
          this.path = this.roompictures[0].path;
        } else {
          this.path = this._rooms[i].path || '';
        }

        this.getTimesFree();
      }
    }
  }

  selectCostOptions(costs: Cost[]): void {
    this.costs = costs || [];
    this.cost = this.costs[0] || { id: 0, idRoom: 0, costPerHour: 0, hour: 0 };
    this.selectedCostId = this.cost.id;
  }

  onCostChange(): void {
    this.cost = this.costs.find(c => String(c.id) === String(this.selectedCostId)) || this.cost;
    this.SelectTime();
  }
  SelectTime(): void {
    this.btnBookingDisabled = this.timeSelected === undefined || this.timeSelected === 0;
    this.checkoutTime = '';

    const selectedSchedule = this.times.find(schedule => String(schedule.id) === String(this.timeSelected));
    if (selectedSchedule && this.cost.hour > 0) {
      this.checkoutTime = this.calculateCheckoutTime(selectedSchedule.inTime, this.cost.hour);
    }
  }

  calculateCheckoutTime(checkInTime: string, durationHours: number): string {
    const [hours, minutes] = checkInTime.split(':').map(Number);
    const totalMinutes = (hours * 60) + minutes + (durationHours * 60);
    const nextDay = totalMinutes >= 24 * 60;
    const checkoutHours = Math.floor((totalMinutes % (24 * 60)) / 60);
    const checkoutMinutes = totalMinutes % 60;
    const formattedTime = `${String(checkoutHours).padStart(2, '0')}:${String(checkoutMinutes).padStart(2, '0')}`;

    return nextDay ? `${formattedTime} del día siguiente` : formattedTime;
  }

  CreateBooking(): void {

    let userGuid = localStorage.getItem('userGuid');


    if (userGuid === null) {
      this.router.navigate(['/login']);
      return;
    }

    this.CheckTemporalAvaiabilityRoomMessage = false;

    this.roomService.CheckTemporalAvaiabilityRoom({
      IdRoom: this.selectedRoom.id,
      Date: this.selectedDate,
      CheckInTimeId: this.timeSelected,
      CostId: this.cost.id,
      userGuid: userGuid
    })
      .subscribe(
        (response: boolean) => {

          this.btnBookingDisabled = !response;
          this.CheckTemporalAvaiabilityRoomMessage = !response;

          if (response) {

            this.nextToBillingData.emit(true);

            this.btnBookingDisabled = true;
            this._booking = {
              IdRoom: this.selectedRoom.id,
              Date: this.selectedDate,
              CheckInTimeId: this.timeSelected,
              CostId: this.cost.id,
              userGuid: userGuid || ''
            };
            this.localDataBookin.setBookingDto(this._booking);
            // localStorage.setItem('roomSelected', JSON.stringify(this._booking));

          }
        }
      );
  }

  showModal(): void {
    const modalToggle = document.getElementById('toggleMyModal');
  }
}
