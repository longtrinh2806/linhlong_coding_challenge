import type { ReactNode } from 'react';

interface AlertProps {
  children: ReactNode;
  variant?: 'error' | 'success' | 'info';
  className?: string;
}

export default function Alert({ children, variant = 'error', className = '' }: AlertProps) {
  const variants = {
    error: 'bg-red-50 text-red-600 border-red-100',
    success: 'bg-green-50 text-green-600 border-green-100',
    info: 'bg-blue-50 text-blue-600 border-blue-100',
  };

  return (
    <div
      className={`
        px-4 py-3 rounded-lg text-sm border
        ${variants[variant]}
        ${className}
      `}
    >
      {children}
    </div>
  );
}
