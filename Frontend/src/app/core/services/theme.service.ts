import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })

export class ThemeService {

 private mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');

  init() {
    this.applyTheme(this.mediaQuery.matches);

    this.mediaQuery.addEventListener('change', (e) => {
      this.applyTheme(e.matches);
    });
  }

  private applyTheme(isDark: boolean) {
    if (isDark) {
      document.documentElement.classList.add('dark-mode');
    } else {
      document.documentElement.classList.remove('dark-mode');
    }
  }
}