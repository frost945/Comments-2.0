import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment } from '../../features/comments/models/comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {

  private apiUrl = '/api/comments';

  constructor(private http: HttpClient) {}

  getComments(skip: number, sortBy: string, ascending: boolean, cursorCreatedAt?: string | null, cursorId?: number | null, direction?: boolean): Observable<Comment[]> {
    let params = new HttpParams()
    .set('sortBy', sortBy)
    .set('ascending', ascending);

    //for sorting by createdAt, we use keyset pagination
    if(sortBy === 'CreatedAt') {
      if (cursorCreatedAt != null && cursorId != null) {
        params = params
        .set('cursorCreatedAt', cursorCreatedAt)
        .set('cursorId', cursorId)
      }
       params = params.set('direction', direction ?? true); // default is true for next page, false for prev page
    }
    else {
      params = params.set('skip', skip);
    }
    
    const url = `${this.apiUrl}`;

    const fullUrl = params.keys().length
    ? `${url}?${params.toString()}`
    : url;

    console.debug('comment GET URL:', fullUrl);

    return this.http.get<Comment[]>(fullUrl);
  } 

  getReplies(parentId: number, lastCreatedAt?: string | null, lastId?: number | null): Observable<Comment[]> {

    let params = new HttpParams()
    console.debug(`Getting replies for parentId=${parentId} with keyset pagination. lastCreatedAt=${lastCreatedAt}, lastId=${lastId}`);
    
    if (lastCreatedAt && lastId) {
    params = params
    .set('cursorCreatedAt', lastCreatedAt)
    .set('cursorId', lastId);
    }

    const url = `${this.apiUrl}/${parentId}/replies`;

    const fullUrl = params.keys().length
    ? `${url}?${params.toString()}`
    : url;

    console.debug('replies GET URL:', fullUrl);

    return this.http.get<Comment[]>(fullUrl)
  }

  getCommentById(id: number): Observable<Comment> {
    return this.http.get<Comment>(`${this.apiUrl}/${id}`);
  }

  createComment(formData: FormData): Observable<Comment> {
    return this.http.post<Comment>(this.apiUrl, formData);
  }
}
