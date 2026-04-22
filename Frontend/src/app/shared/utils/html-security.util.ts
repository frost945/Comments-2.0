const SANITIZER_CONFIG = {
  ALLOWED_TAGS: ['a', 'code', 'i', 'strong'],
  ALLOWED_ATTR: ['href', 'title'],
  ALLOW_DATA_ATTR: false
};

// clean up HTML and adding necessary attributes to links
export function sanitizeHtml(input: string): string {
  if (!input) return '';

  const hook = (node: Element) => {
    if (node.tagName === 'A') {
      node.setAttribute('target', '_blank');
      node.setAttribute('rel', 'noopener noreferrer');
    }
  };

  DOMPurify.addHook('afterSanitizeAttributes', hook);

  const cleaned = DOMPurify.sanitize(input, SANITIZER_CONFIG);

  DOMPurify.removeHook('afterSanitizeAttributes', hook);

  console.debug('Sanitized HTML cleaned:', cleaned);
  return cleaned;
}

// remove attributes to ensure correct comparison of original and cleaned HTML
function normalizeForCompare(html: string): string {
  return html
    .replace(/\s+/g, ' ')
    .replace(/target="_blank"/g, '')
    .replace(/rel="noopener noreferrer"/g, '')
    .trim();
}

export function validateHtml(input: string) {
  if (!input) {
    return { isValid: true, cleaned: '' };
  }
  // use basic sanitizer, otherwise the comparison will be incorrect due to the attributes added to the links
  const cleaned = DOMPurify.sanitize(input, SANITIZER_CONFIG);

  const normalizedInput = normalizeForCompare(input);
  const normalizedCleaned = normalizeForCompare(cleaned);

  console.debug('normalizedInput:', normalizedInput);
  console.debug('normalizedCleaned:', normalizedCleaned);

  const isValid = normalizedInput === normalizedCleaned;

  return { isValid, cleaned};
}