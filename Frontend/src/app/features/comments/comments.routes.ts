import { Routes } from "@angular/router";
import { CommentsPage } from "../comments/pages/comments.page";
import {RepliesPage} from "../comments/pages/replies.page/replies.page";

export const COMMENTS_ROUTES: Routes =[
  {
    path: '',
    component: CommentsPage,
    title: 'Comments'
  },
  {
    path: ':id/replies',
    component: RepliesPage,
    title: 'Replies'
  }
]