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
    value: {
        accessToken: string;
        refreshToken: string;
        userRole: string;
    };
    isSuccess: boolean;
    statusCode: number;
    errors: Record<string, unknown>;
}

export interface RefreshTokenRequest {
    refreshToken: string;
}

export interface ApiError {
    message: string;
    code?: string;
    details?: Record<string, string>;
}
