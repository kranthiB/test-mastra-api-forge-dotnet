# ApiForge — .NET 10 Production API Template

A **Clean Architecture**, production-grade REST API skeleton for .NET 10.
Use it as your team's starting point whenever a new CRUD service is needed.

---

## Architecture

```
ApiForge.Domain          ←  Pure domain entities, NO external deps
ApiForge.Application     ←  Use cases, DTOs, validators, service interfaces
ApiForge.Infrastructure  ←  Repository implementations (in-memory → swap for EF Core)
ApiForge.Api             ←  Controllers, middleware, DI wiring, Swagger, health checks
```

Dependency rule: **outer layers depend on inner layers, never the reverse.**

---

## What's included

| Concern | Implementation |
|---|---|
| CRUD reference | `ProductsController` + `ProductService` |
| Result pattern | `Result<T>` — no exception-driven control flow |
| Pagination | `PagedResult<T>` envelope on all list endpoints |
| Input validation | FluentValidation (auto-wired, fires before the action) |
| Error responses | RFC 7807 ProblemDetails via `ExceptionHandlingMiddleware` |
| Structured logging | Serilog (console + rolling file) |
| API versioning | URL segment (`/api/v1/…`) + `X-Api-Version` header |
| OpenAPI docs | Swashbuckle — `/swagger` in Development |
| Health checks | `/health` and `/health/ready` |
| Container | Multi-stage Dockerfile + docker-compose |

---

## Quick start

```bash
# 1. Restore and run
cd ApiForge.Api
dotnet run

# 2. Open Swagger UI
open http://localhost:5000/swagger

# 3. Try a request
curl http://localhost:5000/api/v1/products
```

### Docker

```bash
docker-compose up --build
# API available at http://localhost:8080
# Health check at http://localhost:8080/health
```

---

## Endpoints (v1)

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/products?page=1&pageSize=10&isActive=true` | Paged list |
| GET | `/api/v1/products/{id}` | Single product |
| POST | `/api/v1/products` | Create |
| PUT | `/api/v1/products/{id}` | Full update |
| DELETE | `/api/v1/products/{id}` | Delete |
| GET | `/health` | Structured health check |

---

## Adding a new resource — checklist

1. **Domain** — add entity in `ApiForge.Domain/<Resource>/`
2. **Application** — add DTOs, `I<Resource>Repository`, `I<Resource>Service`, validators, service impl
3. **Infrastructure** — add repository impl in `Persistence/Repositories/`, register in `DependencyInjection.cs`
4. **API** — copy `ProductsController`, rename, wire up the new service

---

## Swapping the database

The in-memory repositories implement the `IRepository<T>` / `IProductRepository` interfaces.
To switch to EF Core (or any other store):

1. Create `EfProductRepository : IProductRepository` in Infrastructure
2. Update the registration in `ApiForge.Infrastructure/DependencyInjection.cs`:
   ```csharp
   services.AddScoped<IProductRepository, EfProductRepository>();
   ```
3. No changes needed in Application or Domain.

---

## Conventions used throughout

- File-scoped namespaces
- `sealed` on all concrete service / repository classes
- `record` types for DTOs (value equality, immutability)
- All service methods return `Result<T>` — never throw for business errors
- `CancellationToken` threaded through every async call
- Seeded in-memory data so Swagger works immediately out of the box
