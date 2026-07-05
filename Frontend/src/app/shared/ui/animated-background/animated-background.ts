import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild
} from '@angular/core';

interface MouseState {
  x: number | null;
  y: number | null;
  radius: number;
}

class Star {

  x: number;
  y: number;
  vx: number;
  vy: number;
  size: number;

  constructor(
    private canvas: HTMLCanvasElement
  ) {

    this.x = Math.random() * canvas.width;
    this.y = Math.random() * canvas.height;

    this.vx = (Math.random() - 0.5) * 0.25;
    this.vy = (Math.random() - 0.5) * 0.25;

    this.size = Math.random() * 2 + 1;
  }

  update(mouse: MouseState) {

    this.x += this.vx;
    this.y += this.vy;

    if (this.x < 0 || this.x > this.canvas.width) {
      this.vx *= -1;
    }

    if (this.y < 0 || this.y > this.canvas.height) {
      this.vy *= -1;
    }

    if (mouse.x === null || mouse.y === null) {
      return;
    }

    const dx = mouse.x - this.x;
    const dy = mouse.y - this.y;

    const dist = Math.sqrt(dx * dx + dy * dy);

    if (dist < mouse.radius) {

      const force =
        (mouse.radius - dist) / mouse.radius;

      this.x += dx * force * 0.01;
      this.y += dy * force * 0.01;
    }
  }

  draw(
    ctx: CanvasRenderingContext2D,
    mouse: MouseState
  ) {

    let glow = 0;

    if (mouse.x !== null && mouse.y !== null) {

      const dist = Math.hypot(
        this.x - mouse.x,
        this.y - mouse.y
      );

      if (dist < 150) {
        glow = (150 - dist) / 150;
      }
    }

    ctx.save();

    if (glow > 0) {
      ctx.shadowBlur = 20 * glow;
      ctx.shadowColor = "#8ab4ff";
    }

    ctx.beginPath();

    ctx.arc(
      this.x,
      this.y,
      this.size + glow * 2,
      0,
      Math.PI * 2
    );

    ctx.fillStyle =
      `rgba(255,255,255,${0.8 + glow * 0.2})`;

    ctx.fill();

    ctx.restore();
  }
}

@Component({
  selector: 'app-animated-background',
  standalone: true,
  template: `<canvas #canvas></canvas>`,
  styles: [`
    :host{
      position:fixed;
      inset:0;
      z-index:-1;
      display:block;
    }

    canvas{
      width:100%;
      height:100%;
      display:block;
    }
  `]
})
export class AnimatedBackgroundComponent
  implements AfterViewInit, OnDestroy {

  @ViewChild('canvas', { static: true })
  canvasRef!: ElementRef<HTMLCanvasElement>;

  private readonly STAR_COUNT = 120;
  private readonly LINK_DISTANCE = 150;

  private ctx!: CanvasRenderingContext2D;
  private canvas!: HTMLCanvasElement;

  private stars: Star[] = [];

  private animationId = 0;

  private mouse: MouseState = {
    x: null,
    y: null,
    radius: 200
  };

  private resizeHandler = () => {

    this.canvas.width = window.innerWidth;
    this.canvas.height = window.innerHeight;
  };

  private mouseMoveHandler = (e: MouseEvent) => {

    this.mouse.x = e.clientX;
    this.mouse.y = e.clientY;
  };

  private mouseLeaveHandler = () => {

    this.mouse.x = null;
    this.mouse.y = null;
  };

  ngAfterViewInit(): void {

    this.canvas = this.canvasRef.nativeElement;

    const context = this.canvas.getContext('2d');

    if (!context) {
      return;
    }

    this.ctx = context;

    this.resizeHandler();

    window.addEventListener(
      'resize',
      this.resizeHandler
    );

    window.addEventListener(
      'mousemove',
      this.mouseMoveHandler
    );

    window.addEventListener(
      'mouseleave',
      this.mouseLeaveHandler
    );

    this.createStars();

    this.animate();
  }

  ngOnDestroy(): void {

    cancelAnimationFrame(this.animationId);

    window.removeEventListener(
      'resize',
      this.resizeHandler
    );

    window.removeEventListener(
      'mousemove',
      this.mouseMoveHandler
    );

    window.removeEventListener(
      'mouseleave',
      this.mouseLeaveHandler
    );
  }

  private createStars(): void {

    this.stars = [];

    for (let i = 0; i < this.STAR_COUNT; i++) {
      this.stars.push(
        new Star(this.canvas)
      );
    }
  }

  private drawBackground(): void {

    const gradient =
      this.ctx.createRadialGradient(
        this.canvas.width / 2,
        this.canvas.height / 2,
        0,
        this.canvas.width / 2,
        this.canvas.height / 2,
        this.canvas.width
      );

    gradient.addColorStop(0, "#0b1735");
    gradient.addColorStop(1, "#02040d");

    this.ctx.fillStyle = gradient;

    this.ctx.fillRect(
      0,
      0,
      this.canvas.width,
      this.canvas.height
    );
  }

  private drawConnections(): void {

    for (let i = 0; i < this.stars.length; i++) {

      for (
        let j = i + 1;
        j < this.stars.length;
        j++
      ) {

        const dx =
          this.stars[i].x -
          this.stars[j].x;

        const dy =
          this.stars[i].y -
          this.stars[j].y;

        const dist =
          Math.sqrt(dx * dx + dy * dy);

        let maxDistance =
          this.LINK_DISTANCE;

        if (
          this.mouse.x !== null &&
          this.mouse.y !== null
        ) {

          const d1 = Math.hypot(
            this.stars[i].x - this.mouse.x,
            this.stars[i].y - this.mouse.y
          );

          const d2 = Math.hypot(
            this.stars[j].x - this.mouse.x,
            this.stars[j].y - this.mouse.y
          );

          if (d1 < 200 || d2 < 200) {
            maxDistance = 250;
          }
        }

        if (dist >= maxDistance) {
          continue;
        }

        const opacity =
          (1 - dist / maxDistance) * 0.6;

        this.ctx.beginPath();

        this.ctx.moveTo(
          this.stars[i].x,
          this.stars[i].y
        );

        this.ctx.lineTo(
          this.stars[j].x,
          this.stars[j].y
        );

        this.ctx.strokeStyle =
          `rgba(120,180,255,${opacity})`;

        this.ctx.lineWidth = 1;

        this.ctx.stroke();
      }
    }
  }

  private animate = (): void => {

    this.ctx.clearRect(
      0,
      0,
      this.canvas.width,
      this.canvas.height
    );

    this.drawBackground();

    for (const star of this.stars) {

      star.update(this.mouse);

      star.draw(
        this.ctx,
        this.mouse
      );
    }

    this.drawConnections();

    this.animationId =
      requestAnimationFrame(
        this.animate
      );
  };
}