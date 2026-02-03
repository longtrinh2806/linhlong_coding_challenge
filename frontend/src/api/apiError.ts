import { type AxiosError } from 'axios';
import { t, getCurrentLocale, type Locale } from '../locales/i18n';
import { AUTH_STORAGE_KEYS } from '../constants';

export interface ApiErrorResponse {
  message: string;
  code?: string;
  details?: Record<string, string>;
}

// Get current locale dynamically
const getLocale = (): Locale => getCurrentLocale();

export const getApiErrorMessage = (error: unknown): string => {
  if (error instanceof Error && error.message) {
    return error.message;
  }

  const axiosError = error as AxiosError<ApiErrorResponse>;
  const locale = getLocale();
  const translations = t();

  if (axiosError.response?.data?.message) {
    return axiosError.response.data.message;
  }

  if (axiosError.response?.status === 401) {
    return locale === 'vi' ? 'Unauthorized. Please log in again.' : translations.errors.unauthorized;
  }

  if (axiosError.response?.status === 403) {
    return translations.errors.forbidden;
  }

  if (axiosError.response?.status === 404) {
    return translations.errors.notFound;
  }

  if (axiosError.response?.status === 500) {
    return translations.errors.serverError;
  }

  if (axiosError.code === 'ERR_NETWORK') {
    return translations.errors.networkError;
  }

  return translations.errors.unexpectedError;
};

export const isApiError = (error: unknown): error is AxiosError<ApiErrorResponse> => {
  return (error as AxiosError).isAxiosError === true;
};

export const clearAuthStorage = (): void => {
  localStorage.removeItem(AUTH_STORAGE_KEYS.ACCESS_TOKEN);
  localStorage.removeItem(AUTH_STORAGE_KEYS.REFRESH_TOKEN);
};

export const getRefreshToken = (): string | null => {
  return localStorage.getItem(AUTH_STORAGE_KEYS.REFRESH_TOKEN);
};

export const setAuthTokens = (accessToken: string, refreshToken: string): void => {
  localStorage.setItem(AUTH_STORAGE_KEYS.ACCESS_TOKEN, accessToken);
  localStorage.setItem(AUTH_STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
};
