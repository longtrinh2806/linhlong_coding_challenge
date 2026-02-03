export const AUTH_ROUTES = {
  LOGIN: '/login',
  REGISTER: '/register',
  HOME: '/',
} as const;

export const AUTH_STORAGE_KEYS = {
  ACCESS_TOKEN: 'accessToken',
  REFRESH_TOKEN: 'refreshToken',
} as const;
