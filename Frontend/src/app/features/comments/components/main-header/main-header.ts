import { Component, signal } from '@angular/core';
import { CommentFormModal } from '../comment-form-modal/comment-form-modal';
import { Sorting } from '../sorting/sorting';

@Component({
  selector: 'app-main-header',
  imports: [CommentFormModal, Sorting],
  templateUrl: './main-header.html',
  styleUrl: './main-header.css',
})

export class MainHeader {

  isModalOpen = signal(false);
  
  constructor() {
  console.debug('MainHeader INIT');
  }

  openModal(){
    this.isModalOpen.set(true);
  }
}
