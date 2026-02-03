import { API_CONFIG } from './config';

const API_URL = API_CONFIG.baseURL;

export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: `${API_URL}/authentication/login`,
    REGISTER: `${API_URL}/authentication/register`,
    REFRESH: `${API_URL}/authentication/refresh`,
  },
  HEALTH: {
    CHECK: `${API_URL}/health`,
  },
} as const;

export type AuthEndpoint = (typeof API_ENDPOINTS.AUTH)[keyof typeof API_ENDPOINTS.AUTH];
export type HealthEndpoint = (typeof API_ENDPOINTS.HEALTH)[keyof typeof API_ENDPOINTS.HEALTH];
