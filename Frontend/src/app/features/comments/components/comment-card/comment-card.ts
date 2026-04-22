import { Component, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { Comment } from '../../models/comment.model';
import { sanitizeHtml } from '../../../../shared/utils/html-security.util';
import { GlightboxDirective } from '../../../../shared/directives/glightbox.directives';
import { FileDownloadService } from '../../../../core/services/file-download.service';

@Component({
  selector: 'app-comment-card',
  standalone: true,
  imports: [GlightboxDirective],
  templateUrl: './comment-card.html',
  styleUrls: ['./comment-card.css']
})
export class CommentCard {

  comment = input.required<Comment>();
  showReplyButton = input(true);
  private router = inject(Router);
  
  fileDownloadService = inject(FileDownloadService)

  sanitizedText = '';

  ngOnInit() {
    this.sanitizedText = sanitizeHtml(this.comment().text);
  }

  openReplies() {
    this.router.navigate([
      '/comments',
      this.comment().id,
      'replies'
    ]);
  }
}
