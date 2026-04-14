import { Component, effect, inject, signal, untracked } from '@angular/core';
import {CommentsState} from '../../state/comments.state';

@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
})
export class Pagination {
  state = inject(CommentsState);

  displayPageNumber = signal(this.state.pageNumber());
  private shouldUpdatePage = false;

  constructor() {
    console.debug("Pagination init")

    // Effect to handle scrolling after page change
    //The effect is triggered after the data array has been updated, then the scrollbar moves to the top position.
    //  Otherwise, on a slow internet connection, the page will first scroll up, we'll see the current page of old comments,
    //  and only then will the comment array be updated.
    
    effect(() => {
      const comments = this.state.comments();

      // the condition is such that it works if changing the page or if creating a new comment
      if (this.shouldUpdatePage || comments) {
        untracked(() => this.displayPageNumber.set(this.state.pageNumber()));
        this.resetScroll();
        this.shouldUpdatePage = false;
      }
    });
  }

  private resetScroll() {
    window.scrollTo({
      top: 0
    });
  }

  onNextPage() {
  this.shouldUpdatePage = true;
  this.state.nextPage();
}

  onPrevPage() {
  this.shouldUpdatePage = true;
  this.state.prevPage();
}
}
