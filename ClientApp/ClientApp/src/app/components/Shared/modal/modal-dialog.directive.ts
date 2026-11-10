import {
  AfterViewInit, Directive, ElementRef, EventEmitter, HostListener, Input, OnChanges, OnDestroy, Output
} from '@angular/core';
import { SPRING_DEFAULT, SPRING_SHEET, Spring, projectMomentum, rubberband } from './spring';

type ModalState = 'closed' | 'open' | 'closing';

const EXIT_EASING = 'cubic-bezier(0.55, 0, 0.75, 0.2)';

/**
 * Turns a native <dialog> into a modal that follows the apple-design skill:
 *  - opens with showModal(): real focus trap, top layer, native ::backdrop
 *  - Escape / backdrop click / close buttons all go through one exit path
 *  - exits along the same path it entered (symmetric motion)
 *  - on phones it becomes a bottom sheet: 1:1 drag from the header, rubber-band
 *    upwards, momentum projection on release, spring with velocity handoff
 *  - honours prefers-reduced-motion (cross-fade instead of motion)
 *
 * Usage:
 *   <dialog [appModal]="isOpen" (dismiss)="isOpen = false" #m="appModal">
 *     <button (click)="m.requestClose()">Cerrar</button>
 *   </dialog>
 *
 * `dismiss` fires only after the exit animation, so content bound to the
 * parent's state stays on screen while the dialog leaves.
 */
@Directive({
  selector: 'dialog[appModal]',
  exportAs: 'appModal',
  standalone: false,
  host: { class: 'app-modal' }
})
export class ModalDialogDirective implements OnChanges, AfterViewInit, OnDestroy {
  @Input('appModal') open = false;
  @Output() dismiss = new EventEmitter<void>();

  private readonly dialog: HTMLDialogElement;
  private readonly spring: Spring;
  private readonly sheetQuery = window.matchMedia('(max-width: 575.98px)');
  private readonly reducedMotionQuery = window.matchMedia('(prefers-reduced-motion: reduce)');

  private state: ModalState = 'closed';
  private viewReady = false;
  private isSheet = false;
  private grabber?: HTMLElement;

  // Drag tracking
  private dragging = false;
  private pointerId = -1;
  private startY = 0;
  private startValue = 0;
  private samples: { t: number; y: number }[] = [];

  constructor(host: ElementRef<HTMLDialogElement>) {
    this.dialog = host.nativeElement;
    this.spring = new Spring(0, value => this.render(value));
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.sync();
  }

  ngOnChanges(): void {
    if (this.viewReady) {
      this.sync();
    }
  }

  ngOnDestroy(): void {
    this.spring.stop();
    if (this.dialog.open) {
      this.dialog.close();
    }
  }

  /** User-initiated close: animate out, then tell the parent. */
  requestClose(): void {
    if (this.state !== 'open') {
      return;
    }
    this.animateOut(0, () => this.finishClose(true));
  }

  // ---------------------------------------------------------------- lifecycle

  private sync(): void {
    if (this.open && this.state === 'closed') {
      this.show();
    } else if (!this.open && this.state === 'open') {
      // Parent closed it (e.g. after a successful upload): animate, no emit.
      this.animateOut(0, () => this.finishClose(false));
    }
  }

  private show(): void {
    this.isSheet = this.sheetQuery.matches;
    this.dialog.classList.toggle('is-sheet', this.isSheet);
    this.ensureGrabber();
    this.dialog.showModal();
    this.state = 'open';

    if (!this.isSheet) {
      // Desktop enter is the CSS `materialize-in` keyframe on dialog[open].
      return;
    }

    if (this.reducedMotion) {
      this.spring.set(0);
      this.fade(0, 1);
      return;
    }

    // Sheet rises from the bottom edge: the same path it will leave by.
    this.spring.set(this.sheetHeight);
    this.spring.animateTo(0, { config: SPRING_DEFAULT });
  }

  private animateOut(velocity: number, done: () => void): void {
    this.state = 'closing';

    if (this.reducedMotion) {
      this.fade(1, 0).then(done);
      return;
    }

    if (this.isSheet) {
      this.spring.animateTo(this.sheetHeight, { velocity, config: SPRING_DEFAULT, onRest: done });
      return;
    }

    // Desktop: reverse of materialize-in (scale + blur + fade together).
    const animation = this.dialog.animate(
      [
        { opacity: 1, scale: '1', filter: 'blur(0)' },
        { opacity: 0, scale: '0.96', filter: 'blur(4px)' }
      ],
      { duration: 160, easing: EXIT_EASING, fill: 'forwards' }
    );
    this.fadeBackdrop(160);
    animation.finished.then(done, done);
  }

  private finishClose(emit: boolean): void {
    this.spring.stop();
    this.dialog.getAnimations().forEach(a => a.cancel());
    this.dialog.style.removeProperty('transform');
    this.dialog.style.removeProperty('--backdrop-opacity');
    this.state = 'closed';
    if (this.dialog.open) {
      this.dialog.close();
    }
    if (emit) {
      this.dismiss.emit();
    }
  }

