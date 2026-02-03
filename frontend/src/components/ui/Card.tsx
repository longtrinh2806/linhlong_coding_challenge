import type { ReactNode } from 'react';

interface CardProps {
  children: ReactNode;
  className?: string;
}

export default function Card({ children, className = '' }: CardProps) {
  return (
    <div
      className={`
        bg-white rounded-xl shadow-lg
        px-10 py-12
        max-w-md w-full mx-auto
        ${className}
      `}
    >
      {children}
    </div>
  );
}
