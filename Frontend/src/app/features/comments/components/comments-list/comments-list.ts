import { Component } from '@angular/core';
import { CommentCard } from '../comment-card/comment-card';
import { CommentsState } from '../../state/comments.state';
import { inject } from '@angular/core';

@Component({
  selector: 'app-comments-list',
  standalone: true,
  imports: [CommentCard],
  templateUrl: './comments-list.html',
  styleUrls: ['./comments-list.css']
})
export class CommentsList {

  state = inject(CommentsState);

  constructor(){console.debug("CommentsList init")}
}
