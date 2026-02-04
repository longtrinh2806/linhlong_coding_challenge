# Pharma Identity Service - Technical Documentation

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Architecture Overview](#2-architecture-overview)
3. [Technology Stack](#3-technology-stack)
4. [Project Structure](#4-project-structure)
5. [Authentication Flow](#5-authentication-flow)
6. [Security Features](#6-security-features)
7. [API Endpoints](#7-api-endpoints)
8. [Database Schema](#8-database-schema)
9. [Frontend Architecture](#9-frontend-architecture)
10. [Configuration](#10-configuration)

---

## 1. Project Overview

Pharma Identity Service is a robust identity and authentication service built with modern technologies, implementing JWT-based authentication with refresh tokens. The system follows Clean Architecture principles and CQRS pattern for maintainable and scalable code.

### Key Features

- User Registration with secure password validation
- User Login with account lockout protection
- JWT-based authentication with short-lived access tokens
- Refresh token mechanism for seamless session management
- Role-based user management
- Redis-based token caching and session management
- Comprehensive error handling and logging

---

## 2. Architecture Overview

### Clean Architecture Layers

The project follows Clean Architecture with four main layers:

```
┌─────────────────────────────────────────────────┐
│           Presentation Layer (API)             │
│   Controllers, Middlewares, Extensions          │
├─────────────────────────────────────────────────┤
│           Application Layer                      │
│   CQRS Commands/Queries, DTOs, Behaviors        │
├─────────────────────────────────────────────────┤
│           Domain Layer                          │
│   Entities, Business Logic, Abstractions        │
├─────────────────────────────────────────────────┤
│           Infrastructure Layer                  │
│   Database, Repositories, Services, External    │
└─────────────────────────────────────────────────┘
```

### Data Flow

```
┌──────────┐    HTTP Request    ┌─────────────┐
│ Frontend │ ──────────────────▶ │   API Layer │
└──────────┘                     └──────┬──────┘
                                        │
                                        ▼
                               ┌─────────────────┐
                               │  MediatR (CQRS) │
                               └────────┬────────┘
                                        │
                    ┌───────────────────┼───────────────────┐
                    ▼                   ▼                   ▼
           ┌──────────────┐    ┌──────────────┐    ┌──────────────┐
           │   Handlers   │    │  Behaviors   │    │   Validators │
           │ (Business)   │    │ (Logging)    │    │ (Fluent)     │
           └──────┬───────┘    └──────────────┘    └──────────────┘
                    │
                    ▼
           ┌─────────────────┐
           │  Infrastructure │
           │ Repositories &  │
           │     Services    │
           └────────┬────────┘
                    │
         ┌──────────┼──────────┐
         ▼          ▼          ▼
   ┌──────────┐ ┌──────────┐ ┌──────────┐
   │ PostgreSQL│ │  Redis   │ │  JWT     │
   │ Database  │ │  Cache   │ │  Tokens  │
   └───────────┘ └──────────┘ └──────────┘
```

---

## 3. Technology Stack

### Backend (.NET)

| Component | Technology |
|-----------|------------|
| Framework | .NET 10 |
| API | ASP.NET Core Web API |
| Database | PostgreSQL 18 |
| ORM | Entity Framework Core |
| CQRS | MediatR |
| Validation | FluentValidation |
| JWT | System.IdentityModel.Tokens.Jwt |
| Caching | Redis 8.0.5 |
| Logging | Microsoft.Extensions.Logging |

### Frontend (React)

| Component | Technology |
|-----------|------------|
| Framework | React 19.2.0 |
| Language | TypeScript 5.9 |
| Build Tool | Vite 7.2.4 |
| Routing | React Router DOM 7.13.0 |
| HTTP Client | Axios 1.13.4 |
| Styling | Tailwind CSS 4.1.18 |
| ESLint | TypeScript ESLint |

### DevOps

| Component | Technology |
|-----------|------------|
| Containerization | Docker |
| Orchestration | Docker Compose |

---

## 4. Project Structure

### Backend Structure

```
pharma-identity-service/
├── Pharma.Identity.API/
│   ├── Controllers/
│   │   ├── AuthenticationController.cs
│   │   └── HealthController.cs
│   ├── Extensions/
│   │   └── OperationResultExtension.cs
│   ├── Middlewares/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Appsettings.json
│   ├── Program.cs
│   └── Dockerfile
│
├── Pharma.Identity.Application/
│   ├── Common/
│   │   ├── Abstractions/
│   │   │   ├── ICachingService.cs
│   │   │   ├── ICurrentUser.cs
│   │   │   ├── IEmailOtpService.cs
│   │   │   ├── IEncryptionService.cs
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── IHasher.cs
│   │   │   ├── IJwtTokenConfiguration.cs
│   │   │   ├── IJwtTokenService.cs
│   │   │   ├── IReadOnlyRepository.cs
│   │   │   ├── ITotpService.cs
│   │   │   └── IUnitOfWork.cs
│   │   ├── Behaviors/
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── ValidationBehavior.cs
│   │   ├── Models/
│   │   │   ├── Pagination.cs
│   │   │   └── PagingRequest.cs
│   │   └── OperationResult/
│   │       ├── BaseOperationResult.cs
│   │       ├── OperationResult.cs
│   │       └── OperationResultOf.cs
│   ├── Constant.cs
│   ├── DependencyInjection.cs
│   └── Features/
│       └── Authentication/
│           ├── Commands/
│           │   ├── Login.cs
│           │   ├── RefreshToken.cs
│           │   └── Register.cs
│           └── DTOs/
│               ├── LoginRequest.cs
│               ├── LoginResponse.cs
│               ├── RefreshTokenRequest.cs
│               ├── RegisterRequest.cs
│               ├── ResendEmailOtpRequest.cs
│               └── ValidateOtpRequest.cs
│
├── Pharma.Identity.Domain/
│   ├── Abstractions/
│   │   └── AuditableEntity.cs
│   ├── Entities/
│   │   ├── Role.cs
│   │   └── User.cs
│   └── Pharma.Identity.Domain.csproj
│
├── Pharma.Identity.Infrastructure/
│   ├── Configurations/
│   │   ├── CacheConfiguration.cs
│   │   ├── EncryptionConfiguration.cs
│   │   └── JwtTokenConfiguration.cs
│   ├── DependencyInjection.cs
│   ├── Migrations/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   │   ├── RoleConfiguration.cs
│   │   │   └── UserConfiguration.cs
│   │   ├── Interceptors/
│   │   │   └── AuditableEntityInterceptor.cs
│   │   ├── ApplicationDbContext.cs
│   │   ├── ReadOnlyDbContext.cs
│   │   └── UnitOfWork.cs
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   └── ReadOnlyRepository.cs
│   └── Services/
│       ├── CurrentUser.cs
│       ├── EmailOtpService.cs
│       ├── EncryptionService.cs
│       ├── Hasher.cs
│       ├── JwtTokenService.cs
│       ├── RedisCachingService.cs
│       └── TotpService.cs
│
├── docker-compose.yml
├── global.json
└── README.md
```

### Frontend Structure

```
frontend/
├── public/
│   └── vite.svg
├── src/
│   ├── api/
│   │   ├── axios.ts
│   │   ├── config.ts
│   │   ├── endpoints.ts
│   │   ├── apiError.ts
│   │   ├── index.ts
│   │   ├── interceptors/
│   │   │   ├── request.ts
│   │   │   └── response.ts
│   │   └── services/
│   │       ├── authService.ts
│   │       ├── healthService.ts
│   │       └── index.ts
│   ├── assets/
│   │   └── react.svg
│   ├── components/
│   │   ├── ProtectedRoute.tsx
│   │   └── ui/
│   │       ├── Alert.tsx
│   │       ├── Button.tsx
│   │       ├── Card.tsx
│   │       ├── Input.tsx
│   │       └── index.ts
│   ├── constants/
│   │   ├── auth.ts
│   │   └── index.ts
│   ├── locales/
│   │   ├── en.ts
│   │   ├── i18n.ts
│   │   └── index.ts
│   ├── pages/
│   │   ├── Home.tsx
│   │   ├── Login.tsx
│   │   ├── Register.tsx
│   │   └── index.ts
│   ├── types/
│   │   └── auth.ts
│   ├── App.tsx
│   ├── index.css
│   ├── main.tsx
│   └── vite-env.d.ts
├── .env
├── .gitignore
├── eslint.config.js
├── index.html
├── package.json
├── README.md
├── tsconfig.app.json
├── tsconfig.json
├── tsconfig.node.json
└── vite.config.ts
```

---

## 5. Authentication Flow

### Registration Flow

```
┌──────────┐         ┌──────────┐         ┌──────────┐         ┌──────────┐
│  User    │         │ Frontend │         │   API    │         │ Database│
└────┬─────┘         └────┬─────┘         └────┬─────┘         └────┬─────┘
     │                    │                    │                    │
     │  1. Fill form      │                    │                    │
     │◄───────────────────│                    │                    │
     │                    │                    │                    │
     │  2. Submit         │                    │                    │
     │───────────────────▶│                    │                    │
     │                    │                    │                    │
     │                    │  3. Validate       │                    │
     │                    │◄────────────────────│                    │
     │                    │                    │                    │
     │                    │  4. POST /register │                    │
     │                    │───────────────────▶│                    │
     │                    │                    │                    │
     │                    │                    │  5. Check existing │
     │                    │                    │◄───────────────────│
     │                    │                    │                    │
     │                    │                    │  6. Hash password  │
     │                    │                    │                    │
     │                    │                    │  7. Create user   │
     │                    │                    │───────────────────▶│
     │                    │                    │                    │
     │                    │                    │  8. Return success │
     │                    │◄────────────────────│                    │
     │                    │                    │                    │
     │  9. Success        │                    │                    │
     │◄───────────────────│                    │                    │
```

### Login Flow

```
┌──────────┐         ┌──────────┐         ┌──────────┐         ┌──────────┐
│  User    │         │ Frontend │         │   API    │         │ Database│
└────┬─────┘         └────┬─────┘         └────┬─────┘         └────┬─────┘
     │                    │                    │                    │
     │  1. Enter credentials                  │                    │
     │◄───────────────────│                    │                    │
     │                    │                    │                    │
     │  2. Submit        │                    │                    │
     │───────────────────▶│                    │                    │
     │                    │                    │                    │
     │                    │  3. POST /login   │                    │
     │                    │───────────────────▶│                    │
     │                    │                    │                    │
     │                    │                    │  4. Find user by  │
     │                    │                    │     email         │
     │                    │                    │◄───────────────────│
     │                    │                    │                    │
     │                    │                    │  5. Check lockout│
     │                    │                    │                    │
     │                    │                    │  6. Validate      │
     │                    │                    │     password      │
     │                    │                    │                    │
     │                    │                    │  7. Reset failed  │
     │                    │                    │     attempts      │
     │                    │                    │                    │
     │                    │                    │  8. Generate JWT │
     │                    │                    │     tokens        │
     │                    │                    │                    │
     │                    │                    │  9. Store refresh │
     │                    │                    │     in Redis      │
     │                    │                    │                    │
     │                    │                    │  10. Return tokens│
     │                    │◄────────────────────│                    │
     │                    │                    │                    │
     │  11. Store tokens  │                    │                    │
     │◄───────────────────│                    │                    │
     │                    │                    │                    │
```

### Token Refresh Flow

```
┌──────────┐         ┌──────────┐         ┌──────────┐         ┌──────────┐
│  User    │         │ Frontend │         │   API    │         │  Redis   │
└────┬─────┘         └────┬─────┘         └────┬─────┘         └────┬─────┘
     │                    │                    │                    │
     │  1. Access token   │                    │                    │
     │     expires        │                    │                    │
     │◄───────────────────│                    │                    │
     │                    │                    │                    │
     │  2. Call API with  │                    │                    │
     │     stale token    │                    │                    │
     │───────────────────▶│                    │                    │
     │                    │                    │                    │
     │                    │  3. Detect 401    │                    │
     │                    │     interceptor    │                    │
     │                    │                    │                    │
     │                    │  4. POST /refresh  │                    │
     │                    │───────────────────▶│                    │
     │                    │                    │                    │
     │                    │                    │  5. Validate      │
     │                    │                    │     refresh token │
     │                    │                    │◄───────────────────│
     │                    │                    │                    │
     │                    │                    │  6. Check token   │
     │                    │                    │     in cache      │
     │                    │                    │──────────────────▶│
     │                    │                    │                    │
     │                    │                    │  7. Return result │
     │                    │                    │◄──────────────────│
     │                    │                    │                    │
     │                    │                    │  8. Generate new  │
     │                    │                    │     tokens        │
     │                    │                    │                    │
     │                    │                    │  9. Update cache  │
     │                    │                    │──────────────────▶│
     │                    │                    │                    │
     │                    │                    │  10. Return new    │
     │                    │                    │     tokens        │
     │                    │◄────────────────────│                    │
     │                    │                    │                    │
     │  11. Store new     │                    │                    │
     │     tokens         │                    │                    │
     │◄───────────────────│                    │                    │
     │                    │                    │                    │
     │  12. Retry original │                    │                    │
     │     request        │                    │                    │
     │───────────────────▶│                    │                    │
```

---

## 6. Security Features

### Password Security

1. **Password Hashing**: Uses BCrypt for secure password storage
2. **Password Requirements**:
   - Minimum 12 characters
   - At least one uppercase letter
   - At least one special character
   - No empty passwords allowed

### Account Protection

1. **Failed Login Tracking**:
   - Maximum 5 failed attempts allowed
   - Account locked for 30 minutes after max attempts
   - Failed attempts and last failed time are tracked

2. **Account Lockout**:
   - Automatic locking after failed attempts
   - Time-based unlock mechanism
   - Failed attempts reset on successful login

### Token Security

1. **JWT Access Tokens**:
   - Short-lived (1 minute in development, configurable for production)
   - Contains userId and email claims
   - Token type claim identifies it as access token

2. **Refresh Tokens**:
   - Long-lived (7 days default)
   - Stored in Redis for server-side validation
   - New refresh token issued on each refresh (token rotation)
   - TTL preserved from original token

3. **Token Validation**:
   - Issuer and audience validation
   - Signature validation
   - No clock skew tolerance for refresh tokens
   - Token type verification

### Data Encryption

1. **Sensitive Data**: AES encryption for sensitive information
2. **Encryption Key**: Base64-encoded 256-bit key
3. **Encryption IV**: Base64-encoded initialization vector

### CORS Configuration

- Configurable cross-origin policies
- Development allows all origins, methods, and headers
- Production should restrict to specific origins

---

## 7. API Endpoints

### Authentication

#### Register User

```
POST /api/authentication/register
Content-Type: application/json

Request Body:
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}

Response (200 OK):
{
  "value": {
    "accessToken": "eyJhbGc...",
    "refreshToken": "eyJhbGc...",
    "userRole": "Viewer"
  },
  "isSuccess": true,
  "statusCode": 200,
  "errors": {}
}

Response (400 Bad Request):
{
  "value": null,
  "isSuccess": false,
  "statusCode": 400,
  "errors": {
    "message": "User with the given email already exists."
  }
}
```

#### Login

```
POST /api/authentication/login
Content-Type: application/json

Request Body:
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}

Response (200 OK):
{
  "value": {
    "accessToken": "eyJhbGc...",
    "refreshToken": "eyJhbGc...",
    "userRole": "Viewer"
  },
  "isSuccess": true,
  "statusCode": 200,
  "errors": {}
}

Response (400 Bad Request):
{
  "value": null,
  "isSuccess": false,
  "statusCode": 400,
  "errors": {
    "message": "Invalid email or password"
  }
}

Response (400 Bad Request - Locked Account):
{
  "value": null,
  "isSuccess": false,
  "statusCode": 400,
  "errors": {
    "message": "Account is locked. Try again in 15 minutes."
  }
}
```

#### Refresh Token

```
POST /api/authentication/refresh
Content-Type: application/json

Request Body:
{
  "refreshToken": "eyJhbGc..."
}

Response (200 OK):
{
  "value": {
    "accessToken": "eyJhbGc...",
    "refreshToken": "eyJhbGc...",
    "userRole": null
  },
  "isSuccess": true,
  "statusCode": 200,
  "errors": {}
}

Response (401 Unauthorized):
{
  "value": null,
  "isSuccess": false,
  "statusCode": 401,
  "errors": {
    "message": "Invalid or expired refresh token"
  }
}
```

### Health Check

#### Get Health Status

```
GET /api/health

Response:
Pharma Identity Service is healthy.
```

---

## 8. Database Schema

### User Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| UserId | varchar(26) | Primary Key | ULID identifier |
| RoleId | int | Foreign Key | References Role table |
| Email | nvarchar(max) | Unique, Required | User email address |
| Password | nvarchar(max) | Required | BCrypt hashed password |
| FirstName | nvarchar(max) | Optional | User first name |
| LastName | nvarchar(max) | Optional | User last name |
| IsAccountLocked | bit | Default: false | Account lock status |
| FailedLoginAttempts | int | Default: 0 | Failed login counter |
| LastFailedLoginAt | datetimeoffset | Optional | Last failed login time |
| LockedUntil | datetimeoffset | Optional | Lock expiration time |
| CreatedAt | datetimeoffset | Required | Record creation time |
| UpdatedAt | datetimeoffset | Optional | Last update time |

### Role Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| RoleId | int | Primary Key | Role identifier |
| Name | nvarchar(max) | Required | Role name (e.g., Viewer) |

### Default Roles

| RoleId | Name | Description |
|--------|------|-------------|
| 1 | Viewer | Read-only access |

---

## 9. Frontend Architecture

### Component Structure

```
frontend/src/
├── App.tsx                    # Main application with routing
├── main.tsx                   # Application entry point
├── api/
│   ├── axios.ts              # Axios instance with interceptors
│   ├── config.ts             # API configuration
│   ├── endpoints.ts          # API endpoint definitions
│   ├── apiError.ts          # Error handling utilities
│   ├── interceptors/
│   │   ├── request.ts       # Auth token injection
│   │   └── response.ts      # 401 handling & token refresh
│   └── services/
│       ├── authService.ts   # Authentication API calls
│       └── healthService.ts # Health check API calls
├── components/
│   ├── ProtectedRoute.tsx   # Route protection wrapper
│   └── ui/                  # Reusable UI components
├── pages/
│   ├── Home.tsx             # Protected home page
│   ├── Login.tsx            # Login page
│   └── Register.tsx         # Registration page
├── constants/
│   └── auth.ts              # Auth-related constants
├── types/
│   └── auth.ts              # TypeScript type definitions
└── locales/
    ├── i18n.ts              # Internationalization setup
    └── en.ts                # English translations
```

### State Management

1. **Local Storage**: Stores authentication tokens
2. **Component State**: React useState for form data
3. **HTTP Interceptors**: Automatic token injection and refresh

### Authentication State Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend Authentication                  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  localStorage:                                              │
│  ├── accessToken  (JWT access token)                        │
│  ├── refreshToken (JWT refresh token)                      │
│  ├── userEmail    (user's email for display)               │
│  └── userRole     (user's role for display)                │
│                                                             │
│  ProtectedRoute:                                            │
│  ├── Checks for both accessToken and refreshToken          │
│  ├── Redirects to /login if either is missing             │
│                                                             │
│  Request Interceptor:                                       │
│  ├── Adds Authorization: Bearer {accessToken} header       │
│                                                             │
│  Response Interceptor:                                      │
│  ├── Detects 401 Unauthorized responses                    │
│  ├── Attempts to refresh tokens using refreshToken         │
│  ├── Retries original request with new accessToken         │
│  ├── Redirects to /login if refresh fails                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Form Validation

1. **Login Form**:
   - Email: Required, valid email format
   - Password: Required

2. **Register Form**:
   - Email: Required, valid email format
   - Password: Required, min 12 chars, uppercase, special char
   - Confirm Password: Required, must match password

---

## 10. Configuration

### Environment Variables

#### Backend Configuration

| Variable | Description | Example |
|----------|-------------|---------|
| ASPNETCORE_ENVIRONMENT | Environment name | Development |
| DB_CONNECTION_STRING | PostgreSQL connection | Host=localhost;Port=5432;Database=pharma-db |
| READONLY_DB_CONNECTION_STRING | Read-only DB connection | Same as DB_CONNECTION_STRING |
| REDIS_CONNECTION_STRING | Redis connection | localhost:6379 |
| DEFAULT_CACHE_EXPIRATION_MINUTES | Cache TTL | 15 |
| JWT_ISSUER | JWT token issuer | Pharma.Identity |
| JWT_AUDIENCE | JWT token audience | Pharma.Clients |
| JWT_SECRET_KEY | JWT signing key (min 32 chars) | See launchSettings.json |
| JWT_ACCESS_TOKEN_EXPIRATION_MINUTES | Access token lifetime | 1 (60 for production) |
| JWT_REFRESH_TOKEN_EXPIRATION_DAYS | Refresh token lifetime | 7 |
| ENCRYPTION_KEY | Data encryption key (Base64) | See launchSettings.json |
| ENCRYPTION_IV | Encryption IV (Base64) | See launchSettings.json |

#### Frontend Configuration

| Variable | Description | Example |
|----------|-------------|---------|
| VITE_API_URL | Backend API URL | http://localhost:8080 |

### Docker Compose Services

```yaml
services:
  postgres:
    image: postgres:18
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: pharma-db
      POSTGRES_USER: longtk5
      POSTGRES_PASSWORD: 12345

  redis:
    image: redis:8.0.5
    ports:
      - "6379:6379"
```

---

## Appendix

### Code Style Guidelines

1. **Backend (.NET)**:
   - Follow Clean Architecture principles
   - Use CQRS pattern for business logic
   - Implement proper error handling
   - Use FluentValidation for input validation

2. **Frontend (React)**:
   - Use TypeScript for type safety
   - Follow React hooks best practices
   - Use functional components
   - Implement proper error boundaries

### Adding New Features

1. **Adding New API Endpoint**:
   - Create command/query in Application layer
   - Create controller method in API layer
   - Add endpoint documentation

2. **Adding New Entity**:
   - Create entity in Domain layer
   - Create configuration in Infrastructure layer
   - Add database migration
   - Create repository if needed

### Testing

1. **Unit Testing**:
   - Test business logic in Application layer
   - Use mocking for external dependencies

2. **Integration Testing**:
   - Test API endpoints with test database
   - Verify authentication flows

---

*Document generated for Pharma Identity Service*
*Last updated: February 2026*
