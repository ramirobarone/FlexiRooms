import { RoomDto } from "src/models/bookingDto";
import { BillingData } from "../../billing-data/models/billingData";
import { CreditCard } from "../../checkout/Models/creditCard";

export class CheckOut{
    billingData : BillingData | undefined;
    roomDto : RoomDto | undefined;
    creditCard : CreditCard | undefined;
}