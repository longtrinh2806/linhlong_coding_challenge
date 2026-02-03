import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button, Input, Card, Alert } from '../components/ui';
import { authService, getApiErrorMessage } from '../api';
import { t } from '../locales/i18n';

interface RegisterFormData {
    email: string;
    password: string;
    confirmPassword: string;
}

interface FormErrors {
    email?: string;
    password?: string;
    confirmPassword?: string;
}

export default function Register() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState<RegisterFormData>({
        email: '',
        password: '',
        confirmPassword: '',
    });
    const [errors, setErrors] = useState<FormErrors>({});
    const [serverError, setServerError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const translations = t();

    const validatePassword = (password: string): string | undefined => {
        if (!password) {
            return translations.errors.required;
        }
        if (password.length < 12) {
            return translations.errors.passwordTooShort;
        }
        if (!/[A-Z]/.test(password)) {
            return translations.errors.passwordNoUppercase;
        }
        if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
            return translations.errors.passwordNoSpecial;
        }
        return undefined;
    };

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

        const passwordError = validatePassword(formData.password);
        if (passwordError) {
            newErrors.password = passwordError;
            isValid = false;
        }

        if (!formData.confirmPassword) {
            newErrors.confirmPassword = translations.errors.required;
            isValid = false;
        } else if (formData.password !== formData.confirmPassword) {
            newErrors.confirmPassword = translations.errors.passwordMismatch;
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
            await authService.register({
                email: formData.email,
                password: formData.password,
                confirmPassword: formData.confirmPassword,
            });
            navigate('/');
        } catch (error) {
            setServerError(getApiErrorMessage(error));
            setFormData(prev => ({
                ...prev,
                password: '',
                confirmPassword: '',
            }));
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4">
            <Card>
                <div className="text-center mb-8">
                    <h1 className="text-3xl font-bold text-slate-800 mb-2">
                        {translations.register.title}
                    </h1>
                    <p className="text-slate-500">
                        {translations.register.subtitle}
                    </p>
                </div>

                {serverError && (
                    <Alert variant="error" className="mb-6">
                        {serverError}
                    </Alert>
                )}

                <form onSubmit={handleSubmit}>
                    <Input
                        label={translations.register.emailLabel}
                        type="email"
                        name="email"
                        placeholder={translations.register.emailPlaceholder}
                        value={formData.email}
                        onChange={handleChange}
                        error={errors.email}
                        autoComplete="email"
                    />

                    <Input
                        label={translations.register.passwordLabel}
                        type="password"
                        name="password"
                        placeholder={translations.register.passwordPlaceholder}
                        value={formData.password}
                        onChange={handleChange}
                        error={errors.password}
                        autoComplete="new-password"
                    />
                    <p className="text-xs text-slate-400 mb-4 -mt-2">
                        {translations.register.passwordHint}
                    </p>

                    <Input
                        label={translations.register.confirmPasswordLabel}
                        type="password"
                        name="confirmPassword"
                        placeholder={translations.register.confirmPasswordPlaceholder}
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        error={errors.confirmPassword}
                        autoComplete="new-password"
                    />

                    <Button
                        type="submit"
                        fullWidth
                        isLoading={isLoading}
                    >
                        {isLoading ? translations.register.loading : translations.register.registerButton}
                    </Button>
                </form>

                <div className="mt-6 text-center">
                    <p className="text-slate-500">
                        {translations.register.hasAccount}{' '}
                        <Link
                            to="/login"
                            className="text-blue-500 hover:text-blue-600 font-medium"
                        >
                            {translations.register.loginLink}
                        </Link>
                    </p>
                </div>
            </Card>
        </div>
    );
}
