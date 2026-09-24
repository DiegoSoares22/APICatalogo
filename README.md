# APICatalogo

REST API for product and category catalog management, built with C# and ASP.NET Core (.NET 8/9).

`C#` `ASP.NET Core` `Entity Framework Core` `MySQL` `JWT` `Docker` `Swagger`

## Highlights

- Full CRUD for products and categories, with pagination and filtering on list endpoints
- Stateless JWT authentication with access token and refresh token
- Repository Pattern for data access, DTOs mapped with AutoMapper
- Custom action filters for cross-cutting concerns, CORS and rate limiting
- Swagger documentation with authentication support
- Docker support for containerized environments

## Project structure

```
Controllers/   API endpoints
Models/        Domain entities
DTOs/          Data transfer objects
Repositories/  Repository Pattern implementation
Services/      Business logic
Context/       EF Core DbContext
Filters/       Custom action filters
Pagination/    Pagination helpers
Migrations/    EF Core migrations
```

## Running locally

Requirements: .NET 8 SDK and MySQL, local or via Docker.

```bash
git clone https://github.com/DiegoSoares22/APICatalogo.git
cd APICatalogo

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=APICatalogoDB;User=root;Password=yourpassword;"

dotnet ef database update
dotnet run
```

With Docker:

```bash
docker build -t apicatalogo .
docker run -p 8080:80 apicatalogo
```

Swagger runs at `https://localhost:{port}/swagger`. To call protected endpoints, log in, copy the access token and use **Authorize** with `Bearer {token}`.

## Sobre o projeto (PT-BR)

API REST para gestao de catalogo de produtos e categorias, construida em C# com ASP.NET Core (.NET 8/9).

- CRUD completo de produtos e categorias, com paginacao e filtros nos endpoints de listagem
- Autenticacao JWT stateless com access token e refresh token
- Repository Pattern para acesso a dados e DTOs mapeados com AutoMapper
- Filtros customizados, CORS e rate limiting
- Documentacao Swagger com suporte a autenticacao
- Suporte a Docker para ambientes containerizados

Requisitos para rodar: .NET 8 SDK e MySQL, local ou via Docker. A connection string e a chave JWT ficam em User Secrets, nunca versionadas.

---

**Diego Soares** - https://www.linkedin.com/in/diego-soaresdev/ - https://diegosoares.vercel.app
