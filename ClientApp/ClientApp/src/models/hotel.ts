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
  pictures: hotelPicture[]

}
