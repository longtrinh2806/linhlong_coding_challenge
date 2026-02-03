import api from '../axios';
import { API_ENDPOINTS } from '../endpoints';
import { setAuthTokens } from '../apiError';
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  RefreshTokenRequest,
} from '../../types/auth';

export const authService = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>(API_ENDPOINTS.AUTH.LOGIN, data);
    const { accessToken, refreshToken } = response.data.value;
    setAuthTokens(accessToken, refreshToken);
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>(API_ENDPOINTS.AUTH.REGISTER, data);
    const { accessToken, refreshToken } = response.data.value;
    setAuthTokens(accessToken, refreshToken);
    return response.data;
  },

  refreshToken: (data: RefreshTokenRequest) =>
    api.post<AuthResponse>(API_ENDPOINTS.AUTH.REFRESH, data),
};
