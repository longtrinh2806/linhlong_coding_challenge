# React + TypeScript + Tailwind CSS

A modern React frontend project setup with TypeScript and Tailwind CSS v4.

## Features

- ⚡️ **Vite** - Fast build tool and dev server
- 🔷 **TypeScript** - Type-safe JavaScript
- 🎨 **Tailwind CSS v4** - Utility-first CSS framework
- 📦 **ESLint + Prettier** - Code linting and formatting

## Project Structure

```
src/
├── assets/          # Static assets (images, fonts)
├── components/      # Reusable UI components
├── hooks/           # Custom React hooks
├── pages/           # Page components
├── types/           # TypeScript type definitions
├── utils/           # Utility functions
├── App.tsx          # Root component
└── main.tsx         # Entry point
```

## Getting Started

### Prerequisites

- Node.js 18+
- npm or yarn

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

### Build

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## Path Aliases

Use `@` to import from `src`:

```typescript
import Button from '@/components/Button';
import { formatDate } from '@/utils/date';
```

## Environment Variables

Create a `.env` file based on `.env.example`:

```env
VITE_API_URL=http://localhost:3000/api
VITE_APP_NAME=My React App
```

Access in code:

```typescript
import.meta.env.VITE_API_URL;
```
