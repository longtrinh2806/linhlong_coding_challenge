import { type AxiosError } from 'axios';
import { t } from '../locales/i18n';
import { AUTH_STORAGE_KEYS } from '../constants';

// Handle different API response formats
export interface ApiErrorResponse {
  message?: string;
  errors?: {
    message?: string;
    [key: string]: unknown;
  };
  code?: string;
  details?: Record<string, string>;
}

export const getApiErrorMessage = (error: unknown): string => {
  const translations = t();

  // Check if it's an AxiosError first (before checking Error, since AxiosError extends Error)
  const axiosError = error as AxiosError<ApiErrorResponse>;
  
  if (axiosError.isAxiosError && axiosError.response) {
    const responseData = axiosError.response.data;

    if (responseData?.errors?.message) {
      return responseData.errors.message;
    }

    if (responseData?.message) {
      return responseData.message;
    }

    // Handle specific status codes
    switch (axiosError.response.status) {
      case 400:
        return translations.errors.badRequest;
      case 401:
        return translations.errors.unauthorized;
      case 403:
        return translations.errors.forbidden;
      case 404:
        return translations.errors.notFound;
      case 422:
        return translations.errors.validationError;
      case 500:
        return translations.errors.serverError;
      default:
        return `Error: ${axiosError.response.status}`;
    }
  }

  // Handle network errors
  if (axiosError.isAxiosError && axiosError.code === 'ERR_NETWORK') {
    return translations.errors.networkError;
  }

  if (axiosError.isAxiosError && axiosError.code === 'ERR_CANCELED') {
    return translations.errors.requestCancelled;
  }

  // Handle plain Error objects
  if (error instanceof Error) {
    return error.message;
  }

  // Fallback for unknown errors
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
