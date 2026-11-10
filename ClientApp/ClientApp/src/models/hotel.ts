import { Address } from "./Address";
import { hotelPicture } from "./hotelPicture";

export interface Hotel {
  id: number;
  name: string;
  description: string;
  metaDescription: string;
  addressHotel: Address
  email: string;
  codeArea: number;
  phoneNumber: number;
  postalCode: string;
  latitud: string;
  longitud: string;
  reviewScore?: number;
  pictures: hotelPicture[];
  /** Set by the server. Only Approved hotels are listed publicly and take payments. */
  approvalStatus?: HotelApprovalStatus;
  rejectionReason?: string | null;
}

export type HotelApprovalStatus = 'Pending' | 'Approved' | 'Rejected';

/** Response of POST Hotel/CreateHotel: includes a fresh login carrying the new Owner role. */
export interface CreateHotelResponse {
  hotelId: number;
  approvalStatus: HotelApprovalStatus;
  login: { fullName: string; token: string; rolId: number; role: string; userGuid: string } | null;
}

export interface PendingHotel {
  id: number;
  name: string | null;
  description: string | null;
  email: string | null;
  codeArea: number;
  phoneNumber: number;
  street: string | null;
  streetNumber: string | null;
  ownerName: string | null;
  ownerEmail: string | null;
  submittedAtUtc: string | null;
}
