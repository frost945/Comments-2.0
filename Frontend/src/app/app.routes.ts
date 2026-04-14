import { Routes } from '@angular/router';
import { COMMENTS_ROUTES } from './features/comments/comments.routes';
import { CommentsLayout } from './layout/comments-layout/comments.layout';

export const routes: Routes = [
  { path: 'comments',  component: CommentsLayout, children: COMMENTS_ROUTES },
  { path: '', redirectTo: 'comments', pathMatch: 'full' }
];