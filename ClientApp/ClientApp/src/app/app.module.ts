import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { authInterceptor } from './ServicesShared/auth.interceptor';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/home/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { CardRoomComponent } from './components/bookings/card-room/card-room.component';
import { SearchBarComponent } from './components/home/search-bar/search-bar.component';
import { CardHotelComponent } from './components/home/card-hotel/card-hotel.component';
import { FooterComponent } from './components/home/footer/footer.component';
import { ReservedComponent } from './components/reserved/reserved.component';
import { BodyPrincipalComponent } from './components/home/body-principal/body-principal.component';
import { PopupComponent } from './components/Shared/popup/popup.component';
import { LoginComponent } from './components/Account/login/login.component';
import { AccountCreateComponent } from './components/Account/account-create/account-create.component';
import { LogoutComponent } from './components/Account/logout/logout.component';
import { BookingsComponent } from './components/bookings/bookings/bookings.component';
import { ControlRoomComponent } from './components/Account/control-room/control-room.component';
import { BillingDataComponent } from './components/bookings/billing-data/billing-data.component';
import { CheckoutComponent } from './components/bookings/checkout/checkout.component';
import { StepsCheckoutComponent } from './components/bookings/steps-checkout/steps-checkout.component';
import { AdminHotelsComponent } from './components/Account/admin-hotels/admin-hotels.component';
import { AdminBookingsComponent } from './components/Account/admin-bookings/admin-bookings.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CardRoomComponent,
    SearchBarComponent,
    CardHotelComponent,
    FooterComponent,
    ReservedComponent,
    BodyPrincipalComponent,
    PopupComponent,
    CheckoutComponent,
    LoginComponent,
    AccountCreateComponent,
    LogoutComponent,
    BookingsComponent,
    ControlRoomComponent,
    BillingDataComponent,
    StepsCheckoutComponent,
    AdminHotelsComponent,
    AdminBookingsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'room/:id', component: CardRoomComponent, pathMatch: 'full' },
      { path: 'reserve/:id', component: ReservedComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent, pathMatch: 'full' },
      { path: 'accountCreate', component: AccountCreateComponent, pathMatch: 'full' },
      { path: 'logout', component: LogoutComponent, pathMatch: 'full' },
      { path: 'steps/:id', component: StepsCheckoutComponent },
      { path: 'admin', component: ControlRoomComponent, pathMatch: 'full' }
    ])
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
