import { Component, effect, HostListener, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CommentsState } from '../../state/comments.state';
import { fileValidator } from '../../../../shared/validators/file.validator';
import { htmlValidator } from '../../../../shared/validators/html.validator';
import { TextEditor } from '../text-editor/text-editor';
import { ToastService } from '../../../../shared/ui/toast/toast.service';


@Component({
  selector: 'app-comment-form-modal',
  imports: [ReactiveFormsModule, TextEditor],
  templateUrl: './comment-form-modal.html',
  styleUrl: './comment-form-modal.css',
})
export class CommentFormModal {
  
  isOpen = input(false);
  close = output<void>();

  private fb = inject(FormBuilder);
  private state = inject(CommentsState);
  private toast = inject(ToastService);

  isSubmitting = signal(false);

  form = this.fb.nonNullable.group({
    userName: [
      '',
      [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(20),
        Validators.pattern(/^\S+$/)// не должно содержать пробелов
      ]
    ],

    email: ['',[Validators.required, Validators.email]],

    text: ['', [Validators.required, Validators.maxLength(2000), htmlValidator]],

    file: [null as File | null,[fileValidator]] 
  });

  constructor() {
  effect(() => {
    if (this.isOpen()) {
      document.body.classList.add('modal-open');
    } else {
      document.body.classList.remove('modal-open');
    }
  });
}
  closeModal() {
    this.close.emit();
  }
  
   @HostListener('document:keydown.escape')
  onEsc() {
    this.closeModal();
  }

  get userName() {
    return this.form.controls.userName;
  }
    get email() {
    return this.form.controls.email;
  }
  get f() {
    return this.form.controls.file;
  }

  onFileChange(event: Event) {

  const input = event.target as HTMLInputElement;

  if (!input.files?.length) return;

  const file = input.files[0];

  console.debug('Selected file:', file);
  
  this.form.controls.file.setValue(file);
  
  // to display the error immediately
  this.form.controls.file.markAsTouched();
}

  async submit() {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formData = new FormData();

    formData.append('userName', this.form.value.userName!);
    formData.append('email', this.form.value.email!);
    formData.append('text', this.form.value.text!);

    const parentId = this.state.parentId();

    if (parentId) {
      formData.append('parentId', parentId.toString());
    }

    console.debug('Appending file to FormData:', this.form.value.file);
    if (this.form.value.file) {
      
      formData.append('file', this.form.value.file);
    }

    try {
      this.isSubmitting.set(true);
      await this.state.CreateCommentAsync(formData);

      this.closeModal();
      this.form.reset();
      this.form.controls.file.setValue(null);

      this.toast.show('Опубликовано');
    }
    catch (err) {

      console.error(err);
      alert('Ошибка отправки');
    }
    finally {
      this.isSubmitting.set(false);
    }
  }
}
