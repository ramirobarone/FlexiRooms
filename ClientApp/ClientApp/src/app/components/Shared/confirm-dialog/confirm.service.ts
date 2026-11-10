import { Injectable, signal } from '@angular/core';

export interface ConfirmOptions {
  /** Short question, e.g. "¿Eliminar este hotel?" */
  title: string;
  /** What exactly will happen, in plain words. */
  message?: string;
  /** Verb for the action button. Be specific ("Eliminar"), never "Aceptar". */
  confirmText?: string;
  cancelText?: string;
  /** Destructive actions render the action in red and never get default focus. */
  destructive?: boolean;
}

export interface ConfirmRequest extends Required<Omit<ConfirmOptions, 'message'>> {
  message: string;
}

/**
 * App-wide replacement for window.confirm(), rendered by <app-confirm-dialog>
 * (mounted once in AppComponent).
 *
 *   if (await this.confirmService.confirm({ title: '¿Eliminar?', destructive: true })) { ... }
 */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  readonly request = signal<ConfirmRequest | null>(null);
  private resolver?: (result: boolean) => void;

  confirm(options: ConfirmOptions): Promise<boolean> {
    // Only one question at a time: a new one cancels the previous.
    this.resolver?.(false);

    this.request.set({
      title: options.title,
      message: options.message ?? '',
      confirmText: options.confirmText ?? 'Confirmar',
      cancelText: options.cancelText ?? 'Cancelar',
      destructive: options.destructive ?? false
    });

    return new Promise<boolean>(resolve => (this.resolver = resolve));
  }

  /** Called by the dialog once its exit animation has finished. */
  settle(result: boolean): void {
    const resolve = this.resolver;
    this.resolver = undefined;
    this.request.set(null);
    resolve?.(result);
  }
}
