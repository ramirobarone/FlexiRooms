import { Component, EventEmitter, Input, Output, ChangeDetectionStrategy } from '@angular/core';

/**
 * Simple message dialog. Opening, Escape, backdrop click, focus trap and the
 * enter/exit motion are handled by the shared `appModal` directive.
 */
@Component({
    selector: 'app-popup',
    templateUrl: './popup.component.html',
    styleUrls: ['./popup.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class PopupComponent {
  @Input() title = '';
  @Input() description = '';
  @Input() visible = false;
  @Output() closed = new EventEmitter<void>();

  onClose(): void {
    this.closed.emit();
  }
}
