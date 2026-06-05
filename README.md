# FleetOps

## Overview

![CI](https://github.com/MikyGus/FleetOps/actions/workflows/ci.yml/badge.svg)

FleetOps is a backend portfolio project built with **ASP.NET Core**, **PostgreSQL**, **EF Core**, and **Clean Architecture**.

The system models a small fleet scheduling domain where **drivers** are assigned to **vehicles** for specific time intervals.

The main technical focus is **data integrity**:

* a driver cannot be double-booked
* a vehicle cannot be double-booked
* invalid time ranges are rejected
* assignments must reference existing drivers and vehicles
* critical rules are enforced both in application validation and at database level

---

## Why this project exists

FleetOps is built as a structured backend portfolio project.

The goal is not to create a large product, but to demonstrate professional backend engineering practices in a focused domain:

* Clean Architecture
* explicit dependency direction
* PostgreSQL-backed data integrity
* EF Core migrations and mapping
* API validation and structured error responses
* unit tests and integration tests
* GitHub Actions CI

The project is intentionally small enough to review, but complex enough to show real backend design decisions.

---

## Domain overview

FleetOps currently supports three core concepts:

### Drivers

A driver represents a person who can be assigned to vehicles.

Current fields:

* `Id`
* `Name`
* `IsActive`

### Vehicles

A vehicle represents something that can be assigned to a driver.

Current fields:

* `Id`
* `RegistrationNumber`
* `IsActive`

### Assignments

An assignment connects one driver to one vehicle during a time interval.

Current fields:

* `Id`
* `DriverId`
* `VehicleId`
* `StartUtc`
* `EndUtc`

Assignments use `[start, end)` interval semantics.

This means:

* an assignment from `10:00` to `12:00`
* and another from `12:00` to `14:00`

do **not** overlap.

---

## Key features

### Driver endpoints

* create drivers
* get driver by id
* list drivers
* filter drivers by name
* filter drivers by active status
* pagination with `limit` and `offset`

### Vehicle endpoints

* create vehicles
* get vehicle by id
* list vehicles
* filter vehicles by registration number
* filter vehicles by active status
* pagination with `limit` and `offset`

### Assignment endpoints

* create assignments
* get assignment by id
* list assignments
* filter assignments by driver
* filter assignments by vehicle
* filter assignments by date range
* pagination with `limit` and `offset`

### Data integrity rules

FleetOps enforces:

* `EndUtc` must be greater than `StartUtc`
* a driver cannot have overlapping assignments
* a vehicle cannot have overlapping assignments
* an assignment must reference an existing driver
* an assignment must reference an existing vehicle

Some rules are validated in the application layer. Critical rules are also enforced by PostgreSQL constraints.

---

## Architecture

FleetOps follows a Clean Architecture-inspired structure:

```text
src/
  FleetOps.Api
  FleetOps.Application
  FleetOps.Domain
  FleetOps.Infrastructure

tests/
  FleetOps.Tests.Unit
  FleetOps.Tests.Integration
```

### FleetOps.Domain

Contains the core business model.

Responsibilities:

* domain entities
* domain invariants
* framework-independent business concepts

The Domain layer does not depend on ASP.NET Core, EF Core, or PostgreSQL.

### FleetOps.Application

Contains use cases and application-level rules.

Responsibilities:

* commands
* queries
* handlers
* validation
* repository/query interfaces
* application DTOs

The Application layer depends on Domain, but not on Infrastructure or API.

### FleetOps.Infrastructure

Contains external implementation details.

Responsibilities:

* EF Core `DbContext`
* PostgreSQL persistence
* repository implementations
* query implementations
* migrations
* database constraints

Infrastructure depends on Application and Domain.

### FleetOps.Api

Contains the HTTP interface.

Responsibilities:

* controllers
* request/response contracts
* dependency injection setup
* middleware
* Swagger/OpenAPI
* HTTP status code mapping

The API layer is kept thin. It translates HTTP requests into application commands and queries.

---

## Database integrity

FleetOps uses PostgreSQL constraints for important scheduling rules.

The `assignments` table includes:

* foreign key to `drivers`
* foreign key to `vehicles`
* check constraint for valid time range
* exclusion constraint preventing driver overlap
* exclusion constraint preventing vehicle overlap

PostgreSQL `EXCLUDE` constraints are used to prevent overlapping time ranges.

This is intentional: overlap prevention is too important to rely only on application-side checks.

---

## Error response format

FleetOps returns structured error responses.

Example validation response:

```json
{
  "code": "validation_error",
  "message": "One or more validation errors occurred.",
  "details": {
    "DriverId": [
      {
        "errorCode": "Assignment.DriverId.NotFound",
        "message": "Driver does not exist."
      }
    ]
  }
}
```

The API uses stable error codes so clients and tests can assert behavior without depending only on human-readable messages.

---

## Technology stack

* C#
* .NET 8
* ASP.NET Core
* EF Core
* PostgreSQL
* Docker Compose
* FluentValidation
* xUnit
* Shouldly
* GitHub Actions

---

## Running locally

### Prerequisites

You need:

* .NET 8 SDK
* Docker
* Git

Optional:

* GitHub CLI
* PostgreSQL client tools

---

### 1. Clone the repository

```bash
git clone https://github.com/MikyGus/FleetOps.git
cd FleetOps
```

---

### 2. Restore tools and dependencies

```bash
dotnet tool restore
dotnet restore FleetOps.sln
```

---

### 3. Configure environment variables

Copy the example environment file:

```bash
cp .env.example .env
```

Edit `.env` if needed.

> If you change `POSTGRES_PASSWORD`, make sure the API connection string uses the same password.
> The committed development connection string uses `dev_password_change_me`.

Example:

```env
POSTGRES_DB=fleetops
POSTGRES_USER=fleetops
POSTGRES_PASSWORD=dev_password_change_me
POSTGRES_PORT=5432
```

#### Check existing secrets

If you have previously worked on FleetOps on the same machine, user-secrets may override the committed development connection string.

```bash
dotnet user-secrets list --project ./src/FleetOps.Api
```

To use the README defaults, remove the local override:

```bash
dotnet user-secrets remove "ConnectionStrings:Postgres" --project ./src/FleetOps.Api
```

---

### 4. Start PostgreSQL

```bash
docker compose up -d
```

Check that the container is running:

```bash
docker compose ps
```

---

### 5. Apply database migrations

```bash
dotnet ef database update \
  --project ./src/FleetOps.Infrastructure \
  --startup-project ./src/FleetOps.Api
```

---

### 6. Run the API

```bash
dotnet run --project ./src/FleetOps.Api
```

Swagger is available at:

```text
http://localhost:5144/swagger
```

depending on the local launch profile and port.

Health endpoints:

```text
http://localhost:5144/health/live
http://localhost:5144/health/ready
```

---

## Running tests

### Setup database for integration tests

Integration tests use a real PostgreSQL database.

The recommended local setup is to create a separate test database:

```text
fleetops_test
```

This keeps test data separate from manual development data.

Create the test database:

```bash
docker exec -it fleetops-postgres psql -U fleetops -d postgres -c "CREATE DATABASE fleetops_test;"
```

Apply migrations to the test database:

```bash
dotnet ef database update \
  --project ./src/FleetOps.Infrastructure \
  --startup-project ./src/FleetOps.Api \
  --connection "Host=localhost;Port=5432;Database=fleetops_test;Username=fleetops;Password=dev_password_change_me"
```

Copy the integration test settings file:

```bash
cp ./tests/FleetOps.Tests.Integration/appsettings.Testing.example.json ./tests/FleetOps.Tests.Integration/appsettings.Testing.json 
```

Edit `appsettings.Testing.json` if needed.

Example:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=fleetops_test;Username=fleetops;Password=dev_password_change_me"
  }
}
```

### Run the tests

Run all tests:

```bash
dotnet test FleetOps.sln
```

Only run unit tests:

```bash
dotnet test ./tests/FleetOps.Tests.Unit/FleetOps.Tests.Unit.csproj
```

Or, only run integration tests:

```bash
dotnet test ./tests/FleetOps.Tests.Integration/FleetOps.Tests.Integration.csproj
```

The test suite includes:

* unit tests
* integration tests
* API behavior tests
* validation tests
* database-backed integration tests

---

## CI

The repository uses GitHub Actions to run CI.

The CI workflow:

* restores dependencies
* checks formatting
* builds the solution
* starts PostgreSQL
* applies EF Core migrations
* runs unit and integration tests

This ensures pull requests are verified before merging.

---

## Example API usage

### Create driver

```http
POST /drivers
Content-Type: application/json

