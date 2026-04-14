import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommentsState } from '../../state/comments.state';
import { CommentCard } from "../../components/comment-card/comment-card";
import { RepliesList } from '../../components/replies-list/replies-list';

@Component({
  selector: 'app-replies-page',
  imports: [RepliesList, CommentCard],
  template: `
  <div class="main-replies-page">
    @if(state.parentComment()){
      <div class="parent-comment">
        <app-comment-card 
        [comment]=" state.parentComment()!"
        [showReplyButton]="false"
        />
      </div>
    }
    <div class="replies-container">
      <app-replies-list />
    </div>
  </div>
  `,
  styleUrls: ['./replies.page.css']
})
export class RepliesPage {

  private route = inject(ActivatedRoute);
  state = inject(CommentsState);
  
  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.state.parentId.set(id);

    console.debug('RepliesPage INIT, parentId=', id);
  }  
}