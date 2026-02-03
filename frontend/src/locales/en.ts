export const en = {
  errors: {
    noRefreshToken: 'Session expired. Please log in again.',
    unauthorized: 'Unauthorized. Please log in again.',
    forbidden: 'You do not have permission to perform this action.',
    notFound: 'The requested resource was not found.',
    serverError: 'Server error. Please try again later.',
    networkError: 'Network error. Please check your connection.',
    unexpectedError: 'An unexpected error occurred.',
    loginFailed: 'Login failed. Please check your credentials.',
    registerFailed: 'Registration failed. Please try again.',
    sessionExpired: 'Your session has expired. Please log in again.',
  },
  auth: {
    loggingOut: 'Logging out...',
  },
} as const;

export type TranslationKey = keyof typeof en;
