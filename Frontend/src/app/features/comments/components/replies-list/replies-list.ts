import { AfterViewInit, Component, ElementRef, inject, OnDestroy, ViewChild } from '@angular/core';
import { CommentsState } from '../../state/comments.state';
import { CommentCard } from '../comment-card/comment-card';

@Component({
  selector: 'app-replies-list',
  imports: [CommentCard],
  templateUrl: './replies-list.html',
  styleUrl: './replies-list.css',
})
export class RepliesList implements AfterViewInit, OnDestroy{
  state = inject(CommentsState);

  constructor() {console.debug("RepliesList init")}

    @ViewChild('scrollTrigger')
    trigger!: ElementRef<HTMLDivElement>;

    private observer!: IntersectionObserver;

     ngAfterViewInit() {
      console.debug("ngAfterViewInit start-  ", ' loadingReplies:', this.state.loadingReplies);

      this.observer = new IntersectionObserver(
      entries => {

        // no observer needed, if no more replies or already loading
      if(!this.state.hasMoreReplies || this.state.loadingReplies)
        return;

      const entry = entries[0];

      // Skip loading if not visible.
      if (!entry.isIntersecting) return;
        
        console.debug("Load more replies");
        this.state.loadReplies(this.state.parentId()!);
      },
      {
        root: null,       // viewport
        rootMargin: "100px", // pre-loading
        threshold: 0
      }
    );

    this.observer.observe(this.trigger.nativeElement);

    console.debug("ngAfterViewInit finish");
  }

  ngOnDestroy() {
    console.debug("ngOnDestroy");
    this.observer.disconnect();
  }
}
