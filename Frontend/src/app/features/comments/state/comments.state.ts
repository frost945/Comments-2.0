import { Injectable, signal, effect, computed } from '@angular/core';
import { CommentsService } from '../../../core/services/comments.service';
import { Comment } from '../models/comment.model';
import { SessionStorageService } from '../../../core/storage/session-storage.service';
import { STORAGE_KEYS } from '../../../core/storage/storage.keys';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';
import { untracked } from '@angular/core/primitives/signals';

@Injectable({ providedIn: 'root' })
export class CommentsState {

  // comments data
  comments = signal<Comment[]>([]);
  replies = signal<Comment[]>([]);
  parentId = signal<number | null>(null);
  parentComment = signal<Comment | null>(null);

  // pagination
  readonly pageSize = 25;
  pageNumber = signal<number>(1);
  skip = signal<number>(0);
  skipReplies = computed(() => this.replies().length);
  
  // sorting
  sortField = signal<string>('CreatedAt');
  sortDir = signal<boolean>(true); // true - ascending, false - descending
  sortClicked = signal<boolean>(false);

  // replies loading flags
  loadingReplies = false;
  hasMoreReplies = true;

  // keyset pagination flags
  isKeysetMode = computed(() => this.sortField() === 'CreatedAt');
  isPrevPageKeyset = false;

  // keyset comments cursors
  lastCreatedAtComment = <string | null>(null);
  lastIdComment = <number | null>(null);
  firstCreatedAtComment = <string | null>(null);
  firstIdComment = <number | null>(null);

  // keyset replies cursors
  lastCreatedAtReply = computed(() => {
    const replies = this.replies();
    return replies.length > 0 ? replies[replies.length - 1].createdAt : null;
  });

  lastIdReply = computed(() => {
    const replies = this.replies();
    return replies.length > 0 ? replies[replies.length - 1].id : null;
  });

  constructor(private service: CommentsService, private storage: SessionStorageService) {
    // Restore the startup state
    const saved = this.storage.get<any>(STORAGE_KEYS.COMMENTS_STATE);
    if (saved) {
      this.sortField.set(saved.sortField);
      this.sortDir.set(saved.sortDir);
      this.skip.set(saved.skip);
      this.sortClicked.set(saved.sortClicked );
      this.pageNumber.set(saved.pageNumber);
    }

    // Autosave on any change
    effect(() => {
      const stateSnapshot = {
        sortField: this.sortField(),
        sortDir: this.sortDir(),
        skip: this.skip(),
        sortClicked: this.sortClicked(),
        pageNumber: this.pageNumber(),
      };

      this.storage.set(STORAGE_KEYS.COMMENTS_STATE, stateSnapshot);
    });

    effect(() => {
      console.debug('EFFECT START');
      untracked(() => {
         console.debug('sort ', this.sortField(), ' dir ', this.sortDir(), ' skip ', this.skip(), ' parentId ', this.parentId(),
         'parentComment ', this.parentComment(), 'skipReplies', this.skipReplies(),'hasMoreReplies', this.hasMoreReplies,
         'loadingReplies', this.loadingReplies);
        });

      const parentId=this.parentId();

      if (parentId === null){
        // signals tracked for main page of comments
        const sort = this.sortField();
        const dir = this.sortDir();
        const skip = this.skip();
        // set default replies loading flags
        this.loadingReplies = false;
        this.hasMoreReplies = true;

        this.loadCommentsAsync(skip, sort, dir);
        
        // reset data, if replies page is closed
        untracked(() => { 
          this.parentComment.set(null);
          this.replies.set([]);
        });
      }
    console.debug('EFFECT END');
    });
  }

  private requestId = 0;
  async loadCommentsAsync(skip: number, sort: string, dir: boolean) {
   // only for sorting by CreatedAt, we use reset pagination
    if(this.requestId===0 && this.sortField() === 'CreatedAt') this. resetPagination();
    ++this.requestId;

    const cursorCreatedAt = this.isPrevPageKeyset
      ? this.firstCreatedAtComment
      : this.lastCreatedAtComment;
    
    const cursorId = this.isPrevPageKeyset
      ? this.firstIdComment
      : this.lastIdComment;

    const sign = this.isPrevPageKeyset ? -1 : 1;

    console.debug("loadComments- CursorCreatedAt: ", cursorCreatedAt, " CursorId: ", cursorId, " isPrevPageKeyset: ", this.isPrevPageKeyset, "sign: ", sign);

    const data = await firstValueFrom(this.service.getComments(skip, sort, dir, cursorCreatedAt, cursorId, sign));

    console.debug("comments loaded: ", [...data]);

    //If 25 comments on the last page, then we restore the display for the nextPage button
    if (data.length === 0 && this.pageNumber() > 1) {
      console.debug("No comments loaded, returning to the previous page");
      this.prevPage();
      return;
    }
    
    this.comments.set(data);

    this.isPrevPageKeyset = false;

    this.lastCreatedAtComment = null;
    this.lastIdComment = null;
    this.firstCreatedAtComment = null;
    this.firstIdComment = null;
  }

