import { AbstractControl, ValidationErrors } from '@angular/forms';

export function fileValidator(control: AbstractControl): ValidationErrors | null {

  const file = control.value as File;

  if (!file) return null;

  const allowedImageTypes = [
    'image/jpeg',
    'image/jpg',
    'image/png',
    'image/gif'
  ];

  const allowedTextTypes = ['text/plain'];

  if (
    !allowedImageTypes.includes(file.type) &&
    !allowedTextTypes.includes(file.type)
  ) {
    return { fileType: true };
  }

  if (
    allowedImageTypes.includes(file.type) &&
    file.size > 5 * 1024 * 1024
  ) {
    return { imageSize: true };
  }

  if (
    allowedTextTypes.includes(file.type) &&
    file.size > 100 * 1024
  ) {
    return { textSize: true };
  }

  return null;
}