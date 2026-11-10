/**
 * Minimal interruptible spring, parameterised the way Apple does it
 * (Designing Fluid Interfaces, WWDC 2018):
 *   - dampingRatio: 1 = critically damped (no overshoot), < 1 = bouncy
 *   - response: seconds to (approximately) reach the target — not a duration
 *
 * It always starts from the current value and the current velocity, so a new
 * target can be set at any moment without a visible jump (interruptibility).
 */
export interface SpringConfig {
  dampingRatio: number;
  response: number;
}

export const SPRING_DEFAULT: SpringConfig = { dampingRatio: 1, response: 0.35 };
/** Only when the gesture itself carried momentum (a flick / drag release). */
export const SPRING_SHEET: SpringConfig = { dampingRatio: 0.85, response: 0.3 };

export class Spring {
  value: number;
  velocity = 0;
  private target: number;
  private frame = 0;
  private lastTime = 0;
  private onUpdate: (value: number) => void;
  private onRest?: () => void;
  private config: SpringConfig = SPRING_DEFAULT;

  constructor(initial: number, onUpdate: (value: number) => void) {
    this.value = initial;
    this.target = initial;
    this.onUpdate = onUpdate;
  }

  get isAnimating(): boolean {
    return this.frame !== 0;
  }

  /** Animate to `target`, inheriting `velocity` (px/s) if given. */
  animateTo(target: number, options: { velocity?: number; config?: SpringConfig; onRest?: () => void } = {}): void {
    this.target = target;
    if (options.velocity !== undefined) {
      this.velocity = options.velocity;
    }
    this.config = options.config ?? SPRING_DEFAULT;
    this.onRest = options.onRest;

    if (!this.frame) {
      this.lastTime = performance.now();
      this.frame = requestAnimationFrame(this.tick);
    }
  }

  /** Jump without animating (used while the finger drives the value 1:1). */
  set(value: number): void {
    this.stop();
    this.value = value;
    this.target = value;
    this.velocity = 0;
    this.onUpdate(value);
  }

  /** Freeze at the current presentation value so a gesture can take over. */
  stop(): void {
    if (this.frame) {
      cancelAnimationFrame(this.frame);
      this.frame = 0;
    }
    this.onRest = undefined;
  }

  private tick = (now: number): void => {
    // Clamp dt so a backgrounded tab doesn't explode the integration.
    let dt = Math.min((now - this.lastTime) / 1000, 1 / 30);
    this.lastTime = now;

    const { dampingRatio, response } = this.config;
    const stiffness = Math.pow((2 * Math.PI) / response, 2);
    const damping = (4 * Math.PI * dampingRatio) / response;

    // Semi-implicit Euler with fixed sub-steps: stable for UI-range springs.
    const step = 1 / 240;
    while (dt > 0) {
      const h = Math.min(step, dt);
      const force = -stiffness * (this.value - this.target) - damping * this.velocity;
      this.velocity += force * h;
      this.value += this.velocity * h;
      dt -= h;
    }

    const settled = Math.abs(this.velocity) < 5 && Math.abs(this.value - this.target) < 0.5;
    if (settled) {
      this.value = this.target;
      this.velocity = 0;
      this.onUpdate(this.value);
      this.frame = 0;
      const done = this.onRest;
      this.onRest = undefined;
      done?.();
      return;
    }

    this.onUpdate(this.value);
    this.frame = requestAnimationFrame(this.tick);
  };
}

/** Apple's momentum projection (scroll-like exponential deceleration). */
export function projectMomentum(velocity: number, decelerationRate = 0.99): number {
  return ((velocity / 1000) * decelerationRate) / (1 - decelerationRate);
}

/** Progressive resistance past a boundary: things slow before they stop. */
export function rubberband(overshoot: number, dimension: number, constant = 0.55): number {
  const sign = Math.sign(overshoot);
  const distance = Math.abs(overshoot);
  return sign * (distance * dimension * constant) / (dimension + constant * distance);
}
