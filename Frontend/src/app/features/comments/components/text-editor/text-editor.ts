import {Component, ElementRef, ViewChild, signal, computed, Input, OnInit} from '@angular/core';
import { FormControl, ReactiveFormsModule} from '@angular/forms';
import { wrapSelection, clientSidePreview } from './text-editor.utils';

@Component({
  selector: 'app-text-editor',
  imports: [ReactiveFormsModule],
  templateUrl: './text-editor.html',
  styleUrl: './text-editor.css',
})
export class TextEditor implements OnInit {
  @ViewChild('commentText')
  textarea!: ElementRef<HTMLTextAreaElement>;

  @Input({required:true})
  control!: FormControl<string>;

  constructor(){}

   ngOnInit() {
    this.control.valueChanges.subscribe(v =>
      this.textSignal.set(v ?? '')
    );
  }

  toolbarButtons = [
    { tag: 'i', title: 'Курсив' },
    { tag: 'strong', title: 'Жирный шрифт' },
    { tag: 'code', title: 'Код' }
  ];

  textSignal = signal('');

  preview = computed(() =>
    clientSidePreview(this.textSignal())
  );

  wrapTag(tag: string)
  {
    const ta = this.textarea.nativeElement;

    wrapSelection(
      ta,
      `<${tag}>`,
      `</${tag}>`
    );
    this.control.setValue(ta.value);
  }

  insertLink()
  {
    const ta = this.textarea.nativeElement;

    let selection =
      ta.value.slice(
        ta.selectionStart,
        ta.selectionEnd
      );

    const href = prompt(
      'URL (href) — полный, например https://example.com',
      'https://'
    );

    if (!href) return;

    let title = prompt('Title (необязательно)', '');

    title = title
      ? ` title="${title.replace(/"/g,'&quot;')}"`
      : '';

    if (!selection)
      selection = href;

    const open =
      `<a href="${href.replace(/"/g,'&quot;')}"${title}>`;

    const close = '</a>';

    wrapSelection(
      ta,
      open,
      selection + close
    );

    this.control.setValue(ta.value);
  }
}
