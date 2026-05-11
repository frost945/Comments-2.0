import { AbstractControl, ValidationErrors } from '@angular/forms';

const MAX_IMAGE_SIZE = 5 * 1024 * 1024;
const MAX_TEXT_SIZE = 100 * 1024;

const allowedImageTypes = [
    'image/jpeg',
    'image/jpg',
    'image/png',
    'image/gif'
];

const allowedTextTypes = ['text/plain'];

export function fileValidator(control: AbstractControl): ValidationErrors | null {

  const file = control.value as File;

  if (!file) return null;

  const isImage = allowedImageTypes.includes(file.type);

  const isText = allowedTextTypes.includes(file.type);

  if (!isImage && !isText) {
    return { fileType: true };
  }

  if (isImage && (file.size > MAX_IMAGE_SIZE || file.size === 0)) {
    return { imageSize: true };
  }

  if (isText) {

    if (file.size === 0) {
      return { emptyTextFile: true };
    }

    if (file.size > MAX_TEXT_SIZE) {
      return { textSize: true };
    }
  }

  return null;
}