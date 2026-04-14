import { Component, inject } from '@angular/core';
import { CommentsList } from '../components/comments-list/comments-list';
import { CommentsState } from '../state/comments.state';

@Component({
  selector: 'app-comments-page',
  imports: [CommentsList],
  template: `
    <app-comments-list />
  `
})
export class CommentsPage {
  
  private state = inject(CommentsState);

  constructor() {
    this.state.parentId.set(null);
    console.debug('CommentsPage INIT');
}
}
