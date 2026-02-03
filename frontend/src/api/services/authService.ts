import api from '../axios';
import { API_ENDPOINTS } from '../endpoints';
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  RefreshTokenRequest,
} from '../../types/auth';

export const authService = {
  login: (data: LoginRequest) =>
    api.post<AuthResponse>(API_ENDPOINTS.AUTH.LOGIN, data),

  register: (data: RegisterRequest) =>
    api.post<AuthResponse>(API_ENDPOINTS.AUTH.REGISTER, data),

  refreshToken: (data: RefreshTokenRequest) =>
    api.post<AuthResponse>(API_ENDPOINTS.AUTH.REFRESH, data),
};
