export interface AdminBooking {
  id: number;
  checkInTimeId: number;
  idRoom: number;
  price: number;
  userGuid: string;
  dateReserved: string;
  checkInTime?: {
    id: number;
    time: string;
  };
  user?: {
    id: number;
    name: string;
    secondName?: string;
    lastName?: string;
    email?: string;
    phoneNumber?: string;
    isAdmin: boolean;
  };
}
