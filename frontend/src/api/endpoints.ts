import { API_CONFIG } from './config';

const API_URL = API_CONFIG.baseURL;

export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: `${API_URL}/authentication/login`,
    REGISTER: `${API_URL}/authentication/register`,
    REFRESH: `${API_URL}/authentication/refresh`,
  },
} as const;

export type AuthEndpoint = (typeof API_ENDPOINTS.AUTH)[keyof typeof API_ENDPOINTS.AUTH];
