import { Component, inject, signal } from '@angular/core';
import { CommentFormModal } from '../comment-form-modal/comment-form-modal';
import { CommentsState } from '../../state/comments.state';
import { Router } from '@angular/router';


@Component({
  selector: 'app-replies-header',
  imports: [CommentFormModal],
  templateUrl: './replies-header.html',
  styleUrl: './replies-header.css',
})
export class RepliesHeader {
  isModalOpen = signal(false);

  state = inject(CommentsState);
  private router = inject(Router);

  constructor() {
  console.debug('RepliesHeader INIT');
}

closeReplies(){
  console.debug("closeReplies");
  
  this.router.navigate(['/comments']);
}

openModal(){
    this.isModalOpen.set(true);
  }
}
