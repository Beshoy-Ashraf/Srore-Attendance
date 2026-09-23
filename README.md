# StoreAttendance

StoreAttendance is a .NET 10 clean-architecture API for retail staff attendance and store operations management. It supports attendance tracking, schedule approvals, mission workflows, store configuration, user management, and secure authentication.

## Overview

This solution is organized into the following projects:

- API: ASP.NET Core Web API, authentication, Swagger, controllers, and middleware
- Application: CQRS command/query handlers, DTOs, validation, and business logic
- Domain: entities, enums, repository contracts, and core exceptions
- Infrastructure: EF Core persistence, repositories, auth services, and database initialization

## Tech stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT authentication
- MediatR
- FluentValidation
- Swagger / OpenAPI

## Main features

- User authentication and JWT refresh-token flow
- Store management
- Staff and device tracking
- Attendance check-in/check-out logic
- Attendance settings per store
- Schedule creation and approval workflows
- Request and mission handling
- Role-based access control and scoped queries

## Project structure

```text
StoreAttendance/
├── API/
│   ├── Controllers/
│   ├── Contracts/
│   ├── Auth/
│   ├── Middlewares/
│   ├── Program.cs
│   └── appsettings*.json
├── Application/
│   ├── Attendance/
│   ├── Authentication/
│   ├── Devices/
│   ├── Missions/
│   ├── Requests/
│   ├── Schedules/
│   ├── Stores/
│   ├── Users/
│   └── Common/
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Exceptions/
│   ├── Interfaces/
│   └── Models/
├── Infrastructure/
│   ├── Identity/
│   ├── Persistence/
│   ├── Services/
│   └── DependencyInjection.cs
├── Directory.Build.props
├── Directory.Packages.props
├── Csproj.slnx
├── README.md
└── .gitignore
```

## Prerequisites

Before running the project, make sure you have:

- .NET 10 SDK
- PostgreSQL server running locally or in a reachable environment
- A database user with access to the configured database

## Configuration

The application reads its settings from the API project configuration files, mainly:

- API/appsettings.json
- API/appsettings.Development.json

Important settings include:

- ConnectionStrings:DefaultConnection
- JwtSettings:SecretKey, Issuer, Audience
- Attendance:TimeZoneId
- Seed:AdminEmail, AdminPassword, AdminUsername
- Cors:AllowedOrigins

For local development, you can use user secrets instead of storing sensitive values in source-controlled config files.

## Getting started

1. Restore dependencies:

```bash
dotnet restore
```

2. Update the database connection string and JWT settings in the appsettings file or user secrets.

3. Apply migrations and create the database:

```bash
dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj
```

If `dotnet-ef` is not installed yet:

```bash
dotnet tool install --global dotnet-ef
```

4. Run the API:

```bash
dotnet run --project API/API.csproj
```

5. Open the Swagger UI in the browser at the root URL while the app is running.

## Authentication

The API uses JWT bearer authentication with refresh tokens. The main authentication endpoints are:

- POST /api/auth/login
- POST /api/auth/refresh
- POST /api/auth/logout

Protected endpoints require a valid Bearer token in the Authorization header.

## API modules

The API exposes domain-focused modules such as:

- Attendance
- AttendanceSettings
- Auth
- Devices
- Missions
- Requests
- Schedules
- Stores
- Users

These modules are organized by feature and follow the CQRS pattern in the Application layer.

## Notes

- The solution uses a clean architecture structure to keep business rules away from the web layer.
- Database initialization is handled through infrastructure services, and the app runs startup initialization automatically.
- Exception handling middleware centralizes HTTP-friendly error responses.

## License

This project does not currently include a license file. Add one before publishing or sharing the repository publicly.
