# Pharma Identity Service

A robust identity and authentication service built with .NET 9, implementing JWT-based authentication with refresh tokens, using Clean Architecture principles and CQRS pattern.

## 🏗️ Architecture

This project follows **Clean Architecture** with the following layers:

- **Pharma.Identity.API** - Presentation layer (Controllers, Middlewares)
- **Pharma.Identity.Application** - Application logic (CQRS Commands/Queries, DTOs, Behaviors)
- **Pharma.Identity.Domain** - Domain entities and business logic
- **Pharma.Identity.Infrastructure** - External concerns (Database, Repositories, Services)

## 🛠️ Tech Stack

- **.NET 9** - Latest .NET framework
- **PostgreSQL 18** - Primary database
- **Redis 8.0.5** - Caching and token management
- **Entity Framework Core** - ORM
- **MediatR** - CQRS pattern implementation
- **JWT** - Token-based authentication
- **Docker & Docker Compose** - Containerization

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [PostgreSQL 18](https://www.postgresql.org/download/) (optional if using Docker)
- [Redis](https://redis.io/download/) (optional if using Docker)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd pharma-identity-service
```

### 2. Start Infrastructure Services

Start PostgreSQL and Redis using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- **PostgreSQL** on port `5432`
- **Redis** on port `6379`

### 3. Configure Environment Variables

Configuration is managed through environment variables in `Pharma.Identity.API/Properties/launchSettings.json`:

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:8080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DB_CONNECTION_STRING": "Host=localhost;Port=5432;Database=pharma-db;Username=longtk5;Password=12345;SSL Mode=Disable;Include Error Detail=true",
        "READONLY_DB_CONNECTION_STRING": "Host=localhost;Port=5432;Database=pharma-db;Username=longtk5;Password=12345;SSL Mode=Disable;Include Error Detail=true",
        "REDIS_CONNECTION_STRING": "localhost:6379",
        "DEFAULT_CACHE_EXPIRATION_MINUTES": "15",
        "JWT_ISSUER": "Pharma.Identity",
        "JWT_AUDIENCE": "Pharma.Clients",
        "JWT_SECRET_KEY": "YourSuperSecretKey123456789~!@#$%^&*()_++_)(*&^%$#@!~~!@#$%^&*()_++_)(*&^%$#@!~",
        "JWT_ACCESS_TOKEN_EXPIRATION_MINUTES": "1",
        "JWT_REFRESH_TOKEN_EXPIRATION_DAYS": "7",
        "ENCRYPTION_KEY": "HQ6Q3Z0S1Ap9Iq+x4idcu/NGCTN1h4NpZt6o8wB6Cmw=",
        "ENCRYPTION_IV": "5+ctVfuAm8eRXJJvnDa59g=="
      }
    }
  }
}
```

> **Note:** The JWT secret key must be at least 32 characters long for security.

### 4. Run Database Migrations

```bash
cd Pharma.Identity.API
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger

## 📚 API Endpoints

### Authentication

#### Register
```http
POST /api/authentication/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

**Response:**
```json
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
```

#### Login
```http
POST /api/authentication/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

#### Refresh Token
```http
POST /api/authentication/refresh
Content-Type: application/json

{
  "refreshToken": "eyJhbGc..."
}
```

### Health Check
```http
GET /api/health
```

**Response:**
```
Pharma Identity Service is healthy.
```

## 🔒 Security Features

- **Password Hashing** - Secure password storage with BCrypt
- **JWT Access Tokens** - Short-lived tokens (1 minute in dev, configurable for production)
- **Refresh Tokens** - Long-lived tokens (7 days default) stored in Redis
- **Token Rotation** - New refresh token issued on each refresh
- **Data Encryption** - Sensitive data encryption with AES
- **CORS Configuration** - Configurable cross-origin policies

## 🗂️ Project Structure

```
pharma-identity-service/
├── Pharma.Identity.API/
│   ├── Controllers/         # API Controllers
│   ├── Middlewares/         # Exception handling, logging
│   ├── Extensions/          # Helper extensions
│   └── Program.cs           # Application entry point
├── Pharma.Identity.Application/
│   ├── Features/            # CQRS Commands/Queries
│   ├── Common/              # Shared DTOs, behaviors
│   └── DependencyInjection.cs
├── Pharma.Identity.Domain/
│   ├── Entities/            # Domain models
│   └── Abstractions/        # Domain interfaces
├── Pharma.Identity.Infrastructure/
│   ├── Persistence/         # DbContext, configurations
│   ├── Repositories/        # Data access
│   ├── Services/            # External services (JWT, Redis)
│   └── Migrations/          # EF Core migrations
└── docker-compose.yml       # Infrastructure services
```

## 🔧 Development

### Adding Migrations

```bash
cd Pharma.Identity.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../Pharma.Identity.API
```

### Code Standards

- Follow Clean Architecture principles
- Use CQRS pattern for business logic
- Implement proper error handling
- Write unit tests for critical business logic

## 🐳 Docker Deployment

### Build Docker Image

```bash
docker build -t pharma-identity-service -f Pharma.Identity.API/Dockerfile .
```

### Run with Docker Compose

```bash
docker-compose up --build
```

## 📝 Environment Variables

All configuration is done through environment variables defined in `launchSettings.json`:

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Development` |
| `DB_CONNECTION_STRING` | PostgreSQL connection string | `Host=localhost;Port=5432;Database=pharma-db;Username=longtk5;Password=12345` |
| `READONLY_DB_CONNECTION_STRING` | Read-only DB connection | Same as DB_CONNECTION_STRING |
| `REDIS_CONNECTION_STRING` | Redis connection | `localhost:6379` |
| `DEFAULT_CACHE_EXPIRATION_MINUTES` | Cache TTL | `15` |
| `JWT_ISSUER` | JWT token issuer | `Pharma.Identity` |
| `JWT_AUDIENCE` | JWT token audience | `Pharma.Clients` |
| `JWT_SECRET_KEY` | JWT signing key (min 32 chars) | See launchSettings.json |
| `JWT_ACCESS_TOKEN_EXPIRATION_MINUTES` | Access token lifetime | `1` (60 for production) |
| `JWT_REFRESH_TOKEN_EXPIRATION_DAYS` | Refresh token lifetime | `7` |
| `ENCRYPTION_KEY` | Data encryption key (Base64) | See launchSettings.json |
| `ENCRYPTION_IV` | Encryption IV (Base64) | See launchSettings.json |

> **Security Note:** Never commit sensitive values to version control. Use environment-specific configurations or secret managers in production.