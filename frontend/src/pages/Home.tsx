import { useEffect, useState } from 'react';
import { Card, Button, Alert } from '../components/ui';
import { healthService } from '../api';
import { AUTH_STORAGE_KEYS } from '../constants';

interface UserInfo {
  email: string;
  role: string;
}

interface HealthStatus {
  status: 'healthy' | 'unhealthy' | null;
  message: string;
  timestamp?: string;
  version?: string;
}

export default function Home() {
  const [userInfo, setUserInfo] = useState<UserInfo | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [healthStatus, setHealthStatus] = useState<HealthStatus>({
    status: null,
    message: 'Click the button to check API health',
  });
  const [isCheckingHealth, setIsCheckingHealth] = useState(false);

  useEffect(() => {
    // Get user info from localStorage (set during login)
    const userEmail = localStorage.getItem('userEmail');
    const userRole = localStorage.getItem('userRole');

    if (userEmail && userRole) {
      setUserInfo({
        email: userEmail,
        role: userRole,
      });
    } else {
      setError('User information not found. Please log in again.');
    }
  }, []);

  const handleCheckHealth = async () => {
    setIsCheckingHealth(true);
    setHealthStatus(prev => ({ ...prev, message: 'Checking...' }));

    try {
      const response = await healthService.check();
      const message = response.data;

      setHealthStatus({
        status: 'healthy',
        message,
        timestamp: new Date().toISOString(),
      });
    } catch (err) {
      setHealthStatus({
        status: 'unhealthy',
        message: 'Failed to connect to API',
      });
    } finally {
      setIsCheckingHealth(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem(AUTH_STORAGE_KEYS.ACCESS_TOKEN);
    localStorage.removeItem(AUTH_STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem('userEmail');
    localStorage.removeItem('userRole');
    window.location.href = '/login';
  };

  const getRoleBadgeColor = (role: string): string => {
    switch (role.toLowerCase()) {
      case 'admin':
        return 'bg-purple-100 text-purple-700';
      case 'editor':
        return 'bg-blue-100 text-blue-700';
      case 'viewer':
        return 'bg-gray-100 text-gray-700';
      default:
        return 'bg-gray-100 text-gray-700';
    }
  };

  const getHealthBadgeColor = (status: string | null): string => {
    if (status === null) return 'bg-gray-100 text-gray-700';
    return status === 'healthy'
      ? 'bg-green-100 text-green-700'
      : 'bg-red-100 text-red-700';
  };

  if (error) {
    return (
      <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4">
        <Card className="max-w-md">
          <Alert variant="error">{error}</Alert>
          <div className="mt-4 text-center">
            <Button onClick={handleLogout} variant="secondary">
              Go to Login
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  if (!userInfo) {
    return (
      <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4">
        <Card className="max-w-md text-center">
          <p className="text-slate-500">Loading...</p>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 p-8">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold text-slate-800">Dashboard</h1>
            <p className="text-slate-500 mt-1">Welcome back!</p>
          </div>
          <Button variant="secondary" onClick={handleLogout}>
            Logout
          </Button>
        </div>

        {/* User Info Card */}
        <Card className="mb-6">
          <div className="flex items-center gap-6">
            <div className="w-16 h-16 bg-blue-500 rounded-full flex items-center justify-center">
              <span className="text-white text-2xl font-bold">
                {userInfo.email.charAt(0).toUpperCase()}
              </span>
            </div>
            <div className="flex-1">
              <h2 className="text-xl font-semibold text-slate-800">
                {userInfo.email}
              </h2>
              <p className="text-slate-500 text-sm mt-1">
                Your assigned role
              </p>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-slate-600 text-sm">Role:</span>
              <span
                className={`px-4 py-2 rounded-full text-sm font-medium ${getRoleBadgeColor(
                  userInfo.role
                )}`}
              >
                {userInfo.role}
              </span>
            </div>
          </div>
        </Card>

        {/* Health Check Card */}
        <Card className="mb-6">
          <h3 className="text-lg font-semibold text-slate-800 mb-4">
            API Health Check
          </h3>
          <div className="flex items-center gap-4 mb-4">
            <Button
              onClick={handleCheckHealth}
              isLoading={isCheckingHealth}
              disabled={isCheckingHealth}
            >
              Check Health
            </Button>
            {healthStatus.status && (
              <span
                className={`px-4 py-2 rounded-full text-sm font-medium ${getHealthBadgeColor(
                  healthStatus.status
                )}`}
              >
                {healthStatus.status === 'healthy' ? 'Healthy' : 'Unhealthy'}
              </span>
            )}
          </div>
          <div className="bg-slate-50 rounded-lg p-4">
            <p className="text-slate-700">{healthStatus.message}</p>
            {healthStatus.timestamp && (
              <p className="text-slate-500 text-sm mt-2">
                Last checked: {new Date(healthStatus.timestamp).toLocaleString()}
              </p>
            )}
            {healthStatus.version && (
              <p className="text-slate-500 text-sm">
                Version: {healthStatus.version}
              </p>
            )}
          </div>
        </Card>
      </div>
    </div>
  );
}
