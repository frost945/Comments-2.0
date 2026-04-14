import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ToastService {
  message = signal<string | null>(null);

  show(text: string, duration = 3000) {
    this.message.set(text);

    setTimeout(() => {
      this.message.set(null);
    }, duration);
  }
}