import { Component, OnInit, ChangeDetectionStrategy, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmService } from '../../Shared/confirm-dialog/confirm.service';
import { LocalStorageService } from '../../../ServicesShared/local-storage.service';
import { HotelPaymentAccountStatus, MercadoPagoAccountService } from '../../../../services/MercadoPagoAccountService/mercado-pago-account.service';

type PaymentNotice = { kind: 'success' | 'warning' | 'danger'; text: string };
import { Hotel } from '../../../../models/hotel';
import { Province } from '../../../../models/Province';
import { City } from '../../../../models/City';
import { hotelPicture } from '../../../../models/hotelPicture';
import { HotelService } from '../../../../services/HotelService/hotel.service';
import { GeographyService } from '../../../../services/GeographyService/geography.service';

@Component({
    selector: 'app-admin-hotels',
    templateUrl: './admin-hotels.component.html',
    styleUrls: ['./admin-hotels.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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

  // Mercado Pago Marketplace: each hotel collects with its own account
  paymentAccounts = new Map<number, HotelPaymentAccountStatus>();
  marketplaceConfigured = true;
  connectingHotelId: number | null = null;
  paymentNotice: PaymentNotice | null = null;
  private pendingReturnHotelId: number | null = null;

  private readonly confirmService = inject(ConfirmService);
  private readonly mercadoPagoAccounts = inject(MercadoPagoAccountService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly localStorage = inject(LocalStorageService);

  constructor(private hotelService: HotelService, private geographyService: GeographyService) { }

  ngOnInit(): void {
    this.readMercadoPagoReturn();
    this.loadHotels();
    this.loadProvinces();
    this.loadPaymentAccounts();
  }

  loadHotels(): void {
    this.hotelService.getMyHotels().subscribe({
      next: (response) => {
        this.hotels = response ?? [];
        // Back from Mercado Pago: reopen the hotel that was being configured.
        const returned = this.hotels.find(hotel => hotel.id === this.pendingReturnHotelId);
        if (returned) {
          this.editHotel(returned);
        }
        this.pendingReturnHotelId = null;
      },
      error: () => this.hotels = []
    });
  }

  // ------------------------------------------------------------ Mercado Pago

  paymentStatus(hotelId: number): HotelPaymentAccountStatus | undefined {
    return this.paymentAccounts.get(hotelId);
  }

  loadPaymentAccounts(): void {
    this.mercadoPagoAccounts.getMyAccounts().subscribe({
      next: (response) => {
        this.marketplaceConfigured = response?.marketplaceConfigured ?? false;
        this.paymentAccounts = new Map((response?.hotels ?? []).map(status => [status.hotelId, status]));
      },
      error: () => this.paymentAccounts = new Map()
    });
  }

  /** Leaves the app for Mercado Pago's authorization page; it redirects back to /admin. */
  connectPayments(hotel: Hotel): void {
    this.connectingHotelId = hotel.id;
    this.paymentNotice = null;

    this.mercadoPagoAccounts.connect(hotel.id).subscribe({
      next: ({ authorizationUrl }) => window.location.assign(authorizationUrl),
      error: (error) => {
        this.connectingHotelId = null;
        this.paymentNotice = {
          kind: 'danger',
          text: error?.status === 503
            ? 'Mercado Pago todavía no está configurado en el servidor.'
            : 'No pudimos iniciar la conexión con Mercado Pago. Volvé a intentarlo.'
        };
      }
    });
  }

  async disconnectPayments(hotel: Hotel): Promise<void> {
    const confirmed = await this.confirmService.confirm({
      title: `¿Desconectar Mercado Pago de "${hotel.name}"?`,
      message: 'Los huéspedes no van a poder pagar online las reservas de este hotel hasta que lo vuelvas a conectar.',
      confirmText: 'Desconectar',
      destructive: true
    });
    if (!confirmed) {
      return;
    }

    this.mercadoPagoAccounts.disconnect(hotel.id).subscribe({
      next: () => {
        this.paymentNotice = { kind: 'warning', text: `"${hotel.name}" ya no recibe pagos online.` };
        this.loadPaymentAccounts();
      },
      error: () => this.paymentNotice = { kind: 'danger', text: 'No pudimos desconectar la cuenta. Volvé a intentarlo.' }
    });
  }

  dismissPaymentNotice(): void {
    this.paymentNotice = null;
  }

  private readMercadoPagoReturn(): void {
    const params = this.route.snapshot.queryParamMap;

    // Just registered through "Sumá tu hotel": open it and point to the next step.
    const newHotelId = Number(params.get('nuevo'));
    if (Number.isFinite(newHotelId) && newHotelId > 0) {
      this.pendingReturnHotelId = newHotelId;
      this.paymentNotice = {
        kind: 'success',
        text: '¡Tu hotel quedó registrado! Lo estamos revisando para publicarlo. Mientras tanto, conectá Mercado Pago y cargá tus habitaciones.'
      };
      this.router.navigate([], { relativeTo: this.route, queryParams: {}, replaceUrl: true });
      return;
    }

    const result = params.get('mp');
    if (!result) {
      return;
    }

    const hotelId = Number(params.get('hotelId'));
    this.pendingReturnHotelId = Number.isFinite(hotelId) && hotelId > 0 ? hotelId : null;

    const notices: Record<string, PaymentNotice> = {
      connected: { kind: 'success', text: 'Listo: el hotel ya cobra las reservas con su propia cuenta de Mercado Pago.' },
      cancelled: { kind: 'warning', text: 'Se canceló la conexión con Mercado Pago. Podés volver a intentarlo cuando quieras.' },
      forbidden: { kind: 'danger', text: 'No tenés permisos para configurar los cobros de ese hotel.' },
      error: { kind: 'danger', text: 'No pudimos conectar la cuenta de Mercado Pago. Volvé a intentarlo.' }
    };
    this.paymentNotice = notices[result] ?? notices['error'];

    // Clean the URL so a reload doesn't repeat the message.
    this.router.navigate([], { relativeTo: this.route, queryParams: {}, replaceUrl: true });
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
    if (this.isEditing) {
      const wasRejected = this.selectedHotel.approvalStatus === 'Rejected';
      this.hotelService.updateHotel(this.selectedHotel).subscribe({
        next: () => {
          if (wasRejected) {
            this.paymentNotice = { kind: 'success', text: 'Guardamos los cambios y enviamos el hotel otra vez a revisión.' };
          }
          this.newHotel();
          this.loadHotels();
        }
      });
      return;
    }

    this.hotelService.createHotel(this.selectedHotel).subscribe({
      next: (response) => {
        // Creating a hotel makes the user its Owner: keep the refreshed token.
        if (response?.login) {
          this.localStorage.saveLogin(response.login);
          dispatchEvent(new Event('refrescar'));
        }
        if (response?.approvalStatus === 'Pending') {
          this.paymentNotice = { kind: 'success', text: 'Hotel registrado. Lo vamos a revisar antes de publicarlo.' };
        }
        this.newHotel();
        this.loadHotels();
        this.loadPaymentAccounts();
      }
    });
  }

  approvalLabel(hotel: Hotel): string | null {
    switch (hotel.approvalStatus) {
      case 'Pending': return 'En revisión';
      case 'Rejected': return 'Rechazado';
      default: return null;
    }
  }

  async deleteHotel(id: number): Promise<void> {
    // Destructive and irreversible: confirm, naming exactly what will be lost.
    const name = this.hotels.find(hotel => hotel.id === id)?.name ?? 'este hotel';
    const confirmed = await this.confirmService.confirm({
      title: `¿Eliminar "${name}"?`,
      message: 'Esta acción no se puede deshacer.',
      confirmText: 'Eliminar',
      destructive: true
    });
    if (!confirmed) {
      return;
    }

    this.hotelService.deleteHotel(id).subscribe({
      next: () => {
        if (this.isEditing && this.selectedHotel.id === id) {
          this.newHotel();
        }
        this.loadHotels();
      }
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
