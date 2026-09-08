import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from '@angular/core';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.css']
})
export class PopupComponent implements AfterViewInit, OnChanges {

  @Input() title = '';
  @Input() description = '';
  @Input() visible = false;
  @Output() closed = new EventEmitter<void>();
  @ViewChild('genericModal') genericModal?: ElementRef<HTMLDialogElement>;

  ngAfterViewInit(): void {
    this.updateVisibility();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible']) {
      this.updateVisibility();
    }
  }

  close(): void {
    this.genericModal?.nativeElement.close();
  }

  onClose(): void {
    this.closed.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    const dialog = this.genericModal?.nativeElement;
    if (!dialog || event.target !== dialog) {
      return;
    }

    const bounds = dialog.getBoundingClientRect();
    const clickedInside = bounds.top <= event.clientY && event.clientY <= bounds.bottom
      && bounds.left <= event.clientX && event.clientX <= bounds.right;

    if (!clickedInside) {
      dialog.close();
    }
  }

  private updateVisibility(): void {
    const dialog = this.genericModal?.nativeElement;
    if (!dialog) {
      return;
    }

    if (this.visible && !dialog.open) {
      dialog.showModal();
    }

    if (!this.visible && dialog.open) {
      dialog.close();
    }
  }
}
