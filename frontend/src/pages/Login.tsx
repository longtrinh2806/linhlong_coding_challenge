import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button, Input, Card, Alert } from '../components/ui';
import { authService, getApiErrorMessage } from '../api';
import { t } from '../locales/i18n';

interface LoginFormData {
  email: string;
  password: string;
}

interface FormErrors {
  email?: string;
  password?: string;
}

export default function Login() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<LoginFormData>({
    email: '',
    password: '',
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [serverError, setServerError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const translations = t();

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};
    let isValid = true;

    if (!formData.email) {
      newErrors.email = translations.errors.required;
      isValid = false;
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = translations.errors.invalidEmail;
      isValid = false;
    }

    if (!formData.password) {
      newErrors.password = translations.errors.required;
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    setErrors(prev => ({ ...prev, [name]: undefined }));
    setServerError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setServerError(null);

    if (!validateForm()) return;

    setIsLoading(true);

    try {
      const response = await authService.login(formData);

      // Store user info for Home page display
      localStorage.setItem('userEmail', formData.email);
      localStorage.setItem('userRole', response.value.userRole);

      navigate('/home');
    } catch (error) {
      setServerError(getApiErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4">
      <Card>
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-slate-800 mb-2">
            {translations.login.title}
          </h1>
          <p className="text-slate-500">
            {translations.login.subtitle}
          </p>
        </div>

        {serverError && (
          <Alert variant="error" className="mb-6">
            {serverError}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <Input
            label={translations.login.emailLabel}
            type="email"
            name="email"
            placeholder={translations.login.emailPlaceholder}
            value={formData.email}
            onChange={handleChange}
            error={errors.email}
            autoComplete="email"
          />

          <Input
            label={translations.login.passwordLabel}
            type="password"
            name="password"
            placeholder={translations.login.passwordPlaceholder}
            value={formData.password}
            onChange={handleChange}
            error={errors.password}
            autoComplete="current-password"
          />

          <div className="text-right mb-6">
            <button
              type="button"
              className="text-sm text-blue-500 hover:text-blue-600 font-medium"
            >
              {translations.login.forgotPassword}
            </button>
          </div>

          <Button
            type="submit"
            fullWidth
            isLoading={isLoading}
          >
            {isLoading ? translations.login.loading : translations.login.loginButton}
          </Button>
        </form>

        <div className="mt-6 text-center">
          <p className="text-slate-500">
            {translations.login.noAccount}{' '}
            <Link
              to="/register"
              className="text-blue-500 hover:text-blue-600 font-medium"
            >
              {translations.login.registerLink}
            </Link>
          </p>
        </div>
      </Card>
    </div>
  );
}
