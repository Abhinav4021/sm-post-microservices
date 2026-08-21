---

# 🎯 Social Media Post Microservices

### CQRS • Event Sourcing • Event-Driven Architecture • Apache Kafka • MongoDB • SQL Server

An event-driven Social Media Post backend built with C#, ASP.NET Core, CQRS, Event Sourcing, Apache Kafka, MongoDB, SQL Server, Entity Framework Core, and the Mediator/Dispatcher pattern.

> 🚧 Project status: learning/reference implementation — evolving incrementally. Not production-ready.

---

## 📑 Table of Contents

- [🎯 Project Overview](#-project-overview)
- [🏗️ Architecture](#️-architecture)
- [🔄 End-to-End Flow](#-end-to-end-flow)
- [🧠 CQRS](#-cqrs)
- [📜 Event Sourcing](#-event-sourcing)
- [📡 Event-Driven Communication](#-event-driven-communication)
- [🎯 Command Side](#-command-side)
- [🔎 Query Side](#-query-side)
- [🧩 Mediator / Dispatcher Pattern](#-mediator--dispatcher-pattern)
- [📦 Domain Events](#-domain-events)
- [🗂️ Project Structure](#️-project-structure)
- [🛠️ Technology Stack](#️-technology-stack)
- [⚙️ Prerequisites](#️-prerequisites)
- [🐳 Infrastructure Setup](#-infrastructure-setup)
- [🔧 Configuration](#-configuration)
- [▶️ Running the Application](#️-running-the-application)
- [📘 Swagger](#-swagger)
- [📡 API Reference](#-api-reference)
- [⏱️ Eventual Consistency](#️-eventual-consistency)
- [🔐 Concurrency Handling](#-concurrency-handling)
- [⚠️ Failure Scenarios](#-failure-scenarios)
- [🧠 Key Design Decisions](#-key-design-decisions)
- [🎨 Architecture Assets](#-architecture-assets)
- [🚧 Current Limitations](#-current-limitations)
- [🛣️ Roadmap](#️-roadmap)
- [🎓 Learning Objectives](#-learning-objectives)
- [🤝 Contributing](#-contributing)

---

# 🎯 Project Overview

This repository demonstrates an event-driven Social Media Post backend using a CQRS + Event Sourcing architecture.

Instead of a single CRUD model, responsibilities are split:

| Component | Responsibility |
|---|---|
| `Post.Cmd` | Write/command side — handles commands and event persistence |
| `Post.Query` | Read/query side — exposes read-optimized models |
| `Post.Common` | Shared DTOs and domain event contracts |
| `CQRS.Core` | Reusable CQRS/Event Sourcing primitives |
| MongoDB | Event store for the command side |
| Apache Kafka | Asynchronous event transport |
| SQL Server | Read model database populated by consumers |

## 🏗️ Architecture

High-level flow:

```text
Client => Post.Cmd (command path) => MongoDB (event store) => Kafka => Post.Query (consumers) => SQL Server (read model)
```

The repository contains diagram assets under Assets/ (editable Draw.io sources and exported PNGs).

### Responsibilities

Command side (Post.Cmd): HTTP requests map to Commands → Dispatcher → Handler → Aggregate → Persist events to MongoDB → Produce events to Kafka.

Query side (Post.Query): Kafka Consumer → Event Handler → Update SQL Server read model → Query endpoints serve read-optimized DTOs.

---

## 🔄 End-to-End Flow (example: Create Post)

1. Client issues HTTP POST /api/v1/NewPost
2. Post.Cmd.Api generates a new PostId and dispatches NewPostCommand
3. Command handler validates and applies to PostAggregate
4. Aggregate raises PostCreatedEvent and the event stream is appended to MongoDB
5. Kafka producer publishes the domain event to a topic
6. Post.Query consumer reads the event and projects it into the SQL Server read model
7. Query API (Post.Query.Api) exposes the data to clients

---

## 🧠 CQRS

Commands represent intentions to change state (e.g., NewPostCommand, LikePostCommand, DeletePostCommand, AddCommentCommand).

Queries request data (e.g., FindAllPostsQuery, FindPostByIdQuery, FindPostsByAuthorQuery).

Dispatchers decouple controllers from handlers (ICommandDispatcher, IQueryDispatcher).

---

## 📜 Event Sourcing

The command side persists events (PostCreatedEvent, MessageUpdatedEvent, PostLikedEvent, CommentAddedEvent, etc.). The current aggregate state can be rebuilt by replaying its event stream.

---

## 📡 Event-Driven Communication

Apache Kafka is used as the asynchronous transport between the command and query sides. Producers publish domain events to topics; consumers project events into the read model.

---

## 🎯 Command Side (SM-Post/Post.Cmd)

Structure:

- Post.Cmd.Api — controllers, DTOs, command models, dispatching, Swagger
- Post.Cmd.Domain — PostAggregate, business rules, domain events
- Post.Cmd.Infrastructure — MongoDB event store implementation, producers, repositories, dispatchers, handlers

Controllers dispatch commands through ICommandDispatcher. Registered command handlers include:
- NewPostCommand
- LikePostCommand
- DeletePostCommand
- EditMessageCommand
- AddCommentCommand
- EditCommentCommand
- RemoveCommentCommand

---

## 🔎 Query Side (SM-Post/Post.Query)

Structure:

- Post.Query.Api — query controllers, DTOs, Swagger
- Post.Query.Domain — read-side entities optimized for queries
- Post.Query.Infrastructure — Kafka consumers, data access (EF Core), repositories, hosted background services

The query API exposes PostLookUp endpoints and runs a hosted Kafka consumer to update the read model asynchronously.

---

## 🧩 Mediator / Dispatcher Pattern

Controllers use dispatcher abstractions to avoid direct coupling to handlers:

Controller → ICommandDispatcher / IQueryDispatcher → Handler → Domain / Repository

This keeps API concerns separated from domain/application logic.

---

## 📦 Domain Events

Shared events live in SM-Post/Post.Common/Events/ and include:
- PostCreatedEvent
- PostLikedEvent
- PostRemovedEvent
- MessageUpdatedEvent
- CommentAddedEvent
- CommentUpdatedEvent
- CommentRemovedEvent

These events are the inputs to read-side projections.

---

## 🗂️ Project Structure

Repository root highlights:

- Assets/ — architecture diagrams, docker-compose, setup notes
- CQRS-ES/CQRS.Core/ — reusable CQRS & ES primitives (Commands, Queries, Events, Aggregates, Event Store, Dispatchers)
- SM-Post/
  - Post.Common/
  - Post.Cmd/
  - Post.Query/
- SM-Post.sln

See [RepositoryStructure&FolderHierarchy.txt](RepositoryStructure&FolderHierarchy.txt) for the detailed layout and the Assets/ folder for diagrams.

---

## 🛠️ Technology Stack

- Language: C# (.NET 10 / net10.0)
- Framework: ASP.NET Core
- Architecture: Microservices, CQRS, Event Sourcing
- Messaging: Apache Kafka
- Event store: MongoDB
- Read DB: Microsoft SQL Server (Entity Framework Core)
- API docs: Swagger / OpenAPI
- Containerization: Docker, Docker Compose
- CI: GitHub Actions (workflows present)

---

## ⚙️ Prerequisites

Required:
- .NET 10 SDK
- Docker Desktop
- Docker Compose
- Git

Recommended:
- Visual Studio / VS Code / Rider
- Postman
- MongoDB Compass
- SQL Server client (SSMS, Azure Data Studio, etc.)

Note: Review each project's appsettings.json for connection strings before running.

---

## 🐳 Infrastructure Setup (local dev)

Expected local services:
- MongoDB (localhost:27017) — event store
- Kafka (localhost:9092) + Zookeeper (localhost:2181)
- SQL Server (localhost:1433) — read model

Quick steps (examples):

1. Create Docker network

    docker network create --attachable -d bridge mydockernetwork

2. Start Kafka + Zookeeper (provided docker-compose in Assets/)

    docker compose -f "Assets/docker-compose (1).yml" up -d

3. Start MongoDB

    docker run -it -d --name mongo-container -p 27017:27017 --network mydockernetwork --restart always -v mongodb_data_container:/data/db mongo:latest

4. Start SQL Server (example)

    docker run -d --name sql-container --network mydockernetwork --restart always -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YOUR_STRONG_PASSWORD>" -e "MSSQL_PID=Express" -p 1433:1433 mcr.microsoft.com/mssql/server:2017-latest-ubuntu

⚠️ Never commit real credentials to source control.

---

## 🔧 Configuration

- Command API configures MongoDbConfig, ProducerConfig, and related settings
- Query API configures ConsumerConfig and SQL Server connection string

Review appsettings.json files in each API project before running.

---

## ▶️ Running the Application

From repository root:

1. Restore dependencies

    dotnet restore

2. Build solution

    dotnet build SM-Post/SM-Post.sln

3. Start infrastructure (MongoDB, SQL Server, Kafka, Zookeeper)

4. Start Command API

    dotnet run --project SM-Post/Post.Cmd/Post.Cmd.Api

5. Start Query API (in another terminal)

    dotnet run --project SM-Post/Post.Query/Post.Query.Api

The Query API runs a hosted Kafka consumer as a background service to process events.

---

## 📘 Swagger

Both APIs expose Swagger UI in Development environment. Local URLs depend on the ASP.NET Core launch configuration for each project.

---

## 📡 API Reference

Base route for controllers:

    api/v1/[controller]

Command API examples:
- POST /api/v1/NewPost — create a post (server generates PostId)
- PUT /api/v1/LikePost/{id} — like a post
- DELETE /api/v1/DeletePost/{id} — remove post
- Endpoints for AddComment, EditComment, RemoveComment, EditMessage

Query API (PostLookUp):
- GET /api/v1/PostLookUp — get all posts
- GET /api/v1/PostLookUp/byId/{postId} — get post by id
- GET /api/v1/PostLookUp/byAuthor/{author} — posts by author
- GET /api/v1/PostLookUp/withComments — posts with comments
- GET /api/v1/PostLookUp/withLikes/{numberOfLikes} — posts with at least N likes

---

## ⏱️ Eventual Consistency

Writes and reads are decoupled. There may be a small delay between a successful command and the read model update.

---

## 🔐 Concurrency Handling

Optimistic concurrency is used on event streams. The CQRS core contains ConcurrencyException and related infrastructure to detect conflicting updates when expected versions diverge.

---

## ⚠️ Failure Scenarios & Operational Concerns

- Kafka unavailability: affects event delivery; ensure reliable publication and retries
- Query service unavailability: read model projections stop until consumers resume
- Duplicate event delivery: projection handlers should be idempotent
- Configure retry policies and dead-letter topics for resilience

---

## 🧠 Key Design Decisions

- CQRS to separate read/write responsibilities and enable independent optimization and scaling
- Event Sourcing to persist the history of state transitions
- MongoDB as an event store (append-only event streams)
- SQL Server for read-optimized relational queries
- Kafka for asynchronous event transport and loose coupling
- Dispatcher/Mediator pattern to decouple controllers from handlers and keep domain logic in aggregates

---

## 📦 CQRS.Core

The CQRS-ES/CQRS.Core project provides reusable abstractions for Commands, Queries, Events, Aggregates, Event Store, Dispatchers, Producers/Consumers, and exceptions used across the solution.

---

## 🎨 Architecture Assets

Editable diagrams and supporting setup notes live in the Assets/ folder:
- Architecture Overview.drawio / Architecture Overview.png
- Kafka Architecture.drawio
- Apache Kafka Producer.drawio
- Apache Kafka Consumer (.NET).drawio
- Mediator Pattern.drawio
- docker-compose (1).yml

---

## 🚧 Current Limitations

This repository is a learning/reference implementation. Areas to improve before production:

- Automated unit tests
- Integration tests / contract testing
- Kafka retry policies and dead-letter topics
- Idempotent event processing
- Improved error handling
- Authentication & Authorization
- Secret management
- Health checks, structured logging, distributed tracing (OpenTelemetry), and metrics
- Database migrations
- Containerization of application services
- API Gateway, rate limiting, resilience patterns (circuit breakers, bulkheads)
- Production CI/CD and Kubernetes deployment

---

## 🛣️ Roadmap

Phase 1 — Documentation
- Consolidate README, diagrams, and setup docs

Phase 2 — Code Quality
- Improve domain model, validation, exception handling
- Add unit and integration tests

Phase 3 — Messaging & Reliability
- Retry strategies, dead-letter topics, idempotent consumers, event versioning

Phase 4 — Production Readiness
- Auth, health checks, logging, tracing, metrics, API Gateway, rate limiting, Dockerized services, Kubernetes

---

## 🎓 Learning Objectives

This repository is primarily intended to demonstrate and explore:
- CQRS, Event Sourcing, Domain-Driven Design, Aggregate Roots, Domain Events
- Event-driven architecture using Apache Kafka
- MongoDB for event storage and SQL Server for the read model
- Entity Framework Core, Mediator pattern, Dependency Injection
- Optimistic concurrency and eventual consistency

---

## 🤝 Contributing

Guidelines:
- Create a feature branch: `git checkout -b feature/my-improvement`
- Keep changes focused and update documentation when architecture changes
- Respect command/query separation and keep domain rules inside domain models
- Add tests when introducing behavior changes
- Keep diagrams synchronized with implementation
- Use meaningful commit messages

Example workflow:

```bash
git checkout main
git pull origin main
git checkout -b feature/my-improvement
# make changes
git add .
git commit -m "Improve <feature>"
git push origin feature/my-improvement
```

---

## 📌 Repository

GitHub: https://github.com/Abhinav4021/sm-post-microservices

If this repo helped you learn CQRS, Event Sourcing, or event-driven .NET architecture, consider giving it a star.

---

*Last updated: 2026-08-21*
