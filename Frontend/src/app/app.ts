import { Component } from '@angular/core';
import { RouterOutlet } from "@angular/router";
import { ThemeService } from '../app/core/services/theme.service';
import { ToastComponent } from './shared/ui/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
})

export class App {

  constructor(public theme: ThemeService) {}

  ngOnInit() {
    this.theme.init();
  }
}