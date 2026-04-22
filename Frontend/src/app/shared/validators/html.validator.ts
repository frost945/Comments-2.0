import { AbstractControl, ValidationErrors } from '@angular/forms';
import { validateHtml } from '../utils/html-security.util';

export function htmlValidator(control: AbstractControl): ValidationErrors | null {

  const value = control.value;

  if (!value) return null;

  const validation = validateHtml(value);

  if (!validation.isValid) {
    return { unsafeHtml: true };
  }

  return null;
}