# FleetOps

FleetOps is a backend system built with ASP.NET Core and PostgreSQL using Clean Architecture principles.

The project focuses on:

- Explicit domain modeling
- Strong data integrity
- Database-enforced overlap prevention
- Clear architectural boundaries
- Production-style API behavior

## Domain Concept

FleetOps manages:

- Drivers
- Vehicles
- Time-based assignments (who drove what, when)

Assignments enforce strict non-overlapping time intervals at both application and database level.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- Docker

## Status

Phase 0 - Repository initialized  
Implementation roadmap available.

---

This project is built as part of a structured backend portfolio.
