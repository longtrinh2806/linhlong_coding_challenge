import { en } from './en';

export type Locale = 'en' | 'vi' | 'ja' | 'ko' | 'zh';

export const SUPPORTED_LOCALES = ['en', 'vi', 'ja', 'ko', 'zh'] as const;

const translations: Record<Locale, typeof en> = {
  en,
  vi: en,
  ja: en,
  ko: en,
  zh: en,
};

const STORAGE_KEY = 'locale';

export const getCurrentLocale = (): Locale => {
  const stored = localStorage.getItem(STORAGE_KEY) as Locale | null;
  if (stored && SUPPORTED_LOCALES.includes(stored)) {
    return stored;
  }
  return 'en';
};

export const setLocale = (locale: Locale): void => {
  localStorage.setItem(STORAGE_KEY, locale);
};

export const getTranslation = (): typeof en => {
  const locale = getCurrentLocale();
  return translations[locale];
};

// Shorthand for getting translations
export const t = (): typeof en => getTranslation();
