import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment } from '../../features/comments/models/comment.model';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {

  private apiUrl = '/api/comments';

  constructor(private http: HttpClient) {}

  getComments(skip: number, sortBy: string, ascending: boolean): Observable<Comment[]> {
    const params = new HttpParams()
    .set('skip', skip)
    .set('sortBy', sortBy)
    .set('ascending', ascending);

    return this.http.get<Comment[]>(this.apiUrl, {params})
    .pipe(tap(comments => {
        console.debug('get comments:', comments);
      })
    );
  } 

  getReplies(parentId: number, skip: number): Observable<Comment[]> {
    const params = new HttpParams()
    .set('skip', skip);

    return this.http.get<Comment[]>(`${this.apiUrl}/${parentId}/replies`, {params})
    .pipe(tap(replies => {
        console.debug('get replies:', replies);
      })
    );
  }

  getCommentById(id: number): Observable<Comment> {
    return this.http.get<Comment>(`${this.apiUrl}/${id}`);
  }

  createComment(formData: FormData): Observable<Comment> {
    return this.http.post<Comment>(this.apiUrl, formData);
  }
}
