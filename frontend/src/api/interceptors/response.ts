import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { API_ENDPOINTS } from '../endpoints';
import {
  getRefreshToken,
  setAuthTokens,
  clearAuthStorage,
  getApiErrorMessage,
} from '../apiError';
import { AUTH_ROUTES } from '../../constants';
import { t } from '../../locales/i18n';

interface RetryableConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

export const handleAuthError = async (error: AxiosError): Promise<never> => {
  const originalRequest = error.config as RetryableConfig;
  const translations = t();

  if (error.response?.status === 401 && !originalRequest._retry) {
    originalRequest._retry = true;

    const refreshToken = getRefreshToken();
    if (!refreshToken) {
      clearAuthStorage();
      window.location.href = AUTH_ROUTES.LOGIN;
      throw new Error(translations.errors.noRefreshToken);
    }

    try {
      const response = await axios.post(API_ENDPOINTS.AUTH.REFRESH, {
        refreshToken,
      });

      const { accessToken, refreshToken: newRefreshToken } = response.data;
      setAuthTokens(accessToken, newRefreshToken);

      if (originalRequest.headers) {
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
      }
      return axios(originalRequest);
    } catch {
      clearAuthStorage();
      window.location.href = AUTH_ROUTES.LOGIN;
      throw new Error(translations.errors.sessionExpired);
    }
  }

  throw getApiErrorMessage(error);
};
