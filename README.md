# Social Media Post Microservices (CQRS & Event Sourcing)

An event-driven microservices application built with **.NET Core**, **CQRS (Command Query Responsibility Segregation)**, and **Event Sourcing**.

## 🏗️ Solution Overview
* **Post.Cmd**: Command API for write operations (Event Store in MongoDB, publishes to Kafka).
* **Post.Query**: Query API for read operations (Consumes Kafka events, stores read models in MSSQL).
* **Post.Common**: Shared events, contracts, and DTOs.

# Social Media Post Microservices (CQRS & Event Sourcing)

An event-driven microservices application built with **.NET Core**, **CQRS (Command Query Responsibility Segregation)**, and **Event Sourcing** principles.

---

## 🏗️ Architecture Overview

The solution separates read and write operations into distinct services communicating asynchronously via Apache Kafka.

```text
┌────────────────┐      Commands       ┌──────────────────┐
│  Post.Cmd.Api  │ ──────────────────> │  Event Store     │
└───────┬────────┘                     │  (MongoDB)       │
        │                              └────────┬─────────┘
        │ Publishes Events                      │
        ▼                                       │
┌────────────────┐                              │
│  Apache Kafka  │ <────────────────────────────┘
└───────┬────────┘
        │ Consumes Events
        ▼
┌────────────────┐      Persists       ┌──────────────────┐
│ Post.Query.Api │ ──────────────────> │ Read Database    │
└────────────────┘                     │ (MSSQL / Azure)  │
                                       └──────────────────┘

## 🚀 Quick Start
```bash
dotnet build SM-Post.sln
ef
eof
ls -la README.md