  // ------------------------------------------------------------------- events

  /** Escape key: route through the animated exit instead of an instant close. */
  @HostListener('cancel', ['$event'])
  onCancel(event: Event): void {
    event.preventDefault();
    this.requestClose();
  }

  /** Closed by something else (e.g. form method="dialog"). Keep parent in sync. */
  @HostListener('close')
  onNativeClose(): void {
    if (this.state !== 'closed') {
      this.finishClose(true);
    }
  }

  /** Click on the ::backdrop (the dialog element itself, outside its box). */
  @HostListener('click', ['$event'])
  onClick(event: MouseEvent): void {
    if (event.target !== this.dialog || this.dragging) {
      return;
    }
    const r = this.dialog.getBoundingClientRect();
    const inside = event.clientX >= r.left && event.clientX <= r.right
      && event.clientY >= r.top && event.clientY <= r.bottom;
    if (!inside) {
      this.requestClose();
    }
  }

  @HostListener('pointerdown', ['$event'])
  onPointerDown(event: PointerEvent): void {
    if (!this.isSheet || this.state !== 'open' || event.button !== 0) {
      return;
    }
    const target = event.target as HTMLElement;
    const onHandle = target.closest('.sheet-grabber, .modal-header');
    const onControl = target.closest('button, a, input, select, textarea, label');
    if (!onHandle || onControl) {
      return;
    }

    // Grab from the presentation value: interrupt any running spring.
    this.spring.stop();
    this.dragging = true;
    this.pointerId = event.pointerId;
    this.dialog.setPointerCapture(event.pointerId);
    this.startY = event.clientY;
    this.startValue = this.spring.value;
    this.samples = [{ t: event.timeStamp, y: this.spring.value }];
  }

  @HostListener('pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (!this.dragging || event.pointerId !== this.pointerId) {
      return;
    }
    const raw = this.startValue + (event.clientY - this.startY);
    // Downwards tracks 1:1; upwards resists progressively.
    const value = raw < 0 ? rubberband(raw, this.sheetHeight) : raw;
    this.spring.set(value);

    this.samples.push({ t: event.timeStamp, y: value });
    const cutoff = event.timeStamp - 100;
    while (this.samples.length > 2 && this.samples[0].t < cutoff) {
      this.samples.shift();
    }
  }

  @HostListener('pointerup', ['$event'])
  @HostListener('pointercancel', ['$event'])
  onPointerUp(event: PointerEvent): void {
    if (!this.dragging || event.pointerId !== this.pointerId) {
      return;
    }
    this.dragging = false;
    if (this.dialog.hasPointerCapture(event.pointerId)) {
      this.dialog.releasePointerCapture(event.pointerId);
    }

    const velocity = this.releaseVelocity();
    const current = this.spring.value;
    // Decide from where the gesture is *going*, not where it was released.
    const projected = current + projectMomentum(velocity);

    if (projected > this.sheetHeight * 0.5 && velocity >= 0) {
      this.animateOut(velocity, () => this.finishClose(true));
    } else {
      // Snap back, inheriting the finger's velocity; a little bounce is earned here.
      this.spring.animateTo(0, { velocity, config: SPRING_SHEET });
    }
  }

  // ------------------------------------------------------------------ helpers

  private render(value: number): void {
    this.dialog.style.transform = value === 0 ? '' : `translate3d(0, ${value}px, 0)`;
    const progress = 1 - Math.min(Math.max(value / this.sheetHeight, 0), 1);
    this.dialog.style.setProperty('--backdrop-opacity', progress.toFixed(3));
  }

  private releaseVelocity(): number {
    if (this.samples.length < 2) {
      return 0;
    }
    const first = this.samples[0];
    const last = this.samples[this.samples.length - 1];
    const dt = (last.t - first.t) / 1000;
    return dt > 0 ? (last.y - first.y) / dt : 0;
  }

  private ensureGrabber(): void {
    if (this.isSheet && !this.grabber) {
      this.grabber = document.createElement('div');
      this.grabber.className = 'sheet-grabber';
      this.grabber.setAttribute('aria-hidden', 'true');
      this.dialog.prepend(this.grabber);
    } else if (!this.isSheet && this.grabber) {
      this.grabber.remove();
      this.grabber = undefined;
    }
  }

  private fade(from: number, to: number): Promise<unknown> {
    return this.dialog
      .animate([{ opacity: from }, { opacity: to }], { duration: 150, easing: 'ease', fill: 'forwards' })
      .finished.catch(() => undefined);
  }

  private fadeBackdrop(duration: number): void {
    try {
      this.dialog.animate([{ opacity: 1 }, { opacity: 0 }], {
        duration, easing: EXIT_EASING, fill: 'forwards', pseudoElement: '::backdrop'
      });
    } catch {
      // ::backdrop animation not supported: the dialog fade alone is fine.
    }
  }

  private get sheetHeight(): number {
    return this.dialog.getBoundingClientRect().height || window.innerHeight * 0.6;
  }

  private get reducedMotion(): boolean {
    return this.reducedMotionQuery.matches;
  }
}
