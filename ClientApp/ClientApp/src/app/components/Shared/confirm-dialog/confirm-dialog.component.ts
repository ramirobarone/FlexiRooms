import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { ModalDialogDirective } from '../modal/modal-dialog.directive';
import { ConfirmService } from './confirm.service';

/**
 * Single confirmation dialog for the whole app (see ConfirmService).
 *
 * Follows the apple-design skill for alerts:
 *  - specific verb on the action button, not "Aceptar"
 *  - destructive action in red, and the *safe* choice (Cancelar) has focus
 *  - Escape / backdrop / dragging the sheet down all mean "cancel"
 *  - the promise resolves after the exit animation, so the list updates
 *    once the dialog is gone instead of behind it
 */
@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: false
})
export class ConfirmDialogComponent {
  readonly confirmService = inject(ConfirmService);
  private choice = false;

  choose(result: boolean, modal: ModalDialogDirective): void {
    this.choice = result;
    modal.requestClose();
  }

  onDismiss(): void {
    const result = this.choice;
    this.choice = false; // anything else (Escape, backdrop, drag) is a cancel
    this.confirmService.settle(result);
  }
}