  async loadRepliesAsync(parentId: number){
    console.debug("loadreplies start:", "loadingReplies: ", this.loadingReplies, 'parentId: ', parentId);

    let parentComment = this.parentComment();

  // On first replies load, parentComment may be missing — load it
    if (!parentComment || parentComment.id !== parentId) {
     parentComment = await this.loadParentCommentAsync(parentId);
    }

    const replyCount = parentComment?.replyCount ?? 0;
    console.debug("replyCount: ", replyCount);
    
    // If no replies, do not make a request to server
    if (replyCount === 0) {
      this.replies.set([]);
      this.hasMoreReplies = false;
      return;
    }

    if (this.loadingReplies) return;
    this.loadingReplies = true;

    const skipReplies = untracked(() => this.skipReplies());

    const data = await firstValueFrom(this.service.getReplies(parentId, this.lastCreatedAtReply(), this.lastIdReply()));

    console.debug("replies loaded: ", data);

    const skip = this.skipReplies();
    if(skip + data.length >= replyCount){
      this.hasMoreReplies = false;
    }

    if (skipReplies === 0) {
      this.replies.set(data);
    }
    else{
      this.replies.update(list => [...list, ...data]);
    }

    this.loadingReplies = false;
      console.debug("loadreplies finish:", "skip: ", skip, "hasMoreReplies: ", this.hasMoreReplies);
  }

  async loadParentCommentAsync(id: number): Promise<Comment | null> {
    console.debug("loadParentCommentAsync start, id: ", id);
    const existParentComment = untracked(() => this.comments().find(c => c.id === id));
    
    if(existParentComment){
      console.debug("parentComment already exists in comments: ", existParentComment);
      untracked(() => this.parentComment.set(existParentComment));
      return existParentComment;
    }

    const comment = await firstValueFrom(this.service.getCommentById(id));

    //Race protection: If you fast exit the replies page and enter a new replies page,
    // the check will prevent the parentComment from being displayed that is no longer relevant
    const parentId = untracked(() => this.parentId());
    if (parentId === id) {
      console.debug("parentComment not found in comments, loading from API");
      untracked(() => this.parentComment.set(comment));
      return comment;
    }
    console.debug("loadParentCommentAsync finish, loaded parentComment: ", comment);
    return null;
  }

  async CreateCommentAsync(formData: FormData) {
    console.debug("start coment create");

    const comment = await firstValueFrom(this.service.createComment(formData));
    const parentId = untracked(()=> this.parentId());

    // on main page comment creation — reload first page with default sorting
    if (!parentId) {
      this.loadDefaultPageComments();
      return;
    }

    console.debug('coment create for repliess page');

    // Update replyCount for parent comment
    this.parentComment.update(c => c ? { ...c, replyCount: c.replyCount + 1 } : c);

    // on the last replies page — append new reply to the end
    if(!this.hasMoreReplies){
      this.replies.update(list => [...list, comment]);
    }
    
    console.debug("finish comment create:", comment);
  }

 setSort(field: string) {

  if (this.sortField() === field) {
    return; // sorting not changed
  }

  this.sortField.set(field);
  this.sortClicked.set(true);
  
  //If you changed the sorting not on the first page, return to the first page
 if (this.pageNumber() > 1) {
    this.resetPagination();
  }
}

// If sort is changes , sort button will be highlighted
  activeSort(field: string) {
    return this.sortClicked() &&  this.sortField() === field;
  }

  toggleDir() {
    this.sortDir.update(v => !v);
    //If you changed the sorting direction not on the first page, return to the first page
    if (this.pageNumber() > 1) {
      this.resetPagination();}
  }

hasNextPage = computed(() =>
  this.comments().length === this.pageSize
);

hasPrevPage = computed(() =>
  this.pageNumber() > 1
);
nextPage() {
  console.debug('nextPage');
  if (!this.hasNextPage()) return;

  if (this.isKeysetMode()) {
    this.lastCreatedAtComment = this.comments()[this.comments().length - 1].createdAt;
    this.lastIdComment = this.comments()[this.comments().length - 1].id;
  }
  this.skip.update(v => v + this.pageSize);
  this.pageNumber.update(v => v + 1);
}

prevPage() {
  if (!this.hasPrevPage()) return;

    if (this.isKeysetMode()) {
      this.firstCreatedAtComment = this.comments()[0].createdAt;
      this.firstIdComment = this.comments()[0].id;
      this.isPrevPageKeyset = true;
      console.debug('prevPage in keyset mode- isPrevPageKeyset set to true', this.isPrevPageKeyset);
    }

  this.skip.update(v => v - this.pageSize);
  this.pageNumber.update(v => v - 1);
}

private resetPagination(){
  console.debug("resetPagination");
  this.skip.set(0); // for start effect
  this.pageNumber.set(1);
}

private loadDefaultPageComments(){
  this.resetPagination();
  this.sortField.set("CreatedAt");
  this.sortDir.set(true);
}
}