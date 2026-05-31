# APICatalogo

REST API for product and category catalog management, built with C# and ASP.NET Core (.NET 8/9). The project applies professional development practices including stateless JWT authentication, Repository Pattern, DTOs, pagination, custom filters, and Docker containerization.

---

## Technologies

- C# / ASP.NET Core (.NET 8/9)
- Entity Framework Core
- MySQL
- JWT Authentication (Access Token and Refresh Token)
- Swagger / OpenAPI
- Docker
- AutoMapper (DTOs)

---

## Architecture and Structure

```
APICatalogo/
├── Controllers/       # API endpoints and route handling
├── Models/            # Domain entities
├── DTOs/              # Data Transfer Objects
├── Repositories/      # Repository Pattern implementation
├── Services/          # Business logic layer
├── Context/           # EF Core DbContext configuration
├── Filters/           # Custom action filters
├── Logging/           # Logging configuration
├── Migrations/        # EF Core database migrations
├── Pagination/        # Pagination helpers
└── Program.cs         # Application entry point and DI setup
```

---

## Features

- Full CRUD for products and categories
- Stateless JWT authentication with Access Token and Refresh Token
- Repository Pattern for data access abstraction
- DTO layer with AutoMapper for input and output mapping
- Pagination and filtering on list endpoints
- Custom filters for cross-cutting concerns
- Swagger documentation with authentication support
- CORS and Rate Limiting configuration
- Docker support for containerized environments

---

## Getting Started

### Prerequisites

- .NET 8 SDK or higher
- MySQL server running locally or via Docker
- Docker (optional)

### Running locally

1. Clone the repository

```bash
git clone https://github.com/DiegoSoares22/APICatalogo.git
cd APICatalogo
```

2. Configure the connection string in `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=APICatalogoDB;User=root;Password=yourpassword;"
}
```

3. Apply migrations

```bash
dotnet ef database update
```

4. Run the application

```bash
dotnet run
```

5. Access Swagger at `https://localhost:{port}/swagger`

### Running with Docker

```bash
docker build -t apicatalogo .
docker run -p 8080:80 apicatalogo
```

---

## Authentication

The API uses JWT Bearer authentication. To access protected endpoints:

1. Register or log in via the auth endpoints
2. Copy the returned Access Token
3. In Swagger, click "Authorize" and enter: `Bearer {your_token}`

Refresh Token support is included for session renewal without re-authentication.

---

Diego Soares
Backend Developer, C# and ASP.NET Core

[Portfolio](https://diegosoares.vercel.app) · [LinkedIn](https://www.linkedin.com/in/diego-soaresdev/) · [GitHub](https://github.com/DiegoSoares22)