{
  "name": "Mikael Gustafsson"
}
```

Expected result:

```http
201 Created
```

---

### Create vehicle

```http
POST /vehicles
Content-Type: application/json

{
  "registrationNumber": "ABC123"
}
```

Expected result:

```http
201 Created
```

---

### Create assignment

The `driverId` and `vehicleId` values must refer to existing records created through the driver and vehicle endpoints.

Replace the example IDs below with the `id` values returned from `POST /drivers` and `POST /vehicles`.

```http
POST /assignments
Content-Type: application/json

{
  "driverId": "11111111-1111-1111-1111-111111111111",
  "vehicleId": "22222222-2222-2222-2222-222222222222",
  "startUtc": "2026-03-05T10:00:00Z",
  "endUtc": "2026-03-05T12:00:00Z"
}
```

Expected result:

```http
201 Created
```

---

### List assignments

```http
GET /assignments
```

With filters:

```http
GET /assignments?driverId=11111111-1111-1111-1111-111111111111
```

```http
GET /assignments?vehicleId=22222222-2222-2222-2222-222222222222
```

```http
GET /assignments?fromUtc=2026-03-05T00:00:00Z&toUtc=2026-03-06T00:00:00Z
```

---

## What this project demonstrates

FleetOps demonstrates:

* backend API design in ASP.NET Core
* Clean Architecture layering
* PostgreSQL constraint-based data integrity
* EF Core configuration and migrations
* structured API error handling
* command/query separation
* validation with FluentValidation
* unit testing
* integration testing against a real database
* CI with GitHub Actions

The project is intentionally focused on backend correctness rather than UI features.

---

## Current status

FleetOps is a portfolio backend project under active development.

Implemented:

* Drivers
* Vehicles
* Assignments
* PostgreSQL constraints
* structured error responses
* unit tests
* integration tests
* CI pipeline

Planned future improvements:

* authentication and authorization
* richer driver qualification rules
* vehicle requirement validation
* update/deactivate endpoints
* sample data scripts
* additional documentation and diagrams

## Troubleshooting

### Migration fails with an authentication error

Check whether user-secrets override the committed development connection string:

```bash
dotnet user-secrets list --project ./src/FleetOps.Api
```

Remove the override if you want to use the README defaults:

```bash
dotnet user-secrets remove "ConnectionStrings:Postgres" --project ./src/FleetOps.Api
```

or as an alternative you can set it to the default password:

```bash
dotnet user-secrets set \
  "ConnectionStrings:Postgres" \
  "Host=localhost;Port=5432;Database=fleetops;Username=fleetops;Password=dev_password_change_me" \
  --project ./src/FleetOps.Api
```

## Remove FleetOps

If you want to remove the local FleetOps environment completely, run the following commands from the FleetOps repository root.

**WARNING**: This is destructive. It removes the FleetOps PostgreSQL container, database volume, optionally the PostgreSQL Docker image, and the local FleetOps source folder.

Make sure you are in the FleetOps repository root.

```bash
cd FleetOps
```

### Stop and remove Docker container + database volume

```bash
docker compose down -v
```

This removes:

* fleetops-postgres container
* fleetops_default network
* fleetops_fleetops_pgdata volume

Verify:

```bash
docker ps -a
docker volume ls | grep fleetops
```

You should not see `fleetops-postgres` or `fleetops_fleetops_pgdata`.

### Remove docker image (optional)

If you have no need for the `postgres:16` image you can remove it.

```bash
docker rmi postgres:16
```

### Remove FleetOps-files

You can now safely remove the entire `FleetOps` folder.

Move one folder up to the parent folder of `FleetOps` in order to remove the entire `FleetOps`-folder.

```bash
cd ..
rm -rf FleetOps
```
