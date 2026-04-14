import { Component, inject } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  template: `
    @if (toast.message()) {
      <div class="toast">
        {{ toast.message() }}
      </div>
    }
  `,
})
export class ToastComponent {
  toast = inject(ToastService);
}