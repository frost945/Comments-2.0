import { Component, inject } from '@angular/core';
import { CommentsState } from '../../state/comments.state';

@Component({
  selector: 'app-sorting',
  imports: [],
  templateUrl: './sorting.html',
  styleUrl: './sorting.css',
})

export class Sorting {

  state = inject(CommentsState);
}
