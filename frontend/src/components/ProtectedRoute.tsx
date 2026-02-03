import { Navigate } from 'react-router-dom';
import { AUTH_STORAGE_KEYS } from '../constants';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const accessToken = localStorage.getItem(AUTH_STORAGE_KEYS.ACCESS_TOKEN);
  const refreshToken = localStorage.getItem(AUTH_STORAGE_KEYS.REFRESH_TOKEN);

  if (!accessToken || !refreshToken) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
}
