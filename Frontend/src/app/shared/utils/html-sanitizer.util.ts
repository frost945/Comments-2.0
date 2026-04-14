
const SANITIZER_CONFIG = {
  ALLOWED_TAGS: ['a', 'code', 'i', 'strong'],
  ALLOWED_ATTR: ['href', 'title'],
  ALLOW_DATA_ATTR: false
};

 export function sanitizeHtml(input: string): string {

  if (typeof DOMPurify === 'undefined') {
        console.warn('DOMPurify is unavailable');
        return input;
    }

  if (!input) return '';

   let cleaned = DOMPurify.sanitize(input, SANITIZER_CONFIG);

  // добавляем target="_blank" и rel="noopener noreferrer" ко всем ссылкам
  cleaned = cleaned.replace(
    /<a\s+([^>]+)>/g,
    '<a $1 target="_blank" rel="noopener noreferrer">'
  );

  return cleaned;
}

export function validateHtml(input: string) {

  if (!input) {
    return { isValid: true, cleaned: '' };
  }

  const cleaned = sanitizeHtml(input);
  const isValid = cleaned === input;

  return {
    isValid,
    cleaned
  };
}