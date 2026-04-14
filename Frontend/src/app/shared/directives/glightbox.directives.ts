import {Directive, ElementRef, AfterViewInit, OnDestroy, HostListener} from '@angular/core';
import GLightbox from 'glightbox';

@Directive({
  selector: '[appLightbox]'
})
export class GlightboxDirective implements AfterViewInit, OnDestroy {

  private lightbox: any = null;

  constructor(private el: ElementRef) {}

  ngAfterViewInit() {
    //small delay to ensure DOM updated
    queueMicrotask(() => {
      this.init();
    });
  }

  private init() {
    if (this.lightbox) {
      this.lightbox.destroy();
    }

    this.lightbox = GLightbox({
      elements: [
        {
          href: this.el.nativeElement.href,
          type: 'image'
        }
      ],
        slideEffect: 'none',
        keyboardNavigation: true,
        touchNavigation: false,
        draggable: false,
        loop: false,
        zoomable: true,
        closeButton: true,
        closeOnOutsideClick: true
    }as any);
  }

  @HostListener('click', ['$event'])
  onClick(event: Event) {
    event.preventDefault();
    this.lightbox?.open();
  }

  ngOnDestroy() {
    if (this.lightbox) {
      this.lightbox.destroy();
    }
  }
}