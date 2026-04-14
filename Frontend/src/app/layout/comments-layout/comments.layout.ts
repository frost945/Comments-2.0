import { Component, computed, inject } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { MainHeader } from '../../features/comments/components/main-header/main-header';
import { RepliesHeader } from '../../features/comments/components/replies-header/replies-header';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';
import { Pagination } from '../../features/comments/components/pagination/pagination';

@Component({
  selector: 'app-comments-layout',
  standalone: true,
  imports: [RouterOutlet, MainHeader, RepliesHeader, Pagination],
   template: `
    @if (isRepliesRoute()) {
      <app-replies-header />
    } @else {
      <app-main-header />
      <app-pagination />
    }

    <router-outlet />
  `
})

export class CommentsLayout {
  private router = inject(Router);

  private navigationEnd = toSignal(
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ),
    { initialValue: null }
  );

  isRepliesRoute = computed(() =>{
    this.navigationEnd();
    return this.router.url.includes('/replies');
});
}
