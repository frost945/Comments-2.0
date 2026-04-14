export function wrapSelection(
  textarea: HTMLTextAreaElement,
  openTag: string,
  closeTag: string
) {

  const start = textarea.selectionStart;
  const end = textarea.selectionEnd;

  const before = textarea.value.slice(0, start);
  const sel = textarea.value.slice(start, end);
  const after = textarea.value.slice(end);

  textarea.value =
    before + openTag + sel + closeTag + after;

  if (!sel) {

    const cursor = start + openTag.length;

    textarea.selectionStart = cursor;
    textarea.selectionEnd = cursor;

  } else {

    const cursor =
      start + openTag.length + sel.length + closeTag.length;

    textarea.selectionStart = cursor;
    textarea.selectionEnd = cursor;
  }

  textarea.focus();
}

export function clientSidePreview(raw: string) {

  if (!raw) return '';

  const wrapper = document.createElement('div');
  wrapper.innerHTML = raw;

  return wrapper.innerHTML;
}