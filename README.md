# UserServiceCrudGrpc

gRPC User Management Service built with:

- .NET 9
- Clean Architecture
- CQRS with MediatR
- gRPC
- EF Core + SQLite
- FluentValidation
- Serilog
- OpenTelemetry
- Health Checks
- Structured Logging
- Concurrency Handling
- Soft Delete + Hard Delete
- Filtering + Pagination + Sorting
- Integration-ready architecture

---

# Architecture

Solution follows Clean Architecture principles:

```text
Api
Application
Domain
Infrastructure
IntegrationTests
```

---

# Tech Stack

| Technology | Purpose |
|---|---|
| .NET 9 | Runtime |
| gRPC | Transport |
| MediatR | CQRS |
| EF Core | ORM |
| SQLite | Database |
| FluentValidation | Validation |
| Serilog | Structured Logging |
| OpenTelemetry | Metrics + Tracing |
| HealthChecks | Service Monitoring |

---

# Features

## User CRUD

### Create User
- Unique NationalCode validation
- Validation pipeline
- Transaction handling
- Concurrency-safe

### Get User
- By UserId

### Get All Users
Supports:

- Pagination
- Filtering
- Sorting
- Search

### Update User
- Full update
- Optimistic concurrency
- Transaction support
- Version checking

### Soft Delete
- Marks entity deleted
- Keeps data in database

### Hard Delete
- Permanently removes row

---

# Database Design

## Users Table

| Column | Type |
|---|---|
| UserId | UUID |
| FirstName | TEXT |
| LastName | TEXT |
| NationalCode | TEXT UNIQUE |
| BirthDate | DATETIME NULL |
| IsDeleted | BOOLEAN |
| CreatedAtUtc | DATETIME |
| UpdatedAtUtc | DATETIME |
| DeletedAtUtc | DATETIME NULL |
| Version | UUID |

---

# Concurrency Handling

Uses optimistic concurrency.

Each row contains:

```text
Version : Guid
```

Client must send latest version during update.

If version mismatch occurs:

```text
FAILED_PRECONDITION
```

is returned.

Works across:
- threads
- instances
- containers
- scaled deployments

---

# CQRS

Implemented using MediatR.

## Commands

- CreateUserCommand
- UpdateUserCommand
- SoftDeleteUserCommand
- HardDeleteUserCommand

## Queries

- GetUserByIdQuery
- GetUsersQuery

---

# Validation Pipeline

Implemented centrally using:

```text
FluentValidation + MediatR PipelineBehavior
```

Benefits:

- No duplicated validation
- Clean handlers
- Centralized validation logic
- Production-grade request validation

---

# Logging

Implemented using:

```text
Serilog
```

Features:
- Structured logging
- JSON logs
- Request correlation
- Exception logging
- gRPC interceptor logging

---

# OpenTelemetry

Integrated for:

## Tracing
Tracks:
- gRPC requests
- EF Core queries
- request lifecycle

## Metrics
Tracks:
- runtime metrics
- GC
- thread pool
- request timings

## Health Monitoring
Supports future:
- Prometheus
- Jaeger
- Grafana
- Tempo

No external setup required currently.

For using it, uncomment it inside Program.cs.
It is commented due to avoid high-load console printing and unwanted and unnecessary data prints.
Console exporter can be enabled.

---

# gRPC Interceptors

Implemented:

## LoggingInterceptor
Logs:
- request
- response
- duration
- exceptions

## ExceptionInterceptor
Maps exceptions to proper gRPC status codes.

---

# Exception Mapping

| Exception | gRPC Status |
|---|---|
| ValidationException | InvalidArgument |
| NotFoundException | NotFound |
| AlreadyExistsException | AlreadyExists |
| ConcurrencyException | FailedPrecondition |
| Unknown Exception | Internal |

---

# Filtering / Pagination / Sorting

Supported in GetUsers.

## Pagination

```text
page
pageSize
```

## Sorting

```text
sortBy
sortDirection
```

## Filtering

```text
firstName
lastName
nationalCode
```

---

# Soft Delete

Soft deleted users:
- remain in DB
- excluded from queries

---

# Health Checks

Endpoint:

```text
/health
```

Used for:
- Docker health probes
- Kubernetes readiness/liveness
- monitoring systems

---

# Running The Project

## Restore Packages

```bash
dotnet restore
```

---

# Run Migrations

## Create Migration

```bash
dotnet ef migrations add InitialCreate -p Infrastructure -s Api
```

## Apply Migration

```bash
dotnet ef database update -p Infrastructure -s Api
```

---

# Run Service

```bash
dotnet run --project Api
```

Default:

```text
http://localhost:5247
```

---

# Testing gRPC

Use:
- grpcurl
- Postman
- custom .NET client

---

# Example grpcurl

## Create User

```bash
grpcurl -plaintext \
-d '{
  "firstName":"Navid",
  "lastName":"FN",
  "nationalCode":"123456",
  "birthDate":"1999-01-01T00:00:00Z"
}' \
localhost:5247 user.UserService/CreateUser
```

---

# Docker - Not Tested Due To Internet Blockout (Recommend To Use Traditional Dot Net Build)

## Build Image

```bash
docker build -t rira-userservice .
```

## Run Container

```bash
docker run -p 8080:8080 rira-userservice
```

---

# Production-Grade Concepts Implemented

## Clean Architecture
Clear layer separation.

## CQRS
Read/write separation.

## Validation Pipeline
Centralized validation.

## Structured Logging
Machine-readable logs.

## Observability
Tracing + metrics.

## Concurrency Safety
Optimistic concurrency.

## Transaction Safety
Database transactions for writes.

## gRPC Interceptors
Cross-cutting concerns centralized.

## Soft Delete
Audit-friendly deletion strategy.

## Pagination
Scalable querying.

## SQLite
Lightweight embedded DB.

---

# Future Improvements

Possible next upgrades:

- Redis caching
- JWT Authentication
- Refresh Tokens
- Role-based Authorization
- API Gateway
- Kubernetes deployment
- Prometheus + Grafana
- Jaeger tracing
- Outbox Pattern
- Domain Events
- Event Bus
- RabbitMQ/Kafka
- CI/CD pipelines
- Distributed caching
- Retry policies
- Rate limiting

---

## Why CQRS?
- Separation of concerns
- Cleaner scaling path
- Better maintainability

## Why NOT full event-driven now?
Current scope does not require asynchronous distributed workflows.

Would be overengineering.

## Why FluentValidation?
- Clean handlers
- Reusable validation
- Centralized rules
- Better testing

## Why OpenTelemetry?
Industry standard observability.

Future-proof monitoring stack.

## Why Optimistic Concurrency?
Best approach for distributed systems without locking.

---

# Author Notes

This project intentionally focuses on:

- maintainability
- scalability
- production-readiness
- clean boundaries
- observability

# Navid Firooznia
