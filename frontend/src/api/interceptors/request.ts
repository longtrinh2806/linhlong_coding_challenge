import { type InternalAxiosRequestConfig } from 'axios';

export const addAuthInterceptor = (config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem('accessToken');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
};
