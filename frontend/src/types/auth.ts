export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    confirmPassword: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    userRole: string;
}

export interface RefreshTokenRequest {
    refreshToken: string;
}

export interface ApiError {
    message: string;
    code?: string;
    details?: Record<string, string>;
}
